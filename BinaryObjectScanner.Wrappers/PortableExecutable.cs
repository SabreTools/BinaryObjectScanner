using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO;
using static SabreTools.Serialization.Extensions;

namespace BinaryObjectScanner.Wrappers
{
    public class PortableExecutable : WrapperBase<SabreTools.Models.PortableExecutable.Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Portable Executable (PE)";

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
        public ushort[]? Stub_Reserved2 => _model.Stub?.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
#if NET48
        public uint Stub_NewExeHeaderAddr => _model.Stub.Header.NewExeHeaderAddr;
#else
        public uint? Stub_NewExeHeaderAddr => _model.Stub?.Header?.NewExeHeaderAddr;
#endif

        #endregion

        #endregion

        /// <inheritdoc cref="Models.PortableExecutable.Executable.Signature"/>
#if NET48
        public string Signature => _model.Signature;
#else
        public string? Signature => _model.Signature;
#endif

        #region COFF File Header

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.Machine"/>
#if NET48
        public SabreTools.Models.PortableExecutable.MachineType Machine => _model.COFFFileHeader.Machine;
#else
        public SabreTools.Models.PortableExecutable.MachineType? Machine => _model.COFFFileHeader?.Machine;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.NumberOfSections"/>
#if NET48
        public ushort NumberOfSections => _model.COFFFileHeader.NumberOfSections;
#else
        public ushort? NumberOfSections => _model.COFFFileHeader?.NumberOfSections;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.TimeDateStamp"/>
#if NET48
        public uint TimeDateStamp => _model.COFFFileHeader.TimeDateStamp;
#else
        public uint? TimeDateStamp => _model.COFFFileHeader?.TimeDateStamp;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.PointerToSymbolTable"/>
#if NET48
        public uint PointerToSymbolTable => _model.COFFFileHeader.PointerToSymbolTable;
#else
        public uint? PointerToSymbolTable => _model.COFFFileHeader?.PointerToSymbolTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.NumberOfSymbols"/>
#if NET48
        public uint NumberOfSymbols => _model.COFFFileHeader.NumberOfSymbols;
#else
        public uint? NumberOfSymbols => _model.COFFFileHeader?.NumberOfSymbols;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.SizeOfOptionalHeader"/>
#if NET48
        public uint SizeOfOptionalHeader => _model.COFFFileHeader.SizeOfOptionalHeader;
#else
        public uint? SizeOfOptionalHeader => _model.COFFFileHeader?.SizeOfOptionalHeader;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.Characteristics"/>
#if NET48
        public SabreTools.Models.PortableExecutable.Characteristics Characteristics => _model.COFFFileHeader.Characteristics;
#else
        public SabreTools.Models.PortableExecutable.Characteristics? Characteristics => _model.COFFFileHeader?.Characteristics;
#endif

        #endregion

        #region Optional Header

        #region Standard Fields

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Machine"/>
#if NET48
        public SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber OH_Magic => _model.OptionalHeader.Magic;
#else
        public SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber? OH_Magic => _model.OptionalHeader?.Magic;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorLinkerVersion"/>
#if NET48
        public byte OH_MajorLinkerVersion => _model.OptionalHeader.MajorLinkerVersion;
#else
        public byte? OH_MajorLinkerVersion => _model.OptionalHeader?.MajorLinkerVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorLinkerVersion"/>
#if NET48
        public byte OH_MinorLinkerVersion => _model.OptionalHeader.MinorLinkerVersion;
#else
        public byte? OH_MinorLinkerVersion => _model.OptionalHeader?.MinorLinkerVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfCode"/>
#if NET48
        public uint OH_SizeOfCode => _model.OptionalHeader.SizeOfCode;
#else
        public uint? OH_SizeOfCode => _model.OptionalHeader?.SizeOfCode;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfInitializedData"/>
#if NET48
        public uint OH_SizeOfInitializedData => _model.OptionalHeader.SizeOfInitializedData;
#else
        public uint? OH_SizeOfInitializedData => _model.OptionalHeader?.SizeOfInitializedData;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfUninitializedData"/>
#if NET48
        public uint OH_SizeOfUninitializedData => _model.OptionalHeader.SizeOfUninitializedData;
#else
        public uint? OH_SizeOfUninitializedData => _model.OptionalHeader?.SizeOfUninitializedData;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.AddressOfEntryPoint"/>
#if NET48
        public uint OH_AddressOfEntryPoint => _model.OptionalHeader.AddressOfEntryPoint;
#else
        public uint? OH_AddressOfEntryPoint => _model.OptionalHeader?.AddressOfEntryPoint;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseOfCode"/>
#if NET48
        public uint OH_BaseOfCode => _model.OptionalHeader.BaseOfCode;
#else
        public uint? OH_BaseOfCode => _model.OptionalHeader?.BaseOfCode;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseOfData"/>
        public uint? OH_BaseOfData => _model.OptionalHeader?.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? (uint?)_model.OptionalHeader.BaseOfData
            : null;

        #endregion

        #region Windows-Specific Fields

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImageBase_PE32"/>
        public ulong OH_ImageBase => _model.OptionalHeader?.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.ImageBase_PE32
            : _model.OptionalHeader?.ImageBase_PE32Plus ?? 0;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SectionAlignment"/>
#if NET48
        public uint OH_SectionAlignment => _model.OptionalHeader.SectionAlignment;
#else
        public uint? OH_SectionAlignment => _model.OptionalHeader?.SectionAlignment;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.FileAlignment"/>
#if NET48
        public uint OH_FileAlignment => _model.OptionalHeader.FileAlignment;
#else
        public uint? OH_FileAlignment => _model.OptionalHeader?.FileAlignment;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorOperatingSystemVersion"/>
#if NET48
        public ushort OH_MajorOperatingSystemVersion => _model.OptionalHeader.MajorOperatingSystemVersion;
#else
        public ushort? OH_MajorOperatingSystemVersion => _model.OptionalHeader?.MajorOperatingSystemVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorOperatingSystemVersion"/>
#if NET48
        public ushort OH_MinorOperatingSystemVersion => _model.OptionalHeader.MinorOperatingSystemVersion;
#else
        public ushort? OH_MinorOperatingSystemVersion => _model.OptionalHeader?.MinorOperatingSystemVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorImageVersion"/>
#if NET48
        public ushort OH_MajorImageVersion => _model.OptionalHeader.MajorImageVersion;
#else
        public ushort? OH_MajorImageVersion => _model.OptionalHeader?.MajorImageVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorImageVersion"/>
#if NET48
        public ushort OH_MinorImageVersion => _model.OptionalHeader.MinorImageVersion;
#else
        public ushort? OH_MinorImageVersion => _model.OptionalHeader?.MinorImageVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorSubsystemVersion"/>
#if NET48
        public ushort OH_MajorSubsystemVersion => _model.OptionalHeader.MajorSubsystemVersion;
#else
        public ushort? OH_MajorSubsystemVersion => _model.OptionalHeader?.MajorSubsystemVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorSubsystemVersion"/>
#if NET48
        public ushort OH_MinorSubsystemVersion => _model.OptionalHeader.MinorSubsystemVersion;
#else
        public ushort? OH_MinorSubsystemVersion => _model.OptionalHeader?.MinorSubsystemVersion;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Win32VersionValue"/>
#if NET48
        public uint OH_Win32VersionValue => _model.OptionalHeader.Win32VersionValue;
#else
        public uint? OH_Win32VersionValue => _model.OptionalHeader?.Win32VersionValue;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfImage"/>
#if NET48
        public uint OH_SizeOfImage => _model.OptionalHeader.SizeOfImage;
#else
        public uint? OH_SizeOfImage => _model.OptionalHeader?.SizeOfImage;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeaders"/>
#if NET48
        public uint OH_SizeOfHeaders => _model.OptionalHeader.SizeOfHeaders;
#else
        public uint? OH_SizeOfHeaders => _model.OptionalHeader?.SizeOfHeaders;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CheckSum"/>
#if NET48
        public uint OH_CheckSum => _model.OptionalHeader.CheckSum;
#else
        public uint? OH_CheckSum => _model.OptionalHeader?.CheckSum;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Subsystem"/>
#if NET48
        public SabreTools.Models.PortableExecutable.WindowsSubsystem OH_Subsystem => _model.OptionalHeader.Subsystem;
#else
        public SabreTools.Models.PortableExecutable.WindowsSubsystem? OH_Subsystem => _model.OptionalHeader?.Subsystem;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.DllCharacteristics"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DllCharacteristics OH_DllCharacteristics => _model.OptionalHeader.DllCharacteristics;
#else
        public SabreTools.Models.PortableExecutable.DllCharacteristics? OH_DllCharacteristics => _model.OptionalHeader?.DllCharacteristics;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfStackReserve_PE32"/>
        public ulong OH_SizeOfStackReserve => _model.OptionalHeader?.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfStackReserve_PE32
            : _model.OptionalHeader?.SizeOfStackReserve_PE32Plus ?? 0;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfStackCommit_PE32"/>
        public ulong OH_SizeOfStackCommit => _model.OptionalHeader?.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfStackCommit_PE32
            : _model.OptionalHeader?.SizeOfStackCommit_PE32Plus ?? 0;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapReserve_PE32"/>
        public ulong OH_SizeOfHeapReserve => _model.OptionalHeader?.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfHeapReserve_PE32
            : _model.OptionalHeader?.SizeOfHeapReserve_PE32Plus ?? 0;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapCommit_PE32"/>
        public ulong OH_SizeOfHeapCommit => _model.OptionalHeader?.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfHeapCommit_PE32
            : _model.OptionalHeader?.SizeOfHeapCommit_PE32Plus ?? 0;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.LoaderFlags"/>
#if NET48
        public uint OH_LoaderFlags => _model.OptionalHeader.LoaderFlags;
#else
        public uint? OH_LoaderFlags => _model.OptionalHeader?.LoaderFlags;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.NumberOfRvaAndSizes"/>
#if NET48
        public uint OH_NumberOfRvaAndSizes => _model.OptionalHeader.NumberOfRvaAndSizes;
#else
        public uint? OH_NumberOfRvaAndSizes => _model.OptionalHeader?.NumberOfRvaAndSizes;
#endif

