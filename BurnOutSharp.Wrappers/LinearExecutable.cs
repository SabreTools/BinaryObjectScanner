using System;
using System.IO;
using System.Text;

namespace BurnOutSharp.Wrappers
{
    public class LinearExecutable : WrapperBase
    {
        #region Pass-Through Properties

        #region MS-DOS Stub

        #region Standard Fields

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
        public string Stub_Magic => _executable.Stub.Header.Magic;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
        public ushort Stub_LastPageBytes => _executable.Stub.Header.LastPageBytes;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
        public ushort Stub_Pages => _executable.Stub.Header.Pages;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
        public ushort Stub_RelocationItems => _executable.Stub.Header.RelocationItems;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
        public ushort Stub_HeaderParagraphSize => _executable.Stub.Header.HeaderParagraphSize;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
        public ushort Stub_MinimumExtraParagraphs => _executable.Stub.Header.MinimumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
        public ushort Stub_MaximumExtraParagraphs => _executable.Stub.Header.MaximumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
        public ushort Stub_InitialSSValue => _executable.Stub.Header.InitialSSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
        public ushort Stub_InitialSPValue => _executable.Stub.Header.InitialSPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
        public ushort Stub_Checksum => _executable.Stub.Header.Checksum;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
        public ushort Stub_InitialIPValue => _executable.Stub.Header.InitialIPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
        public ushort Stub_InitialCSValue => _executable.Stub.Header.InitialCSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
        public ushort Stub_RelocationTableAddr => _executable.Stub.Header.RelocationTableAddr;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
        public ushort Stub_OverlayNumber => _executable.Stub.Header.OverlayNumber;

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
        public ushort[] Stub_Reserved1 => _executable.Stub.Header.Reserved1;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
        public ushort Stub_OEMIdentifier => _executable.Stub.Header.OEMIdentifier;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
        public ushort Stub_OEMInformation => _executable.Stub.Header.OEMInformation;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
        public ushort[] Stub_Reserved2 => _executable.Stub.Header.Reserved2;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
        public uint Stub_NewExeHeaderAddr => _executable.Stub.Header.NewExeHeaderAddr;

        #endregion

        #endregion

