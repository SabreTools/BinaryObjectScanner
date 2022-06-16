/*
 * String Table Functions
 *
 * Copyright 2002-2004, Mike McCormack for CodeWeavers
 * Copyright 2007 Robert Shearman for CodeWeavers
 * Copyright 2010 Hans Leidekker for CodeWeavers
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
using System.Text;
using LibGSF.Input;
using static LibMSI.LibmsiTable;
using static LibMSI.MsiPriv;

namespace LibMSI
{
    internal class msistring
    {
        public ushort PersistentRefCount { get; set; }

        public ushort NonPersistentRefCount { get; set; }

        public string Str { get; set; }
    }

    internal class StringTable
    {
        #region Constants

        private const int CP_ACP = 0;

        #endregion

        #region Properties

        /// <summary>
        /// The number of strings
        /// </summary>
        public int MaxCount { get; set; }

        public int FreeSlot { get; set; }

        public int CodePage { get; set; }

        public int SortCount { get; set; }

        /// <summary>
        /// An array of strings
        /// </summary>
        public msistring[] Strings { get; set; }

        /// <summary>
        /// Index
        /// </summary>
        public int[] Sorted { get; set; }

        #endregion

        #region Functions

        public void Destroy()
        {
            Strings = null;
            Sorted = null;
        }

        public int AddString(string data, int len, ushort refcount, StringPersistence persistence)
        {
            if (string.IsNullOrEmpty(data) || data[0] == '\0')
                return 0;

            if (IdFromStringUTF8(data, out int n) == LibmsiResult.LIBMSI_RESULT_SUCCESS )
            {
                if (persistence == StringPersistence.StringPersistent)
                    Strings[n].PersistentRefCount += refcount;
                else
                    Strings[n].NonPersistentRefCount += refcount;

                return n;
            }

            n = FindFreeEntry();
            if (n == -1)
                return -1;

            // Allocate a new string
            if (len < 0)
                len = data.Length;

            SetEntry(n, data + '\0', refcount, persistence);

            return n;
        }

        /// <summary>
        /// Find the string identified by an id - return null if there's none
        /// </summary>
        public string LookupId(int id)
        {
            if (id == 0)
                return szEmpty;

            if (id >= MaxCount)
                return null;

            if (id != 0 && Strings[id].PersistentRefCount == 0 && Strings[id].NonPersistentRefCount == 0)
                return null;

            return Strings[id].Str;
        }

        /// <param name="str">String to find in the string table</param>
        /// <param name="id">Id of the string, if found</param>
        public LibmsiResult IdFromStringUTF8(string str, out int id)
        {
            int low = 0, high = SortCount - 1;

            while (low <= high)
            {
                int i = (low + high) / 2;
                int c = str.CompareTo(Strings[Sorted[i]].Str);

                if (c < 0)
                {
                    high = i - 1;
                }
                else if (c > 0)
                {
                    low = i + 1;
                }
                else
                {
                    id = Sorted[i];
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                }
            }

            id = 0;
            return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;
        }

        public static StringTable InitStringTable(out int bytes_per_strref)
        {
            bytes_per_strref = sizeof(ushort);
            return InitStringTable(1, CP_ACP);
        }

        public static StringTable LoadStringTable(GsfInfile stg, out int bytes_per_strref)
        {
            int codepage;
            int count, len;
            ushort refs;

            bytes_per_strref = 0;
            LibmsiResult r = ReadStreamData(stg, szStringPool, out byte[] pool, out int poolsize);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return null;

            r = ReadStreamData(stg, szStringData, out byte[] data, out int datasize);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return null;

            if ((poolsize > 4) && (BitConverter.ToUInt16(pool, ((1) * 2)) & 0x8000) != 0)
                bytes_per_strref = LONG_STR_BYTES;
            else
                bytes_per_strref = sizeof(ushort);

            count = poolsize / 4;
            if (poolsize > 4)
                codepage = (int)(BitConverter.ToUInt16(pool, ((0) * 2)) | ( (BitConverter.ToUInt16(pool, ((1) * 2)) & ~0x8000) << 16));
            else
                codepage = CP_ACP;

            StringTable st = InitStringTable(count, codepage);
            if (st == null)
                return null;

            int offset = 0;
            int n = 1;
            int i = 1;
            while (i < count)
            {
                // The string reference count is always the second word
                refs = BitConverter.ToUInt16(pool, ((i * 2 + 1) * 2));

                // Empty entries have two zeros, still have a string id
                if (BitConverter.ToUInt16(pool, ((i * 2) * 2)) == 0 && refs == 0)
                {
                    i++;
                    n++;
                    continue;
                }

                // If a string is over 64k, the previous string entry is made null
                // and the high word of the length is inserted in the null string's
                // reference count field.

                if (BitConverter.ToUInt16(pool, ((i * 2) * 2)) == 0)
                {
                    len = (int)((BitConverter.ToUInt16(pool, ((i * 2 + 3 * 2))) << 16) + BitConverter.ToUInt16(pool, ((i * 2 + 2) * 2)));
                    i += 2;
                }
                else
                {
                    len = (int)BitConverter.ToUInt16(pool,((i * 2) * 2));
                    i += 1;
                }

                if ((offset + len) > datasize)
                {
                    Console.Error.WriteLine("String table corrupt?");
                    break;
                }

                int s = st.AddString(n, data, offset, len, refs, StringPersistence .StringPersistent);
                if (s != n)
                    Console.Error.WriteLine($"Failed to add string {n}");

                n++;
                offset += len;
            }

            if (datasize != offset)
                Console.Error.WriteLine($"String table load failed! ({datasize} != {offset}), please report");

            return st;
        }

        public LibmsiResult SaveStringTable(LibmsiDatabase db, out int bytes_per_strref)
        {
            // Construct the new table in memory first
            StringTotalSize(out int datasize, out int poolsize);

            byte[] data = new byte[datasize];
            byte[] pool = new byte[poolsize];

            int used = 0;
            int codepage = CodePage;
            pool[0] = (byte)(codepage & 0xff);
            pool[1] = (byte)(codepage >> 8);
            pool[2] = (byte)(codepage >> 16);
            pool[3] = (byte)(codepage >> 24);

            if (MaxCount > 0xffff)
            {
                pool[3] |= 0x80;
                bytes_per_strref = LONG_STR_BYTES;
            }
            else
            {
                bytes_per_strref = sizeof(ushort);
            }

            int i = 1;
            for (int n=1; n < MaxCount; n++ )
            {
                if (Strings[n].PersistentRefCount == 0)
                {
                    pool[i * 4]     = 0;
                    pool[i * 4 + 1] = 0;
                    pool[i * 4 + 2] = 0;
                    pool[i * 4 + 3] = 0;
                    i++;

                    continue;
                }

                int sz = datasize - used;


                LibmsiResult s = StringId(n, ref data, ref used, ref sz);
                if (s != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine("Failed to fetch string");
                    sz = 0;
                }

                if (sz == 0)
                {
                    pool[i * 4]     = 0;
                    pool[i * 4 + 1] = 0;
                    pool[i * 4 + 2] = 0;
                    pool[i * 4 + 3] = 0;
                    i++;

                    continue;
                }

                if (sz >= 0x10000)
                {
                    // Write a dummy entry, with the high part of the length
                    // in the reference count.
                    pool[i * 4]     = 0;
                    pool[i * 4 + 1] = 0;
                    pool[i * 4 + 2] = (byte)(sz >> 16);
                    pool[i * 4 + 3] = (byte)(sz >> 24);
                    i++;
                }

                pool[i * 4]     = (byte)sz;
                pool[i * 4 + 1] = (byte)(sz >> 8);
                pool[i * 4 + 2] = (byte)Strings[n].PersistentRefCount;
                pool[i * 4 + 3] = (byte)(Strings[n].PersistentRefCount >> 8);
                i++;

                used += sz;
                if (used > datasize)
                {
                    Console.Error.WriteLine($"Oops overran {used} >= {datasize}");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }
            }

            if (used != datasize)
            {
                Console.Error.WriteLine($"Oops used {used} != datasize {datasize}");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            // Write the streams
            LibmsiResult r = WriteStreamData(db, szStringData, data, datasize);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            r = WriteStreamData(db, szStringPool, pool, poolsize);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        public int GetCodePage() => CodePage;

        public LibmsiResult SetCodePage(int codepage)
        {
            if (ValidateCodePage(codepage))
            {
                CodePage = codepage;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }
            
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        #endregion

        #region Utilities

        private static bool ValidateCodePage(int codepage)
        {
            switch (codepage)
            {
                case CP_ACP:
                case 37: case 424: case 437: case 500: case 737: case 775: case 850:
                case 852: case 855: case 856: case 857: case 860: case 861: case 862:
                case 863: case 864: case 865: case 866: case 869: case 874: case 875:
                case 878: case 932: case 936: case 949: case 950: case 1006: case 1026:
                case 1250: case 1251: case 1252: case 1253: case 1254: case 1255:
                case 1256: case 1257: case 1258: case 1361:
                case 10000: case 10006: case 10007: case 10029: case 10079: case 10081:
                case 20127: case 20866: case 20932: case 21866: case 28591: case 28592:
                case 28593: case 28594: case 28595: case 28596: case 28597: case 28598:
                case 28599: case 28600: case 28603: case 28604: case 28605: case 28606:
                case 65000: case 65001:
                    return true;

                default:
                    return false;
            }
        }

        private static StringTable InitStringTable(int entries, int codepage)
        {
            if (!ValidateCodePage(codepage))
                return null;

            if (entries < 1)
                entries = 1;

            return new StringTable
            {
                Strings = new msistring[entries],
                Sorted = new int[entries],
                MaxCount = entries,
                FreeSlot = 1,
                CodePage = codepage,
                SortCount = 0,
            };
        }

        private int FindFreeEntry()
        {
            if (FreeSlot != 0)
            {
                for (int i = FreeSlot; i < MaxCount; i++)
                {
                    if (Strings[i].PersistentRefCount == 0 && Strings[i].NonPersistentRefCount == 0)
                        return i;
                }
            }

            for (int i = 1; i < MaxCount; i++)
            {
                if (Strings[i].PersistentRefCount == 0 && Strings[i].NonPersistentRefCount == 0)
                    return i;
            }

            // Dynamically resize
            int sz = MaxCount + 1 + MaxCount / 2;

            msistring[] p = Strings;
            Array.Resize(ref p, sz);
            Strings = p;

            int[] s = Sorted;
            Array.Resize(ref s, sz);
            Sorted = s;

            FreeSlot = MaxCount;
            MaxCount = sz;

            if (Strings[FreeSlot].PersistentRefCount != 0 || Strings[FreeSlot].NonPersistentRefCount != 0)
                Console.Error.WriteLine("Oops. expected freeslot to be free...");

            return FreeSlot;
        }

        private int FindInsertIndex(int string_id)
        {
            int low = 0, high = SortCount - 1;

            while (low <= high)
            {
                int i = (low + high) / 2;
                int c = Strings[string_id].Str.CompareTo(Strings[Sorted[i]].Str);

                if (c < 0)
                    high = i - 1;
                else if (c > 0)
                    low = i + 1;
                else
                    return -1; // Already exists
            }

            return high + 1;
        }

        private void InsertStringSorted(int string_id)
        {
            int i = FindInsertIndex(string_id);
            if (i == -1)
                return;

            int[] temp = new int[SortCount - i];
            Array.Copy(Sorted, i, temp, 0, SortCount - i);
            Array.Copy(temp, 0, Sorted, i + 1, SortCount - i);
            Sorted[i] = string_id;
            SortCount++;
        }

        private void SetEntry(int n, string str, ushort refcount, StringPersistence persistence)
        {
            if (str == null)
                return;

            if (persistence == StringPersistence.StringPersistent)
            {
                Strings[n].PersistentRefCount = refcount;
                Strings[n].NonPersistentRefCount = 0;
            }
            else
            {
                Strings[n].PersistentRefCount = 0;
                Strings[n].NonPersistentRefCount = refcount;
            }

            Strings[n].Str = str;

            InsertStringSorted(n);

            if (n < MaxCount)
                FreeSlot = n + 1;
        }

        private LibmsiResult IdFromString(byte[] buffer, int offset, out int id)
        {
            id = 0;
            if (buffer == null || buffer[offset] == 0)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            int codepage = CodePage != 0 ? CodePage : Encoding.Default.CodePage;
            Encoding cpconv = Encoding.GetEncoding(codepage);
            string str = cpconv.GetString(new ReadOnlySpan<byte>(buffer, offset, buffer.Length - offset).ToArray());
            if (str == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            LibmsiResult r = IdFromStringUTF8(str, out id);
            return r;
        }

        private int AddString(int n, byte[] data, int offset, int len, ushort refcount, StringPersistence persistence)
        {
            if (data == null || data[offset] == '\0')
                return 0;

            if (n > 0)
            {
                if (Strings[n].PersistentRefCount != 0 || Strings[n].NonPersistentRefCount != 0)
                    return -1;
            }
            else
            {
                if (IdFromString(data, offset, out n) == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    if (persistence == StringPersistence.StringPersistent)
                        Strings[n].PersistentRefCount += refcount;
                    else
                        Strings[n].NonPersistentRefCount += refcount;

                    return n;
                }

                n = FindFreeEntry();
                if (n == -1)
                    return -1;
            }

            if (n < 1)
            {
                Console.Error.WriteLine($"Invalid index adding {data} ({n})");
                return -1;
            }

            // Allocate a new string
            int codepage = CodePage != 0 ? CodePage : Encoding.Default.CodePage;
            Encoding cpconv = Encoding.GetEncoding(codepage);
            string str = cpconv.GetString(data);
            SetEntry(n, str, refcount, persistence);

            return n;
        }

        /// <param name="st">Pointer to the string table</param>
        /// <param name="id">Id of the string to retrieve</param>
        /// <param name="buffer">Destination of the UTF8 string</param>
        /// <param name="sz">
        /// Number of bytes available in the buffer on input
        /// Number of bytes used on output
        /// </param>
        /// <remarks>Returned string is not NUL-terminated.</remarks>
        private LibmsiResult StringId(int id, ref byte[] buffer, ref int offset, ref int sz)
        {
            string str_utf8 = LookupId(id);
            if (str_utf8 == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            int codepage = CodePage != 0 ? CodePage : Encoding.Default.CodePage;
            Encoding cpconv = Encoding.GetEncoding(codepage);
            byte[] str = Encoding.Convert(Encoding.UTF8, cpconv, Encoding.UTF8.GetBytes(str_utf8));
            int len = str.Length;

            if (sz < len)
            {
                sz = len;
                return LibmsiResult.LIBMSI_RESULT_MORE_DATA;
            }

            sz = len;
            Array.Copy(str, 0, buffer, offset, str.Length);
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private void StringTotalSize(out int datasize, out int poolsize)
        {
            if (Strings[0].Str != null || Strings[0].PersistentRefCount != 0 || Strings[0].NonPersistentRefCount != 0)
                Console.Error.WriteLine("Oops. element 0 has a string");

            int codepage = CodePage != 0 ? CodePage : Encoding.Default.CodePage;

            poolsize = 4;
            datasize = 0;
            int holesize = 0;
            for (int i = 1; i < MaxCount; i++)
            {
                if (Strings[i].PersistentRefCount == 0)
                {
                    poolsize += 4;
                }
                else if (Strings[i].Str != null)
                {
                    Encoding cpconv = Encoding.GetEncoding(codepage);
                    string str = cpconv.GetString(Encoding.UTF8.GetBytes(Strings[i].Str));

                    datasize += str.Length;
                    if (str.Length > 0xffff)
                        poolsize += 4;

                    poolsize += holesize + 4;
                    holesize = 0;
                }
                else
                {
                    holesize += 4;
                }
            }
        }

        #endregion
    }
}