        #endregion

        #region Data Directories

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ExportTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ExportTable => _model.OptionalHeader.ExportTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_ExportTable => _model.OptionalHeader?.ExportTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImportTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ImportTable => _model.OptionalHeader.ImportTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_ImportTable => _model.OptionalHeader?.ImportTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ResourceTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ResourceTable => _model.OptionalHeader.ResourceTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_ResourceTable => _model.OptionalHeader?.ResourceTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ExceptionTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ExceptionTable => _model.OptionalHeader.ExceptionTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_ExceptionTable => _model.OptionalHeader?.ExceptionTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CertificateTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_CertificateTable => _model.OptionalHeader.CertificateTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_CertificateTable => _model.OptionalHeader?.CertificateTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseRelocationTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_BaseRelocationTable => _model.OptionalHeader.BaseRelocationTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_BaseRelocationTable => _model.OptionalHeader?.BaseRelocationTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Debug"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_Debug => _model.OptionalHeader.Debug;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_Debug => _model.OptionalHeader?.Debug;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Architecture"/>
#if NET48
        public ulong OH_Architecture => _model.OptionalHeader.Architecture;
#else
        public ulong? OH_Architecture => _model.OptionalHeader?.Architecture;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.GlobalPtr"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_GlobalPtr => _model.OptionalHeader.GlobalPtr;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_GlobalPtr => _model.OptionalHeader?.GlobalPtr;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ThreadLocalStorageTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ThreadLocalStorageTable => _model.OptionalHeader.ThreadLocalStorageTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_ThreadLocalStorageTable => _model.OptionalHeader?.ThreadLocalStorageTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.LoadConfigTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_LoadConfigTable => _model.OptionalHeader.LoadConfigTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_LoadConfigTable => _model.OptionalHeader?.LoadConfigTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BoundImport"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_BoundImport => _model.OptionalHeader.BoundImport;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_BoundImport => _model.OptionalHeader?.BoundImport;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImportAddressTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ImportAddressTable => _model.OptionalHeader.ImportAddressTable;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_ImportAddressTable => _model.OptionalHeader?.ImportAddressTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.DelayImportDescriptor"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_DelayImportDescriptor => _model.OptionalHeader.DelayImportDescriptor;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_DelayImportDescriptor => _model.OptionalHeader?.DelayImportDescriptor;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CLRRuntimeHeader"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DataDirectory OH_CLRRuntimeHeader => _model.OptionalHeader.CLRRuntimeHeader;
#else
        public SabreTools.Models.PortableExecutable.DataDirectory? OH_CLRRuntimeHeader => _model.OptionalHeader?.CLRRuntimeHeader;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Reserved"/>
#if NET48
        public ulong OH_Reserved => _model.OptionalHeader.Reserved;
#else
        public ulong? OH_Reserved => _model.OptionalHeader?.Reserved;
#endif

        #endregion

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.PortableExecutable.SectionTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.SectionHeader[] SectionTable => _model.SectionTable;
#else
        public SabreTools.Models.PortableExecutable.SectionHeader?[]? SectionTable => _model.SectionTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFSymbolTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.COFFSymbolTableEntry[] COFFSymbolTable => _model.COFFSymbolTable;
#else
        public SabreTools.Models.PortableExecutable.COFFSymbolTableEntry?[]? COFFSymbolTable => _model.COFFSymbolTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.COFFStringTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.COFFStringTable COFFStringTable => _model.COFFStringTable;
#else
        public SabreTools.Models.PortableExecutable.COFFStringTable? COFFStringTable => _model.COFFStringTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.AttributeCertificateTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.AttributeCertificateTableEntry[] AttributeCertificateTable => _model.AttributeCertificateTable;
#else
        public SabreTools.Models.PortableExecutable.AttributeCertificateTableEntry?[]? AttributeCertificateTable => _model.AttributeCertificateTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.DelayLoadDirectoryTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DelayLoadDirectoryTable DelayLoadDirectoryTable => _model.DelayLoadDirectoryTable;
#else
        public SabreTools.Models.PortableExecutable.DelayLoadDirectoryTable? DelayLoadDirectoryTable => _model.DelayLoadDirectoryTable;
#endif

        #endregion

        #region Sections

        /// <inheritdoc cref="Models.PortableExecutable.BaseRelocationTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.BaseRelocationBlock[] BaseRelocationTable => _model.BaseRelocationTable;
#else
        public SabreTools.Models.PortableExecutable.BaseRelocationBlock?[]? BaseRelocationTable => _model.BaseRelocationTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.DebugTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.DebugTable DebugTable => _model.DebugTable;
#else
        public SabreTools.Models.PortableExecutable.DebugTable? DebugTable => _model.DebugTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.ExportTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.ExportTable ExportTable => _model.ExportTable;
#else
        public SabreTools.Models.PortableExecutable.ExportTable? ExportTable => _model.ExportTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.ExportTable.ExportNameTable"/>
#if NET48
        public string[] ExportNameTable => _model.ExportTable?.ExportNameTable?.Strings;
#else
        public string[]? ExportNameTable => _model.ExportTable?.ExportNameTable?.Strings;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.ImportTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.ImportTable ImportTable => _model.ImportTable;
#else
        public SabreTools.Models.PortableExecutable.ImportTable? ImportTable => _model.ImportTable;
#endif

        /// <inheritdoc cref="Models.PortableExecutable.ImportTable.HintNameTable"/>
#if NET48
        public string[] ImportHintNameTable => _model.ImportTable?.HintNameTable != null
#else
        public string?[]? ImportHintNameTable => _model.ImportTable?.HintNameTable != null
#endif
            ? _model.ImportTable.HintNameTable.Select(entry => entry?.Name).ToArray()
            : null;

        /// <inheritdoc cref="Models.PortableExecutable.ResourceDirectoryTable"/>
#if NET48
        public SabreTools.Models.PortableExecutable.ResourceDirectoryTable ResourceDirectoryTable => _model.ResourceDirectoryTable;
#else
        public SabreTools.Models.PortableExecutable.ResourceDirectoryTable? ResourceDirectoryTable => _model.ResourceDirectoryTable;
#endif

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Header padding data, if it exists
        /// </summary>
#if NET48
        public byte[] HeaderPaddingData
#else
        public byte[]? HeaderPaddingData
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_headerPaddingData != null)
                        return _headerPaddingData;

                    // TODO: Don't scan the known header data as well

                    // If the section table is missing
                    if (SectionTable == null)
                        return null;

                    // Populate the raw header padding data based on the source
#if NET48
                    uint headerStartAddress = Stub_NewExeHeaderAddr;
#else
                    uint headerStartAddress = Stub_NewExeHeaderAddr ?? 0;
#endif
                    uint firstSectionAddress = SectionTable
                        .Select(s => s?.PointerToRawData ?? 0)
                        .Where(s => s != 0)
                        .OrderBy(s => s)
                        .First();
                    int headerLength = (int)(firstSectionAddress - headerStartAddress);
                    _headerPaddingData = ReadFromDataSource((int)headerStartAddress, headerLength);

                    // Cache and return the header padding data, even if null
                    return _headerPaddingData;
                }
            }
        }

        /// <summary>
        /// Header padding strings, if they exist
        /// </summary>
#if NET48
        public List<string> HeaderPaddingStrings
