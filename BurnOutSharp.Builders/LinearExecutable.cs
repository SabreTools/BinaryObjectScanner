using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.LinearExecutable;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.LinearExecutable.Constants;

namespace BurnOutSharp.Builders
{
    public static class LinearExecutable
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Linear Executable
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
        /// Parse a Stream into a Linear Executable
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

            #region Information Block

            // Try to parse the executable header
            data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
            var informationBlock = ParseInformationBlock(data);
            if (informationBlock == null)
                return null;

            // Set the executable header
            executable.InformationBlock = informationBlock;

            #endregion

            #region Object Table

            // Get the object table offset
            long offset = informationBlock.ObjectTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the object table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the object table
                executable.ObjectTable = new ObjectTableEntry[informationBlock.ObjectTableCount];

                // Try to parse the object table
                for (int i = 0; i < executable.ObjectTable.Length; i++)
                {
                    var entry = ParseObjectTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ObjectTable[i] = entry;
                }
            }

            #endregion

            #region Object Page Map

            // Get the object page map offset
            offset = informationBlock.ObjectPageMapOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the object page map
                data.Seek(offset, SeekOrigin.Begin);

                // Create the object page map
                executable.ObjectPageMap = new ObjectPageMapEntry[informationBlock.ObjectTableCount];

                // Try to parse the object page map
                for (int i = 0; i < executable.ObjectPageMap.Length; i++)
                {
                    var entry = ParseObjectPageMapEntry(data);
                    if (entry == null)
                        return null;

                    executable.ObjectPageMap[i] = entry;
                }
            }

            #endregion

            #region Object Iterate Data Map

            offset = informationBlock.ObjectIterateDataMapOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the object page map
                data.Seek(offset, SeekOrigin.Begin);

                // TODO: Implement when model found
                // No model has been found in the documentation about what
                // each of the entries looks like for this map.
            }

            #endregion

            #region Resource Table

            // Get the resource table offset
            offset = informationBlock.ResourceTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the resource table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the resource table
                executable.ResourceTable = new ResourceTableEntry[informationBlock.ResourceTableCount];

                // Try to parse the resource table
                for (int i = 0; i < executable.ResourceTable.Length; i++)
                {
                    var entry = ParseResourceTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ResourceTable[i] = entry;
                }
            }

            #endregion

            #region Resident Names Table

            // Get the resident names table offset
            offset = informationBlock.ResidentNamesTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the resident names table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the resident names table
                var residentNamesTable = new List<ResidentNamesTableEntry>();

                // Try to parse the resident names table
                while (true)
                {
                    var entry = ParseResidentNamesTableEntry(data);
                    residentNamesTable.Add(entry);

                    // If we have a 0-length entry
                    if (entry.Length == 0)
                        break;
                }

                // Assign the resident names table
                executable.ResidentNamesTable = residentNamesTable.ToArray();
            }

            #endregion

            #region Entry Table

            // Get the entry table offset
            offset = informationBlock.EntryTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the entry table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the entry table
                var entryTable = new List<EntryTableBundle>();

                // Try to parse the entry table
                while (true)
                {
                    var bundle = ParseEntryTableBundle(data);
                    entryTable.Add(bundle);

                    // If we have a 0-length entry
                    if (bundle.Entries == 0)
                        break;
                }

                // Assign the entry table
                executable.EntryTable = entryTable.ToArray();
            }

            #endregion

            #region Module Format Directives Table

            // Get the module format directives table offset
            offset = informationBlock.ModuleDirectivesTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the module format directives table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the module format directives table
                executable.ModuleFormatDirectivesTable = new ModuleFormatDirectivesTableEntry[informationBlock.ModuleDirectivesCount];

                // Try to parse the module format directives table
                for (int i = 0; i < executable.ModuleFormatDirectivesTable.Length; i++)
                {
                    var entry = ParseModuleFormatDirectivesTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ModuleFormatDirectivesTable[i] = entry;
                }
            }

            #endregion

            #region Verify Record Directive Table

            // TODO: Figure out where the offset to this table is stored
            // The documentation suggests it's either part of or immediately following
            // the Module Format Directives Table

            #endregion

            #region Fix-up Page Table

            // Get the fix-up page table offset
            offset = informationBlock.FixupPageTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the fix-up page table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the fix-up page table
                executable.FixupPageTable = new FixupPageTableEntry[executable.ObjectPageMap.Length + 1];

                // Try to parse the fix-up page table
                for (int i = 0; i < executable.FixupPageTable.Length; i++)
                {
                    var entry = ParseFixupPageTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.FixupPageTable[i] = entry;
                }
            }

            #endregion

            #region Fix-up Record Table

            // Get the fix-up record table offset
            offset = informationBlock.FixupRecordTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the fix-up record table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the fix-up record table
                executable.FixupRecordTable = new FixupRecordTableEntry[executable.ObjectPageMap.Length + 1];

                // Try to parse the fix-up record table
                for (int i = 0; i < executable.FixupRecordTable.Length; i++)
                {
                    var entry = ParseFixupRecordTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.FixupRecordTable[i] = entry;
                }
            }

            #endregion

            #region Imported Module Name Table

            // Get the imported module name table offset
            offset = informationBlock.ImportedModulesNameTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the imported module name table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the imported module name table
                executable.ImportModuleNameTable = new ImportModuleNameTableEntry[informationBlock.ImportedModulesCount];

                // Try to parse the imported module name table
                for (int i = 0; i < executable.ImportModuleNameTable.Length; i++)
                {
                    var entry = ParseImportModuleNameTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ImportModuleNameTable[i] = entry;
                }
            }

            #endregion

            #region Imported Module Procedure Name Table

            // Get the imported module procedure name table offset
            offset = informationBlock.ImportProcedureNameTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the imported module procedure name table
                data.Seek(offset, SeekOrigin.Begin);

                // Get the size of the imported module procedure name table
                long tableSize = informationBlock.FixupPageTableOffset
                    + informationBlock.FixupSectionSize
                    - informationBlock.ImportProcedureNameTableOffset;

                // Create the imported module procedure name table
                var importModuleProcedureNameTable = new List<ImportModuleProcedureNameTableEntry>();

                // Try to parse the imported module procedure name table
                while (data.Position < offset + tableSize)
                {
                    var entry = ParseImportModuleProcedureNameTableEntry(data);
                    if (entry == null)
                        return null;

                    importModuleProcedureNameTable.Add(entry);
                }

                // Assign the resident names table
                executable.ImportModuleProcedureNameTable = importModuleProcedureNameTable.ToArray();
            }

            #endregion

            #region Per-Page Checksum Table

            // Get the per-page checksum table offset
            offset = informationBlock.PerPageChecksumTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the per-page checksum name table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the per-page checksum name table
                executable.PerPageChecksumTable = new PerPageChecksumTableEntry[informationBlock.ModuleNumberPages];

                // Try to parse the per-page checksum name table
                for (int i = 0; i < executable.PerPageChecksumTable.Length; i++)
                {
                    var entry = ParsePerPageChecksumTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.PerPageChecksumTable[i] = entry;
                }
            }

            #endregion

            #region Non-Resident Names Table

            // Get the non-resident names table offset
            offset = informationBlock.NonResidentNamesTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the non-resident names table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the non-resident names table
                var nonResidentNamesTable = new List<NonResidentNamesTableEntry>();

                // Try to parse the non-resident names table
                while (true)
                {
                    var entry = ParseNonResidentNameTableEntry(data);
                    nonResidentNamesTable.Add(entry);

                    // If we have a 0-length entry
                    if (entry.Length == 0)
                        break;
                }

                // Assign the non-resident names table
                executable.NonResidentNamesTable = nonResidentNamesTable.ToArray();
            }

            #endregion

            #region Debug Information

            // Get the debug information offset
            offset = informationBlock.NonResidentNamesTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the debug information
                data.Seek(offset, SeekOrigin.Begin);

                // Try to parse the debug information
                var debugInformation = ParseDebugInformation(data, informationBlock.DebugInformationLength);
                if (debugInformation == null)
                    return null;

                // Set the debug information
                executable.DebugInformation = debugInformation;
            }

            #endregion

            return executable;
        }

        /// <summary>
        /// Parse a Stream into an information block
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled information block on success, null on error</returns>
        private static InformationBlock ParseInformationBlock(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var informationBlock = new InformationBlock();

            byte[] magic = data.ReadBytes(2);
            informationBlock.Signature = Encoding.ASCII.GetString(magic);
            if (informationBlock.Signature != LESignatureString && informationBlock.Signature != LXSignatureString)
                return null;

            informationBlock.ByteOrder = (ByteOrder)data.ReadByteValue();
            informationBlock.WordOrder = (WordOrder)data.ReadByteValue();
            informationBlock.ExecutableFormatLevel = data.ReadUInt32();
            informationBlock.CPUType = (CPUType)data.ReadUInt16();
            informationBlock.ModuleOS = (OperatingSystem)data.ReadUInt16();
            informationBlock.ModuleVersion = data.ReadUInt32();
            informationBlock.ModuleTypeFlags = (ModuleFlags)data.ReadUInt32();
            informationBlock.ModuleNumberPages = data.ReadUInt32();
            informationBlock.InitialObjectCS = data.ReadUInt32();
            informationBlock.InitialEIP = data.ReadUInt32();
            informationBlock.InitialObjectSS = data.ReadUInt32();
            informationBlock.InitialESP = data.ReadUInt32();
            informationBlock.MemoryPageSize = data.ReadUInt32();
            informationBlock.BytesOnLastPage = data.ReadUInt32();
            informationBlock.FixupSectionSize = data.ReadUInt32();
            informationBlock.FixupSectionChecksum = data.ReadUInt32();
            informationBlock.LoaderSectionSize = data.ReadUInt32();
            informationBlock.LoaderSectionChecksum = data.ReadUInt32();
            informationBlock.ObjectTableOffset = data.ReadUInt32();
            informationBlock.ObjectTableCount = data.ReadUInt32();
            informationBlock.ObjectPageMapOffset = data.ReadUInt32();
            informationBlock.ObjectIterateDataMapOffset = data.ReadUInt32();
            informationBlock.ResourceTableOffset = data.ReadUInt32();
            informationBlock.ResourceTableCount = data.ReadUInt32();
            informationBlock.ResidentNamesTableOffset = data.ReadUInt32();
            informationBlock.EntryTableOffset = data.ReadUInt32();
            informationBlock.ModuleDirectivesTableOffset = data.ReadUInt32();
            informationBlock.ModuleDirectivesCount = data.ReadUInt32();
            informationBlock.FixupPageTableOffset = data.ReadUInt32();
            informationBlock.FixupRecordTableOffset = data.ReadUInt32();
            informationBlock.ImportedModulesNameTableOffset = data.ReadUInt32();
            informationBlock.ImportedModulesCount = data.ReadUInt32();
            informationBlock.ImportProcedureNameTableOffset = data.ReadUInt32();
            informationBlock.PerPageChecksumTableOffset = data.ReadUInt32();
            informationBlock.DataPagesOffset = data.ReadUInt32();
            informationBlock.PreloadPageCount = data.ReadUInt32();
            informationBlock.NonResidentNamesTableOffset = data.ReadUInt32();
            informationBlock.NonResidentNamesTableLength = data.ReadUInt32();
            informationBlock.NonResidentNamesTableChecksum = data.ReadUInt32();
            informationBlock.AutomaticDataObject = data.ReadUInt32();
            informationBlock.DebugInformationOffset = data.ReadUInt32();
            informationBlock.DebugInformationLength = data.ReadUInt32();
            informationBlock.PreloadInstancePagesNumber = data.ReadUInt32();
            informationBlock.DemandInstancePagesNumber = data.ReadUInt32();
            informationBlock.ExtraHeapAllocation = data.ReadUInt32();

            return informationBlock;
        }

        /// <summary>
        /// Parse a Stream into an object table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled object table entry on success, null on error</returns>
        private static ObjectTableEntry ParseObjectTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ObjectTableEntry();

            entry.VirtualSegmentSize = data.ReadUInt32();
            entry.RelocationBaseAddress = data.ReadUInt32();
            entry.ObjectFlags = (ObjectFlags)data.ReadUInt16();
            entry.PageTableIndex = data.ReadUInt32();
            entry.PageTableEntries = data.ReadUInt32();
            entry.Reserved = data.ReadUInt32();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into an object page map entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled object page map entry on success, null on error</returns>
        private static ObjectPageMapEntry ParseObjectPageMapEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ObjectPageMapEntry();

            entry.PageDataOffset = data.ReadUInt32();
            entry.DataSize = data.ReadUInt16();
            entry.Flags = (ObjectPageFlags)data.ReadUInt16();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a resource table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled resource table entry on success, null on error</returns>
        private static ResourceTableEntry ParseResourceTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ResourceTableEntry();

            entry.TypeID = (ResourceTableEntryType)data.ReadUInt32();
            entry.NameID = data.ReadUInt16();
            entry.ResourceSize = data.ReadUInt32();
            entry.ObjectNumber = data.ReadUInt16();
            entry.Offset = data.ReadUInt32();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a resident names table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled resident names table entry on success, null on error</returns>
        private static ResidentNamesTableEntry ParseResidentNamesTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ResidentNamesTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[] name = data.ReadBytes(entry.Length);
                entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }
            entry.OrdinalNumber = data.ReadUInt16();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into an entry table bundle
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled entry table bundle on success, null on error</returns>
        private static EntryTableBundle ParseEntryTableBundle(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var bundle = new EntryTableBundle();

            bundle.Entries = data.ReadByteValue();
            if (bundle.Entries == 0)
                return bundle;

            bundle.BundleType = (BundleType)data.ReadByteValue();
            bundle.TableEntries = new EntryTableEntry[bundle.Entries];
            for (int i = 0; i < bundle.Entries; i++)
            {
                var entry = new EntryTableEntry();

                switch (bundle.BundleType & ~BundleType.ParameterTypingInformationPresent)
                {
                    case BundleType.UnusedEntry:
                        // Empty entry with no information
                        break;

                    case BundleType.SixteenBitEntry:
                        entry.SixteenBitObjectNumber = data.ReadUInt16();
                        entry.SixteenBitEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.SixteenBitOffset = data.ReadUInt16();
                        break;

                    case BundleType.TwoEightySixCallGateEntry:
                        entry.TwoEightySixObjectNumber = data.ReadUInt16();
                        entry.TwoEightySixEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.TwoEightySixOffset = data.ReadUInt16();
                        entry.TwoEightySixCallgate = data.ReadUInt16();
                        break;

                    case BundleType.ThirtyTwoBitEntry:
                        entry.ThirtyTwoBitObjectNumber = data.ReadUInt16();
                        entry.ThirtyTwoBitEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.ThirtyTwoBitOffset = data.ReadUInt32();
                        break;

                    case BundleType.ForwarderEntry:
                        entry.ForwarderReserved = data.ReadUInt16();
                        entry.ForwarderFlags = (ForwarderFlags)data.ReadByteValue();
                        entry.ForwarderModuleOrdinalNumber = data.ReadUInt16();
                        entry.ProcedureNameOffset = data.ReadUInt32();
                        entry.ImportOrdinalNumber = data.ReadUInt32();
                        break;

                    default:
                        return null;
                }

                bundle.TableEntries[i] = entry;
            }

            return bundle;
        }

        /// <summary>
        /// Parse a Stream into a module format directives table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled module format directives table entry on success, null on error</returns>
        private static ModuleFormatDirectivesTableEntry ParseModuleFormatDirectivesTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ModuleFormatDirectivesTableEntry();

            entry.DirectiveNumber = (DirectiveNumber)data.ReadUInt16();
            entry.DirectiveDataLength = data.ReadUInt16();
            entry.DirectiveDataOffset = data.ReadUInt32();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a verify record directive table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled verify record directive table entry on success, null on error</returns>
        private static VerifyRecordDirectiveTableEntry ParseVerifyRecordDirectiveTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new VerifyRecordDirectiveTableEntry();

            entry.EntryCount = data.ReadUInt16();
            entry.OrdinalIndex = data.ReadUInt16();
            entry.Version = data.ReadUInt16();
            entry.ObjectEntriesCount = data.ReadUInt16();
            entry.ObjectNumberInModule = data.ReadUInt16();
            entry.ObjectLoadBaseAddress = data.ReadUInt16();
            entry.ObjectVirtualAddressSize = data.ReadUInt16();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a fix-up page table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled fix-up page table entry on success, null on error</returns>
        private static FixupPageTableEntry ParseFixupPageTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new FixupPageTableEntry();

            entry.Offset = data.ReadUInt32();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a fix-up record table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled fix-up record table entry on success, null on error</returns>
        private static FixupRecordTableEntry ParseFixupRecordTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new FixupRecordTableEntry();

            entry.SourceType = (FixupRecordSourceType)data.ReadByteValue();
            entry.TargetFlags = (FixupRecordTargetFlags)data.ReadByteValue();

            // Source list flag
            if (entry.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
                entry.SourceOffsetListCount = data.ReadByteValue();
            else
                entry.SourceOffset = data.ReadUInt16();

            // OBJECT / TRGOFF
            if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReference))
            {
                // 16-bit Object Number/Module Ordinal Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                    entry.TargetObjectNumberWORD = data.ReadUInt16();
                else
                    entry.TargetObjectNumberByte = data.ReadByteValue();

                // 16-bit Selector fixup
                if (!entry.SourceType.HasFlag(FixupRecordSourceType.SixteenBitSelectorFixup))
                {
                    // 32-bit Target Offset Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                        entry.TargetOffsetDWORD = data.ReadUInt32();
                    else
                        entry.TargetOffsetWORD = data.ReadUInt16();
                }
            }

            // MOD ORD# / IMPORT ORD / ADDITIVE
            else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByOrdinal))
            {
                // 16-bit Object Number/Module Ordinal Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                    entry.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16();
                else
                    entry.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // 8-bit Ordinal Flag & 32-bit Target Offset Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.EightBitOrdinalFlag))
                    entry.ImportedOrdinalNumberByte = data.ReadByteValue();
                else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                    entry.ImportedOrdinalNumberDWORD = data.ReadUInt32();
                else
                    entry.ImportedOrdinalNumberWORD = data.ReadUInt16();

                // Additive Fixup Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
                {
                    // 32-bit Additive Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                        entry.AdditiveFixupValueDWORD = data.ReadUInt32();
                    else
                        entry.AdditiveFixupValueWORD = data.ReadUInt16();
                }
            }

            // MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE
            else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByName))
            {
                // 16-bit Object Number/Module Ordinal Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                    entry.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16();
                else
                    entry.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // 32-bit Target Offset Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                    entry.OffsetImportProcedureNameTableDWORD = data.ReadUInt32();
                else
                    entry.OffsetImportProcedureNameTableWORD = data.ReadUInt16();

                // Additive Fixup Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
                {
                    // 32-bit Additive Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                        entry.AdditiveFixupValueDWORD = data.ReadUInt32();
                    else
                        entry.AdditiveFixupValueWORD = data.ReadUInt16();
                }
            }

            // ORD # / ADDITIVE
            else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReferenceViaEntryTable))
            {
                // 16-bit Object Number/Module Ordinal Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                    entry.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16();
                else
                    entry.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // Additive Fixup Flag
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
                {
                    // 32-bit Additive Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                        entry.AdditiveFixupValueDWORD = data.ReadUInt32();
                    else
                        entry.AdditiveFixupValueWORD = data.ReadUInt16();
                }
            }

            // No other top-level flags recognized
            else
            {
                return null;
            }

            #region SCROFFn

            if (entry.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
            {
                entry.SourceOffsetList = new ushort[entry.SourceOffsetListCount];
                for (int i = 0; i < entry.SourceOffsetList.Length; i++)
                {
                    entry.SourceOffsetList[i] = data.ReadUInt16();
                }
            }

            #endregion

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a import module name table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled import module name table entry on success, null on error</returns>
        private static ImportModuleNameTableEntry ParseImportModuleNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ImportModuleNameTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[] name = data.ReadBytes(entry.Length);
                entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a import module name table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled import module name table entry on success, null on error</returns>
        private static ImportModuleProcedureNameTableEntry ParseImportModuleProcedureNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ImportModuleProcedureNameTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[] name = data.ReadBytes(entry.Length);
                entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a per-page checksum table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled per-page checksum table entry on success, null on error</returns>
        private static PerPageChecksumTableEntry ParsePerPageChecksumTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new PerPageChecksumTableEntry();

            entry.Checksum = data.ReadUInt32();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a non-resident names table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled non-resident names table entry on success, null on error</returns>
        private static NonResidentNamesTableEntry ParseNonResidentNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new NonResidentNamesTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[] name = data.ReadBytes(entry.Length);
                entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }
            entry.OrdinalNumber = data.ReadUInt16();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a debug information
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="size">Total size of the debug information</param>
        /// <returns>Filled debug information on success, null on error</returns>
        private static DebugInformation ParseDebugInformation(Stream data, long size)
        {
            // TODO: Use marshalling here instead of building
            var debugInformation = new DebugInformation();

            byte[] signature = data.ReadBytes(3);
            debugInformation.Signature = Encoding.ASCII.GetString(signature);
            if (debugInformation.Signature != DebugInformationSignatureString)
                return null;

            debugInformation.FormatType = (DebugFormatType)data.ReadByteValue();
            debugInformation.DebuggerData = data.ReadBytes((int)(size - 4));

            return debugInformation;
        }

        #endregion
    }
}