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
        public string Stub_Magic => this.Model.Stub.Header.Magic;
#else
        public string? Stub_Magic => this.Model.Stub?.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
#if NET48
        public ushort Stub_LastPageBytes => this.Model.Stub.Header.LastPageBytes;
#else
        public ushort? Stub_LastPageBytes => this.Model.Stub?.Header?.LastPageBytes;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
#if NET48
        public ushort Stub_Pages => this.Model.Stub.Header.Pages;
#else
        public ushort? Stub_Pages => this.Model.Stub?.Header?.Pages;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
#if NET48
        public ushort Stub_RelocationItems => this.Model.Stub.Header.RelocationItems;
#else
        public ushort? Stub_RelocationItems => this.Model.Stub?.Header?.RelocationItems;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
#if NET48
        public ushort Stub_HeaderParagraphSize => this.Model.Stub.Header.HeaderParagraphSize;
#else
        public ushort? Stub_HeaderParagraphSize => this.Model.Stub?.Header?.HeaderParagraphSize;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
#if NET48
        public ushort Stub_MinimumExtraParagraphs => this.Model.Stub.Header.MinimumExtraParagraphs;
#else
        public ushort? Stub_MinimumExtraParagraphs => this.Model.Stub?.Header?.MinimumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
#if NET48
        public ushort Stub_MaximumExtraParagraphs => this.Model.Stub.Header.MaximumExtraParagraphs;
#else
        public ushort? Stub_MaximumExtraParagraphs => this.Model.Stub?.Header?.MaximumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
#if NET48
        public ushort Stub_InitialSSValue => this.Model.Stub.Header.InitialSSValue;
#else
        public ushort? Stub_InitialSSValue => this.Model.Stub?.Header?.InitialSSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
#if NET48
        public ushort Stub_InitialSPValue => this.Model.Stub.Header.InitialSPValue;
#else
        public ushort? Stub_InitialSPValue => this.Model.Stub?.Header?.InitialSPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
#if NET48
        public ushort Stub_Checksum => this.Model.Stub.Header.Checksum;
#else
        public ushort? Stub_Checksum => this.Model.Stub?.Header?.Checksum;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
#if NET48
        public ushort Stub_InitialIPValue => this.Model.Stub.Header.InitialIPValue;
#else
        public ushort? Stub_InitialIPValue => this.Model.Stub?.Header?.InitialIPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
#if NET48
        public ushort Stub_InitialCSValue => this.Model.Stub.Header.InitialCSValue;
#else
        public ushort? Stub_InitialCSValue => this.Model.Stub?.Header?.InitialCSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
#if NET48
        public ushort Stub_RelocationTableAddr => this.Model.Stub.Header.RelocationTableAddr;
#else
        public ushort? Stub_RelocationTableAddr => this.Model.Stub?.Header?.RelocationTableAddr;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
#if NET48
        public ushort Stub_OverlayNumber => this.Model.Stub.Header.OverlayNumber;
#else
        public ushort? Stub_OverlayNumber => this.Model.Stub?.Header?.OverlayNumber;
#endif

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
#if NET48
        public ushort[] Stub_Reserved1 => this.Model.Stub.Header.Reserved1;
#else
        public ushort[]? Stub_Reserved1 => this.Model.Stub?.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
#if NET48
        public ushort Stub_OEMIdentifier => this.Model.Stub.Header.OEMIdentifier;
#else
        public ushort? Stub_OEMIdentifier => this.Model.Stub?.Header?.OEMIdentifier;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
#if NET48
        public ushort Stub_OEMInformation => this.Model.Stub.Header.OEMInformation;
#else
        public ushort? Stub_OEMInformation => this.Model.Stub?.Header?.OEMInformation;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
#if NET48
        public ushort[] Stub_Reserved2 => this.Model.Stub.Header.Reserved2;
#else
        public ushort[]? Stub_Reserved2 => Model?.Stub?.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
#if NET48
        public uint Stub_NewExeHeaderAddr => this.Model.Stub.Header.NewExeHeaderAddr;
#else
        public uint? Stub_NewExeHeaderAddr => this.Model.Stub?.Header?.NewExeHeaderAddr;
