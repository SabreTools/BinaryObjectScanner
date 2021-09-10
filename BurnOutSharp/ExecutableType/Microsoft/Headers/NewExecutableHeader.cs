using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Headers
{
    /// <summary>
    /// The NE header is a relatively large structure with multiple characteristics.
    /// Because of the age of the format some items are unclear in meaning.
    /// </summary>
    /// <remarks>http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm</remarks>
    internal class NewExecutableHeader
    {
        /// <summary>
        /// Signature word. [00]
        /// "N" is low-order byte.
        /// "E" is high-order byte.
        /// </summary>
        public ushort Magic;
        
        /// <summary>
        /// Version number of the linker. [02]
        /// </summary>
        public byte LinkerVersion;
        
        /// <summary>
        /// Revision number of the linker. [03]
        /// </summary>
        public byte LinkerRevision;
        
        /// <summary>
        /// Entry Table file offset, relative to the beginning of the segmented EXE header. [04]
        /// </summary>
        public ushort EntryTableOffset;
        
        /// <summary>
        /// Number of bytes in the entry table. [06]
        /// </summary>
        public ushort EntryTableSize;
        
        /// <summary>
        /// 32-bit CRC of entire contents of file. [08]
        /// These words are taken as 00 during the calculation.
        /// </summary>
        public uint CrcChecksum;
        
        /// <summary>
        /// Program flags, bitmapped [0C]
        /// </summary>
        public byte ProgramFlags;

        /// <summary>
        /// Application flags, bitmapped [0D]
        /// </summary>
        public byte ApplicationFlags;
        
        /// <summary>
        /// Automatic data segment number [0E]
        /// </summary>
        public ushort Autodata;
        
        /// <summary>
        /// Initial heap allocation [10]
        /// </summary>
        public ushort InitialHeapAlloc;
        
        /// <summary>
        /// Initial stack allocation [12]
        /// </summary>
        public ushort InitialStackAlloc;
        
        /// <summary>
        /// CS:IP entry point, CS is index into segment table [14]
        /// </summary>
        public uint InitialCSIPSetting;
        
        /// <summary>
        /// SS:SP inital stack pointer, SS is index into segment table [18]
        /// </summary>
        public uint InitialSSSPSetting;
        
        /// <summary>
        /// Number of segments in segment table [1C]
        /// </summary>
        public ushort FileSegmentCount;
        
        /// <summary>
        /// Entries in Module Reference Table [1E]
        /// </summary>
        public ushort ModuleReferenceTableSize;
        
        /// <summary>
        /// Size of non-resident name table [20]
        /// </summary>
        public ushort NonResidentNameTableSize;
        
        /// <summary>
        /// Offset of Segment Table [22]
        /// </summary>
        public ushort SegmentTableOffset;
        
        /// <summary>
        /// Offset of Resource Table [24]
        /// </summary>
        public ushort ResourceTableOffset;
        
        /// <summary>
        /// Offset of resident name table [26]
        /// </summary>
        public ushort ResidentNameTableOffset;
        
        /// <summary>
        /// Offset of Module Reference Table [28]
        /// </summary>
        public ushort ModuleReferenceTableOffset;
        
        /// <summary>
        /// Offset of Imported Names Table [2A]
        /// </summary>
        public ushort ImportedNamesTableOffset;
        
        /// <summary>
        /// Offset of Non-resident Names Table [2C]
        /// </summary>
        public uint NonResidentNamesTableOffset;
        
        /// <summary>
        /// Count of moveable entry points listed in entry table [30]
        /// </summary>
        public ushort MovableEntriesCount;
        
        /// <summary>
        /// File allignment size shift count (0-9 (default 512 byte pages)) [32]
        /// </summary>
        public ushort SegmentAlignmentShiftCount;
        
        /// <summary>
        /// Count of resource table entries [34]
        /// </summary>
        public ushort ResourceEntriesCount;
        
        /// <summary>
        /// Target operating system [36]
        /// </summary>
        public byte TargetOperatingSystem;
        
        /// <summary>
        /// Other OS/2 flags [37]
        /// </summary>
        public byte AdditionalFlags;
        
        /// <summary>
        /// Offset to return thunks or start of gangload area [38]
        /// </summary>
        public ushort ReturnThunkOffset;
        
        /// <summary>
        /// Offset to segment reference thunks or size of gangload area [3A]
        /// </summary>
        public ushort SegmentReferenceThunkOffset;

        /// <summary>
        /// Minimum code swap area size [3C]
        /// </summary>
        public ushort MinCodeSwapAreaSize;
        
        /// <summary>
        /// Windows SDK revison number [3E]
        /// </summary>
        public byte WindowsSDKRevision;
        
        /// <summary>
        /// Windows SDK version number [3F]
        /// </summary>
        public byte WindowsSDKVersion;

        public static NewExecutableHeader Deserialize(Stream stream)
        {
            var neh = new NewExecutableHeader();

            neh.Magic = stream.ReadUInt16();
            neh.LinkerVersion = stream.ReadByteValue();
            neh.LinkerRevision = stream.ReadByteValue();
            neh.EntryTableOffset = stream.ReadUInt16();
            neh.EntryTableSize = stream.ReadUInt16();
            neh.CrcChecksum = stream.ReadUInt32();
            neh.ProgramFlags = stream.ReadByteValue();
            neh.ApplicationFlags = stream.ReadByteValue();
            neh.Autodata = stream.ReadUInt16();
            neh.InitialHeapAlloc = stream.ReadUInt16();
            neh.InitialStackAlloc = stream.ReadUInt16();
            neh.InitialCSIPSetting = stream.ReadUInt32();
            neh.InitialSSSPSetting = stream.ReadUInt32();
            neh.FileSegmentCount = stream.ReadUInt16();
            neh.ModuleReferenceTableSize = stream.ReadUInt16();
            neh.NonResidentNameTableSize = stream.ReadUInt16();
            neh.SegmentTableOffset = stream.ReadUInt16();
            neh.ResourceTableOffset = stream.ReadUInt16();
            neh.ResidentNameTableOffset = stream.ReadUInt16();
            neh.ModuleReferenceTableOffset = stream.ReadUInt16();
            neh.ImportedNamesTableOffset = stream.ReadUInt16();
            neh.NonResidentNamesTableOffset = stream.ReadUInt32();
            neh.MovableEntriesCount = stream.ReadUInt16();
            neh.SegmentAlignmentShiftCount = stream.ReadUInt16();
            neh.ResourceEntriesCount = stream.ReadUInt16();
            neh.TargetOperatingSystem = stream.ReadByteValue();
            neh.AdditionalFlags = stream.ReadByteValue();
            neh.ReturnThunkOffset = stream.ReadUInt16();
            neh.SegmentReferenceThunkOffset = stream.ReadUInt16();
            neh.MinCodeSwapAreaSize = stream.ReadUInt16();
            neh.WindowsSDKRevision = stream.ReadByteValue();
            neh.WindowsSDKVersion = stream.ReadByteValue();

            return neh;
        }

        public static NewExecutableHeader Deserialize(byte[] content, ref int offset)
        {
            var neh = new NewExecutableHeader();

            neh.Magic = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.LinkerVersion = content[offset++];
            neh.LinkerRevision = content[offset++];
            neh.EntryTableOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.EntryTableSize = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.CrcChecksum = BitConverter.ToUInt32(content, offset); offset += 4;
            neh.ProgramFlags = content[offset++];
            neh.ApplicationFlags = content[offset++];
            neh.Autodata = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.InitialHeapAlloc = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.InitialStackAlloc = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.InitialCSIPSetting = BitConverter.ToUInt32(content, offset); offset += 4;
            neh.InitialSSSPSetting = BitConverter.ToUInt32(content, offset); offset += 4;
            neh.FileSegmentCount = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.ModuleReferenceTableSize = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.NonResidentNameTableSize = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.SegmentTableOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.ResourceTableOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.ResidentNameTableOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.ModuleReferenceTableOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.ImportedNamesTableOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.NonResidentNamesTableOffset = BitConverter.ToUInt32(content, offset); offset += 4;
            neh.MovableEntriesCount = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.SegmentAlignmentShiftCount = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.ResourceEntriesCount = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.TargetOperatingSystem = content[offset++];
            neh.AdditionalFlags = content[offset++];
            neh.ReturnThunkOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.SegmentReferenceThunkOffset = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.MinCodeSwapAreaSize = BitConverter.ToUInt16(content, offset); offset += 2;
            neh.WindowsSDKRevision = content[offset++];
            neh.WindowsSDKVersion = content[offset++];

            return neh;
        }
    }
}