        #region Information Block

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.Signature"/>
        public string Signature => _executable.InformationBlock.Signature;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ByteOrder"/>
        public Models.LinearExecutable.ByteOrder ByteOrder => _executable.InformationBlock.ByteOrder;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.WordOrder"/>
        public Models.LinearExecutable.WordOrder WordOrder => _executable.InformationBlock.WordOrder;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ExecutableFormatLevel"/>
        public uint ExecutableFormatLevel => _executable.InformationBlock.ExecutableFormatLevel;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.CPUType"/>
        public Models.LinearExecutable.CPUType CPUType => _executable.InformationBlock.CPUType;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleOS"/>
        public Models.LinearExecutable.OperatingSystem ModuleOS => _executable.InformationBlock.ModuleOS;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleVersion"/>
        public uint ModuleVersion => _executable.InformationBlock.ModuleVersion;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleTypeFlags"/>
        public Models.LinearExecutable.ModuleFlags ModuleTypeFlags => _executable.InformationBlock.ModuleTypeFlags;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleNumberPages"/>
        public uint ModuleNumberPages => _executable.InformationBlock.ModuleNumberPages;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialObjectCS"/>
        public uint InitialObjectCS => _executable.InformationBlock.InitialObjectCS;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialEIP"/>
        public uint InitialEIP => _executable.InformationBlock.InitialEIP;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialObjectSS"/>
        public uint InitialObjectSS => _executable.InformationBlock.InitialObjectSS;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialESP"/>
        public uint InitialESP => _executable.InformationBlock.InitialESP;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.MemoryPageSize"/>
        public uint MemoryPageSize => _executable.InformationBlock.MemoryPageSize;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.BytesOnLastPage"/>
        public uint BytesOnLastPage => _executable.InformationBlock.BytesOnLastPage;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupSectionSize"/>
        public uint FixupSectionSize => _executable.InformationBlock.FixupSectionSize;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupSectionChecksum"/>
        public uint FixupSectionChecksum => _executable.InformationBlock.FixupSectionChecksum;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.LoaderSectionSize"/>
        public uint LoaderSectionSize => _executable.InformationBlock.LoaderSectionSize;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.LoaderSectionChecksum"/>
        public uint LoaderSectionChecksum => _executable.InformationBlock.LoaderSectionChecksum;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectTableOffset"/>
        public uint ObjectTableOffset => _executable.InformationBlock.ObjectTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectTableCount"/>
        public uint ObjectTableCount => _executable.InformationBlock.ObjectTableCount;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectPageMapOffset"/>
        public uint ObjectPageMapOffset => _executable.InformationBlock.ObjectPageMapOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectIterateDataMapOffset"/>
        public uint ObjectIterateDataMapOffset => _executable.InformationBlock.ObjectIterateDataMapOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResourceTableOffset"/>
        public uint ResourceTableOffset => _executable.InformationBlock.ResourceTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResourceTableCount"/>
        public uint ResourceTableCount => _executable.InformationBlock.ResourceTableCount;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResidentNamesTableOffset"/>
        public uint ResidentNamesTableOffset => _executable.InformationBlock.ResidentNamesTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.EntryTableOffset"/>
        public uint EntryTableOffset => _executable.InformationBlock.EntryTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleDirectivesTableOffset"/>
        public uint ModuleDirectivesTableOffset => _executable.InformationBlock.ModuleDirectivesTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleDirectivesCount"/>
        public uint ModuleDirectivesCount => _executable.InformationBlock.ModuleDirectivesCount;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupPageTableOffset"/>
        public uint FixupPageTableOffset => _executable.InformationBlock.FixupPageTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupRecordTableOffset"/>
        public uint FixupRecordTableOffset => _executable.InformationBlock.FixupRecordTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportedModulesNameTableOffset"/>
        public uint ImportedModulesNameTableOffset => _executable.InformationBlock.ImportedModulesNameTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportedModulesCount"/>
        public uint ImportedModulesCount => _executable.InformationBlock.ImportedModulesCount;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportProcedureNameTableOffset"/>
        public uint ImportProcedureNameTableOffset => _executable.InformationBlock.ImportProcedureNameTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PerPageChecksumTableOffset"/>
        public uint PerPageChecksumTableOffset => _executable.InformationBlock.PerPageChecksumTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DataPagesOffset"/>
        public uint DataPagesOffset => _executable.InformationBlock.DataPagesOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PreloadPageCount"/>
        public uint PreloadPageCount => _executable.InformationBlock.PreloadPageCount;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableOffset"/>
        public uint NonResidentNamesTableOffset => _executable.InformationBlock.NonResidentNamesTableOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableLength"/>
        public uint NonResidentNamesTableLength => _executable.InformationBlock.NonResidentNamesTableLength;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableChecksum"/>
        public uint NonResidentNamesTableChecksum => _executable.InformationBlock.NonResidentNamesTableChecksum;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.AutomaticDataObject"/>
        public uint AutomaticDataObject => _executable.InformationBlock.AutomaticDataObject;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DebugInformationOffset"/>
        public uint DebugInformationOffset => _executable.InformationBlock.DebugInformationOffset;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DebugInformationLength"/>
        public uint DebugInformationLength => _executable.InformationBlock.DebugInformationLength;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PreloadInstancePagesNumber"/>
        public uint PreloadInstancePagesNumber => _executable.InformationBlock.PreloadInstancePagesNumber;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DemandInstancePagesNumber"/>
        public uint DemandInstancePagesNumber => _executable.InformationBlock.DemandInstancePagesNumber;

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ExtraHeapAllocation"/>
        public uint ExtraHeapAllocation => _executable.InformationBlock.ExtraHeapAllocation;

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.LinearExecutable.ObjectTable"/>
        public Models.LinearExecutable.ObjectTableEntry[] ObjectTable => _executable.ObjectTable;

        /// <inheritdoc cref="Models.LinearExecutable.ObjectPageMap"/>
        public Models.LinearExecutable.ObjectPageMapEntry[] ObjectPageMap => _executable.ObjectPageMap;

        /// <inheritdoc cref="Models.LinearExecutable.ResourceTable"/>
        public Models.LinearExecutable.ResourceTableEntry[] ResourceTable => _executable.ResourceTable;

        /// <inheritdoc cref="Models.LinearExecutable.ResidentNamesTable"/>
        public Models.LinearExecutable.ResidentNamesTableEntry[] ResidentNamesTable => _executable.ResidentNamesTable;

        /// <inheritdoc cref="Models.LinearExecutable.EntryTable"/>
        public Models.LinearExecutable.EntryTableBundle[] EntryTable => _executable.EntryTable;

        /// <inheritdoc cref="Models.LinearExecutable.ModuleFormatDirectivesTable"/>
        public Models.LinearExecutable.ModuleFormatDirectivesTableEntry[] ModuleFormatDirectivesTable => _executable.ModuleFormatDirectivesTable;

        /// <inheritdoc cref="Models.LinearExecutable.VerifyRecordDirectiveTable"/>
        public Models.LinearExecutable.VerifyRecordDirectiveTableEntry[] VerifyRecordDirectiveTable => _executable.VerifyRecordDirectiveTable;

        /// <inheritdoc cref="Models.LinearExecutable.PerPageChecksumTable"/>
        public Models.LinearExecutable.PerPageChecksumTableEntry[] PerPageChecksumTable => _executable.PerPageChecksumTable;

        /// <inheritdoc cref="Models.LinearExecutable.FixupPageTable"/>
        public Models.LinearExecutable.FixupPageTableEntry[] FixupPageTable => _executable.FixupPageTable;

