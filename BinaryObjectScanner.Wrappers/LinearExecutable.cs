using System;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class LinearExecutable : WrapperBase<SabreTools.Models.LinearExecutable.Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Linear Executable (LE/LX)";

        #endregion

        #region Pass-Through Properties

        #region MS-DOS Stub

        #region Standard Fields

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
#if NET48
        public string Stub_Magic => _model.Stub.Header.Magic;
#else
        public string? Stub_Magic => _model.Stub?.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
#if NET48
        public ushort Stub_LastPageBytes => _model.Stub.Header.LastPageBytes;
#else
        public ushort? Stub_LastPageBytes => _model.Stub?.Header?.LastPageBytes;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
#if NET48
        public ushort Stub_Pages => _model.Stub.Header.Pages;
#else
        public ushort? Stub_Pages => _model.Stub?.Header?.Pages;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
#if NET48
        public ushort Stub_RelocationItems => _model.Stub.Header.RelocationItems;
#else
        public ushort? Stub_RelocationItems => _model.Stub?.Header?.RelocationItems;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
#if NET48
        public ushort Stub_HeaderParagraphSize => _model.Stub.Header.HeaderParagraphSize;
#else
        public ushort? Stub_HeaderParagraphSize => _model.Stub?.Header?.HeaderParagraphSize;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
#if NET48
        public ushort Stub_MinimumExtraParagraphs => _model.Stub.Header.MinimumExtraParagraphs;
#else
        public ushort? Stub_MinimumExtraParagraphs => _model.Stub?.Header?.MinimumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
#if NET48
        public ushort Stub_MaximumExtraParagraphs => _model.Stub.Header.MaximumExtraParagraphs;
#else
        public ushort? Stub_MaximumExtraParagraphs => _model.Stub?.Header?.MaximumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
#if NET48
        public ushort Stub_InitialSSValue => _model.Stub.Header.InitialSSValue;
#else
        public ushort? Stub_InitialSSValue => _model.Stub?.Header?.InitialSSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
#if NET48
        public ushort Stub_InitialSPValue => _model.Stub.Header.InitialSPValue;
#else
        public ushort? Stub_InitialSPValue => _model.Stub?.Header?.InitialSPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
#if NET48
        public ushort Stub_Checksum => _model.Stub.Header.Checksum;
#else
        public ushort? Stub_Checksum => _model.Stub?.Header?.Checksum;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
#if NET48
        public ushort Stub_InitialIPValue => _model.Stub.Header.InitialIPValue;
#else
        public ushort? Stub_InitialIPValue => _model.Stub?.Header?.InitialIPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
#if NET48
        public ushort Stub_InitialCSValue => _model.Stub.Header.InitialCSValue;
#else
        public ushort? Stub_InitialCSValue => _model.Stub?.Header?.InitialCSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
#if NET48
        public ushort Stub_RelocationTableAddr => _model.Stub.Header.RelocationTableAddr;
#else
        public ushort? Stub_RelocationTableAddr => _model.Stub?.Header?.RelocationTableAddr;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
#if NET48
        public ushort Stub_OverlayNumber => _model.Stub.Header.OverlayNumber;
#else
        public ushort? Stub_OverlayNumber => _model.Stub?.Header?.OverlayNumber;
#endif

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
#if NET48
        public ushort[] Stub_Reserved1 => _model.Stub.Header.Reserved1;
#else
        public ushort[]? Stub_Reserved1 => _model.Stub?.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
#if NET48
        public ushort Stub_OEMIdentifier => _model.Stub.Header.OEMIdentifier;
#else
        public ushort? Stub_OEMIdentifier => _model.Stub?.Header?.OEMIdentifier;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
#if NET48
        public ushort Stub_OEMInformation => _model.Stub.Header.OEMInformation;
#else
        public ushort? Stub_OEMInformation => _model.Stub?.Header?.OEMInformation;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
