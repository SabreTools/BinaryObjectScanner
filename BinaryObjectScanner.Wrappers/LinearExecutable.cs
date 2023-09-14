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
            Printing.LinearExecutable.Print(builder, _model);
            return builder;
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