#endif

        #endregion

        #endregion

        #region Information Block

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.Signature"/>
#if NET48
        public string Signature => this.Model.InformationBlock.Signature;
#else
        public string? Signature => this.Model.InformationBlock?.Signature;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ByteOrder"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ByteOrder ByteOrder => this.Model.InformationBlock.ByteOrder;
#else
        public SabreTools.Models.LinearExecutable.ByteOrder? ByteOrder => this.Model.InformationBlock?.ByteOrder;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.WordOrder"/>
#if NET48
        public SabreTools.Models.LinearExecutable.WordOrder WordOrder => this.Model.InformationBlock.WordOrder;
#else
        public SabreTools.Models.LinearExecutable.WordOrder? WordOrder => this.Model.InformationBlock?.WordOrder;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ExecutableFormatLevel"/>
#if NET48
        public uint ExecutableFormatLevel => this.Model.InformationBlock.ExecutableFormatLevel;
#else
        public uint? ExecutableFormatLevel => this.Model.InformationBlock?.ExecutableFormatLevel;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.CPUType"/>
#if NET48
        public SabreTools.Models.LinearExecutable.CPUType CPUType => this.Model.InformationBlock.CPUType;
#else
        public SabreTools.Models.LinearExecutable.CPUType? CPUType => this.Model.InformationBlock?.CPUType;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleOS"/>
#if NET48
        public SabreTools.Models.LinearExecutable.OperatingSystem ModuleOS => this.Model.InformationBlock.ModuleOS;
#else
        public SabreTools.Models.LinearExecutable.OperatingSystem? ModuleOS => this.Model.InformationBlock?.ModuleOS;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleVersion"/>
#if NET48
        public uint ModuleVersion => this.Model.InformationBlock.ModuleVersion;
#else
        public uint? ModuleVersion => this.Model.InformationBlock?.ModuleVersion;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleTypeFlags"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ModuleFlags ModuleTypeFlags => this.Model.InformationBlock.ModuleTypeFlags;
#else
        public SabreTools.Models.LinearExecutable.ModuleFlags? ModuleTypeFlags => this.Model.InformationBlock?.ModuleTypeFlags;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleNumberPages"/>
#if NET48
        public uint ModuleNumberPages => this.Model.InformationBlock.ModuleNumberPages;
#else
        public uint? ModuleNumberPages => this.Model.InformationBlock?.ModuleNumberPages;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialObjectCS"/>
#if NET48
        public uint InitialObjectCS => this.Model.InformationBlock.InitialObjectCS;
#else
        public uint? InitialObjectCS => this.Model.InformationBlock?.InitialObjectCS;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialEIP"/>
#if NET48
        public uint InitialEIP => this.Model.InformationBlock.InitialEIP;
#else
        public uint? InitialEIP => this.Model.InformationBlock?.InitialEIP;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialObjectSS"/>
#if NET48
        public uint InitialObjectSS => this.Model.InformationBlock.InitialObjectSS;
#else
        public uint? InitialObjectSS => this.Model.InformationBlock?.InitialObjectSS;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.InitialESP"/>
#if NET48
        public uint InitialESP => this.Model.InformationBlock.InitialESP;
#else
        public uint? InitialESP => this.Model.InformationBlock?.InitialESP;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.MemoryPageSize"/>
#if NET48
        public uint MemoryPageSize => this.Model.InformationBlock.MemoryPageSize;
#else
        public uint? MemoryPageSize => this.Model.InformationBlock?.MemoryPageSize;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.BytesOnLastPage"/>
#if NET48
        public uint BytesOnLastPage => this.Model.InformationBlock.BytesOnLastPage;
#else
        public uint? BytesOnLastPage => this.Model.InformationBlock?.BytesOnLastPage;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupSectionSize"/>
#if NET48
        public uint FixupSectionSize => this.Model.InformationBlock.FixupSectionSize;
#else
        public uint? FixupSectionSize => this.Model.InformationBlock?.FixupSectionSize;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupSectionChecksum"/>
#if NET48
        public uint FixupSectionChecksum => this.Model.InformationBlock.FixupSectionChecksum;
