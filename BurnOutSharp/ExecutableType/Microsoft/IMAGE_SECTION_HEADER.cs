using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_SECTION_HEADER
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.IMAGE_SIZEOF_SHORT_NAME)]
        public byte[] Name;
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public SectionCharacteristics Characteristics;

        public static IMAGE_SECTION_HEADER Deserialize(Stream stream)
        {
            var ish = new IMAGE_SECTION_HEADER();

            ish.Name = stream.ReadBytes(Constants.IMAGE_SIZEOF_SHORT_NAME);
            ish.VirtualSize = stream.ReadUInt32();
            ish.VirtualAddress = stream.ReadUInt32();
            ish.SizeOfRawData = stream.ReadUInt32();
            ish.PointerToRawData = stream.ReadUInt32();
            ish.PointerToRelocations = stream.ReadUInt32();
            ish.PointerToLinenumbers = stream.ReadUInt32();
            ish.NumberOfRelocations = stream.ReadUInt16();
            ish.NumberOfLinenumbers = stream.ReadUInt16();
            ish.Characteristics = (SectionCharacteristics)stream.ReadUInt32();

            return ish;
        }

        public static IMAGE_SECTION_HEADER Deserialize(byte[] content, int offset)
        {
            var ish = new IMAGE_SECTION_HEADER();

            ish.Name = new byte[Constants.IMAGE_SIZEOF_SHORT_NAME];
            Array.Copy(content, offset, ish.Name, 0, Constants.IMAGE_SIZEOF_SHORT_NAME); offset += Constants.IMAGE_SIZEOF_SHORT_NAME;
            ish.VirtualSize = BitConverter.ToUInt32(content, offset); offset += 4;
            ish.VirtualAddress = BitConverter.ToUInt32(content, offset); offset += 4;
            ish.SizeOfRawData = BitConverter.ToUInt32(content, offset); offset += 4;
            ish.PointerToRawData = BitConverter.ToUInt32(content, offset); offset += 4;
            ish.PointerToRelocations = BitConverter.ToUInt32(content, offset); offset += 4;
            ish.PointerToLinenumbers = BitConverter.ToUInt32(content, offset); offset += 4;
            ish.NumberOfRelocations = BitConverter.ToUInt16(content, offset); offset += 2;
            ish.NumberOfLinenumbers = BitConverter.ToUInt16(content, offset); offset += 2;
            ish.Characteristics = (SectionCharacteristics)BitConverter.ToUInt32(content, offset); offset += 4;

            return ish;
        }
    }
}