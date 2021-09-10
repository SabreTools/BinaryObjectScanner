using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Entries;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The structure and content of the import address table are identical to those of the import lookup table, until the file is bound.
    /// During binding, the entries in the import address table are overwritten with the 32-bit (for PE32) or 64-bit (for PE32+) addresses of the symbols that are being imported.
    /// These addresses are the actual memory addresses of the symbols, although technically they are still called "virtual addresses."
    /// The loader typically processes the binding.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-address-table</remarks>
    public class ImportAddressTable
    {
        /// <remarks>Number of entries is known after parsing</remarks>
        public ImportAddressTableEntry[] Entries;

        public static ImportAddressTable Deserialize(Stream stream)
        {
            var iat = new ImportAddressTable();

            List<ImportAddressTableEntry> tempEntries = new List<ImportAddressTableEntry>();
            while (true)
            {
                var entry = ImportAddressTableEntry.Deserialize(stream);
                tempEntries.Add(entry);
                if (entry.IsNull())
                    break;
            }

            iat.Entries = tempEntries.ToArray();
            return iat;
        }

        public static ImportAddressTable Deserialize(byte[] content, ref int offset)
        {
            var iat = new ImportAddressTable();

            List<ImportAddressTableEntry> tempEntries = new List<ImportAddressTableEntry>();
            while (true)
            {
                var entry = ImportAddressTableEntry.Deserialize(content, ref offset);
                tempEntries.Add(entry);
                if (entry.IsNull())
                    break;
            }

            iat.Entries = tempEntries.ToArray();
            return iat;
        }
    }
}