#else
        public uint? FixupSectionChecksum => this.Model.InformationBlock?.FixupSectionChecksum;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.LoaderSectionSize"/>
#if NET48
        public uint LoaderSectionSize => this.Model.InformationBlock.LoaderSectionSize;
#else
        public uint? LoaderSectionSize => this.Model.InformationBlock?.LoaderSectionSize;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.LoaderSectionChecksum"/>
#if NET48
        public uint LoaderSectionChecksum => this.Model.InformationBlock.LoaderSectionChecksum;
#else
        public uint? LoaderSectionChecksum => this.Model.InformationBlock?.LoaderSectionChecksum;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectTableOffset"/>
#if NET48
        public uint ObjectTableOffset => this.Model.InformationBlock.ObjectTableOffset;
#else
        public uint? ObjectTableOffset => this.Model.InformationBlock?.ObjectTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectTableCount"/>
#if NET48
        public uint ObjectTableCount => this.Model.InformationBlock.ObjectTableCount;
#else
        public uint? ObjectTableCount => this.Model.InformationBlock?.ObjectTableCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectPageMapOffset"/>
#if NET48
        public uint ObjectPageMapOffset => this.Model.InformationBlock.ObjectPageMapOffset;
#else
        public uint? ObjectPageMapOffset => this.Model.InformationBlock?.ObjectPageMapOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ObjectIterateDataMapOffset"/>
#if NET48
        public uint ObjectIterateDataMapOffset => this.Model.InformationBlock.ObjectIterateDataMapOffset;
#else
        public uint? ObjectIterateDataMapOffset => this.Model.InformationBlock?.ObjectIterateDataMapOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResourceTableOffset"/>
#if NET48
        public uint ResourceTableOffset => this.Model.InformationBlock.ResourceTableOffset;
#else
        public uint? ResourceTableOffset => this.Model.InformationBlock?.ResourceTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResourceTableCount"/>
#if NET48
        public uint ResourceTableCount => this.Model.InformationBlock.ResourceTableCount;
#else
        public uint? ResourceTableCount => this.Model.InformationBlock?.ResourceTableCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ResidentNamesTableOffset"/>
#if NET48
        public uint ResidentNamesTableOffset => this.Model.InformationBlock.ResidentNamesTableOffset;
#else
        public uint? ResidentNamesTableOffset => this.Model.InformationBlock?.ResidentNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.EntryTableOffset"/>
#if NET48
        public uint EntryTableOffset => this.Model.InformationBlock.EntryTableOffset;
#else
        public uint? EntryTableOffset => this.Model.InformationBlock?.EntryTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleDirectivesTableOffset"/>
#if NET48
        public uint ModuleDirectivesTableOffset => this.Model.InformationBlock.ModuleDirectivesTableOffset;
#else
        public uint? ModuleDirectivesTableOffset => this.Model.InformationBlock?.ModuleDirectivesTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ModuleDirectivesCount"/>
#if NET48
        public uint ModuleDirectivesCount => this.Model.InformationBlock.ModuleDirectivesCount;
#else
        public uint? ModuleDirectivesCount => this.Model.InformationBlock?.ModuleDirectivesCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupPageTableOffset"/>
#if NET48
        public uint FixupPageTableOffset => this.Model.InformationBlock.FixupPageTableOffset;
#else
        public uint? FixupPageTableOffset => this.Model.InformationBlock?.FixupPageTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.FixupRecordTableOffset"/>
#if NET48
        public uint FixupRecordTableOffset => this.Model.InformationBlock.FixupRecordTableOffset;
#else
        public uint? FixupRecordTableOffset => this.Model.InformationBlock?.FixupRecordTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportedModulesNameTableOffset"/>
#if NET48
        public uint ImportedModulesNameTableOffset => this.Model.InformationBlock.ImportedModulesNameTableOffset;
#else
        public uint? ImportedModulesNameTableOffset => this.Model.InformationBlock?.ImportedModulesNameTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportedModulesCount"/>
#if NET48
        public uint ImportedModulesCount => this.Model.InformationBlock.ImportedModulesCount;
