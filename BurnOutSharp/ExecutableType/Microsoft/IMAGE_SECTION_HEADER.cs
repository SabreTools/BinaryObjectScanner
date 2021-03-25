/*
 *	  NEWEXE.H (C) Copyright Microsoft Corp 1984-1987
 *
 *	  Data structure definitions for the OS/2 & Windows
 *	  executable file format.
 *
 *	  Modified by IVS on 24-Jan-1991 for Resource DeCompiler
 *	  (C) Copyright IVS 1991
 *
 *    http://csn.ul.ie/~caolan/pub/winresdump/winresdump/newexe.h
 */

using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    internal class IMAGE_SECTION_HEADER
    {
        public byte[] Name { get; private set; }
        
        // Misc
        public uint PhysicalAddress { get; private set; }
        public uint VirtualSize { get; private set; }

        public uint VirtualAddress { get; private set; }
        public uint SizeOfRawData { get; private set; }
        public uint PointerToRawData { get; private set; }
        public uint PointerToRelocations { get; private set; }
        public uint PointerToLinenumbers { get; private set; }
        public ushort NumberOfRelocations { get; private set; }
        public ushort NumberOfLinenumbers { get; private set; }
        public SectionCharacteristics Characteristics { get; private set; }

        public static IMAGE_SECTION_HEADER Deserialize(Stream stream)
        {
            IMAGE_SECTION_HEADER ish = new IMAGE_SECTION_HEADER();

            ish.Name = stream.ReadBytes(Constants.IMAGE_SIZEOF_SHORT_NAME);

            // Misc
            ish.PhysicalAddress = stream.ReadUInt32();
            ish.VirtualSize = ish.PhysicalAddress;

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
    }
}