#if NET48
        public ushort[] Stub_Reserved2 => _model.Stub.Header.Reserved2;
#else
        public ushort[]? Stub_Reserved2 => _model?.Stub?.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
#if NET48
        public uint Stub_NewExeHeaderAddr => _model.Stub.Header.NewExeHeaderAddr;
#else
        public uint? Stub_NewExeHeaderAddr => _model.Stub?.Header?.NewExeHeaderAddr;
#endif

        #endregion

        #endregion

        #region Information Block

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.Signature"/>
#if NET48
        public string Signature => _model.InformationBlock.Signature;
#else
        public string? Signature => _model.InformationBlock?.Signature;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ByteOrder"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ByteOrder ByteOrder => _model.InformationBlock.ByteOrder;
#else
        public SabreTools.Models.LinearExecutable.ByteOrder? ByteOrder => _model.InformationBlock?.ByteOrder;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.WordOrder"/>
#if NET48
        public SabreTools.Models.LinearExecutable.WordOrder WordOrder => _model.InformationBlock.WordOrder;
#else
        public SabreTools.Models.LinearExecutable.WordOrder? WordOrder => _model.InformationBlock?.WordOrder;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ExecutableFormatLevel"/>
#if NET48
        public uint ExecutableFormatLevel => _model.InformationBlock.ExecutableFormatLevel;
#else
        public uint? ExecutableFormatLevel => _model.InformationBlock?.ExecutableFormatLevel;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.CPUType"/>
#if NET48
        public SabreTools.Models.LinearExecutable.CPUType CPUType => _model.InformationBlock.CPUType;
#else
        public SabreTools.Models.LinearExecutable.CPUType? CPUType => _model.InformationBlock?.CPUType;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleOS"/>
#if NET48
        public SabreTools.Models.LinearExecutable.OperatingSystem ModuleOS => _model.InformationBlock.ModuleOS;
#else
        public SabreTools.Models.LinearExecutable.OperatingSystem? ModuleOS => _model.InformationBlock?.ModuleOS;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleVersion"/>
#if NET48
        public uint ModuleVersion => _model.InformationBlock.ModuleVersion;
#else
        public uint? ModuleVersion => _model.InformationBlock?.ModuleVersion;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleTypeFlags"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ModuleFlags ModuleTypeFlags => _model.InformationBlock.ModuleTypeFlags;
#else
        public SabreTools.Models.LinearExecutable.ModuleFlags? ModuleTypeFlags => _model.InformationBlock?.ModuleTypeFlags;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleNumberPages"/>
#if NET48
        public uint ModuleNumberPages => _model.InformationBlock.ModuleNumberPages;
#else
        public uint? ModuleNumberPages => _model.InformationBlock?.ModuleNumberPages;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialObjectCS"/>
#if NET48
        public uint InitialObjectCS => _model.InformationBlock.InitialObjectCS;
#else
        public uint? InitialObjectCS => _model.InformationBlock?.InitialObjectCS;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialEIP"/>
#if NET48
        public uint InitialEIP => _model.InformationBlock.InitialEIP;
#else
        public uint? InitialEIP => _model.InformationBlock?.InitialEIP;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialObjectSS"/>
#if NET48
        public uint InitialObjectSS => _model.InformationBlock.InitialObjectSS;
#else
        public uint? InitialObjectSS => _model.InformationBlock?.InitialObjectSS;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialESP"/>
#if NET48
        public uint InitialESP => _model.InformationBlock.InitialESP;
#else
        public uint? InitialESP => _model.InformationBlock?.InitialESP;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.MemoryPageSize"/>
#if NET48
        public uint MemoryPageSize => _model.InformationBlock.MemoryPageSize;
#else
        public uint? MemoryPageSize => _model.InformationBlock?.MemoryPageSize;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.BytesOnLastPage"/>
#if NET48
        public uint BytesOnLastPage => _model.InformationBlock.BytesOnLastPage;