        /// <inheritdoc cref="Models.LinearExecutable.FixupRecordTable"/>
        public Models.LinearExecutable.FixupRecordTableEntry[] FixupRecordTable => _executable.FixupRecordTable;

        /// <inheritdoc cref="Models.LinearExecutable.ImportModuleNameTable"/>
        public Models.LinearExecutable.ImportModuleNameTableEntry[] ImportModuleNameTable => _executable.ImportModuleNameTable;

        /// <inheritdoc cref="Models.LinearExecutable.ImportModuleProcedureNameTable"/>
        public Models.LinearExecutable.ImportModuleProcedureNameTableEntry[] ImportModuleProcedureNameTable => _executable.ImportModuleProcedureNameTable;

        /// <inheritdoc cref="Models.LinearExecutable.NonResidentNamesTable"/>
        public Models.LinearExecutable.NonResidentNamesTableEntry[] NonResidentNamesTable => _executable.NonResidentNamesTable;

        #endregion

        #region Debug Information

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.Signature"/>
        public string DI_Signature => _executable.DebugInformation?.Signature;

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.FormatType"/>
        public Models.LinearExecutable.DebugFormatType? DI_FormatType => _executable.DebugInformation?.FormatType;

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.DebuggerData"/>
        public byte[] DebuggerData => _executable.DebugInformation?.DebuggerData;

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Determine what extension properties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the executable
        /// </summary>
        private Models.LinearExecutable.Executable _executable;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private LinearExecutable() { }