#else
        public List<string>? HeaderPaddingStrings
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_headerPaddingStrings != null)
                        return _headerPaddingStrings;

                    // TODO: Don't scan the known header data as well

                    // If the section table is missing
                    if (SectionTable == null)
                        return null;

                    // Populate the raw header padding data based on the source
#if NET48
                    uint headerStartAddress = Stub_NewExeHeaderAddr;
#else
                    uint headerStartAddress = Stub_NewExeHeaderAddr ?? 0;
#endif
                    uint firstSectionAddress = SectionTable
                        .Select(s => s?.PointerToRawData ?? 0)
                        .Where(s => s != 0)
                        .OrderBy(s => s)
                        .First();
                    int headerLength = (int)(firstSectionAddress - headerStartAddress);
                    _headerPaddingStrings = ReadStringsFromDataSource((int)headerStartAddress, headerLength, charLimit: 3);

                    // Cache and return the header padding data, even if null
                    return _headerPaddingStrings;
                }
            }
        }

        /// <summary>
        /// Entry point data, if it exists
        /// </summary>
#if NET48
        public byte[] EntryPointData
#else
        public byte[]? EntryPointData
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If the section table is missing
                    if (SectionTable == null)
                        return null;

#if NET6_0_OR_GREATER
                    // If the address is missing
                    if (OH_AddressOfEntryPoint == null)
                        return null;
#endif

                    // If we have no entry point
#if NET48
                    int entryPointAddress = (int)OH_AddressOfEntryPoint.ConvertVirtualAddress(SectionTable);
#else
                    int entryPointAddress = (int)OH_AddressOfEntryPoint.Value.ConvertVirtualAddress(SectionTable);
#endif
                    if (entryPointAddress == 0)
                        return null;

                    // If the entry point matches with the start of a section, use that
                    int entryPointSection = FindEntryPointSectionIndex();
                    if (entryPointSection >= 0 && OH_AddressOfEntryPoint == SectionTable[entryPointSection]?.VirtualAddress)
                        return GetSectionData(entryPointSection);

                    // If we already have cached data, just use that immediately
                    if (_entryPointData != null)
                        return _entryPointData;

                    // Read the first 128 bytes of the entry point
                    _entryPointData = ReadFromDataSource(entryPointAddress, length: 128);

                    // Cache and return the entry point padding data, even if null
                    return _entryPointData;
                }
            }
        }

        /// <summary>
        /// Address of the overlay, if it exists
        /// </summary>
        /// <see href="https://www.autoitscript.com/forum/topic/153277-pe-file-overlay-extraction/"/>
        public int OverlayAddress
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // Use the cached data if possible
                    if (_overlayAddress != null)
                        return _overlayAddress.Value;

                    // Get the end of the file, if possible
                    int endOfFile = GetEndOfFile();
                    if (endOfFile == -1)
                        return -1;

                    // If the section table is missing
                    if (SectionTable == null)
                        return -1;

                    // If we have certificate data, use that as the end
                    if (OH_CertificateTable != null)
                    {
                        int certificateTableAddress = (int)OH_CertificateTable.VirtualAddress.ConvertVirtualAddress(SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in SectionTable)
                    {
                        // If we have an invalid section
                        if (section == null)
                            continue;

                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(SectionTable);
                        if (sectionAddress == 0)
                            continue;

                        // If we have an invalid section size
                        if (section.SizeOfRawData == 0 && section.VirtualSize == 0)
                            continue;

                        // Get the real section size
                        int sectionSize;
                        if (section.SizeOfRawData < section.VirtualSize)
                            sectionSize = (int)section.VirtualSize;
                        else
                            sectionSize = (int)section.SizeOfRawData;

                        // Compare and set the end of section data
                        if (sectionAddress + sectionSize > endOfSectionData)
                            endOfSectionData = sectionAddress + sectionSize;
                    }

                    // If we didn't find the end of section data
                    if (endOfSectionData <= 0)
                        endOfSectionData = -1;

                    // Cache and return the position
                    _overlayAddress = endOfSectionData;
                    return _overlayAddress.Value;
                }
            }
        }

        /// <summary>
        /// Overlay data, if it exists
        /// </summary>
        /// <see href="https://www.autoitscript.com/forum/topic/153277-pe-file-overlay-extraction/"/>
#if NET48
        public byte[] OverlayData
#else
        public byte[]? OverlayData
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // Use the cached data if possible
                    if (_overlayData != null)
                        return _overlayData;

                    // Get the end of the file, if possible
                    int endOfFile = GetEndOfFile();
                    if (endOfFile == -1)
                        return null;

                    // If the section table is missing
                    if (SectionTable == null)
                        return null;

                    // If we have certificate data, use that as the end
                    if (OH_CertificateTable != null)
                    {
                        int certificateTableAddress = (int)OH_CertificateTable.VirtualAddress.ConvertVirtualAddress(SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in SectionTable)
                    {
                        // If we have an invalid section
                        if (section == null)
                            continue;

                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(SectionTable);
                        if (sectionAddress == 0)
                            continue;

                        // If we have an invalid section size
                        if (section.SizeOfRawData == 0 && section.VirtualSize == 0)
                            continue;

                        // Get the real section size
                        int sectionSize;
                        if (section.SizeOfRawData < section.VirtualSize)
                            sectionSize = (int)section.VirtualSize;
                        else
                            sectionSize = (int)section.SizeOfRawData;

                        // Compare and set the end of section data
                        if (sectionAddress + sectionSize > endOfSectionData)
                            endOfSectionData = sectionAddress + sectionSize;
                    }

                    // If we didn't find the end of section data
                    if (endOfSectionData <= 0)
                        return null;

                    // If we're at the end of the file, cache an empty byte array
                    if (endOfSectionData >= endOfFile)
                    {
                        _overlayData = new byte[0];
                        return _overlayData;
                    }

                    // Otherwise, cache and return the data
                    int overlayLength = endOfFile - endOfSectionData;
                    _overlayData = ReadFromDataSource(endOfSectionData, overlayLength);
                    return _overlayData;
                }
            }
        }

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
#if NET48
        public List<string> OverlayStrings
#else
        public List<string>? OverlayStrings
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // Use the cached data if possible
                    if (_overlayStrings != null)
                        return _overlayStrings;

                    // Get the end of the file, if possible
                    int endOfFile = GetEndOfFile();
                    if (endOfFile == -1)
                        return null;

                    // If the section table is missing
                    if (SectionTable == null)
                        return null;

                    // If we have certificate data, use that as the end
                    if (OH_CertificateTable != null)
                    {
                        var certificateTable = OH_CertificateTable;
                        int certificateTableAddress = (int)certificateTable.VirtualAddress.ConvertVirtualAddress(SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in SectionTable)
                    {
                        // If we have an invalid section
                        if (section == null)
                            continue;

                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(SectionTable);
                        if (sectionAddress == 0)
                            continue;

                        // If we have an invalid section size
                        if (section.SizeOfRawData == 0 && section.VirtualSize == 0)
                            continue;

                        // Get the real section size
                        int sectionSize;
                        if (section.SizeOfRawData < section.VirtualSize)
                            sectionSize = (int)section.VirtualSize;
                        else
                            sectionSize = (int)section.SizeOfRawData;

                        // Compare and set the end of section data
                        if (sectionAddress + sectionSize > endOfSectionData)
                            endOfSectionData = sectionAddress + sectionSize;
                    }

                    // If we didn't find the end of section data
                    if (endOfSectionData <= 0)
                        return null;

                    // If we're at the end of the file, cache an empty list
                    if (endOfSectionData >= endOfFile)
                    {
                        _overlayStrings = new List<string>();
                        return _overlayStrings;
                    }

                    // Otherwise, cache and return the strings
                    int overlayLength = endOfFile - endOfSectionData;
                    _overlayStrings = ReadStringsFromDataSource(endOfSectionData, overlayLength, charLimit: 3);
                    return _overlayStrings;
                }
            }
        }

        /// <summary>
        /// Sanitized section names
        /// </summary>
#if NET48
        public string[] SectionNames
#else
        public string[]? SectionNames
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // Use the cached data if possible
                    if (_sectionNames != null)
                        return _sectionNames;

                    // If there are no sections
                    if (SectionTable == null)
                        return null;

                    // Otherwise, build and return the cached array
                    _sectionNames = new string[SectionTable.Length];
                    for (int i = 0; i < _sectionNames.Length; i++)
                    {
                        var section = SectionTable[i];
                        if (section == null)
                            continue;

                        // TODO: Handle long section names with leading `/`
#if NET48
                        byte[] sectionNameBytes = section.Name;
#else
                        byte[]? sectionNameBytes = section.Name;
#endif
                        if (sectionNameBytes != null)
                        {
                            string sectionNameString = Encoding.UTF8.GetString(sectionNameBytes).TrimEnd('\0');
                            _sectionNames[i] = sectionNameString;
                        }
                    }

                    return _sectionNames;
                }
            }
        }

        /// <summary>
        /// Stub executable data, if it exists
        /// </summary>
