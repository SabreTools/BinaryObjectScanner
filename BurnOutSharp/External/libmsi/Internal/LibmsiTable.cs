/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002-2005 Mike McCormack for CodeWeavers
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301, USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGSF.Input;
using LibGSF.Output;
using LibMSI.Views;
using static LibMSI.LibmsiTypes;
using static LibMSI.Internal.MsiPriv;
using static LibMSI.Internal.StringTable;

namespace LibMSI.Internal
{
    internal class LibmsiColumnInfo
    {
        public string TableName { get; set; }

        public int Number { get; set; }

        public string ColName { get; set; }

        public int Type { get; set; }

        public int Offset { get; set; }

        public int RefCount { get; set; }

        public bool Temporary { get; set; }

        public LibmsiColumnHashEntry[] HashTable { get; set; }
    }

    internal class TRANSFORMDATA
    {
        public string Name { get; set; }
    }

    internal class LibmsiTable
    {
        #region Constants

        public const int LibmsiTable_HASH_TABLE_SIZE = 37;

        public const string szDot = ".";

        // Information for default tables
        public const string szTables = "_Tables";
        public const string szTable = "Table";
        public const string szColumns = "_Columns";
        public const string szNumber = "Number";
        public const string szType = "Type";

        public static readonly LibmsiColumnInfo[] _Columns_cols = new LibmsiColumnInfo[]
        {
            new LibmsiColumnInfo
            {
                TableName = szColumns,
                Number = 1,
                ColName = szTable,
                Type = MSITYPE_VALID | MSITYPE_STRING | MSITYPE_KEY | 64,
                Offset = 0,
                RefCount = 0,
                Temporary = false,
                HashTable = null
            },
            new LibmsiColumnInfo
            {
                TableName = szColumns,
                Number = 2,
                ColName = szNumber,
                Type = MSITYPE_VALID | MSITYPE_KEY | 2,
                Offset = 2,
                RefCount = 0,
                Temporary = false,
                HashTable = null
            },
            new LibmsiColumnInfo
            {
                TableName = szColumns,
                Number = 3,
                ColName = szName,
                Type = MSITYPE_VALID | MSITYPE_STRING | 64,
                Offset = 4,
                RefCount = 0,
                Temporary = false,
                HashTable = null
            },
            new LibmsiColumnInfo
            {
                TableName = szColumns,
                Number = 4,
                ColName = szType,
                Type = MSITYPE_VALID | 2,
                Offset = 6,
                RefCount = 0,
                Temporary = false,
                HashTable = null
            },
        };

        public static readonly LibmsiColumnInfo[] _Tables_cols = new LibmsiColumnInfo[]
        {
            new LibmsiColumnInfo
            {
                TableName = szTables,
                Number = 1,
                ColName = szName,
                Type = MSITYPE_VALID | MSITYPE_STRING | MSITYPE_KEY | 64,
                Offset = 0,
                RefCount = 0,
                Temporary = false,
                HashTable = null
            },
        };

        private const int MAX_STREAM_NAME = 0x1f;

        #endregion

        #region Properties

        public byte[][] Data { get; set; }

        public bool[] DataPersistent { get; set; }

        public int RowCount { get; set; }

        public LibmsiColumnInfo[] ColInfo { get; set; }

        public int ColCount { get; set; }

        public LibmsiCondition Persistent { get; set; }

        public int RefCount { get; set; }

        public string Name { get; set; } = string.Empty;

        #endregion

        #region Functions

        public static int BytesPerColumn(LibmsiColumnInfo col, int bytes_per_strref)
        {
            if (MSITYPE_IS_BINARY(col.Type))
                return 2;

            if ((col.Type & MSITYPE_STRING) != 0)
                return bytes_per_strref;

            if ((col.Type & 0xff) <= 2)
                return 2;

            if ((col.Type & 0xff) != 4)
                Console.Error.WriteLine("Invalid column size!");

            return 4;
        }

        public static string EncodeStreamName(bool bTable, string input)
        {
            int count = MAX_STREAM_NAME;
            int next;

            if (!bTable)
                count = input.Length + 2;

            byte[] output = new byte[count * 3];
            int p = 0; // output[0]

            if (bTable)
            {
                // UTF-8 encoding of 0x4840.
                output[p++] = 0xe4;
                output[p++] = 0xa1;
                output[p++] = 0x80;
                count--;
            }

            int inputPtr = 0; // input[0]
            while (count-- != 0)
            {
                int ch = inputPtr < input.Length ? input[inputPtr++] : 0;
                if (ch == 0)
                {
                    output[p] = (byte)ch;
                    return Encoding.UTF8.GetString(output);
                }

                if ((ch < 0x80) && (Utf2Mime(ch) >= 0))
                {
                    ch = Utf2Mime(ch);

                    if (inputPtr < input.Length)
                    {
                        next = input[inputPtr];
                        if (next != 0 && (next < 0x80))
                            next = Utf2Mime(next);
                        else
                            next = -1;
                    }
                    else
                    {
                        next = -1;
                    }

                    if (next == -1)
                    {
                        // UTF-8 encoding of 0x4800..0x483f.
                        output[p++] = 0xe4;
                        output[p++] = 0xa0;
                        output[p++] = (byte)(0x80 | ch);
                    }
                    else
                    {
                        // UTF-8 encoding of 0x3800..0x47ff.
                        output[p++] = (byte)(0xe3 + (next >> 5));
                        output[p++] = (byte)(0xa0 ^ next);
                        output[p++] = (byte)(0x80 | ch);
                        inputPtr++;
                    }
                }
                else
                {
                    output[p++] = (byte)ch;
                }
            }

            Console.Error.WriteLine($"Failed to encode stream name ({input})");
            return null;
        }

