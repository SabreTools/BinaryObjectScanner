using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Headers
{
    internal class DataDirectoryHeader
    {
        /// <summary>
        /// The first field, VirtualAddress, is actually the RVA of the table.
        /// The RVA is the address of the table relative to the base address of the image when the table is loaded.
        /// </summary>
        public uint VirtualAddress;

        /// <summary>
        /// The second field gives the size in bytes.
        /// </summary>
        public uint Size;

        public static DataDirectoryHeader Deserialize(Stream stream)
        {
            var ddh = new DataDirectoryHeader();

            ddh.VirtualAddress = stream.ReadUInt32();
            ddh.Size = stream.ReadUInt32();

            return ddh;
        }

        public static DataDirectoryHeader Deserialize(byte[] content, ref int offset)
        {
            var ddh = new DataDirectoryHeader();

            ddh.VirtualAddress = BitConverter.ToUInt32(content, offset); offset += 4;
            ddh.Size = BitConverter.ToUInt32(content, offset); offset += 4;

            return ddh;
        }
    }
}