#else
        public uint? ImportedModulesCount => this.Model.InformationBlock?.ImportedModulesCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ImportProcedureNameTableOffset"/>
#if NET48
        public uint ImportProcedureNameTableOffset => this.Model.InformationBlock.ImportProcedureNameTableOffset;
#else
        public uint? ImportProcedureNameTableOffset => this.Model.InformationBlock?.ImportProcedureNameTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PerPageChecksumTableOffset"/>
#if NET48
        public uint PerPageChecksumTableOffset => this.Model.InformationBlock.PerPageChecksumTableOffset;
#else
        public uint? PerPageChecksumTableOffset => this.Model.InformationBlock?.PerPageChecksumTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DataPagesOffset"/>
#if NET48
        public uint DataPagesOffset => this.Model.InformationBlock.DataPagesOffset;
#else
        public uint? DataPagesOffset => this.Model.InformationBlock?.DataPagesOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PreloadPageCount"/>
#if NET48
        public uint PreloadPageCount => this.Model.InformationBlock.PreloadPageCount;
#else
        public uint? PreloadPageCount => this.Model.InformationBlock?.PreloadPageCount;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableOffset"/>
#if NET48
        public uint NonResidentNamesTableOffset => this.Model.InformationBlock.NonResidentNamesTableOffset;
#else
        public uint? NonResidentNamesTableOffset => this.Model.InformationBlock?.NonResidentNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableLength"/>
#if NET48
        public uint NonResidentNamesTableLength => this.Model.InformationBlock.NonResidentNamesTableLength;
#else
        public uint? NonResidentNamesTableLength => this.Model.InformationBlock?.NonResidentNamesTableLength;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.NonResidentNamesTableChecksum"/>
#if NET48
        public uint NonResidentNamesTableChecksum => this.Model.InformationBlock.NonResidentNamesTableChecksum;
#else
        public uint? NonResidentNamesTableChecksum => this.Model.InformationBlock?.NonResidentNamesTableChecksum;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.AutomaticDataObject"/>
#if NET48
        public uint AutomaticDataObject => this.Model.InformationBlock.AutomaticDataObject;
#else
        public uint? AutomaticDataObject => this.Model.InformationBlock?.AutomaticDataObject;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DebugInformationOffset"/>
#if NET48
        public uint DebugInformationOffset => this.Model.InformationBlock.DebugInformationOffset;
#else
        public uint? DebugInformationOffset => this.Model.InformationBlock?.DebugInformationOffset;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DebugInformationLength"/>
#if NET48
        public uint DebugInformationLength => this.Model.InformationBlock.DebugInformationLength;
#else
        public uint? DebugInformationLength => this.Model.InformationBlock?.DebugInformationLength;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.PreloadInstancePagesNumber"/>
#if NET48
        public uint PreloadInstancePagesNumber => this.Model.InformationBlock.PreloadInstancePagesNumber;
#else
        public uint? PreloadInstancePagesNumber => this.Model.InformationBlock?.PreloadInstancePagesNumber;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.DemandInstancePagesNumber"/>
#if NET48
        public uint DemandInstancePagesNumber => this.Model.InformationBlock.DemandInstancePagesNumber;
#else
        public uint? DemandInstancePagesNumber => this.Model.InformationBlock?.DemandInstancePagesNumber;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.InformationBlock.ExtraHeapAllocation"/>
#if NET48
        public uint ExtraHeapAllocation => this.Model.InformationBlock.ExtraHeapAllocation;
#else
        public uint? ExtraHeapAllocation => this.Model.InformationBlock?.ExtraHeapAllocation;
#endif

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.LinearExecutable.ObjectTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ObjectTableEntry[] ObjectTable => this.Model.ObjectTable;
#else
        public SabreTools.Models.LinearExecutable.ObjectTableEntry?[]? ObjectTable => this.Model.ObjectTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ObjectPageMap"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ObjectPageMapEntry[] ObjectPageMap => this.Model.ObjectPageMap;
#else
        public SabreTools.Models.LinearExecutable.ObjectPageMapEntry?[]? ObjectPageMap => this.Model.ObjectPageMap;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ResourceTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ResourceTableEntry[] ResourceTable => this.Model.ResourceTable;