#else
        public uint? BytesOnLastPage => _model.InformationBlock?.BytesOnLastPage;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupSectionSize"/>
#if NET48
        public uint FixupSectionSize => _model.InformationBlock.FixupSectionSize;
#else
        public uint? FixupSectionSize => _model.InformationBlock?.FixupSectionSize;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupSectionChecksum"/>
#if NET48
        public uint FixupSectionChecksum => _model.InformationBlock.FixupSectionChecksum;
#else
        public uint? FixupSectionChecksum => _model.InformationBlock?.FixupSectionChecksum;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.LoaderSectionSize"/>
#if NET48
        public uint LoaderSectionSize => _model.InformationBlock.LoaderSectionSize;
#else
        public uint? LoaderSectionSize => _model.InformationBlock?.LoaderSectionSize;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.LoaderSectionChecksum"/>
#if NET48
        public uint LoaderSectionChecksum => _model.InformationBlock.LoaderSectionChecksum;
#else
        public uint? LoaderSectionChecksum => _model.InformationBlock?.LoaderSectionChecksum;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectTableOffset"/>
#if NET48
        public uint ObjectTableOffset => _model.InformationBlock.ObjectTableOffset;
#else
        public uint? ObjectTableOffset => _model.InformationBlock?.ObjectTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectTableCount"/>
#if NET48
        public uint ObjectTableCount => _model.InformationBlock.ObjectTableCount;
#else
        public uint? ObjectTableCount => _model.InformationBlock?.ObjectTableCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectPageMapOffset"/>
#if NET48
        public uint ObjectPageMapOffset => _model.InformationBlock.ObjectPageMapOffset;
#else
        public uint? ObjectPageMapOffset => _model.InformationBlock?.ObjectPageMapOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectIterateDataMapOffset"/>
#if NET48
        public uint ObjectIterateDataMapOffset => _model.InformationBlock.ObjectIterateDataMapOffset;
#else
        public uint? ObjectIterateDataMapOffset => _model.InformationBlock?.ObjectIterateDataMapOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResourceTableOffset"/>
#if NET48
        public uint ResourceTableOffset => _model.InformationBlock.ResourceTableOffset;
#else
        public uint? ResourceTableOffset => _model.InformationBlock?.ResourceTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResourceTableCount"/>
#if NET48
        public uint ResourceTableCount => _model.InformationBlock.ResourceTableCount;
#else
        public uint? ResourceTableCount => _model.InformationBlock?.ResourceTableCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResidentNamesTableOffset"/>
#if NET48
        public uint ResidentNamesTableOffset => _model.InformationBlock.ResidentNamesTableOffset;
#else
        public uint? ResidentNamesTableOffset => _model.InformationBlock?.ResidentNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.EntryTableOffset"/>
#if NET48
        public uint EntryTableOffset => _model.InformationBlock.EntryTableOffset;
#else
        public uint? EntryTableOffset => _model.InformationBlock?.EntryTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleDirectivesTableOffset"/>
#if NET48
        public uint ModuleDirectivesTableOffset => _model.InformationBlock.ModuleDirectivesTableOffset;
#else
        public uint? ModuleDirectivesTableOffset => _model.InformationBlock?.ModuleDirectivesTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleDirectivesCount"/>
#if NET48
        public uint ModuleDirectivesCount => _model.InformationBlock.ModuleDirectivesCount;
#else
        public uint? ModuleDirectivesCount => _model.InformationBlock?.ModuleDirectivesCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupPageTableOffset"/>
#if NET48
        public uint FixupPageTableOffset => _model.InformationBlock.FixupPageTableOffset;
#else
        public uint? FixupPageTableOffset => _model.InformationBlock?.FixupPageTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupRecordTableOffset"/>
#if NET48
        public uint FixupRecordTableOffset => _model.InformationBlock.FixupRecordTableOffset;
