using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SabreTools.ASN1;
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
        public string Stub_Magic => _model.Stub.Header.Magic;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
        public ushort Stub_LastPageBytes => _model.Stub.Header.LastPageBytes;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
        public ushort Stub_Pages => _model.Stub.Header.Pages;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
        public ushort Stub_RelocationItems => _model.Stub.Header.RelocationItems;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
        public ushort Stub_HeaderParagraphSize => _model.Stub.Header.HeaderParagraphSize;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
        public ushort Stub_MinimumExtraParagraphs => _model.Stub.Header.MinimumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
        public ushort Stub_MaximumExtraParagraphs => _model.Stub.Header.MaximumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
        public ushort Stub_InitialSSValue => _model.Stub.Header.InitialSSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
        public ushort Stub_InitialSPValue => _model.Stub.Header.InitialSPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
        public ushort Stub_Checksum => _model.Stub.Header.Checksum;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
        public ushort Stub_InitialIPValue => _model.Stub.Header.InitialIPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
        public ushort Stub_InitialCSValue => _model.Stub.Header.InitialCSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
        public ushort Stub_RelocationTableAddr => _model.Stub.Header.RelocationTableAddr;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
        public ushort Stub_OverlayNumber => _model.Stub.Header.OverlayNumber;

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
        public ushort[] Stub_Reserved1 => _model.Stub.Header.Reserved1;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
        public ushort Stub_OEMIdentifier => _model.Stub.Header.OEMIdentifier;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
        public ushort Stub_OEMInformation => _model.Stub.Header.OEMInformation;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
        public ushort[] Stub_Reserved2 => _model.Stub.Header.Reserved2;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
        public uint Stub_NewExeHeaderAddr => _model.Stub.Header.NewExeHeaderAddr;

        #endregion

        #endregion

        /// <inheritdoc cref="Models.PortableExecutable.Executable.Signature"/>
        public string Signature => _model.Signature;

        #region COFF File Header

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.Machine"/>
        public SabreTools.Models.PortableExecutable.MachineType Machine => _model.COFFFileHeader.Machine;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.NumberOfSections"/>
        public ushort NumberOfSections => _model.COFFFileHeader.NumberOfSections;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.TimeDateStamp"/>
        public uint TimeDateStamp => _model.COFFFileHeader.TimeDateStamp;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.PointerToSymbolTable"/>
        public uint PointerToSymbolTable => _model.COFFFileHeader.PointerToSymbolTable;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.NumberOfSymbols"/>
        public uint NumberOfSymbols => _model.COFFFileHeader.NumberOfSymbols;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.SizeOfOptionalHeader"/>
        public uint SizeOfOptionalHeader => _model.COFFFileHeader.SizeOfOptionalHeader;

        /// <inheritdoc cref="Models.PortableExecutable.COFFFileHeader.Characteristics"/>
        public SabreTools.Models.PortableExecutable.Characteristics Characteristics => _model.COFFFileHeader.Characteristics;

        #endregion

        #region Optional Header

        #region Standard Fields

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Machine"/>
        public SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber OH_Magic => _model.OptionalHeader.Magic;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorLinkerVersion"/>
        public byte OH_MajorLinkerVersion => _model.OptionalHeader.MajorLinkerVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorLinkerVersion"/>
        public byte OH_MinorLinkerVersion => _model.OptionalHeader.MinorLinkerVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfCode"/>
        public uint OH_SizeOfCode => _model.OptionalHeader.SizeOfCode;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfInitializedData"/>
        public uint OH_SizeOfInitializedData => _model.OptionalHeader.SizeOfInitializedData;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfUninitializedData"/>
        public uint OH_SizeOfUninitializedData => _model.OptionalHeader.SizeOfUninitializedData;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.AddressOfEntryPoint"/>
        public uint OH_AddressOfEntryPoint => _model.OptionalHeader.AddressOfEntryPoint;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseOfCode"/>
        public uint OH_BaseOfCode => _model.OptionalHeader.BaseOfCode;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseOfData"/>
        public uint? OH_BaseOfData => _model.OptionalHeader.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? (uint?)_model.OptionalHeader.BaseOfData
            : null;

        #endregion

        #region Windows-Specific Fields

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImageBase_PE32"/>
        public ulong OH_ImageBase => _model.OptionalHeader.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.ImageBase_PE32
            : _model.OptionalHeader.ImageBase_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SectionAlignment"/>
        public uint OH_SectionAlignment => _model.OptionalHeader.SectionAlignment;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.FileAlignment"/>
        public uint OH_FileAlignment => _model.OptionalHeader.FileAlignment;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorOperatingSystemVersion"/>
        public ushort OH_MajorOperatingSystemVersion => _model.OptionalHeader.MajorOperatingSystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorOperatingSystemVersion"/>
        public ushort OH_MinorOperatingSystemVersion => _model.OptionalHeader.MinorOperatingSystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorImageVersion"/>
        public ushort OH_MajorImageVersion => _model.OptionalHeader.MajorImageVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorImageVersion"/>
        public ushort OH_MinorImageVersion => _model.OptionalHeader.MinorImageVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MajorSubsystemVersion"/>
        public ushort OH_MajorSubsystemVersion => _model.OptionalHeader.MajorSubsystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.MinorSubsystemVersion"/>
        public ushort OH_MinorSubsystemVersion => _model.OptionalHeader.MinorSubsystemVersion;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Win32VersionValue"/>
        public uint OH_Win32VersionValue => _model.OptionalHeader.Win32VersionValue;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfImage"/>
        public uint OH_SizeOfImage => _model.OptionalHeader.SizeOfImage;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeaders"/>
        public uint OH_SizeOfHeaders => _model.OptionalHeader.SizeOfHeaders;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CheckSum"/>
        public uint OH_CheckSum => _model.OptionalHeader.CheckSum;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Subsystem"/>
        public SabreTools.Models.PortableExecutable.WindowsSubsystem OH_Subsystem => _model.OptionalHeader.Subsystem;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.DllCharacteristics"/>
        public SabreTools.Models.PortableExecutable.DllCharacteristics OH_DllCharacteristics => _model.OptionalHeader.DllCharacteristics;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfStackReserve_PE32"/>
        public ulong OH_SizeOfStackReserve => _model.OptionalHeader.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfStackReserve_PE32
            : _model.OptionalHeader.SizeOfStackReserve_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfStackCommit_PE32"/>
        public ulong OH_SizeOfStackCommit => _model.OptionalHeader.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfStackCommit_PE32
            : _model.OptionalHeader.SizeOfStackCommit_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapReserve_PE32"/>
        public ulong OH_SizeOfHeapReserve => _model.OptionalHeader.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfHeapReserve_PE32
            : _model.OptionalHeader.SizeOfHeapReserve_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapCommit_PE32"/>
        public ulong OH_SizeOfHeapCommit => _model.OptionalHeader.Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _model.OptionalHeader.SizeOfHeapCommit_PE32
            : _model.OptionalHeader.SizeOfHeapCommit_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.LoaderFlags"/>
        public uint OH_LoaderFlags => _model.OptionalHeader.LoaderFlags;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.NumberOfRvaAndSizes"/>
        public uint OH_NumberOfRvaAndSizes => _model.OptionalHeader.NumberOfRvaAndSizes;

        #endregion

        #region Data Directories

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ExportTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ExportTable => _model.OptionalHeader.ExportTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImportTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ImportTable => _model.OptionalHeader.ImportTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ResourceTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ResourceTable => _model.OptionalHeader.ResourceTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ExceptionTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ExceptionTable => _model.OptionalHeader.ExceptionTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CertificateTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_CertificateTable => _model.OptionalHeader.CertificateTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BaseRelocationTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_BaseRelocationTable => _model.OptionalHeader.BaseRelocationTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Debug"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_Debug => _model.OptionalHeader.Debug;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Architecture"/>
        public ulong OH_Architecture => _model.OptionalHeader.Architecture;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.GlobalPtr"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_GlobalPtr => _model.OptionalHeader.GlobalPtr;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ThreadLocalStorageTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ThreadLocalStorageTable => _model.OptionalHeader.ThreadLocalStorageTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.LoadConfigTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_LoadConfigTable => _model.OptionalHeader.LoadConfigTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.BoundImport"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_BoundImport => _model.OptionalHeader.BoundImport;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImportAddressTable"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_ImportAddressTable => _model.OptionalHeader.ImportAddressTable;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.DelayImportDescriptor"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_DelayImportDescriptor => _model.OptionalHeader.DelayImportDescriptor;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.CLRRuntimeHeader"/>
        public SabreTools.Models.PortableExecutable.DataDirectory OH_CLRRuntimeHeader => _model.OptionalHeader.CLRRuntimeHeader;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.Reserved"/>
        public ulong OH_Reserved => _model.OptionalHeader.Reserved;

        #endregion

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.PortableExecutable.SectionTable"/>
        public SabreTools.Models.PortableExecutable.SectionHeader[] SectionTable => _model.SectionTable;

        /// <inheritdoc cref="Models.PortableExecutable.COFFSymbolTable"/>
        public SabreTools.Models.PortableExecutable.COFFSymbolTableEntry[] COFFSymbolTable => _model.COFFSymbolTable;

        /// <inheritdoc cref="Models.PortableExecutable.COFFStringTable"/>
        public SabreTools.Models.PortableExecutable.COFFStringTable COFFStringTable => _model.COFFStringTable;

        /// <inheritdoc cref="Models.PortableExecutable.AttributeCertificateTable"/>
        public SabreTools.Models.PortableExecutable.AttributeCertificateTableEntry[] AttributeCertificateTable => _model.AttributeCertificateTable;

        /// <inheritdoc cref="Models.PortableExecutable.DelayLoadDirectoryTable"/>
        public SabreTools.Models.PortableExecutable.DelayLoadDirectoryTable DelayLoadDirectoryTable => _model.DelayLoadDirectoryTable;

        #endregion

        #region Sections

        /// <inheritdoc cref="Models.PortableExecutable.BaseRelocationTable"/>
        public SabreTools.Models.PortableExecutable.BaseRelocationBlock[] BaseRelocationTable => _model.BaseRelocationTable;

        /// <inheritdoc cref="Models.PortableExecutable.DebugTable"/>
        public SabreTools.Models.PortableExecutable.DebugTable DebugTable => _model.DebugTable;

        /// <inheritdoc cref="Models.PortableExecutable.ExportTable"/>
        public SabreTools.Models.PortableExecutable.ExportTable ExportTable => _model.ExportTable;

        /// <inheritdoc cref="Models.PortableExecutable.ExportTable.ExportNameTable"/>
        public string[] ExportNameTable => _model.ExportTable?.ExportNameTable?.Strings;

        /// <inheritdoc cref="Models.PortableExecutable.ImportTable"/>
        public SabreTools.Models.PortableExecutable.ImportTable ImportTable => _model.ImportTable;

        /// <inheritdoc cref="Models.PortableExecutable.ImportTable.HintNameTable"/>
        public string[] ImportHintNameTable => _model.ImportTable?.HintNameTable != null
            ? _model.ImportTable.HintNameTable.Select(entry => entry.Name).ToArray()
            : null;

        /// <inheritdoc cref="Models.PortableExecutable.ResourceDirectoryTable"/>
        public SabreTools.Models.PortableExecutable.ResourceDirectoryTable ResourceDirectoryTable => _model.ResourceDirectoryTable;

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Header padding data, if it exists
        /// </summary>
        public byte[] HeaderPaddingData
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_headerPaddingData != null)
                        return _headerPaddingData;

                    // TODO: Don't scan the known header data as well

                    // Populate the raw header padding data based on the source
                    uint headerStartAddress = Stub_NewExeHeaderAddr;
                    uint firstSectionAddress = SectionTable.Select(s => s.PointerToRawData).Where(s => s != 0).OrderBy(s => s).First();
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
        public List<string> HeaderPaddingStrings
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_headerPaddingStrings != null)
                        return _headerPaddingStrings;

                    // TODO: Don't scan the known header data as well

                    // Populate the raw header padding data based on the source
                    uint headerStartAddress = Stub_NewExeHeaderAddr;
                    uint firstSectionAddress = SectionTable.Select(s => s.PointerToRawData).Where(s => s != 0).OrderBy(s => s).First();
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
        public byte[] EntryPointData
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If we have no entry point
                    int entryPointAddress = (int)OH_AddressOfEntryPoint.ConvertVirtualAddress(SectionTable);
                    if (entryPointAddress == 0)
                        return null;

                    // If the entry point matches with the start of a section, use that
                    int entryPointSection = FindEntryPointSectionIndex();
                    if (entryPointSection >= 0 && OH_AddressOfEntryPoint == SectionTable[entryPointSection].VirtualAddress)
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

                    // If we have certificate data, use that as the end
                    if (OH_CertificateTable != null)
                    {
                        var certificateTable = _model.OptionalHeader.CertificateTable;
                        int certificateTableAddress = (int)certificateTable.VirtualAddress.ConvertVirtualAddress(_model.SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in _model.SectionTable)
                    {
                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(_model.SectionTable);
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
        public byte[] OverlayData
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

                    // If we have certificate data, use that as the end
                    if (OH_CertificateTable != null)
                    {
                        var certificateTable = _model.OptionalHeader.CertificateTable;
                        int certificateTableAddress = (int)certificateTable.VirtualAddress.ConvertVirtualAddress(_model.SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in _model.SectionTable)
                    {
                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(_model.SectionTable);
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
        public List<string> OverlayStrings
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

                    // If we have certificate data, use that as the end
                    if (OH_CertificateTable != null)
                    {
                        var certificateTable = _model.OptionalHeader.CertificateTable;
                        int certificateTableAddress = (int)certificateTable.VirtualAddress.ConvertVirtualAddress(_model.SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in _model.SectionTable)
                    {
                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(_model.SectionTable);
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
        public string[] SectionNames
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // Use the cached data if possible
                    if (_sectionNames != null)
                        return _sectionNames;

                    // Otherwise, build and return the cached array
                    _sectionNames = new string[_model.SectionTable.Length];
                    for (int i = 0; i < _sectionNames.Length; i++)
                    {
                        var section = _model.SectionTable[i];

                        // TODO: Handle long section names with leading `/`
                        byte[] sectionNameBytes = section.Name;
                        string sectionNameString = Encoding.UTF8.GetString(sectionNameBytes).TrimEnd('\0');
                        _sectionNames[i] = sectionNameString;
                    }

                    return _sectionNames;
                }
            }
        }

        /// <summary>
        /// Stub executable data, if it exists
        /// </summary>
        public byte[] StubExecutableData
        {
            get
            {
                lock (_sourceDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_stubExecutableData != null)
                        return _stubExecutableData;

                    // Populate the raw stub executable data based on the source
                    int endOfStubHeader = 0x40;
                    int lengthOfStubExecutableData = (int)_model.Stub.Header.NewExeHeaderAddr - endOfStubHeader;
                    _stubExecutableData = ReadFromDataSource(endOfStubHeader, lengthOfStubExecutableData);

                    // Cache and return the stub executable data, even if null
                    return _stubExecutableData;
                }
            }
        }

        /// <summary>
        /// Dictionary of debug data
        /// </summary>
        public Dictionary<int, object> DebugData
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
        public Dictionary<string, object> ResourceData
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
        /// </summary>
        public string BuildGuid => GetVersionInfoString("BuildGuid");

        /// <summary>
        /// "Build signature"
        /// </summary>
        public string BuildSignature => GetVersionInfoString("BuildSignature");

        /// <summary>
        /// Additional information that should be displayed for diagnostic purposes.
        /// </summary>
        public string Comments => GetVersionInfoString("Comments");

        /// <summary>
        /// Company that produced the file—for example, "Microsoft Corporation" or
        /// "Standard Microsystems Corporation, Inc." This string is required.
        /// </summary>
        public string CompanyName => GetVersionInfoString("CompanyName");

        /// <summary>
        /// "Debug version"
        /// </summary>
        public string DebugVersion => GetVersionInfoString("DebugVersion");

        /// <summary>
        /// File description to be presented to users. This string may be displayed in a
        /// list box when the user is choosing files to install—for example, "Keyboard
        /// Driver for AT-Style Keyboards". This string is required.
        /// </summary>
        public string FileDescription => GetVersionInfoString("FileDescription");

        /// <summary>
        /// Version number of the file—for example, "3.10" or "5.00.RC2". This string
        /// is required.
        /// </summary>
        public string FileVersion => GetVersionInfoString("FileVersion");

        /// <summary>
        /// Internal name of the file, if one exists—for example, a module name if the
        /// file is a dynamic-link library. If the file has no internal name, this
        /// string should be the original filename, without extension. This string is required.
        /// </summary>
        public string InternalName => GetVersionInfoString(key: "InternalName");

        /// <summary>
        /// Copyright notices that apply to the file. This should include the full text of
        /// all notices, legal symbols, copyright dates, and so on. This string is optional.
        /// </summary>
        public string LegalCopyright => GetVersionInfoString(key: "LegalCopyright");

        /// <summary>
        /// Trademarks and registered trademarks that apply to the file. This should include
        /// the full text of all notices, legal symbols, trademark numbers, and so on. This
        /// string is optional.
        /// </summary>
        public string LegalTrademarks => GetVersionInfoString(key: "LegalTrademarks");

        /// <summary>
        /// Original name of the file, not including a path. This information enables an
        /// application to determine whether a file has been renamed by a user. The format of
        /// the name depends on the file system for which the file was created. This string
        /// is required.
        /// </summary>
        public string OriginalFilename => GetVersionInfoString(key: "OriginalFilename");

        /// <summary>
        /// Information about a private version of the file—for example, "Built by TESTER1 on
        /// \TESTBED". This string should be present only if VS_FF_PRIVATEBUILD is specified in
        /// the fileflags parameter of the root block.
        /// </summary>
        public string PrivateBuild => GetVersionInfoString(key: "PrivateBuild");

        /// <summary>
        /// "Product GUID"
        /// </summary>
        public string ProductGuid => GetVersionInfoString("ProductGuid");

        /// <summary>
        /// Name of the product with which the file is distributed. This string is required.
        /// </summary>
        public string ProductName => GetVersionInfoString(key: "ProductName");

        /// <summary>
        /// Version of the product with which the file is distributed—for example, "3.10" or
        /// "5.00.RC2". This string is required.
        /// </summary>
        public string ProductVersion => GetVersionInfoString(key: "ProductVersion");

        /// <summary>
        /// Text that specifies how this version of the file differs from the standard
        /// version—for example, "Private build for TESTER1 solving mouse problems on M250 and
        /// M250E computers". This string should be present only if VS_FF_SPECIALBUILD is
        /// specified in the fileflags parameter of the root block.
        /// </summary>
        public string SpecialBuild => GetVersionInfoString(key: "SpecialBuild") ?? GetVersionInfoString(key: "Special Build");

        /// <summary>
        /// "Trade name"
        /// </summary>
        public string TradeName => GetVersionInfoString(key: "TradeName");

        /// <summary>
        /// Get the internal version as reported by the resources
        /// </summary>
        /// <returns>Version string, null on error</returns>
        /// <remarks>The internal version is either the file version, product version, or assembly version, in that order</remarks>
        public string GetInternalVersion()
        {
            string version = this.FileVersion;
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
        public string AssemblyDescription
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
        public string AssemblyVersion
        {
            get
            {
                var manifest = GetAssemblyManifest();
                return manifest?
                    .AssemblyIdentities?
                    .FirstOrDefault(ai => !string.IsNullOrWhiteSpace(ai.Version))?
                    .Version;
            }
        }

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Header padding data, if it exists
        /// </summary>
        private byte[] _headerPaddingData = null;

        /// <summary>
        /// Header padding strings, if they exist
        /// </summary>
        private List<string> _headerPaddingStrings = null;

        /// <summary>
        /// Entry point data, if it exists and isn't aligned to a section
        /// </summary>
        private byte[] _entryPointData = null;

        /// <summary>
        /// Address of the overlay, if it exists
        /// </summary>
        private int? _overlayAddress = null;

        /// <summary>
        /// Overlay data, if it exists
        /// </summary>
        private byte[] _overlayData = null;

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
        private List<string> _overlayStrings = null;

        /// <summary>
        /// Stub executable data, if it exists
        /// </summary>
        private byte[] _stubExecutableData = null;

        /// <summary>
        /// Sanitized section names
        /// </summary>
        private string[] _sectionNames = null;

        /// <summary>
        /// Cached raw section data
        /// </summary>
        private byte[][] _sectionData = null;

        /// <summary>
        /// Cached found string data in sections
        /// </summary>
        private List<string>[] _sectionStringData = null;

        /// <summary>
        /// Cached raw table data
        /// </summary>
        private byte[][] _tableData = null;

        /// <summary>
        /// Cached found string data in tables
        /// </summary>
        private List<string>[] _tableStringData = null;

        /// <summary>
        /// Cached debug data
        /// </summary>
        private readonly Dictionary<int, object> _debugData = new Dictionary<int, object>();

        /// <summary>
        /// Cached resource data
        /// </summary>
        private readonly Dictionary<string, object> _resourceData = new Dictionary<string, object>();

        /// <summary>
        /// Cached version info data
        /// </summary>
        private SabreTools.Models.PortableExecutable.VersionInfo _versionInfo = null;

        /// <summary>
        /// Cached assembly manifest data
        /// </summary>
        private SabreTools.Models.PortableExecutable.AssemblyManifest _assemblyManifest = null;

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
        public static PortableExecutable Create(byte[] data, int offset)
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
        public static PortableExecutable Create(Stream data)
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
        public string GetVersionInfoString(string key)
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
                .SelectMany(st => st.Children)
                .FirstOrDefault(sd => key.Equals(sd.Key, StringComparison.OrdinalIgnoreCase));

            // Return either the match or null
            return match?.Value?.TrimEnd('\0');
        }

        /// <summary>
        /// Get the assembly manifest, if possible
        /// </summary>
        /// <returns>Assembly manifest object, null on error</returns>
        private SabreTools.Models.PortableExecutable.AssemblyManifest GetAssemblyManifest()
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

            builder.AppendLine("Portable Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Stub
            PrintStubHeader(builder);
            PrintStubExtendedHeader(builder);

            // Header
            PrintCOFFFileHeader(builder);
            PrintOptionalHeader(builder);

            // Tables
            PrintSectionTable(builder);
            PrintCOFFSymbolTable(builder);
            PrintAttributeCertificateTable(builder);
            PrintDelayLoadDirectoryTable(builder);

            // Named Sections
            PrintBaseRelocationTable(builder);
            PrintDebugTable(builder);
            PrintExportTable(builder);
            PrintImportTable(builder);
            PrintResourceDirectoryTable(builder);

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
        /// Print COFF file header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintCOFFFileHeader(StringBuilder builder)
        {
            builder.AppendLine("  COFF File Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Machine: {Machine} (0x{Machine:X})");
            builder.AppendLine($"  Number of sections: {NumberOfSections} (0x{NumberOfSections:X})");
            builder.AppendLine($"  Time/Date stamp: {TimeDateStamp} (0x{TimeDateStamp:X})");
            builder.AppendLine($"  Pointer to symbol table: {PointerToSymbolTable} (0x{PointerToSymbolTable:X})");
            builder.AppendLine($"  Number of symbols: {NumberOfSymbols} (0x{NumberOfSymbols:X})");
            builder.AppendLine($"  Size of optional header: {SizeOfOptionalHeader} (0x{SizeOfOptionalHeader:X})");
            builder.AppendLine($"  Characteristics: {Characteristics} (0x{Characteristics:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print optional header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintOptionalHeader(StringBuilder builder)
        {
            builder.AppendLine("  Optional Header Information:");
            builder.AppendLine("  -------------------------");
            if (SizeOfOptionalHeader == 0)
            {
                builder.AppendLine("  No optional header present");
            }
            else
            {
                builder.AppendLine($"  Magic: {OH_Magic} (0x{OH_Magic:X})");
                builder.AppendLine($"  Major linker version: {OH_MajorLinkerVersion} (0x{OH_MajorLinkerVersion:X})");
                builder.AppendLine($"  Minor linker version: {OH_MinorLinkerVersion} (0x{OH_MinorLinkerVersion:X})");
                builder.AppendLine($"  Size of code section: {OH_SizeOfCode} (0x{OH_SizeOfCode:X})");
                builder.AppendLine($"  Size of initialized data: {OH_SizeOfInitializedData} (0x{OH_SizeOfInitializedData:X})");
                builder.AppendLine($"  Size of uninitialized data: {OH_SizeOfUninitializedData} (0x{OH_SizeOfUninitializedData:X})");
                builder.AppendLine($"  Address of entry point: {OH_AddressOfEntryPoint} (0x{OH_AddressOfEntryPoint:X})");
                builder.AppendLine($"  Base of code: {OH_BaseOfCode} (0x{OH_BaseOfCode:X})");
                if (OH_Magic == SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    builder.AppendLine($"  Base of data: {OH_BaseOfData} (0x{OH_BaseOfData:X})");

                builder.AppendLine($"  Image base: {OH_ImageBase} (0x{OH_ImageBase:X})");
                builder.AppendLine($"  Section alignment: {OH_SectionAlignment} (0x{OH_SectionAlignment:X})");
                builder.AppendLine($"  File alignment: {OH_FileAlignment} (0x{OH_FileAlignment:X})");
                builder.AppendLine($"  Major operating system version: {OH_MajorOperatingSystemVersion} (0x{OH_MajorOperatingSystemVersion:X})");
                builder.AppendLine($"  Minor operating system version: {OH_MinorOperatingSystemVersion} (0x{OH_MinorOperatingSystemVersion:X})");
                builder.AppendLine($"  Major image version: {OH_MajorImageVersion} (0x{OH_MajorImageVersion:X})");
                builder.AppendLine($"  Minor image version: {OH_MinorImageVersion} (0x{OH_MinorImageVersion:X})");
                builder.AppendLine($"  Major subsystem version: {OH_MajorSubsystemVersion} (0x{OH_MajorSubsystemVersion:X})");
                builder.AppendLine($"  Minor subsystem version: {OH_MinorSubsystemVersion} (0x{OH_MinorSubsystemVersion:X})");
                builder.AppendLine($"  Win32 version value: {OH_Win32VersionValue} (0x{OH_Win32VersionValue:X})");
                builder.AppendLine($"  Size of image: {OH_SizeOfImage} (0x{OH_SizeOfImage:X})");
                builder.AppendLine($"  Size of headers: {OH_SizeOfHeaders} (0x{OH_SizeOfHeaders:X})");
                builder.AppendLine($"  Checksum: {OH_CheckSum} (0x{OH_CheckSum:X})");
                builder.AppendLine($"  Subsystem: {OH_Subsystem} (0x{OH_Subsystem:X})");
                builder.AppendLine($"  DLL characteristics: {OH_DllCharacteristics} (0x{OH_DllCharacteristics:X})");
                builder.AppendLine($"  Size of stack reserve: {OH_SizeOfStackReserve} (0x{OH_SizeOfStackReserve:X})");
                builder.AppendLine($"  Size of stack commit: {OH_SizeOfStackCommit} (0x{OH_SizeOfStackCommit:X})");
                builder.AppendLine($"  Size of heap reserve: {OH_SizeOfHeapReserve} (0x{OH_SizeOfHeapReserve:X})");
                builder.AppendLine($"  Size of heap commit: {OH_SizeOfHeapCommit} (0x{OH_SizeOfHeapCommit:X})");
                builder.AppendLine($"  Loader flags: {OH_LoaderFlags} (0x{OH_LoaderFlags:X})");
                builder.AppendLine($"  Number of data-directory entries: {OH_NumberOfRvaAndSizes} (0x{OH_NumberOfRvaAndSizes:X})");

                if (OH_ExportTable != null)
                {
                    builder.AppendLine("    Export Table (1)");
                    builder.AppendLine($"      Virtual address: {OH_ExportTable.VirtualAddress} (0x{OH_ExportTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_ExportTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_ExportTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_ExportTable.Size} (0x{OH_ExportTable.Size:X})");
                }
                if (OH_ImportTable != null)
                {
                    builder.AppendLine("    Import Table (2)");
                    builder.AppendLine($"      Virtual address: {OH_ImportTable.VirtualAddress} (0x{OH_ImportTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_ImportTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_ImportTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_ImportTable.Size} (0x{OH_ImportTable.Size:X})");
                }
                if (OH_ResourceTable != null)
                {
                    builder.AppendLine("    Resource Table (3)");
                    builder.AppendLine($"      Virtual address: {OH_ResourceTable.VirtualAddress} (0x{OH_ResourceTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_ResourceTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_ResourceTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_ResourceTable.Size} (0x{OH_ResourceTable.Size:X})");
                }
                if (OH_ExceptionTable != null)
                {
                    builder.AppendLine("    Exception Table (4)");
                    builder.AppendLine($"      Virtual address: {OH_ExceptionTable.VirtualAddress} (0x{OH_ExceptionTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_ExceptionTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_ExceptionTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_ExceptionTable.Size} (0x{OH_ExceptionTable.Size:X})");
                }
                if (OH_CertificateTable != null)
                {
                    builder.AppendLine("    Certificate Table (5)");
                    builder.AppendLine($"      Virtual address: {OH_CertificateTable.VirtualAddress} (0x{OH_CertificateTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_CertificateTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_CertificateTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_CertificateTable.Size} (0x{OH_CertificateTable.Size:X})");
                }
                if (OH_BaseRelocationTable != null)
                {
                    builder.AppendLine("    Base Relocation Table (6)");
                    builder.AppendLine($"      Virtual address: {OH_BaseRelocationTable.VirtualAddress} (0x{OH_BaseRelocationTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_BaseRelocationTable.Size} (0x{OH_BaseRelocationTable.Size:X})");
                }
                if (OH_Debug != null)
                {
                    builder.AppendLine("    Debug Table (7)");
                    builder.AppendLine($"      Virtual address: {OH_Debug.VirtualAddress} (0x{OH_Debug.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_Debug.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_Debug.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_Debug.Size} (0x{OH_Debug.Size:X})");
                }
                if (OH_NumberOfRvaAndSizes >= 8)
                {
                    builder.AppendLine("    Architecture Table (8)");
                    builder.AppendLine($"      Virtual address: 0 (0x00000000)");
                    builder.AppendLine($"      Physical address: 0 (0x00000000)");
                    builder.AppendLine($"      Size: 0 (0x00000000)");
                }
                if (OH_GlobalPtr != null)
                {
                    builder.AppendLine("    Global Pointer Register (9)");
                    builder.AppendLine($"      Virtual address: {OH_GlobalPtr.VirtualAddress} (0x{OH_GlobalPtr.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_GlobalPtr.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_GlobalPtr.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_GlobalPtr.Size} (0x{OH_GlobalPtr.Size:X})");
                }
                if (OH_ThreadLocalStorageTable != null)
                {
                    builder.AppendLine("    Thread Local Storage (TLS) Table (10)");
                    builder.AppendLine($"      Virtual address: {OH_ThreadLocalStorageTable.VirtualAddress} (0x{OH_ThreadLocalStorageTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_ThreadLocalStorageTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_ThreadLocalStorageTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_ThreadLocalStorageTable.Size} (0x{OH_ThreadLocalStorageTable.Size:X})");
                }
                if (OH_LoadConfigTable != null)
                {
                    builder.AppendLine("    Load Config Table (11)");
                    builder.AppendLine($"      Virtual address: {OH_LoadConfigTable.VirtualAddress} (0x{OH_LoadConfigTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_LoadConfigTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_LoadConfigTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_LoadConfigTable.Size} (0x{OH_LoadConfigTable.Size:X})");
                }
                if (OH_BoundImport != null)
                {
                    builder.AppendLine("    Bound Import Table (12)");
                    builder.AppendLine($"      Virtual address: {OH_BoundImport.VirtualAddress} (0x{OH_BoundImport.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_BoundImport.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_BoundImport.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_BoundImport.Size} (0x{OH_BoundImport.Size:X})");
                }
                if (OH_ImportAddressTable != null)
                {
                    builder.AppendLine("    Import Address Table (13)");
                    builder.AppendLine($"      Virtual address: {OH_ImportAddressTable.VirtualAddress} (0x{OH_ImportAddressTable.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_ImportAddressTable.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_ImportAddressTable.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_ImportAddressTable.Size} (0x{OH_ImportAddressTable.Size:X})");
                }
                if (OH_DelayImportDescriptor != null)
                {
                    builder.AppendLine("    Delay Import Descriptior (14)");
                    builder.AppendLine($"      Virtual address: {OH_DelayImportDescriptor.VirtualAddress} (0x{OH_DelayImportDescriptor.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_DelayImportDescriptor.Size} (0x{OH_DelayImportDescriptor.Size:X})");
                }
                if (OH_CLRRuntimeHeader != null)
                {
                    builder.AppendLine("    CLR Runtime Header (15)");
                    builder.AppendLine($"      Virtual address: {OH_CLRRuntimeHeader.VirtualAddress} (0x{OH_CLRRuntimeHeader.VirtualAddress:X})");
                    builder.AppendLine($"      Physical address: {OH_CLRRuntimeHeader.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{OH_CLRRuntimeHeader.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"      Size: {OH_CLRRuntimeHeader.Size} (0x{OH_CLRRuntimeHeader.Size:X})");
                }
                if (OH_NumberOfRvaAndSizes >= 16)
                {
                    builder.AppendLine("    Reserved (16)");
                    builder.AppendLine($"      Virtual address: 0 (0x00000000)");
                    builder.AppendLine($"      Physical address: 0 (0x00000000)");
                    builder.AppendLine($"      Size: 0 (0x00000000)");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print section table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintSectionTable(StringBuilder builder)
        {
            builder.AppendLine("  Section Table Information:");
            builder.AppendLine("  -------------------------");
            if (NumberOfSections == 0 || SectionTable.Length == 0)
            {
                builder.AppendLine("  No section table items");
            }
            else
            {
                for (int i = 0; i < SectionTable.Length; i++)
                {
                    var entry = SectionTable[i];
                    builder.AppendLine($"  Section Table Entry {i}");
                    builder.AppendLine($"    Name: {Encoding.UTF8.GetString(entry.Name).TrimEnd('\0')}");
                    builder.AppendLine($"    Virtual size: {entry.VirtualSize} (0x{entry.VirtualSize:X})");
                    builder.AppendLine($"    Virtual address: {entry.VirtualAddress} (0x{entry.VirtualAddress:X})");
                    builder.AppendLine($"    Physical address: {entry.VirtualAddress.ConvertVirtualAddress(SectionTable)} (0x{entry.VirtualAddress.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"    Size of raw data: {entry.SizeOfRawData} (0x{entry.SizeOfRawData:X})");
                    builder.AppendLine($"    Pointer to raw data: {entry.PointerToRawData} (0x{entry.PointerToRawData:X})");
                    builder.AppendLine($"    Pointer to relocations: {entry.PointerToRelocations} (0x{entry.PointerToRelocations:X})");
                    builder.AppendLine($"    Pointer to linenumbers: {entry.PointerToLinenumbers} (0x{entry.PointerToLinenumbers:X})");
                    builder.AppendLine($"    Number of relocations: {entry.NumberOfRelocations} (0x{entry.NumberOfRelocations:X})");
                    builder.AppendLine($"    Number of linenumbers: {entry.NumberOfLinenumbers} (0x{entry.NumberOfLinenumbers:X})");
                    builder.AppendLine($"    Characteristics: {entry.Characteristics} (0x{entry.Characteristics:X})");
                    // TODO: Add COFFRelocations
                    // TODO: Add COFFLineNumbers
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print COFF symbol table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintCOFFSymbolTable(StringBuilder builder)
        {
            builder.AppendLine("  COFF Symbol Table Information:");
            builder.AppendLine("  -------------------------");
            if (PointerToSymbolTable == 0
                || NumberOfSymbols == 0
                || COFFSymbolTable == null
                || COFFSymbolTable.Length == 0)
            {
                builder.AppendLine("  No COFF symbol table items");
            }
            else
            {
                int auxSymbolsRemaining = 0;
                int currentSymbolType = 0;

                for (int i = 0; i < COFFSymbolTable.Length; i++)
                {
                    var entry = COFFSymbolTable[i];
                    builder.AppendLine($"  COFF Symbol Table Entry {i} (Subtype {currentSymbolType})");
                    if (currentSymbolType == 0)
                    {
                        if (entry.ShortName != null)
                        {
                            builder.AppendLine($"    Short name: {Encoding.UTF8.GetString(entry.ShortName).TrimEnd('\0')}");
                        }
                        else
                        {
                            builder.AppendLine($"    Zeroes: {entry.Zeroes} (0x{entry.Zeroes:X})");
                            builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                        }
                        builder.AppendLine($"    Value: {entry.Value} (0x{entry.Value:X})");
                        builder.AppendLine($"    Section number: {entry.SectionNumber} (0x{entry.SectionNumber:X})");
                        builder.AppendLine($"    Symbol type: {entry.SymbolType} (0x{entry.SymbolType:X})");
                        builder.AppendLine($"    Storage class: {entry.StorageClass} (0x{entry.StorageClass:X})");
                        builder.AppendLine($"    Number of aux symbols: {entry.NumberOfAuxSymbols} (0x{entry.NumberOfAuxSymbols:X})");

                        auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                        if (auxSymbolsRemaining == 0)
                            continue;

                        if (entry.StorageClass == SabreTools.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SymbolType == SabreTools.Models.PortableExecutable.SymbolType.IMAGE_SYM_TYPE_FUNC
                        && entry.SectionNumber > 0)
                        {
                            currentSymbolType = 1;
                        }
                        else if (entry.StorageClass == SabreTools.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_FUNCTION
                            && entry.ShortName != null
                            && ((entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x62 && entry.ShortName[2] == 0x66)  // .bf
                                || (entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x65 && entry.ShortName[2] == 0x66))) // .ef
                        {
                            currentSymbolType = 2;
                        }
                        else if (entry.StorageClass == SabreTools.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                            && entry.SectionNumber == (ushort)SabreTools.Models.PortableExecutable.SectionNumber.IMAGE_SYM_UNDEFINED
                            && entry.Value == 0)
                        {
                            currentSymbolType = 3;
                        }
                        else if (entry.StorageClass == SabreTools.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_FILE)
                        {
                            // TODO: Symbol name should be ".file"
                            currentSymbolType = 4;
                        }
                        else if (entry.StorageClass == SabreTools.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_STATIC)
                        {
                            // TODO: Should have the name of a section (like ".text")
                            currentSymbolType = 5;
                        }
                        else if (entry.StorageClass == SabreTools.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
                        {
                            currentSymbolType = 6;
                        }
                    }
                    else if (currentSymbolType == 1)
                    {
                        builder.AppendLine($"    Tag index: {entry.AuxFormat1TagIndex} (0x{entry.AuxFormat1TagIndex:X})");
                        builder.AppendLine($"    Total size: {entry.AuxFormat1TotalSize} (0x{entry.AuxFormat1TotalSize:X})");
                        builder.AppendLine($"    Pointer to linenumber: {entry.AuxFormat1PointerToLinenumber} (0x{entry.AuxFormat1PointerToLinenumber:X})");
                        builder.AppendLine($"    Pointer to next function: {entry.AuxFormat1PointerToNextFunction} (0x{entry.AuxFormat1PointerToNextFunction:X})");
                        builder.AppendLine($"    Unused: {entry.AuxFormat1Unused} (0x{entry.AuxFormat1Unused:X})");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 2)
                    {
                        builder.AppendLine($"    Unused: {entry.AuxFormat2Unused1} (0x{entry.AuxFormat2Unused1:X})");
                        builder.AppendLine($"    Linenumber: {entry.AuxFormat2Linenumber} (0x{entry.AuxFormat2Linenumber:X})");
                        builder.AppendLine($"    Unused: {entry.AuxFormat2Unused2} (0x{entry.AuxFormat2Unused2:X})");
                        builder.AppendLine($"    Pointer to next function: {entry.AuxFormat2PointerToNextFunction} (0x{entry.AuxFormat2PointerToNextFunction:X})");
                        builder.AppendLine($"    Unused: {entry.AuxFormat2Unused3} (0x{entry.AuxFormat2Unused3:X})");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 3)
                    {
                        builder.AppendLine($"    Tag index: {entry.AuxFormat3TagIndex} (0x{entry.AuxFormat3TagIndex:X})");
                        builder.AppendLine($"    Characteristics: {entry.AuxFormat3Characteristics} (0x{entry.AuxFormat3Characteristics:X})");
                        builder.AppendLine($"    Unused: {BitConverter.ToString(entry.AuxFormat3Unused).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 4)
                    {
                        builder.AppendLine($"    File name: {Encoding.ASCII.GetString(entry.AuxFormat4FileName).TrimEnd('\0')}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 5)
                    {
                        builder.AppendLine($"    Length: {entry.AuxFormat5Length} (0x{entry.AuxFormat5Length:X})");
                        builder.AppendLine($"    Number of relocations: {entry.AuxFormat5NumberOfRelocations} (0x{entry.AuxFormat5NumberOfRelocations:X})");
                        builder.AppendLine($"    Number of linenumbers: {entry.AuxFormat5NumberOfLinenumbers} (0x{entry.AuxFormat5NumberOfLinenumbers:X})");
                        builder.AppendLine($"    Checksum: {entry.AuxFormat5CheckSum} (0x{entry.AuxFormat5CheckSum:X})");
                        builder.AppendLine($"    Number: {entry.AuxFormat5Number} (0x{entry.AuxFormat5Number:X})");
                        builder.AppendLine($"    Selection: {entry.AuxFormat5Selection} (0x{entry.AuxFormat5Selection:X})");
                        builder.AppendLine($"    Unused: {BitConverter.ToString(entry.AuxFormat5Unused).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 6)
                    {
                        builder.AppendLine($"    Aux type: {entry.AuxFormat6AuxType} (0x{entry.AuxFormat6AuxType:X})");
                        builder.AppendLine($"    Reserved: {entry.AuxFormat6Reserved1} (0x{entry.AuxFormat6Reserved1:X})");
                        builder.AppendLine($"    Symbol table index: {entry.AuxFormat6SymbolTableIndex} (0x{entry.AuxFormat6SymbolTableIndex:X})");
                        builder.AppendLine($"    Reserved: {BitConverter.ToString(entry.AuxFormat6Reserved2).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }

                    // If we hit the last aux symbol, go back to normal format
                    if (auxSymbolsRemaining == 0)
                        currentSymbolType = 0;
                }

                builder.AppendLine();
                builder.AppendLine("  COFF String Table Information:");
                builder.AppendLine("  -------------------------");
                if (COFFStringTable == null
                    || COFFStringTable.Strings == null
                    || COFFStringTable.Strings.Length == 0)
                {
                    builder.AppendLine("  No COFF string table items");
                }
                else
                {
                    builder.AppendLine($"  Total size: {COFFStringTable.TotalSize} (0x{COFFStringTable.TotalSize:X})");
                    for (int i = 0; i < COFFStringTable.Strings.Length; i++)
                    {
                        string entry = COFFStringTable.Strings[i];
                        builder.AppendLine($"  COFF String Table Entry {i})");
                        builder.AppendLine($"    Value: {entry}");
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print attribute certificate table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintAttributeCertificateTable(StringBuilder builder)
        {
            builder.AppendLine("  Attribute Certificate Table Information:");
            builder.AppendLine("  -------------------------");
            if (OH_CertificateTable == null
                || OH_CertificateTable.VirtualAddress == 0
                || AttributeCertificateTable.Length == 0)
            {
                builder.AppendLine("  No attribute certificate table items");
            }
            else
            {
                for (int i = 0; i < AttributeCertificateTable.Length; i++)
                {
                    var entry = AttributeCertificateTable[i];
                    builder.AppendLine($"  Attribute Certificate Table Entry {i}");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Revision: {entry.Revision} (0x{entry.Revision:X})");
                    builder.AppendLine($"    Certificate type: {entry.CertificateType} (0x{entry.CertificateType:X})");
                    builder.AppendLine();
                    if (entry.CertificateType == SabreTools.Models.PortableExecutable.WindowsCertificateType.WIN_CERT_TYPE_PKCS_SIGNED_DATA)
                    {
                        builder.AppendLine("    Certificate Data [Formatted]");
                        builder.AppendLine("    -------------------------");
                        var topLevelValues = AbstractSyntaxNotationOne.Parse(entry.Certificate, pointer: 0);
                        if (topLevelValues == null)
                        {
                            builder.AppendLine("    INVALID DATA FOUND");
                            builder.AppendLine($"    {BitConverter.ToString(entry.Certificate).Replace("-", string.Empty)}");
                        }
                        else
                        {
                            foreach (TypeLengthValue tlv in topLevelValues)
                            {
                                string tlvString = tlv.Format(paddingLevel: 4);
                                builder.AppendLine(tlvString);
                            }
                        }
                    }
                    else
                    {
                        builder.AppendLine($"    Certificate Data [Binary]");
                        builder.AppendLine("  -------------------------");
                        try
                        {
                            builder.AppendLine($"    {BitConverter.ToString(entry.Certificate).Replace("-", string.Empty)}");
                        }
                        catch
                        {
                            builder.AppendLine($"    [DATA TOO LARGE TO FORMAT]");
                        }
                    }

                    builder.AppendLine();
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print delay-load directory table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDelayLoadDirectoryTable(StringBuilder builder)
        {
            builder.AppendLine("  Delay-Load Directory Table Information:");
            builder.AppendLine("  -------------------------");
            if (OH_DelayImportDescriptor == null
                || OH_DelayImportDescriptor.VirtualAddress == 0
                || DelayLoadDirectoryTable == null)
            {
                builder.AppendLine("  No delay-load directory table items");
            }
            else
            {
                builder.AppendLine($"  Attributes: {DelayLoadDirectoryTable.Attributes} (0x{DelayLoadDirectoryTable.Attributes:X})");
                builder.AppendLine($"  Name RVA: {DelayLoadDirectoryTable.Name} (0x{DelayLoadDirectoryTable.Name:X})");
                builder.AppendLine($"  Module handle: {DelayLoadDirectoryTable.ModuleHandle} (0x{DelayLoadDirectoryTable.ModuleHandle:X})");
                builder.AppendLine($"  Delay import address table RVA: {DelayLoadDirectoryTable.DelayImportAddressTable} (0x{DelayLoadDirectoryTable.DelayImportAddressTable:X})");
                builder.AppendLine($"  Delay import name table RVA: {DelayLoadDirectoryTable.DelayImportNameTable} (0x{DelayLoadDirectoryTable.DelayImportNameTable:X})");
                builder.AppendLine($"  Bound delay import table RVA: {DelayLoadDirectoryTable.BoundDelayImportTable} (0x{DelayLoadDirectoryTable.BoundDelayImportTable:X})");
                builder.AppendLine($"  Unload delay import table RVA: {DelayLoadDirectoryTable.UnloadDelayImportTable} (0x{DelayLoadDirectoryTable.UnloadDelayImportTable:X})");
                builder.AppendLine($"  Timestamp: {DelayLoadDirectoryTable.TimeStamp} (0x{DelayLoadDirectoryTable.TimeStamp:X})");
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print base relocation table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintBaseRelocationTable(StringBuilder builder)
        {
            builder.AppendLine("  Base Relocation Table Information:");
            builder.AppendLine("  -------------------------");
            if (OH_BaseRelocationTable == null
                || OH_BaseRelocationTable.VirtualAddress == 0
                || BaseRelocationTable == null)
            {
                builder.AppendLine("  No base relocation table items");
            }
            else
            {
                for (int i = 0; i < BaseRelocationTable.Length; i++)
                {
                    var baseRelocationTableEntry = BaseRelocationTable[i];
                    builder.AppendLine($"  Base Relocation Table Entry {i}");
                    builder.AppendLine($"    Page RVA: {baseRelocationTableEntry.PageRVA} (0x{baseRelocationTableEntry.PageRVA:X})");
                    builder.AppendLine($"    Page physical address: {baseRelocationTableEntry.PageRVA.ConvertVirtualAddress(SectionTable)} (0x{baseRelocationTableEntry.PageRVA.ConvertVirtualAddress(SectionTable):X})");
                    builder.AppendLine($"    Block size: {baseRelocationTableEntry.BlockSize} (0x{baseRelocationTableEntry.BlockSize:X})");

                    builder.AppendLine($"    Base Relocation Table {i} Type and Offset Information:");
                    builder.AppendLine("    -------------------------");
                    if (baseRelocationTableEntry.TypeOffsetFieldEntries == null || baseRelocationTableEntry.TypeOffsetFieldEntries.Length == 0)
                    {
                        builder.AppendLine("    No base relocation table type and offset entries");
                    }
                    else
                    {
                        for (int j = 0; j < baseRelocationTableEntry.TypeOffsetFieldEntries.Length; j++)
                        {
                            var typeOffsetFieldEntry = baseRelocationTableEntry.TypeOffsetFieldEntries[j];
                            builder.AppendLine($"    Type and Offset Entry {j}");
                            builder.AppendLine($"      Type: {typeOffsetFieldEntry.BaseRelocationType} (0x{typeOffsetFieldEntry.BaseRelocationType:X})");
                            builder.AppendLine($"      Offset: {typeOffsetFieldEntry.Offset} (0x{typeOffsetFieldEntry.Offset:X})");
                        }
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print debug table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDebugTable(StringBuilder builder)
        {
            builder.AppendLine("  Debug Table Information:");
            builder.AppendLine("  -------------------------");
            if (OH_Debug == null
                || OH_Debug.VirtualAddress == 0
                || DebugTable == null)
            {
                builder.AppendLine("  No debug table items");
            }
            else
            {
                // TODO: If more sections added, model this after the Export Table
                for (int i = 0; i < DebugTable.DebugDirectoryTable.Length; i++)
                {
                    var debugDirectoryEntry = DebugTable.DebugDirectoryTable[i];
                    builder.AppendLine($"  Debug Directory Table Entry {i}");
                    builder.AppendLine($"    Characteristics: {debugDirectoryEntry.Characteristics} (0x{debugDirectoryEntry.Characteristics:X})");
                    builder.AppendLine($"    Time/Date stamp: {debugDirectoryEntry.TimeDateStamp} (0x{debugDirectoryEntry.TimeDateStamp:X})");
                    builder.AppendLine($"    Major version: {debugDirectoryEntry.MajorVersion} (0x{debugDirectoryEntry.MajorVersion:X})");
                    builder.AppendLine($"    Minor version: {debugDirectoryEntry.MinorVersion} (0x{debugDirectoryEntry.MinorVersion:X})");
                    builder.AppendLine($"    Debug type: {debugDirectoryEntry.DebugType} (0x{debugDirectoryEntry.DebugType:X})");
                    builder.AppendLine($"    Size of data: {debugDirectoryEntry.SizeOfData} (0x{debugDirectoryEntry.SizeOfData:X})");
                    builder.AppendLine($"    Address of raw data: {debugDirectoryEntry.AddressOfRawData} (0x{debugDirectoryEntry.AddressOfRawData:X})");
                    builder.AppendLine($"    Pointer to raw data: {debugDirectoryEntry.PointerToRawData} (0x{debugDirectoryEntry.PointerToRawData:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print export table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintExportTable(StringBuilder builder)
        {
            builder.AppendLine("  Export Table Information:");
            builder.AppendLine("  -------------------------");
            if (OH_ExportTable == null
                || OH_ExportTable.VirtualAddress == 0
                || ExportTable == null)
            {
                builder.AppendLine("  No export table items");
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine("    Export Directory Table Information:");
                builder.AppendLine("    -------------------------");
                builder.AppendLine($"    Export flags: {ExportTable.ExportDirectoryTable.ExportFlags} (0x{ExportTable.ExportDirectoryTable.ExportFlags:X})");
                builder.AppendLine($"    Time/Date stamp: {ExportTable.ExportDirectoryTable.TimeDateStamp} (0x{ExportTable.ExportDirectoryTable.TimeDateStamp:X})");
                builder.AppendLine($"    Major version: {ExportTable.ExportDirectoryTable.MajorVersion} (0x{ExportTable.ExportDirectoryTable.MajorVersion:X})");
                builder.AppendLine($"    Minor version: {ExportTable.ExportDirectoryTable.MinorVersion} (0x{ExportTable.ExportDirectoryTable.MinorVersion:X})");
                builder.AppendLine($"    Name RVA: {ExportTable.ExportDirectoryTable.NameRVA} (0x{ExportTable.ExportDirectoryTable.NameRVA:X})");
                builder.AppendLine($"    Name: {ExportTable.ExportDirectoryTable.Name}");
                builder.AppendLine($"    Ordinal base: {ExportTable.ExportDirectoryTable.OrdinalBase} (0x{ExportTable.ExportDirectoryTable.OrdinalBase:X})");
                builder.AppendLine($"    Address table entries: {ExportTable.ExportDirectoryTable.AddressTableEntries} (0x{ExportTable.ExportDirectoryTable.AddressTableEntries:X})");
                builder.AppendLine($"    Number of name pointers: {ExportTable.ExportDirectoryTable.NumberOfNamePointers} (0x{ExportTable.ExportDirectoryTable.NumberOfNamePointers:X})");
                builder.AppendLine($"    Export address table RVA: {ExportTable.ExportDirectoryTable.ExportAddressTableRVA} (0x{ExportTable.ExportDirectoryTable.ExportAddressTableRVA:X})");
                builder.AppendLine($"    Name pointer table RVA: {ExportTable.ExportDirectoryTable.NamePointerRVA} (0x{ExportTable.ExportDirectoryTable.NamePointerRVA:X})");
                builder.AppendLine($"    Ordinal table RVA: {ExportTable.ExportDirectoryTable.OrdinalTableRVA} (0x{ExportTable.ExportDirectoryTable.OrdinalTableRVA:X})");
                builder.AppendLine();

                builder.AppendLine("    Export Address Table Information:");
                builder.AppendLine("    -------------------------");
                if (ExportTable.ExportAddressTable == null || ExportTable.ExportAddressTable.Length == 0)
                {
                    builder.AppendLine("    No export address table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.ExportAddressTable.Length; i++)
                    {
                        var exportAddressTableEntry = ExportTable.ExportAddressTable[i];
                        builder.AppendLine($"    Export Address Table Entry {i}");
                        builder.AppendLine($"      Export RVA / Forwarder RVA: {exportAddressTableEntry.ExportRVA} (0x{exportAddressTableEntry.ExportRVA:X})");
                    }
                }
                builder.AppendLine();

                builder.AppendLine("    Name Pointer Table Information:");
                builder.AppendLine("    -------------------------");
                if (ExportTable.NamePointerTable?.Pointers == null || ExportTable.NamePointerTable.Pointers.Length == 0)
                {
                    builder.AppendLine("    No name pointer table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.NamePointerTable.Pointers.Length; i++)
                    {
                        var namePointerTableEntry = ExportTable.NamePointerTable.Pointers[i];
                        builder.AppendLine($"    Name Pointer Table Entry {i}");
                        builder.AppendLine($"      Pointer: {namePointerTableEntry} (0x{namePointerTableEntry:X})");
                    }
                }
                builder.AppendLine();

                builder.AppendLine("    Ordinal Table Information:");
                builder.AppendLine("    -------------------------");
                if (ExportTable.OrdinalTable?.Indexes == null || ExportTable.OrdinalTable.Indexes.Length == 0)
                {
                    builder.AppendLine("    No ordinal table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.OrdinalTable.Indexes.Length; i++)
                    {
                        var ordinalTableEntry = ExportTable.OrdinalTable.Indexes[i];
                        builder.AppendLine($"    Ordinal Table Entry {i}");
                        builder.AppendLine($"      Index: {ordinalTableEntry} (0x{ordinalTableEntry:X})");
                    }
                }
                builder.AppendLine();

                builder.AppendLine("    Export Name Table Information:");
                builder.AppendLine("    -------------------------");
                if (ExportTable.ExportNameTable?.Strings == null || ExportTable.ExportNameTable.Strings.Length == 0)
                {
                    builder.AppendLine("    No export name table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.ExportNameTable.Strings.Length; i++)
                    {
                        var exportNameTableEntry = ExportTable.ExportNameTable.Strings[i];
                        builder.AppendLine($"    Export Name Table Entry {i}");
                        builder.AppendLine($"      String: {exportNameTableEntry}");
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print import table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintImportTable(StringBuilder builder)
        {
            builder.AppendLine("  Import Table Information:");
            builder.AppendLine("  -------------------------");
            if (OH_ImportTable == null
                || OH_ImportTable.VirtualAddress == 0
                || ImportTable == null)
            {
                builder.AppendLine("  No import table items");
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine("    Import Directory Table Information:");
                builder.AppendLine("    -------------------------");
                if (ImportTable.ImportDirectoryTable == null || ImportTable.ImportDirectoryTable.Length == 0)
                {
                    builder.AppendLine("    No import directory table items");
                }
                else
                {
                    for (int i = 0; i < ImportTable.ImportDirectoryTable.Length; i++)
                    {
                        var importDirectoryTableEntry = ImportTable.ImportDirectoryTable[i];
                        builder.AppendLine($"    Import Directory Table Entry {i}");
                        builder.AppendLine($"      Import lookup table RVA: {importDirectoryTableEntry.ImportLookupTableRVA} (0x{importDirectoryTableEntry.ImportLookupTableRVA:X})");
                        builder.AppendLine($"      Import lookup table Physical Address: {importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(SectionTable)} (0x{importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(SectionTable):X})");
                        builder.AppendLine($"      Time/Date stamp: {importDirectoryTableEntry.TimeDateStamp} (0x{importDirectoryTableEntry.TimeDateStamp:X})");
                        builder.AppendLine($"      Forwarder chain: {importDirectoryTableEntry.ForwarderChain} (0x{importDirectoryTableEntry.ForwarderChain:X})");
                        builder.AppendLine($"      Name RVA: {importDirectoryTableEntry.NameRVA} (0x{importDirectoryTableEntry.NameRVA:X})");
                        builder.AppendLine($"      Name: {importDirectoryTableEntry.Name}");
                        builder.AppendLine($"      Import address table RVA: {importDirectoryTableEntry.ImportAddressTableRVA} (0x{importDirectoryTableEntry.ImportAddressTableRVA:X})");
                        builder.AppendLine($"      Import address table Physical Address: {importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(SectionTable)} (0x{importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(SectionTable):X})");
                    }
                }
                builder.AppendLine();

                builder.AppendLine("    Import Lookup Tables Information:");
                builder.AppendLine("    -------------------------");
                if (ImportTable.ImportLookupTables == null || ImportTable.ImportLookupTables.Count == 0)
                {
                    builder.AppendLine("    No import lookup tables");
                }
                else
                {
                    foreach (var kvp in ImportTable.ImportLookupTables)
                    {
                        int index = kvp.Key;
                        var importLookupTable = kvp.Value;

                        builder.AppendLine();
                        builder.AppendLine($"      Import Lookup Table {index} Information:");
                        builder.AppendLine("      -------------------------");
                        if (importLookupTable == null || importLookupTable.Length == 0)
                        {
                            builder.AppendLine("      No import lookup table items");
                        }
                        else
                        {
                            for (int i = 0; i < importLookupTable.Length; i++)
                            {
                                var importLookupTableEntry = importLookupTable[i];
                                builder.AppendLine($"      Import Lookup Table {index} Entry {i}");
                                builder.AppendLine($"        Ordinal/Name flag: {importLookupTableEntry.OrdinalNameFlag} (0x{importLookupTableEntry.OrdinalNameFlag:X})");
                                if (importLookupTableEntry.OrdinalNameFlag)
                                {
                                    builder.AppendLine($"        Ordinal number: {importLookupTableEntry.OrdinalNumber} (0x{importLookupTableEntry.OrdinalNumber:X})");
                                }
                                else
                                {
                                    builder.AppendLine($"        Hint/Name table RVA: {importLookupTableEntry.HintNameTableRVA} (0x{importLookupTableEntry.HintNameTableRVA:X})");
                                    builder.AppendLine($"        Hint/Name table Physical Address: {importLookupTableEntry.HintNameTableRVA.ConvertVirtualAddress(SectionTable)} (0x{importLookupTableEntry.HintNameTableRVA.ConvertVirtualAddress(SectionTable):X})");
                                }
                            }
                        }
                    }
                }
                builder.AppendLine();

                builder.AppendLine("    Import Address Tables Information:");
                builder.AppendLine("    -------------------------");
                if (ImportTable.ImportAddressTables == null || ImportTable.ImportAddressTables.Count == 0)
                {
                    builder.AppendLine("    No import address tables");
                }
                else
                {
                    foreach (var kvp in ImportTable.ImportAddressTables)
                    {
                        int index = kvp.Key;
                        var importAddressTable = kvp.Value;

                        builder.AppendLine();
                        builder.AppendLine($"      Import Address Table {index} Information:");
                        builder.AppendLine("      -------------------------");
                        if (importAddressTable == null || importAddressTable.Length == 0)
                        {
                            builder.AppendLine("      No import address table items");
                        }
                        else
                        {
                            for (int i = 0; i < importAddressTable.Length; i++)
                            {
                                var importAddressTableEntry = importAddressTable[i];
                                builder.AppendLine($"      Import Address Table {index} Entry {i}");
                                builder.AppendLine($"        Ordinal/Name flag: {importAddressTableEntry.OrdinalNameFlag} (0x{importAddressTableEntry.OrdinalNameFlag:X})");
                                if (importAddressTableEntry.OrdinalNameFlag)
                                {
                                    builder.AppendLine($"        Ordinal number: {importAddressTableEntry.OrdinalNumber} (0x{importAddressTableEntry.OrdinalNumber:X})");
                                }
                                else
                                {
                                    builder.AppendLine($"        Hint/Name table RVA: {importAddressTableEntry.HintNameTableRVA} (0x{importAddressTableEntry.HintNameTableRVA:X})");
                                    builder.AppendLine($"        Hint/Name table Physical Address: {importAddressTableEntry.HintNameTableRVA.ConvertVirtualAddress(SectionTable)} (0x{importAddressTableEntry.HintNameTableRVA.ConvertVirtualAddress(SectionTable):X})");
                                }
                            }
                        }
                    }
                }
                builder.AppendLine();

                builder.AppendLine("    Hint/Name Table Information:");
                builder.AppendLine("    -------------------------");
                if (ImportTable.HintNameTable == null || ImportTable.HintNameTable.Length == 0)
                {
                    builder.AppendLine("    No hint/name table items");
                }
                else
                {
                    for (int i = 0; i < ImportTable.HintNameTable.Length; i++)
                    {
                        var hintNameTableEntry = ImportTable.HintNameTable[i];
                        builder.AppendLine($"    Hint/Name Table Entry {i}");
                        builder.AppendLine($"      Hint: {hintNameTableEntry.Hint} (0x{hintNameTableEntry.Hint:X})");
                        builder.AppendLine($"      Name: {hintNameTableEntry.Name}");
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print resource directory table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintResourceDirectoryTable(StringBuilder builder)
        {
            builder.AppendLine("  Resource Directory Table Information:");
            builder.AppendLine("  -------------------------");
            if (OH_ResourceTable == null
                || OH_ResourceTable.VirtualAddress == 0
                || ResourceDirectoryTable == null)
            {
                builder.AppendLine("  No resource directory table items");
            }
            else
            {
                PrintResourceDirectoryTable(ResourceDirectoryTable, level: 0, types: new List<object>(), builder);
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Pretty print the resource directory table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceDirectoryTable(SabreTools.Models.PortableExecutable.ResourceDirectoryTable table, int level, List<object> types, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            builder.AppendLine($"{padding}Table level: {level}");
            builder.AppendLine($"{padding}Characteristics: {table.Characteristics} (0x{table.Characteristics:X})");
            builder.AppendLine($"{padding}Time/Date stamp: {table.TimeDateStamp} (0x{table.TimeDateStamp:X})");
            builder.AppendLine($"{padding}Major version: {table.MajorVersion} (0x{table.MajorVersion:X})");
            builder.AppendLine($"{padding}Minor version: {table.MinorVersion} (0x{table.MinorVersion:X})");
            builder.AppendLine($"{padding}Number of name entries: {table.NumberOfNameEntries} (0x{table.NumberOfNameEntries:X})");
            builder.AppendLine($"{padding}Number of ID entries: {table.NumberOfIDEntries} (0x{table.NumberOfIDEntries:X})");
            builder.AppendLine();

            builder.AppendLine($"{padding}Entries");
            builder.AppendLine($"{padding}-------------------------");
            if (table.NumberOfNameEntries == 0 && table.NumberOfIDEntries == 0)
            {
                builder.AppendLine($"{padding}No entries");
                builder.AppendLine();
            }
            else
            {
                for (int i = 0; i < table.Entries.Length; i++)
                {
                    var entry = table.Entries[i];
                    var newTypes = new List<object>(types ?? new List<object>());
                    if (entry.Name != null)
                        newTypes.Add(Encoding.UTF8.GetString(entry.Name.UnicodeString ?? new byte[0]));
                    else
                        newTypes.Add(entry.IntegerID);

                    PrintResourceDirectoryEntry(entry, level + 1, newTypes, builder);
                }
            }
        }

        /// <summary>
        /// Pretty print the resource directory entry information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceDirectoryEntry(SabreTools.Models.PortableExecutable.ResourceDirectoryEntry entry, int level, List<object> types, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            builder.AppendLine($"{padding}Item level: {level}");
            if (entry.NameOffset != default)
            {
                builder.AppendLine($"{padding}Name offset: {entry.NameOffset} (0x{entry.NameOffset:X})");
                builder.AppendLine($"{padding}Name ({entry.Name.Length}): {Encoding.UTF8.GetString(entry.Name.UnicodeString ?? new byte[0])}");
            }
            else
            {
                builder.AppendLine($"{padding}Integer ID: {entry.IntegerID} (0x{entry.IntegerID:X})");
            }

            if (entry.DataEntry != null)
                PrintResourceDataEntry(entry.DataEntry, level: level + 1, types, builder);
            else if (entry.Subdirectory != null)
                PrintResourceDirectoryTable(entry.Subdirectory, level: level + 1, types, builder);
        }

        /// <summary>
        /// Pretty print the resource data entry information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceDataEntry(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, List<object> types, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            // TODO: Use ordered list of base types to determine the shape of the data
            builder.AppendLine($"{padding}Base types: {string.Join(", ", types)}");

            builder.AppendLine($"{padding}Entry level: {level}");
            builder.AppendLine($"{padding}Data RVA: {entry.DataRVA} (0x{entry.DataRVA:X})");
            builder.AppendLine($"{padding}Size: {entry.Size} (0x{entry.Size:X})");
            builder.AppendLine($"{padding}Codepage: {entry.Codepage} (0x{entry.Codepage:X})");
            builder.AppendLine($"{padding}Reserved: {entry.Reserved} (0x{entry.Reserved:X})");

            // TODO: Print out per-type data
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                switch ((SabreTools.Models.PortableExecutable.ResourceType)resourceType)
                {
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_CURSOR:
                        PrintResourceRT_CURSOR(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_BITMAP:
                        PrintResourceRT_BITMAP(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_ICON:
                        PrintResourceRT_ICON(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_MENU:
                        PrintResourceRT_MENU(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_DIALOG:
                        PrintResourceRT_DIALOG(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_STRING:
                        PrintResourceRT_STRING(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_FONTDIR:
                        PrintResourceRT_FONTDIR(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_FONT:
                        PrintResourceRT_FONT(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_ACCELERATOR:
                        PrintResourceRT_ACCELERATOR(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_RCDATA:
                        PrintResourceRT_RCDATA(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_MESSAGETABLE:
                        PrintResourceRT_MESSAGETABLE(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_GROUP_CURSOR:
                        PrintResourceRT_GROUP_CURSOR(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_GROUP_ICON:
                        PrintResourceRT_GROUP_ICON(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_VERSION:
                        PrintResourceRT_VERSION(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_DLGINCLUDE:
                        PrintResourceRT_DLGINCLUDE(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_PLUGPLAY:
                        PrintResourceRT_PLUGPLAY(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_VXD:
                        PrintResourceRT_VXD(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_ANICURSOR:
                        PrintResourceRT_ANICURSOR(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_ANIICON:
                        PrintResourceRT_ANIICON(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_HTML:
                        PrintResourceRT_HTML(entry, level, builder);
                        break;
                    case SabreTools.Models.PortableExecutable.ResourceType.RT_MANIFEST:
                        PrintResourceRT_MANIFEST(entry, level, builder);
                        break;
                    default:
                        PrintResourceUNKNOWN(entry, level, types[0], builder);
                        break;
                }
            }
            else if (types != null && types.Count > 0 && types[0] is string resourceString)
            {
                PrintResourceUNKNOWN(entry, level, types[0], builder);
            }

            builder.AppendLine();
        }

        /// <summary>
        /// Print an RT_CURSOR resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_CURSOR(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-dependent cursor resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_BITMAP resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_BITMAP(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Bitmap resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ICON resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_ICON(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-dependent icon resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_MENU resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_MENU(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            SabreTools.Models.PortableExecutable.MenuResource menu = null;
            try { menu = entry.AsMenu(); } catch { }
            if (menu == null)
            {
                builder.AppendLine($"{padding}Menu resource found, but malformed");
                return;
            }

            if (menu.MenuHeader != null)
            {
                builder.AppendLine($"{padding}Version: {menu.MenuHeader.Version} (0x{menu.MenuHeader.Version:X})");
                builder.AppendLine($"{padding}Header size: {menu.MenuHeader.HeaderSize} (0x{menu.MenuHeader.HeaderSize:X})");
                builder.AppendLine();
                builder.AppendLine($"{padding}Menu items");
                builder.AppendLine($"{padding}-------------------------");
                if (menu.MenuItems == null || menu.MenuItems.Length == 0)
                {
                    builder.AppendLine($"{padding}No menu items");
                    return;
                }

                for (int i = 0; i < menu.MenuItems.Length; i++)
                {
                    var menuItem = menu.MenuItems[i];

                    builder.AppendLine($"{padding}Menu item {i}");
                    if (menuItem.NormalMenuText != null)
                    {
                        builder.AppendLine($"{padding}  Resource info: {menuItem.NormalResInfo} (0x{menuItem.NormalResInfo:X})");
                        builder.AppendLine($"{padding}  Menu text: {menuItem.NormalMenuText} (0x{menuItem.NormalMenuText:X})");
                    }
                    else
                    {
                        builder.AppendLine($"{padding}  Item type: {menuItem.PopupItemType} (0x{menuItem.PopupItemType:X})");
                        builder.AppendLine($"{padding}  State: {menuItem.PopupState} (0x{menuItem.PopupState:X})");
                        builder.AppendLine($"{padding}  ID: {menuItem.PopupID} (0x{menuItem.PopupID:X})");
                        builder.AppendLine($"{padding}  Resource info: {menuItem.PopupResInfo} (0x{menuItem.PopupResInfo:X})");
                        builder.AppendLine($"{padding}  Menu text: {menuItem.PopupMenuText} (0x{menuItem.PopupMenuText:X})");
                    }
                }
            }
            else if (menu.ExtendedMenuHeader != null)
            {
                builder.AppendLine($"{padding}Version: {menu.ExtendedMenuHeader.Version} (0x{menu.ExtendedMenuHeader.Version:X})");
                builder.AppendLine($"{padding}Offset: {menu.ExtendedMenuHeader.Offset} (0x{menu.ExtendedMenuHeader.Offset:X})");
                builder.AppendLine($"{padding}Help ID: {menu.ExtendedMenuHeader.HelpID} (0x{menu.ExtendedMenuHeader.HelpID:X})");
                builder.AppendLine();
                builder.AppendLine($"{padding}Menu items");
                builder.AppendLine($"{padding}-------------------------");
                if (menu.ExtendedMenuHeader.Offset == 0
                    || menu.ExtendedMenuItems == null
                    || menu.ExtendedMenuItems.Length == 0)
                {
                    builder.AppendLine($"{padding}No menu items");
                    return;
                }

                for (int i = 0; i < menu.ExtendedMenuItems.Length; i++)
                {
                    var menuItem = menu.ExtendedMenuItems[i];

                    builder.AppendLine($"{padding}Dialog item template {i}");
                    builder.AppendLine($"{padding}  Item type: {menuItem.ItemType} (0x{menuItem.ItemType:X})");
                    builder.AppendLine($"{padding}  State: {menuItem.State} (0x{menuItem.State:X})");
                    builder.AppendLine($"{padding}  ID: {menuItem.ID} (0x{menuItem.ID:X})");
                    builder.AppendLine($"{padding}  Flags: {menuItem.Flags} (0x{menuItem.Flags:X})");
                    builder.AppendLine($"{padding}  Menu text: {menuItem.MenuText} (0x{menuItem.MenuText:X})");
                }
            }
            else
            {
                builder.AppendLine($"{padding}Menu resource found, but malformed");
            }
        }

        /// <summary>
        /// Print an RT_DIALOG resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_DIALOG(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            SabreTools.Models.PortableExecutable.DialogBoxResource dialogBox = null;
            try { dialogBox = entry.AsDialogBox(); } catch { }
            if (dialogBox == null)
            {
                builder.AppendLine($"{padding}Dialog box resource found, but malformed");
                return;
            }

            if (dialogBox.DialogTemplate != null)
            {
                builder.AppendLine($"{padding}Style: {dialogBox.DialogTemplate.Style} (0x{dialogBox.DialogTemplate.Style:X})");
                builder.AppendLine($"{padding}Extended style: {dialogBox.DialogTemplate.ExtendedStyle} (0x{dialogBox.DialogTemplate.ExtendedStyle:X})");
                builder.AppendLine($"{padding}Item count: {dialogBox.DialogTemplate.ItemCount} (0x{dialogBox.DialogTemplate.ItemCount:X})");
                builder.AppendLine($"{padding}X-coordinate of upper-left corner: {dialogBox.DialogTemplate.PositionX} (0x{dialogBox.DialogTemplate.PositionX:X})");
                builder.AppendLine($"{padding}Y-coordinate of upper-left corner: {dialogBox.DialogTemplate.PositionY} (0x{dialogBox.DialogTemplate.PositionY:X})");
                builder.AppendLine($"{padding}Width of the dialog box: {dialogBox.DialogTemplate.WidthX} (0x{dialogBox.DialogTemplate.WidthX:X})");
                builder.AppendLine($"{padding}Height of the dialog box: {dialogBox.DialogTemplate.HeightY} (0x{dialogBox.DialogTemplate.HeightY:X})");
                builder.AppendLine($"{padding}Menu resource: {dialogBox.DialogTemplate.MenuResource ?? "[EMPTY]"}");
                builder.AppendLine($"{padding}Menu resource ordinal: {dialogBox.DialogTemplate.MenuResourceOrdinal} (0x{dialogBox.DialogTemplate.MenuResourceOrdinal:X})");
                builder.AppendLine($"{padding}Class resource: {dialogBox.DialogTemplate.ClassResource ?? "[EMPTY]"}");
                builder.AppendLine($"{padding}Class resource ordinal: {dialogBox.DialogTemplate.ClassResourceOrdinal} (0x{dialogBox.DialogTemplate.ClassResourceOrdinal:X})");
                builder.AppendLine($"{padding}Title resource: {dialogBox.DialogTemplate.TitleResource ?? "[EMPTY]"}");
                builder.AppendLine($"{padding}Point size value: {dialogBox.DialogTemplate.PointSizeValue} (0x{dialogBox.DialogTemplate.PointSizeValue:X})");
                builder.AppendLine($"{padding}Typeface: {dialogBox.DialogTemplate.Typeface ?? "[EMPTY]"}");
                builder.AppendLine();
                builder.AppendLine($"{padding}Dialog item templates");
                builder.AppendLine($"{padding}-------------------------");
                if (dialogBox.DialogTemplate.ItemCount == 0
                    || dialogBox.DialogItemTemplates == null
                    || dialogBox.DialogItemTemplates.Length == 0)
                {
                    builder.AppendLine($"{padding}No dialog item templates");
                    return;
                }

                for (int i = 0; i < dialogBox.DialogItemTemplates.Length; i++)
                {
                    var dialogItemTemplate = dialogBox.DialogItemTemplates[i];

                    builder.AppendLine($"{padding}Dialog item template {i}");
                    builder.AppendLine($"{padding}  Style: {dialogItemTemplate.Style} (0x{dialogItemTemplate.Style:X})");
                    builder.AppendLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle} (0x{dialogItemTemplate.ExtendedStyle:X})");
                    builder.AppendLine($"{padding}  X-coordinate of upper-left corner: {dialogItemTemplate.PositionX} (0x{dialogItemTemplate.PositionX:X})");
                    builder.AppendLine($"{padding}  Y-coordinate of upper-left corner: {dialogItemTemplate.PositionY} (0x{dialogItemTemplate.PositionY:X})");
                    builder.AppendLine($"{padding}  Width of the control: {dialogItemTemplate.WidthX} (0x{dialogItemTemplate.WidthX:X})");
                    builder.AppendLine($"{padding}  Height of the control: {dialogItemTemplate.HeightY} (0x{dialogItemTemplate.HeightY:X})");
                    builder.AppendLine($"{padding}  ID: {dialogItemTemplate.ID} (0x{dialogItemTemplate.ID:X})");
                    builder.AppendLine($"{padding}  Class resource: {dialogItemTemplate.ClassResource ?? "[EMPTY]"}");
                    builder.AppendLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal} (0x{dialogItemTemplate.ClassResourceOrdinal:X})");
                    builder.AppendLine($"{padding}  Title resource: {dialogItemTemplate.TitleResource ?? "[EMPTY]"}");
                    builder.AppendLine($"{padding}  Title resource ordinal: {dialogItemTemplate.TitleResourceOrdinal} (0x{dialogItemTemplate.TitleResourceOrdinal:X})");
                    builder.AppendLine($"{padding}  Creation data size: {dialogItemTemplate.CreationDataSize} (0x{dialogItemTemplate.CreationDataSize:X})");
                    if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                        builder.AppendLine($"{padding}  Creation data: {BitConverter.ToString(dialogItemTemplate.CreationData).Replace("-", string.Empty)}");
                    else
                        builder.AppendLine($"{padding}  Creation data: [EMPTY]");
                }
            }
            else if (dialogBox.ExtendedDialogTemplate != null)
            {
                builder.AppendLine($"{padding}Version: {dialogBox.ExtendedDialogTemplate.Version} (0x{dialogBox.ExtendedDialogTemplate.Version:X})");
                builder.AppendLine($"{padding}Signature: {dialogBox.ExtendedDialogTemplate.Signature} (0x{dialogBox.ExtendedDialogTemplate.Signature:X})");
                builder.AppendLine($"{padding}Help ID: {dialogBox.ExtendedDialogTemplate.HelpID} (0x{dialogBox.ExtendedDialogTemplate.HelpID:X})");
                builder.AppendLine($"{padding}Extended style: {dialogBox.ExtendedDialogTemplate.ExtendedStyle} (0x{dialogBox.ExtendedDialogTemplate.ExtendedStyle:X})");
                builder.AppendLine($"{padding}Style: {dialogBox.ExtendedDialogTemplate.Style} (0x{dialogBox.ExtendedDialogTemplate.Style:X})");
                builder.AppendLine($"{padding}Item count: {dialogBox.ExtendedDialogTemplate.DialogItems} (0x{dialogBox.ExtendedDialogTemplate.DialogItems:X})");
                builder.AppendLine($"{padding}X-coordinate of upper-left corner: {dialogBox.ExtendedDialogTemplate.PositionX} (0x{dialogBox.ExtendedDialogTemplate.PositionX:X})");
                builder.AppendLine($"{padding}Y-coordinate of upper-left corner: {dialogBox.ExtendedDialogTemplate.PositionY} (0x{dialogBox.ExtendedDialogTemplate.PositionY:X})");
                builder.AppendLine($"{padding}Width of the dialog box: {dialogBox.ExtendedDialogTemplate.WidthX} (0x{dialogBox.ExtendedDialogTemplate.WidthX:X})");
                builder.AppendLine($"{padding}Height of the dialog box: {dialogBox.ExtendedDialogTemplate.HeightY} (0x{dialogBox.ExtendedDialogTemplate.HeightY:X})");
                builder.AppendLine($"{padding}Menu resource: {dialogBox.ExtendedDialogTemplate.MenuResource ?? "[EMPTY]"}");
                builder.AppendLine($"{padding}Menu resource ordinal: {dialogBox.ExtendedDialogTemplate.MenuResourceOrdinal} (0x{dialogBox.ExtendedDialogTemplate.MenuResourceOrdinal:X})");
                builder.AppendLine($"{padding}Class resource: {dialogBox.ExtendedDialogTemplate.ClassResource ?? "[EMPTY]"}");
                builder.AppendLine($"{padding}Class resource ordinal: {dialogBox.ExtendedDialogTemplate.ClassResourceOrdinal} (0x{dialogBox.ExtendedDialogTemplate.ClassResourceOrdinal:X})");
                builder.AppendLine($"{padding}Title resource: {dialogBox.ExtendedDialogTemplate.TitleResource ?? "[EMPTY]"}");
                builder.AppendLine($"{padding}Point size: {dialogBox.ExtendedDialogTemplate.PointSize} (0x{dialogBox.ExtendedDialogTemplate.PointSize:X})");
                builder.AppendLine($"{padding}Weight: {dialogBox.ExtendedDialogTemplate.Weight} (0x{dialogBox.ExtendedDialogTemplate.Weight:X})");
                builder.AppendLine($"{padding}Italic: {dialogBox.ExtendedDialogTemplate.Italic} (0x{dialogBox.ExtendedDialogTemplate.Italic:X})");
                builder.AppendLine($"{padding}Character set: {dialogBox.ExtendedDialogTemplate.CharSet} (0x{dialogBox.ExtendedDialogTemplate.CharSet:X})");
                builder.AppendLine($"{padding}Typeface: {dialogBox.ExtendedDialogTemplate.Typeface ?? "[EMPTY]"}");
                builder.AppendLine();
                builder.AppendLine($"{padding}Dialog item templates");
                builder.AppendLine($"{padding}-------------------------");
                if (dialogBox.ExtendedDialogTemplate.DialogItems == 0
                    || dialogBox.ExtendedDialogItemTemplates == null
                    || dialogBox.ExtendedDialogItemTemplates.Length == 0)
                {
                    builder.AppendLine($"{padding}No dialog item templates");
                    return;
                }

                for (int i = 0; i < dialogBox.ExtendedDialogItemTemplates.Length; i++)
                {
                    var dialogItemTemplate = dialogBox.ExtendedDialogItemTemplates[i];

                    builder.AppendLine($"{padding}Dialog item template {i}");
                    builder.AppendLine($"{padding}  Help ID: {dialogItemTemplate.HelpID} (0x{dialogItemTemplate.HelpID:X})");
                    builder.AppendLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle} (0x{dialogItemTemplate.ExtendedStyle:X})");
                    builder.AppendLine($"{padding}  Style: {dialogItemTemplate.Style} (0x{dialogItemTemplate.Style:X})");
                    builder.AppendLine($"{padding}  X-coordinate of upper-left corner: {dialogItemTemplate.PositionX} (0x{dialogItemTemplate.PositionX:X})");
                    builder.AppendLine($"{padding}  Y-coordinate of upper-left corner: {dialogItemTemplate.PositionY} (0x{dialogItemTemplate.PositionY:X})");
                    builder.AppendLine($"{padding}  Width of the control: {dialogItemTemplate.WidthX} (0x{dialogItemTemplate.WidthX:X})");
                    builder.AppendLine($"{padding}  Height of the control: {dialogItemTemplate.HeightY} (0x{dialogItemTemplate.HeightY:X})");
                    builder.AppendLine($"{padding}  ID: {dialogItemTemplate.ID} (0x{dialogItemTemplate.ID:X})");
                    builder.AppendLine($"{padding}  Class resource: {dialogItemTemplate.ClassResource ?? "[EMPTY]"}");
                    builder.AppendLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal} (0x{dialogItemTemplate.ClassResourceOrdinal:X})");
                    builder.AppendLine($"{padding}  Title resource: {dialogItemTemplate.TitleResource ?? "[EMPTY]"}");
                    builder.AppendLine($"{padding}  Title resource ordinal: {dialogItemTemplate.TitleResourceOrdinal} (0x{dialogItemTemplate.TitleResourceOrdinal:X})");
                    builder.AppendLine($"{padding}  Creation data size: {dialogItemTemplate.CreationDataSize} (0x{dialogItemTemplate.CreationDataSize:X})");
                    if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                        builder.AppendLine($"{padding}  Creation data: {BitConverter.ToString(dialogItemTemplate.CreationData).Replace("-", string.Empty)}");
                    else
                        builder.AppendLine($"{padding}  Creation data: [EMPTY]");
                }
            }
            else
            {
                builder.AppendLine($"{padding}Dialog box resource found, but malformed");
            }
        }

        /// <summary>
        /// Print an RT_STRING resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_STRING(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            Dictionary<int, string> stringTable = null;
            try { stringTable = entry.AsStringTable(); } catch { }
            if (stringTable == null)
            {
                builder.AppendLine($"{padding}String table resource found, but malformed");
                return;
            }

            foreach (var kvp in stringTable)
            {
                int index = kvp.Key;
                string stringValue = kvp.Value;
                builder.AppendLine($"{padding}String entry {index}: {stringValue}");
            }
        }

        /// <summary>
        /// Print an RT_FONTDIR resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_FONTDIR(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Font directory resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_FONT resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_FONT(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Font resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ACCELERATOR resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_ACCELERATOR(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            SabreTools.Models.PortableExecutable.AcceleratorTableEntry[] acceleratorTable = null;
            try { acceleratorTable = entry.AsAcceleratorTableResource(); } catch { }
            if (acceleratorTable == null)
            {
                builder.AppendLine($"{padding}Accelerator table resource found, but malformed");
                return;
            }

            for (int i = 0; i < acceleratorTable.Length; i++)
            {
                var acceleratorTableEntry = acceleratorTable[i];
                builder.AppendLine($"{padding}Accelerator Table Entry {i}:");
                builder.AppendLine($"{padding}  Flags: {acceleratorTableEntry.Flags} (0x{acceleratorTableEntry.Flags:X})");
                builder.AppendLine($"{padding}  Ansi: {acceleratorTableEntry.Ansi} (0x{acceleratorTableEntry.Ansi:X})");
                builder.AppendLine($"{padding}  Id: {acceleratorTableEntry.Id} (0x{acceleratorTableEntry.Id:X})");
                builder.AppendLine($"{padding}  Padding: {acceleratorTableEntry.Padding} (0x{acceleratorTableEntry.Padding:X})");
            }
        }

        /// <summary>
        /// Print an RT_RCDATA resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_RCDATA(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Application-defined resource found, not parsed yet");

            // Then print the data, if needed
            if (entry.Data == null)
            {
                builder.AppendLine($"{padding}Data: [NULL] (This may indicate a very large resource)");
            }
            else
            {
                int offset = 0;
                byte[] magic = entry.Data.ReadBytes(ref offset, Math.Min(entry.Data.Length, 16));

                if (entry.Data[0] == 0x4D && entry.Data[1] == 0x5A)
                {
                    builder.AppendLine($"{padding}Data: [Embedded Executable File]"); // TODO: Parse this out and print separately
                }
                else if (entry.Data[0] == 0x4D && entry.Data[1] == 0x53 && entry.Data[2] == 0x46 && entry.Data[3] == 0x54)
                {
                    builder.AppendLine($"{padding}Data: [Embedded OLE Library File]"); // TODO: Parse this out and print separately
                }
                else
                {
                    builder.AppendLine($"{padding}Data: {BitConverter.ToString(magic).Replace('-', ' ')} ...");

                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (Byte Data): {BitConverter.ToString(entry.Data).Replace('-', ' ')}");
                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (ASCII): {Encoding.ASCII.GetString(entry.Data)}");
                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (UTF-8): {Encoding.UTF8.GetString(entry.Data)}");
                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (Unicode): {Encoding.Unicode.GetString(entry.Data)}");
                }
            }
        }

        /// <summary>
        /// Print an RT_MESSAGETABLE resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_MESSAGETABLE(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            SabreTools.Models.PortableExecutable.MessageResourceData messageTable = null;
            try { messageTable = entry.AsMessageResourceData(); } catch { }
            if (messageTable == null)
            {
                builder.AppendLine($"{padding}Message resource data found, but malformed");
                return;
            }

            builder.AppendLine($"{padding}Number of blocks: {messageTable.NumberOfBlocks} (0x{messageTable.NumberOfBlocks:X})");
            builder.AppendLine();
            builder.AppendLine($"{padding}Message resource blocks");
            builder.AppendLine($"{padding}-------------------------");
            if (messageTable.NumberOfBlocks == 0
                || messageTable.Blocks == null
                || messageTable.Blocks.Length == 0)
            {
                builder.AppendLine($"{padding}No message resource blocks");
            }
            else
            {
                for (int i = 0; i < messageTable.Blocks.Length; i++)
                {
                    var messageResourceBlock = messageTable.Blocks[i];

                    builder.AppendLine($"{padding}Message resource block {i}");
                    builder.AppendLine($"{padding}  Low ID: {messageResourceBlock.LowId} (0x{messageResourceBlock.LowId:X})");
                    builder.AppendLine($"{padding}  High ID: {messageResourceBlock.HighId} (0x{messageResourceBlock.HighId:X})");
                    builder.AppendLine($"{padding}  Offset to entries: {messageResourceBlock.OffsetToEntries} (0x{messageResourceBlock.OffsetToEntries:X})");
                }
            }
            builder.AppendLine();

            builder.AppendLine($"{padding}Message resource entries");
            builder.AppendLine($"{padding}-------------------------");
            if (messageTable.Entries == null
                || messageTable.Entries.Count == 0)
            {
                builder.AppendLine($"{padding}No message resource entries");
            }
            else
            {
                foreach (var kvp in messageTable.Entries)
                {
                    uint index = kvp.Key;
                    var messageResourceEntry = kvp.Value;

                    builder.AppendLine($"{padding}Message resource entry {index}");
                    builder.AppendLine($"{padding}  Length: {messageResourceEntry.Length} (0x{messageResourceEntry.Length:X})");
                    builder.AppendLine($"{padding}  Flags: {messageResourceEntry.Flags} (0x{messageResourceEntry.Flags:X})");
                    builder.AppendLine($"{padding}  Text: {messageResourceEntry.Text}");
                }
            }
        }

        /// <summary>
        /// Print an RT_GROUP_CURSOR resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_GROUP_CURSOR(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-independent cursor resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_GROUP_ICON resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_GROUP_ICON(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-independent icon resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_VERSION resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_VERSION(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            SabreTools.Models.PortableExecutable.VersionInfo versionInfo = null;
            try { versionInfo = entry.AsVersionInfo(); } catch { }
            if (versionInfo == null)
            {
                builder.AppendLine($"{padding}Version info resource found, but malformed");
                return;
            }

            builder.AppendLine($"{padding}Length: {versionInfo.Length} (0x{versionInfo.Length:X})");
            builder.AppendLine($"{padding}Value length: {versionInfo.ValueLength} (0x{versionInfo.ValueLength:X})");
            builder.AppendLine($"{padding}Resource type: {versionInfo.ResourceType} (0x{versionInfo.ResourceType:X})");
            builder.AppendLine($"{padding}Key: {versionInfo.Key}");
            if (versionInfo.ValueLength != 0 && versionInfo.Value != null)
            {
                builder.AppendLine($"{padding}[Fixed File Info] Signature: {versionInfo.Value.Signature} (0x{versionInfo.Value.Signature:X})");
                builder.AppendLine($"{padding}[Fixed File Info] Struct version: {versionInfo.Value.StrucVersion} (0x{versionInfo.Value.StrucVersion:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File version (MS): {versionInfo.Value.FileVersionMS} (0x{versionInfo.Value.FileVersionMS:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File version (LS): {versionInfo.Value.FileVersionLS} (0x{versionInfo.Value.FileVersionLS:X})");
                builder.AppendLine($"{padding}[Fixed File Info] Product version (MS): {versionInfo.Value.ProductVersionMS} (0x{versionInfo.Value.ProductVersionMS:X})");
                builder.AppendLine($"{padding}[Fixed File Info] Product version (LS): {versionInfo.Value.ProductVersionLS} (0x{versionInfo.Value.ProductVersionLS:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File flags mask: {versionInfo.Value.FileFlagsMask} (0x{versionInfo.Value.FileFlagsMask:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File flags: {versionInfo.Value.FileFlags} (0x{versionInfo.Value.FileFlags:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File OS: {versionInfo.Value.FileOS} (0x{versionInfo.Value.FileOS:X})");
                builder.AppendLine($"{padding}[Fixed File Info] Type: {versionInfo.Value.FileType} (0x{versionInfo.Value.FileType:X})");
                builder.AppendLine($"{padding}[Fixed File Info] Subtype: {versionInfo.Value.FileSubtype} (0x{versionInfo.Value.FileSubtype:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File date (MS): {versionInfo.Value.FileDateMS} (0x{versionInfo.Value.FileDateMS:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File date (LS): {versionInfo.Value.FileDateLS} (0x{versionInfo.Value.FileDateLS:X})");
            }

            if (versionInfo.StringFileInfo != null)
            {
                builder.AppendLine($"{padding}[String File Info] Length: {versionInfo.StringFileInfo.Length} (0x{versionInfo.StringFileInfo.Length:X})");
                builder.AppendLine($"{padding}[String File Info] Value length: {versionInfo.StringFileInfo.ValueLength} (0x{versionInfo.StringFileInfo.ValueLength:X})");
                builder.AppendLine($"{padding}[String File Info] Resource type: {versionInfo.StringFileInfo.ResourceType} (0x{versionInfo.StringFileInfo.ResourceType:X})");
                builder.AppendLine($"{padding}[String File Info] Key: {versionInfo.StringFileInfo.Key}");
                builder.AppendLine($"{padding}Children:");
                builder.AppendLine($"{padding}-------------------------");
                if (versionInfo.StringFileInfo.Children == null || versionInfo.StringFileInfo.Children.Length == 0)
                {
                    builder.AppendLine($"{padding}No string file info children");
                }
                else
                {
                    for (int i = 0; i < versionInfo.StringFileInfo.Children.Length; i++)
                    {
                        var stringFileInfoChildEntry = versionInfo.StringFileInfo.Children[i];

                        builder.AppendLine($"{padding}  [String Table {i}] Length: {stringFileInfoChildEntry.Length} (0x{stringFileInfoChildEntry.Length:X})");
                        builder.AppendLine($"{padding}  [String Table {i}] Value length: {stringFileInfoChildEntry.ValueLength} (0x{stringFileInfoChildEntry.ValueLength:X})");
                        builder.AppendLine($"{padding}  [String Table {i}] ResourceType: {stringFileInfoChildEntry.ResourceType} (0x{stringFileInfoChildEntry.ResourceType:X})");
                        builder.AppendLine($"{padding}  [String Table {i}] Key: {stringFileInfoChildEntry.Key}");
                        builder.AppendLine($"{padding}  [String Table {i}] Children:");
                        builder.AppendLine($"{padding}  -------------------------");
                        if (stringFileInfoChildEntry.Children == null || stringFileInfoChildEntry.Children.Length == 0)
                        {
                            builder.AppendLine($"{padding}  No string table {i} children");
                        }
                        else
                        {
                            for (int j = 0; j < stringFileInfoChildEntry.Children.Length; j++)
                            {
                                var stringDataEntry = stringFileInfoChildEntry.Children[j];

                                builder.AppendLine($"{padding}    [String Data {j}] Length: {stringDataEntry.Length} (0x{stringDataEntry.Length:X})");
                                builder.AppendLine($"{padding}    [String Data {j}] Value length: {stringDataEntry.ValueLength} (0x{stringDataEntry.ValueLength:X})");
                                builder.AppendLine($"{padding}    [String Data {j}] ResourceType: {stringDataEntry.ResourceType} (0x{stringDataEntry.ResourceType:X})");
                                builder.AppendLine($"{padding}    [String Data {j}] Key: {stringDataEntry.Key}");
                                builder.AppendLine($"{padding}    [String Data {j}] Value: {stringDataEntry.Value}");
                            }
                        }
                    }
                }
            }

            if (versionInfo.VarFileInfo != null)
            {
                builder.AppendLine($"{padding}[Var File Info] Length: {versionInfo.VarFileInfo.Length} (0x{versionInfo.VarFileInfo.Length:X})");
                builder.AppendLine($"{padding}[Var File Info] Value length: {versionInfo.VarFileInfo.ValueLength} (0x{versionInfo.VarFileInfo.ValueLength:X})");
                builder.AppendLine($"{padding}[Var File Info] Resource type: {versionInfo.VarFileInfo.ResourceType} (0x{versionInfo.VarFileInfo.ResourceType:X})");
                builder.AppendLine($"{padding}[Var File Info] Key: {versionInfo.VarFileInfo.Key}");
                builder.AppendLine($"{padding}Children:");
                builder.AppendLine($"{padding}-------------------------");
                if (versionInfo.VarFileInfo.Children == null || versionInfo.VarFileInfo.Children.Length == 0)
                {
                    builder.AppendLine($"{padding}No var file info children");
                }
                else
                {
                    for (int i = 0; i < versionInfo.VarFileInfo.Children.Length; i++)
                    {
                        var varFileInfoChildEntry = versionInfo.VarFileInfo.Children[i];

                        builder.AppendLine($"{padding}  [String Table {i}] Length: {varFileInfoChildEntry.Length} (0x{varFileInfoChildEntry.Length:X})");
                        builder.AppendLine($"{padding}  [String Table {i}] Value length: {varFileInfoChildEntry.ValueLength} (0x{varFileInfoChildEntry.ValueLength:X})");
                        builder.AppendLine($"{padding}  [String Table {i}] ResourceType: {varFileInfoChildEntry.ResourceType} (0x{varFileInfoChildEntry.ResourceType:X})");
                        builder.AppendLine($"{padding}  [String Table {i}] Key: {varFileInfoChildEntry.Key}");
                        builder.AppendLine($"{padding}  [String Table {i}] Value: {string.Join(",", varFileInfoChildEntry.Value)}");
                    }
                }
            }
        }

        /// <summary>
        /// Print an RT_DLGINCLUDE resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_DLGINCLUDE(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}External header resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_PLUGPLAY resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_PLUGPLAY(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Plug and Play resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_VXD resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_VXD(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}VXD found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ANICURSOR resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_ANICURSOR(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Animated cursor found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ANIICON resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_ANIICON(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Animated icon found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_HTML resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_HTML(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}HTML resource found, not parsed yet");

            //if (entry.Data != null)
            //    builder.AppendLine($"{padding}Value (ASCII): {Encoding.ASCII.GetString(entry.Data)}");
            //if (entry.Data != null)
            //    builder.AppendLine($"{padding}Value (UTF-8): {Encoding.UTF8.GetString(entry.Data)}");
            //if (entry.Data != null)
            //    builder.AppendLine($"{padding}Value (Unicode): {Encoding.Unicode.GetString(entry.Data)}");
        }

        /// <summary>
        /// Print an RT_MANIFEST resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceRT_MANIFEST(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            SabreTools.Models.PortableExecutable.AssemblyManifest assemblyManifest = null;
            try { assemblyManifest = entry.AsAssemblyManifest(); } catch { }
            if (assemblyManifest == null)
            {
                builder.AppendLine($"{padding}Assembly manifest found, but malformed");
                return;
            }

            builder.AppendLine($"{padding}Manifest version: {assemblyManifest.ManifestVersion}");
            if (assemblyManifest.AssemblyIdentities != null && assemblyManifest.AssemblyIdentities.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.AssemblyIdentities.Length; i++)
                {
                    var assemblyIdentity = assemblyManifest.AssemblyIdentities[i];
                    builder.AppendLine($"{padding}[Assembly Identity {i}] Name: {assemblyIdentity.Name}");
                    builder.AppendLine($"{padding}[Assembly Identity {i}] Version: {assemblyIdentity.Version}");
                    builder.AppendLine($"{padding}[Assembly Identity {i}] Type: {assemblyIdentity.Type}");
                    builder.AppendLine($"{padding}[Assembly Identity {i}] Processor architecture: {assemblyIdentity.ProcessorArchitecture}");
                    builder.AppendLine($"{padding}[Assembly Identity {i}] Public key token: {assemblyIdentity.PublicKeyToken}");
                    builder.AppendLine($"{padding}[Assembly Identity {i}] Language: {assemblyIdentity.Language}");
                }
            }

            if (assemblyManifest.Description != null)
                builder.AppendLine($"{padding}[Assembly Description] Value: {assemblyManifest.Description.Value}");

            if (assemblyManifest.COMInterfaceExternalProxyStub != null && assemblyManifest.COMInterfaceExternalProxyStub.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.COMInterfaceExternalProxyStub.Length; i++)
                {
                    var comInterfaceExternalProxyStub = assemblyManifest.COMInterfaceExternalProxyStub[i];
                    builder.AppendLine($"{padding}[COM Interface External Proxy Stub {i}] IID: {comInterfaceExternalProxyStub.IID}");
                    builder.AppendLine($"{padding}[COM Interface External Proxy Stub {i}] Name: {comInterfaceExternalProxyStub.Name}");
                    builder.AppendLine($"{padding}[COM Interface External Proxy Stub {i}] TLBID: {comInterfaceExternalProxyStub.TLBID}");
                    builder.AppendLine($"{padding}[COM Interface External Proxy Stub {i}] Number of methods: {comInterfaceExternalProxyStub.NumMethods}");
                    builder.AppendLine($"{padding}[COM Interface External Proxy Stub {i}] Proxy stub (CLSID32): {comInterfaceExternalProxyStub.ProxyStubClsid32}");
                    builder.AppendLine($"{padding}[COM Interface External Proxy Stub {i}] Base interface: {comInterfaceExternalProxyStub.BaseInterface}");
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
                            builder.AppendLine($"{padding}[Dependency {i} Assembly Identity] Name: {dependency.DependentAssembly.AssemblyIdentity.Name}");
                            builder.AppendLine($"{padding}[Dependency {i} Assembly Identity] Version: {dependency.DependentAssembly.AssemblyIdentity.Version}");
                            builder.AppendLine($"{padding}[Dependency {i} Assembly Identity] Type: {dependency.DependentAssembly.AssemblyIdentity.Type}");
                            builder.AppendLine($"{padding}[Dependency {i} Assembly Identity] Processor architecture: {dependency.DependentAssembly.AssemblyIdentity.ProcessorArchitecture}");
                            builder.AppendLine($"{padding}[Dependency {i} Assembly Identity] Public key token: {dependency.DependentAssembly.AssemblyIdentity.PublicKeyToken}");
                            builder.AppendLine($"{padding}[Dependency {i} Assembly Identity] Language: {dependency.DependentAssembly.AssemblyIdentity.Language}");
                        }
                        if (dependency.DependentAssembly.BindingRedirect != null && dependency.DependentAssembly.BindingRedirect.Length > 0)
                        {
                            for (int j = 0; j < dependency.DependentAssembly.BindingRedirect.Length; j++)
                            {
                                var bindingRedirect = dependency.DependentAssembly.BindingRedirect[j];
                                builder.AppendLine($"{padding}[Dependency {i} Binding Redirect {j}] Old version: {bindingRedirect.OldVersion}");
                                builder.AppendLine($"{padding}[Dependency {i} Binding Redirect {j}] New version: {bindingRedirect.NewVersion}");
                            }
                        }
                    }

                    builder.AppendLine($"{padding}[Dependency {i}] Optional: {dependency.Optional}");
                }
            }

            if (assemblyManifest.File != null && assemblyManifest.File.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.File.Length; i++)
                {
                    var file = assemblyManifest.File[i];
                    builder.AppendLine($"{padding}[File {i}] Name: {file.Name}");
                    builder.AppendLine($"{padding}[File {i}] Hash: {file.Hash}");
                    builder.AppendLine($"{padding}[File {i}] Hash algorithm: {file.HashAlgorithm}");
                    builder.AppendLine($"{padding}[File {i}] Size: {file.Size}");

                    if (file.COMClass != null && file.COMClass.Length > 0)
                    {
                        for (int j = 0; j < file.COMClass.Length; j++)
                        {
                            var comClass = file.COMClass[j];
                            builder.AppendLine($"{padding}[File {i} COM Class {j}] CLSID: {comClass.CLSID}");
                            builder.AppendLine($"{padding}[File {i} COM Class {j}] Threading model: {comClass.ThreadingModel}");
                            builder.AppendLine($"{padding}[File {i} COM Class {j}] Prog ID: {comClass.ProgID}");
                            builder.AppendLine($"{padding}[File {i} COM Class {j}] TLBID: {comClass.TLBID}");
                            builder.AppendLine($"{padding}[File {i} COM Class {j}] Description: {comClass.Description}");

                            if (comClass.ProgIDs != null && comClass.ProgIDs.Length > 0)
                            {
                                for (int k = 0; k < comClass.ProgIDs.Length; k++)
                                {
                                    var progId = comClass.ProgIDs[k];
                                    builder.AppendLine($"{padding}[File {i} COM Class {j} Prog ID {k}] Value: {progId.Value}");
                                }
                            }
                        }
                    }

                    if (file.COMInterfaceProxyStub != null && file.COMInterfaceProxyStub.Length > 0)
                    {
                        for (int j = 0; j < file.COMInterfaceProxyStub.Length; j++)
                        {
                            var comInterfaceProxyStub = file.COMInterfaceProxyStub[j];
                            builder.AppendLine($"{padding}[File {i} COM Interface Proxy Stub {j}] IID: {comInterfaceProxyStub.IID}");
                            builder.AppendLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Name: {comInterfaceProxyStub.Name}");
                            builder.AppendLine($"{padding}[File {i} COM Interface Proxy Stub {j}] TLBID: {comInterfaceProxyStub.TLBID}");
                            builder.AppendLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Number of methods: {comInterfaceProxyStub.NumMethods}");
                            builder.AppendLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Proxy stub (CLSID32): {comInterfaceProxyStub.ProxyStubClsid32}");
                            builder.AppendLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Base interface: {comInterfaceProxyStub.BaseInterface}");
                        }
                    }

                    if (file.Typelib != null && file.Typelib.Length > 0)
                    {
                        for (int j = 0; j < file.Typelib.Length; j++)
                        {
                            var typeLib = file.Typelib[j];
                            builder.AppendLine($"{padding}[File {i} Type Lib {j}] TLBID: {typeLib.TLBID}");
                            builder.AppendLine($"{padding}[File {i} Type Lib {j}] Version: {typeLib.Version}");
                            builder.AppendLine($"{padding}[File {i} Type Lib {j}] Help directory: {typeLib.HelpDir}");
                            builder.AppendLine($"{padding}[File {i} Type Lib {j}] Resource ID: {typeLib.ResourceID}");
                            builder.AppendLine($"{padding}[File {i} Type Lib {j}] Flags: {typeLib.Flags}");
                        }
                    }

                    if (file.WindowClass != null && file.WindowClass.Length > 0)
                    {
                        for (int j = 0; j < file.WindowClass.Length; j++)
                        {
                            var windowClass = file.WindowClass[j];
                            builder.AppendLine($"{padding}[File {i} Window Class {j}] Versioned: {windowClass.Versioned}");
                            builder.AppendLine($"{padding}[File {i} Window Class {j}] Value: {windowClass.Value}");
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
                        builder.AppendLine($"{padding}Unparsed XML Element {i}: {element.OuterXml}");
                    }
                    else
                    {
                        builder.AppendLine($"{padding}Unparsed Item {i}: {thing}");
                    }
                }
            }
        }

        /// <summary>
        /// Print an UNKNOWN or custom resource
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private static void PrintResourceUNKNOWN(SabreTools.Models.PortableExecutable.ResourceDataEntry entry, int level, object resourceType, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            // Print the type first
            if (resourceType is uint numericType)
                builder.AppendLine($"{padding}Type {(SabreTools.Models.PortableExecutable.ResourceType)numericType} found, not parsed yet");
            else if (resourceType is string stringType)
                builder.AppendLine($"{padding}Type {stringType} found, not parsed yet");
            else
                builder.AppendLine($"{padding}Unknown type {resourceType} found, not parsed yet");

            // Then print the data, if needed
            if (entry.Data == null)
            {
                builder.AppendLine($"{padding}Data: [NULL] (This may indicate a very large resource)");
            }
            else
            {
                int offset = 0;
                byte[] magic = entry.Data.ReadBytes(ref offset, Math.Min(entry.Data.Length, 16));

                if (entry.Data[0] == 0x4D && entry.Data[1] == 0x5A)
                {
                    builder.AppendLine($"{padding}Data: [Embedded Executable File]"); // TODO: Parse this out and print separately
                }
                else if (entry.Data[0] == 0x4D && entry.Data[1] == 0x53 && entry.Data[2] == 0x46 && entry.Data[3] == 0x54)
                {
                    builder.AppendLine($"{padding}Data: [Embedded OLE Library File]"); // TODO: Parse this out and print separately
                }
                else
                {
                    builder.AppendLine($"{padding}Data: {BitConverter.ToString(magic).Replace('-', ' ')} ...");

                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (Byte Data): {BitConverter.ToString(entry.Data).Replace('-', ' ')}");
                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (ASCII): {Encoding.ASCII.GetString(entry.Data)}");
                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (UTF-8): {Encoding.UTF8.GetString(entry.Data)}");
                    //if (entry.Data != null)
                    //    builder.AppendLine($"{padding}Value (Unicode): {Encoding.Unicode.GetString(entry.Data)}");
                }
            }
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
        public IEnumerable<object> FindCodeViewDebugTableByPath(string path)
        {
            // Ensure that we have the debug data cached
            if (DebugData == null)
                return Enumerable.Empty<object>();

            var nb10Found = DebugData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.NB10ProgramDatabase)
                .Where(n => n != null)
                .Where(n => n.PdbFileName.Contains(path))
                .Select(n => (object)n);

            var rsdsFound = DebugData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.RSDSProgramDatabase)
                .Where(r => r != null)
                .Where(r => r.PathAndFileName.Contains(path))
                .Select(r => (object)r);

            return nb10Found.Concat(rsdsFound);
        }

        /// <summary>
        /// Find unparsed debug data by string value
        /// </summary>
        /// <param name="value">String value to check for</param>
        /// <returns>Enumerable of matching debug data</returns>
        public IEnumerable<byte[]> FindGenericDebugTableByValue(string value)
        {
            // Ensure that we have the resource data cached
            if (DebugData == null)
                return Enumerable.Empty<byte[]>();

            return DebugData.Select(r => r.Value)
                .Select(b => b as byte[])
                .Where(b => b != null)
                .Where(b =>
                {
                    try
                    {
                        string arrayAsASCII = Encoding.ASCII.GetString(b);
                        if (arrayAsASCII.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
                        string arrayAsUTF8 = Encoding.UTF8.GetString(b);
                        if (arrayAsUTF8.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
                        string arrayAsUnicode = Encoding.Unicode.GetString(b);
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
            // Loop through all debug table entries
            for (int i = 0; i < DebugTable.DebugDirectoryTable.Length; i++)
            {
                var entry = DebugTable.DebugDirectoryTable[i];

                uint address = entry.PointerToRawData;
                uint size = entry.SizeOfData;

                byte[] entryData = ReadFromDataSource((int)address, (int)size);
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
        public IEnumerable<SabreTools.Models.PortableExecutable.DialogBoxResource> FindDialogByTitle(string title)
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return Enumerable.Empty<SabreTools.Models.PortableExecutable.DialogBoxResource>();

            return ResourceData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.DialogBoxResource)
                .Where(d => d != null)
                .Where(d =>
                {
                    return (d.DialogTemplate?.TitleResource?.Contains(title) ?? false)
                        || (d.ExtendedDialogTemplate?.TitleResource?.Contains(title) ?? false);
                });
        }

        /// <summary>
        /// Find dialog box resources by contained item title
        /// </summary>
        /// <param name="title">Dialog box item title to check for</param>
        /// <returns>Enumerable of matching resources</returns>
        public IEnumerable<SabreTools.Models.PortableExecutable.DialogBoxResource> FindDialogBoxByItemTitle(string title)
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return Enumerable.Empty<SabreTools.Models.PortableExecutable.DialogBoxResource>();

            return ResourceData.Select(r => r.Value)
                .Select(r => r as SabreTools.Models.PortableExecutable.DialogBoxResource)
                .Where(d => d != null)
                .Where(d =>
                {
                    if (d.DialogItemTemplates != null)
                    {
                        return d.DialogItemTemplates
                            .Where(dit => dit?.TitleResource != null)
                            .Any(dit => dit.TitleResource.Contains(title));
                    }
                    else if (d.ExtendedDialogItemTemplates != null)
                    {
                        return d.ExtendedDialogItemTemplates
                            .Where(edit => edit?.TitleResource != null)
                            .Any(edit => edit.TitleResource.Contains(title));
                    }

                    return false;
                });
        }

        /// <summary>
        /// Find string table resources by contained string entry
        /// </summary>
        /// <param name="entry">String entry to check for</param>
        /// <returns>Enumerable of matching resources</returns>
        public IEnumerable<Dictionary<int, string>> FindStringTableByEntry(string entry)
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return Enumerable.Empty<Dictionary<int, string>>();

            return ResourceData.Select(r => r.Value)
                .Select(r => r as Dictionary<int, string>)
                .Where(st => st != null)
                .Where(st => st.Select(kvp => kvp.Value)
                    .Any(s => s.Contains(entry)));
        }

        /// <summary>
        /// Find unparsed resources by type name
        /// </summary>
        /// <param name="typeName">Type name to check for</param>
        /// <returns>Enumerable of matching resources</returns>
        public IEnumerable<byte[]> FindResourceByNamedType(string typeName)
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return Enumerable.Empty<byte[]>();

            return ResourceData.Where(kvp => kvp.Key.Contains(typeName))
                .Select(kvp => kvp.Value as byte[])
                .Where(b => b != null);
        }

        /// <summary>
        /// Find unparsed resources by string value
        /// </summary>
        /// <param name="value">String value to check for</param>
        /// <returns>Enumerable of matching resources</returns>
        public IEnumerable<byte[]> FindGenericResource(string value)
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return Enumerable.Empty<byte[]>();

            return ResourceData.Select(r => r.Value)
                .Select(r => r as byte[])
                .Where(b => b != null)
                .Where(b =>
                {
                    try
                    {
                        string arrayAsASCII = Encoding.ASCII.GetString(b);
                        if (arrayAsASCII.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
                        string arrayAsUTF8 = Encoding.UTF8.GetString(b);
                        if (arrayAsUTF8.Contains(value))
                            return true;
                    }
                    catch { }

                    try
                    {
                        string arrayAsUnicode = Encoding.Unicode.GetString(b);
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
            int totalEntries = table?.Entries?.Length ?? 0;
            for (int i = 0; i < totalEntries; i++)
            {
                var entry = table.Entries[i];
                var newTypes = new List<object>(types ?? new List<object>());

                if (entry.Name != null)
                    newTypes.Add(Encoding.UTF8.GetString(entry.Name.UnicodeString ?? new byte[0]));
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
            object value = entry.Data;

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
        public bool ContainsSection(string sectionName, bool exact = false)
        {
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
            // If we don't have an entry point
            if (OH_AddressOfEntryPoint.ConvertVirtualAddress(SectionTable) == 0)
                return -1;

            // Otherwise, find the section it exists within
            return OH_AddressOfEntryPoint.ContainingSectionIndex(SectionTable);
        }

        /// <summary>
        /// Get the first section based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public SabreTools.Models.PortableExecutable.SectionHeader GetFirstSection(string name, bool exact = false)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
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
        public SabreTools.Models.PortableExecutable.SectionHeader GetLastSection(string name, bool exact = false)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
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
        public SabreTools.Models.PortableExecutable.SectionHeader GetSection(int index)
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
        public byte[] GetFirstSectionData(string name, bool exact = false)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
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
        public byte[] GetLastSectionData(string name, bool exact = false)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
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
        public byte[] GetSectionData(int index)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            // Get the section data from the table
            var section = SectionTable[index];
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
                byte[] sectionData = ReadFromDataSource((int)address, (int)size);

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
        public List<string> GetFirstSectionStrings(string name, bool exact = false)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
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
        public List<string> GetLastSectionStrings(string name, bool exact = false)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
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
        public List<string> GetSectionStrings(int index)
        {
            // If we have no sections
            if (SectionTable == null || !SectionTable.Any())
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            // Get the section data from the table
            var section = SectionTable[index];
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
                List<string> sectionStringData = ReadStringsFromDataSource((int)address, (int)size);

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
        public byte[] GetTableData(int index)
        {
            // If the table doesn't exist
            if (index < 0 || index > 16)
                return null;

            // Get the virtual address and size from the entries
            uint virtualAddress = 0, size = 0;
            switch (index)
            {
                case 1:
                    virtualAddress = OH_ExportTable.VirtualAddress;
                    size = OH_ExportTable.Size;
                    break;
                case 2:
                    virtualAddress = OH_ImportTable.VirtualAddress;
                    size = OH_ImportTable.Size;
                    break;
                case 3:
                    virtualAddress = OH_ResourceTable.VirtualAddress;
                    size = OH_ResourceTable.Size;
                    break;
                case 4:
                    virtualAddress = OH_ExceptionTable.VirtualAddress;
                    size = OH_ExceptionTable.Size;
                    break;
                case 5:
                    virtualAddress = OH_CertificateTable.VirtualAddress;
                    size = OH_CertificateTable.Size;
                    break;
                case 6:
                    virtualAddress = OH_BaseRelocationTable.VirtualAddress;
                    size = OH_BaseRelocationTable.Size;
                    break;
                case 7:
                    virtualAddress = OH_Debug.VirtualAddress;
                    size = OH_Debug.Size;
                    break;
                case 8: // Architecture Table
                    virtualAddress = 0;
                    size = 0;
                    break;
                case 9:
                    virtualAddress = OH_GlobalPtr.VirtualAddress;
                    size = OH_GlobalPtr.Size;
                    break;
                case 10:
                    virtualAddress = OH_ThreadLocalStorageTable.VirtualAddress;
                    size = OH_ThreadLocalStorageTable.Size;
                    break;
                case 11:
                    virtualAddress = OH_LoadConfigTable.VirtualAddress;
                    size = OH_LoadConfigTable.Size;
                    break;
                case 12:
                    virtualAddress = OH_BoundImport.VirtualAddress;
                    size = OH_BoundImport.Size;
                    break;
                case 13:
                    virtualAddress = OH_ImportAddressTable.VirtualAddress;
                    size = OH_ImportAddressTable.Size;
                    break;
                case 14:
                    virtualAddress = OH_DelayImportDescriptor.VirtualAddress;
                    size = OH_DelayImportDescriptor.Size;
                    break;
                case 15:
                    virtualAddress = OH_CLRRuntimeHeader.VirtualAddress;
                    size = OH_CLRRuntimeHeader.Size;
                    break;
                case 16: // Reserved
                    virtualAddress = 0;
                    size = 0;
                    break;
            }

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
                byte[] tableData = ReadFromDataSource((int)address, (int)size);

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
        public List<string> GetTableStrings(int index)
        {
            // If the table doesn't exist
            if (index < 0 || index > 16)
                return null;

            // Get the virtual address and size from the entries
            uint virtualAddress = 0, size = 0;
            switch (index)
            {
                case 1:
                    virtualAddress = OH_ExportTable.VirtualAddress;
                    size = OH_ExportTable.Size;
                    break;
                case 2:
                    virtualAddress = OH_ImportTable.VirtualAddress;
                    size = OH_ImportTable.Size;
                    break;
                case 3:
                    virtualAddress = OH_ResourceTable.VirtualAddress;
                    size = OH_ResourceTable.Size;
                    break;
                case 4:
                    virtualAddress = OH_ExceptionTable.VirtualAddress;
                    size = OH_ExceptionTable.Size;
                    break;
                case 5:
                    virtualAddress = OH_CertificateTable.VirtualAddress;
                    size = OH_CertificateTable.Size;
                    break;
                case 6:
                    virtualAddress = OH_BaseRelocationTable.VirtualAddress;
                    size = OH_BaseRelocationTable.Size;
                    break;
                case 7:
                    virtualAddress = OH_Debug.VirtualAddress;
                    size = OH_Debug.Size;
                    break;
                case 8: // Architecture Table
                    virtualAddress = 0;
                    size = 0;
                    break;
                case 9:
                    virtualAddress = OH_GlobalPtr.VirtualAddress;
                    size = OH_GlobalPtr.Size;
                    break;
                case 10:
                    virtualAddress = OH_ThreadLocalStorageTable.VirtualAddress;
                    size = OH_ThreadLocalStorageTable.Size;
                    break;
                case 11:
                    virtualAddress = OH_LoadConfigTable.VirtualAddress;
                    size = OH_LoadConfigTable.Size;
                    break;
                case 12:
                    virtualAddress = OH_BoundImport.VirtualAddress;
                    size = OH_BoundImport.Size;
                    break;
                case 13:
                    virtualAddress = OH_ImportAddressTable.VirtualAddress;
                    size = OH_ImportAddressTable.Size;
                    break;
                case 14:
                    virtualAddress = OH_DelayImportDescriptor.VirtualAddress;
                    size = OH_DelayImportDescriptor.Size;
                    break;
                case 15:
                    virtualAddress = OH_CLRRuntimeHeader.VirtualAddress;
                    size = OH_CLRRuntimeHeader.Size;
                    break;
                case 16: // Reserved
                    virtualAddress = 0;
                    size = 0;
                    break;
            }

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
                List<string> tableStringData = ReadStringsFromDataSource((int)address, (int)size);

                // Cache and return the table string data, even if null
                _tableStringData[index] = tableStringData;
                return tableStringData;
            }
        }

        #endregion
    }
}