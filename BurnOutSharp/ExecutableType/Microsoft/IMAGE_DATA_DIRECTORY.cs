using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_DATA_DIRECTORY
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

        public static IMAGE_DATA_DIRECTORY Deserialize(Stream stream)
        {
            var idd = new IMAGE_DATA_DIRECTORY();

            idd.VirtualAddress = stream.ReadUInt32();
            idd.Size = stream.ReadUInt32();

            return idd;
        }

        public static IMAGE_DATA_DIRECTORY Deserialize(byte[] content, int offset)
        {
            var idd = new IMAGE_DATA_DIRECTORY();

            idd.VirtualAddress = BitConverter.ToUInt32(content, offset); offset += 4;
            idd.Size = BitConverter.ToUInt32(content, offset); offset += 4;

            return idd;
        }
    }
}