using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class LinearExecutable : WrapperBase
    {
        #region Pass-Through Properties

        #region MS-DOS Stub

        #region Standard Fields

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
        public byte[] Stub_Magic => _executable.Stub.Header.Magic;

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
        public char[] Signature => _executable.InformationBlock.Signature;

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

        /// <inheritdoc cref="Models.LinearExecutable.ObjectTable"/>
        public Models.LinearExecutable.ObjectPageTableEntry[] ObjectPageTable => _executable.ObjectPageTable;

        // TODO: Object iterate data map table [Does this exist?]

        /// <inheritdoc cref="Models.LinearExecutable.ResourceTable"/>
        public Models.LinearExecutable.ResourceTableEntry[] ResourceTable => _executable.ResourceTable;

        /// <inheritdoc cref="Models.LinearExecutable.ResidentNameTable"/>
        public Models.LinearExecutable.ResidentNameTableEntry[] ResidentNameTable => _executable.ResidentNameTable;

        /// <inheritdoc cref="Models.LinearExecutable.EntryTable"/>
        public Models.LinearExecutable.EntryTableEntry[] EntryTable => _executable.EntryTable;

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

        // TODO: Preload Pages
        // TODO: Demand Load Pages
        // TODO: Iterated Pages

        /// <inheritdoc cref="Models.LinearExecutable.NonResidentNameTable"/>
        public Models.LinearExecutable.NonResidentNameTableEntry[] NonResidentNameTable => _executable.NonResidentNameTable;

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation"/>
        public Models.LinearExecutable.DebugInformation DebugInformation => _executable.DebugInformation;

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
            MemoryStream dataStream = new MemoryStream(data);
            dataStream.Position = offset;
            return Create(dataStream);
        }

        /// <summary>
        /// Create an LE/LX executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
        public static LinearExecutable Create(Stream data)
        {
            var executable = Builder.LinearExecutable.ParseExecutable(data);
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
            // TODO: Implement printing
        }

        #endregion
    }
}