#else
        public uint? FixupRecordTableOffset => _model.InformationBlock?.FixupRecordTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportedModulesNameTableOffset"/>
#if NET48
        public uint ImportedModulesNameTableOffset => _model.InformationBlock.ImportedModulesNameTableOffset;
#else
        public uint? ImportedModulesNameTableOffset => _model.InformationBlock?.ImportedModulesNameTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportedModulesCount"/>
#if NET48
        public uint ImportedModulesCount => _model.InformationBlock.ImportedModulesCount;
#else
        public uint? ImportedModulesCount => _model.InformationBlock?.ImportedModulesCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportProcedureNameTableOffset"/>
#if NET48
        public uint ImportProcedureNameTableOffset => _model.InformationBlock.ImportProcedureNameTableOffset;
#else
        public uint? ImportProcedureNameTableOffset => _model.InformationBlock?.ImportProcedureNameTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PerPageChecksumTableOffset"/>
#if NET48
        public uint PerPageChecksumTableOffset => _model.InformationBlock.PerPageChecksumTableOffset;
#else
        public uint? PerPageChecksumTableOffset => _model.InformationBlock?.PerPageChecksumTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DataPagesOffset"/>
#if NET48
        public uint DataPagesOffset => _model.InformationBlock.DataPagesOffset;
#else
        public uint? DataPagesOffset => _model.InformationBlock?.DataPagesOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PreloadPageCount"/>
#if NET48
        public uint PreloadPageCount => _model.InformationBlock.PreloadPageCount;
#else
        public uint? PreloadPageCount => _model.InformationBlock?.PreloadPageCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableOffset"/>
#if NET48
        public uint NonResidentNamesTableOffset => _model.InformationBlock.NonResidentNamesTableOffset;
#else
        public uint? NonResidentNamesTableOffset => _model.InformationBlock?.NonResidentNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableLength"/>
#if NET48
        public uint NonResidentNamesTableLength => _model.InformationBlock.NonResidentNamesTableLength;
#else
        public uint? NonResidentNamesTableLength => _model.InformationBlock?.NonResidentNamesTableLength;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableChecksum"/>
#if NET48
        public uint NonResidentNamesTableChecksum => _model.InformationBlock.NonResidentNamesTableChecksum;
#else
        public uint? NonResidentNamesTableChecksum => _model.InformationBlock?.NonResidentNamesTableChecksum;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.AutomaticDataObject"/>
#if NET48
        public uint AutomaticDataObject => _model.InformationBlock.AutomaticDataObject;
#else
        public uint? AutomaticDataObject => _model.InformationBlock?.AutomaticDataObject;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DebugInformationOffset"/>
#if NET48
        public uint DebugInformationOffset => _model.InformationBlock.DebugInformationOffset;
#else
        public uint? DebugInformationOffset => _model.InformationBlock?.DebugInformationOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DebugInformationLength"/>
#if NET48
        public uint DebugInformationLength => _model.InformationBlock.DebugInformationLength;
#else
        public uint? DebugInformationLength => _model.InformationBlock?.DebugInformationLength;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PreloadInstancePagesNumber"/>
#if NET48
        public uint PreloadInstancePagesNumber => _model.InformationBlock.PreloadInstancePagesNumber;
#else
        public uint? PreloadInstancePagesNumber => _model.InformationBlock?.PreloadInstancePagesNumber;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DemandInstancePagesNumber"/>
#if NET48
        public uint DemandInstancePagesNumber => _model.InformationBlock.DemandInstancePagesNumber;
#else
        public uint? DemandInstancePagesNumber => _model.InformationBlock?.DemandInstancePagesNumber;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ExtraHeapAllocation"/>
#if NET48
        public uint ExtraHeapAllocation => _model.InformationBlock.ExtraHeapAllocation;
#else
        public uint? ExtraHeapAllocation => _model.InformationBlock?.ExtraHeapAllocation;
