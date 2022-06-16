/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002-2004 Mike McCormack for CodeWeavers
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
using System.IO;
using LibGSF.Input;
using LibMSI.Internal;
using static LibMSI.LibmsiTypes;

namespace LibMSI
{
    /* maybe we can use a Variant instead of doing it ourselves? */
    internal class LibmsiField
    {
        public int Type { get; set; }

        #region Union (u)

        public int iVal
        {
            get
            {
                try { return (int)u; }
                catch { return default; }
            }
            set => u = value;
        }

        public string szVal
        {
            get => u as string;
            set => u = value;
        }

        public GsfInput Stream
        {
            get => u as GsfInput;
            set => u = value;
        }

        private object u = new object();

        #endregion
    }

    public class LibmsiRecord
    {
        #region Constants

        private const int LIBMSI_FIELD_TYPE_NULL = 0;
        private const int LIBMSI_FIELD_TYPE_INT = 1;
        private const int LIBMSI_FIELD_TYPE_STR = 3;
        private const int LIBMSI_FIELD_TYPE_STREAM = 4;

        #endregion

        #region Properties

        /// <summary>
        /// as passed to libmsi_record_new
        /// </summary>
        internal int Count { get; set; }

        /// <summary>
        /// nb. array size is count+1
        /// </summary>
        internal LibmsiField[] Fields { get; set; }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiRecord() { }

        public static LibmsiRecord Create(int count)
        {
            if (count >= 65535)
                return null;

            return new LibmsiRecord
            {
                Count = count,
                Fields = new LibmsiField[count],
            };
        }

        #endregion

        #region Functions

        /// <summary>
        /// Clear record fields.
        /// <summary>
        /// <returns>True on success.</returns>
        public bool Clear()
        {
            for (int i = 0; i <= Count; i++)
            {
                Fields[i].Type = LIBMSI_FIELD_TYPE_NULL;
                Fields[i].iVal = 0;
            }

            return true;
        }

        /// <returns>The number of record fields.</returns>
        public int GetFieldCount() => Count;

        /// <param name="field">A field identifier</param>
        /// <returns>True if the field is null (or <paramref name="field"/> > record field count)</returns>
        public bool IsNull(int field) => (field > Count) || (Fields[field].Type == LIBMSI_FIELD_TYPE_NULL);

        /// <summary>
        /// Set the <paramref name="field"/> to the integer value <paramref name="iVal"/>.
        /// <summary>
        /// <param name="field">A field identifier</param>
        /// <param name="iVal">Value to set field to</param>
        /// <returns>True on success.</returns>
        public bool SetInt(int field, int iVal)
        {
            if (field > Count)
                return false;

            Fields[field].Type = LIBMSI_FIELD_TYPE_INT;
            Fields[field].iVal = iVal;

            return true;
        }

        /// <summary>
        /// Get the integer value of <paramref name="field"/>. If the field is a string
        /// representing an integer, it will be converted to an integer value.
        /// Other values and types will return LIBMSI_NULL_INT.
        /// <summary>
        /// <param name="field">A field identifier</param>
        /// <returns>The integer value, or LIBMSI_NULL_INT if the field is not an integer.</returns>
        public int GetInt(int field)
        {
            if (field > Count)
                unchecked { return (int)LIBMSI_NULL_INT; }

            switch (Fields[field].Type)
            {
                case LIBMSI_FIELD_TYPE_INT:
                    return Fields[field].iVal;
                case LIBMSI_FIELD_TYPE_STR:
                    if (ExprIntFromString(Fields[field].szVal, out int ret))
                        return ret;

                    unchecked { return (int)LIBMSI_NULL_INT; }
                default:
                    Console.Error.WriteLine($"Invalid field type {Fields[field].Type}");
                    break;
            }

            unchecked { return (int)LIBMSI_NULL_INT; }
        }

        /// <summary>
        /// Set the <paramref name="field"/> value to <paramref name="szValue"/> string.
        /// <summary>
        /// <param name="field">A field identifier</param>
        /// <param name="szValue">A string or null</param>
        /// <returns>True on success.</returns>
        public bool SetString(int field, string szValue)
        {
            if (field > Count)
                return false;

            if (szValue != null && szValue[0] != '\0')
            {
                Fields[field].Type = LIBMSI_FIELD_TYPE_STR;
                Fields[field].szVal = szValue;
            }
            else
            {
                Fields[field].Type = LIBMSI_FIELD_TYPE_NULL;
                Fields[field].szVal = null;
            }

            return true;
        }

        /// <summary>
        /// Get a string representation of <paramref name="field"/>.
        /// <summary>
        /// <param name="field">A field identifier</param>
        /// <returns>A string, or null on error.</returns>
        public string GetString(int field)
        {
            if (field > Count)
                return string.Empty; // FIXME: really?

            switch (Fields[field].Type)
            {
                case LIBMSI_FIELD_TYPE_INT:
                    return Fields[field].iVal.ToString();
                case LIBMSI_FIELD_TYPE_STR:
                    return Fields[field].szVal;
                case LIBMSI_FIELD_TYPE_NULL:
                    return string.Empty;
                default:
                    Console.Error.WriteLine($"Invalid type {Fields[field].Type}");
                    break;
            }

            return null;
        }