#if NET48
        public byte[] StubExecutableData
#else
        public byte[]? StubExecutableData
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_stubExecutableData != null)
                        return _stubExecutableData;

#if NET6_0_OR_GREATER
                    if (Stub_NewExeHeaderAddr == null)
                        return null;
#endif

                    // Populate the raw stub executable data based on the source
                    int endOfStubHeader = 0x40;
                    int lengthOfStubExecutableData = (int)Stub_NewExeHeaderAddr - endOfStubHeader;
                    _stubExecutableData = ReadFromDataSource(endOfStubHeader, lengthOfStubExecutableData);

                    // Cache and return the stub executable data, even if null
                    return _stubExecutableData;
                }
            }
        }

        /// <summary>
        /// Dictionary of debug data
        /// </summary>
#if NET48
        public Dictionary<int, object> DebugData
#else
        public Dictionary<int, object>? DebugData
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // Use the cached data if possible
                    if (_debugData != null && _debugData.Count != 0)
                        return _debugData;

                    // If we have no resource table, just return
                    if (DebugTable?.DebugDirectoryTable == null
                        || DebugTable.DebugDirectoryTable.Length == 0)
                        return null;

                    // Otherwise, build and return the cached dictionary
                    ParseDebugTable();
                    return _debugData;
                }
            }
        }

        /// <summary>
        /// Dictionary of resource data
        /// </summary>
#if NET48
        public Dictionary<string, object> ResourceData
#else
        public Dictionary<string, object?>? ResourceData
#endif
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // Use the cached data if possible
                    if (_resourceData != null && _resourceData.Count != 0)
                        return _resourceData;

                    // If we have no resource table, just return
                    if (OH_ResourceTable == null
                        || OH_ResourceTable.VirtualAddress == 0
                        || ResourceDirectoryTable == null)
                        return null;

                    // Otherwise, build and return the cached dictionary
                    ParseResourceDirectoryTable(ResourceDirectoryTable, types: new List<object>());
                    return _resourceData;
                }
            }
        }

        #region Version Information

        /// <summary>
        /// "Build GUID"
        /// </summary/>
#if NET48
        public string BuildGuid => GetVersionInfoString("BuildGuid");
#else
        public string? BuildGuid => GetVersionInfoString("BuildGuid");
#endif

        /// <summary>
        /// "Build signature"
        /// </summary/>
#if NET48
        public string BuildSignature => GetVersionInfoString("BuildSignature");
#else
        public string? BuildSignature => GetVersionInfoString("BuildSignature");
#endif

        /// <summary>
        /// Additional information that should be displayed for diagnostic purposes.
        /// </summary/>
#if NET48
        public string Comments => GetVersionInfoString("Comments");
#else
        public string? Comments => GetVersionInfoString("Comments");
#endif

        /// <summary>
        /// Company that produced the file—for example, "Microsoft Corporation" or
        /// "Standard Microsystems Corporation, Inc." This string is required.
        /// </summary/>
#if NET48
        public string CompanyName => GetVersionInfoString("CompanyName");
#else
        public string? CompanyName => GetVersionInfoString("CompanyName");
#endif

        /// <summary>
        /// "Debug version"
        /// </summary/>
#if NET48
        public string DebugVersion => GetVersionInfoString("DebugVersion");
#else
        public string? DebugVersion => GetVersionInfoString("DebugVersion");
#endif

        /// <summary>
        /// File description to be presented to users. This string may be displayed in a
        /// list box when the user is choosing files to install—for example, "Keyboard
        /// Driver for AT-Style Keyboards". This string is required.
        /// </summary/>
#if NET48
        public string FileDescription => GetVersionInfoString("FileDescription");
#else
        public string? FileDescription => GetVersionInfoString("FileDescription");
#endif

        /// <summary>
        /// Version number of the file—for example, "3.10" or "5.00.RC2". This string
        /// is required.
        /// </summary/>
#if NET48
        public string FileVersion => GetVersionInfoString("FileVersion");
#else
        public string? FileVersion => GetVersionInfoString("FileVersion");
#endif

        /// <summary>
        /// Internal name of the file, if one exists—for example, a module name if the
        /// file is a dynamic-link library. If the file has no internal name, this
        /// string should be the original filename, without extension. This string is required.
        /// </summary/>
#if NET48
        public string InternalName => GetVersionInfoString(key: "InternalName");
#else
        public string? InternalName => GetVersionInfoString(key: "InternalName");
#endif

        /// <summary>
        /// Copyright notices that apply to the file. This should include the full text of
        /// all notices, legal symbols, copyright dates, and so on. This string is optional.
        /// </summary/>
#if NET48
        public string LegalCopyright => GetVersionInfoString(key: "LegalCopyright");
#else
        public string? LegalCopyright => GetVersionInfoString(key: "LegalCopyright");
#endif

        /// <summary>
        /// Trademarks and registered trademarks that apply to the file. This should include
        /// the full text of all notices, legal symbols, trademark numbers, and so on. This
        /// string is optional.
        /// </summary/>
#if NET48
        public string LegalTrademarks => GetVersionInfoString(key: "LegalTrademarks");
#else
        public string? LegalTrademarks => GetVersionInfoString(key: "LegalTrademarks");
#endif

        /// <summary>
        /// Original name of the file, not including a path. This information enables an
        /// application to determine whether a file has been renamed by a user. The format of
        /// the name depends on the file system for which the file was created. This string
        /// is required.
        /// </summary/>
#if NET48
        public string OriginalFilename => GetVersionInfoString(key: "OriginalFilename");
#else
        public string? OriginalFilename => GetVersionInfoString(key: "OriginalFilename");
#endif

        /// <summary>
        /// Information about a private version of the file—for example, "Built by TESTER1 on
        /// \TESTBED". This string should be present only if VS_FF_PRIVATEBUILD is specified in
        /// the fileflags parameter of the root block.
        /// </summary/>
#if NET48
        public string PrivateBuild => GetVersionInfoString(key: "PrivateBuild");
#else
        public string? PrivateBuild => GetVersionInfoString(key: "PrivateBuild");
#endif

        /// <summary>
        /// "Product GUID"
        /// </summary/>
#if NET48
        public string ProductGuid => GetVersionInfoString("ProductGuid");
#else
        public string? ProductGuid => GetVersionInfoString("ProductGuid");
#endif

        /// <summary>
        /// Name of the product with which the file is distributed. This string is required.
        /// </summary/>
#if NET48
        public string ProductName => GetVersionInfoString(key: "ProductName");
#else
        public string? ProductName => GetVersionInfoString(key: "ProductName");
#endif

        /// <summary>
        /// Version of the product with which the file is distributed—for example, "3.10" or
        /// "5.00.RC2". This string is required.
        /// </summary/>
#if NET48
        public string ProductVersion => GetVersionInfoString(key: "ProductVersion");
#else
        public string? ProductVersion => GetVersionInfoString(key: "ProductVersion");
#endif

        /// <summary>
        /// Text that specifies how this version of the file differs from the standard
        /// version—for example, "Private build for TESTER1 solving mouse problems on M250 and
        /// M250E computers". This string should be present only if VS_FF_SPECIALBUILD is
        /// specified in the fileflags parameter of the root block.
        /// </summary/>
#if NET48
        public string SpecialBuild => GetVersionInfoString(key: "SpecialBuild") ?? GetVersionInfoString(key: "Special Build");
#else
        public string? SpecialBuild => GetVersionInfoString(key: "SpecialBuild") ?? GetVersionInfoString(key: "Special Build");
#endif

        /// <summary>
        /// "Trade name"
        /// </summary/>
#if NET48
        public string TradeName => GetVersionInfoString(key: "TradeName");
#else
        public string? TradeName => GetVersionInfoString(key: "TradeName");
#endif

        /// <summary>
        /// Get the internal version as reported by the resources
        /// </summary>
        /// <returns>Version string, null on error</returns>
        /// <remarks>The internal version is either the file version, product version, or assembly version, in that order</remarks>
#if NET48
        public string GetInternalVersion()
#else
        public string? GetInternalVersion()
#endif
        {
#if NET48
            string version = this.FileVersion;
#else
            string? version = this.FileVersion;
#endif
            if (!string.IsNullOrWhiteSpace(version))
                return version.Replace(", ", ".");

            version = this.ProductVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version.Replace(", ", ".");

            version = this.AssemblyVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version;

            return null;
        }

        #endregion

        #region Manifest Information

        /// <summary>
        /// Description as derived from the assembly manifest
        /// </summary>
