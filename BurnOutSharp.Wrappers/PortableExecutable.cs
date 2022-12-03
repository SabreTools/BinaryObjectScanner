using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using static BurnOutSharp.Builder.Extensions;

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
    
        /// <summary>
        /// Pretty print the New Executable information
        /// </summary>
        public void Print()
        {
            Console.WriteLine("Portable Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            Console.WriteLine("  MS-DOS Stub Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine();

            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {BitConverter.ToString(_executable.Stub.Header.Magic).Replace("-", string.Empty)}");
            Console.WriteLine($"  Last page bytes: {_executable.Stub.Header.LastPageBytes}");
            Console.WriteLine($"  Pages: {_executable.Stub.Header.Pages}");
            Console.WriteLine($"  Relocation items: {_executable.Stub.Header.RelocationItems}");
            Console.WriteLine($"  Header paragraph size: {_executable.Stub.Header.HeaderParagraphSize}");
            Console.WriteLine($"  Minimum extra paragraphs: {_executable.Stub.Header.MinimumExtraParagraphs}");
            Console.WriteLine($"  Maximum extra paragraphs: {_executable.Stub.Header.MaximumExtraParagraphs}");
            Console.WriteLine($"  Initial SS value: {_executable.Stub.Header.InitialSSValue}");
            Console.WriteLine($"  Initial SP value: {_executable.Stub.Header.InitialSPValue}");
            Console.WriteLine($"  Checksum: {_executable.Stub.Header.Checksum}");
            Console.WriteLine($"  Initial IP value: {_executable.Stub.Header.InitialIPValue}");
            Console.WriteLine($"  Initial CS value: {_executable.Stub.Header.InitialCSValue}");
            Console.WriteLine($"  Relocation table address: {_executable.Stub.Header.RelocationTableAddr}");
            Console.WriteLine($"  Overlay number: {_executable.Stub.Header.OverlayNumber}");
            Console.WriteLine();

            Console.WriteLine("  Extended Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Reserved words: {string.Join(", ", _executable.Stub.Header.Reserved1)}");
            Console.WriteLine($"  OEM identifier: {_executable.Stub.Header.OEMIdentifier}");
            Console.WriteLine($"  OEM information: {_executable.Stub.Header.OEMInformation}");
            Console.WriteLine($"  Reserved words: {string.Join(", ", _executable.Stub.Header.Reserved2)}");
            Console.WriteLine($"  New EXE header address: {_executable.Stub.Header.NewExeHeaderAddr}");
            Console.WriteLine();

            Console.WriteLine("  COFF File Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {BitConverter.ToString(_executable.Signature).Replace("-", string.Empty)}");
            Console.WriteLine($"  Machine: {_executable.COFFFileHeader.Machine}");
            Console.WriteLine($"  Number of sections: {_executable.COFFFileHeader.NumberOfSections}");
            Console.WriteLine($"  Time/Date stamp: {_executable.COFFFileHeader.TimeDateStamp}");
            Console.WriteLine($"  Pointer to symbol table: {_executable.COFFFileHeader.PointerToSymbolTable}");
            Console.WriteLine($"  Number of symbols: {_executable.COFFFileHeader.NumberOfSymbols}");
            Console.WriteLine($"  Size of optional header: {_executable.COFFFileHeader.SizeOfOptionalHeader}");
            Console.WriteLine($"  Characteristics: {_executable.COFFFileHeader.Characteristics}");
            Console.WriteLine();

            Console.WriteLine("  Optional Header Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.COFFFileHeader.SizeOfOptionalHeader == 0 || _executable.OptionalHeader == null)
            {
                Console.WriteLine("  No optional header present");
            }
            else
            {
                Console.WriteLine($"  Magic: {_executable.OptionalHeader.Magic}");
                Console.WriteLine($"  Major linker version: {_executable.OptionalHeader.MajorLinkerVersion}");
                Console.WriteLine($"  Minor linker version: {_executable.OptionalHeader.MinorLinkerVersion}");
                Console.WriteLine($"  Size of code section: {_executable.OptionalHeader.SizeOfCode}");
                Console.WriteLine($"  Size of initialized data: {_executable.OptionalHeader.SizeOfInitializedData}");
                Console.WriteLine($"  Size of uninitialized data: {_executable.OptionalHeader.SizeOfUninitializedData}");
                Console.WriteLine($"  Address of entry point: {_executable.OptionalHeader.AddressOfEntryPoint}");
                Console.WriteLine($"  Base of code: {_executable.OptionalHeader.BaseOfCode}");
                if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Base of data: {_executable.OptionalHeader.BaseOfData}");

                if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Image base: {_executable.OptionalHeader.ImageBase_PE32}");
                else if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Image base: {_executable.OptionalHeader.ImageBase_PE32Plus}");
                Console.WriteLine($"  Section alignment: {_executable.OptionalHeader.SectionAlignment}");
                Console.WriteLine($"  File alignment: {_executable.OptionalHeader.FileAlignment}");
                Console.WriteLine($"  Major operating system version: {_executable.OptionalHeader.MajorOperatingSystemVersion}");
                Console.WriteLine($"  Minor operating system version: {_executable.OptionalHeader.MinorOperatingSystemVersion}");
                Console.WriteLine($"  Major image version: {_executable.OptionalHeader.MajorImageVersion}");
                Console.WriteLine($"  Minor image version: {_executable.OptionalHeader.MinorImageVersion}");
                Console.WriteLine($"  Major subsystem version: {_executable.OptionalHeader.MajorSubsystemVersion}");
                Console.WriteLine($"  Minor subsystem version: {_executable.OptionalHeader.MinorSubsystemVersion}");
                Console.WriteLine($"  Win32 version value: {_executable.OptionalHeader.Win32VersionValue}");
                Console.WriteLine($"  Size of image: {_executable.OptionalHeader.SizeOfImage}");
                Console.WriteLine($"  Size of headers: {_executable.OptionalHeader.SizeOfHeaders}");
                Console.WriteLine($"  Checksum: {_executable.OptionalHeader.CheckSum}");
                Console.WriteLine($"  Subsystem: {_executable.OptionalHeader.Subsystem}");
                Console.WriteLine($"  DLL characteristics: {_executable.OptionalHeader.DllCharacteristics}");
                if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of stack reserve: {_executable.OptionalHeader.SizeOfStackReserve_PE32}");
                else if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of stack reserve: {_executable.OptionalHeader.SizeOfStackReserve_PE32Plus}");
                if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of stack commit: {_executable.OptionalHeader.SizeOfStackCommit_PE32}");
                else if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of stack commit: {_executable.OptionalHeader.SizeOfStackCommit_PE32Plus}");
                if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of heap reserve: {_executable.OptionalHeader.SizeOfHeapReserve_PE32}");
                else if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of heap reserve: {_executable.OptionalHeader.SizeOfHeapReserve_PE32Plus}");
                if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of heap commit: {_executable.OptionalHeader.SizeOfHeapCommit_PE32}");
                else if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of heap commit: {_executable.OptionalHeader.SizeOfHeapCommit_PE32Plus}");
                Console.WriteLine($"  Loader flags: {_executable.OptionalHeader.LoaderFlags}");
                Console.WriteLine($"  Number of data-directory entries: {_executable.OptionalHeader.NumberOfRvaAndSizes}");
            
                if (_executable.OptionalHeader.ExportTable != null)
                {
                    Console.WriteLine("    Export Table (1)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.ExportTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.ExportTable.Size}");
                }
                if (_executable.OptionalHeader.ImportTable != null)
                {
                    Console.WriteLine("    Import Table (2)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.ImportTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.ImportTable.Size}");
                }
                if (_executable.OptionalHeader.ResourceTable != null)
                {
                    Console.WriteLine("    Resource Table (3)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.ResourceTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.ResourceTable.Size}");
                }
                if (_executable.OptionalHeader.ExceptionTable != null)
                {
                    Console.WriteLine("    Exception Table (4)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.ExceptionTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.ExceptionTable.Size}");
                }
                if (_executable.OptionalHeader.CertificateTable != null)
                {
                    Console.WriteLine("    Certificate Table (5)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.CertificateTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.CertificateTable.Size}");
                }
                if (_executable.OptionalHeader.BaseRelocationTable != null)
                {
                    Console.WriteLine("    Base Relocation Table (6)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.BaseRelocationTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.BaseRelocationTable.Size}");
                }
                if (_executable.OptionalHeader.Debug != null)
                {
                    Console.WriteLine("    Debug Table (7)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.Debug.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.Debug.Size}");
                }
                if (_executable.OptionalHeader.NumberOfRvaAndSizes >= 8)
                {
                    Console.WriteLine("    Architecture Table (8)");
                    Console.WriteLine($"      Virtual address: 0");
                    Console.WriteLine($"      Size: 0");
                }
                if (_executable.OptionalHeader.GlobalPtr != null)
                {
                    Console.WriteLine("    Global Pointer Register (9)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.GlobalPtr.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.GlobalPtr.Size}");
                }
                if (_executable.OptionalHeader.ThreadLocalStorageTable != null)
                {
                    Console.WriteLine("    Thread Local Storage (TLS) Table (10)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.ThreadLocalStorageTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.ThreadLocalStorageTable.Size}");
                }
                if (_executable.OptionalHeader.LoadConfigTable != null)
                {
                    Console.WriteLine("    Load Config Table (11)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.LoadConfigTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.LoadConfigTable.Size}");
                }
                if (_executable.OptionalHeader.BoundImport != null)
                {
                    Console.WriteLine("    Bound Import Table (12)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.BoundImport.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.BoundImport.Size}");
                }
                if (_executable.OptionalHeader.ImportAddressTable != null)
                {
                    Console.WriteLine("    Import Address Table (13)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.ImportAddressTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.ImportAddressTable.Size}");
                }
                if (_executable.OptionalHeader.DelayImportDescriptor != null)
                {
                    Console.WriteLine("    Delay Import Descriptior (14)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.DelayImportDescriptor.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.DelayImportDescriptor.Size}");
                }
                if (_executable.OptionalHeader.CLRRuntimeHeader != null)
                {
                    Console.WriteLine("    CLR Runtime Header (15)");
                    Console.WriteLine($"      Virtual address: {_executable.OptionalHeader.CLRRuntimeHeader.VirtualAddress}");
                    Console.WriteLine($"      Size: {_executable.OptionalHeader.CLRRuntimeHeader.Size}");
                }
                if (_executable.OptionalHeader.NumberOfRvaAndSizes >= 16)
                {
                    Console.WriteLine("    Reserved (16)");
                    Console.WriteLine($"      Virtual address: 0");
                    Console.WriteLine($"      Size: 0");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Section Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.COFFFileHeader.NumberOfSections == 0 || _executable.SectionTable.Length == 0)
            {
                Console.WriteLine("  No section table items");
            }
            else
            {
                for (int i = 0; i < _executable.SectionTable.Length; i++)
                {
                    var entry = _executable.SectionTable[i];
                    Console.WriteLine($"  Section Table Entry {i}");
                    Console.WriteLine($"    Name = {Encoding.UTF8.GetString(entry.Name)}");
                    Console.WriteLine($"    Virtual size = {entry.VirtualSize}");
                    Console.WriteLine($"    Virtual address = {entry.VirtualAddress}");
                    Console.WriteLine($"    Size of raw data = {entry.SizeOfRawData}");
                    Console.WriteLine($"    Pointer to raw data = {entry.PointerToRawData}");
                    Console.WriteLine($"    Pointer to relocations = {entry.PointerToRelocations}");
                    Console.WriteLine($"    Pointer to linenumbers = {entry.PointerToLinenumbers}");
                    Console.WriteLine($"    Number of relocations = {entry.NumberOfRelocations}");
                    Console.WriteLine($"    Number of linenumbers = {entry.NumberOfLinenumbers}");
                    Console.WriteLine($"    Characteristics = {entry.Characteristics}");
                    // TODO: Add COFFRelocations
                    // TODO: Add COFFLineNumbers
                }
            }
            Console.WriteLine();

            Console.WriteLine("  COFF Symbol Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.COFFFileHeader.PointerToSymbolTable == 0
                || _executable.COFFFileHeader.NumberOfSymbols == 0
                || _executable.COFFSymbolTable.Length == 0)
            {
                Console.WriteLine("  No COFF symbol table items");
            }
            else
            {
                int auxSymbolsRemaining = 0;
                int currentSymbolType = 0;

                for (int i = 0; i < _executable.COFFSymbolTable.Length; i++)
                {
                    var entry = _executable.COFFSymbolTable[i];
                    Console.WriteLine($"  COFF Symbol Table Entry {i} (Subtype {currentSymbolType})");
                    if (currentSymbolType == 0)
                    {
                        if (entry.ShortName != null)
                        {
                            Console.WriteLine($"    Short name = {Encoding.UTF8.GetString(entry.ShortName)}");
                        }
                        else
                        {
                            Console.WriteLine($"    Zeroes = {entry.Zeroes}");
                            Console.WriteLine($"    Offset = {entry.Offset}");
                        }
                        Console.WriteLine($"    Value = {entry.Value}");
                        Console.WriteLine($"    Section number = {entry.SectionNumber}");
                        Console.WriteLine($"    Symbol type = {entry.SymbolType}");
                        Console.WriteLine($"    Storage class = {entry.StorageClass}");
                        Console.WriteLine($"    Number of aux symbols = {entry.NumberOfAuxSymbols}");

                        auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                        if (auxSymbolsRemaining == 0)
                            continue;

                        if (entry.StorageClass == Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SymbolType == Models.PortableExecutable.SymbolType.IMAGE_SYM_TYPE_FUNC
                        && entry.SectionNumber > 0)
                        {
                            currentSymbolType = 1;
                        }
                        else if (entry.StorageClass == Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_FUNCTION
                            && entry.ShortName != null
                            && ((entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x62 && entry.ShortName[2] == 0x66)  // .bf
                                || (entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x65 && entry.ShortName[2] == 0x66))) // .ef
                        {
                            currentSymbolType = 2;
                        }
                        else if (entry.StorageClass == Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                            && entry.SectionNumber == (ushort)Models.PortableExecutable.SectionNumber.IMAGE_SYM_UNDEFINED
                            && entry.Value == 0)
                        {
                            currentSymbolType = 3;
                        }
                        else if (entry.StorageClass == Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_FILE)
                        {
                            // TODO: Symbol name should be ".file"
                            currentSymbolType = 4;
                        }
                        else if (entry.StorageClass == Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_STATIC)
                        {
                            // TODO: Should have the name of a section (like ".text")
                            currentSymbolType = 5;
                        }
                        else if (entry.StorageClass == Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
                        {
                            currentSymbolType = 6;
                        }
                    }
                    else if (currentSymbolType == 1)
                    {
                        Console.WriteLine($"    Tag index = {entry.AuxFormat1TagIndex}");
                        Console.WriteLine($"    Total size = {entry.AuxFormat1TotalSize}");
                        Console.WriteLine($"    Pointer to linenumber = {entry.AuxFormat1PointerToLinenumber}");
                        Console.WriteLine($"    Pointer to next function = {entry.AuxFormat1PointerToNextFunction}");
                        Console.WriteLine($"    Unused = {entry.AuxFormat1Unused}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 2)
                    {
                        Console.WriteLine($"    Unused = {entry.AuxFormat2Unused1}");
                        Console.WriteLine($"    Linenumber = {entry.AuxFormat2Linenumber}");
                        Console.WriteLine($"    Unused = {entry.AuxFormat2Unused2}");
                        Console.WriteLine($"    Pointer to next function = {entry.AuxFormat2PointerToNextFunction}");
                        Console.WriteLine($"    Unused = {entry.AuxFormat2Unused3}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 3)
                    {
                        Console.WriteLine($"    Tag index = {entry.AuxFormat3TagIndex}");
                        Console.WriteLine($"    Characteristics = {entry.AuxFormat3Characteristics}");
                        Console.WriteLine($"    Unused = {BitConverter.ToString(entry.AuxFormat3Unused).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 4)
                    {
                        Console.WriteLine($"    File name = {Encoding.ASCII.GetString(entry.AuxFormat4FileName)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 5)
                    {
                        Console.WriteLine($"    Length = {entry.AuxFormat5Length}");
                        Console.WriteLine($"    Number of relocations = {entry.AuxFormat5NumberOfRelocations}");
                        Console.WriteLine($"    Number of linenumbers = {entry.AuxFormat5NumberOfLinenumbers}");
                        Console.WriteLine($"    Checksum = {entry.AuxFormat5CheckSum}");
                        Console.WriteLine($"    Number = {entry.AuxFormat5Number}");
                        Console.WriteLine($"    Selection = {entry.AuxFormat5Selection}");
                        Console.WriteLine($"    Unused = {BitConverter.ToString(entry.AuxFormat5Unused).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 6)
                    {
                        Console.WriteLine($"    Aux type = {entry.AuxFormat6AuxType}");
                        Console.WriteLine($"    Reserved = {entry.AuxFormat6Reserved1}");
                        Console.WriteLine($"    Symbol table index = {entry.AuxFormat6SymbolTableIndex}");
                        Console.WriteLine($"    Reserved = {BitConverter.ToString(entry.AuxFormat6Reserved2).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }

                    // If we hit the last aux symbol, go back to normal format
                    if (auxSymbolsRemaining == 0)
                        currentSymbolType = 0;
                }

                Console.WriteLine();
                Console.WriteLine("  COFF String Table Information:");
                Console.WriteLine("  -------------------------");
                if (_executable.COFFStringTable == null
                    || _executable.COFFStringTable.Strings == null
                    || _executable.COFFStringTable.Strings.Length == 0)
                {
                    Console.WriteLine("  No COFF string table items");
                }
                else
                {
                    Console.WriteLine($"  Total size: {_executable.COFFStringTable.TotalSize}");
                    for (int i = 0; i < _executable.COFFStringTable.Strings.Length; i++)
                    {
                        string entry = _executable.COFFStringTable.Strings[i];
                        Console.WriteLine($"  COFF String Table Entry {i})");
                        Console.WriteLine($"    Value = {entry}");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Attribute Certificate Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.OptionalHeader?.CertificateTable == null
                || _executable.OptionalHeader.CertificateTable.VirtualAddress == 0
                || _executable.AttributeCertificateTable.Length == 0)
            {
                Console.WriteLine("  No attribute certificate table items");
            }
            else
            {
                for (int i = 0; i < _executable.AttributeCertificateTable.Length; i++)
                {
                    var entry = _executable.AttributeCertificateTable[i];
                    Console.WriteLine($"  Attribute Certificate Table Entry {i}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Revision = {entry.Revision}");
                    Console.WriteLine($"    Certificate type = {entry.CertificateType}");
                    Console.WriteLine();
                    if (entry.CertificateType == Models.PortableExecutable.WindowsCertificateType.WIN_CERT_TYPE_PKCS_SIGNED_DATA)
                    {
                        Console.WriteLine("    Certificate Data [Formatted]");
                        Console.WriteLine("    -------------------------");
                        var topLevelValues = Builder.AbstractSyntaxNotationOne.Parse(entry.Certificate, pointer: 0);
                        if (topLevelValues == null)
                        {
                            Console.WriteLine("    INVALID DATA FOUND");
                            Console.WriteLine($"    {BitConverter.ToString(entry.Certificate).Replace("-", string.Empty)}");
                        }
                        else
                        {
                            foreach (Builder.ASN1TypeLengthValue tlv in topLevelValues)
                            {
                                string tlvString = tlv.Format(paddingLevel: 4);
                                Console.WriteLine(tlvString);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"    Certificate Data [Binary]");
                        Console.WriteLine("  -------------------------");
                        Console.WriteLine($"    {BitConverter.ToString(entry.Certificate).Replace("-", string.Empty)}");
                    }

                    Console.WriteLine();
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Delay-Load Directory Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.OptionalHeader?.DelayImportDescriptor == null
                || _executable.OptionalHeader.DelayImportDescriptor.VirtualAddress == 0
                || _executable.DelayLoadDirectoryTable == null)
            {
                Console.WriteLine("  No delay-load directory table items");
            }
            else
            {
                Console.WriteLine($"  Attributes = {_executable.DelayLoadDirectoryTable.Attributes}");
                Console.WriteLine($"  Name RVA = {_executable.DelayLoadDirectoryTable.Name}");
                Console.WriteLine($"  Module handle = {_executable.DelayLoadDirectoryTable.ModuleHandle}");
                Console.WriteLine($"  Delay import address table RVA = {_executable.DelayLoadDirectoryTable.DelayImportAddressTable}");
                Console.WriteLine($"  Delay import name table RVA = {_executable.DelayLoadDirectoryTable.DelayImportNameTable}");
                Console.WriteLine($"  Bound delay import table RVA = {_executable.DelayLoadDirectoryTable.BoundDelayImportTable}");
                Console.WriteLine($"  Unload delay import table RVA = {_executable.DelayLoadDirectoryTable.UnloadDelayImportTable}");
                Console.WriteLine($"  Timestamp = {_executable.DelayLoadDirectoryTable.TimeStamp}");
            }
            Console.WriteLine();

            Console.WriteLine("  Debug Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.OptionalHeader?.Debug == null
                || _executable.OptionalHeader.Debug.VirtualAddress == 0
                || _executable.DebugTable == null)
            {
                Console.WriteLine("  No debug table items");
            }
            else
            {
                // TODO: If more sections added, model this after the Export Table
                for (int i = 0; i < _executable.DebugTable.DebugDirectoryTable.Length; i++)
                {
                    var debugDirectoryEntry = _executable.DebugTable.DebugDirectoryTable[i];
                    Console.WriteLine($"  Debug Directory Table Entry {i}");
                    Console.WriteLine($"    Characteristics: {debugDirectoryEntry.Characteristics}");
                    Console.WriteLine($"    Time/Date stamp: {debugDirectoryEntry.TimeDateStamp}");
                    Console.WriteLine($"    Major version: {debugDirectoryEntry.MajorVersion}");
                    Console.WriteLine($"    Minor version: {debugDirectoryEntry.MinorVersion}");
                    Console.WriteLine($"    Debug type: {debugDirectoryEntry.DebugType}");
                    Console.WriteLine($"    Size of data: {debugDirectoryEntry.SizeOfData}");
                    Console.WriteLine($"    Address of raw data: {debugDirectoryEntry.AddressOfRawData}");
                    Console.WriteLine($"    Pointer to raw data: {debugDirectoryEntry.PointerToRawData}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Export Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.OptionalHeader?.ExportTable == null
                || _executable.OptionalHeader.ExportTable.VirtualAddress == 0
                || _executable.ExportTable == null)
            {
                Console.WriteLine("  No export table items");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("    Export Directory Table Information:");
                Console.WriteLine("    -------------------------");
                Console.WriteLine($"    Export flags: {_executable.ExportTable.ExportDirectoryTable.ExportFlags}");
                Console.WriteLine($"    Time/Date stamp: {_executable.ExportTable.ExportDirectoryTable.TimeDateStamp}");
                Console.WriteLine($"    Major version: {_executable.ExportTable.ExportDirectoryTable.MajorVersion}");
                Console.WriteLine($"    Minor version: {_executable.ExportTable.ExportDirectoryTable.MinorVersion}");
                Console.WriteLine($"    Name RVA: {_executable.ExportTable.ExportDirectoryTable.NameRVA}");
                Console.WriteLine($"    Name: {_executable.ExportTable.ExportDirectoryTable.Name}");
                Console.WriteLine($"    Ordinal base: {_executable.ExportTable.ExportDirectoryTable.OrdinalBase}");
                Console.WriteLine($"    Address table entries: {_executable.ExportTable.ExportDirectoryTable.AddressTableEntries}");
                Console.WriteLine($"    Number of name pointers: {_executable.ExportTable.ExportDirectoryTable.NumberOfNamePointers}");
                Console.WriteLine($"    Export address table RVA: {_executable.ExportTable.ExportDirectoryTable.ExportAddressTableRVA}");
                Console.WriteLine($"    Name pointer table RVA: {_executable.ExportTable.ExportDirectoryTable.NamePointerRVA}");
                Console.WriteLine($"    Ordinal table RVA: {_executable.ExportTable.ExportDirectoryTable.OrdinalTableRVA}");
                Console.WriteLine();

                Console.WriteLine("    Export Address Table Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ExportTable.ExportAddressTable == null || _executable.ExportTable.ExportAddressTable.Length == 0)
                {
                    Console.WriteLine("    No export address table items");
                }
                else
                {
                    for (int i = 0; i < _executable.ExportTable.ExportAddressTable.Length; i++)
                    {
                        var exportAddressTableEntry = _executable.ExportTable.ExportAddressTable[i];
                        Console.WriteLine($"    Export Address Table Entry {i}");
                        Console.WriteLine($"      Export RVA / Forwarder RVA: {exportAddressTableEntry.ExportRVA}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Name Pointer Table Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ExportTable.NamePointerTable?.Pointers == null || _executable.ExportTable.NamePointerTable.Pointers.Length == 0)
                {
                    Console.WriteLine("    No name pointer table items");
                }
                else
                {
                    for (int i = 0; i < _executable.ExportTable.NamePointerTable.Pointers.Length; i++)
                    {
                        var namePointerTableEntry = _executable.ExportTable.NamePointerTable.Pointers[i];
                        Console.WriteLine($"    Name Pointer Table Entry {i}");
                        Console.WriteLine($"      Pointer: {namePointerTableEntry}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Ordinal Table Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ExportTable.OrdinalTable?.Indexes == null || _executable.ExportTable.OrdinalTable.Indexes.Length == 0)
                {
                    Console.WriteLine("    No ordinal table items");
                }
                else
                {
                    for (int i = 0; i < _executable.ExportTable.OrdinalTable.Indexes.Length; i++)
                    {
                        var ordinalTableEntry = _executable.ExportTable.OrdinalTable.Indexes[i];
                        Console.WriteLine($"    Ordinal Table Entry {i}");
                        Console.WriteLine($"      Index: {ordinalTableEntry}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Export Name Table Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ExportTable.ExportNameTable?.Strings == null || _executable.ExportTable.ExportNameTable.Strings.Length == 0)
                {
                    Console.WriteLine("    No export name table items");
                }
                else
                {
                    for (int i = 0; i < _executable.ExportTable.ExportNameTable.Strings.Length; i++)
                    {
                        var exportNameTableEntry = _executable.ExportTable.ExportNameTable.Strings[i];
                        Console.WriteLine($"    Export Name Table Entry {i}");
                        Console.WriteLine($"      String: {exportNameTableEntry}");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Import Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.OptionalHeader?.ImportTable == null
                || _executable.OptionalHeader.ImportTable.VirtualAddress == 0
                || _executable.ImportTable == null)
            {
                Console.WriteLine("  No import table items");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("    Import Directory Table Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ImportTable.ImportDirectoryTable == null || _executable.ImportTable.ImportDirectoryTable.Length == 0)
                {
                    Console.WriteLine("    No import directory table items");
                }
                else
                {
                    for (int i = 0; i < _executable.ImportTable.ImportDirectoryTable.Length; i++)
                    {
                        var importDirectoryTableEntry = _executable.ImportTable.ImportDirectoryTable[i];
                        Console.WriteLine($"    Import Directory Table Entry {i}");
                        Console.WriteLine($"      Import lookup table RVA: {importDirectoryTableEntry.ImportLookupTableRVA}");
                        Console.WriteLine($"      Time/Date stamp: {importDirectoryTableEntry.TimeDateStamp}");
                        Console.WriteLine($"      Forwarder chain: {importDirectoryTableEntry.ForwarderChain}");
                        Console.WriteLine($"      Name RVA: {importDirectoryTableEntry.NameRVA}");
                        Console.WriteLine($"      Name: {importDirectoryTableEntry.Name}");
                        Console.WriteLine($"      Import address table RVA: {importDirectoryTableEntry.ImportAddressTableRVA}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Import Lookup Tables Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ImportTable.ImportLookupTables == null || _executable.ImportTable.ImportLookupTables.Count == 0)
                {
                    Console.WriteLine("    No import lookup tables");
                }
                else
                {
                    foreach (var kvp in _executable.ImportTable.ImportLookupTables)
                    {
                        int index = kvp.Key;
                        var importLookupTable = kvp.Value;

                        Console.WriteLine();
                        Console.WriteLine($"      Import Lookup Table {index} Information:");
                        Console.WriteLine("      -------------------------");
                        if (importLookupTable == null || importLookupTable.Length == 0)
                        {
                            Console.WriteLine("      No import lookup table items");
                        }
                        else
                        {
                            for (int i = 0; i < importLookupTable.Length; i++)
                            {
                                var importLookupTableEntry = importLookupTable[i];
                                Console.WriteLine($"      Import Lookup Table {index} Entry {i}");
                                Console.WriteLine($"        Ordinal/Name flag: {importLookupTableEntry.OrdinalNameFlag}");
                                if (importLookupTableEntry.OrdinalNameFlag)
                                    Console.WriteLine($"        Ordinal number: {importLookupTableEntry.OrdinalNumber}");
                                else
                                    Console.WriteLine($"        Hint/Name table RVA: {importLookupTableEntry.HintNameTableRVA}");
                            }
                        }
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Import Address Tables Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ImportTable.ImportAddressTables == null || _executable.ImportTable.ImportAddressTables.Count == 0)
                {
                    Console.WriteLine("    No import address tables");
                }
                else
                {
                    foreach (var kvp in _executable.ImportTable.ImportAddressTables)
                    {
                        int index = kvp.Key;
                        var importAddressTable = kvp.Value;

                        Console.WriteLine();
                        Console.WriteLine($"      Import Address Table {index} Information:");
                        Console.WriteLine("      -------------------------");
                        if (importAddressTable == null || importAddressTable.Length == 0)
                        {
                            Console.WriteLine("      No import address table items");
                        }
                        else
                        {
                            for (int i = 0; i < importAddressTable.Length; i++)
                            {
                                var importLookupTableEntry = importAddressTable[i];
                                Console.WriteLine($"      Import Address Table {index} Entry {i}");
                                if (_executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                                    Console.WriteLine($"        Address: {importLookupTableEntry.Address_PE32}");
                                else
                                    Console.WriteLine($"        Address: {importLookupTableEntry.Address_PE32Plus}");
                            }
                        }
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Hint/Name Table Information:");
                Console.WriteLine("    -------------------------");
                if (_executable.ImportTable.HintNameTable == null || _executable.ImportTable.HintNameTable.Length == 0)
                {
                    Console.WriteLine("    No hint/name table items");
                }
                else
                {
                    for (int i = 0; i < _executable.ImportTable.HintNameTable.Length; i++)
                    {
                        var hintNameTableEntry = _executable.ImportTable.HintNameTable[i];
                        Console.WriteLine($"    Hint/Name Table Entry {i}");
                        Console.WriteLine($"      Hint: {hintNameTableEntry.Hint}");
                        Console.WriteLine($"      Name: {hintNameTableEntry.Name}");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Resource Directory Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.OptionalHeader?.ResourceTable == null
                || _executable.OptionalHeader.ResourceTable.VirtualAddress == 0
                || _executable.ResourceDirectoryTable == null)
            {
                Console.WriteLine("  No resource directory table items");
            }
            else
            {
                PrintResourceDirectoryTable(_executable.ResourceDirectoryTable, level: 0, types: new List<object>());
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Pretty print the Portable Executable resource directory table information
        /// </summary>
        private static void PrintResourceDirectoryTable(Models.PortableExecutable.ResourceDirectoryTable table, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            Console.WriteLine($"{padding}Table level: {level}");
            Console.WriteLine($"{padding}Characteristics: {table.Characteristics}");
            Console.WriteLine($"{padding}Time/Date stamp: {table.TimeDateStamp}");
            Console.WriteLine($"{padding}Major version: {table.MajorVersion}");
            Console.WriteLine($"{padding}Minor version: {table.MinorVersion}");
            Console.WriteLine($"{padding}Number of name entries: {table.NumberOfNameEntries}");
            Console.WriteLine($"{padding}Number of ID entries: {table.NumberOfIDEntries}");
            Console.WriteLine();

            Console.WriteLine($"{padding}Name entries");
            Console.WriteLine($"{padding}-------------------------");
            if (table.NumberOfNameEntries == 0)
            {
                Console.WriteLine($"{padding}No named entries");
                Console.WriteLine();
            }
            else
            {
                for (int i = 0; i < table.NumberOfNameEntries; i++)
                {
                    var entry = table.NameEntries[i];
                    var newTypes = new List<object>(types ?? new List<object>());
                    newTypes.Add(Encoding.UTF8.GetString(entry.Name.UnicodeString ?? new byte[0]));
                    PrintNameResourceDirectoryEntry(entry, level + 1, newTypes);
                }
            }

            Console.WriteLine($"{padding}ID entries");
            Console.WriteLine($"{padding}-------------------------");
            if (table.NumberOfIDEntries == 0)
            {
                Console.WriteLine($"{padding}No ID entries");
                Console.WriteLine();
            }
            else
            {

                for (int i = 0; i < table.NumberOfIDEntries; i++)
                {
                    var entry = table.IDEntries[i];
                    var newTypes = new List<object>(types ?? new List<object>());
                    newTypes.Add(entry.IntegerID);
                    PrintIDResourceDirectoryEntry(entry, level + 1, newTypes);
                }
            }
        }

        /// <summary>
        /// Pretty print the Portable Executable name resource directory entry information
        /// </summary>
        private static void PrintNameResourceDirectoryEntry(Models.PortableExecutable.ResourceDirectoryEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            Console.WriteLine($"{padding}Item level: {level}");
            Console.WriteLine($"{padding}Name offset: {entry.NameOffset}");
            Console.WriteLine($"{padding}Name ({entry.Name.Length}): {Encoding.UTF8.GetString(entry.Name.UnicodeString ?? new byte[0])}");
            if (entry.DataEntry != null)
                PrintResourceDataEntry(entry.DataEntry, level: level + 1, types);
            else if (entry.Subdirectory != null)
                PrintResourceDirectoryTable(entry.Subdirectory, level: level + 1, types);
        }

        /// <summary>
        /// Pretty print the Portable Executable ID resource directory entry information
        /// </summary>
        private static void PrintIDResourceDirectoryEntry(Models.PortableExecutable.ResourceDirectoryEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            Console.WriteLine($"{padding}Item level: {level}");
            Console.WriteLine($"{padding}Integer ID: {entry.IntegerID}");
            if (entry.DataEntry != null)
                PrintResourceDataEntry(entry.DataEntry, level: level + 1, types);
            else if (entry.Subdirectory != null)
                PrintResourceDirectoryTable(entry.Subdirectory, level: level + 1, types);
        }

        /// <summary>
        /// Pretty print the Portable Executable resource data entry information
        /// </summary>
        private static void PrintResourceDataEntry(Models.PortableExecutable.ResourceDataEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            // TODO: Use ordered list of base types to determine the shape of the data
            //Console.WriteLine($"{padding}Base types: {string.Join(", ", types)}");

            Console.WriteLine($"{padding}Entry level: {level}");
            Console.WriteLine($"{padding}Data RVA: {entry.DataRVA}");
            Console.WriteLine($"{padding}Size: {entry.Size}");
            Console.WriteLine($"{padding}Codepage: {entry.Codepage}");
            Console.WriteLine($"{padding}Reserved: {entry.Reserved}");

            // TODO: Print out per-type data
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                switch ((Models.PortableExecutable.ResourceType)resourceType)
                {
                    case Models.PortableExecutable.ResourceType.RT_CURSOR:
                        Console.WriteLine($"{padding}Hardware-dependent cursor resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_BITMAP:
                        Console.WriteLine($"{padding}Bitmap resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ICON:
                        Console.WriteLine($"{padding}Hardware-dependent icon resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_MENU:
                        var menu = entry.AsMenu();
                        if (menu == null)
                        {
                            Console.WriteLine($"{padding}Menu resource found, but malformed");
                        }
                        else
                        {
                            if (menu.MenuHeader != null)
                            {
                                Console.WriteLine($"{padding}Version: {menu.MenuHeader.Version}");
                                Console.WriteLine($"{padding}Header size: {menu.MenuHeader.HeaderSize}");
                                Console.WriteLine();
                                Console.WriteLine($"{padding}Menu items");
                                Console.WriteLine($"{padding}-------------------------");
                                if (menu.MenuItems == null
                                    || menu.MenuItems.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No menu items");
                                }
                                else
                                {
                                    for (int i = 0; i < menu.MenuItems.Length; i++)
                                    {
                                        var menuItem = menu.MenuItems[i];

                                        Console.WriteLine($"{padding}Menu item {i}");
                                        if (menuItem.NormalMenuText != null)
                                        {
                                            Console.WriteLine($"{padding}  Resource info: {menuItem.NormalResInfo}");
                                            Console.WriteLine($"{padding}  Menu text: {menuItem.NormalMenuText}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{padding}  Item type: {menuItem.PopupItemType}");
                                            Console.WriteLine($"{padding}  State: {menuItem.PopupState}");
                                            Console.WriteLine($"{padding}  ID: {menuItem.PopupID}");
                                            Console.WriteLine($"{padding}  Resource info: {menuItem.PopupResInfo}");
                                            Console.WriteLine($"{padding}  Menu text: {menuItem.PopupMenuText}");
                                        }
                                    }
                                }
                            }
                            else if (menu.ExtendedMenuHeader != null)
                            {
                                Console.WriteLine($"{padding}Version: {menu.ExtendedMenuHeader.Version}");
                                Console.WriteLine($"{padding}Offset: {menu.ExtendedMenuHeader.Offset}");
                                Console.WriteLine($"{padding}Help ID: {menu.ExtendedMenuHeader.HelpID}");
                                Console.WriteLine();
                                Console.WriteLine($"{padding}Menu items");
                                Console.WriteLine($"{padding}-------------------------");
                                if (menu.ExtendedMenuHeader.Offset == 0
                                    || menu.ExtendedMenuItems == null
                                    || menu.ExtendedMenuItems.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No menu items");
                                }
                                else
                                {
                                    for (int i = 0; i < menu.ExtendedMenuItems.Length; i++)
                                    {
                                        var menuItem = menu.ExtendedMenuItems[i];

                                        Console.WriteLine($"{padding}Dialog item template {i}");
                                        Console.WriteLine($"{padding}  Item type: {menuItem.ItemType}");
                                        Console.WriteLine($"{padding}  State: {menuItem.State}");
                                        Console.WriteLine($"{padding}  ID: {menuItem.ID}");
                                        Console.WriteLine($"{padding}  Flags: {menuItem.Flags}");
                                        Console.WriteLine($"{padding}  Menu text: {menuItem.MenuText}");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{padding}Menu resource found, but malformed");
                            }
                        }
                        break;
                    case Models.PortableExecutable.ResourceType.RT_DIALOG:
                        var dialogBox = entry.AsDialogBox();
                        if (dialogBox == null)
                        {
                            Console.WriteLine($"{padding}Dialog box resource found, but malformed");
                        }
                        else
                        {
                            if (dialogBox.DialogTemplate != null)
                            {
                                Console.WriteLine($"{padding}Style: {dialogBox.DialogTemplate.Style}");
                                Console.WriteLine($"{padding}Extended style: {dialogBox.DialogTemplate.ExtendedStyle}");
                                Console.WriteLine($"{padding}Item count: {dialogBox.DialogTemplate.ItemCount}");
                                Console.WriteLine($"{padding}X-coordinate of upper-left corner: {dialogBox.DialogTemplate.PositionX}");
                                Console.WriteLine($"{padding}Y-coordinate of upper-left corner: {dialogBox.DialogTemplate.PositionY}");
                                Console.WriteLine($"{padding}Width of the dialog box: {dialogBox.DialogTemplate.WidthX}");
                                Console.WriteLine($"{padding}Height of the dialog box: {dialogBox.DialogTemplate.HeightY}");
                                Console.WriteLine($"{padding}Menu resource: {dialogBox.DialogTemplate.MenuResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Menu resource ordinal: {dialogBox.DialogTemplate.MenuResourceOrdinal}");
                                Console.WriteLine($"{padding}Class resource: {dialogBox.DialogTemplate.ClassResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Class resource ordinal: {dialogBox.DialogTemplate.ClassResourceOrdinal}");
                                Console.WriteLine($"{padding}Title resource: {dialogBox.DialogTemplate.TitleResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Point size value: {dialogBox.DialogTemplate.PointSizeValue}");
                                Console.WriteLine($"{padding}Typeface: {dialogBox.DialogTemplate.Typeface ?? "[EMPTY]"}");
                                Console.WriteLine();
                                Console.WriteLine($"{padding}Dialog item templates");
                                Console.WriteLine($"{padding}-------------------------");
                                if (dialogBox.DialogTemplate.ItemCount == 0
                                    || dialogBox.DialogItemTemplates == null
                                    || dialogBox.DialogItemTemplates.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No dialog item templates");
                                }
                                else
                                {
                                    for (int i = 0; i < dialogBox.DialogItemTemplates.Length; i++)
                                    {
                                        var dialogItemTemplate = dialogBox.DialogItemTemplates[i];

                                        Console.WriteLine($"{padding}Dialog item template {i}");
                                        Console.WriteLine($"{padding}  Style: {dialogItemTemplate.Style}");
                                        Console.WriteLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle}");
                                        Console.WriteLine($"{padding}  X-coordinate of upper-left corner: {dialogItemTemplate.PositionX}");
                                        Console.WriteLine($"{padding}  Y-coordinate of upper-left corner: {dialogItemTemplate.PositionY}");
                                        Console.WriteLine($"{padding}  Width of the control: {dialogItemTemplate.WidthX}");
                                        Console.WriteLine($"{padding}  Height of the control: {dialogItemTemplate.HeightY}");
                                        Console.WriteLine($"{padding}  ID: {dialogItemTemplate.ID}");
                                        Console.WriteLine($"{padding}  Class resource: {dialogItemTemplate.ClassResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Title resource: {dialogItemTemplate.TitleResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Title resource ordinal: {dialogItemTemplate.TitleResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Creation data size: {dialogItemTemplate.CreationDataSize}");
                                        if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                                            Console.WriteLine($"{padding}  Creation data: {BitConverter.ToString(dialogItemTemplate.CreationData).Replace("-", string.Empty)}");
                                        else
                                            Console.WriteLine($"{padding}  Creation data: [EMPTY]");
                                    }
                                }
                            }
                            else if (dialogBox.ExtendedDialogTemplate != null)
                            {
                                Console.WriteLine($"{padding}Version: {dialogBox.ExtendedDialogTemplate.Version}");
                                Console.WriteLine($"{padding}Signature: {dialogBox.ExtendedDialogTemplate.Signature}");
                                Console.WriteLine($"{padding}Help ID: {dialogBox.ExtendedDialogTemplate.HelpID}");
                                Console.WriteLine($"{padding}Extended style: {dialogBox.ExtendedDialogTemplate.ExtendedStyle}");
                                Console.WriteLine($"{padding}Style: {dialogBox.ExtendedDialogTemplate.Style}");
                                Console.WriteLine($"{padding}Item count: {dialogBox.ExtendedDialogTemplate.DialogItems}");
                                Console.WriteLine($"{padding}X-coordinate of upper-left corner: {dialogBox.ExtendedDialogTemplate.PositionX}");
                                Console.WriteLine($"{padding}Y-coordinate of upper-left corner: {dialogBox.ExtendedDialogTemplate.PositionY}");
                                Console.WriteLine($"{padding}Width of the dialog box: {dialogBox.ExtendedDialogTemplate.WidthX}");
                                Console.WriteLine($"{padding}Height of the dialog box: {dialogBox.ExtendedDialogTemplate.HeightY}");
                                Console.WriteLine($"{padding}Menu resource: {dialogBox.ExtendedDialogTemplate.MenuResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Menu resource ordinal: {dialogBox.ExtendedDialogTemplate.MenuResourceOrdinal}");
                                Console.WriteLine($"{padding}Class resource: {dialogBox.ExtendedDialogTemplate.ClassResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Class resource ordinal: {dialogBox.ExtendedDialogTemplate.ClassResourceOrdinal}");
                                Console.WriteLine($"{padding}Title resource: {dialogBox.ExtendedDialogTemplate.TitleResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Point size: {dialogBox.ExtendedDialogTemplate.PointSize}");
                                Console.WriteLine($"{padding}Weight: {dialogBox.ExtendedDialogTemplate.Weight}");
                                Console.WriteLine($"{padding}Italic: {dialogBox.ExtendedDialogTemplate.Italic}");
                                Console.WriteLine($"{padding}Character set: {dialogBox.ExtendedDialogTemplate.CharSet}");
                                Console.WriteLine($"{padding}Typeface: {dialogBox.ExtendedDialogTemplate.Typeface ?? "[EMPTY]"}");
                                Console.WriteLine();
                                Console.WriteLine($"{padding}Dialog item templates");
                                Console.WriteLine($"{padding}-------------------------");
                                if (dialogBox.ExtendedDialogTemplate.DialogItems == 0
                                    || dialogBox.ExtendedDialogItemTemplates == null
                                    || dialogBox.ExtendedDialogItemTemplates.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No dialog item templates");
                                }
                                else
                                {
                                    for (int i = 0; i < dialogBox.ExtendedDialogItemTemplates.Length; i++)
                                    {
                                        var dialogItemTemplate = dialogBox.ExtendedDialogItemTemplates[i];

                                        Console.WriteLine($"{padding}Dialog item template {i}");
                                        Console.WriteLine($"{padding}  Help ID: {dialogItemTemplate.HelpID}");
                                        Console.WriteLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle}");
                                        Console.WriteLine($"{padding}  Style: {dialogItemTemplate.Style}");
                                        Console.WriteLine($"{padding}  X-coordinate of upper-left corner: {dialogItemTemplate.PositionX}");
                                        Console.WriteLine($"{padding}  Y-coordinate of upper-left corner: {dialogItemTemplate.PositionY}");
                                        Console.WriteLine($"{padding}  Width of the control: {dialogItemTemplate.WidthX}");
                                        Console.WriteLine($"{padding}  Height of the control: {dialogItemTemplate.HeightY}");
                                        Console.WriteLine($"{padding}  ID: {dialogItemTemplate.ID}");
                                        Console.WriteLine($"{padding}  Class resource: {dialogItemTemplate.ClassResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Title resource: {dialogItemTemplate.TitleResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Title resource ordinal: {dialogItemTemplate.TitleResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Creation data size: {dialogItemTemplate.CreationDataSize}");
                                        if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                                            Console.WriteLine($"{padding}  Creation data: {BitConverter.ToString(dialogItemTemplate.CreationData).Replace("-", string.Empty)}");
                                        else
                                            Console.WriteLine($"{padding}  Creation data: [EMPTY]");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{padding}Dialog box resource found, but malformed");
                            }
                        }
                        break;
                    case Models.PortableExecutable.ResourceType.RT_STRING:
                        var stringTable = entry.AsStringTable();
                        if (stringTable == null)
                        {
                            Console.WriteLine($"{padding}String table resource found, but malformed");
                        }
                        else
                        {
                            foreach (var kvp in stringTable)
                            {
                                int index = kvp.Key;
                                string stringValue = kvp.Value;
                                Console.WriteLine($"{padding}String entry {index}: {stringValue}");
                            }
                        }
                        break;
                    case Models.PortableExecutable.ResourceType.RT_FONTDIR:
                        Console.WriteLine($"{padding}Font directory resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_FONT:
                        Console.WriteLine($"{padding}Font resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ACCELERATOR:
                        var acceleratorTable = entry.AsAcceleratorTableResource();
                        if (acceleratorTable == null)
                        {
                            Console.WriteLine($"{padding}Accelerator table resource found, but malformed");
                        }
                        else
                        {
                            for (int i = 0; i < acceleratorTable.Length; i++)
                            {
                                var acceleratorTableEntry = acceleratorTable[i];
                                Console.WriteLine($"{padding}Flags: {acceleratorTableEntry.Flags}");
                                Console.WriteLine($"{padding}Ansi: {acceleratorTableEntry.Ansi}");
                                Console.WriteLine($"{padding}Id: {acceleratorTableEntry.Id}");
                                Console.WriteLine($"{padding}Padding: {acceleratorTableEntry.Padding}");
                            }
                        }
                        break;
                    case Models.PortableExecutable.ResourceType.RT_RCDATA:
                        Console.WriteLine($"{padding}Application-defined resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_MESSAGETABLE:
                        var messageTable = entry.AsMessageResourceData();
                        if (messageTable == null)
                        {
                            Console.WriteLine($"{padding}Message resource data found, but malformed");
                        }
                        else
                        {
                            Console.WriteLine($"{padding}Number of blocks: {messageTable.NumberOfBlocks}");
                            Console.WriteLine();
                            Console.WriteLine($"{padding}Message resource blocks");
                            Console.WriteLine($"{padding}-------------------------");
                            if (messageTable.NumberOfBlocks == 0
                                || messageTable.Blocks == null
                                || messageTable.Blocks.Length == 0)
                            {
                                Console.WriteLine($"{padding}No message resource blocks");
                            }
                            else
                            {
                                for (int i = 0; i < messageTable.Blocks.Length; i++)
                                {
                                    var messageResourceBlock = messageTable.Blocks[i];

                                    Console.WriteLine($"{padding}Message resource block {i}");
                                    Console.WriteLine($"{padding}  Low ID: {messageResourceBlock.LowId}");
                                    Console.WriteLine($"{padding}  High ID: {messageResourceBlock.HighId}");
                                    Console.WriteLine($"{padding}  Offset to entries: {messageResourceBlock.OffsetToEntries}");
                                }
                            }
                            Console.WriteLine();

                            Console.WriteLine($"{padding}Message resource entries");
                            Console.WriteLine($"{padding}-------------------------");
                            if (messageTable.Entries == null
                                || messageTable.Entries.Count == 0)
                            {
                                Console.WriteLine($"{padding}No message resource entries");
                            }
                            else
                            {
                                foreach (var kvp in messageTable.Entries)
                                {
                                    uint index = kvp.Key;
                                    var messageResourceEntry = kvp.Value;

                                    Console.WriteLine($"{padding}Message resource entry {index}");
                                    Console.WriteLine($"{padding}  Length: {messageResourceEntry.Length}");
                                    Console.WriteLine($"{padding}  Flags: {messageResourceEntry.Flags}");
                                    Console.WriteLine($"{padding}  Text: {messageResourceEntry.Text}");
                                }
                            }
                        }
                        break;
                    case Models.PortableExecutable.ResourceType.RT_GROUP_CURSOR:
                        Console.WriteLine($"{padding}Hardware-independent cursor resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_GROUP_ICON:
                        Console.WriteLine($"{padding}Hardware-independent icon resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_VERSION:
                        var versionInfo = entry.AsVersionInfo();
                        if (versionInfo == null)
                        {
                            Console.WriteLine($"{padding}Version info resource found, but malformed");
                        }
                        else
                        {
                            Console.WriteLine($"{padding}Length: {versionInfo.Length}");
                            Console.WriteLine($"{padding}Value length: {versionInfo.ValueLength}");
                            Console.WriteLine($"{padding}Resource type: {versionInfo.ResourceType}");
                            Console.WriteLine($"{padding}Key: {versionInfo.Key}");
                            if (versionInfo.ValueLength != 0 && versionInfo.Value != null)
                            {
                                Console.WriteLine($"{padding}[Fixed File Info] Signature: {versionInfo.Value.Signature}");
                                Console.WriteLine($"{padding}[Fixed File Info] Struct version: {versionInfo.Value.StrucVersion}");
                                Console.WriteLine($"{padding}[Fixed File Info] File version (MS): {versionInfo.Value.FileVersionMS}");
                                Console.WriteLine($"{padding}[Fixed File Info] File version (LS): {versionInfo.Value.FileVersionLS}");
                                Console.WriteLine($"{padding}[Fixed File Info] Product version (MS): {versionInfo.Value.ProductVersionMS}");
                                Console.WriteLine($"{padding}[Fixed File Info] Product version (LS): {versionInfo.Value.ProductVersionLS}");
                                Console.WriteLine($"{padding}[Fixed File Info] File flags mask: {versionInfo.Value.FileFlagsMask}");
                                Console.WriteLine($"{padding}[Fixed File Info] File flags: {versionInfo.Value.FileFlags}");
                                Console.WriteLine($"{padding}[Fixed File Info] File OS: {versionInfo.Value.FileOS}");
                                Console.WriteLine($"{padding}[Fixed File Info] Type: {versionInfo.Value.FileType}");
                                Console.WriteLine($"{padding}[Fixed File Info] Subtype: {versionInfo.Value.FileSubtype}");
                                Console.WriteLine($"{padding}[Fixed File Info] File date (MS): {versionInfo.Value.FileDateMS}");
                                Console.WriteLine($"{padding}[Fixed File Info] File date (LS): {versionInfo.Value.FileDateLS}");
                            }
                            if (versionInfo.StringFileInfo != null)
                            {
                                Console.WriteLine($"{padding}[String File Info] Length: {versionInfo.StringFileInfo.Length}");
                                Console.WriteLine($"{padding}[String File Info] Value length: {versionInfo.StringFileInfo.ValueLength}");
                                Console.WriteLine($"{padding}[String File Info] Resource type: {versionInfo.StringFileInfo.ResourceType}");
                                Console.WriteLine($"{padding}[String File Info] Key: {versionInfo.StringFileInfo.Key}");
                                Console.WriteLine($"{padding}Children:");
                                Console.WriteLine($"{padding}-------------------------");
                                if (versionInfo.StringFileInfo.Children == null || versionInfo.StringFileInfo.Children.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No string file info children");
                                }
                                else
                                {
                                    for (int i = 0; i < versionInfo.StringFileInfo.Children.Length; i++)
                                    {
                                        var stringFileInfoChildEntry = versionInfo.StringFileInfo.Children[i];

                                        Console.WriteLine($"{padding}  [String Table {i}] Length: {stringFileInfoChildEntry.Length}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Value length: {stringFileInfoChildEntry.ValueLength}");
                                        Console.WriteLine($"{padding}  [String Table {i}] ResourceType: {stringFileInfoChildEntry.ResourceType}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Key: {stringFileInfoChildEntry.Key}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Children:");
                                        Console.WriteLine($"{padding}  -------------------------");
                                        if (stringFileInfoChildEntry.Children == null || stringFileInfoChildEntry.Children.Length == 0)
                                        {
                                            Console.WriteLine($"{padding}  No string table {i} children");
                                        }
                                        else
                                        {
                                            for (int j = 0; j < stringFileInfoChildEntry.Children.Length; j++)
                                            {
                                                var stringDataEntry = stringFileInfoChildEntry.Children[j];

                                                Console.WriteLine($"{padding}    [String Data {j}] Length: {stringDataEntry.Length}");
                                                Console.WriteLine($"{padding}    [String Data {j}] Value length: {stringDataEntry.ValueLength}");
                                                Console.WriteLine($"{padding}    [String Data {j}] ResourceType: {stringDataEntry.ResourceType}");
                                                Console.WriteLine($"{padding}    [String Data {j}] Key: {stringDataEntry.Key}");
                                                Console.WriteLine($"{padding}    [String Data {j}] Value: {stringDataEntry.Value}");
                                            }
                                        }
                                    }
                                }
                            }
                            if (versionInfo.VarFileInfo != null)
                            {
                                Console.WriteLine($"{padding}[Var File Info] Length: {versionInfo.VarFileInfo.Length}");
                                Console.WriteLine($"{padding}[Var File Info] Value length: {versionInfo.VarFileInfo.ValueLength}");
                                Console.WriteLine($"{padding}[Var File Info] Resource type: {versionInfo.VarFileInfo.ResourceType}");
                                Console.WriteLine($"{padding}[Var File Info] Key: {versionInfo.VarFileInfo.Key}");
                                Console.WriteLine($"{padding}Children:");
                                Console.WriteLine($"{padding}-------------------------");
                                if (versionInfo.VarFileInfo.Children == null || versionInfo.VarFileInfo.Children.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No var file info children");
                                }
                                else
                                {
                                    for (int i = 0; i < versionInfo.VarFileInfo.Children.Length; i++)
                                    {
                                        var varFileInfoChildEntry = versionInfo.VarFileInfo.Children[i];

                                        Console.WriteLine($"{padding}  [String Table {i}] Length: {varFileInfoChildEntry.Length}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Value length: {varFileInfoChildEntry.ValueLength}");
                                        Console.WriteLine($"{padding}  [String Table {i}] ResourceType: {varFileInfoChildEntry.ResourceType}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Key: {varFileInfoChildEntry.Key}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Value: {string.Join(",", varFileInfoChildEntry.Value)}");
                                    }
                                }
                            }
                        }
                        break;
                    case Models.PortableExecutable.ResourceType.RT_DLGINCLUDE:
                        Console.WriteLine($"{padding}External header resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_PLUGPLAY:
                        Console.WriteLine($"{padding}Plug and Play resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_VXD:
                        Console.WriteLine($"{padding}VXD found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ANICURSOR:
                        Console.WriteLine($"{padding}Animated cursor found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ANIICON:
                        Console.WriteLine($"{padding}Animated icon found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_HTML:
                        Console.WriteLine($"{padding}HTML resource found, not parsed yet");
                        break;
                    case Models.PortableExecutable.ResourceType.RT_MANIFEST:
                        var assemblyManifest = entry.AsAssemblyManifest();
                        if (assemblyManifest == null)
                        {
                            Console.WriteLine($"{padding}Assembly manifest found, but malformed");
                        }
                        else
                        {
                            Console.WriteLine($"{padding}Manifest version: {assemblyManifest.ManifestVersion}");
                            if (assemblyManifest.AssemblyIdentities != null && assemblyManifest.AssemblyIdentities.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.AssemblyIdentities.Length; i++)
                                {
                                    var assemblyIdentity = assemblyManifest.AssemblyIdentities[i];
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Name: {assemblyIdentity.Name}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Version: {assemblyIdentity.Version}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Type: {assemblyIdentity.Type}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Processor architecture: {assemblyIdentity.ProcessorArchitecture}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Public key token: {assemblyIdentity.PublicKeyToken}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Language: {assemblyIdentity.Language}");
                                }
                            }
                            if (assemblyManifest.Description != null)
                            {
                                Console.WriteLine($"{padding}[Assembly Description] Value: {assemblyManifest.Description.Value}");
                            }
                            if (assemblyManifest.COMInterfaceExternalProxyStub != null && assemblyManifest.COMInterfaceExternalProxyStub.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.COMInterfaceExternalProxyStub.Length; i++)
                                {
                                    var comInterfaceExternalProxyStub = assemblyManifest.COMInterfaceExternalProxyStub[i];
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] IID: {comInterfaceExternalProxyStub.IID}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Name: {comInterfaceExternalProxyStub.Name}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] TLBID: {comInterfaceExternalProxyStub.TLBID}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Number of methods: {comInterfaceExternalProxyStub.NumMethods}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Proxy stub (CLSID32): {comInterfaceExternalProxyStub.ProxyStubClsid32}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Base interface: {comInterfaceExternalProxyStub.BaseInterface}");
                                }
                            }
                            if (assemblyManifest.Dependency != null && assemblyManifest.Dependency.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.Dependency.Length; i++)
                                {
                                    var dependency = assemblyManifest.Dependency[i];
                                    if (dependency.DependentAssembly != null)
                                    {
                                        if (dependency.DependentAssembly.AssemblyIdentity != null)
                                        {
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Name: {dependency.DependentAssembly.AssemblyIdentity.Name}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Version: {dependency.DependentAssembly.AssemblyIdentity.Version}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Type: {dependency.DependentAssembly.AssemblyIdentity.Type}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Processor architecture: {dependency.DependentAssembly.AssemblyIdentity.ProcessorArchitecture}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Public key token: {dependency.DependentAssembly.AssemblyIdentity.PublicKeyToken}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Language: {dependency.DependentAssembly.AssemblyIdentity.Language}");
                                        }
                                        if (dependency.DependentAssembly.BindingRedirect != null && dependency.DependentAssembly.BindingRedirect.Length > 0)
                                        {
                                            for (int j = 0; j < dependency.DependentAssembly.BindingRedirect.Length; j++)
                                            {
                                                var bindingRedirect = dependency.DependentAssembly.BindingRedirect[j];
                                                Console.WriteLine($"{padding}[Dependency {i} Binding Redirect {j}] Old version: {bindingRedirect.OldVersion}");
                                                Console.WriteLine($"{padding}[Dependency {i} Binding Redirect {j}] New version: {bindingRedirect.NewVersion}");
                                            }
                                        }
                                    }

                                    Console.WriteLine($"{padding}[Dependency {i}] Optional: {dependency.Optional}");
                                }
                            }
                            if (assemblyManifest.File != null && assemblyManifest.File.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.File.Length; i++)
                                {
                                    var file = assemblyManifest.File[i];
                                    Console.WriteLine($"{padding}[File {i}] Name: {file.Name}");
                                    Console.WriteLine($"{padding}[File {i}] Hash: {file.Hash}");
                                    Console.WriteLine($"{padding}[File {i}] Hash algorithm: {file.HashAlgorithm}");
                                    Console.WriteLine($"{padding}[File {i}] Size: {file.Size}");

                                    if (file.COMClass != null && file.COMClass.Length > 0)
                                    {
                                        for (int j = 0; j < file.COMClass.Length; j++)
                                        {
                                            var comClass = file.COMClass[j];
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] CLSID: {comClass.CLSID}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] Threading model: {comClass.ThreadingModel}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] Prog ID: {comClass.ProgID}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] TLBID: {comClass.TLBID}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] Description: {comClass.Description}");

                                            if (comClass.ProgIDs != null && comClass.ProgIDs.Length > 0)
                                            {
                                                for (int k = 0; k < comClass.ProgIDs.Length; k++)
                                                {
                                                    var progId = comClass.ProgIDs[k];
                                                    Console.WriteLine($"{padding}[File {i} COM Class {j} Prog ID {k}] Value: {progId.Value}");
                                                }
                                            }
                                        }
                                    }
                                    if (file.COMInterfaceProxyStub != null && file.COMInterfaceProxyStub.Length > 0)
                                    {
                                        for (int j = 0; j < file.COMInterfaceProxyStub.Length; j++)
                                        {
                                            var comInterfaceProxyStub = file.COMInterfaceProxyStub[j];
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] IID: {comInterfaceProxyStub.IID}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Name: {comInterfaceProxyStub.Name}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] TLBID: {comInterfaceProxyStub.TLBID}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Number of methods: {comInterfaceProxyStub.NumMethods}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Proxy stub (CLSID32): {comInterfaceProxyStub.ProxyStubClsid32}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Base interface: {comInterfaceProxyStub.BaseInterface}");
                                        }
                                    }
                                    if (file.Typelib != null && file.Typelib.Length > 0)
                                    {
                                        for (int j = 0; j < file.Typelib.Length; j++)
                                        {
                                            var typeLib = file.Typelib[j];
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] TLBID: {typeLib.TLBID}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Version: {typeLib.Version}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Help directory: {typeLib.HelpDir}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Resource ID: {typeLib.ResourceID}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Flags: {typeLib.Flags}");
                                        }
                                    }
                                    if (file.WindowClass != null && file.WindowClass.Length > 0)
                                    {
                                        for (int j = 0; j < file.WindowClass.Length; j++)
                                        {
                                            var windowClass = file.WindowClass[j];
                                            Console.WriteLine($"{padding}[File {i} Window Class {j}] Versioned: {windowClass.Versioned}");
                                            Console.WriteLine($"{padding}[File {i} Window Class {j}] Value: {windowClass.Value}");
                                        }
                                    }
                                }
                            }
                            if (assemblyManifest.EverythingElse != null && assemblyManifest.EverythingElse.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.EverythingElse.Length; i++)
                                {
                                    var thing = assemblyManifest.EverythingElse[i];
                                    if (thing is XmlElement element)
                                    {
                                        Console.WriteLine($"{padding}Unparsed XML Element {i}: {element.OuterXml}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{padding}Unparsed Item {i}: {thing}");
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        Console.WriteLine($"{padding}Type {(Models.PortableExecutable.ResourceType)resourceType} found, not parsed yet");
                        //Console.WriteLine($"{padding}Data: {BitConverter.ToString(entry.Data).Replace("-", string.Empty)}");
                        //Console.WriteLine($"{padding}Data: {Encoding.Unicode.GetString(entry.Data)}");
                        break;
                }
            }
            else if (types != null && types.Count > 0 && types[0] is string resourceString)
            {
                Console.WriteLine($"{padding}Custom data type: {resourceString}");
                //Console.WriteLine($"{padding}Data: {BitConverter.ToString(entry.Data).Replace("-", string.Empty)}");
                //Console.WriteLine($"{padding}Data: {Encoding.Unicode.GetString(entry.Data)}");
            }

            Console.WriteLine();
        }
    }
}