#else
        public SabreTools.Models.LinearExecutable.ResourceTableEntry?[]? ResourceTable => this.Model.ResourceTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ResidentNamesTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ResidentNamesTableEntry[] ResidentNamesTable => this.Model.ResidentNamesTable;
#else
        public SabreTools.Models.LinearExecutable.ResidentNamesTableEntry?[]? ResidentNamesTable => this.Model.ResidentNamesTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.EntryTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.EntryTableBundle[] EntryTable => this.Model.EntryTable;
#else
        public SabreTools.Models.LinearExecutable.EntryTableBundle?[]? EntryTable => this.Model.EntryTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ModuleFormatDirectivesTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ModuleFormatDirectivesTableEntry[] ModuleFormatDirectivesTable => this.Model.ModuleFormatDirectivesTable;
#else
        public SabreTools.Models.LinearExecutable.ModuleFormatDirectivesTableEntry?[]? ModuleFormatDirectivesTable => this.Model.ModuleFormatDirectivesTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.VerifyRecordDirectiveTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.VerifyRecordDirectiveTableEntry[] VerifyRecordDirectiveTable => this.Model.VerifyRecordDirectiveTable;
#else
        public SabreTools.Models.LinearExecutable.VerifyRecordDirectiveTableEntry?[]? VerifyRecordDirectiveTable => this.Model.VerifyRecordDirectiveTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.PerPageChecksumTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.PerPageChecksumTableEntry[] PerPageChecksumTable => this.Model.PerPageChecksumTable;
#else
        public SabreTools.Models.LinearExecutable.PerPageChecksumTableEntry?[]? PerPageChecksumTable => this.Model.PerPageChecksumTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.FixupPageTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.FixupPageTableEntry[] FixupPageTable => this.Model.FixupPageTable;
#else
        public SabreTools.Models.LinearExecutable.FixupPageTableEntry?[]? FixupPageTable => this.Model.FixupPageTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.FixupRecordTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.FixupRecordTableEntry[] FixupRecordTable => this.Model.FixupRecordTable;
#else
        public SabreTools.Models.LinearExecutable.FixupRecordTableEntry?[]? FixupRecordTable => this.Model.FixupRecordTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ImportModuleNameTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ImportModuleNameTableEntry[] ImportModuleNameTable => this.Model.ImportModuleNameTable;
#else
        public SabreTools.Models.LinearExecutable.ImportModuleNameTableEntry?[]? ImportModuleNameTable => this.Model.ImportModuleNameTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.ImportModuleProcedureNameTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.ImportModuleProcedureNameTableEntry[] ImportModuleProcedureNameTable => this.Model.ImportModuleProcedureNameTable;
#else
        public SabreTools.Models.LinearExecutable.ImportModuleProcedureNameTableEntry?[]? ImportModuleProcedureNameTable => this.Model.ImportModuleProcedureNameTable;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.NonResidentNamesTable"/>
#if NET48
        public SabreTools.Models.LinearExecutable.NonResidentNamesTableEntry[] NonResidentNamesTable => this.Model.NonResidentNamesTable;
#else
        public SabreTools.Models.LinearExecutable.NonResidentNamesTableEntry?[]? NonResidentNamesTable => this.Model.NonResidentNamesTable;
#endif

        #endregion

        #region Debug Information

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.Signature"/>
#if NET48
        public string DI_Signature => this.Model.DebugInformation?.Signature;
#else
        public string? DI_Signature => this.Model.DebugInformation?.Signature;
#endif

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.FormatType"/>
        public SabreTools.Models.LinearExecutable.DebugFormatType? DI_FormatType => this.Model.DebugInformation?.FormatType;

        /// <inheritdoc cref="Models.LinearExecutable.DebugInformation.DebuggerData"/>
#if NET48
        public byte[] DebuggerData => this.Model.DebugInformation?.DebuggerData;
#else
        public byte[]? DebuggerData => this.Model.DebugInformation?.DebuggerData;
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
            Printing.LinearExecutable.Print(builder, this.Model);
            return builder;
        }

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