        public static string DecodeStreamName(string input)
        {
            if (input == null)
                return null;

            int count = 0;
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            int p = 0; // inputBytes[0]

            byte[] output = new byte[inputBytes.Length + 1];
            int q = 0; // output[0]
            while (p < inputBytes.Length && inputBytes[p] != 0)
            {
                int ch = inputBytes[p];
                if ((ch == 0xe3 && inputBytes[p + 1] >= 0xa0) || (ch == 0xe4 && inputBytes[p + 1] < 0xa0))
                {
                    // UTF-8 encoding of 0x3800..0x47ff. 
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 2] & 0x7f);
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 1] ^ 0xa0);
                    p += 3;
                    count += 2;
                    continue;
                }

                if (ch == 0xe4 && inputBytes[p + 1] == 0xa0)
                {
                    // UTF-8 encoding of 0x4800..0x483f.
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 2] & 0x7f);
                    p += 3;
                    count++;
                    continue;
                }

                output[q++] = inputBytes[p++];
                if (ch >= 0xc1)
                    output[q++] = inputBytes[p++];
                if (ch >= 0xe0)
                    output[q++] = inputBytes[p++];
                if (ch >= 0xf0)
                    output[q++] = inputBytes[p++];

                count++;
            }

            output[q] = 0;
            return Encoding.ASCII.GetString(output);
        }

        public static void EnumStreamNames(GsfInfile stg)
        {
            int n = stg.NumChildren();
            for (int i = 0; i < n; i++)
            {
                string stname = stg.NameByIndex(i);
                if (stname == null)
                    continue;

                string name = DecodeStreamName(stname);
                Console.WriteLine($"Stream {n} . {stname} {name}");
            }
        }

        public static LibmsiResult ReadStreamData(GsfInfile stg, string stname, out byte[] pdata, out int psz)
        {
            pdata = null; psz = 0;
            if (stg == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            string encname = EncodeStreamName(true, stname).TrimEnd('\0');

            Exception err = null;
            GsfInput stm = stg.ChildByName(encname, ref err);
            if (stm == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if ((stm.Size >> 32) != 0)
            {
                Console.Error.WriteLine("Too big!");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            int sz = (int)stm.Size;
            byte[] data;
            if (sz == 0)
            {
                data = null;
            }
            else
            {
                data = new byte[sz];
                if (stm.Read(sz, data) == null)
                {
                    Console.Error.WriteLine("Read stream failed");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }
            }

            pdata = data;
            psz = sz;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move to LibmsiDatabase
        public static LibmsiResult WriteStreamData(LibmsiDatabase db, string stname, byte[] data, int sz)
        {
            if (db.Outfile == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            string encname = EncodeStreamName(true, stname).TrimEnd('\0');
            GsfOutput stm = db.Outfile.NewChild(encname, false);

            if (stm == null)
            {
                Console.Error.WriteLine("Open stream failed");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            if (!stm.Write(sz, data))
            {
                Console.Error.WriteLine("Failed to Write");
                stm.Close();
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            stm.Close();
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move to LibmsiDatabase
        public static int GetRowSize(LibmsiColumnInfo[] cols, int count, int bytes_per_strref)
        {
            if (count == 0)
                return 0;

            if (bytes_per_strref != LONG_STR_BYTES)
            {
                int size = 0;
                for (int i = 0; i < count; i++)
                {
                    size += BytesPerColumn(cols[i], bytes_per_strref);
                }

                return size;
            }

            LibmsiColumnInfo last_col = cols[count - 1];
            return last_col.Offset + BytesPerColumn(last_col, bytes_per_strref);
        }

        // TODO: Move to LibmsiDatabase
        public static void FreeCachedTables(LibmsiDatabase db)
        {
            db.Tables.Clear();
        }

        // TODO: Move to LibmsiDatabase
        public static LibmsiResult OpenTable(LibmsiDatabase db, string name, bool encoded)
        {
            string decname = null;
            byte[] name8 = Encoding.UTF8.GetBytes(name);

            if (encoded)
            {
                //assert(name8[0] == 0xe4 && name8[1] == 0xa1 && name8[2] == 0x80);
                decname = DecodeStreamName(name.Substring(1));
            }

            LibmsiTable table = new LibmsiTable
            {
                Persistent = LibmsiCondition.LIBMSI_CONDITION_TRUE,
                Name = name,
            };

            if (name == szTables || name == szColumns)
                table.Persistent = LibmsiCondition.LIBMSI_CONDITION_NONE;

            db.Tables.AddLast(table);
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move to LibmsiDatabase
        public static LibmsiResult GetTable(LibmsiDatabase db, string name, out LibmsiTable table_ret)
        {
            table_ret = null;
            LibmsiResult r;

            // First, see if the table is cached
            LibmsiTable table = FindCachedTable(db, name);
            if (table == null)
            {
                // Nonexistent tables should be interpreted as empty tables
                r = OpenTable(db, name, false);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                table = FindCachedTable(db, name);
            }

            if (table.ColInfo != null)
            {
                table_ret = table;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            r = TableGetColumnInfo(db, name, out LibmsiColumnInfo[] col_info, out int col_count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            table.ColInfo = col_info;
            table.ColCount = col_count;

            r = ReadTableFromStorage(db, table, db.Infile);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            table_ret = table;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        public static int ReadTableInt(byte[][] data, int row, int col, int bytes)
        {
            int ret = 0;
            for (int i = 0; i < bytes; i++)
            {
                ret += data[row][col + i] << i * 8;
            }

            return ret;
        }

        // TODO: Move to LibmsiDatabase
        public static LibmsiResult CreateTable(LibmsiDatabase db, string name, column_info col_info, LibmsiCondition persistent)
        {
            StringPersistence string_persistence = (persistent != LibmsiCondition.LIBMSI_CONDITION_FALSE) ? StringPersistence.StringPersistent : StringPersistence.StringNonPersistent;
            int nField;
            LibmsiRecord rec = null;

            // Only add tables that don't exist already
            if (TableViewExists(db, name))
            {
                Console.Error.WriteLine($"Table {name} exists");
                return LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX;
            }

            LibmsiTable table = new LibmsiTable
            {
                RefCount = 1,
                RowCount = 0,
                Data = null,
                DataPersistent = null,
                ColInfo = null,
                ColCount = 0,
                Persistent = persistent,
                Name = name,
            };

            column_info col = col_info;
            for (; col != null; col = col.Next)
            {
                table.ColCount++;
            }

            table.ColInfo = new LibmsiColumnInfo[table.ColCount];

            int i = 0; col = col_info;
            for (; col != null; i++, col = col.Next)
            {
                int table_id = db.Strings.AddString(col.Table, -1, 1, string_persistence);
                int col_id = db.Strings.AddString(col.Column, -1, 1, string_persistence);

                table.ColInfo[i] = new LibmsiColumnInfo
                {
                    TableName = db.Strings.LookupId(table_id),
                    Number = i + 1,
                    ColName = db.Strings.LookupId(col_id),
                    Type = col.Type,
                    Offset = 0,
                    RefCount = 0,
                    HashTable = null,
                    Temporary = col.Temporary,
                };
            }

            CalcColumnOffsets(table.ColInfo, table.ColCount);

            LibmsiResult r = LibmsiTableView.Create(db, szTables, out LibmsiView tv);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = tv.Execute(null);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto err;

            rec = LibmsiRecord.Create(1);
            if (rec == null)
                goto err;

            r = rec.SetString(1, name) ? LibmsiResult.LIBMSI_RESULT_SUCCESS : LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto err;

            r = tv.InsertRow(rec, -1, persistent == LibmsiCondition.LIBMSI_CONDITION_FALSE);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto err;

            tv.Delete();
            tv = null;

            rec = null;

            if (persistent != LibmsiCondition.LIBMSI_CONDITION_FALSE)
            {
                // Add each column to the _Columns table
                r = LibmsiTableView.Create(db, szColumns, out tv);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                r = tv.Execute(null);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    goto err;

                rec = LibmsiRecord.Create(4);
                if (rec == null)
                    goto err;

                if (!rec.SetString(1, name))
                    goto err;

                // Need to set the table, column number, col name and type
                // for each column we enter in the table
                nField = 1;
                for (col = col_info; col != null; col = col.Next)
                {
                    if (!rec.SetInt(2, nField)
                        || !rec.SetString(3, col.Column)
                        || !rec.SetInt(4, col.Type))
                    {
                        goto err;
                    }

                    r = tv.InsertRow(rec, -1, false);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        goto err;

                    nField++;
                }

                if (col == null)
                    r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

        err:
            // FIXME: remove values from the string table on error
            if (tv != null)
                tv.Delete();

            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                db.Tables.AddLast(table);

            return r;
        }

        // TODO: Move to LibmsiDatabase
        public static void UpdateTableColumns(LibmsiDatabase db, string name)
        {
            LibmsiTable table = FindCachedTable(db, name);
            int old_count = table.ColCount;
            table.ColInfo = null;

            TableGetColumnInfo(db, name, out LibmsiColumnInfo[] col_info, out int col_count);
            table.ColInfo = col_info;
            table.ColCount = col_count;

            if (table.ColCount == 0)
                return;

            int size = GetRowSize(table.ColInfo, table.ColCount, LONG_STR_BYTES);
            int offset = table.ColInfo[table.ColCount - 1].Offset;

            for (int n = 0; n < table.RowCount; n++)
            {
                byte[] tempData = table.Data[n];
                Array.Resize(ref tempData, size);
                table.Data[n] = tempData;

                if (old_count < table.ColCount)
                {
                    for (int i = offset; i < size; i++)
                    {
                        table.Data[n][i] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Try to find the table name in the _Tables table
        /// </summary>
        // TODO: Move to LibmsiDatabase
        public static bool TableViewExists(LibmsiDatabase db, string name)
        {
            if (name == szTables || name == szColumns || name == szStreams || name == szStorages)
                return true;

            LibmsiResult r = db.Strings.IdFromStringUTF8(name, out int table_id);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return false;

            r = GetTable(db, szTables, out LibmsiTable table);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Console.Error.WriteLine($"Table {szTables} not available");
                return false;
            }

            for (int i = 0; i < table.RowCount; i++)
            {
                if (ReadTableInt(table.Data, i, 0, LONG_STR_BYTES) == table_id)
                    return true;
            }

            return false;
        }

        // TODO: Move to LibmsiDatabase
        public static LibmsiResult AddStream(LibmsiDatabase db, string name, GsfInput data)
        {
            LibmsiRecord rec = LibmsiRecord.Create(2);
            if (rec == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            if (!rec.SetString(1, name))
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            LibmsiResult r = rec.SetGsfInput(2, data);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            string insert = "INSERT INTO `_Streams`(`Name`, `Data`) VALUES (?, ?)";
            Exception err = null;
            LibmsiQuery query = LibmsiQuery.Create(db, insert, ref err);
            return query.Execute(rec);
        }

        // TODO: Move to LibmsiDatabase
        public static LibmsiResult DatabseCommitTables(LibmsiDatabase db, int bytes_per_strref)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;

            // Ensure the Tables stream is written.
            GetTable(db, szTables, out LibmsiTable t);

            foreach (LibmsiTable table in db.Tables)
            {
                r = GetTable(db, table.Name, out t);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine($"Failed to load table {table.Name} (r={r})");
                    return r;
                }

                r = SaveTable(db, table, bytes_per_strref);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine($"Failed to save table {table.Name} (r={r})");
                    return r;
                }
            }

            db.Tables.Clear();

            return r;
        }

        // TODO: Move to LibmsiDatabase
        public static LibmsiCondition IsTablePersistent(LibmsiDatabase db, string table)
        {
            if (table == null)
                return LibmsiCondition.LIBMSI_CONDITION_ERROR;

            LibmsiResult r = GetTable(db, table, out LibmsiTable t);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return LibmsiCondition.LIBMSI_CONDITION_NONE;

            return t.Persistent;
        }

        // TODO: Move to LibmsiTableView
        public static LibmsiResult FindRow(LibmsiTableView tv, LibmsiRecord rec, out int row, out int column)
        {
            row = 0; column = 0;
            int[] data = RecordToRow(tv, rec);
            if (data == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            for (int i = 0; i < tv.Table.RowCount; i++)
            {
                r = RowMatches(tv, i, data, out column);
                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    row = i;
                    break;
                }
            }

            return r;
        }

        /// <summary>
        /// Enumerate the table transforms in a transform storage and apply each one.
        /// </summary>
        // TODO: Move to LibmsiDatabase
        public static LibmsiResult ApplyTransform(LibmsiDatabase db, GsfInfile stg)
        {
            TRANSFORMDATA tables = null, columns = null;
            LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            StringTable strings = LoadStringTable(stg, out int bytes_per_strref);
            if (strings == null)
                goto end;

            int n = stg.NumChildren();

            LinkedList<TRANSFORMDATA> transforms = new LinkedList<TRANSFORMDATA>();

            for (int i = 0; i < n; i++)
            {
                string encname = stg.NameByIndex(i);
                byte[] encnameBytes = Encoding.UTF8.GetBytes(encname);
                if (encnameBytes[0] != 0xe4 || encnameBytes[1] != 0xa1 || encnameBytes[2] != 0x80)
                    continue;

                string name = DecodeStreamName(encname);
                if (name.Substring(3) == szStringPool || name.Substring(3) == szStringData)
                    continue;

                TRANSFORMDATA transform = new TRANSFORMDATA { Name = $"{name}\0" };
                transforms.AddFirst(transform);

                if (transform.Name == szTables)
                    tables = transform;
                else if (transform.Name == szColumns)
                    columns = transform;

                // Load the table
                LibmsiResult r = LibmsiTableView.Create(db, transform.Name, out LibmsiView view);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    continue;

                LibmsiTableView tv = (view as LibmsiTableView);
                if (tv == null)
                    continue;

                r = tv.Execute(null);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    tv.Delete();
                    continue;
                }

                tv.Delete();
            }

            // Apply _Tables and _Columns transforms first so that
            // the table metadata is correct, and empty tables exist.
            ret = LoadTransform(db, stg, strings, tables, bytes_per_strref);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS && ret != LibmsiResult.LIBMSI_RESULT_INVALID_TABLE)
                goto end;

            ret = LoadTransform(db, stg, strings, columns, bytes_per_strref);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS && ret != LibmsiResult.LIBMSI_RESULT_INVALID_TABLE)
                goto end;

            ret = LibmsiResult.LIBMSI_RESULT_SUCCESS;

            foreach (TRANSFORMDATA transform in transforms)
            {
                if (transform.Name == szColumns && transform.Name == szTables && ret == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    ret = LoadTransform(db, stg, strings, transform, bytes_per_strref);
            }

            transforms.Clear();

            if (ret == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                db.AppendStorageToDb(stg);

            end:
            if (strings != null)
                strings.Destroy();

            return ret;
        }

        #endregion

        #region Utilities

        private static int Utf2Mime(int x)
        {
            if ((x >= '0') && (x <= '9'))
                return x - '0';
            if ((x >= 'A') && (x <= 'Z'))
                return x - 'A' + 10;
            if ((x >= 'a') && (x <= 'z'))
                return x - 'a' + 10 + 26;
            if (x == '.')
                return 10 + 26 + 26;
            if (x == '_')
                return 10 + 26 + 26 + 1;
            return -1;
        }

        private static int Mime2Utf(int x)
        {
            if (x < 10)
                return x + '0';
            if (x < (10 + 26))
                return x - 10 + 'A';
            if (x < (10 + 26 + 26))
                return x - 10 - 26 + 'a';
            if (x == (10 + 26 + 26))
                return '.';
            return '_';
        }

        /// <summary>
        /// Add this table to the list of cached tables in the database
        /// </summary>
        // TODO: Move to LibmsiDatabase
        private static LibmsiResult ReadTableFromStorage(LibmsiDatabase db, LibmsiTable t, GsfInfile stg)
        {
            int row_size = GetRowSize(t.ColInfo, t.ColCount, db.BytesPerStrref);
            int row_size_mem = GetRowSize(t.ColInfo, t.ColCount, LONG_STR_BYTES);

            // If we can't read the table, just assume that it's empty 
            ReadStreamData(stg, t.Name, out byte[] rawdata, out int rawsize);
            if (rawdata == null)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            if ((rawsize % row_size) != 0)
            {
                Console.Error.WriteLine($"Table size is invalid {rawsize}/{row_size}");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            t.RowCount = rawsize / row_size;
            t.Data = new byte[t.RowCount][];
            t.DataPersistent = new bool[t.RowCount];

            // Transpose all the data
            for (int i = 0; i < t.RowCount; i++)
            {
                int ofs = 0, ofs_mem = 0;

                t.Data[i] = new byte[row_size_mem];
                t.DataPersistent[i] = true;

                for (int j = 0; j < t.ColCount; j++)
                {
                    int m = BytesPerColumn(t.ColInfo[j], LONG_STR_BYTES);
                    int n = BytesPerColumn(t.ColInfo[j], db.BytesPerStrref);

                    if (n != 2 && n != 3 && n != 4)
                    {
                        Console.Error.WriteLine("oops - unknown column width %d\n", n);
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                    }

                    if ((t.ColInfo[j].Type & MSITYPE_STRING) != 0 && n < m)
                    {
                        for (int k = 0; k < m; k++)
                        {
                            if (k < n)
                                t.Data[i][ofs_mem + k] = rawdata[ofs * t.RowCount + i * n + k];
                            else
                                t.Data[i][ofs_mem + k] = 0;
                        }
                    }
                    else
                    {
                        for (int k = 0; k < n; k++)
                        {
                            t.Data[i][ofs_mem + k] = rawdata[ofs * t.RowCount + i * n + k];
                        }
                    }

                    ofs_mem += m;
                    ofs += n;
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move this to the correct section
        // TODO: Move to LibmsiDatabase
        public static LibmsiTable FindCachedTable(LibmsiDatabase db, string name)
        {
            foreach (LibmsiTable t in db.Tables)
            {
                if (name == t.Name)
                    return t;
            }

            return null;
        }

        private static void CalcColumnOffsets(LibmsiColumnInfo[] colinfo, int count)
        {
            for (int i = 0; colinfo[i] != null && i < count; i++)
            {
                //assert(i + 1 == colinfo[i].number);
                if (i != 0)
                    colinfo[i].Offset = colinfo[i - 1].Offset + BytesPerColumn(colinfo[i - 1], LONG_STR_BYTES);
                else
                    colinfo[i].Offset = 0;
            }
        }

        // TODO: Move to LibmsiDatabase
        private static LibmsiResult GetDefaultTableColumns(string name, LibmsiColumnInfo[] colinfo, ref int sz)
        {
            LibmsiColumnInfo[] p;
            int n;

            if (name == szTables)
            {
                p = _Tables_cols;
                n = 1;
            }
            else if (name == szColumns)
            {
                p = _Columns_cols;
                n = 4;
            }
            else
            {
                sz = 0;
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            for (int i = 0; i < n; i++)
            {
                if (colinfo != null && i < sz)
                    colinfo[i] = p[i];

                if (colinfo != null && i >= sz)
                    break;
            }

            CalcColumnOffsets(colinfo, n);
            sz = n;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move to LibmsiDatabase
        private static LibmsiResult TableGetColumnInfo(LibmsiDatabase db, string name, out LibmsiColumnInfo[] pcols, out int pcount)
        {
            pcols = null; pcount = 0;

            // Get the number of columns in this table
            int column_count = 0;
            LibmsiResult r = GetTableColumns(db, name, null, ref column_count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            pcount = column_count;

            // if there's no columns, there's no table
            if (column_count == 0)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            LibmsiColumnInfo[] columns = new LibmsiColumnInfo[column_count];
            r = GetTableColumns(db, name, columns, ref column_count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            pcols = columns;
            return r;
        }

        // TODO: Move to LibmsiDatabase
        private static LibmsiResult GetTableColumns(LibmsiDatabase db, string szTableName, LibmsiColumnInfo[] colinfo, ref int sz)
        {
            int maxcount = sz;
            int n = 0;

            // First check if there is a default table with that name
            LibmsiResult r = GetDefaultTableColumns(szTableName, colinfo, ref sz);
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS && sz != 0)
                return r;

            r = GetTable(db, szColumns, out LibmsiTable table);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Console.Error.WriteLine("Couldn't load _Columns table");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            // Convert table and column names to IDs from the string table
            r = db.Strings.IdFromStringUTF8(szTableName, out int table_id);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Console.Error.WriteLine($"Couldn't find id for {szTableName}");
                return r;
            }

            // Note: _Columns table doesn't have non-persistent data

            // If maxcount is non-zero, assume it's exactly right for this table
            if (colinfo != null)
                Array.Clear(colinfo, 0, maxcount);

            int count = table.RowCount;
            for (int i = 0; i < count; i++)
            {
                if (ReadTableInt(table.Data, i, 0, LONG_STR_BYTES) != table_id)
                    continue;

                if (colinfo != null)
                {
                    int id = ReadTableInt(table.Data, i, table.ColInfo[2].Offset, LONG_STR_BYTES);
                    int col = ReadTableInt(table.Data, i, table.ColInfo[1].Offset, sizeof(ushort)) - (1 << 15);

                    // Check the column number is in range
                    if (col < 1 || col > maxcount)
                    {
                        Console.Error.WriteLine("column %d out of range (maxcount: %d)\n", col, maxcount);
                        continue;
                    }

                    // Check if this column was already set
                    if (colinfo[col - 1].Number != 0)
                    {
                        Console.Error.WriteLine($"Duplicate column {col}");
                        continue;
                    }

                    colinfo[col - 1].TableName = db.Strings.LookupId(table_id);
                    colinfo[col - 1].Number = col;
                    colinfo[col - 1].ColName = db.Strings.LookupId(id);
                    colinfo[col - 1].Type = ReadTableInt(table.Data, i, table.ColInfo[3].Offset, sizeof(ushort)) - (1 << 15);
                    colinfo[col - 1].Offset = 0;
                    colinfo[col - 1].RefCount = 0;
                    colinfo[col - 1].HashTable = null;
                }

                n++;
            }

            if (colinfo != null && n != maxcount)
            {
                Console.Error.WriteLine($"Missing column in table {szTableName}");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            CalcColumnOffsets(colinfo, n);
            sz = n;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move to LibmsiDatabase
        private static LibmsiResult SaveTable(LibmsiDatabase db, LibmsiTable t, int bytes_per_strref)
        {
            // Nothing to do for non-persistent tables
            if (t.Persistent == LibmsiCondition.LIBMSI_CONDITION_FALSE)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            // All tables are copied to the new file when committing, so
            // we can just skip them if they are empty.  However, always
            // save the Tables stream.
            if (t.RowCount == 0 && t.Name != szTables)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            int row_size = GetRowSize(t.ColInfo, t.ColCount, bytes_per_strref);
            int row_count = t.RowCount;
            for (int i = 0; i < t.RowCount; i++)
            {
                if (!t.DataPersistent[i])
                {
                    row_count = 1; // Yes, this is bizarre
                    break;
                }
            }

            int rawsize = row_count * row_size;
            byte[] rawdata = new byte[rawsize];

            rawsize = 0;
            for (int i = 0; i < t.RowCount; i++)
            {
                int ofs = 0, ofs_mem = 0;

                if (!t.DataPersistent[i])
                    break;

                for (int j = 0; j < t.ColCount; j++)
                {
                    int m = BytesPerColumn(t.ColInfo[j], LONG_STR_BYTES);
                    int n = BytesPerColumn(t.ColInfo[j], bytes_per_strref);

                    if (n != 2 && n != 3 && n != 4)
                    {
                        Console.Error.WriteLine($"Oops - unknown column width {n}");
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                    }

                    if ((t.ColInfo[j].Type & MSITYPE_STRING) != 0 && n < m)
                    {
                        int id = ReadTableInt(t.Data, i, ofs_mem, LONG_STR_BYTES);
                        if (id > 1 << bytes_per_strref * 8)
                        {
                            Console.Error.WriteLine($"String id {id} out of range");
                            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                        }
                    }

                    for (int k = 0; k < n; k++)
                    {
                        rawdata[ofs * row_count + i * n + k] = t.Data[i][ofs_mem + k];
                    }

                    ofs_mem += m;
                    ofs += n;
                }

                rawsize += row_size;
            }

            return WriteStreamData(db, t.Name, rawdata, rawsize);
        }

        private static int ReadRawInt(byte[] data, int col, int bytes)
        {
            int ret = 0;
            for (int i = 0; i < bytes; i++)
            {
                ret += (data[col + i] << i * 8);
            }

            return ret;
        }

        // TODO: Move to LibmsiTableView
        private static LibmsiResult RecordEncodedStreamName(LibmsiTableView tv, LibmsiRecord rec, out string pstname)
        {
            int len = tv.Name.Length + 1;
            string stname = tv.Name + '\0';

            LibmsiResult r;
            for (int i = 0; i < tv.NumCols; i++)
            {
                if ((tv.Columns[i].Type & MSITYPE_KEY) != 0)
                {
                    string sval = rec.DupRecordField(i + 1);
                    if (sval == null)
                    {
                        r = LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;
                        goto err;
                    }

                    len += szDot.Length + sval.Length;
                    stname += szDot + sval;
                }
                else
                {
                    continue;
                }
            }

            pstname = EncodeStreamName(false, stname).TrimEnd('\0');
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;

        err:
            pstname = null;
            return r;
        }

        // TODO: Move to LibmsiTableView
        private static LibmsiRecord GetTransformRecord(LibmsiTableView tv, StringTable st, GsfInfile stg, byte[] rawdata, int bytes_per_strref)
        {
            int val, ofs = 0;
            LibmsiColumnInfo[] columns = tv.Columns;

            ushort mask = (ushort)(rawdata[0] | (rawdata[1] << 8));
            rawdata = rawdata.Skip(2).ToArray();

            LibmsiRecord rec = LibmsiRecord.Create(tv.NumCols);
            if (rec == null)
                return rec;

            for (int i = 0; i < tv.NumCols; i++)
            {
                if ((mask & 1) != 0 && (i >= (mask >> 8)))
                    break;

                // All keys must be present
                if ((~mask & 1) != 0 && (~columns[i].Type & MSITYPE_KEY) != 0 && ((1 << i) & ~mask) != 0)
                    continue;

                if (MSITYPE_IS_BINARY(tv.Columns[i].Type))
                {
                    GsfInput stm = null;

                    ofs += BytesPerColumn(columns[i], bytes_per_strref);

                    LibmsiResult r = RecordEncodedStreamName(tv, rec, out string encname);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return null;

                    Exception err = null;
                    stm = stg.ChildByName(encname, ref err);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return null;

                    rec.LoadStream(i + 1, stm);
                }
                else if ((columns[i].Type & MSITYPE_STRING) != 0)
                {
                    val = ReadRawInt(rawdata, ofs, bytes_per_strref);
                    string sval = st.LookupId(val);
                    rec.SetString(i + 1, sval);
                    ofs += bytes_per_strref;
                }
                else
                {
                    int n = BytesPerColumn(columns[i], bytes_per_strref);
                    switch (n)
                    {
                        case 2:
                            val = ReadRawInt(rawdata, ofs, n);
                            if (val != 0)
                                rec.SetInt(i + 1, val - 0x8000);

                            break;
                        case 4:
                            val = ReadRawInt(rawdata, ofs, n);
                            if (val != 0)
                                rec.SetInt(i + 1, (int)(val ^ 0x80000000));

                            break;
                        default:
                            Console.Error.WriteLine($"Oops - unknown column width {n}");
                            break;
                    }

                    ofs += n;
                }
            }

            return rec;
        }

        // TODO: Move to LibmsiRecord
        private static void DumpRecord(LibmsiRecord rec)
        {
            int n = rec.GetFieldCount();
            for (int i = 1; i <= n; i++)
            {
                string sval;
                if (rec.IsNull(i))
                {
                    Console.WriteLine("row . []");
                }
                else if ((sval = rec.GetStringRaw(i)) != null)
                {
                    Console.WriteLine($"row . [{sval}]");
                }
                else
                {
                    Console.WriteLine($"row . [0x{rec.GetInt(i):8x}]");
                }
            }
        }

        // TODO: Move to StringTable
        private static void DumpTable(StringTable st, ushort[] rawdata, int rawsize)
        {
            for (int i = 0; i < (rawsize / 2); i++)
            {
                string sval = st.LookupId(rawdata[i]);
                Console.WriteLine($" {rawdata[i]:4x} {sval}");
            }
        }

        // TODO: Move to LibmsiTableView
        private static int[] RecordToRow(LibmsiTableView tv, LibmsiRecord rec)
        {
            string str;
            LibmsiResult r;

            int[] data = new int[tv.NumCols];
            for (int i = 0; i < tv.NumCols; i++)
            {
                data[i] = 0;

                if ((~tv.Columns[i].Type & MSITYPE_KEY) != 0)
                    continue;

                // Turn the transform column value into a row value
                if ((tv.Columns[i].Type & MSITYPE_STRING) != 0 && !MSITYPE_IS_BINARY(tv.Columns[i].Type))
                {
                    str = rec.GetStringRaw(i + 1);
                    if (str != null)
                    {
                        r = tv.Database.Strings.IdFromStringUTF8(str, out data[i]);

                        /* if there's no matching string in the string table,
                        these keys can't match any record, so fail now. */
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            return null;
                    }
                    else
                    {
                        data[i] = 0;
                    }
                }
                else
                {
                    data[i] = rec.GetInt(i + 1);

                    if (data[i] == LIBMSI_NULL_INT)
                        data[i] = 0;
                    else if ((tv.Columns[i].Type & 0xff) == 2)
                        data[i] += 0x8000;
                    else
                        unchecked { data[i] += (int)0x80000000; }
                }
            }
            return data;
        }

        // TODO: Move to LibmsiTableView
        private static LibmsiResult RowMatches(LibmsiTableView tv, int row, int[] data, out int column)
        {
            column = 0;

            LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            for (int i = 0; i < tv.NumCols; i++)
            {
                if ((~tv.Columns[i].Type & MSITYPE_KEY) != 0)
                    continue;

                // Turn the transform column value into a row value
                LibmsiResult r = tv.FetchInt(row, i + 1, out int x);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine("FetchInt shouldn't fail here");
                    break;
                }

                // If this key matches, move to the next column
                if (x != data[i])
                {
                    ret = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                    break;
                }

                column = i;
                ret = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            return ret;
        }

        // TODO: Move to LibmsiDatabase
        private static LibmsiResult LoadTransform(LibmsiDatabase db, GsfInfile stg, StringTable st, TRANSFORMDATA transform, int bytes_per_strref)
        {
            int i, colcol = 0;

            if (transform == null)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            string name = transform.Name;
            string coltable = "\0";

            // Read the transform data
            ReadStreamData(stg, name, out byte[] rawdata, out int rawsize);
            if (rawdata == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_TABLE;

            // Create a table view
            LibmsiResult r = LibmsiTableView.Create(db, name, out LibmsiView view);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                if (view != null)
                    view.Delete();

                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            LibmsiTableView tv = (view as LibmsiTableView);
            if (tv == null)
            {
                if (view != null)
                    view.Delete();

                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            r = tv.Execute(null);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                if (tv != null)
                    tv.Delete();

                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            // Interpret the data
            for (int n = 0; n < rawsize;)
            {
                int sz, num_cols;

                int mask = rawdata[n] | (rawdata[n + 1] << 8);
                if ((mask & 1) != 0)
                {
                    // If the low bit is set, columns are continuous and
                    // the number of columns is specified in the high byte

                    sz = 2;
                    num_cols = mask >> 8;
                    for (i = 0; i < num_cols; i++)
                    {
                        if ((tv.Columns[i].Type & MSITYPE_STRING) != 0 && !MSITYPE_IS_BINARY(tv.Columns[i].Type))
                            sz += bytes_per_strref;
                        else
                            sz += BytesPerColumn(tv.Columns[i], bytes_per_strref);
                    }
                }
                else
                {
                    // If the low bit is not set, mask is a bitmask.
                    // Excepting for key fields, which are always present,
                    //  each bit indicates that a field is present in the transform record.
                    //
                    // mask == 0 is a special case ... only the keys will be present
                    // and it means that this row should be deleted.

                    sz = 2;
                    num_cols = tv.NumCols;
                    for (i = 0; i < num_cols; i++)
                    {
                        if ((tv.Columns[i].Type & MSITYPE_KEY) != 0 || ((1 << i) & mask) != 0)
                        {
                            if ((tv.Columns[i].Type & MSITYPE_STRING) != 0 && !MSITYPE_IS_BINARY(tv.Columns[i].Type))
                                sz += bytes_per_strref;
                            else
                                sz += BytesPerColumn(tv.Columns[i], bytes_per_strref);
                        }
                    }
                }

                // Check we didn't run of the end of the table
                if (n + sz > rawsize)
                {
                    Console.Error.WriteLine("Borked.");
                    DumpTable(st, rawdata.Cast<ushort>().ToArray(), rawsize);
                    break;
                }

                LibmsiRecord rec = GetTransformRecord(tv, st, stg, rawdata.Skip(n).ToArray(), bytes_per_strref);
                if (rec != null)
                {
                    string table = string.Empty;
                    int number = 0;
                    unchecked { number = (int)LIBMSI_NULL_INT; }
                    int row = 0;

                    if (name == szColumns)
                    {
                        int tablesz = 32;
                        rec.GetString(1, out table, ref tablesz);
                        number = rec.GetInt(2);

                        // Native msi seems writes nul into the Number (2nd) column of
                        // the _Columns table, only when the columns are from a new table
                        if (number == LIBMSI_NULL_INT)
                        {
                            // Reset the column number on a new table
                            if (coltable != table)
                            {
                                colcol = 0;
                                coltable = table;
                            }

                            // Fix nul column numbers
                            rec.SetInt(2, ++colcol);
                        }
                    }

                    bool TRACE_ON = false; // TODO: Make configurable
                    if (TRACE_ON)
                        DumpRecord(rec);

                    r = FindRow(tv, rec, out row, out _);
                    if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    {
                        if (mask == 0)
                        {
                            r = tv.DeleteRow(row);
                            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                                Console.Error.WriteLine($"Failed to delete row {r}");
                        }
                        else if ((mask & 1) != 0)
                        {
                            r = tv.SetRow(row, rec, (1 << tv.NumCols) - 1);
                            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                                Console.Error.WriteLine($"Failed to modify row {r}");
                        }
                        else
                        {
                            r = tv.SetRow(row, rec, mask);
                            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                                Console.Error.WriteLine($"Failed to modify row {r}");
                        }
                    }
                    else
                    {
                        r = tv.InsertRow(rec, -1, false);
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            Console.Error.WriteLine($"Failed to insert row {r}");
                    }

                    if (number != LIBMSI_NULL_INT && name == szColumns)
                        UpdateTableColumns(db, table);
                }

                n += sz;
            }

            // No need to free the table, it's associated with the database
            if (tv != null)
                tv.Delete();

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion
    }
}