using System;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// An import lookup table is an array of 32-bit numbers for PE32 or an array of 64-bit numbers for PE32+.
    /// Each entry uses the bit-field format that is described in the following table.
    /// In this format, bit 31 is the most significant bit for PE32 and bit 63 is the most significant bit for PE32+.
    /// The collection of these entries describes all imports from a given DLL.
    /// The last entry is set to zero (NULL) to indicate the end of the table.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-lookup-table</remarks>
    public class ImportLookupTable
    {
        /// <remarks>Number of entries is known after parsing</remarks>
        public uint[] EntriesPE32;

        /// <remarks>Number of entries is known after parsing</remarks>
        public ulong[] EntriesPE32Plus;

        public static ImportLookupTable Deserialize(Stream stream, bool pe32plus)
        {
            var ilt = new ImportLookupTable();

            // PE32+ has 8-byte values
            if (pe32plus)
            {
                List<ulong> tempEntries = new List<ulong>();
                while (true)
                {
                    ulong bitfield = stream.ReadUInt64();
                    tempEntries.Add(bitfield);
                    if (bitfield == 0)
                        break;
                }

                if (tempEntries.Count > 0)
                    ilt.EntriesPE32Plus = tempEntries.ToArray();
            }
            else
            {
                List<uint> tempEntries = new List<uint>();
                while (true)
                {
                    uint bitfield = stream.ReadUInt32();
                    tempEntries.Add(bitfield);
                    if (bitfield == 0)
                        break;
                }

                if (tempEntries.Count > 0)
                    ilt.EntriesPE32 = tempEntries.ToArray();
            }

            return ilt;
        }

        public static ImportLookupTable Deserialize(byte[] content, ref int offset, bool pe32plus)
        {
            var ilt = new ImportLookupTable();

            // PE32+ has 8-byte values
            if (pe32plus)
            {
                List<ulong> tempEntries = new List<ulong>();
                while (true)
                {
                    ulong bitfield = content.ReadUInt64(ref offset);
                    tempEntries.Add(bitfield);
                    if (bitfield == 0)
                        break;
                }

                if (tempEntries.Count > 0)
                    ilt.EntriesPE32Plus = tempEntries.ToArray();
            }
            else
            {
                List<uint> tempEntries = new List<uint>();
                while (true)
                {
                    uint bitfield = content.ReadUInt32(ref offset);
                    tempEntries.Add(bitfield);
                    if (bitfield == 0)
                        break;
                }

                if (tempEntries.Count > 0)
                    ilt.EntriesPE32 = tempEntries.ToArray();
            }

            return ilt;
        }
    }
}