#endif

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.LinearExecutable.ObjectTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ObjectTableEntry[] ObjectTable => _model.ObjectTable;
#else
        public SabreTools.Models.LinearExecutable.ObjectTableEntry?[]? ObjectTable => _model.ObjectTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ObjectPageMap"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ObjectPageMapEntry[] ObjectPageMap => _model.ObjectPageMap;
#else
        public SabreTools.Models.LinearExecutable.ObjectPageMapEntry?[]? ObjectPageMap => _model.ObjectPageMap;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ResourceTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ResourceTableEntry[] ResourceTable => _model.ResourceTable;
#else
        public SabreTools.Models.LinearExecutable.ResourceTableEntry?[]? ResourceTable => _model.ResourceTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ResidentNamesTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ResidentNamesTableEntry[] ResidentNamesTable => _model.ResidentNamesTable;
#else
        public SabreTools.Models.LinearExecutable.ResidentNamesTableEntry?[]? ResidentNamesTable => _model.ResidentNamesTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.EntryTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.EntryTableBundle[] EntryTable => _model.EntryTable;
#else
        public SabreTools.Models.LinearExecutable.EntryTableBundle?[]? EntryTable => _model.EntryTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ModuleFormatDirectivesTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ModuleFormatDirectivesTableEntry[] ModuleFormatDirectivesTable => _model.ModuleFormatDirectivesTable;
#else
        public SabreTools.Models.LinearExecutable.ModuleFormatDirectivesTableEntry?[]? ModuleFormatDirectivesTable => _model.ModuleFormatDirectivesTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.VerifyRecordDirectiveTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.VerifyRecordDirectiveTableEntry[] VerifyRecordDirectiveTable => _model.VerifyRecordDirectiveTable;
#else
        public SabreTools.Models.LinearExecutable.VerifyRecordDirectiveTableEntry?[]? VerifyRecordDirectiveTable => _model.VerifyRecordDirectiveTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.PerPageChecksumTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.PerPageChecksumTableEntry[] PerPageChecksumTable => _model.PerPageChecksumTable;
#else
        public SabreTools.Models.LinearExecutable.PerPageChecksumTableEntry?[]? PerPageChecksumTable => _model.PerPageChecksumTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.FixupPageTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.FixupPageTableEntry[] FixupPageTable => _model.FixupPageTable;
#else
        public SabreTools.Models.LinearExecutable.FixupPageTableEntry?[]? FixupPageTable => _model.FixupPageTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.FixupRecordTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.FixupRecordTableEntry[] FixupRecordTable => _model.FixupRecordTable;
#else
        public SabreTools.Models.LinearExecutable.FixupRecordTableEntry?[]? FixupRecordTable => _model.FixupRecordTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ImportModuleNameTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ImportModuleNameTableEntry[] ImportModuleNameTable => _model.ImportModuleNameTable;
#else
        public SabreTools.Models.LinearExecutable.ImportModuleNameTableEntry?[]? ImportModuleNameTable => _model.ImportModuleNameTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ImportModuleProcedureNameTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ImportModuleProcedureNameTableEntry[] ImportModuleProcedureNameTable => _model.ImportModuleProcedureNameTable;
#else
        public SabreTools.Models.LinearExecutable.ImportModuleProcedureNameTableEntry?[]? ImportModuleProcedureNameTable => _model.ImportModuleProcedureNameTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.NonResidentNamesTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.NonResidentNamesTableEntry[] NonResidentNamesTable => _model.NonResidentNamesTable;
#else
        public SabreTools.Models.LinearExecutable.NonResidentNamesTableEntry?[]? NonResidentNamesTable => _model.NonResidentNamesTable;
#endif

        #endregion

        #region Debug Information

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.Signature"/>
#if NET48
        public string DI_Signature => _model.DebugInformation?.Signature;
#else
        public string? DI_Signature => _model.DebugInformation?.Signature;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.FormatType"/>
        public SabreTools.Models.LinearExecutable.DebugFormatType? DI_FormatType => _model.DebugInformation?.FormatType;

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.DebuggerData"/>
#if NET48
        public byte[] DebuggerData => _model.DebugInformation?.DebuggerData;
