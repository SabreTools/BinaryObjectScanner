using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Entries;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The export address table contains the address of exported entry points and exported data and absolutes.
    /// An ordinal number is used as an index into the export address table.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#export-address-table</remarks>
    internal class ExportAddressTable
    {
        /// <remarks>Number of entries is defined externally</remarks>
        public ExportAddressTableEntry[] Entries;

        public static ExportAddressTable Deserialize(Stream stream, int count)
        {
            var eat = new ExportAddressTable();

            eat.Entries = new ExportAddressTableEntry[count];
            for (int i = 0; i < count; i++)
            {
                eat.Entries[i] = ExportAddressTableEntry.Deserialize(stream);
            }

            return eat;
        }

        public static ExportAddressTable Deserialize(byte[] content, int offset, int count)
        {
            var eat = new ExportAddressTable();

            eat.Entries = new ExportAddressTableEntry[count];
            for (int i = 0; i < count; i++)
            {
                eat.Entries[i] = ExportAddressTableEntry.Deserialize(content, offset); offset += 4;
            }

            return eat;
        }
    }
}