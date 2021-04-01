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
using System.Runtime.InteropServices;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_SECTION_HEADER
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name;
        
        // Misc
        public uint PhysicalAddress;
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