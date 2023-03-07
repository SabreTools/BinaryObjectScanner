using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Models.NewExecutable;
using BinaryObjectScanner.Utilities;
using static BurnOutSharp.Models.NewExecutable.Constants;

namespace BurnOutSharp.Builders
{
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

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseExecutable(dataStream);
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
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
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
            data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
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
            var resourceTable = ParseResourceTable(data, executableHeader.ResourceEntriesCount);
            if (resourceTable == null)
                return null;

            // Set the resource table
            executable.ResourceTable = resourceTable;

            #endregion

            #region Resident-Name Table

            // If the offset for the resident-name table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ResidentNameTableOffset;
            int endOffset = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ModuleReferenceTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the resident-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var residentNameTable = ParseResidentNameTable(data, endOffset);
            if (residentNameTable == null)
                return null;

            // Set the resident-name table
            executable.ResidentNameTable = residentNameTable;

            #endregion

            #region Module-Reference Table

            // If the offset for the module-reference table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ModuleReferenceTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the module-reference table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var moduleReferenceTable = ParseModuleReferenceTable(data, executableHeader.ModuleReferenceTableSize);
            if (moduleReferenceTable == null)
                return null;

            // Set the module-reference table
            executable.ModuleReferenceTable = moduleReferenceTable;

            #endregion

            #region Imported-Name Table

            // If the offset for the imported-name table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ImportedNamesTableOffset;
            endOffset = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.EntryTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the imported-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var importedNameTable = ParseImportedNameTable(data, endOffset);
            if (importedNameTable == null)
                return null;

            // Set the imported-name table
            executable.ImportedNameTable = importedNameTable;

            #endregion

            #region Entry Table

            // If the offset for the imported-name table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.EntryTableOffset;
            endOffset = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.EntryTableOffset
                + executableHeader.EntryTableSize;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the imported-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var entryTable = ParseEntryTable(data, endOffset);
            if (entryTable == null)
                return null;

            // Set the entry table
            executable.EntryTable = entryTable;

            #endregion

            #region Nonresident-Name Table

            // If the offset for the nonresident-name table doesn't exist
            tableAddress = initialOffset
                + (int)executableHeader.NonResidentNamesTableOffset;
            endOffset = initialOffset
                + (int)executableHeader.NonResidentNamesTableOffset
                + executableHeader.NonResidentNameTableSize;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the nonresident-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var nonResidentNameTable = ParseNonResidentNameTable(data, endOffset);
            if (nonResidentNameTable == null)
                return null;

            // Set the nonresident-name table
            executable.NonResidentNameTable = nonResidentNameTable;

            #endregion

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

            byte[] magic = data.ReadBytes(2);
            header.Magic = Encoding.ASCII.GetString(magic);
            if (header.Magic != SignatureString)
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
            resourceTable.TypeAndNameStrings = new Dictionary<ushort, ResourceTypeAndNameString>();
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

        /// <summary>
        /// Parse a Stream into a resident-name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the resident-name table</param>
        /// <returns>Filled resident-name table on success, null on error</returns>
        private static ResidentNameTableEntry[] ParseResidentNameTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var residentNameTable = new List<ResidentNameTableEntry>();

            while (data.Position < endOffset)
            {
                var entry = new ResidentNameTableEntry();
                entry.Length = data.ReadByteValue();
                entry.NameString = data.ReadBytes(entry.Length);
                entry.OrdinalNumber = data.ReadUInt16();
                residentNameTable.Add(entry);
            }

            return residentNameTable.ToArray();
        }

        /// <summary>
        /// Parse a Stream into a module-reference table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of module-reference table entries to read</param>
        /// <returns>Filled module-reference table on success, null on error</returns>
        private static ModuleReferenceTableEntry[] ParseModuleReferenceTable(Stream data, int count)
        {
            // TODO: Use marshalling here instead of building
            var moduleReferenceTable = new ModuleReferenceTableEntry[count];

            for (int i = 0; i < count; i++)
            {
                var entry = new ModuleReferenceTableEntry();
                entry.Offset = data.ReadUInt16();
                moduleReferenceTable[i] = entry;
            }

            return moduleReferenceTable;
        }

        /// <summary>
        /// Parse a Stream into an imported-name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the imported-name table</param>
        /// <returns>Filled imported-name table on success, null on error</returns>
        private static Dictionary<ushort, ImportedNameTableEntry> ParseImportedNameTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var importedNameTable = new Dictionary<ushort, ImportedNameTableEntry>();

            while (data.Position < endOffset)
            {
                ushort currentOffset = (ushort)data.Position;
                var entry = new ImportedNameTableEntry();
                entry.Length = data.ReadByteValue();
                entry.NameString = data.ReadBytes(entry.Length);
                importedNameTable[currentOffset] = entry;
            }

            return importedNameTable;
        }

        /// <summary>
        /// Parse a Stream into an entry table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the entry table</param>
        /// <returns>Filled entry table on success, null on error</returns>
        private static EntryTableBundle[] ParseEntryTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var entryTable = new List<EntryTableBundle>();

            while (data.Position < endOffset)
            {
                var entry = new EntryTableBundle();
                entry.EntryCount = data.ReadByteValue();
                entry.SegmentIndicator = data.ReadByteValue();
                switch (entry.GetEntryType())
                {
                    case SegmentEntryType.Unused:
                        break;

                    case SegmentEntryType.FixedSegment:
                        entry.FixedFlagWord = (FixedSegmentEntryFlag)data.ReadByteValue();
                        entry.FixedOffset = data.ReadUInt16();
                        break;

                    case SegmentEntryType.MoveableSegment:
                        entry.MoveableFlagWord = (MoveableSegmentEntryFlag)data.ReadByteValue();
                        entry.MoveableReserved = data.ReadUInt16();
                        entry.MoveableSegmentNumber = data.ReadByteValue();
                        entry.MoveableOffset = data.ReadUInt16();
                        break;
                }
                entryTable.Add(entry);
            }

            return entryTable.ToArray();
        }

        /// <summary>
        /// Parse a Stream into a nonresident-name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the nonresident-name table</param>
        /// <returns>Filled nonresident-name table on success, null on error</returns>
        private static NonResidentNameTableEntry[] ParseNonResidentNameTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var residentNameTable = new List<NonResidentNameTableEntry>();

            while (data.Position < endOffset)
            {
                var entry = new NonResidentNameTableEntry();
                entry.Length = data.ReadByteValue();
                entry.NameString = data.ReadBytes(entry.Length);
                entry.OrdinalNumber = data.ReadUInt16();
                residentNameTable.Add(entry);
            }

            return residentNameTable.ToArray();
        }

        #endregion
    }
}