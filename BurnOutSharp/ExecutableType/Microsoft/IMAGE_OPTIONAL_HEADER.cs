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

using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_OPTIONAL_HEADER
    {
        // Standard fields

        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;

        // NT additional fields.

        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Reserved1;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public uint SizeOfStackReserve;
        public uint SizeOfStackCommit;
        public uint SizeOfHeapReserve;
        public uint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory;

        public static IMAGE_OPTIONAL_HEADER Deserialize(Stream stream)
        {
            var ioh = new IMAGE_OPTIONAL_HEADER();

            ioh.Magic = stream.ReadUInt16();
            ioh.MajorLinkerVersion = stream.ReadByteValue();
            ioh.MinorLinkerVersion = stream.ReadByteValue();
            ioh.SizeOfCode = stream.ReadUInt32();
            ioh.SizeOfInitializedData = stream.ReadUInt32();
            ioh.SizeOfUninitializedData = stream.ReadUInt32();
            ioh.AddressOfEntryPoint = stream.ReadUInt32();
            ioh.BaseOfCode = stream.ReadUInt32();
            ioh.BaseOfData = stream.ReadUInt32();

            ioh.ImageBase = stream.ReadUInt32();
            ioh.SectionAlignment = stream.ReadUInt32();
            ioh.FileAlignment = stream.ReadUInt32();
            ioh.MajorOperatingSystemVersion = stream.ReadUInt16();
            ioh.MinorOperatingSystemVersion = stream.ReadUInt16();
            ioh.MajorImageVersion = stream.ReadUInt16();
            ioh.MinorImageVersion = stream.ReadUInt16();
            ioh.MajorSubsystemVersion = stream.ReadUInt16();
            ioh.MinorSubsystemVersion = stream.ReadUInt16();
            ioh.Reserved1 = stream.ReadUInt32();
            ioh.SizeOfImage = stream.ReadUInt32();
            ioh.SizeOfHeaders = stream.ReadUInt32();
            ioh.CheckSum = stream.ReadUInt32();
            ioh.Subsystem = stream.ReadUInt16();
            ioh.DllCharacteristics = stream.ReadUInt16();
            ioh.SizeOfStackReserve = stream.ReadUInt32();
            ioh.SizeOfStackCommit = stream.ReadUInt32();
            ioh.SizeOfHeapReserve = stream.ReadUInt32();
            ioh.SizeOfHeapCommit = stream.ReadUInt32();
            ioh.LoaderFlags = stream.ReadUInt32();
            ioh.NumberOfRvaAndSizes = stream.ReadUInt32();
            ioh.DataDirectory = new IMAGE_DATA_DIRECTORY[Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
            for (int i = 0; i < Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES; i++)
            {
                ioh.DataDirectory[i] = IMAGE_DATA_DIRECTORY.Deserialize(stream);
            }

            return ioh;
        }

        public static IMAGE_OPTIONAL_HEADER Deserialize(byte[] content, int offset)
        {
            var ioh = new IMAGE_OPTIONAL_HEADER();

            ioh.Magic = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MajorLinkerVersion = content[offset]; offset++;
            ioh.MinorLinkerVersion = content[offset]; offset++;
            ioh.SizeOfCode = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfInitializedData = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfUninitializedData = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.AddressOfEntryPoint = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.BaseOfCode = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.BaseOfData = BitConverter.ToUInt32(content, offset); offset += 4;

            ioh.ImageBase = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SectionAlignment = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.FileAlignment = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.MajorOperatingSystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MinorOperatingSystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MajorImageVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MinorImageVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MajorSubsystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MinorSubsystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.Reserved1 = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfImage = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfHeaders = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.CheckSum = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.Subsystem = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.DllCharacteristics = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.SizeOfStackReserve = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfStackCommit = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfHeapReserve = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfHeapCommit = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.LoaderFlags = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.NumberOfRvaAndSizes = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.DataDirectory = new IMAGE_DATA_DIRECTORY[Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
            for (int i = 0; i < Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES; i++)
            {
                ioh.DataDirectory[i] = IMAGE_DATA_DIRECTORY.Deserialize(content, offset); offset += 8;
            }

            return ioh;
        }
    }
}