#if NET48
        public string AssemblyDescription
#else
        public string? AssemblyDescription
#endif
        {
            get
            {
                var manifest = GetAssemblyManifest();
                return manifest?
                    .Description?
                    .Value;
            }
        }

        /// <summary>
        /// Version as derived from the assembly manifest
        /// </summary>
        /// <remarks>
        /// If there are multiple identities included in the manifest,
        /// this will only retrieve the value from the first that doesn't
        /// have a null or empty version.
        /// </remarks>
#if NET48
        public string AssemblyVersion
#else
        public string? AssemblyVersion
#endif
        {
            get
            {
                var manifest = GetAssemblyManifest();
                return manifest?
                    .AssemblyIdentities?
                    .FirstOrDefault(ai => !string.IsNullOrWhiteSpace(ai?.Version))?
                    .Version;
            }
        }

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Header padding data, if it exists
        /// </summary>
#if NET48
        private byte[] _headerPaddingData = null;
#else
        private byte[]? _headerPaddingData = null;
#endif

        /// <summary>
        /// Header padding strings, if they exist
        /// </summary>
#if NET48
        private List<string> _headerPaddingStrings = null;
#else
        private List<string>? _headerPaddingStrings = null;
#endif

        /// <summary>
        /// Entry point data, if it exists and isn't aligned to a section
        /// </summary>
#if NET48
        private byte[] _entryPointData = null;
#else
        private byte[]? _entryPointData = null;
#endif

        /// <summary>
        /// Address of the overlay, if it exists
        /// </summary>
        private int? _overlayAddress = null;

        /// <summary>
        /// Overlay data, if it exists
        /// </summary>
#if NET48
        private byte[] _overlayData = null;
#else
        private byte[]? _overlayData = null;
#endif

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
#if NET48
        private List<string> _overlayStrings = null;
#else
        private List<string>? _overlayStrings = null;
#endif

        /// <summary>
        /// Stub executable data, if it exists
        /// </summary>
#if NET48
        private byte[] _stubExecutableData = null;
#else
        private byte[]? _stubExecutableData = null;
#endif

        /// <summary>
        /// Sanitized section names
        /// </summary>
#if NET48
        private string[] _sectionNames = null;
#else
        private string[]? _sectionNames = null;
#endif

        /// <summary>
        /// Cached raw section data
        /// </summary>
#if NET48
        private byte[][] _sectionData = null;
#else
        private byte[]?[]? _sectionData = null;
#endif

        /// <summary>
        /// Cached found string data in sections
        /// </summary>
#if NET48
        private List<string>[] _sectionStringData = null;
#else
        private List<string>?[]? _sectionStringData = null;
#endif

        /// <summary>
        /// Cached raw table data
        /// </summary>
#if NET48
        private byte[][] _tableData = null;
#else
        private byte[]?[]? _tableData = null;
#endif

        /// <summary>
        /// Cached found string data in tables
        /// </summary>
#if NET48
        private List<string>[] _tableStringData = null;
#else
        private List<string>?[]? _tableStringData = null;
#endif

        /// <summary>
        /// Cached debug data
        /// </summary>
        private readonly Dictionary<int, object> _debugData = new Dictionary<int, object>();

        /// <summary>
        /// Cached resource data
        /// </summary>
#if NET48
        private readonly Dictionary<string, object> _resourceData = new Dictionary<string, object>();
#else
        private readonly Dictionary<string, object?> _resourceData = new Dictionary<string, object?>();
#endif

        /// <summary>
        /// Cached version info data
        /// </summary>
#if NET48
        private SabreTools.Models.PortableExecutable.VersionInfo _versionInfo = null;
#else
        private SabreTools.Models.PortableExecutable.VersionInfo? _versionInfo = null;
#endif

        /// <summary>
        /// Cached assembly manifest data
        /// </summary>
#if NET48
        private SabreTools.Models.PortableExecutable.AssemblyManifest _assemblyManifest = null;
#else
        private SabreTools.Models.PortableExecutable.AssemblyManifest? _assemblyManifest = null;
