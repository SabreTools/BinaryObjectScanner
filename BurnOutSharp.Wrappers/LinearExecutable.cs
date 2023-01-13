using System;
using System.IO;

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
        public override void Print()
        {
            Console.WriteLine("New Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            // Stub
            PrintStubHeader();
            PrintStubExtendedHeader();

            // Information Block
            PrintInformationBlock();

            // Tables
            PrintObjectTable();
            PrintObjectPageMap();
            PrintResourceTable();
            PrintResidentNamesTable();
            PrintEntryTable();
            PrintModuleFormatDirectivesTable();
            PrintVerifyRecordDirectiveTable();
            PrintFixupPageTable();
            PrintFixupRecordTable();
            PrintImportModuleNameTable();
            PrintImportModuleProcedureNameTable();
            PrintPerPageChecksumTable();
            PrintNonResidentNamesTable();

            // Debug
            PrintDebugInformation();
        }

        /// <summary>
        /// Print stub header information
        /// </summary>
        private void PrintStubHeader()
        {
            Console.WriteLine("  MS-DOS Stub Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {Stub_Magic}");
            Console.WriteLine($"  Last page bytes: {Stub_LastPageBytes} (0x{Stub_LastPageBytes:X})");
            Console.WriteLine($"  Pages: {Stub_Pages} (0x{Stub_Pages:X})");
            Console.WriteLine($"  Relocation items: {Stub_RelocationItems} (0x{Stub_RelocationItems:X})");
            Console.WriteLine($"  Header paragraph size: {Stub_HeaderParagraphSize} (0x{Stub_HeaderParagraphSize:X})");
            Console.WriteLine($"  Minimum extra paragraphs: {Stub_MinimumExtraParagraphs} (0x{Stub_MinimumExtraParagraphs:X})");
            Console.WriteLine($"  Maximum extra paragraphs: {Stub_MaximumExtraParagraphs} (0x{Stub_MaximumExtraParagraphs:X})");
            Console.WriteLine($"  Initial SS value: {Stub_InitialSSValue} (0x{Stub_InitialSSValue:X})");
            Console.WriteLine($"  Initial SP value: {Stub_InitialSPValue} (0x{Stub_InitialSPValue:X})");
            Console.WriteLine($"  Checksum: {Stub_Checksum} (0x{Stub_Checksum:X})");
            Console.WriteLine($"  Initial IP value: {Stub_InitialIPValue} (0x{Stub_InitialIPValue:X})");
            Console.WriteLine($"  Initial CS value: {Stub_InitialCSValue} (0x{Stub_InitialCSValue:X})");
            Console.WriteLine($"  Relocation table address: {Stub_RelocationTableAddr} (0x{Stub_RelocationTableAddr:X})");
            Console.WriteLine($"  Overlay number: {Stub_OverlayNumber} (0x{Stub_OverlayNumber:X})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print stub extended header information
        /// </summary>
        private void PrintStubExtendedHeader()
        {
            Console.WriteLine("  MS-DOS Stub Extended Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Reserved words: {string.Join(", ", Stub_Reserved1)}");
            Console.WriteLine($"  OEM identifier: {Stub_OEMIdentifier} (0x{Stub_OEMIdentifier:X})");
            Console.WriteLine($"  OEM information: {Stub_OEMInformation} (0x{Stub_OEMInformation:X})");
            Console.WriteLine($"  Reserved words: {string.Join(", ", Stub_Reserved2)}");
            Console.WriteLine($"  New EXE header address: {Stub_NewExeHeaderAddr} (0x{Stub_NewExeHeaderAddr:X})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print information block information
        /// </summary>
        private void PrintInformationBlock()
        {
            Console.WriteLine("  Information Block Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Byte order: {ByteOrder} (0x{ByteOrder:X})");
            Console.WriteLine($"  Word order: {WordOrder} (0x{WordOrder:X})");
            Console.WriteLine($"  Executable format level: {ExecutableFormatLevel} (0x{ExecutableFormatLevel:X})");
            Console.WriteLine($"  CPU type: {CPUType} (0x{CPUType:X})");
            Console.WriteLine($"  Module OS: {ModuleOS} (0x{ModuleOS:X})");
            Console.WriteLine($"  Module version: {ModuleVersion} (0x{ModuleVersion:X})");
            Console.WriteLine($"  Module type flags: {ModuleTypeFlags} (0x{ModuleTypeFlags:X})");
            Console.WriteLine($"  Module number pages: {ModuleNumberPages} (0x{ModuleNumberPages:X})");
            Console.WriteLine($"  Initial object CS: {InitialObjectCS} (0x{InitialObjectCS:X})");
            Console.WriteLine($"  Initial EIP: {InitialEIP} (0x{InitialEIP:X})");
            Console.WriteLine($"  Initial object SS: {InitialObjectSS} (0x{InitialObjectSS:X})");
            Console.WriteLine($"  Initial ESP: {InitialESP} (0x{InitialESP:X})");
            Console.WriteLine($"  Memory page size: {MemoryPageSize} (0x{MemoryPageSize:X})");
            Console.WriteLine($"  Bytes on last page: {BytesOnLastPage} (0x{BytesOnLastPage:X})");
            Console.WriteLine($"  Fix-up section size: {FixupSectionSize} (0x{FixupSectionSize:X})");
            Console.WriteLine($"  Fix-up section checksum: {FixupSectionChecksum} (0x{FixupSectionChecksum:X})");
            Console.WriteLine($"  Loader section size: {LoaderSectionSize} (0x{LoaderSectionSize:X})");
            Console.WriteLine($"  Loader section checksum: {LoaderSectionChecksum} (0x{LoaderSectionChecksum:X})");
            Console.WriteLine($"  Object table offset: {ObjectTableOffset} (0x{ObjectTableOffset:X})");
            Console.WriteLine($"  Object table count: {ObjectTableCount} (0x{ObjectTableCount:X})");
            Console.WriteLine($"  Object page map offset: {ObjectPageMapOffset} (0x{ObjectPageMapOffset:X})");
            Console.WriteLine($"  Object iterate data map offset: {ObjectIterateDataMapOffset} (0x{ObjectIterateDataMapOffset:X})");
            Console.WriteLine($"  Resource table offset: {ResourceTableOffset} (0x{ResourceTableOffset:X})");
            Console.WriteLine($"  Resource table count: {ResourceTableCount} (0x{ResourceTableCount:X})");
            Console.WriteLine($"  Resident names table offset: {ResidentNamesTableOffset} (0x{ResidentNamesTableOffset:X})");
            Console.WriteLine($"  Entry table offset: {EntryTableOffset} (0x{EntryTableOffset:X})");
            Console.WriteLine($"  Module directives table offset: {ModuleDirectivesTableOffset} (0x{ModuleDirectivesTableOffset:X})");
            Console.WriteLine($"  Module directives table count: {ModuleDirectivesCount} (0x{ModuleDirectivesCount:X})");
            Console.WriteLine($"  Fix-up page table offset: {FixupPageTableOffset} (0x{FixupPageTableOffset:X})");
            Console.WriteLine($"  Fix-up record table offset: {FixupRecordTableOffset} (0x{FixupRecordTableOffset:X})");
            Console.WriteLine($"  Imported modules name table offset: {ImportedModulesNameTableOffset} (0x{ImportedModulesNameTableOffset:X})");
            Console.WriteLine($"  Imported modules count: {ImportedModulesCount} (0x{ImportedModulesCount:X})");
            Console.WriteLine($"  Imported procedure name table count: {ImportProcedureNameTableOffset} (0x{ImportProcedureNameTableOffset:X})");
            Console.WriteLine($"  Per-page checksum table offset: {PerPageChecksumTableOffset} (0x{PerPageChecksumTableOffset:X})");
            Console.WriteLine($"  Data pages offset: {DataPagesOffset} (0x{DataPagesOffset:X})");
            Console.WriteLine($"  Preload page count: {PreloadPageCount} (0x{PreloadPageCount:X})");
            Console.WriteLine($"  Non-resident names table offset: {NonResidentNamesTableOffset} (0x{NonResidentNamesTableOffset:X})");
            Console.WriteLine($"  Non-resident names table length: {NonResidentNamesTableLength} (0x{NonResidentNamesTableLength:X})");
            Console.WriteLine($"  Non-resident names table checksum: {NonResidentNamesTableChecksum} (0x{NonResidentNamesTableChecksum:X})");
            Console.WriteLine($"  Automatic data object: {AutomaticDataObject} (0x{AutomaticDataObject:X})");
            Console.WriteLine($"  Debug information offset: {DebugInformationOffset} (0x{DebugInformationOffset:X})");
            Console.WriteLine($"  Debug information length: {DebugInformationLength} (0x{DebugInformationLength:X})");
            Console.WriteLine($"  Preload instance pages number: {PreloadInstancePagesNumber} (0x{PreloadInstancePagesNumber:X})");
            Console.WriteLine($"  Demand instance pages number: {DemandInstancePagesNumber} (0x{DemandInstancePagesNumber:X})");
            Console.WriteLine($"  Extra heap allocation: {ExtraHeapAllocation} (0x{ExtraHeapAllocation:X})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print object table information
        /// </summary>
        private void PrintObjectTable()
        {
            Console.WriteLine("  Object Table Information:");
            Console.WriteLine("  -------------------------");
            if (ObjectTable == null || ObjectTable.Length == 0)
            {
                Console.WriteLine("  No object table entries");
            }
            else
            {
                for (int i = 0; i < ObjectTable.Length; i++)
                {
                    var entry = ObjectTable[i];
                    Console.WriteLine($"  Object Table Entry {i}");
                    Console.WriteLine($"    Virtual segment size: {entry.VirtualSegmentSize} (0x{entry.VirtualSegmentSize:X})");
                    Console.WriteLine($"    Relocation base address: {entry.RelocationBaseAddress} (0x{entry.RelocationBaseAddress:X})");
                    Console.WriteLine($"    Object flags: {entry.ObjectFlags} (0x{entry.ObjectFlags:X})");
                    Console.WriteLine($"    Page table index: {entry.PageTableIndex} (0x{entry.PageTableIndex:X})");
                    Console.WriteLine($"    Page table entries: {entry.PageTableEntries} (0x{entry.PageTableEntries:X})");
                    Console.WriteLine($"    Reserved: {entry.Reserved} (0x{entry.Reserved:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print object page map information
        /// </summary>
        private void PrintObjectPageMap()
        {
            Console.WriteLine("  Object Page Map Information:");
            Console.WriteLine("  -------------------------");
            if (ObjectPageMap == null || ObjectPageMap.Length == 0)
            {
                Console.WriteLine("  No object page map entries");
            }
            else
            {
                for (int i = 0; i < ObjectPageMap.Length; i++)
                {
                    var entry = ObjectPageMap[i];
                    Console.WriteLine($"  Object Page Map Entry {i}");
                    Console.WriteLine($"    Page data offset: {entry.PageDataOffset} (0x{entry.PageDataOffset:X})");
                    Console.WriteLine($"    Data size: {entry.DataSize} (0x{entry.DataSize:X})");
                    Console.WriteLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print resource table information
        /// </summary>
        private void PrintResourceTable()
        {
            Console.WriteLine("  Resource Table Information:");
            Console.WriteLine("  -------------------------");
            if (ResourceTable == null || ResourceTable.Length == 0)
            {
                Console.WriteLine("  No resource table entries");
            }
            else
            {
                for (int i = 0; i < ResourceTable.Length; i++)
                {
                    var entry = ResourceTable[i];
                    Console.WriteLine($"  Resource Table Entry {i}");
                    Console.WriteLine($"    Type ID: {entry.TypeID} (0x{entry.TypeID:X})");
                    Console.WriteLine($"    Name ID: {entry.NameID} (0x{entry.NameID:X})");
                    Console.WriteLine($"    Resource size: {entry.ResourceSize} (0x{entry.ResourceSize:X})");
                    Console.WriteLine($"    Object number: {entry.ObjectNumber} (0x{entry.ObjectNumber:X})");
                    Console.WriteLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print resident names table information
        /// </summary>
        private void PrintResidentNamesTable()
        {
            Console.WriteLine("  Resident Names Table Information:");
            Console.WriteLine("  -------------------------");
            if (ResidentNamesTable == null || ResidentNamesTable.Length == 0)
            {
                Console.WriteLine("  No resident names table entries");
            }
            else
            {
                for (int i = 0; i < ResidentNamesTable.Length; i++)
                {
                    var entry = ResidentNamesTable[i];
                    Console.WriteLine($"  Resident Names Table Entry {i}");
                    Console.WriteLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    Console.WriteLine($"    Name: {entry.Name ?? "[NULL]"}");
                    Console.WriteLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print entry table information
        /// </summary>
        private void PrintEntryTable()
        {
            Console.WriteLine("  Entry Table Information:");
            Console.WriteLine("  -------------------------");
            if (EntryTable == null || EntryTable.Length == 0)
            {
                Console.WriteLine("  No entry table bundles");
            }
            else
            {
                for (int i = 0; i < EntryTable.Length; i++)
                {
                    var bundle = EntryTable[i];
                    Console.WriteLine($"  Entry Table Bundle {i}");
                    Console.WriteLine($"    Entries: {bundle.Entries} (0x{bundle.Entries:X})");
                    Console.WriteLine($"    Bundle type: {bundle.BundleType} (0x{bundle.BundleType:X})");
                    if (bundle.TableEntries != null && bundle.TableEntries.Length != 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"    Entry Table Entries:");
                        Console.WriteLine("    -------------------------");
                        for (int j = 0; j < bundle.TableEntries.Length; j++)
                        {
                            var entry = bundle.TableEntries[j];
                            Console.WriteLine($"    Entry Table Entry {j}");
                            switch (bundle.BundleType & ~Models.LinearExecutable.BundleType.ParameterTypingInformationPresent)
                            {
                                case Models.LinearExecutable.BundleType.UnusedEntry:
                                    Console.WriteLine($"      Unused, empty entry");
                                    break;

                                case Models.LinearExecutable.BundleType.SixteenBitEntry:
                                    Console.WriteLine($"      Object number: {entry.SixteenBitObjectNumber} (0x{entry.SixteenBitObjectNumber:X})");
                                    Console.WriteLine($"      Entry flags: {entry.SixteenBitEntryFlags} (0x{entry.SixteenBitEntryFlags:X})");
                                    Console.WriteLine($"      Offset: {entry.SixteenBitOffset} (0x{entry.SixteenBitOffset:X})");
                                    break;

                                case Models.LinearExecutable.BundleType.TwoEightySixCallGateEntry:
                                    Console.WriteLine($"      Object number: {entry.TwoEightySixObjectNumber} (0x{entry.TwoEightySixObjectNumber:X})");
                                    Console.WriteLine($"      Entry flags: {entry.TwoEightySixEntryFlags} (0x{entry.TwoEightySixEntryFlags:X})");
                                    Console.WriteLine($"      Offset: {entry.TwoEightySixOffset} (0x{entry.TwoEightySixOffset:X})");
                                    Console.WriteLine($"      Callgate: {entry.TwoEightySixCallgate} (0x{entry.TwoEightySixCallgate:X})");
                                    break;

                                case Models.LinearExecutable.BundleType.ThirtyTwoBitEntry:
                                    Console.WriteLine($"      Object number: {entry.ThirtyTwoBitObjectNumber} (0x{entry.ThirtyTwoBitObjectNumber:X})");
                                    Console.WriteLine($"      Entry flags: {entry.ThirtyTwoBitEntryFlags} (0x{entry.ThirtyTwoBitEntryFlags:X})");
                                    Console.WriteLine($"      Offset: {entry.ThirtyTwoBitOffset} (0x{entry.ThirtyTwoBitOffset:X})");
                                    break;

                                case Models.LinearExecutable.BundleType.ForwarderEntry:
                                    Console.WriteLine($"      Reserved: {entry.ForwarderReserved} (0x{entry.ForwarderReserved:X})");
                                    Console.WriteLine($"      Forwarder flags: {entry.ForwarderFlags} (0x{entry.ForwarderFlags:X})");
                                    Console.WriteLine($"      Module ordinal number: {entry.ForwarderModuleOrdinalNumber} (0x{entry.ForwarderModuleOrdinalNumber:X})");
                                    Console.WriteLine($"      Procedure name offset: {entry.ProcedureNameOffset} (0x{entry.ProcedureNameOffset:X})");
                                    Console.WriteLine($"      Import ordinal number: {entry.ImportOrdinalNumber} (0x{entry.ImportOrdinalNumber:X})");
                                    break;

                                default:
                                    Console.WriteLine($"      Unknown entry type {bundle.BundleType}");
                                    break;
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print module format directives table information
        /// </summary>
        private void PrintModuleFormatDirectivesTable()
        {
            Console.WriteLine("  Module Format Directives Table Information:");
            Console.WriteLine("  -------------------------");
            if (ModuleFormatDirectivesTable == null || ModuleFormatDirectivesTable.Length == 0)
            {
                Console.WriteLine("  No module format directives table entries");
            }
            else
            {
                for (int i = 0; i < ModuleFormatDirectivesTable.Length; i++)
                {
                    var entry = ModuleFormatDirectivesTable[i];
                    Console.WriteLine($"  Moduile Format Directives Table Entry {i}");
                    Console.WriteLine($"    Directive number: {entry.DirectiveNumber} (0x{entry.DirectiveNumber:X})");
                    Console.WriteLine($"    Directive data length: {entry.DirectiveDataLength} (0x{entry.DirectiveDataLength:X})");
                    Console.WriteLine($"    Directive data offset: {entry.DirectiveDataOffset} (0x{entry.DirectiveDataOffset:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print verify record directive table information
        /// </summary>
        private void PrintVerifyRecordDirectiveTable()
        {
            Console.WriteLine("  Verify Record Directive Table Information:");
            Console.WriteLine("  -------------------------");
            if (VerifyRecordDirectiveTable == null || VerifyRecordDirectiveTable.Length == 0)
            {
                Console.WriteLine("  No verify record directive table entries");
            }
            else
            {
                for (int i = 0; i < VerifyRecordDirectiveTable.Length; i++)
                {
                    var entry = VerifyRecordDirectiveTable[i];
                    Console.WriteLine($"  Verify Record Directive Table Entry {i}");
                    Console.WriteLine($"    Entry count: {entry.EntryCount} (0x{entry.EntryCount:X})");
                    Console.WriteLine($"    Ordinal index: {entry.OrdinalIndex} (0x{entry.OrdinalIndex:X})");
                    Console.WriteLine($"    Version: {entry.Version} (0x{entry.Version:X})");
                    Console.WriteLine($"    Object entries count: {entry.ObjectEntriesCount} (0x{entry.ObjectEntriesCount:X})");
                    Console.WriteLine($"    Object number in module: {entry.ObjectNumberInModule} (0x{entry.ObjectNumberInModule:X})");
                    Console.WriteLine($"    Object load base address: {entry.ObjectLoadBaseAddress} (0x{entry.ObjectLoadBaseAddress:X})");
                    Console.WriteLine($"    Object virtual address size: {entry.ObjectVirtualAddressSize} (0x{entry.ObjectVirtualAddressSize:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print fix-up page table information
        /// </summary>
        private void PrintFixupPageTable()
        {
            Console.WriteLine("  Fix-up Page Table Information:");
            Console.WriteLine("  -------------------------");
            if (FixupPageTable == null || FixupPageTable.Length == 0)
            {
                Console.WriteLine("  No fix-up page table entries");
            }
            else
            {
                for (int i = 0; i < FixupPageTable.Length; i++)
                {
                    var entry = FixupPageTable[i];
                    Console.WriteLine($"  Fix-up Page Table Entry {i}");
                    Console.WriteLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print fix-up record table information
        /// </summary>
        private void PrintFixupRecordTable()
        {
            Console.WriteLine("  Fix-up Record Table Information:");
            Console.WriteLine("  -------------------------");
            if (FixupRecordTable == null || FixupRecordTable.Length == 0)
            {
                Console.WriteLine("  No fix-up record table entries");
                Console.WriteLine();
            }
            else
            {
                for (int i = 0; i < FixupRecordTable.Length; i++)
                {
                    var entry = FixupRecordTable[i];
                    Console.WriteLine($"  Fix-up Record Table Entry {i}");
                    Console.WriteLine($"    Source type: {entry.SourceType} (0x{entry.SourceType:X})");
                    Console.WriteLine($"    Target flags: {entry.TargetFlags} (0x{entry.TargetFlags:X})");

                    // Source list flag
                    if (entry.SourceType.HasFlag(Models.LinearExecutable.FixupRecordSourceType.SourceListFlag))
                        Console.WriteLine($"    Source offset list count: {entry.SourceOffsetListCount} (0x{entry.SourceOffsetListCount:X})");
                    else
                        Console.WriteLine($"    Source offset: {entry.SourceOffset} (0x{entry.SourceOffset:X})");

                    // OBJECT / TRGOFF
                    if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.InternalReference))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            Console.WriteLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                        else
                            Console.WriteLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                        // 16-bit Selector fixup
                        if (!entry.SourceType.HasFlag(Models.LinearExecutable.FixupRecordSourceType.SixteenBitSelectorFixup))
                        {
                            // 32-bit Target Offset Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                                Console.WriteLine($"    Target offset: {entry.TargetOffsetDWORD} (0x{entry.TargetOffsetDWORD:X})");
                            else
                                Console.WriteLine($"    Target offset: {entry.TargetOffsetWORD} (0x{entry.TargetOffsetWORD:X})");
                        }
                    }

                    // MOD ORD# / IMPORT ORD / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ImportedReferenceByOrdinal))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            Console.WriteLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                        else
                            Console.WriteLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                        // 8-bit Ordinal Flag & 32-bit Target Offset Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.EightBitOrdinalFlag))
                            Console.WriteLine(value: $"    Imported ordinal number: {entry.ImportedOrdinalNumberByte} (0x{entry.ImportedOrdinalNumberByte:X})");
                        else if (entry.TargetFlags.HasFlag(flag: Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                            Console.WriteLine(value: $"    Imported ordinal number: {entry.ImportedOrdinalNumberDWORD} (0x{entry.ImportedOrdinalNumberDWORD:X})");
                        else
                            Console.WriteLine(value: $"    Imported ordinal number: {entry.ImportedOrdinalNumberWORD} (0x{entry.ImportedOrdinalNumberWORD:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                Console.WriteLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                Console.WriteLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ImportedReferenceByName))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            Console.WriteLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                        else
                            Console.WriteLine(value: $"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                        // 32-bit Target Offset Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                            Console.WriteLine(value: $"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableDWORD} (0x{entry.OffsetImportProcedureNameTableDWORD:X})");
                        else
                            Console.WriteLine(value: $"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableWORD} (0x{entry.OffsetImportProcedureNameTableWORD:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                Console.WriteLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                Console.WriteLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // ORD # / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.InternalReferenceViaEntryTable))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            Console.WriteLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                        else
                            Console.WriteLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                Console.WriteLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                Console.WriteLine(value: $"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // No other top-level flags recognized
                    else
                    {
                        Console.WriteLine($"    Unknown entry format");
                    }

                    Console.WriteLine();
                    Console.WriteLine($"    Source Offset List:");
                    Console.WriteLine("    -------------------------");
                    if (entry.SourceOffsetList == null || entry.SourceOffsetList.Length == 0)
                    {
                        Console.WriteLine($"    No source offset list entries");
                    }
                    else
                    {
                        for (int j = 0; j < entry.SourceOffsetList.Length; j++)
                        {
                            Console.WriteLine($"    Source Offset List Entry {j}: {entry.SourceOffsetList[j]} (0x{entry.SourceOffsetList[j]:X})");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Print import module name table information
        /// </summary>
        private void PrintImportModuleNameTable()
        {
            Console.WriteLine("  Import Module Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (ImportModuleNameTable == null || ImportModuleNameTable.Length == 0)
            {
                Console.WriteLine("  No import module name table entries");
            }
            else
            {
                for (int i = 0; i < ImportModuleNameTable.Length; i++)
                {
                    var entry = ImportModuleNameTable[i];
                    Console.WriteLine($"  Import Module Name Table Entry {i}");
                    Console.WriteLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    Console.WriteLine($"    Name: {entry.Name ?? "[NULL]"}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print import module procedure name table information
        /// </summary>
        private void PrintImportModuleProcedureNameTable()
        {
            Console.WriteLine("  Import Module Procedure Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (ImportModuleProcedureNameTable == null || ImportModuleProcedureNameTable.Length == 0)
            {
                Console.WriteLine("  No import module procedure name table entries");
            }
            else
            {
                for (int i = 0; i < ImportModuleProcedureNameTable.Length; i++)
                {
                    var entry = ImportModuleProcedureNameTable[i];
                    Console.WriteLine($"  Import Module Procedure Name Table Entry {i}");
                    Console.WriteLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    Console.WriteLine($"    Name: {entry.Name ?? "[NULL]"}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print per-page checksum table information
        /// </summary>
        private void PrintPerPageChecksumTable()
        {
            Console.WriteLine("  Per-Page Checksum Table Information:");
            Console.WriteLine("  -------------------------");
            if (PerPageChecksumTable == null || PerPageChecksumTable.Length == 0)
            {
                Console.WriteLine("  No per-page checksum table entries");
            }
            else
            {
                for (int i = 0; i < PerPageChecksumTable.Length; i++)
                {
                    var entry = PerPageChecksumTable[i];
                    Console.WriteLine($" Per-Page Checksum Table Entry {i}");
                    Console.WriteLine($"    Checksum: {entry.Checksum} (0x{entry.Checksum:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print non-resident names table information
        /// </summary>
        private void PrintNonResidentNamesTable()
        {
            Console.WriteLine("  Non-Resident Names Table Information:");
            Console.WriteLine("  -------------------------");
            if (NonResidentNamesTable == null || NonResidentNamesTable.Length == 0)
            {
                Console.WriteLine("  No non-resident names table entries");
            }
            else
            {
                for (int i = 0; i < NonResidentNamesTable.Length; i++)
                {
                    var entry = NonResidentNamesTable[i];
                    Console.WriteLine($"  Non-Resident Names Table Entry {i}");
                    Console.WriteLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    Console.WriteLine($"    Name: {entry.Name ?? "[NULL]"}");
                    Console.WriteLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print debug information
        /// </summary>
        private void PrintDebugInformation()
        {
            Console.WriteLine("  Debug Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.DebugInformation == null)
            {
                Console.WriteLine("  No debug information");
            }
            else
            {
                Console.WriteLine($"  Signature: {DI_Signature ?? "[NULL]"}");
                Console.WriteLine($"  Format type: {DI_FormatType} (0x{DI_FormatType:X})");
                // Debugger data
            }
            Console.WriteLine();
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