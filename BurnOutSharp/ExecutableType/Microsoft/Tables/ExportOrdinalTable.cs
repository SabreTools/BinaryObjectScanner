using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The export ordinal table is an array of 16-bit unbiased indexes into the export address table.
    /// Ordinals are biased by the Ordinal Base field of the export directory table.
    /// In other words, the ordinal base must be subtracted from the ordinals to obtain true indexes into the export address table.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#export-ordinal-table</remarks>
    public class ExportOrdinalTable
    {
        /// <remarks>Number of entries is defined externally</remarks>
        public ushort[] Entries;

        public static ExportOrdinalTable Deserialize(Stream stream, int count)
        {
            var edt = new ExportOrdinalTable();

            edt.Entries = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                edt.Entries[i] = stream.ReadUInt16();
            }

            return edt;
        }

        public static ExportOrdinalTable Deserialize(byte[] content, ref int offset, int count)
        {
            var edt = new ExportOrdinalTable();

            edt.Entries = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                edt.Entries[i] = content.ReadUInt16(ref offset);
            }

            return edt;
        }
    }
}