#endif

        /// <summary>
        /// Lock object for reading from the source
        /// </summary>
        private readonly object _sourceDataLock = new object();

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public PortableExecutable(SabreTools.Models.PortableExecutable.Executable model, byte[] data, int offset)
#else
        public PortableExecutable(SabreTools.Models.PortableExecutable.Executable? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public PortableExecutable(SabreTools.Models.PortableExecutable.Executable model, Stream data)
#else
        public PortableExecutable(SabreTools.Models.PortableExecutable.Executable? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
#if NET48
        public static PortableExecutable Create(byte[] data, int offset)
#else
        public static PortableExecutable? Create(byte[]? data, int offset)
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
        /// Create a PE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
#if NET48
        public static PortableExecutable Create(Stream data)
#else
        public static PortableExecutable? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var executable = new SabreTools.Serialization.Streams.PortableExecutable().Deserialize(data);
            if (executable == null)
                return null;

            try
            {
                return new PortableExecutable(executable, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Data

        // TODO: Cache all certificate objects

        /// <summary>
        /// Get the version info string associated with a key, if possible
        /// </summary>
        /// <param name="key">Case-insensitive key to find in the version info</param>
        /// <returns>String representing the data, null on error</returns>
        /// <remarks>
        /// This code does not take into account the locale and will find and return
        /// the first available value. This may not actually matter for version info,
        /// but it is worth mentioning.
        /// </remarks>
#if NET48
        public string GetVersionInfoString(string key)
#else
        public string? GetVersionInfoString(string key)
#endif
        {
            // If we have an invalid key, we can't do anything
            if (string.IsNullOrEmpty(key))
                return null;

            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return null;

            // If we don't have string version info in this executable
            var stringTable = _versionInfo?.StringFileInfo?.Children;
            if (stringTable == null || !stringTable.Any())
                return null;

            // Try to find a key that matches
            var match = stringTable
                .SelectMany(st => st?.Children ?? Array.Empty<SabreTools.Models.PortableExecutable.StringData>())
                .FirstOrDefault(sd => sd != null && key.Equals(sd.Key, StringComparison.OrdinalIgnoreCase));

            // Return either the match or null
            return match?.Value?.TrimEnd('\0');
        }

        /// <summary>
        /// Get the assembly manifest, if possible
        /// </summary>
        /// <returns>Assembly manifest object, null on error</returns>
#if NET48
        private SabreTools.Models.PortableExecutable.AssemblyManifest GetAssemblyManifest()
#else
        private SabreTools.Models.PortableExecutable.AssemblyManifest? GetAssemblyManifest()
#endif
        {
            // Use the cached data if possible
            if (_assemblyManifest != null)
                return _assemblyManifest;

            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return null;

            // Return the now-cached assembly manifest
            return _assemblyManifest;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.PortableExecutable.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion

        #region Debug Data

        /// <summary>
        /// Find CodeView debug data by path
        /// </summary>
        /// <param name="path">Partial path to check for</param>
        /// <returns>Enumerable of matching debug data</returns>
#if NET48
        public IEnumerable<object> FindCodeViewDebugTableByPath(string path)
#else
        public IEnumerable<object?> FindCodeViewDebugTableByPath(string path)
#endif
        {
            // Ensure that we have the debug data cached
            if (DebugData == null)
#if NET48
                return Enumerable.Empty<object>();
#else
                return Enumerable.Empty<object?>();
#endif

            var nb10Found = DebugData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.NB10ProgramDatabase)
                .Where(n => n != null)
                .Where(n => n?.PdbFileName?.Contains(path) == true)
                .Select(n => n as object);

            var rsdsFound = DebugData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.RSDSProgramDatabase)
                .Where(r => r != null)
                .Where(r => r?.PathAndFileName?.Contains(path) == true)
                .Select(r => r as object);

            return nb10Found.Concat(rsdsFound);
        }

        /// <summary>
        /// Find unparsed debug data by string value
        /// </summary>
        /// <param name="value">String value to check for</param>
        /// <returns>Enumerable of matching debug data</returns>
#if NET48
        public IEnumerable<byte[]> FindGenericDebugTableByValue(string value)
#else
        public IEnumerable<byte[]?> FindGenericDebugTableByValue(string value)
#endif
        {
            // Ensure that we have the resource data cached
            if (DebugData == null)
#if NET48
                return Enumerable.Empty<byte[]>();
#else
                return Enumerable.Empty<byte[]>();
#endif

            return DebugData.Select(r => r.Value)
                .Select(b => b as byte[])
                .Where(b => b != null)
                .Where(b =>
                {
                    try
                    {
#if NET48
                        string arrayAsASCII = Encoding.ASCII.GetString(b);
#else
                        string? arrayAsASCII = Encoding.ASCII.GetString(b!);
#endif
                        if (arrayAsASCII.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
#if NET48
                        string arrayAsUTF8 = Encoding.UTF8.GetString(b);
#else
                        string? arrayAsUTF8 = Encoding.UTF8.GetString(b!);
#endif
                        if (arrayAsUTF8.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
#if NET48
                        string arrayAsUnicode = Encoding.Unicode.GetString(b);
#else
                        string? arrayAsUnicode = Encoding.Unicode.GetString(b!);
#endif
                        if (arrayAsUnicode.Contains(value))
                            return true;
                    }
                    catch { }

                    return false;
                });
        }

        #endregion

        #region Debug Parsing

        /// <summary>
        /// Parse the debug directory table information
        /// </summary>
        private void ParseDebugTable()
        {
            // If there is no debug table
            if (DebugTable?.DebugDirectoryTable == null)
                return;

            // Loop through all debug table entries
            for (int i = 0; i < DebugTable.DebugDirectoryTable.Length; i++)
            {
                var entry = DebugTable.DebugDirectoryTable[i];
                if (entry == null)
                    continue;

                uint address = entry.PointerToRawData;
                uint size = entry.SizeOfData;

#if NET48
                byte[] entryData = ReadFromDataSource((int)address, (int)size);
#else
                byte[]? entryData = ReadFromDataSource((int)address, (int)size);
#endif
                if (entryData == null)
                    continue;

                // If we have CodeView debug data, try to parse it
                if (entry.DebugType == SabreTools.Models.PortableExecutable.DebugType.IMAGE_DEBUG_TYPE_CODEVIEW)
                {
                    // Read the signature
                    int offset = 0;
                    uint signature = entryData.ReadUInt32(ref offset);

                    // Reset the offset
                    offset = 0;

                    // NB10
                    if (signature == 0x3031424E)
                    {
                        var nb10ProgramDatabase = entryData.AsNB10ProgramDatabase(ref offset);
                        if (nb10ProgramDatabase != null)
                        {
                            _debugData[i] = nb10ProgramDatabase;
                            continue;
                        }
                    }

                    // RSDS
                    else if (signature == 0x53445352)
                    {
                        var rsdsProgramDatabase = entryData.AsRSDSProgramDatabase(ref offset);
                        if (rsdsProgramDatabase != null)
                        {
                            _debugData[i] = rsdsProgramDatabase;
                            continue;
                        }
                    }
                }
                else
                {
                    _debugData[i] = entryData;
                }
            }
        }

        #endregion

        #region Resource Data

        /// <summary>
        /// Find dialog box resources by title
        /// </summary>
        /// <param name="title">Dialog box title to check for</param>
        /// <returns>Enumerable of matching resources</returns>
#if NET48
        public IEnumerable<SabreTools.Models.PortableExecutable.DialogBoxResource> FindDialogByTitle(string title)
#else
        public IEnumerable<SabreTools.Models.PortableExecutable.DialogBoxResource?> FindDialogByTitle(string title)
#endif
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
#if NET48
                return Enumerable.Empty<SabreTools.Models.PortableExecutable.DialogBoxResource>();
#else
                return Enumerable.Empty<SabreTools.Models.PortableExecutable.DialogBoxResource?>();
#endif

            return ResourceData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.DialogBoxResource)
                .Where(d => d != null)
                .Where(d =>
                {
                    return (d?.DialogTemplate?.TitleResource?.Contains(title) ?? false)
                        || (d?.ExtendedDialogTemplate?.TitleResource?.Contains(title) ?? false);
                });
        }

        /// <summary>
        /// Find dialog box resources by contained item title
        /// </summary>
        /// <param name="title">Dialog box item title to check for</param>
        /// <returns>Enumerable of matching resources</returns>
#if NET48
        public IEnumerable<SabreTools.Models.PortableExecutable.DialogBoxResource> FindDialogBoxByItemTitle(string title)
#else
        public IEnumerable<SabreTools.Models.PortableExecutable.DialogBoxResource?> FindDialogBoxByItemTitle(string title)
#endif
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
#if NET48
                return Enumerable.Empty<SabreTools.Models.PortableExecutable.DialogBoxResource>();
#else
                return Enumerable.Empty<SabreTools.Models.PortableExecutable.DialogBoxResource?>();
#endif

            return ResourceData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.DialogBoxResource)
                .Where(d => d != null)
                .Where(d =>
                {
                    if (d?.DialogItemTemplates != null)
                    {
                        return d.DialogItemTemplates
                            .Where(dit => dit?.TitleResource != null)
                            .Any(dit => dit?.TitleResource?.Contains(title) == true);
                    }
                    else if (d?.ExtendedDialogItemTemplates != null)
                    {
                        return d.ExtendedDialogItemTemplates
                            .Where(edit => edit?.TitleResource != null)
                            .Any(edit => edit?.TitleResource?.Contains(title) == true);
                    }

                    return false;
                });
        }

        /// <summary>
        /// Find string table resources by contained string entry
        /// </summary>
        /// <param name="entry">String entry to check for</param>
        /// <returns>Enumerable of matching resources</returns>
#if NET48
        public IEnumerable<Dictionary<int, string>> FindStringTableByEntry(string entry)
#else
        public IEnumerable<Dictionary<int, string?>?> FindStringTableByEntry(string entry)
#endif
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
#if NET48
                return Enumerable.Empty<Dictionary<int, string>>();
#else
                return Enumerable.Empty<Dictionary<int, string?>?>();
#endif

            return ResourceData.Select(r => r.Value)
#if NET48
                .Select(r => r as Dictionary<int, string>)
#else
                .Select(r => r as Dictionary<int, string?>)
#endif
                .Where(st => st != null)
                .Where(st => st?.Select(kvp => kvp.Value)?
                    .Any(s => s != null && s.Contains(entry)) == true);
        }

        /// <summary>
        /// Find unparsed resources by type name
        /// </summary>
        /// <param name="typeName">Type name to check for</param>
        /// <returns>Enumerable of matching resources</returns>
#if NET48
        public IEnumerable<byte[]> FindResourceByNamedType(string typeName)
#else
        public IEnumerable<byte[]?> FindResourceByNamedType(string typeName)
#endif
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
#if NET48
                return Enumerable.Empty<byte[]>();
#else
                return Enumerable.Empty<byte[]?>();
#endif

            return ResourceData.Where(kvp => kvp.Key.Contains(typeName))
                .Select(kvp => kvp.Value as byte[])
                .Where(b => b != null);
        }

        /// <summary>
        /// Find unparsed resources by string value
        /// </summary>
        /// <param name="value">String value to check for</param>
        /// <returns>Enumerable of matching resources</returns>
#if NET48
        public IEnumerable<byte[]> FindGenericResource(string value)
#else
        public IEnumerable<byte[]?> FindGenericResource(string value)
#endif
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
#if NET48
                return Enumerable.Empty<byte[]>();
#else
                return Enumerable.Empty<byte[]?>();
