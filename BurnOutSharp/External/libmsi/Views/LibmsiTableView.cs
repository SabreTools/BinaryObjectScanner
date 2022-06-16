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
using System.Linq;
using LibGSF.Input;
using LibMSI.Internal;
using static LibMSI.LibmsiQuery;
using static LibMSI.LibmsiRecord;
using static LibMSI.LibmsiTypes;
using static LibMSI.Internal.LibmsiTable;
using static LibMSI.Internal.MsiPriv;

namespace LibMSI.Views
{
    internal class LibmsiColumnHashEntry
    {
        public LibmsiColumnHashEntry Next { get; set; }

        public int Value { get; set; }

        public int Row { get; set; }
    }

    internal class LibmsiTableView : LibmsiView
    {
        #region Properties

        public LibmsiDatabase Database { get; set; }

        public LibmsiTable Table { get; set; }

        public LibmsiColumnInfo[] Columns { get; set; }

        public int NumCols { get; set; }

        public int RowSize { get; set; }

        public string Name { get; set; }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiTableView() { }

        public static LibmsiResult Create(LibmsiDatabase db, string name, out LibmsiView view)
        {
            if (name == szStreams)
                return LibmsiStreamsView.Create(db, out view);
            else if (name == szStorages)
                return LibmsiStorageView.Create(db, out view);

            LibmsiTableView tv = new LibmsiTableView();
            LibmsiResult r = GetTable(db, name, out LibmsiTable table);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                view = null;
                Console.Error.WriteLine("Table not found");
                return r;
            }

            // Fill the structure
            tv.Table = table;
            tv.Database = db;
            tv.Columns = tv.Table.ColInfo;
            tv.NumCols = tv.Table.ColCount;
            tv.RowSize = GetRowSize(db, tv.Table.ColInfo, tv.Table.ColCount, LONG_STR_BYTES);
            tv.Name = name;

