using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The export name pointer table is an array of addresses (RVAs) into the export name table.
    /// The pointers are 32 bits each and are relative to the image base.
    /// The pointers are ordered lexically to allow binary searches.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#export-name-pointer-table</remarks>
    internal class ExportNamePointerTable
    {
        /// <remarks>Number of entries is defined externally</remarks>
        public uint[] Entries;

        public static ExportNamePointerTable Deserialize(Stream stream, int count)
        {
            var enpt = new ExportNamePointerTable();

            enpt.Entries = new uint[count];
            for (int i = 0; i < count; i++)
            {
                enpt.Entries[i] = stream.ReadUInt32();
            }

            return enpt;
        }

        public static ExportNamePointerTable Deserialize(byte[] content, ref int offset, int count)
        {
            var enpt = new ExportNamePointerTable();

            enpt.Entries = new uint[count];
            for (int i = 0; i < count; i++)
            {
                enpt.Entries[i] = BitConverter.ToUInt32(content, offset); offset += 4;
            }

            return enpt;
        }
    }
}