#endif

            return ResourceData.Select(r => r.Value)
                .Select(r => r as byte[])
                .Where(b => b != null)
                .Where(b =>
                {
                    try
                    {
#if NET48
                        string arrayAsASCII = Encoding.ASCII.GetString(b);
#else
                        string? arrayAsASCII = Encoding.ASCII.GetString(b!);
#endif
                        if (arrayAsASCII.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
#if NET48
                        string arrayAsUTF8 = Encoding.UTF8.GetString(b);
#else
                        string? arrayAsUTF8 = Encoding.UTF8.GetString(b!);
#endif
                        if (arrayAsUTF8.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
#if NET48
                        string arrayAsUnicode = Encoding.Unicode.GetString(b);
#else
                        string? arrayAsUnicode = Encoding.Unicode.GetString(b!);
#endif
                        if (arrayAsUnicode.Contains(value))
                            return true;
                    }
                    catch { }

                    return false;
                });
        }

        #endregion

        #region Resource Parsing

        /// <summary>
        /// Parse the resource directory table information
        /// </summary>
        private void ParseResourceDirectoryTable(SabreTools.Models.PortableExecutable.ResourceDirectoryTable table, List<object> types)
        {
            if (table?.Entries == null)
                return;

            for (int i = 0; i < table.Entries.Length; i++)
            {
                var entry = table.Entries[i];
                if (entry == null)
                    continue;

                var newTypes = new List<object>(types ?? new List<object>());

                if (entry.Name?.UnicodeString != null)
                    newTypes.Add(Encoding.UTF8.GetString(entry.Name.UnicodeString));
                else
                    newTypes.Add(entry.IntegerID);

                ParseResourceDirectoryEntry(entry, newTypes);
            }
        }

        /// <summary>
        /// Parse the name resource directory entry information
        /// </summary>
        private void ParseResourceDirectoryEntry(SabreTools.Models.PortableExecutable.ResourceDirectoryEntry entry, List<object> types)
        {
            if (entry.DataEntry != null)
                ParseResourceDataEntry(entry.DataEntry, types);
            else if (entry.Subdirectory != null)
                ParseResourceDirectoryTable(entry.Subdirectory, types);
        }

        /// <summary>
        /// Parse the resource data entry information
        /// </summary>
        /// <remarks>
        /// When caching the version information and assembly manifest, this code assumes that there is only one of each
        /// of those resources in the entire exectuable. This means that only the last found version or manifest will
        /// ever be cached.
        /// </remarks>
        private void ParseResourceDataEntry(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, List<object> types)
        {
            // Create the key and value objects
            string key = types == null ? $"UNKNOWN_{Guid.NewGuid()}" : string.Join(", ", types);
#if NET48
            object value = entry.Data;
#else
            object? value = entry.Data;
#endif

            // If we have a known resource type
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                try
                {
                    switch ((SabreTools.Models.PortableExecutable.ResourceType)resourceType)
                    {
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_CURSOR:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_BITMAP:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_ICON:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_MENU:
                            value = entry.AsMenu();
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_DIALOG:
                            value = entry.AsDialogBox();
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_STRING:
                            value = entry.AsStringTable();
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_FONTDIR:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_FONT:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_ACCELERATOR:
                            value = entry.AsAcceleratorTableResource();
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_RCDATA:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_MESSAGETABLE:
                            value = entry.AsMessageResourceData();
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_GROUP_CURSOR:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_GROUP_ICON:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_VERSION:
                            _versionInfo = entry.AsVersionInfo();
                            value = _versionInfo;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_DLGINCLUDE:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_PLUGPLAY:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_VXD:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_ANICURSOR:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_ANIICON:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_HTML:
                            value = entry.Data;
                            break;
                        case SabreTools.Models.PortableExecutable.ResourceType.RT_MANIFEST:
                            _assemblyManifest = entry.AsAssemblyManifest();
                            value = _versionInfo;
                            break;
                        default:
                            value = entry.Data;
                            break;
                    }
                }
                catch
                {
                    // Fall back on byte array data for malformed items
                    value = entry.Data;
                }
            }

            // If we have a custom resource type
            else if (types != null && types.Count > 0 && types[0] is string)
            {
                value = entry.Data;
            }

            // Add the key and value to the cache
            _resourceData[key] = value;
        }

        #endregion

        #region Sections

        /// <summary>
        /// Determine if a section is contained within the section table
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>True if the section is in the executable, false otherwise</returns>
#if NET48
        public bool ContainsSection(string sectionName, bool exact = false)
#else
        public bool ContainsSection(string? sectionName, bool exact = false)
#endif
        {
            // If no section name is provided
            if (sectionName == null)
                return false;

            // Get all section names first
            if (SectionNames == null)
                return false;

            // If we're checking exactly, return only exact matches
            if (exact)
                return SectionNames.Any(n => n.Equals(sectionName));

            // Otherwise, check if section name starts with the value
            else
                return SectionNames.Any(n => n.StartsWith(sectionName));
        }

        /// <summary>
        /// Get the section index corresponding to the entry point, if possible
        /// </summary>
        /// <returns>Section index on success, null on error</returns>
        public int FindEntryPointSectionIndex()
        {
            // If the section table is missing
            if (SectionTable == null)
                return -1;

#if NET6_0_OR_GREATER
            // If the address is missing
            if (OH_AddressOfEntryPoint == null)
                return -1;
#endif

            // If we don't have an entry point
#if NET48
            if (OH_AddressOfEntryPoint.ConvertVirtualAddress(SectionTable) == 0)
                return -1;

            // Otherwise, find the section it exists within
            return OH_AddressOfEntryPoint.ContainingSectionIndex(SectionTable);
#else
            if (OH_AddressOfEntryPoint.Value.ConvertVirtualAddress(SectionTable) == 0)
                return -1;

            // Otherwise, find the section it exists within
            return OH_AddressOfEntryPoint.Value.ContainingSectionIndex(SectionTable
                .Where(sh => sh != null)
                .Cast<SabreTools.Models.PortableExecutable.SectionHeader>()
                .ToArray());
#endif
        }

        /// <summary>
        /// Get the first section based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
#if NET48
        public SabreTools.Models.PortableExecutable.SectionHeader GetFirstSection(string name, bool exact = false)
#else
        public SabreTools.Models.PortableExecutable.SectionHeader? GetFirstSection(string? name, bool exact = false)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the first index of the section
            int index = Array.IndexOf(SectionNames, name);
            if (index == -1)
                return null;

            // Return the section
            return SectionTable[index];
        }

        /// <summary>
        /// Get the last section based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
#if NET48
        public SabreTools.Models.PortableExecutable.SectionHeader GetLastSection(string name, bool exact = false)
#else
        public SabreTools.Models.PortableExecutable.SectionHeader? GetLastSection(string? name, bool exact = false)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the last index of the section
            int index = Array.LastIndexOf(SectionNames, name);
            if (index == -1)
                return null;

            // Return the section
            return SectionTable[index];
        }

        /// <summary>
        /// Get the section based on index, if possible
        /// </summary>
        /// <param name="index">Index of the section to check for</param>
        /// <returns>Section data on success, null on error</returns>
#if NET48
        public SabreTools.Models.PortableExecutable.SectionHeader GetSection(int index)
#else
        public SabreTools.Models.PortableExecutable.SectionHeader? GetSection(int index)
#endif
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            // Return the section
            return SectionTable[index];
        }

        /// <summary>
        /// Get the first section data based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
#if NET48
        public byte[] GetFirstSectionData(string name, bool exact = false)
#else
        public byte[]? GetFirstSectionData(string? name, bool exact = false)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the first index of the section
            int index = Array.IndexOf(SectionNames, name);
            return GetSectionData(index);
        }

        /// <summary>
        /// Get the last section data based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
#if NET48
        public byte[] GetLastSectionData(string name, bool exact = false)
#else
        public byte[]? GetLastSectionData(string? name, bool exact = false)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the last index of the section
            int index = Array.LastIndexOf(SectionNames, name);
            return GetSectionData(index);
        }

        /// <summary>
        /// Get the section data based on index, if possible
        /// </summary>
        /// <param name="index">Index of the section to check for</param>
        /// <returns>Section data on success, null on error</returns>
#if NET48
        public byte[] GetSectionData(int index)
#else
        public byte[]? GetSectionData(int index)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            // Get the section data from the table
            var section = SectionTable[index];
            if (section == null)
                return null;

            uint address = section.VirtualAddress.ConvertVirtualAddress(SectionTable);
            if (address == 0)
                return null;

            // Set the section size
            uint size = section.SizeOfRawData;
            lock (_sourceDataLock)
            {
                // Create the section data array if we have to
                if (_sectionData == null)
                    _sectionData = new byte[SectionNames.Length][];

                // If we already have cached data, just use that immediately
                if (_sectionData[index] != null)
                    return _sectionData[index];

                // Populate the raw section data based on the source
#if NET48
                byte[] sectionData = ReadFromDataSource((int)address, (int)size);
#else
                byte[]? sectionData = ReadFromDataSource((int)address, (int)size);
#endif

                // Cache and return the section data, even if null
                _sectionData[index] = sectionData;
                return sectionData;
            }
        }

        /// <summary>
        /// Get the first section strings based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section strings on success, null on error</returns>
#if NET48
        public List<string> GetFirstSectionStrings(string name, bool exact = false)
#else
        public List<string>? GetFirstSectionStrings(string? name, bool exact = false)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the first index of the section
            int index = Array.IndexOf(SectionNames, name);
            return GetSectionStrings(index);
        }

        /// <summary>
        /// Get the last section strings based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section strings on success, null on error</returns>
#if NET48
        public List<string> GetLastSectionStrings(string name, bool exact = false)
#else
        public List<string>? GetLastSectionStrings(string? name, bool exact = false)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the last index of the section
            int index = Array.LastIndexOf(SectionNames, name);
            return GetSectionStrings(index);
        }

        /// <summary>
        /// Get the section strings based on index, if possible
        /// </summary>
        /// <param name="index">Index of the section to check for</param>
        /// <returns>Section strings on success, null on error</returns>
#if NET48
        public List<string> GetSectionStrings(int index)
#else
        public List<string>? GetSectionStrings(int index)
