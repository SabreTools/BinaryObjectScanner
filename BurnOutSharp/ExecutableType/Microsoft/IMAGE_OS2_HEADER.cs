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
    /// <summary>
    /// New .EXE header
    /// </summary>
    internal class IMAGE_OS2_HEADER
    {
        public ushort Magic { get; private set; }               // 00 Magic number NE_MAGIC
        public byte LinkerVersion { get; private set; }         // 02 Linker Version number
        public byte LinkerRevision { get; private set; }        // 03 Linker Revision number
        public ushort EntryTableOffset { get; private set; }    // 04 Offset of Entry Table
        public ushort EntryTableSize { get; private set; }      // 06 Number of bytes in Entry Table
        public uint CrcChecksum { get; private set; }           // 08 Checksum of whole file
        public ushort Flags { get; private set; }               // 0C Flag word
        public ushort Autodata { get; private set; }            // 0E Automatic data segment number
        public ushort InitialHeapAlloc { get; private set; }    // 10 Initial heap allocation
        public ushort InitialStackAlloc { get; private set; }   // 12 Initial stack allocation
        public uint InitialCSIPSetting { get; private set; }    // 14 Initial CS:IP setting
        public uint InitialSSSPSetting { get; private set; }    // 18 Initial SS:SP setting
        public ushort FileSegmentCount { get; private set; }    // 1C Count of file segments
        public ushort ModuleReferenceTableSize { get; private set; }    // 1E Entries in Module Reference Table
        public ushort NonResidentNameTableSize { get; private set; }    // 20 Size of non-resident name table
        public ushort SegmentTableOffset { get; private set; }  // 22 Offset of Segment Table
        public ushort ResourceTableOffset { get; private set; } // 24 Offset of Resource Table
        public ushort ResidentNameTableOffset { get; private set; }     // 26 Offset of resident name table
        public ushort ModuleReferenceTableOffset { get; private set; }  // 28 Offset of Module Reference Table
        public ushort ImportedNamesTableOffset { get; private set; }    // 2A Offset of Imported Names Table
        public uint NonResidentNamesTableOffset { get; private set; }   // 2C Offset of Non-resident Names Table
        public ushort MovableEntriesCount { get; private set; }         // 30 Count of movable entries
        public ushort SegmentAlignmentShiftCount { get; private set; }  // 32 Segment alignment shift count
        public ushort ResourceEntriesCount { get; private set; }        // 34 Count of resource entries
        public byte TargetOperatingSystem { get; private set; }         // 36 Target operating system
        public byte AdditionalFlags { get; private set; }       // 37 Additional flags
        public ushort[] Reserved { get; private set; }          // 38 3 reserved words
        public byte WindowsSDKRevision { get; private set; }    // 3E Windows SDK revison number
        public byte WindowsSDKVersion { get; private set; }     // 3F Windows SDK version number

        public static IMAGE_OS2_HEADER Deserialize(Stream stream)
        {
            IMAGE_OS2_HEADER ioh = new IMAGE_OS2_HEADER();

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