        /// <summary>
        /// Create an LE/LX executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
        public static LinearExecutable Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an LE/LX executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
        public static LinearExecutable Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var executable = Builders.LinearExecutable.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new LinearExecutable
            {
                _executable = executable,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("New Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Stub
            PrintStubHeader(builder);
            PrintStubExtendedHeader(builder);

            // Information Block
            PrintInformationBlock(builder);

            // Tables
            PrintObjectTable(builder);
            PrintObjectPageMap(builder);
            PrintResourceTable(builder);
            PrintResidentNamesTable(builder);
            PrintEntryTable(builder);
            PrintModuleFormatDirectivesTable(builder);
            PrintVerifyRecordDirectiveTable(builder);
            PrintFixupPageTable(builder);
            PrintFixupRecordTable(builder);
            PrintImportModuleNameTable(builder);
            PrintImportModuleProcedureNameTable(builder);
            PrintPerPageChecksumTable(builder);
            PrintNonResidentNamesTable(builder);

            // Debug
            PrintDebugInformation(builder);

            return builder;
        }

        /// <summary>
        /// Print stub header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintStubHeader(StringBuilder builder)
        {
            builder.AppendLine("  MS-DOS Stub Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Magic number: {Stub_Magic}");
            builder.AppendLine($"  Last page bytes: {Stub_LastPageBytes} (0x{Stub_LastPageBytes:X})");
            builder.AppendLine($"  Pages: {Stub_Pages} (0x{Stub_Pages:X})");
            builder.AppendLine($"  Relocation items: {Stub_RelocationItems} (0x{Stub_RelocationItems:X})");
            builder.AppendLine($"  Header paragraph size: {Stub_HeaderParagraphSize} (0x{Stub_HeaderParagraphSize:X})");
            builder.AppendLine($"  Minimum extra paragraphs: {Stub_MinimumExtraParagraphs} (0x{Stub_MinimumExtraParagraphs:X})");
            builder.AppendLine($"  Maximum extra paragraphs: {Stub_MaximumExtraParagraphs} (0x{Stub_MaximumExtraParagraphs:X})");
            builder.AppendLine($"  Initial SS value: {Stub_InitialSSValue} (0x{Stub_InitialSSValue:X})");
            builder.AppendLine($"  Initial SP value: {Stub_InitialSPValue} (0x{Stub_InitialSPValue:X})");
            builder.AppendLine($"  Checksum: {Stub_Checksum} (0x{Stub_Checksum:X})");
            builder.AppendLine($"  Initial IP value: {Stub_InitialIPValue} (0x{Stub_InitialIPValue:X})");
            builder.AppendLine($"  Initial CS value: {Stub_InitialCSValue} (0x{Stub_InitialCSValue:X})");
            builder.AppendLine($"  Relocation table address: {Stub_RelocationTableAddr} (0x{Stub_RelocationTableAddr:X})");
            builder.AppendLine($"  Overlay number: {Stub_OverlayNumber} (0x{Stub_OverlayNumber:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print stub extended header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintStubExtendedHeader(StringBuilder builder)
        {
            builder.AppendLine("  MS-DOS Stub Extended Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Reserved words: {string.Join(", ", Stub_Reserved1)}");
            builder.AppendLine($"  OEM identifier: {Stub_OEMIdentifier} (0x{Stub_OEMIdentifier:X})");
            builder.AppendLine($"  OEM information: {Stub_OEMInformation} (0x{Stub_OEMInformation:X})");
            builder.AppendLine($"  Reserved words: {string.Join(", ", Stub_Reserved2)}");
            builder.AppendLine($"  New EXE header address: {Stub_NewExeHeaderAddr} (0x{Stub_NewExeHeaderAddr:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print information block information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintInformationBlock(StringBuilder builder)
        {
            builder.AppendLine("  Information Block Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Byte order: {ByteOrder} (0x{ByteOrder:X})");
            builder.AppendLine($"  Word order: {WordOrder} (0x{WordOrder:X})");
            builder.AppendLine($"  Executable format level: {ExecutableFormatLevel} (0x{ExecutableFormatLevel:X})");
            builder.AppendLine($"  CPU type: {CPUType} (0x{CPUType:X})");
            builder.AppendLine($"  Module OS: {ModuleOS} (0x{ModuleOS:X})");
            builder.AppendLine($"  Module version: {ModuleVersion} (0x{ModuleVersion:X})");
            builder.AppendLine($"  Module type flags: {ModuleTypeFlags} (0x{ModuleTypeFlags:X})");
            builder.AppendLine($"  Module number pages: {ModuleNumberPages} (0x{ModuleNumberPages:X})");
            builder.AppendLine($"  Initial object CS: {InitialObjectCS} (0x{InitialObjectCS:X})");
            builder.AppendLine($"  Initial EIP: {InitialEIP} (0x{InitialEIP:X})");
            builder.AppendLine($"  Initial object SS: {InitialObjectSS} (0x{InitialObjectSS:X})");
            builder.AppendLine($"  Initial ESP: {InitialESP} (0x{InitialESP:X})");
            builder.AppendLine($"  Memory page size: {MemoryPageSize} (0x{MemoryPageSize:X})");
            builder.AppendLine($"  Bytes on last page: {BytesOnLastPage} (0x{BytesOnLastPage:X})");
            builder.AppendLine($"  Fix-up section size: {FixupSectionSize} (0x{FixupSectionSize:X})");
            builder.AppendLine($"  Fix-up section checksum: {FixupSectionChecksum} (0x{FixupSectionChecksum:X})");
            builder.AppendLine($"  Loader section size: {LoaderSectionSize} (0x{LoaderSectionSize:X})");
            builder.AppendLine($"  Loader section checksum: {LoaderSectionChecksum} (0x{LoaderSectionChecksum:X})");
            builder.AppendLine($"  Object table offset: {ObjectTableOffset} (0x{ObjectTableOffset:X})");
            builder.AppendLine($"  Object table count: {ObjectTableCount} (0x{ObjectTableCount:X})");
            builder.AppendLine($"  Object page map offset: {ObjectPageMapOffset} (0x{ObjectPageMapOffset:X})");
            builder.AppendLine($"  Object iterate data map offset: {ObjectIterateDataMapOffset} (0x{ObjectIterateDataMapOffset:X})");
            builder.AppendLine($"  Resource table offset: {ResourceTableOffset} (0x{ResourceTableOffset:X})");
            builder.AppendLine($"  Resource table count: {ResourceTableCount} (0x{ResourceTableCount:X})");
            builder.AppendLine($"  Resident names table offset: {ResidentNamesTableOffset} (0x{ResidentNamesTableOffset:X})");
            builder.AppendLine($"  Entry table offset: {EntryTableOffset} (0x{EntryTableOffset:X})");
            builder.AppendLine($"  Module directives table offset: {ModuleDirectivesTableOffset} (0x{ModuleDirectivesTableOffset:X})");
            builder.AppendLine($"  Module directives table count: {ModuleDirectivesCount} (0x{ModuleDirectivesCount:X})");
            builder.AppendLine($"  Fix-up page table offset: {FixupPageTableOffset} (0x{FixupPageTableOffset:X})");
            builder.AppendLine($"  Fix-up record table offset: {FixupRecordTableOffset} (0x{FixupRecordTableOffset:X})");
            builder.AppendLine($"  Imported modules name table offset: {ImportedModulesNameTableOffset} (0x{ImportedModulesNameTableOffset:X})");
            builder.AppendLine($"  Imported modules count: {ImportedModulesCount} (0x{ImportedModulesCount:X})");
            builder.AppendLine($"  Imported procedure name table count: {ImportProcedureNameTableOffset} (0x{ImportProcedureNameTableOffset:X})");
            builder.AppendLine($"  Per-page checksum table offset: {PerPageChecksumTableOffset} (0x{PerPageChecksumTableOffset:X})");
            builder.AppendLine($"  Data pages offset: {DataPagesOffset} (0x{DataPagesOffset:X})");
            builder.AppendLine($"  Preload page count: {PreloadPageCount} (0x{PreloadPageCount:X})");
            builder.AppendLine($"  Non-resident names table offset: {NonResidentNamesTableOffset} (0x{NonResidentNamesTableOffset:X})");
            builder.AppendLine($"  Non-resident names table length: {NonResidentNamesTableLength} (0x{NonResidentNamesTableLength:X})");
            builder.AppendLine($"  Non-resident names table checksum: {NonResidentNamesTableChecksum} (0x{NonResidentNamesTableChecksum:X})");
            builder.AppendLine($"  Automatic data object: {AutomaticDataObject} (0x{AutomaticDataObject:X})");
            builder.AppendLine($"  Debug information offset: {DebugInformationOffset} (0x{DebugInformationOffset:X})");
            builder.AppendLine($"  Debug information length: {DebugInformationLength} (0x{DebugInformationLength:X})");
            builder.AppendLine($"  Preload instance pages number: {PreloadInstancePagesNumber} (0x{PreloadInstancePagesNumber:X})");
            builder.AppendLine($"  Demand instance pages number: {DemandInstancePagesNumber} (0x{DemandInstancePagesNumber:X})");
            builder.AppendLine($"  Extra heap allocation: {ExtraHeapAllocation} (0x{ExtraHeapAllocation:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print object table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintObjectTable(StringBuilder builder)
        {
            builder.AppendLine("  Object Table Information:");
            builder.AppendLine("  -------------------------");
            if (ObjectTable == null || ObjectTable.Length == 0)
            {
                builder.AppendLine("  No object table entries");
            }
            else
            {
                for (int i = 0; i < ObjectTable.Length; i++)
                {
                    var entry = ObjectTable[i];
                    builder.AppendLine($"  Object Table Entry {i}");
                    builder.AppendLine($"    Virtual segment size: {entry.VirtualSegmentSize} (0x{entry.VirtualSegmentSize:X})");
                    builder.AppendLine($"    Relocation base address: {entry.RelocationBaseAddress} (0x{entry.RelocationBaseAddress:X})");
                    builder.AppendLine($"    Object flags: {entry.ObjectFlags} (0x{entry.ObjectFlags:X})");
                    builder.AppendLine($"    Page table index: {entry.PageTableIndex} (0x{entry.PageTableIndex:X})");
                    builder.AppendLine($"    Page table entries: {entry.PageTableEntries} (0x{entry.PageTableEntries:X})");
                    builder.AppendLine($"    Reserved: {entry.Reserved} (0x{entry.Reserved:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print object page map information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintObjectPageMap(StringBuilder builder)
        {
            builder.AppendLine("  Object Page Map Information:");
            builder.AppendLine("  -------------------------");
            if (ObjectPageMap == null || ObjectPageMap.Length == 0)
            {
                builder.AppendLine("  No object page map entries");
            }
            else
            {
                for (int i = 0; i < ObjectPageMap.Length; i++)
                {
                    var entry = ObjectPageMap[i];
                    builder.AppendLine($"  Object Page Map Entry {i}");
                    builder.AppendLine($"    Page data offset: {entry.PageDataOffset} (0x{entry.PageDataOffset:X})");
                    builder.AppendLine($"    Data size: {entry.DataSize} (0x{entry.DataSize:X})");
                    builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print resource table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintResourceTable(StringBuilder builder)
        {
            builder.AppendLine("  Resource Table Information:");
            builder.AppendLine("  -------------------------");
            if (ResourceTable == null || ResourceTable.Length == 0)
            {
                builder.AppendLine("  No resource table entries");
            }
            else
            {
                for (int i = 0; i < ResourceTable.Length; i++)
                {
                    var entry = ResourceTable[i];
                    builder.AppendLine($"  Resource Table Entry {i}");
                    builder.AppendLine($"    Type ID: {entry.TypeID} (0x{entry.TypeID:X})");
                    builder.AppendLine($"    Name ID: {entry.NameID} (0x{entry.NameID:X})");
                    builder.AppendLine($"    Resource size: {entry.ResourceSize} (0x{entry.ResourceSize:X})");
                    builder.AppendLine($"    Object number: {entry.ObjectNumber} (0x{entry.ObjectNumber:X})");
                    builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print resident names table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintResidentNamesTable(StringBuilder builder)
        {
            builder.AppendLine("  Resident Names Table Information:");
            builder.AppendLine("  -------------------------");
            if (ResidentNamesTable == null || ResidentNamesTable.Length == 0)
            {
                builder.AppendLine("  No resident names table entries");
            }
            else
            {
                for (int i = 0; i < ResidentNamesTable.Length; i++)
                {
                    var entry = ResidentNamesTable[i];
                    builder.AppendLine($"  Resident Names Table Entry {i}");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print entry table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintEntryTable(StringBuilder builder)
        {
            builder.AppendLine("  Entry Table Information:");
            builder.AppendLine("  -------------------------");
            if (EntryTable == null || EntryTable.Length == 0)
            {
                builder.AppendLine("  No entry table bundles");
            }
            else
            {
                for (int i = 0; i < EntryTable.Length; i++)
                {
                    var bundle = EntryTable[i];
                    builder.AppendLine($"  Entry Table Bundle {i}");
                    builder.AppendLine($"    Entries: {bundle.Entries} (0x{bundle.Entries:X})");
                    builder.AppendLine($"    Bundle type: {bundle.BundleType} (0x{bundle.BundleType:X})");
                    if (bundle.TableEntries != null && bundle.TableEntries.Length != 0)
                    {
                        builder.AppendLine();
                        builder.AppendLine($"    Entry Table Entries:");
                        builder.AppendLine("    -------------------------");
                        for (int j = 0; j < bundle.TableEntries.Length; j++)
                        {
                            var entry = bundle.TableEntries[j];
                            builder.AppendLine($"    Entry Table Entry {j}");
                            switch (bundle.BundleType & ~Models.LinearExecutable.BundleType.ParameterTypingInformationPresent)
                            {
                                case Models.LinearExecutable.BundleType.UnusedEntry:
                                    builder.AppendLine($"      Unused, empty entry");
                                    break;

                                case Models.LinearExecutable.BundleType.SixteenBitEntry:
                                    builder.AppendLine($"      Object number: {entry.SixteenBitObjectNumber} (0x{entry.SixteenBitObjectNumber:X})");
                                    builder.AppendLine($"      Entry flags: {entry.SixteenBitEntryFlags} (0x{entry.SixteenBitEntryFlags:X})");
                                    builder.AppendLine($"      Offset: {entry.SixteenBitOffset} (0x{entry.SixteenBitOffset:X})");
                                    break;

                                case Models.LinearExecutable.BundleType.TwoEightySixCallGateEntry:
                                    builder.AppendLine($"      Object number: {entry.TwoEightySixObjectNumber} (0x{entry.TwoEightySixObjectNumber:X})");
                                    builder.AppendLine($"      Entry flags: {entry.TwoEightySixEntryFlags} (0x{entry.TwoEightySixEntryFlags:X})");
                                    builder.AppendLine($"      Offset: {entry.TwoEightySixOffset} (0x{entry.TwoEightySixOffset:X})");
                                    builder.AppendLine($"      Callgate: {entry.TwoEightySixCallgate} (0x{entry.TwoEightySixCallgate:X})");
                                    break;

                                case Models.LinearExecutable.BundleType.ThirtyTwoBitEntry:
                                    builder.AppendLine($"      Object number: {entry.ThirtyTwoBitObjectNumber} (0x{entry.ThirtyTwoBitObjectNumber:X})");
                                    builder.AppendLine($"      Entry flags: {entry.ThirtyTwoBitEntryFlags} (0x{entry.ThirtyTwoBitEntryFlags:X})");
                                    builder.AppendLine($"      Offset: {entry.ThirtyTwoBitOffset} (0x{entry.ThirtyTwoBitOffset:X})");
                                    break;

                                case Models.LinearExecutable.BundleType.ForwarderEntry:
                                    builder.AppendLine($"      Reserved: {entry.ForwarderReserved} (0x{entry.ForwarderReserved:X})");
                                    builder.AppendLine($"      Forwarder flags: {entry.ForwarderFlags} (0x{entry.ForwarderFlags:X})");
                                    builder.AppendLine($"      Module ordinal number: {entry.ForwarderModuleOrdinalNumber} (0x{entry.ForwarderModuleOrdinalNumber:X})");
                                    builder.AppendLine($"      Procedure name offset: {entry.ProcedureNameOffset} (0x{entry.ProcedureNameOffset:X})");
                                    builder.AppendLine($"      Import ordinal number: {entry.ImportOrdinalNumber} (0x{entry.ImportOrdinalNumber:X})");
                                    break;

                                default:
                                    builder.AppendLine($"      Unknown entry type {bundle.BundleType}");
                                    break;
                            }
                        }
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print module format directives table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintModuleFormatDirectivesTable(StringBuilder builder)
        {
            builder.AppendLine("  Module Format Directives Table Information:");
            builder.AppendLine("  -------------------------");
            if (ModuleFormatDirectivesTable == null || ModuleFormatDirectivesTable.Length == 0)
            {
                builder.AppendLine("  No module format directives table entries");
            }
            else
            {
                for (int i = 0; i < ModuleFormatDirectivesTable.Length; i++)
                {
                    var entry = ModuleFormatDirectivesTable[i];
                    builder.AppendLine($"  Moduile Format Directives Table Entry {i}");
                    builder.AppendLine($"    Directive number: {entry.DirectiveNumber} (0x{entry.DirectiveNumber:X})");
                    builder.AppendLine($"    Directive data length: {entry.DirectiveDataLength} (0x{entry.DirectiveDataLength:X})");
                    builder.AppendLine($"    Directive data offset: {entry.DirectiveDataOffset} (0x{entry.DirectiveDataOffset:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print verify record directive table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintVerifyRecordDirectiveTable(StringBuilder builder)
        {
            builder.AppendLine("  Verify Record Directive Table Information:");
            builder.AppendLine("  -------------------------");
            if (VerifyRecordDirectiveTable == null || VerifyRecordDirectiveTable.Length == 0)
            {
                builder.AppendLine("  No verify record directive table entries");
            }
            else
            {
                for (int i = 0; i < VerifyRecordDirectiveTable.Length; i++)
                {
                    var entry = VerifyRecordDirectiveTable[i];
                    builder.AppendLine($"  Verify Record Directive Table Entry {i}");
                    builder.AppendLine($"    Entry count: {entry.EntryCount} (0x{entry.EntryCount:X})");
                    builder.AppendLine($"    Ordinal index: {entry.OrdinalIndex} (0x{entry.OrdinalIndex:X})");
                    builder.AppendLine($"    Version: {entry.Version} (0x{entry.Version:X})");
                    builder.AppendLine($"    Object entries count: {entry.ObjectEntriesCount} (0x{entry.ObjectEntriesCount:X})");
                    builder.AppendLine($"    Object number in module: {entry.ObjectNumberInModule} (0x{entry.ObjectNumberInModule:X})");
                    builder.AppendLine($"    Object load base address: {entry.ObjectLoadBaseAddress} (0x{entry.ObjectLoadBaseAddress:X})");
                    builder.AppendLine($"    Object virtual address size: {entry.ObjectVirtualAddressSize} (0x{entry.ObjectVirtualAddressSize:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print fix-up page table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFixupPageTable(StringBuilder builder)
        {
            builder.AppendLine("  Fix-up Page Table Information:");
            builder.AppendLine("  -------------------------");
            if (FixupPageTable == null || FixupPageTable.Length == 0)
            {
                builder.AppendLine("  No fix-up page table entries");
            }
            else
            {
                for (int i = 0; i < FixupPageTable.Length; i++)
                {
                    var entry = FixupPageTable[i];
                    builder.AppendLine($"  Fix-up Page Table Entry {i}");
                    builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print fix-up record table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFixupRecordTable(StringBuilder builder)
        {
            builder.AppendLine("  Fix-up Record Table Information:");
            builder.AppendLine("  -------------------------");
            if (FixupRecordTable == null || FixupRecordTable.Length == 0)
            {
                builder.AppendLine("  No fix-up record table entries");
                builder.AppendLine();
            }
            else
            {
                for (int i = 0; i < FixupRecordTable.Length; i++)
                {
                    var entry = FixupRecordTable[i];
                    builder.AppendLine($"  Fix-up Record Table Entry {i}");
                    builder.AppendLine($"    Source type: {entry.SourceType} (0x{entry.SourceType:X})");
                    builder.AppendLine($"    Target flags: {entry.TargetFlags} (0x{entry.TargetFlags:X})");

                    // Source list flag
                    if (entry.SourceType.HasFlag(Models.LinearExecutable.FixupRecordSourceType.SourceListFlag))
                        builder.AppendLine($"    Source offset list count: {entry.SourceOffsetListCount} (0x{entry.SourceOffsetListCount:X})");
                    else
                        builder.AppendLine($"    Source offset: {entry.SourceOffset} (0x{entry.SourceOffset:X})");

                    // OBJECT / TRGOFF
                    if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.InternalReference))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                        else
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                        // 16-bit Selector fixup
                        if (!entry.SourceType.HasFlag(Models.LinearExecutable.FixupRecordSourceType.SixteenBitSelectorFixup))
                        {
                            // 32-bit Target Offset Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                                builder.AppendLine($"    Target offset: {entry.TargetOffsetDWORD} (0x{entry.TargetOffsetDWORD:X})");
                            else
                                builder.AppendLine($"    Target offset: {entry.TargetOffsetWORD} (0x{entry.TargetOffsetWORD:X})");
                        }
                    }

                    // MOD ORD# / IMPORT ORD / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ImportedReferenceByOrdinal))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                        else
                            builder.AppendLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                        // 8-bit Ordinal Flag & 32-bit Target Offset Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.EightBitOrdinalFlag))
                            builder.AppendLine(value: $"    Imported ordinal number: {entry.ImportedOrdinalNumberByte} (0x{entry.ImportedOrdinalNumberByte:X})");
                        else if (entry.TargetFlags.HasFlag(flag: Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                            builder.AppendLine(value: $"    Imported ordinal number: {entry.ImportedOrdinalNumberDWORD} (0x{entry.ImportedOrdinalNumberDWORD:X})");
                        else
                            builder.AppendLine(value: $"    Imported ordinal number: {entry.ImportedOrdinalNumberWORD} (0x{entry.ImportedOrdinalNumberWORD:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                builder.AppendLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                builder.AppendLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ImportedReferenceByName))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                        else
                            builder.AppendLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                        // 32-bit Target Offset Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                            builder.AppendLine(value: $"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableDWORD} (0x{entry.OffsetImportProcedureNameTableDWORD:X})");
                        else
                            builder.AppendLine(value: $"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableWORD} (0x{entry.OffsetImportProcedureNameTableWORD:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                builder.AppendLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                builder.AppendLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // ORD # / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.InternalReferenceViaEntryTable))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                        else
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                builder.AppendLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                builder.AppendLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // No other top-level flags recognized
                    else
                    {
                        builder.AppendLine($"    Unknown entry format");
                    }

                    builder.AppendLine();
                    builder.AppendLine($"    Source Offset List:");
                    builder.AppendLine("    -------------------------");
                    if (entry.SourceOffsetList == null || entry.SourceOffsetList.Length == 0)
                    {
                        builder.AppendLine($"    No source offset list entries");
                    }
                    else
                    {
                        for (int j = 0; j < entry.SourceOffsetList.Length; j++)
                        {
                            builder.AppendLine($"    Source Offset List Entry {j}: {entry.SourceOffsetList[j]} (0x{entry.SourceOffsetList[j]:X})");
                        }
                    }
                    builder.AppendLine();
                }
            }
        }

        /// <summary>
        /// Print import module name table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintImportModuleNameTable(StringBuilder builder)
        {
            builder.AppendLine("  Import Module Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (ImportModuleNameTable == null || ImportModuleNameTable.Length == 0)
            {
                builder.AppendLine("  No import module name table entries");
            }
            else
            {
                for (int i = 0; i < ImportModuleNameTable.Length; i++)
                {
                    var entry = ImportModuleNameTable[i];
                    builder.AppendLine($"  Import Module Name Table Entry {i}");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print import module procedure name table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintImportModuleProcedureNameTable(StringBuilder builder)
        {
            builder.AppendLine("  Import Module Procedure Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (ImportModuleProcedureNameTable == null || ImportModuleProcedureNameTable.Length == 0)
            {
                builder.AppendLine("  No import module procedure name table entries");
            }
            else
            {
                for (int i = 0; i < ImportModuleProcedureNameTable.Length; i++)
                {
                    var entry = ImportModuleProcedureNameTable[i];
                    builder.AppendLine($"  Import Module Procedure Name Table Entry {i}");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print per-page checksum table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintPerPageChecksumTable(StringBuilder builder)
        {
            builder.AppendLine("  Per-Page Checksum Table Information:");
            builder.AppendLine("  -------------------------");
            if (PerPageChecksumTable == null || PerPageChecksumTable.Length == 0)
            {
                builder.AppendLine("  No per-page checksum table entries");
            }
            else
            {
                for (int i = 0; i < PerPageChecksumTable.Length; i++)
                {
                    var entry = PerPageChecksumTable[i];
                    builder.AppendLine($" Per-Page Checksum Table Entry {i}");
                    builder.AppendLine($"    Checksum: {entry.Checksum} (0x{entry.Checksum:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print non-resident names table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintNonResidentNamesTable(StringBuilder builder)
        {
            builder.AppendLine("  Non-Resident Names Table Information:");
            builder.AppendLine("  -------------------------");
            if (NonResidentNamesTable == null || NonResidentNamesTable.Length == 0)
            {
                builder.AppendLine("  No non-resident names table entries");
            }
            else
            {
                for (int i = 0; i < NonResidentNamesTable.Length; i++)
                {
                    var entry = NonResidentNamesTable[i];
                    builder.AppendLine($"  Non-Resident Names Table Entry {i}");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print debug information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDebugInformation(StringBuilder builder)
        {
            builder.AppendLine("  Debug Information:");
            builder.AppendLine("  -------------------------");
            if (_executable.DebugInformation == null)
            {
                builder.AppendLine("  No debug information");
            }
            else
            {
                builder.AppendLine($"  Signature: {DI_Signature ?? "[NULL]"}");
                builder.AppendLine($"  Format type: {DI_FormatType} (0x{DI_FormatType:X})");
                // Debugger data
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_executable, _jsonSerializerOptions);

#endif

        #endregion

        #region REMOVE -- DO NOT USE

        /// <summary>
        /// Read an arbitrary range from the source
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns>Byte array representing the range, null on error</returns>
        [Obsolete]
        public byte[] ReadArbitraryRange(int rangeStart = -1, int length = -1)
        {
            // If we have an unset range start, read from the start of the source
            if (rangeStart == -1)
                rangeStart = 0;

            // If we have an unset length, read the whole source
            if (length == -1)
            {
                switch (_dataSource)
                {
                    case DataSource.ByteArray:
                        length = _byteArrayData.Length - _byteArrayOffset;
                        break;

                    case DataSource.Stream:
                        length = (int)_streamData.Length;
                        break;
                }
            }

            return ReadFromDataSource(rangeStart, length);
        }

        #endregion
    }
}