#endif
        {
            // If we have no sections
            if (SectionNames == null || !SectionNames.Any() || SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            // Get the section data from the table
            var section = SectionTable[index];
            if (section == null)
                return null;

            uint address = section.VirtualAddress.ConvertVirtualAddress(SectionTable);
            if (address == 0)
                return null;

            // Set the section size
            uint size = section.SizeOfRawData;
            lock (_sourceDataLock)
            {
                // Create the section string array if we have to
                if (_sectionStringData == null)
                    _sectionStringData = new List<string>[SectionNames.Length];

                // If we already have cached data, just use that immediately
                if (_sectionStringData[index] != null)
                    return _sectionStringData[index];

                // Populate the section string data based on the source
#if NET48
                List<string> sectionStringData = ReadStringsFromDataSource((int)address, (int)size);
#else
                List<string>? sectionStringData = ReadStringsFromDataSource((int)address, (int)size);
#endif

                // Cache and return the section string data, even if null
                _sectionStringData[index] = sectionStringData;
                return sectionStringData;
            }
        }

        #endregion

        #region Tables

        /// <summary>
        /// Get the table data based on index, if possible
        /// </summary>
        /// <param name="index">Index of the table to check for</param>
        /// <returns>Table data on success, null on error</returns>
#if NET48
        public byte[] GetTableData(int index)
#else
        public byte[]? GetTableData(int index)
#endif
        {
            // If the table doesn't exist
            if (index < 0 || index > 16)
                return null;

            // Get the virtual address and size from the entries
            uint virtualAddress = 0, size = 0;
            switch (index)
            {
                case 1:
                    virtualAddress = OH_ExportTable?.VirtualAddress ?? 0;
                    size = OH_ExportTable?.Size ?? 0;
                    break;
                case 2:
                    virtualAddress = OH_ImportTable?.VirtualAddress ?? 0;
                    size = OH_ImportTable?.Size ?? 0;
                    break;
                case 3:
                    virtualAddress = OH_ResourceTable?.VirtualAddress ?? 0;
                    size = OH_ResourceTable?.Size ?? 0;
                    break;
                case 4:
                    virtualAddress = OH_ExceptionTable?.VirtualAddress ?? 0;
                    size = OH_ExceptionTable?.Size ?? 0;
                    break;
                case 5:
                    virtualAddress = OH_CertificateTable?.VirtualAddress ?? 0;
                    size = OH_CertificateTable?.Size ?? 0;
                    break;
                case 6:
                    virtualAddress = OH_BaseRelocationTable?.VirtualAddress ?? 0;
                    size = OH_BaseRelocationTable?.Size ?? 0;
                    break;
                case 7:
                    virtualAddress = OH_Debug?.VirtualAddress ?? 0;
                    size = OH_Debug?.Size ?? 0;
                    break;
                case 8: // Architecture Table
                    virtualAddress = 0;
                    size = 0;
                    break;
                case 9:
                    virtualAddress = OH_GlobalPtr?.VirtualAddress ?? 0;
                    size = OH_GlobalPtr?.Size ?? 0;
                    break;
                case 10:
                    virtualAddress = OH_ThreadLocalStorageTable?.VirtualAddress ?? 0;
                    size = OH_ThreadLocalStorageTable?.Size ?? 0;
                    break;
                case 11:
                    virtualAddress = OH_LoadConfigTable?.VirtualAddress ?? 0;
                    size = OH_LoadConfigTable?.Size ?? 0;
                    break;
                case 12:
                    virtualAddress = OH_BoundImport?.VirtualAddress ?? 0;
                    size = OH_BoundImport?.Size ?? 0;
                    break;
                case 13:
                    virtualAddress = OH_ImportAddressTable?.VirtualAddress ?? 0;
                    size = OH_ImportAddressTable?.Size ?? 0;
                    break;
                case 14:
                    virtualAddress = OH_DelayImportDescriptor?.VirtualAddress ?? 0;
                    size = OH_DelayImportDescriptor?.Size ?? 0;
                    break;
                case 15:
                    virtualAddress = OH_CLRRuntimeHeader?.VirtualAddress ?? 0;
                    size = OH_CLRRuntimeHeader?.Size ?? 0;
                    break;
                case 16: // Reserved
                    virtualAddress = 0;
                    size = 0;
                    break;
            }

            // If there is  no section table
            if (SectionTable == null)
                return null;

            // Get the physical address from the virtual one
            uint address = virtualAddress.ConvertVirtualAddress(SectionTable);
            if (address == 0 || size == 0)
                return null;

            lock (_sourceDataLock)
            {
                // Create the table data array if we have to
                if (_tableData == null)
                    _tableData = new byte[16][];

                // If we already have cached data, just use that immediately
                if (_tableData[index] != null)
                    return _tableData[index];

                // Populate the raw table data based on the source
#if NET48
                byte[] tableData = ReadFromDataSource((int)address, (int)size);
#else
                byte[]? tableData = ReadFromDataSource((int)address, (int)size);
#endif

                // Cache and return the table data, even if null
                _tableData[index] = tableData;
                return tableData;
            }
        }

        /// <summary>
        /// Get the table strings based on index, if possible
        /// </summary>
        /// <param name="index">Index of the table to check for</param>
        /// <returns>Table strings on success, null on error</returns>
#if NET48
        public List<string> GetTableStrings(int index)
#else
        public List<string>? GetTableStrings(int index)
#endif
        {
            // If the table doesn't exist
            if (index < 0 || index > 16)
                return null;

            // Get the virtual address and size from the entries
            uint virtualAddress = 0, size = 0;
            switch (index)
            {
                case 1:
                    virtualAddress = OH_ExportTable?.VirtualAddress ?? 0;
                    size = OH_ExportTable?.Size ?? 0;
                    break;
                case 2:
                    virtualAddress = OH_ImportTable?.VirtualAddress ?? 0;
                    size = OH_ImportTable?.Size ?? 0;
                    break;
                case 3:
                    virtualAddress = OH_ResourceTable?.VirtualAddress ?? 0;
                    size = OH_ResourceTable?.Size ?? 0;
                    break;
                case 4:
                    virtualAddress = OH_ExceptionTable?.VirtualAddress ?? 0;
                    size = OH_ExceptionTable?.Size ?? 0;
                    break;
                case 5:
                    virtualAddress = OH_CertificateTable?.VirtualAddress ?? 0;
                    size = OH_CertificateTable?.Size ?? 0;
                    break;
                case 6:
                    virtualAddress = OH_BaseRelocationTable?.VirtualAddress ?? 0;
                    size = OH_BaseRelocationTable?.Size ?? 0;
                    break;
                case 7:
                    virtualAddress = OH_Debug?.VirtualAddress ?? 0;
                    size = OH_Debug?.Size ?? 0;
                    break;
                case 8: // Architecture Table
                    virtualAddress = 0;
                    size = 0;
                    break;
                case 9:
                    virtualAddress = OH_GlobalPtr?.VirtualAddress ?? 0;
                    size = OH_GlobalPtr?.Size ?? 0;
                    break;
                case 10:
                    virtualAddress = OH_ThreadLocalStorageTable?.VirtualAddress ?? 0;
                    size = OH_ThreadLocalStorageTable?.Size ?? 0;
                    break;
                case 11:
                    virtualAddress = OH_LoadConfigTable?.VirtualAddress ?? 0;
                    size = OH_LoadConfigTable?.Size ?? 0;
                    break;
                case 12:
                    virtualAddress = OH_BoundImport?.VirtualAddress ?? 0;
                    size = OH_BoundImport?.Size ?? 0;
                    break;
                case 13:
                    virtualAddress = OH_ImportAddressTable?.VirtualAddress ?? 0;
                    size = OH_ImportAddressTable?.Size ?? 0;
                    break;
                case 14:
                    virtualAddress = OH_DelayImportDescriptor?.VirtualAddress ?? 0;
                    size = OH_DelayImportDescriptor?.Size ?? 0;
                    break;
                case 15:
                    virtualAddress = OH_CLRRuntimeHeader?.VirtualAddress ?? 0;
                    size = OH_CLRRuntimeHeader?.Size ?? 0;
                    break;
                case 16: // Reserved
                    virtualAddress = 0;
                    size = 0;
                    break;
            }

            // If there is  no section table
            if (SectionTable == null)
                return null;

            // Get the physical address from the virtual one
            uint address = virtualAddress.ConvertVirtualAddress(SectionTable);
            if (address == 0 || size == 0)
                return null;

            lock (_sourceDataLock)
            {
                // Create the table string array if we have to
                if (_tableStringData == null)
                    _tableStringData = new List<string>[16];

                // If we already have cached data, just use that immediately
                if (_tableStringData[index] != null)
                    return _tableStringData[index];

                // Populate the table string data based on the source
#if NET48
                List<string> tableStringData = ReadStringsFromDataSource((int)address, (int)size);
#else
                List<string>? tableStringData = ReadStringsFromDataSource((int)address, (int)size);
#endif

                // Cache and return the table string data, even if null
                _tableStringData[index] = tableStringData;
                return tableStringData;
            }
        }

        #endregion
    }
}