#else
        public byte[]? DebuggerData => _model.DebugInformation?.DebuggerData;
#endif

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Determine what extension properties are needed

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public LinearExecutable(SabreTools.Models.LinearExecutable.Executable model, byte[] data, int offset)
#else
        public LinearExecutable(SabreTools.Models.LinearExecutable.Executable? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public LinearExecutable(SabreTools.Models.LinearExecutable.Executable model, Stream data)
#else
        public LinearExecutable(SabreTools.Models.LinearExecutable.Executable? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }/// <summary>
         /// Create an LE/LX executable from a byte array and offset
         /// </summary>
         /// <param name="data">Byte array representing the executable</param>
         /// <param name="offset">Offset within the array to parse</param>
         /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
#if NET48
        public static LinearExecutable Create(byte[] data, int offset)
#else
        public static LinearExecutable? Create(byte[]? data, int offset)
#endif
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
#if NET48
        public static LinearExecutable Create(Stream data)
#else
        public static LinearExecutable? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var executable = new SabreTools.Serialization.Streams.LinearExecutable().Deserialize(data);
            if (executable == null)
                return null;

            try
            {
                return new LinearExecutable(executable, data);
            }
            catch
            {
                return null;
            }
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
            builder.AppendLine($"  Reserved words: {(Stub_Reserved1 == null ? "[NULL]" : string.Join(", ", Stub_Reserved1))}");
            builder.AppendLine($"  OEM identifier: {Stub_OEMIdentifier} (0x{Stub_OEMIdentifier:X})");
            builder.AppendLine($"  OEM information: {Stub_OEMInformation} (0x{Stub_OEMInformation:X})");
            builder.AppendLine($"  Reserved words: {(Stub_Reserved2 == null ? "[NULL]" : string.Join(", ", Stub_Reserved2))}");
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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (bundle == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                            if (entry == null)
                            {
                                builder.AppendLine("    [NULL]");
                                continue;
                            }

                            switch (bundle.BundleType & ~SabreTools.Models.LinearExecutable.BundleType.ParameterTypingInformationPresent)
                            {
                                case SabreTools.Models.LinearExecutable.BundleType.UnusedEntry:
                                    builder.AppendLine($"      Unused, empty entry");
                                    break;

                                case SabreTools.Models.LinearExecutable.BundleType.SixteenBitEntry:
                                    builder.AppendLine($"      Object number: {entry.SixteenBitObjectNumber} (0x{entry.SixteenBitObjectNumber:X})");
                                    builder.AppendLine($"      Entry flags: {entry.SixteenBitEntryFlags} (0x{entry.SixteenBitEntryFlags:X})");
                                    builder.AppendLine($"      Offset: {entry.SixteenBitOffset} (0x{entry.SixteenBitOffset:X})");
                                    break;

                                case SabreTools.Models.LinearExecutable.BundleType.TwoEightySixCallGateEntry:
                                    builder.AppendLine($"      Object number: {entry.TwoEightySixObjectNumber} (0x{entry.TwoEightySixObjectNumber:X})");
                                    builder.AppendLine($"      Entry flags: {entry.TwoEightySixEntryFlags} (0x{entry.TwoEightySixEntryFlags:X})");
                                    builder.AppendLine($"      Offset: {entry.TwoEightySixOffset} (0x{entry.TwoEightySixOffset:X})");
                                    builder.AppendLine($"      Callgate: {entry.TwoEightySixCallgate} (0x{entry.TwoEightySixCallgate:X})");
                                    break;

                                case SabreTools.Models.LinearExecutable.BundleType.ThirtyTwoBitEntry:
                                    builder.AppendLine($"      Object number: {entry.ThirtyTwoBitObjectNumber} (0x{entry.ThirtyTwoBitObjectNumber:X})");
                                    builder.AppendLine($"      Entry flags: {entry.ThirtyTwoBitEntryFlags} (0x{entry.ThirtyTwoBitEntryFlags:X})");
                                    builder.AppendLine($"      Offset: {entry.ThirtyTwoBitOffset} (0x{entry.ThirtyTwoBitOffset:X})");
                                    break;

                                case SabreTools.Models.LinearExecutable.BundleType.ForwarderEntry:
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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Source type: {entry.SourceType} (0x{entry.SourceType:X})");
                    builder.AppendLine($"    Target flags: {entry.TargetFlags} (0x{entry.TargetFlags:X})");

                    // Source list flag
                    if (entry.SourceType.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordSourceType.SourceListFlag))
                        builder.AppendLine($"    Source offset list count: {entry.SourceOffsetListCount} (0x{entry.SourceOffsetListCount:X})");
                    else
                        builder.AppendLine($"    Source offset: {entry.SourceOffset} (0x{entry.SourceOffset:X})");

                    // OBJECT / TRGOFF
                    if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.InternalReference))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                        else
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                        // 16-bit Selector fixup
                        if (!entry.SourceType.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordSourceType.SixteenBitSelectorFixup))
                        {
                            // 32-bit Target Offset Flag
                            if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                                builder.AppendLine($"    Target offset: {entry.TargetOffsetDWORD} (0x{entry.TargetOffsetDWORD:X})");
                            else
                                builder.AppendLine($"    Target offset: {entry.TargetOffsetWORD} (0x{entry.TargetOffsetWORD:X})");
                        }
                    }

                    // MOD ORD# / IMPORT ORD / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ImportedReferenceByOrdinal))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                        else
                            builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                        // 8-bit Ordinal Flag & 32-bit Target Offset Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.EightBitOrdinalFlag))
                            builder.AppendLine($"    Imported ordinal number: {entry.ImportedOrdinalNumberByte} (0x{entry.ImportedOrdinalNumberByte:X})");
                        else if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                            builder.AppendLine($"    Imported ordinal number: {entry.ImportedOrdinalNumberDWORD} (0x{entry.ImportedOrdinalNumberDWORD:X})");
                        else
                            builder.AppendLine($"    Imported ordinal number: {entry.ImportedOrdinalNumberWORD} (0x{entry.ImportedOrdinalNumberWORD:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ImportedReferenceByName))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                        else
                            builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                        // 32-bit Target Offset Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                            builder.AppendLine($"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableDWORD} (0x{entry.OffsetImportProcedureNameTableDWORD:X})");
                        else
                            builder.AppendLine($"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableWORD} (0x{entry.OffsetImportProcedureNameTableWORD:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                        }
                    }

                    // ORD # / ADDITIVE
                    else if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.InternalReferenceViaEntryTable))
                    {
                        // 16-bit Object Number/Module Ordinal Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                        else
                            builder.AppendLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                        // Additive Fixup Flag
                        if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.AdditiveFixupFlag))
                        {
                            // 32-bit Additive Flag
                            if (entry.TargetFlags.HasFlag(SabreTools.Models.LinearExecutable.FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                                builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                            else
                                builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
            if (_model.DebugInformation == null)
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
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

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
#if NET48
        public byte[] ReadArbitraryRange(int rangeStart = -1, int length = -1)
#else
        public byte[]? ReadArbitraryRange(int rangeStart = -1, int length = -1)
#endif
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
#if NET48
                        length = _byteArrayData.Length - _byteArrayOffset;
#else
                        length = _byteArrayData!.Length - _byteArrayOffset;
#endif
                        break;

                    case DataSource.Stream:
#if NET48
                        length = (int)_streamData.Length;
#else
                        length = (int)_streamData!.Length;
#endif
                        break;
                }
            }

            return ReadFromDataSource(rangeStart, length);
        }

        #endregion
    }
}