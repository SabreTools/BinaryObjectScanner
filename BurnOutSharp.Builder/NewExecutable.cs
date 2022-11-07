using System.IO;
using System.Linq;
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

            #region MS-DOS Stub

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data, offset);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            #endregion

            #region Executable Header

            // Try to parse the executable header
            var executableHeader = ParseExecutableHeader(data, offset);
            if (executableHeader == null)
                return null;

            // Set the executable header
            executable.Header = executableHeader;

            #endregion

            #region Segment Table

            // If the offset for the segment table doesn't exist
            int tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.SegmentTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the segment table
            var segmentTable = ParseSegmentTable(data, tableAddress, executableHeader.FileSegmentCount);
            if (segmentTable == null)
                return null;

            // Set the segment table
            executable.SegmentTable = segmentTable;

            #endregion

            #region Resource Table

            // If the offset for the segment table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.SegmentTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the resource table
            var resourceTable = ParseResourceTable(data, tableAddress, executableHeader.FileSegmentCount);
            if (resourceTable == null)
                return null;

            // Set the resource table
            executable.ResourceTable = resourceTable;

            #endregion

            // TODO: Complete NE parsing
            return executable;
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

        /// <summary>
        /// Parse a byte array into a resource table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="count">Number of resource table entries to read</param>
        /// <returns>Filled resource table on success, null on error</returns>
        private static ResourceTable ParseResourceTable(byte[] data, int offset, int count)
        {
            int initialOffset = offset;

            // TODO: Use marshalling here instead of building
            var resourceTable = new ResourceTable();

            resourceTable.AlignmentShiftCount = data.ReadUInt16(ref offset);
            resourceTable.ResourceTypes = new ResourceTypeInformationEntry[count];
            for (int i = 0; i < resourceTable.ResourceTypes.Length; i++)
            {
                var entry = new ResourceTypeInformationEntry();
                entry.TypeID = data.ReadUInt16(ref offset);
                entry.ResourceCount = data.ReadUInt16(ref offset);
                entry.Reserved = data.ReadUInt32(ref offset);
                entry.Resources = new ResourceTypeResourceEntry[entry.ResourceCount];
                for (int j = 0; j < entry.ResourceCount; j++)
                {
                    // TODO: Should we read and store the resource data?
                    var resource = new ResourceTypeResourceEntry();
                    resource.Offset = data.ReadUInt16(ref offset);
                    resource.Length = data.ReadUInt16(ref offset);
                    resource.FlagWord = (ResourceTypeResourceFlag)data.ReadUInt16(ref offset);
                    resource.ResourceID = data.ReadUInt16(ref offset);
                    resource.Reserved = data.ReadUInt32(ref offset);
                    entry.Resources[j] = resource;
                }
                resourceTable.ResourceTypes[i] = entry;
            }

            // Get the full list of unique string offsets
            var stringOffsets = resourceTable.ResourceTypes
                .Where(rt => rt.IsIntegerType() == false)
                .Select(rt => rt.TypeID)
                .Union(resourceTable.ResourceTypes
                    .SelectMany(rt => rt.Resources)
                    .Where(r => r.IsIntegerType() == false)
                    .Select(r => r.ResourceID))
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            // Populate the type and name string dictionary
            for (int i = 0; i < stringOffsets.Count; i++)
            {
                int stringOffset = stringOffsets[i] + initialOffset;
                var str = new ResourceTypeAndNameString();
                str.Length = data.ReadByte(ref stringOffset);
                str.Text = data.ReadBytes(ref stringOffset, str.Length);
                resourceTable.TypeAndNameStrings[stringOffsets[i]] = str;
            }

            return resourceTable;
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

            #region MS-DOS Stub

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            #endregion

            #region Executable Header

            // Try to parse the executable header
            var executableHeader = ParseExecutableHeader(data);
            if (executableHeader == null)
                return null;

            // Set the executable header
            executable.Header = executableHeader;

            #endregion

            #region Segment Table

            // If the offset for the segment table doesn't exist
            int tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.SegmentTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the segment table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var segmentTable = ParseSegmentTable(data, executableHeader.FileSegmentCount);
            if (segmentTable == null)
                return null;

            // Set the segment table
            executable.SegmentTable = segmentTable;

            #endregion

            #region Resource Table

            // If the offset for the segment table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.SegmentTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the resource table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var resourceTable = ParseResourceTable(data, executableHeader.FileSegmentCount);
            if (resourceTable == null)
                return null;

            // Set the resource table
            executable.ResourceTable = resourceTable;

            #endregion

            // TODO: Complete NE parsing
            return executable;
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

        /// <summary>
        /// Parse a Stream into a resource table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of resource table entries to read</param>
        /// <returns>Filled resource table on success, null on error</returns>
        private static ResourceTable ParseResourceTable(Stream data, int count)
        {
            long initialOffset = data.Position;

            // TODO: Use marshalling here instead of building
            var resourceTable = new ResourceTable();

            resourceTable.AlignmentShiftCount = data.ReadUInt16();
            resourceTable.ResourceTypes = new ResourceTypeInformationEntry[count];
            for (int i = 0; i < resourceTable.ResourceTypes.Length; i++)
            {
                var entry = new ResourceTypeInformationEntry();
                entry.TypeID = data.ReadUInt16();
                entry.ResourceCount = data.ReadUInt16();
                entry.Reserved = data.ReadUInt32();
                entry.Resources = new ResourceTypeResourceEntry[entry.ResourceCount];
                for (int j = 0; j < entry.ResourceCount; j++)
                {
                    // TODO: Should we read and store the resource data?
                    var resource = new ResourceTypeResourceEntry();
                    resource.Offset = data.ReadUInt16();
                    resource.Length = data.ReadUInt16();
                    resource.FlagWord = (ResourceTypeResourceFlag)data.ReadUInt16();
                    resource.ResourceID = data.ReadUInt16();
                    resource.Reserved = data.ReadUInt32();
                    entry.Resources[j] = resource;
                }
                resourceTable.ResourceTypes[i] = entry;
            }

            // Get the full list of unique string offsets
            var stringOffsets = resourceTable.ResourceTypes
                .Where(rt => rt.IsIntegerType() == false)
                .Select(rt => rt.TypeID)
                .Union(resourceTable.ResourceTypes
                    .SelectMany(rt => rt.Resources)
                    .Where(r => r.IsIntegerType() == false)
                    .Select(r => r.ResourceID))
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            // Populate the type and name string dictionary
            for (int i = 0; i < stringOffsets.Count; i++)
            {
                int stringOffset = (int)(stringOffsets[i] + initialOffset);
                data.Seek(stringOffset, SeekOrigin.Begin);
                var str = new ResourceTypeAndNameString();
                str.Length = data.ReadByteValue();
                str.Text = data.ReadBytes(str.Length);
                resourceTable.TypeAndNameStrings[stringOffsets[i]] = str;
            }

            return resourceTable;
        }

        #endregion
    }
}