        /// <summary>
        /// Load the file content as a stream in <paramref name="field"/>.
        /// <summary>
        /// <param name="field">A field identifier</param>
        /// <param name="szFilename">A filename or null</param>
        /// <returns>True on success.</returns>
        public bool LoadStream(int field, string szFilename)
        {
            LibmsiResult ret = LoadStreamFromFile(field, szFilename);
            return ret == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <summary>
        /// Set the stream content from <paramref name="input"/> stream.
        /// <summary>
        /// <param name="field">A field identifier</param>
        /// <param name="input">A GInputStream</param>
        /// <param name="count">The number of bytes to read from <paramref name="input"/></param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success.</returns>
        public bool SetStream(int field, Stream input, int count, ref Exception error)
        {
            if (!input.CanRead)
                return false;
            if (field <= 0 || field > Count)
                return false;
            if (count <= 0)
                return false;
            if (error != null)
                return false;

            byte[] data = new byte[count];

            int bytes_read = input.Read(data, 0, count);
            if (bytes_read != count)
                return false;

            GsfInput stm = GsfInputMemory.Create(data, count, true);
            if (LoadStream(field, stm) != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return false;

            return true;
        }

        /// <summary>
        /// Get the stream associated with the given record <paramref name="field"/>.
        /// <summary>
        /// <param name="field">A field identifier</param>
        /// <returns>True on success.</returns>
        public Stream GetStream(int field)
        {
            Exception err = null;
            GsfInput stm = GetStream(field, ref err);
            if (stm == null)
                return null;

            return LibmsiIStream.Create(stm);
        }

        #endregion

        #region Internal Functions

        internal LibmsiResult CopyField(int in_n, LibmsiRecord out_rec, int out_n)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;

            if (in_n > Count || out_n > out_rec.Count)
            {
                r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }
            else if (this != out_rec || in_n != out_n)
            {
                LibmsiField input = Fields[in_n];
                LibmsiField output = out_rec.Fields[out_n];

                switch (input.Type)
                {
                    case LIBMSI_FIELD_TYPE_NULL:
                        break;
                    case LIBMSI_FIELD_TYPE_INT:
                        output.iVal = input.iVal;
                        break;
                    case LIBMSI_FIELD_TYPE_STR:
                        output.szVal = input.szVal;
                        break;
                    case LIBMSI_FIELD_TYPE_STREAM:
                        output.Stream = input.Stream;
                        break;
                    default:
                        Console.Error.WriteLine($"Invalid field type {input.Type}");
                        break;
                }

                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    output.Type = input.Type;
            }

            return r;
        }

        internal string GetStringRaw(int field)
        {
            if (field > Count)
                return null;

            if (Fields[field].Type != LIBMSI_FIELD_TYPE_STR)
                return null;

            return Fields[field].szVal;
        }

        internal LibmsiResult GetString(int field, out string szValue, ref int pcchValue)
        {
            if (field > Count)
            {
                if (pcchValue > 0)
                    szValue = "\0";
                else
                    szValue = null;

                pcchValue = 0;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            int len;
            switch (Fields[field].Type)
            {
                case LIBMSI_FIELD_TYPE_INT:
                    szValue = Fields[field].iVal.ToString();
                    len = szValue.Length;
                    break;
                case LIBMSI_FIELD_TYPE_STR:
                    szValue = Fields[field].szVal;
                    len = Fields[field].szVal.Length;
                    break;
                case LIBMSI_FIELD_TYPE_NULL:
                    if (pcchValue > 0)
                        szValue = "\0";
                    else
                        szValue = null;

                    len = 0;
                    break;
                default:
                    szValue = null;
                    len = 0;
                    break;
            }

            LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            if (szValue != null && pcchValue <= len)
                ret = LibmsiResult.LIBMSI_RESULT_MORE_DATA;

            pcchValue = len;

            return ret;
        }

        internal LibmsiResult LoadStream(int field, GsfInput stream)
        {
            if ((field == 0) || (field > Count))
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            Fields[field].Type = LIBMSI_FIELD_TYPE_STREAM;
            Fields[field].Stream = stream;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult LoadStreamFromFile(int field, string szFilename)
        {
            GsfInput stm;
            LibmsiResult r;

            if ((field == 0) || (field > Count))
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            // No filename means we should seek back to the start of the stream
            if (szFilename == null)
            {
                if (Fields[field].Type != LIBMSI_FIELD_TYPE_STREAM)
                    return LibmsiResult.LIBMSI_RESULT_INVALID_FIELD;

                stm = Fields[field].Stream;
                if (stm == null)
                    return LibmsiResult.LIBMSI_RESULT_INVALID_FIELD;

                stm.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                // Read the file into a stream and save the stream in the record
                r = AddStreamFromFile(szFilename, out stm);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                // If all's good, store it in the record
                LoadStream(field, stm);
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult SetGsfInput(int field, GsfInput stm)
        {
            if (field > Count)
                return LibmsiResult.LIBMSI_RESULT_INVALID_FIELD;

            Fields[field].Type = LIBMSI_FIELD_TYPE_STREAM;
            Fields[field].Stream = stm;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult GetGsfInput(int field, out GsfInput pstm)
        {
            pstm = null;
            if (field > Count)
                return LibmsiResult.LIBMSI_RESULT_INVALID_FIELD;

            if (Fields[field].Type != LIBMSI_FIELD_TYPE_STREAM)
                return LibmsiResult.LIBMSI_RESULT_INVALID_FIELD;

            pstm = Fields[field].Stream;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiRecord Clone()
        {
            int count = GetFieldCount();
            LibmsiRecord clone = Create(count);
            if (clone == null)
                return null;

            for (int i = 0; i <= count; i++)
            {
                if (Fields[i].Type == LIBMSI_FIELD_TYPE_STREAM)
                {
                    Exception err = null;
                    GsfInput stm = Fields[i].Stream.Duplicate(ref err);
                    if (stm == null)
                        return null;

                    clone.Fields[i].Stream = stm;
                    clone.Fields[i].Type = LIBMSI_FIELD_TYPE_STREAM;
                }
                else
                {
                    LibmsiResult r = CopyField(i, clone, i);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return null;
                }
            }

            return clone;
        }

        internal static bool RecordCompareFields(LibmsiRecord a, LibmsiRecord b, int field)
        {
            if (a.Fields[field].Type != b.Fields[field].Type)
                return false;

            switch (a.Fields[field].Type)
            {
                case LIBMSI_FIELD_TYPE_NULL:
                    return true;

                case LIBMSI_FIELD_TYPE_INT:
                    return (a.Fields[field].iVal == b.Fields[field].iVal);

                case LIBMSI_FIELD_TYPE_STR:
                    return (a.Fields[field].szVal == b.Fields[field].szVal);

                case LIBMSI_FIELD_TYPE_STREAM:
                default:
                    return false;
            }
        }

        internal static bool RecordCompare(LibmsiRecord a, LibmsiRecord b)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i <= a.Count; i++)
            {
                if (!RecordCompareFields(a, b, i))
                    return false;
            }

            return true;
        }

        internal string DupRecordField(int field)
        {
            if (IsNull(field))
                return null;

            int sz = 0;
            LibmsiResult r = GetString(field, out _, ref sz);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return null;

            sz++;
            r = GetString(field, out string str, ref sz);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Console.Error.WriteLine("Failed to get string!");
                return null;
            }

            return str;
        }

        #endregion

        #region Utilities

        private static bool ExprIntFromString(string str, out int output)
        {
            output = 0;

            int x = 0;
            int p = 0; // str[0]

            // Skip the minus sign
            if (str[p] == '-')
                p++;

            while (p < str.Length && str[p] != '\0')
            {
                if ((str[p] < '0') || (str[p] > '9'))
                    return false;

                x *= 10;
                x += (str[p] - '0');
                p++;
            }

            // Check if it's negative
            if (str[0] == '-')
                x = -x;

            output = x;

            return true;
        }

        /// <summary>
        /// Read the data in a file into a memory-backed GsfInput
        /// </summary>
        private static LibmsiResult AddStreamFromFile(string szFile, out GsfInput pstm)
        {
            pstm = null;

            Exception err = null;
            GsfInput stm = GsfInputStdio.Create(szFile, ref err);
            if (stm == null)
            {
                Console.Error.WriteLine($"Open file failed for {szFile}");
                return LibmsiResult.LIBMSI_RESULT_OPEN_FAILED;
            }

            long sz = stm.Size;
            byte[] data;
            if (sz == 0)
            {
                data = new byte[1];
            }
            else
            {
                data = new byte[sz];
                if (stm.Read((int)sz, data) == null)
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            pstm = GsfInputMemory.Create(data, sz, true);

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private GsfInput GetStream(int field, ref Exception error)
        {
            if (field > Count)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER}");
                return null;
            }

            if (Fields[field].Type == LIBMSI_FIELD_TYPE_NULL)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {LibmsiResult.LIBMSI_RESULT_INVALID_DATA}");
                return null;
            }

            if (Fields[field].Type != LIBMSI_FIELD_TYPE_STREAM)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {LibmsiResult.LIBMSI_RESULT_INVALID_DATATYPE}");
                return null;
            }

            GsfInput stm = Fields[field].Stream;
            if (stm == null)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER}");
                return null;
            }

            return stm;
        }

        #endregion
    }
}