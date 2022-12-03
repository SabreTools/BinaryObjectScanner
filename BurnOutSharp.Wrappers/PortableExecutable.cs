using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class PortableExecutable
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
        public ushort Stub_Stub_InitialSPValue => _executable.Stub.Header.InitialSPValue;

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

        /// <inheritdoc cref="Models.PortableExecutable.Executable.Signature"/>
        public byte[] Signature => _executable.Signature;

        #region COFF File Header

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.Machine"/>
        public Models.PortableExecutable.MachineType Machine => _executable.COFFFileHeader.Machine;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.NumberOfSections"/>
        public ushort NumberOfSections => _executable.COFFFileHeader.NumberOfSections;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.TimeDateStamp"/>
        public uint TimeDateStamp => _executable.COFFFileHeader.TimeDateStamp;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.PointerToSymbolTable"/>
        public uint PointerToSymbolTable => _executable.COFFFileHeader.PointerToSymbolTable;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.NumberOfSymbols"/>
        public uint NumberOfSymbols => _executable.COFFFileHeader.NumberOfSymbols;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.SizeOfOptionalHeader"/>
        public uint SizeOfOptionalHeader => _executable.COFFFileHeader.SizeOfOptionalHeader;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.Characteristics"/>
        public Models.PortableExecutable.Characteristics Characteristics => _executable.COFFFileHeader.Characteristics;

        #endregion

        #region Optional Header

        #region Standard Fields

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Machine"/>
        public Models.PortableExecutable.OptionalHeaderMagicNumber OH_Magic => _executable.OptionalHeader.Magic;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorLinkerVersion"/>
        public byte OH_MajorLinkerVersion => _executable.OptionalHeader.MajorLinkerVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorLinkerVersion"/>
        public byte OH_MinorLinkerVersion => _executable.OptionalHeader.MinorLinkerVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfCode"/>
        public uint OH_SizeOfCode => _executable.OptionalHeader.SizeOfCode;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfInitializedData"/>
        public uint OH_SizeOfInitializedData => _executable.OptionalHeader.SizeOfInitializedData;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfUninitializedData"/>
        public uint OH_SizeOfUninitializedData => _executable.OptionalHeader.SizeOfUninitializedData;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.AddressOfEntryPoint"/>
        public uint OH_AddressOfEntryPoint => _executable.OptionalHeader.AddressOfEntryPoint;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseOfCode"/>
        public uint OH_BaseOfCode => _executable.OptionalHeader.BaseOfCode;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseOfData"/>
        public uint? OH_BaseOfData => _executable.OptionalHeader.Magic ==  Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? (uint?)_executable.OptionalHeader.BaseOfData
            : null;

        #endregion

        #region Windows-Specific Fields

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImageBase_PE32"/>
        public ulong OH_ImageBase => _executable.OptionalHeader.Magic ==  Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.ImageBase_PE32
            : _executable.OptionalHeader.ImageBase_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SectionAlignment"/>
        public uint OH_SectionAlignment => _executable.OptionalHeader.SectionAlignment;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.FileAlignment"/>
        public uint OH_FileAlignment => _executable.OptionalHeader.FileAlignment;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorOperatingSystemVersion"/>
        public ushort OH_MajorOperatingSystemVersion => _executable.OptionalHeader.MajorOperatingSystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorOperatingSystemVersion"/>
        public ushort OH_MinorOperatingSystemVersion => _executable.OptionalHeader.MinorOperatingSystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorImageVersion"/>
        public ushort OH_MajorImageVersion => _executable.OptionalHeader.MajorImageVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorImageVersion"/>
        public ushort OH_MinorImageVersion => _executable.OptionalHeader.MinorImageVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorSubsystemVersion"/>
        public ushort OH_MajorSubsystemVersion => _executable.OptionalHeader.MajorSubsystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorSubsystemVersion"/>
        public ushort OH_MinorSubsystemVersion => _executable.OptionalHeader.MinorSubsystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Win32VersionValue"/>
        public uint OH_Win32VersionValue => _executable.OptionalHeader.Win32VersionValue;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfImage"/>
        public uint OH_SizeOfImage => _executable.OptionalHeader.SizeOfImage;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeaders"/>
        public uint OH_SizeOfHeaders => _executable.OptionalHeader.SizeOfHeaders;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CheckSum"/>
        public uint OH_CheckSum => _executable.OptionalHeader.CheckSum;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Subsystem"/>
        public Models.PortableExecutable.WindowsSubsystem OH_Subsystem => _executable.OptionalHeader.Subsystem;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.DllCharacteristics"/>
        public Models.PortableExecutable.DllCharacteristics OH_DllCharacteristics => _executable.OptionalHeader.DllCharacteristics;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfStackReserve_PE32"/>
        public ulong OH_SizeOfStackReserve => _executable.OptionalHeader.Magic ==  Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.SizeOfStackReserve_PE32
            : _executable.OptionalHeader.SizeOfStackReserve_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfStackCommit_PE32"/>
        public ulong OH_SizeOfStackCommit => _executable.OptionalHeader.Magic ==  Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.SizeOfStackCommit_PE32
            : _executable.OptionalHeader.SizeOfStackCommit_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapReserve_PE32"/>
        public ulong OH_SizeOfHeapReserve => _executable.OptionalHeader.Magic ==  Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.SizeOfHeapReserve_PE32
            : _executable.OptionalHeader.SizeOfHeapReserve_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapCommit_PE32"/>
        public ulong OH_SizeOfHeapCommit => _executable.OptionalHeader.Magic ==  Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.SizeOfHeapCommit_PE32
            : _executable.OptionalHeader.SizeOfHeapCommit_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.LoaderFlags"/>
        public uint OH_LoaderFlags => _executable.OptionalHeader.LoaderFlags;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.NumberOfRvaAndSizes"/>
        public uint OH_NumberOfRvaAndSizes => _executable.OptionalHeader.NumberOfRvaAndSizes;

        #endregion

        #region Data Directories

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ExportTable"/>
        public Models.PortableExecutable.DataDirectory OH_ExportTable => _executable.OptionalHeader.ExportTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImportTable"/>
        public Models.PortableExecutable.DataDirectory OH_ImportTable => _executable.OptionalHeader.ImportTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ResourceTable"/>
        public Models.PortableExecutable.DataDirectory OH_ResourceTable => _executable.OptionalHeader.ResourceTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ExceptionTable"/>
        public Models.PortableExecutable.DataDirectory OH_ExceptionTable => _executable.OptionalHeader.ExceptionTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CertificateTable"/>
        public Models.PortableExecutable.DataDirectory OH_CertificateTable => _executable.OptionalHeader.CertificateTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseRelocationTable"/>
        public Models.PortableExecutable.DataDirectory OH_BaseRelocationTable => _executable.OptionalHeader.BaseRelocationTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Debug"/>
        public Models.PortableExecutable.DataDirectory OH_Debug => _executable.OptionalHeader.Debug;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Architecture"/>
        public ulong OH_Architecture => _executable.OptionalHeader.Architecture;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.GlobalPtr"/>
        public Models.PortableExecutable.DataDirectory OH_GlobalPtr => _executable.OptionalHeader.GlobalPtr;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ThreadLocalStorageTable"/>
        public Models.PortableExecutable.DataDirectory OH_ThreadLocalStorageTable => _executable.OptionalHeader.ThreadLocalStorageTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.LoadConfigTable"/>
        public Models.PortableExecutable.DataDirectory OH_LoadConfigTable => _executable.OptionalHeader.LoadConfigTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BoundImport"/>
        public Models.PortableExecutable.DataDirectory OH_BoundImport => _executable.OptionalHeader.BoundImport;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImportAddressTable"/>
        public Models.PortableExecutable.DataDirectory OH_ImportAddressTable => _executable.OptionalHeader.ImportAddressTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.DelayImportDescriptor"/>
        public Models.PortableExecutable.DataDirectory OH_DelayImportDescriptor => _executable.OptionalHeader.DelayImportDescriptor;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CLRRuntimeHeader"/>
        public Models.PortableExecutable.DataDirectory OH_CLRRuntimeHeader => _executable.OptionalHeader.CLRRuntimeHeader;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Reserved"/>
        public ulong OH_Reserved => _executable.OptionalHeader.Reserved;

        #endregion

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.PortableExecutable.SectionTable"/>
        public Models.PortableExecutable.SectionHeader[] SectionTable => _executable.SectionTable;

        /// <inheritdoc cref="Models.PortableExecutable.COFFSymbolTable"/>
        public Models.PortableExecutable.COFFSymbolTableEntry[] COFFSymbolTable => _executable.COFFSymbolTable;

        /// <inheritdoc cref="Models.PortableExecutable.COFFStringTable"/>
        public Models.PortableExecutable.COFFStringTable COFFStringTable => _executable.COFFStringTable;

        /// <inheritdoc cref="Models.PortableExecutable.AttributeCertificateTable"/>
        public Models.PortableExecutable.AttributeCertificateTableEntry[] AttributeCertificateTable => _executable.AttributeCertificateTable;

        /// <inheritdoc cref="Models.PortableExecutable.DelayLoadDirectoryTable"/>
        public Models.PortableExecutable.DelayLoadDirectoryTable DelayLoadDirectoryTable => _executable.DelayLoadDirectoryTable;

        #endregion

        #region Sections

        /// <inheritdoc cref="Models.PortableExecutable.DebugTable"/>
        public Models.PortableExecutable.DebugTable DebugTable => _executable.DebugTable;

        /// <inheritdoc cref="Models.PortableExecutable.ExportTable"/>
        public Models.PortableExecutable.ExportTable ExportTable => _executable.ExportTable;

        /// <inheritdoc cref="Models.PortableExecutable.ImportTable"/>
        public Models.PortableExecutable.ImportTable ImportTable => _executable.ImportTable;

        /// <inheritdoc cref="Models.PortableExecutable.ResourceDirectoryTable"/>
        public Models.PortableExecutable.ResourceDirectoryTable ResourceDirectoryTable => _executable.ResourceDirectoryTable;

        #endregion

        // TODO: Determine what properties can be passed through

        #endregion

        #region Extension Properties

        // TODO: Determine what extension properties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the executable
        /// </summary>
        private Models.PortableExecutable.Executable _executable;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private PortableExecutable() { }

        /// <summary>
        /// Create a PE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
        public static PortableExecutable Create(byte[] data, int offset)
        {
            var executable = Builder.PortableExecutable.ParseExecutable(data, offset);
            if (executable == null)
                return null;

            var wrapper = new PortableExecutable { _executable = executable };
            return wrapper;
        }

        /// <summary>
        /// Create a PE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
        public static PortableExecutable Create(Stream data)
        {
            var executable = Builder.PortableExecutable.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new PortableExecutable { _executable = executable };
            return wrapper;
        }
    }
}