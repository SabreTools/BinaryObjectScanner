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
    /// <summary>
    /// New .EXE header
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_OS2_HEADER
    {
        public ushort Magic;               // 00 Magic number NE_MAGIC
        public byte LinkerVersion;         // 02 Linker Version number
        public byte LinkerRevision;        // 03 Linker Revision number
        public ushort EntryTableOffset;    // 04 Offset of Entry Table
        public ushort EntryTableSize;      // 06 Number of bytes in Entry Table
        public uint CrcChecksum;           // 08 Checksum of whole file
        public ushort Flags;               // 0C Flag word
        public ushort Autodata;            // 0E Automatic data segment number
        public ushort InitialHeapAlloc;    // 10 Initial heap allocation
        public ushort InitialStackAlloc;   // 12 Initial stack allocation
        public uint InitialCSIPSetting;    // 14 Initial CS:IP setting
        public uint InitialSSSPSetting;    // 18 Initial SS:SP setting
        public ushort FileSegmentCount;    // 1C Count of file segments
        public ushort ModuleReferenceTableSize;    // 1E Entries in Module Reference Table
        public ushort NonResidentNameTableSize;    // 20 Size of non-resident name table
        public ushort SegmentTableOffset;  // 22 Offset of Segment Table
        public ushort ResourceTableOffset; // 24 Offset of Resource Table
        public ushort ResidentNameTableOffset;     // 26 Offset of resident name table
        public ushort ModuleReferenceTableOffset;  // 28 Offset of Module Reference Table
        public ushort ImportedNamesTableOffset;    // 2A Offset of Imported Names Table
        public uint NonResidentNamesTableOffset;   // 2C Offset of Non-resident Names Table
        public ushort MovableEntriesCount;         // 30 Count of movable entries
        public ushort SegmentAlignmentShiftCount;  // 32 Segment alignment shift count
        public ushort ResourceEntriesCount;        // 34 Count of resource entries
        public byte TargetOperatingSystem;         // 36 Target operating system
        public byte AdditionalFlags;       // 37 Additional flags
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.NERESWORDS)]
        public ushort[] Reserved;          // 38 3 reserved words
        public byte WindowsSDKRevision;    // 3E Windows SDK revison number
        public byte WindowsSDKVersion;     // 3F Windows SDK version number

        public static IMAGE_OS2_HEADER Deserialize(Stream stream)
        {
            var ioh = new IMAGE_OS2_HEADER();

            ioh.Magic = stream.ReadUInt16();
            ioh.LinkerVersion = stream.ReadByteValue();
            ioh.LinkerRevision = stream.ReadByteValue();
            ioh.EntryTableOffset = stream.ReadUInt16();
            ioh.EntryTableSize = stream.ReadUInt16();
            ioh.CrcChecksum = stream.ReadUInt32();
            ioh.Flags = stream.ReadUInt16();
            ioh.Autodata = stream.ReadUInt16();
            ioh.InitialHeapAlloc = stream.ReadUInt16();
            ioh.InitialStackAlloc = stream.ReadUInt16();
            ioh.InitialCSIPSetting = stream.ReadUInt32();
            ioh.InitialSSSPSetting = stream.ReadUInt32();
            ioh.FileSegmentCount = stream.ReadUInt16();
            ioh.ModuleReferenceTableSize = stream.ReadUInt16();
            ioh.NonResidentNameTableSize = stream.ReadUInt16();
            ioh.SegmentTableOffset = stream.ReadUInt16();
            ioh.ResourceTableOffset = stream.ReadUInt16();
            ioh.ResidentNameTableOffset = stream.ReadUInt16();
            ioh.ModuleReferenceTableOffset = stream.ReadUInt16();
            ioh.ImportedNamesTableOffset = stream.ReadUInt16();
            ioh.NonResidentNamesTableOffset = stream.ReadUInt32();
            ioh.MovableEntriesCount = stream.ReadUInt16();
            ioh.SegmentAlignmentShiftCount = stream.ReadUInt16();
            ioh.ResourceEntriesCount = stream.ReadUInt16();
            ioh.TargetOperatingSystem = stream.ReadByteValue();
            ioh.AdditionalFlags = stream.ReadByteValue();
            ioh.Reserved = new ushort[Constants.NERESWORDS];
            for (int i = 0; i < Constants.NERESWORDS; i++)
            {
                ioh.Reserved[i] = stream.ReadUInt16();
            }
            ioh.WindowsSDKRevision = stream.ReadByteValue();
            ioh.WindowsSDKVersion = stream.ReadByteValue();

            return ioh;
        }
    }
}
