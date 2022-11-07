﻿using System.IO;
using BurnOutSharp.Models.NewExecutable;

namespace BurnOutSharp.Builder
{
    // TODO: Make Stream Data rely on Byte Data
    public static class NewExecutable
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a New Executable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = offset;

            // Create a new executable to fill
            var executable = new Executable();

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data, offset);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            // Try to parse the executable header
            var executableHeader = ParseExecutableHeader(data, offset);
            if (executableHeader == null)
                return null;

            // Set the executable header
            executable.Header = executableHeader;

            // If the offset for the segment table doesn't exist
            int tableAddress = initialOffset + executableHeader.SegmentTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the segment table
            var relocationTable = ParseSegmentTable(data, tableAddress, executableHeader.FileSegmentCount);
            if (relocationTable == null)
                return null;

            // Set the segment table
            executable.SegmentTable = relocationTable;

            // TODO: Implement NE parsing
            return null;
        }

        /// <summary>
        /// Parse a byte array into a New Executable header
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled executable header on success, null on error</returns>
        private static ExecutableHeader ParseExecutableHeader(byte[] data, int offset)
        {
            // TODO: Use marshalling here instead of building
            var header = new ExecutableHeader();

            header.Magic = new char[2];
            for (int i = 0; i < header.Magic.Length; i++)
            {
                header.Magic[i] = data.ReadChar(ref offset);
            }
            if (header.Magic[0] != 'N' || header.Magic[1] != 'E')
                return null;

            header.LinkerVersion = data.ReadByte(ref offset);
            header.LinkerRevision = data.ReadByte(ref offset);
            header.EntryTableOffset = data.ReadUInt16(ref offset);
            header.EntryTableSize = data.ReadUInt16(ref offset);
            header.CrcChecksum = data.ReadUInt32(ref offset);
            header.FlagWord = (HeaderFlag)data.ReadUInt16(ref offset);
            header.AutomaticDataSegmentNumber = data.ReadUInt16(ref offset);
            header.InitialHeapAlloc = data.ReadUInt16(ref offset);
            header.InitialStackAlloc = data.ReadUInt16(ref offset);
            header.InitialCSIPSetting = data.ReadUInt32(ref offset);
            header.InitialSSSPSetting = data.ReadUInt32(ref offset);
            header.FileSegmentCount = data.ReadUInt16(ref offset);
            header.ModuleReferenceTableSize = data.ReadUInt16(ref offset);
            header.NonResidentNameTableSize = data.ReadUInt16(ref offset);
            header.SegmentTableOffset = data.ReadUInt16(ref offset);
            header.ResourceTableOffset = data.ReadUInt16(ref offset);
            header.ResidentNameTableOffset = data.ReadUInt16(ref offset);
            header.ModuleReferenceTableOffset = data.ReadUInt16(ref offset);
            header.ImportedNamesTableOffset = data.ReadUInt16(ref offset);
            header.NonResidentNamesTableOffset = data.ReadUInt32(ref offset);
            header.MovableEntriesCount = data.ReadUInt16(ref offset);
            header.SegmentAlignmentShiftCount = data.ReadUInt16(ref offset);
            header.ResourceEntriesCount = data.ReadUInt16(ref offset);
            header.TargetOperatingSystem = (OperatingSystem)data.ReadByte(ref offset);
            header.AdditionalFlags = (OS2Flag)data.ReadByte(ref offset);
            header.ReturnThunkOffset = data.ReadUInt16(ref offset);
            header.SegmentReferenceThunkOffset = data.ReadUInt16(ref offset);
            header.MinCodeSwapAreaSize = data.ReadUInt16(ref offset);
            header.WindowsSDKRevision = data.ReadByte(ref offset);
            header.WindowsSDKVersion = data.ReadByte(ref offset);

            return header;
        }

        /// <summary>
        /// Parse a byte array into a segment table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="count">Number of segment table entries to read</param>
        /// <returns>Filled segment table on success, null on error</returns>
        private static SegmentTableEntry[] ParseSegmentTable(byte[] data, int offset, int count)
        {
            // TODO: Use marshalling here instead of building
            var segmentTable = new SegmentTableEntry[count];
            
            for (int i = 0; i < count; i++)
            {
                var entry = new SegmentTableEntry();
                entry.Offset = data.ReadUInt16(ref offset);
                entry.Length = data.ReadUInt16(ref offset);
                entry.FlagWord = (SegmentTableEntryFlag)data.ReadUInt16(ref offset);
                entry.MinimumAllocationSize = data.ReadUInt16(ref offset);
                segmentTable[i] = entry;
            }

            return segmentTable;
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a New Executable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(Stream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new executable to fill
            var executable = new Executable();

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            // Try to parse the executable header
            var executableHeader = ParseExecutableHeader(data);
            if (executableHeader == null)
                return null;

            // Set the executable header
            executable.Header = executableHeader;

            // If the offset for the segment table doesn't exist
            int tableAddress = initialOffset + executableHeader.SegmentTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the segment table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var relocationTable = ParseSegmentTable(data, executableHeader.FileSegmentCount);
            if (relocationTable == null)
                return null;

            // Set the segment table
            executable.SegmentTable = relocationTable;

            // TODO: Implement NE parsing
            return null;
        }

        /// <summary>
        /// Parse a Stream into a New Executable header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable header on success, null on error</returns>
        private static ExecutableHeader ParseExecutableHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var header = new ExecutableHeader();

            header.Magic = new char[2];
            for (int i = 0; i < header.Magic.Length; i++)
            {
                header.Magic[i] = data.ReadChar();
            }
            if (header.Magic[0] != 'N' || header.Magic[1] != 'E')
                return null;

            header.LinkerVersion = data.ReadByteValue();
            header.LinkerRevision = data.ReadByteValue();
            header.EntryTableOffset = data.ReadUInt16();
            header.EntryTableSize = data.ReadUInt16();
            header.CrcChecksum = data.ReadUInt32();
            header.FlagWord = (HeaderFlag)data.ReadUInt16();
            header.AutomaticDataSegmentNumber = data.ReadUInt16();
            header.InitialHeapAlloc = data.ReadUInt16();
            header.InitialStackAlloc = data.ReadUInt16();
            header.InitialCSIPSetting = data.ReadUInt32();
            header.InitialSSSPSetting = data.ReadUInt32();
            header.FileSegmentCount = data.ReadUInt16();
            header.ModuleReferenceTableSize = data.ReadUInt16();
            header.NonResidentNameTableSize = data.ReadUInt16();
            header.SegmentTableOffset = data.ReadUInt16();
            header.ResourceTableOffset = data.ReadUInt16();
            header.ResidentNameTableOffset = data.ReadUInt16();
            header.ModuleReferenceTableOffset = data.ReadUInt16();
            header.ImportedNamesTableOffset = data.ReadUInt16();
            header.NonResidentNamesTableOffset = data.ReadUInt32();
            header.MovableEntriesCount = data.ReadUInt16();
            header.SegmentAlignmentShiftCount = data.ReadUInt16();
            header.ResourceEntriesCount = data.ReadUInt16();
            header.TargetOperatingSystem = (OperatingSystem)data.ReadByteValue();
            header.AdditionalFlags = (OS2Flag)data.ReadByteValue();
            header.ReturnThunkOffset = data.ReadUInt16();
            header.SegmentReferenceThunkOffset = data.ReadUInt16();
            header.MinCodeSwapAreaSize = data.ReadUInt16();
            header.WindowsSDKRevision = data.ReadByteValue();
            header.WindowsSDKVersion = data.ReadByteValue();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a segment table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of segment table entries to read</param>
        /// <returns>Filled segment table on success, null on error</returns>
        private static SegmentTableEntry[] ParseSegmentTable(Stream data, int count)
        {
            // TODO: Use marshalling here instead of building
            var segmentTable = new SegmentTableEntry[count];

            for (int i = 0; i < count; i++)
            {
                var entry = new SegmentTableEntry();
                entry.Offset = data.ReadUInt16();
                entry.Length = data.ReadUInt16();
                entry.FlagWord = (SegmentTableEntryFlag)data.ReadUInt16();
                entry.MinimumAllocationSize = data.ReadUInt16();
                segmentTable[i] = entry;
            }

            return segmentTable;
        }

        #endregion
    }
}