            view = tv;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        public override LibmsiResult FetchInt(int row, int col, out int val)
        {
            val = 0;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if ((col == 0) || (col > NumCols))
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            // How many rows are there?
            if (row >= Table.RowCount)
                return LibmsiResult.NO_MORE_ITEMS;

            if (Columns[col - 1].Offset >= RowSize)
            {
                Console.Error.WriteLine($"Stuffed up {Columns[col - 1].Offset} >= {RowSize}");
                Console.Error.WriteLine($"{this} {Columns}");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            int n = BytesPerColumn(Database, Columns[col - 1], LONG_STR_BYTES);
            if (n != 2 && n != 3 && n != 4)
            {
                Console.Error.WriteLine("oops! what is %d bytes per column?\n", n );
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            int offset = Columns[col-1].Offset;
            val = ReadTableInt(Table.Data, row, offset, n);

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FetchStream(int row, int col, out GsfInput stm)
        {
            stm = null;
            LibmsiResult r = MsiStreamName(row, out string full_name);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Console.Error.WriteLine($"Fetching stream, error = {r}");
                return r;
            }

            string encname = EncodeStreamName(false, full_name);
            r = Database.GetRawStream(encname, out stm);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                Console.Error.WriteLine($"Fetching stream {full_name}, error = {r}");

            if (stm != null)
                stm.Name = full_name;

            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetRow(int row, out LibmsiRecord rec)
        {
            rec = null;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            return MsiViewGetRow(Database, this, row, out rec);
        }

        /// <inheritdoc/>
        public override LibmsiResult SetRow(int row, LibmsiRecord rec, int mask)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            // Test if any of the mask bits are invalid
            if (mask >= (1 << NumCols))
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            for (int i = 0; i < NumCols; i++ )
            {
                // Only update the fields specified in the mask
                if ((mask & (1 << i)) == 0)
                    continue;

                bool persistent = (Table.Persistent != LibmsiCondition.LIBMSI_CONDITION_FALSE) && (Table.DataPersistent[row]);
                
                // FIXME: should we allow updating keys?

                int val = 0;
                if (rec.IsNull(i + 1))
                {
                    r = GetTableValueFromRecord(rec, i + 1, out val);
                    if (MSITYPE_IS_BINARY(Columns[i].Type))
                    {
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

                        r = GetGsfInput(rec, i + 1, out GsfInput stm);
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            return r;

                        r = MsiStreamName(row, out string stname);
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            return r;

                        r = AddStream(Database, stname, stm);
                        if ( r != LibmsiResult.LIBMSI_RESULT_SUCCESS )
                            return r;
                    }
                    else if ((Columns[i].Type & MSITYPE_STRING) != 0)
                    {
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        {
                            string sval = RecordGetStringRaw(rec, i + 1);
                            val = Database.Strings.AddString(sval, -1, 1, persistent ? StringPersistence.StringPersistent : StringPersistence.StringNonPersistent );
                        }
                        else
                        {
                            FetchInt(row, i + 1, out int x);
                            if (val == x)
                                continue;
                        }
                    }
                    else
                    {
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                    }
                }

                r = SetInt(row, i + 1, val);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    break;
            }

            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record) => LibmsiResult.LIBMSI_RESULT_SUCCESS;

        /// <inheritdoc/>
        public override LibmsiResult Close() => LibmsiResult.LIBMSI_RESULT_SUCCESS;

        /// <inheritdoc/>
        public override LibmsiResult GetDimensions(out int rows, out int cols)
        {
            rows = 0;
            cols = NumCols;

            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            rows = Table.RowCount;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            if (n == 0 || n > NumCols)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            name = Columns[n - 1].ColName;
            if (name == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            table_name = Columns[n - 1].TableName;
            if (table_name == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            type = Columns[n - 1].Type;
            temporary = Columns[n - 1].Temporary;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult InsertRow(LibmsiRecord rec, int row, bool temporary)
        {
            // Check that the key is unique - can we find a matching row?
            LibmsiResult r = TableValidateNew(rec, out _);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (row == -1)
                row = FindInsertIndex(rec);

            r = TableCreateNewRow(ref row, temporary);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            // Shift the rows to make room for the new row
            for (int i = Table.RowCount - 1; i > row; i--)
            {
                byte[] temp = new byte[RowSize];
                Array.Copy(Table.Data[i - 1], Table.Data[i], RowSize);
                Table.Data[i - 1] = Enumerable.Repeat<byte>(0x00, RowSize).ToArray();
                Table.DataPersistent[i] = Table.DataPersistent[i - 1];
            }

            // Re-set the persistence flag
            Table.DataPersistent[row] = !temporary;
            return SetRow(row, rec, (1 << NumCols) - 1);
        }

        /// <inheritdoc/>
        public override LibmsiResult DeleteRow(int row)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            LibmsiResult r = GetDimensions(out int num_rows, out int num_cols);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            if (row >= num_rows)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            num_rows = Table.RowCount;
            Table.RowCount--;

            // Reset the hash tables
            for (int i = 0; i < NumCols; i++)
            {
                Columns[i].HashTable = null;
            }

            for (int i = row + 1; i < num_rows; i++)
            {
                Array.Copy(Table.Data[i], Table.Data[i - 1], RowSize);
                Table.DataPersistent[i - 1] = Table.DataPersistent[i];
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult Delete()
        {
            Table = null;
            Columns = null;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            LibmsiColumnHashEntry entry;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (col == 0 || col > NumCols)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (Columns[col - 1].HashTable == null)
            {
                int num_rows = Table.RowCount;
                if (Columns[col - 1].Offset >= RowSize)
                {
                    Console.Error.WriteLine($"Stuffed up {Columns[col - 1].Offset} >= {RowSize}");
                    Console.Error.WriteLine($"{this} {Columns}");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }

                // Allocate contiguous memory for the table and its entries so we
                // don't have to do an expensive cleanup
                LibmsiColumnHashEntry[] hash_table = new LibmsiColumnHashEntry[num_rows];
                Columns[col - 1].HashTable = hash_table;

                int new_entry = 0;
                for (int i = 0; i < num_rows; i++, new_entry++)
                {
                    if (FetchInt(i, col, out int row_value) != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        continue;

                    hash_table[new_entry] = new LibmsiColumnHashEntry
                    {
                        Next = null,
                        Value = row_value,
                        Row = i,
                    };

                    if (hash_table[row_value] != null)
                    {
                        LibmsiColumnHashEntry prev_entry = hash_table[row_value];
                        while (prev_entry.Next != null)
                        {
                            prev_entry = prev_entry.Next;
                        }

                        prev_entry.Next = hash_table[new_entry];
                    }
                    else
                    {
                        hash_table[row_value] = hash_table[new_entry];
                    }
                }
            }

            if (handle == null)
                entry = Columns[col - 1].HashTable[val];
            else
                entry = handle.Next;

            while (entry != null && entry.Value != val)
            {
                entry = entry.Next;
            }

            handle = entry;
            if (entry == null)
                return LibmsiResult.NO_MORE_ITEMS;

            row = entry.Row;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override int AddRef()
        {
            for (int i = 0; i < Table.ColCount; i++)
            {
                if ((Table.ColInfo[i].Type & MSITYPE_TEMPORARY) != 0)
                    Table.ColInfo[i].RefCount++;
            }

            return ++Table.RefCount;
        }

        /// <inheritdoc/>
        public override LibmsiResult RemoveColumn(string table, int number)
        {
            LibmsiRecord rec = LibmsiRecord.Create(2);
            if (rec == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            rec.SetString(1, table);
            rec.SetInt(2, number);

            LibmsiResult r = Create(Database, szColumns, out LibmsiView columns);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = FindRow(columns as LibmsiTableView, rec, out int row, out _);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                columns.Delete();
                return r;
            }

            r = (columns as LibmsiTableView).DeleteRow(row);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                columns.Delete();
                return r;
            }

            UpdateTableColumns(Database, table);
            columns.Delete();
            return r;
        }

        /// <inheritdoc/>
        public override int Release()
        {
            int ref_count = Table.RefCount;
            for (int i = 0; i < Table.ColCount; i++)
            {
                if ((Table.ColInfo[i].Type & MSITYPE_TEMPORARY) != 0)
                {
                    ref_count = --Table.ColInfo[i].RefCount;
                    if (ref_count == 0)
                    {
                        LibmsiResult r = RemoveColumn(Table.ColInfo[i].TableName, Table.ColInfo[i].Number);
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            break;
                    }
                }
            }

            ref_count = --Table.RefCount;
            if (ref_count == 0)
            {
                if (Table.RowCount == 0)
                    Delete();
            }

            return ref_count;
        }

        /// <inheritdoc/>
        public override LibmsiResult AddColumn(string table, int number, string column, int type, bool hold)
        {
            LibmsiRecord rec = LibmsiRecord.Create(4);
            if (rec == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            rec.SetString(1, table);
            rec.SetInt(2, number);
            rec.SetString(3, column);
            rec.SetInt(4, type);

            LibmsiResult r = InsertRow(rec, -1, false);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            UpdateTableColumns(Database, table);

            if (!hold)
                return r;

            LibmsiTable msitable = FindCachedTable(Database, table);
            for (int i = 0; i < msitable.ColCount; i++)
            {
                if (msitable.ColInfo[i].ColName == column)
                {
                    msitable.ColInfo[i].RefCount++;
                    break;
                }
            }

            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult Drop()
        {
            LibmsiResult r;
            for (int i = Table.ColCount - 1; i >= 0; i--)
            {
                r = RemoveColumn(Table.ColInfo[i].TableName, Table.ColInfo[i].Number);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            LibmsiRecord rec = LibmsiRecord.Create(1);
            if (rec == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            rec.SetString(1, Name);

            r = Create(Database, szTables, out LibmsiView tables);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = FindRow((tables as LibmsiTableView), rec, out int row, out _);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                tables.Delete();
                return r;
            }

            r = tables.DeleteRow(row);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                tables.Delete();
                return r;
            }

            tables.Delete();
            return r;
        }

        #endregion

        #region Utilities

        private LibmsiResult MsiStreamName(int row, out string pstname)
        {
            int len = Name.Length + 1;
            string stname = Name;

            LibmsiResult r;
            for (int i = 0; i < NumCols; i++)
            {
                int type = Columns[i].Type;
                if ((type & MSITYPE_KEY) != 0)
                {
                    r = FetchInt(row, i + 1, out int ival);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    {
                        pstname = null;
                        return r;
                    }

                    string sval = string.Empty;
                    if ((Columns[i].Type & MSITYPE_STRING) != 0)
                    {
                        sval = Database.Strings.LookupId(ival );
                        if (sval == null)
                        {
                            pstname = null;
                            return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;
                        }
                    }
                    else
                    {
                        int n = BytesPerColumn(Database, Columns[i], LONG_STR_BYTES);
                        switch (n)
                        {
                            case 2:
                                sval = (ival - 0x8000).ToString();
                                break;
                            case 4:
                                sval = (ival ^ 0x80000000).ToString();
                                break;
                            default:
                                Console.Error.WriteLine($"Oops - unknown column width {n}");
                                pstname = null;
                                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                        }
                    }

                    len += szDot.Length + sval.Length;
                    stname += szDot + sval;
                }
                else
                {
                    continue;
                }
            }

            pstname = stname;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult SetInt(int row, int col, int val)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (col == 0 || col > NumCols)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (row >= Table.RowCount)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (Columns[col - 1].Offset >= RowSize)
            {
                Console.Error.WriteLine($"Stuffed up {Columns[col - 1].Offset} >= {RowSize}");
                Console.Error.WriteLine($"{this} {Columns}");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            Columns[col - 1].HashTable = null;

            int n = BytesPerColumn(Database, Columns[col - 1], LONG_STR_BYTES);
            if (n != 2 && n != 3 && n != 4)
            {
                Console.Error.WriteLine($"Oops! what is {n} bytes per column?");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            int offset = Columns[col - 1].Offset;
            for (int i = 0; i < n; i++)
            {
                Table.Data[row][offset + i] = (byte)((val >> i * 8) & 0xff);
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult GetTableValueFromRecord(LibmsiRecord rec, int iField, out int pvalue)
        {
            LibmsiResult r;

            pvalue = 0;
            if (iField <= 0 || iField > NumCols || rec.IsNull(iField))
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiColumnInfo columninfo = Columns[iField - 1];
            if (MSITYPE_IS_BINARY(columninfo.Type))
            {
                pvalue = 1; // Refers to the first key column
            }
            else if ((columninfo.Type & MSITYPE_STRING) != 0)
            {
                string sval = RecordGetStringRaw(rec, iField);
                if (sval != null)
                {
                    r = Database.Strings.IdFromStringUTF8(sval, out pvalue);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return LibmsiResult.LIBMSI_RESULT_NOT_FOUND;
                }
                else
                {
                    pvalue = 0;
                }
            }
            else if (BytesPerColumn(Database, columninfo, LONG_STR_BYTES) == 2)
            {
                pvalue = 0x8000 + rec.GetInt(iField);
                if ((pvalue & 0xffff0000) != 0)
                {
                    Console.Error.WriteLine($"Field {iField} value {pvalue - 0x8000} out of range");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }
            }
            else
            {
                int ival = rec.GetInt(iField);
                pvalue = (int)(ival ^ 0x80000000);
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult TableCreateNewRow(ref int num, bool temporary)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            byte[] row = new byte[RowSize];
            int data_ptr = 0; // Table.Data[0][]
            int data_persist_ptr = 0; // Table.DataPersistent[0];
            if (num == -1)
                num = Table.RowCount;

            int sz = Table.RowCount + 1;
            if (Table.Data[data_ptr] != null)
            {
                byte[] p = Table.Data[data_ptr];
                Array.Resize(ref p, sz);
                Table.Data[data_ptr] = p;
            }
            else
            {
                Table.Data[data_ptr] = new byte[sz];
            }

            sz = Table.RowCount + 1;
            if (Table.DataPersistent != null)
            {
                bool[] b = Table.DataPersistent;
                Array.Resize(ref b, sz);
                Table.DataPersistent = b;
            }
            else
            {
                Table.DataPersistent = new bool[sz];
            }

            Table.Data[data_ptr + Table.RowCount] = row;
            Table.DataPersistent[data_persist_ptr + Table.RowCount] = !temporary;

            Table.RowCount++;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult TableValidateNew(LibmsiRecord rec, out int column)
        {
            // Check there's no null values where they're not allowed
            for (int i = 0; i < NumCols; i++ )
            {
                if ((Columns[i].Type & MSITYPE_NULLABLE) != 0)
                    continue;

                if (MSITYPE_IS_BINARY(Columns[i].Type))
                {
                    // Skip binary columns
                }
                else if ((Columns[i].Type & MSITYPE_STRING) != 0)
                {
                    string str = RecordGetStringRaw(rec, i + 1);
                    if (str == null || str[0] == 0)
                    {
                        column = i;
                        return LibmsiResult.LIBMSI_RESULT_INVALID_DATA;
                    }
                }
                else
                {
                    int n = rec.GetInt(i + 1);
                    if (n == LIBMSI_NULL_INT)
                    {
                        column = i;
                        return LibmsiResult.LIBMSI_RESULT_INVALID_DATA;
                    }
                }
            }

            // Check there's no duplicate keys
            LibmsiResult r = FindRow(this, rec, out int row, out column);
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private int CompareRecord(int row, LibmsiRecord rec)
        {
            for (int i = 0; i < NumCols; i++ )
            {
                if ((Columns[i].Type & MSITYPE_KEY) == 0)
                    continue;

                LibmsiResult r = GetTableValueFromRecord(rec, i + 1, out int ivalue);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return 1;

                r = FetchInt(row, i + 1, out int x);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine($"FetchInt should not fail here {r}");
                    return -1;
                }

                if (ivalue > x)
                {
                    return 1;
                }
                else if (ivalue == x)
                {
                    if (i < NumCols - 1)
                        continue;

                    return 0;
                }
                else
                {
                    return -1;
                }
            }

            return 1;
        }

        private int FindInsertIndex(LibmsiRecord rec)
        {
            int low = 0, high = Table.RowCount - 1;
            while (low <= high)
            {
                int idx = (low + high) / 2;
                int c = CompareRecord(idx, rec);

                if (c < 0)
                    high = idx - 1;
                else if (c > 0)
                    low = idx + 1;
                else
                    return idx;
            }

            return high + 1;
        }

        #endregion
    }
}