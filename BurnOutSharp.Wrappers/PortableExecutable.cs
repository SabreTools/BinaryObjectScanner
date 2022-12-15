using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using static BurnOutSharp.Builder.Extensions;

namespace BurnOutSharp.Wrappers
{
    public class PortableExecutable : WrapperBase
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
        public uint? OH_BaseOfData => _executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? (uint?)_executable.OptionalHeader.BaseOfData
            : null;

        #endregion

        #region Windows-Specific Fields

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.ImageBase_PE32"/>
        public ulong OH_ImageBase => _executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
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
        public ulong OH_SizeOfStackReserve => _executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.SizeOfStackReserve_PE32
            : _executable.OptionalHeader.SizeOfStackReserve_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfStackCommit_PE32"/>
        public ulong OH_SizeOfStackCommit => _executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.SizeOfStackCommit_PE32
            : _executable.OptionalHeader.SizeOfStackCommit_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapReserve_PE32"/>
        public ulong OH_SizeOfHeapReserve => _executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
            ? _executable.OptionalHeader.SizeOfHeapReserve_PE32
            : _executable.OptionalHeader.SizeOfHeapReserve_PE32Plus;

        /// <inheritdoc cref="Models.PortableExecutable.OptionalHeader.SizeOfHeapCommit_PE32"/>
        public ulong OH_SizeOfHeapCommit => _executable.OptionalHeader.Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
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

        /// <inheritdoc cref="Models.PortableExecutable.BaseRelocationTable"/>
        public Models.PortableExecutable.BaseRelocationBlock[] BaseRelocationTable => _executable.BaseRelocationTable;

        /// <inheritdoc cref="Models.PortableExecutable.DebugTable"/>
        public Models.PortableExecutable.DebugTable DebugTable => _executable.DebugTable;

        /// <inheritdoc cref="Models.PortableExecutable.ExportTable"/>
        public Models.PortableExecutable.ExportTable ExportTable => _executable.ExportTable;

        /// <inheritdoc cref="Models.PortableExecutable.ExportTable.ExportNameTable"/>
        public string[] ExportNameTable => _executable.ExportTable?.ExportNameTable?.Strings;

        /// <inheritdoc cref="Models.PortableExecutable.ImportTable"/>
        public Models.PortableExecutable.ImportTable ImportTable => _executable.ImportTable;

        /// <inheritdoc cref="Models.PortableExecutable.ImportTable.HintNameTable"/>
        public string[] ImportHintNameTable => _executable.ImportTable?.HintNameTable != null
            ? _executable.ImportTable.HintNameTable.Select(entry => entry.Name).ToArray()
            : null;

        /// <inheritdoc cref="Models.PortableExecutable.ResourceDirectoryTable"/>
        public Models.PortableExecutable.ResourceDirectoryTable ResourceDirectoryTable => _executable.ResourceDirectoryTable;

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
                        var certificateTable = _executable.OptionalHeader.CertificateTable;
                        int certificateTableAddress = (int)certificateTable.VirtualAddress.ConvertVirtualAddress(_executable.SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in _executable.SectionTable)
                    {
                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(_executable.SectionTable);
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
                        var certificateTable = _executable.OptionalHeader.CertificateTable;
                        int certificateTableAddress = (int)certificateTable.VirtualAddress.ConvertVirtualAddress(_executable.SectionTable);
                        if (certificateTableAddress != 0 && certificateTableAddress < endOfFile)
                            endOfFile = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in _executable.SectionTable)
                    {
                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(_executable.SectionTable);
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
                    _sectionNames = new string[_executable.SectionTable.Length];
                    for (int i = 0; i < _sectionNames.Length; i++)
                    {
                        var section = _executable.SectionTable[i];

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
                    int lengthOfStubExecutableData = (int)_executable.Stub.Header.NewExeHeaderAddr - endOfStubHeader;
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
        /// Internal representation of the executable
        /// </summary>
        private Models.PortableExecutable.Executable _executable;

        /// <summary>
        /// Header padding data, if it exists
        /// </summary>
        private byte[] _headerPaddingData = null;

        /// <summary>
        /// Header padding strings, if they exist
        /// </summary>
        private List<string> _headerPaddingStrings = null;

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
        private Models.PortableExecutable.VersionInfo _versionInfo = null;

        /// <summary>
        /// Cached assembly manifest data
        /// </summary>
        private Models.PortableExecutable.AssemblyManifest _assemblyManifest = null;

        /// <summary>
        /// Lock object for reading from the source
        /// </summary>
        private readonly object _sourceDataLock = new object();

        #endregion

        #region Constructors

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

            var wrapper = new PortableExecutable
            {
                _executable = executable,
                _dataSource = DataSource.ByteArray,
                _byteArrayData = data,
                _byteArrayOffset = offset,
            };
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

            var wrapper = new PortableExecutable
            {
                _executable = executable,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
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
        private Models.PortableExecutable.AssemblyManifest GetAssemblyManifest()
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

        /// <summary>
        /// Print all sections, including their start and end addresses
        /// </summary>
        public void PrintAllSections()
        {
            foreach (var section in SectionTable)
            {
                // TODO: Handle long section names with leading `/`
                string sectionName = Encoding.UTF8.GetString(section.Name);
                uint sectionAddr = section.PointerToRawData;
                uint sectionEnd = sectionAddr + section.VirtualSize;
                Console.WriteLine($"{sectionName}: {sectionAddr} -> {sectionEnd}");
            }
        }

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("Portable Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            // Stub
            PrintStubHeader();
            PrintStubExtendedHeader();

            // Header
            PrintCOFFFileHeader();
            PrintOptionalHeader();

            // Tables
            PrintSectionTable();
            PrintCOFFSymbolTable();
            PrintAttributeCertificateTable();
            PrintDelayLoadDirectoryTable();

            // Named Sections
            PrintBaseRelocationTable();
            PrintDebugTable();
            PrintExportTable();
            PrintImportTable();
            PrintResourceDirectoryTable();
        }

        /// <summary>
        /// Print stub header information
        /// </summary>
        private void PrintStubHeader()
        {
            Console.WriteLine("  MS-DOS Stub Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {BitConverter.ToString(Stub_Magic).Replace("-", string.Empty)}");
            Console.WriteLine($"  Last page bytes: {Stub_LastPageBytes}");
            Console.WriteLine($"  Pages: {Stub_Pages}");
            Console.WriteLine($"  Relocation items: {Stub_RelocationItems}");
            Console.WriteLine($"  Header paragraph size: {Stub_HeaderParagraphSize}");
            Console.WriteLine($"  Minimum extra paragraphs: {Stub_MinimumExtraParagraphs}");
            Console.WriteLine($"  Maximum extra paragraphs: {Stub_MaximumExtraParagraphs}");
            Console.WriteLine($"  Initial SS value: {Stub_InitialSSValue}");
            Console.WriteLine($"  Initial SP value: {Stub_InitialSPValue}");
            Console.WriteLine($"  Checksum: {Stub_Checksum}");
            Console.WriteLine($"  Initial IP value: {Stub_InitialIPValue}");
            Console.WriteLine($"  Initial CS value: {Stub_InitialCSValue}");
            Console.WriteLine($"  Relocation table address: {Stub_RelocationTableAddr}");
            Console.WriteLine($"  Overlay number: {Stub_OverlayNumber}");
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
            Console.WriteLine($"  OEM identifier: {Stub_OEMIdentifier}");
            Console.WriteLine($"  OEM information: {Stub_OEMInformation}");
            Console.WriteLine($"  Reserved words: {string.Join(", ", Stub_Reserved2)}");
            Console.WriteLine($"  New EXE header address: {Stub_NewExeHeaderAddr}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print COFF file header information
        /// </summary>
        private void PrintCOFFFileHeader()
        {
            Console.WriteLine("  COFF File Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {BitConverter.ToString(Signature).Replace("-", string.Empty)}");
            Console.WriteLine($"  Machine: {Machine}");
            Console.WriteLine($"  Number of sections: {NumberOfSections}");
            Console.WriteLine($"  Time/Date stamp: {TimeDateStamp}");
            Console.WriteLine($"  Pointer to symbol table: {PointerToSymbolTable}");
            Console.WriteLine($"  Number of symbols: {NumberOfSymbols}");
            Console.WriteLine($"  Size of optional header: {SizeOfOptionalHeader}");
            Console.WriteLine($"  Characteristics: {Characteristics}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print optional header information
        /// </summary>
        private void PrintOptionalHeader()
        {
            Console.WriteLine("  Optional Header Information:");
            Console.WriteLine("  -------------------------");
            if (SizeOfOptionalHeader == 0)
            {
                Console.WriteLine("  No optional header present");
            }
            else
            {
                Console.WriteLine($"  Magic: {OH_Magic}");
                Console.WriteLine($"  Major linker version: {OH_MajorLinkerVersion}");
                Console.WriteLine($"  Minor linker version: {OH_MinorLinkerVersion}");
                Console.WriteLine($"  Size of code section: {OH_SizeOfCode}");
                Console.WriteLine($"  Size of initialized data: {OH_SizeOfInitializedData}");
                Console.WriteLine($"  Size of uninitialized data: {OH_SizeOfUninitializedData}");
                Console.WriteLine($"  Address of entry point: {OH_AddressOfEntryPoint}");
                Console.WriteLine($"  Base of code: {OH_BaseOfCode}");
                if (OH_Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Base of data: {OH_BaseOfData}");

                Console.WriteLine($"  Image base: {OH_ImageBase}");
                Console.WriteLine($"  Section alignment: {OH_SectionAlignment}");
                Console.WriteLine($"  File alignment: {OH_FileAlignment}");
                Console.WriteLine($"  Major operating system version: {OH_MajorOperatingSystemVersion}");
                Console.WriteLine($"  Minor operating system version: {OH_MinorOperatingSystemVersion}");
                Console.WriteLine($"  Major image version: {OH_MajorImageVersion}");
                Console.WriteLine($"  Minor image version: {OH_MinorImageVersion}");
                Console.WriteLine($"  Major subsystem version: {OH_MajorSubsystemVersion}");
                Console.WriteLine($"  Minor subsystem version: {OH_MinorSubsystemVersion}");
                Console.WriteLine($"  Win32 version value: {OH_Win32VersionValue}");
                Console.WriteLine($"  Size of image: {OH_SizeOfImage}");
                Console.WriteLine($"  Size of headers: {OH_SizeOfHeaders}");
                Console.WriteLine($"  Checksum: {OH_CheckSum}");
                Console.WriteLine($"  Subsystem: {OH_Subsystem}");
                Console.WriteLine($"  DLL characteristics: {OH_DllCharacteristics}");
                Console.WriteLine($"  Size of stack reserve: {OH_SizeOfStackReserve}");
                Console.WriteLine($"  Size of stack commit: {OH_SizeOfStackCommit}");
                Console.WriteLine($"  Size of heap reserve: {OH_SizeOfHeapReserve}");
                Console.WriteLine($"  Size of heap commit: {OH_SizeOfHeapCommit}");
                Console.WriteLine($"  Loader flags: {OH_LoaderFlags}");
                Console.WriteLine($"  Number of data-directory entries: {OH_NumberOfRvaAndSizes}");

                if (OH_ExportTable != null)
                {
                    Console.WriteLine("    Export Table (1)");
                    Console.WriteLine($"      Virtual address: {OH_ExportTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_ExportTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_ExportTable.Size}");
                }
                if (OH_ImportTable != null)
                {
                    Console.WriteLine("    Import Table (2)");
                    Console.WriteLine($"      Virtual address: {OH_ImportTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_ImportTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_ImportTable.Size}");
                }
                if (OH_ResourceTable != null)
                {
                    Console.WriteLine("    Resource Table (3)");
                    Console.WriteLine($"      Virtual address: {OH_ResourceTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_ResourceTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_ResourceTable.Size}");
                }
                if (OH_ExceptionTable != null)
                {
                    Console.WriteLine("    Exception Table (4)");
                    Console.WriteLine($"      Virtual address: {OH_ExceptionTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_ExceptionTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_ExceptionTable.Size}");
                }
                if (OH_CertificateTable != null)
                {
                    Console.WriteLine("    Certificate Table (5)");
                    Console.WriteLine($"      Virtual address: {OH_CertificateTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_CertificateTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_CertificateTable.Size}");
                }
                if (OH_BaseRelocationTable != null)
                {
                    Console.WriteLine("    Base Relocation Table (6)");
                    Console.WriteLine($"      Virtual address: {OH_BaseRelocationTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_BaseRelocationTable.Size}");
                }
                if (OH_Debug != null)
                {
                    Console.WriteLine("    Debug Table (7)");
                    Console.WriteLine($"      Virtual address: {OH_Debug.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_Debug.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_Debug.Size}");
                }
                if (OH_NumberOfRvaAndSizes >= 8)
                {
                    Console.WriteLine("    Architecture Table (8)");
                    Console.WriteLine($"      Virtual address: 0");
                    Console.WriteLine($"      Physical address: 0");
                    Console.WriteLine($"      Size: 0");
                }
                if (OH_GlobalPtr != null)
                {
                    Console.WriteLine("    Global Pointer Register (9)");
                    Console.WriteLine($"      Virtual address: {OH_GlobalPtr.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_GlobalPtr.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_GlobalPtr.Size}");
                }
                if (OH_ThreadLocalStorageTable != null)
                {
                    Console.WriteLine("    Thread Local Storage (TLS) Table (10)");
                    Console.WriteLine($"      Virtual address: {OH_ThreadLocalStorageTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_ThreadLocalStorageTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_ThreadLocalStorageTable.Size}");
                }
                if (OH_LoadConfigTable != null)
                {
                    Console.WriteLine("    Load Config Table (11)");
                    Console.WriteLine($"      Virtual address: {OH_LoadConfigTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_LoadConfigTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_LoadConfigTable.Size}");
                }
                if (OH_BoundImport != null)
                {
                    Console.WriteLine("    Bound Import Table (12)");
                    Console.WriteLine($"      Virtual address: {OH_BoundImport.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_BoundImport.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_BoundImport.Size}");
                }
                if (OH_ImportAddressTable != null)
                {
                    Console.WriteLine("    Import Address Table (13)");
                    Console.WriteLine($"      Virtual address: {OH_ImportAddressTable.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_ImportAddressTable.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_ImportAddressTable.Size}");
                }
                if (OH_DelayImportDescriptor != null)
                {
                    Console.WriteLine("    Delay Import Descriptior (14)");
                    Console.WriteLine($"      Virtual address: {OH_DelayImportDescriptor.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_DelayImportDescriptor.Size}");
                }
                if (OH_CLRRuntimeHeader != null)
                {
                    Console.WriteLine("    CLR Runtime Header (15)");
                    Console.WriteLine($"      Virtual address: {OH_CLRRuntimeHeader.VirtualAddress}");
                    Console.WriteLine($"      Physical address: {OH_CLRRuntimeHeader.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"      Size: {OH_CLRRuntimeHeader.Size}");
                }
                if (OH_NumberOfRvaAndSizes >= 16)
                {
                    Console.WriteLine("    Reserved (16)");
                    Console.WriteLine($"      Virtual address: 0");
                    Console.WriteLine($"      Physical address: 0");
                    Console.WriteLine($"      Size: 0");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print section table information
        /// </summary>
        private void PrintSectionTable()
        {
            Console.WriteLine("  Section Table Information:");
            Console.WriteLine("  -------------------------");
            if (NumberOfSections == 0 || SectionTable.Length == 0)
            {
                Console.WriteLine("  No section table items");
            }
            else
            {
                for (int i = 0; i < SectionTable.Length; i++)
                {
                    var entry = SectionTable[i];
                    Console.WriteLine($"  Section Table Entry {i}");
                    Console.WriteLine($"    Name = {Encoding.UTF8.GetString(entry.Name)}");
                    Console.WriteLine($"    Virtual size = {entry.VirtualSize}");
                    Console.WriteLine($"    Virtual address = {entry.VirtualAddress}");
                    Console.WriteLine($"    Physical address: {entry.VirtualAddress.ConvertVirtualAddress(SectionTable)}");
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
        }

        /// <summary>
        /// Print COFF symbol table information
        /// </summary>
        private void PrintCOFFSymbolTable()
        {
            Console.WriteLine("  COFF Symbol Table Information:");
            Console.WriteLine("  -------------------------");
            if (PointerToSymbolTable == 0
                || NumberOfSymbols == 0
                || COFFSymbolTable.Length == 0)
            {
                Console.WriteLine("  No COFF symbol table items");
            }
            else
            {
                int auxSymbolsRemaining = 0;
                int currentSymbolType = 0;

                for (int i = 0; i < COFFSymbolTable.Length; i++)
                {
                    var entry = COFFSymbolTable[i];
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
                if (COFFStringTable == null
                    || COFFStringTable.Strings == null
                    || COFFStringTable.Strings.Length == 0)
                {
                    Console.WriteLine("  No COFF string table items");
                }
                else
                {
                    Console.WriteLine($"  Total size: {COFFStringTable.TotalSize}");
                    for (int i = 0; i < COFFStringTable.Strings.Length; i++)
                    {
                        string entry = COFFStringTable.Strings[i];
                        Console.WriteLine($"  COFF String Table Entry {i})");
                        Console.WriteLine($"    Value = {entry}");
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print attribute certificate table information
        /// </summary>
        private void PrintAttributeCertificateTable()
        {
            Console.WriteLine("  Attribute Certificate Table Information:");
            Console.WriteLine("  -------------------------");
            if (OH_CertificateTable == null
                || OH_CertificateTable.VirtualAddress == 0
                || AttributeCertificateTable.Length == 0)
            {
                Console.WriteLine("  No attribute certificate table items");
            }
            else
            {
                for (int i = 0; i < AttributeCertificateTable.Length; i++)
                {
                    var entry = AttributeCertificateTable[i];
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
                        try
                        {
                            Console.WriteLine($"    {BitConverter.ToString(entry.Certificate).Replace("-", string.Empty)}");
                        }
                        catch
                        {
                            Console.WriteLine($"    [DATA TOO LARGE TO FORMAT]");
                        }
                    }

                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print delay-load directory table information
        /// </summary>
        private void PrintDelayLoadDirectoryTable()
        {
            Console.WriteLine("  Delay-Load Directory Table Information:");
            Console.WriteLine("  -------------------------");
            if (OH_DelayImportDescriptor == null
                || OH_DelayImportDescriptor.VirtualAddress == 0
                || DelayLoadDirectoryTable == null)
            {
                Console.WriteLine("  No delay-load directory table items");
            }
            else
            {
                Console.WriteLine($"  Attributes = {DelayLoadDirectoryTable.Attributes}");
                Console.WriteLine($"  Name RVA = {DelayLoadDirectoryTable.Name}");
                Console.WriteLine($"  Module handle = {DelayLoadDirectoryTable.ModuleHandle}");
                Console.WriteLine($"  Delay import address table RVA = {DelayLoadDirectoryTable.DelayImportAddressTable}");
                Console.WriteLine($"  Delay import name table RVA = {DelayLoadDirectoryTable.DelayImportNameTable}");
                Console.WriteLine($"  Bound delay import table RVA = {DelayLoadDirectoryTable.BoundDelayImportTable}");
                Console.WriteLine($"  Unload delay import table RVA = {DelayLoadDirectoryTable.UnloadDelayImportTable}");
                Console.WriteLine($"  Timestamp = {DelayLoadDirectoryTable.TimeStamp}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print base relocation table information
        /// </summary>
        private void PrintBaseRelocationTable()
        {
            Console.WriteLine("  Base Relocation Table Information:");
            Console.WriteLine("  -------------------------");
            if (OH_BaseRelocationTable == null
                || OH_BaseRelocationTable.VirtualAddress == 0
                || BaseRelocationTable == null)
            {
                Console.WriteLine("  No base relocation table items");
            }
            else
            {
                for (int i = 0; i < BaseRelocationTable.Length; i++)
                {
                    var baseRelocationTableEntry = BaseRelocationTable[i];
                    Console.WriteLine($"  Base Relocation Table Entry {i}");
                    Console.WriteLine($"    Page RVA: {baseRelocationTableEntry.PageRVA}");
                    Console.WriteLine($"    Page physical address: {baseRelocationTableEntry.PageRVA.ConvertVirtualAddress(SectionTable)}");
                    Console.WriteLine($"    Block size: {baseRelocationTableEntry.BlockSize}");

                    Console.WriteLine($"    Base Relocation Table {i} Type and Offset Information:");
                    Console.WriteLine("    -------------------------");
                    if (baseRelocationTableEntry.TypeOffsetFieldEntries == null || baseRelocationTableEntry.TypeOffsetFieldEntries.Length == 0)
                    {
                        Console.WriteLine("    No base relocation table type and offset entries");
                    }
                    else
                    {
                        for (int j = 0; j < baseRelocationTableEntry.TypeOffsetFieldEntries.Length; j++)
                        {
                            var typeOffsetFieldEntry = baseRelocationTableEntry.TypeOffsetFieldEntries[j];
                            Console.WriteLine($"    Type and Offset Entry {j}");
                            Console.WriteLine($"      Type: {typeOffsetFieldEntry.BaseRelocationType}");
                            Console.WriteLine($"      Offset: {typeOffsetFieldEntry.Offset}");
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print debug table information
        /// </summary>
        private void PrintDebugTable()
        {
            Console.WriteLine("  Debug Table Information:");
            Console.WriteLine("  -------------------------");
            if (OH_Debug == null
                || OH_Debug.VirtualAddress == 0
                || DebugTable == null)
            {
                Console.WriteLine("  No debug table items");
            }
            else
            {
                // TODO: If more sections added, model this after the Export Table
                for (int i = 0; i < DebugTable.DebugDirectoryTable.Length; i++)
                {
                    var debugDirectoryEntry = DebugTable.DebugDirectoryTable[i];
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
        }

        /// <summary>
        /// Print export table information
        /// </summary>
        private void PrintExportTable()
        {
            Console.WriteLine("  Export Table Information:");
            Console.WriteLine("  -------------------------");
            if (OH_ExportTable == null
                || OH_ExportTable.VirtualAddress == 0
                || ExportTable == null)
            {
                Console.WriteLine("  No export table items");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("    Export Directory Table Information:");
                Console.WriteLine("    -------------------------");
                Console.WriteLine($"    Export flags: {ExportTable.ExportDirectoryTable.ExportFlags}");
                Console.WriteLine($"    Time/Date stamp: {ExportTable.ExportDirectoryTable.TimeDateStamp}");
                Console.WriteLine($"    Major version: {ExportTable.ExportDirectoryTable.MajorVersion}");
                Console.WriteLine($"    Minor version: {ExportTable.ExportDirectoryTable.MinorVersion}");
                Console.WriteLine($"    Name RVA: {ExportTable.ExportDirectoryTable.NameRVA}");
                Console.WriteLine($"    Name: {ExportTable.ExportDirectoryTable.Name}");
                Console.WriteLine($"    Ordinal base: {ExportTable.ExportDirectoryTable.OrdinalBase}");
                Console.WriteLine($"    Address table entries: {ExportTable.ExportDirectoryTable.AddressTableEntries}");
                Console.WriteLine($"    Number of name pointers: {ExportTable.ExportDirectoryTable.NumberOfNamePointers}");
                Console.WriteLine($"    Export address table RVA: {ExportTable.ExportDirectoryTable.ExportAddressTableRVA}");
                Console.WriteLine($"    Name pointer table RVA: {ExportTable.ExportDirectoryTable.NamePointerRVA}");
                Console.WriteLine($"    Ordinal table RVA: {ExportTable.ExportDirectoryTable.OrdinalTableRVA}");
                Console.WriteLine();

                Console.WriteLine("    Export Address Table Information:");
                Console.WriteLine("    -------------------------");
                if (ExportTable.ExportAddressTable == null || ExportTable.ExportAddressTable.Length == 0)
                {
                    Console.WriteLine("    No export address table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.ExportAddressTable.Length; i++)
                    {
                        var exportAddressTableEntry = ExportTable.ExportAddressTable[i];
                        Console.WriteLine($"    Export Address Table Entry {i}");
                        Console.WriteLine($"      Export RVA / Forwarder RVA: {exportAddressTableEntry.ExportRVA}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Name Pointer Table Information:");
                Console.WriteLine("    -------------------------");
                if (ExportTable.NamePointerTable?.Pointers == null || ExportTable.NamePointerTable.Pointers.Length == 0)
                {
                    Console.WriteLine("    No name pointer table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.NamePointerTable.Pointers.Length; i++)
                    {
                        var namePointerTableEntry = ExportTable.NamePointerTable.Pointers[i];
                        Console.WriteLine($"    Name Pointer Table Entry {i}");
                        Console.WriteLine($"      Pointer: {namePointerTableEntry}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Ordinal Table Information:");
                Console.WriteLine("    -------------------------");
                if (ExportTable.OrdinalTable?.Indexes == null || ExportTable.OrdinalTable.Indexes.Length == 0)
                {
                    Console.WriteLine("    No ordinal table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.OrdinalTable.Indexes.Length; i++)
                    {
                        var ordinalTableEntry = ExportTable.OrdinalTable.Indexes[i];
                        Console.WriteLine($"    Ordinal Table Entry {i}");
                        Console.WriteLine($"      Index: {ordinalTableEntry}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Export Name Table Information:");
                Console.WriteLine("    -------------------------");
                if (ExportTable.ExportNameTable?.Strings == null || ExportTable.ExportNameTable.Strings.Length == 0)
                {
                    Console.WriteLine("    No export name table items");
                }
                else
                {
                    for (int i = 0; i < ExportTable.ExportNameTable.Strings.Length; i++)
                    {
                        var exportNameTableEntry = ExportTable.ExportNameTable.Strings[i];
                        Console.WriteLine($"    Export Name Table Entry {i}");
                        Console.WriteLine($"      String: {exportNameTableEntry}");
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print import table information
        /// </summary>
        private void PrintImportTable()
        {
            Console.WriteLine("  Import Table Information:");
            Console.WriteLine("  -------------------------");
            if (OH_ImportTable == null
                || OH_ImportTable.VirtualAddress == 0
                || ImportTable == null)
            {
                Console.WriteLine("  No import table items");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("    Import Directory Table Information:");
                Console.WriteLine("    -------------------------");
                if (ImportTable.ImportDirectoryTable == null || ImportTable.ImportDirectoryTable.Length == 0)
                {
                    Console.WriteLine("    No import directory table items");
                }
                else
                {
                    for (int i = 0; i < ImportTable.ImportDirectoryTable.Length; i++)
                    {
                        var importDirectoryTableEntry = ImportTable.ImportDirectoryTable[i];
                        Console.WriteLine($"    Import Directory Table Entry {i}");
                        Console.WriteLine($"      Import lookup table RVA: {importDirectoryTableEntry.ImportLookupTableRVA}");
                        Console.WriteLine($"      Import lookup table Physical Address: {importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(SectionTable)}");
                        Console.WriteLine($"      Time/Date stamp: {importDirectoryTableEntry.TimeDateStamp}");
                        Console.WriteLine($"      Forwarder chain: {importDirectoryTableEntry.ForwarderChain}");
                        Console.WriteLine($"      Name RVA: {importDirectoryTableEntry.NameRVA}");
                        Console.WriteLine($"      Name: {importDirectoryTableEntry.Name}");
                        Console.WriteLine($"      Import address table RVA: {importDirectoryTableEntry.ImportAddressTableRVA}");
                        Console.WriteLine($"      Import address table Physical Address: {importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(SectionTable)}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Import Lookup Tables Information:");
                Console.WriteLine("    -------------------------");
                if (ImportTable.ImportLookupTables == null || ImportTable.ImportLookupTables.Count == 0)
                {
                    Console.WriteLine("    No import lookup tables");
                }
                else
                {
                    foreach (var kvp in ImportTable.ImportLookupTables)
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
                                {
                                    Console.WriteLine($"        Ordinal number: {importLookupTableEntry.OrdinalNumber}");
                                }
                                else
                                {
                                    Console.WriteLine($"        Hint/Name table RVA: {importLookupTableEntry.HintNameTableRVA}");
                                    Console.WriteLine($"        Hint/Name table Physical Address: {importLookupTableEntry.HintNameTableRVA.ConvertVirtualAddress(SectionTable)}");
                                }
                            }
                        }
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Import Address Tables Information:");
                Console.WriteLine("    -------------------------");
                if (ImportTable.ImportAddressTables == null || ImportTable.ImportAddressTables.Count == 0)
                {
                    Console.WriteLine("    No import address tables");
                }
                else
                {
                    foreach (var kvp in ImportTable.ImportAddressTables)
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
                                var importAddressTableEntry = importAddressTable[i];
                                Console.WriteLine($"      Import Address Table {index} Entry {i}");
                                Console.WriteLine($"        Ordinal/Name flag: {importAddressTableEntry.OrdinalNameFlag}");
                                if (importAddressTableEntry.OrdinalNameFlag)
                                {
                                    Console.WriteLine($"        Ordinal number: {importAddressTableEntry.OrdinalNumber}");
                                }
                                else
                                {
                                    Console.WriteLine($"        Hint/Name table RVA: {importAddressTableEntry.HintNameTableRVA}");
                                    Console.WriteLine($"        Hint/Name table Physical Address: {importAddressTableEntry.HintNameTableRVA.ConvertVirtualAddress(SectionTable)}");
                                }
                            }
                        }
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Hint/Name Table Information:");
                Console.WriteLine("    -------------------------");
                if (ImportTable.HintNameTable == null || ImportTable.HintNameTable.Length == 0)
                {
                    Console.WriteLine("    No hint/name table items");
                }
                else
                {
                    for (int i = 0; i < ImportTable.HintNameTable.Length; i++)
                    {
                        var hintNameTableEntry = ImportTable.HintNameTable[i];
                        Console.WriteLine($"    Hint/Name Table Entry {i}");
                        Console.WriteLine($"      Hint: {hintNameTableEntry.Hint}");
                        Console.WriteLine($"      Name: {hintNameTableEntry.Name}");
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print resource directory table information
        /// </summary>
        private void PrintResourceDirectoryTable()
        {
            Console.WriteLine("  Resource Directory Table Information:");
            Console.WriteLine("  -------------------------");
            if (OH_ResourceTable == null
                || OH_ResourceTable.VirtualAddress == 0
                || ResourceDirectoryTable == null)
            {
                Console.WriteLine("  No resource directory table items");
            }
            else
            {
                PrintResourceDirectoryTable(ResourceDirectoryTable, level: 0, types: new List<object>());
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Pretty print the resource directory table information
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

            Console.WriteLine($"{padding}Entries");
            Console.WriteLine($"{padding}-------------------------");
            if (table.NumberOfNameEntries == 0 && table.NumberOfIDEntries == 0)
            {
                Console.WriteLine($"{padding}No entries");
                Console.WriteLine();
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

                    PrintResourceDirectoryEntry(entry, level + 1, newTypes);
                }
            }
        }

        /// <summary>
        /// Pretty print the resource directory entry information
        /// </summary>
        private static void PrintResourceDirectoryEntry(Models.PortableExecutable.ResourceDirectoryEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            Console.WriteLine($"{padding}Item level: {level}");
            if (entry.NameOffset != default)
            {
                Console.WriteLine($"{padding}Name offset: {entry.NameOffset}");
                Console.WriteLine($"{padding}Name ({entry.Name.Length}): {Encoding.UTF8.GetString(entry.Name.UnicodeString ?? new byte[0])}");
            }
            else
            {
                Console.WriteLine($"{padding}Integer ID: {entry.IntegerID}");
            }

            if (entry.DataEntry != null)
                PrintResourceDataEntry(entry.DataEntry, level: level + 1, types);
            else if (entry.Subdirectory != null)
                PrintResourceDirectoryTable(entry.Subdirectory, level: level + 1, types);
        }

        /// <summary>
        /// Pretty print the resource data entry information
        /// </summary>
        private static void PrintResourceDataEntry(Models.PortableExecutable.ResourceDataEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            // TODO: Use ordered list of base types to determine the shape of the data
            Console.WriteLine($"{padding}Base types: {string.Join(", ", types)}");

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
                        PrintResourceRT_CURSOR(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_BITMAP:
                        PrintResourceRT_BITMAP(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ICON:
                        PrintResourceRT_ICON(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_MENU:
                        PrintResourceRT_MENU(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_DIALOG:
                        PrintResourceRT_DIALOG(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_STRING:
                        PrintResourceRT_STRING(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_FONTDIR:
                        PrintResourceRT_FONTDIR(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_FONT:
                        PrintResourceRT_FONT(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ACCELERATOR:
                        PrintResourceRT_ACCELERATOR(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_RCDATA:
                        PrintResourceRT_RCDATA(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_MESSAGETABLE:
                        PrintResourceRT_MESSAGETABLE(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_GROUP_CURSOR:
                        PrintResourceRT_GROUP_CURSOR(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_GROUP_ICON:
                        PrintResourceRT_GROUP_ICON(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_VERSION:
                        PrintResourceRT_VERSION(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_DLGINCLUDE:
                        PrintResourceRT_DLGINCLUDE(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_PLUGPLAY:
                        PrintResourceRT_PLUGPLAY(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_VXD:
                        PrintResourceRT_VXD(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ANICURSOR:
                        PrintResourceRT_ANICURSOR(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_ANIICON:
                        PrintResourceRT_ANIICON(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_HTML:
                        PrintResourceRT_HTML(entry, level);
                        break;
                    case Models.PortableExecutable.ResourceType.RT_MANIFEST:
                        PrintResourceRT_MANIFEST(entry, level);
                        break;
                    default:
                        PrintResourceUNKNOWN(entry, level, types[0]);
                        break;
                }
            }
            else if (types != null && types.Count > 0 && types[0] is string resourceString)
            {
                PrintResourceUNKNOWN(entry, level, types[0]);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print an RT_CURSOR resource
        /// </summary>
        private static void PrintResourceRT_CURSOR(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Hardware-dependent cursor resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_BITMAP resource
        /// </summary>
        private static void PrintResourceRT_BITMAP(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Bitmap resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ICON resource
        /// </summary>
        private static void PrintResourceRT_ICON(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Hardware-dependent icon resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_MENU resource
        /// </summary>
        private static void PrintResourceRT_MENU(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);

            Models.PortableExecutable.MenuResource menu = null;
            try { menu = entry.AsMenu(); } catch { }
            if (menu == null)
            {
                Console.WriteLine($"{padding}Menu resource found, but malformed");
                return;
            }

            if (menu.MenuHeader != null)
            {
                Console.WriteLine($"{padding}Version: {menu.MenuHeader.Version}");
                Console.WriteLine($"{padding}Header size: {menu.MenuHeader.HeaderSize}");
                Console.WriteLine();
                Console.WriteLine($"{padding}Menu items");
                Console.WriteLine($"{padding}-------------------------");
                if (menu.MenuItems == null || menu.MenuItems.Length == 0)
                {
                    Console.WriteLine($"{padding}No menu items");
                    return;
                }

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
                    return;
                }

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
            else
            {
                Console.WriteLine($"{padding}Menu resource found, but malformed");
            }
        }

        /// <summary>
        /// Print an RT_DIALOG resource
        /// </summary>
        private static void PrintResourceRT_DIALOG(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);

            Models.PortableExecutable.DialogBoxResource dialogBox = null;
            try { dialogBox = entry.AsDialogBox(); } catch { }
            if (dialogBox == null)
            {
                Console.WriteLine($"{padding}Dialog box resource found, but malformed");
                return;
            }

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
                    return;
                }

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
                    return;
                }

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
            else
            {
                Console.WriteLine($"{padding}Dialog box resource found, but malformed");
            }
        }

        /// <summary>
        /// Print an RT_STRING resource
        /// </summary>
        private static void PrintResourceRT_STRING(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);

            Dictionary<int, string> stringTable = null;
            try { stringTable = entry.AsStringTable(); } catch { }
            if (stringTable == null)
            {
                Console.WriteLine($"{padding}String table resource found, but malformed");
                return;
            }

            foreach (var kvp in stringTable)
            {
                int index = kvp.Key;
                string stringValue = kvp.Value;
                Console.WriteLine($"{padding}String entry {index}: {stringValue}");
            }
        }

        /// <summary>
        /// Print an RT_FONTDIR resource
        /// </summary>
        private static void PrintResourceRT_FONTDIR(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Font directory resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_FONT resource
        /// </summary>
        private static void PrintResourceRT_FONT(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Font resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ACCELERATOR resource
        /// </summary>
        private static void PrintResourceRT_ACCELERATOR(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);

            Models.PortableExecutable.AcceleratorTableEntry[] acceleratorTable = null;
            try { acceleratorTable = entry.AsAcceleratorTableResource(); } catch { }
            if (acceleratorTable == null)
            {
                Console.WriteLine($"{padding}Accelerator table resource found, but malformed");
                return;
            }

            for (int i = 0; i < acceleratorTable.Length; i++)
            {
                var acceleratorTableEntry = acceleratorTable[i];
                Console.WriteLine($"{padding}Accelerator Table Entry {i}:");
                Console.WriteLine($"{padding}  Flags: {acceleratorTableEntry.Flags}");
                Console.WriteLine($"{padding}  Ansi: {acceleratorTableEntry.Ansi}");
                Console.WriteLine($"{padding}  Id: {acceleratorTableEntry.Id}");
                Console.WriteLine($"{padding}  Padding: {acceleratorTableEntry.Padding}");
            }
        }

        /// <summary>
        /// Print an RT_RCDATA resource
        /// </summary>
        private static void PrintResourceRT_RCDATA(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Application-defined resource found, not parsed yet");

            // Then print the data, if needed
            if (entry.Data[0] == 0x4D && entry.Data[1] == 0x5A)
            {
                Console.WriteLine($"{padding}Data: [Embedded Executable File]"); // TODO: Parse this out and print separately
            }
            else if (entry.Data[0] == 0x4D && entry.Data[1] == 0x53 && entry.Data[2] == 0x46 && entry.Data[3] == 0x54)
            {
                Console.WriteLine($"{padding}Data: [Embedded OLE Library File]"); // TODO: Parse this out and print separately
            }
            else
            {
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (Byte Data): {BitConverter.ToString(entry.Data).Replace('-', ' ')}");
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (ASCII): {Encoding.ASCII.GetString(entry.Data)}");
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (UTF-8): {Encoding.UTF8.GetString(entry.Data)}");
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (Unicode): {Encoding.Unicode.GetString(entry.Data)}");
            }
        }

        /// <summary>
        /// Print an RT_MESSAGETABLE resource
        /// </summary>
        private static void PrintResourceRT_MESSAGETABLE(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);

            Models.PortableExecutable.MessageResourceData messageTable = null;
            try { messageTable = entry.AsMessageResourceData(); } catch { }
            if (messageTable == null)
            {
                Console.WriteLine($"{padding}Message resource data found, but malformed");
                return;
            }

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

        /// <summary>
        /// Print an RT_GROUP_CURSOR resource
        /// </summary>
        private static void PrintResourceRT_GROUP_CURSOR(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Hardware-independent cursor resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_GROUP_ICON resource
        /// </summary>
        private static void PrintResourceRT_GROUP_ICON(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Hardware-independent icon resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_VERSION resource
        /// </summary>
        private static void PrintResourceRT_VERSION(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);

            Models.PortableExecutable.VersionInfo versionInfo = null;
            try { versionInfo = entry.AsVersionInfo(); } catch { }
            if (versionInfo == null)
            {
                Console.WriteLine($"{padding}Version info resource found, but malformed");
                return;
            }

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

        /// <summary>
        /// Print an RT_DLGINCLUDE resource
        /// </summary>
        private static void PrintResourceRT_DLGINCLUDE(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}External header resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_PLUGPLAY resource
        /// </summary>
        private static void PrintResourceRT_PLUGPLAY(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Plug and Play resource found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_VXD resource
        /// </summary>
        private static void PrintResourceRT_VXD(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}VXD found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ANICURSOR resource
        /// </summary>
        private static void PrintResourceRT_ANICURSOR(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Animated cursor found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_ANIICON resource
        /// </summary>
        private static void PrintResourceRT_ANIICON(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}Animated icon found, not parsed yet");
        }

        /// <summary>
        /// Print an RT_HTML resource
        /// </summary>
        private static void PrintResourceRT_HTML(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);
            Console.WriteLine($"{padding}HTML resource found, not parsed yet");

            //if (entry.Data != null)
            //    Console.WriteLine($"{padding}Value (ASCII): {Encoding.ASCII.GetString(entry.Data)}");
            //if (entry.Data != null)
            //    Console.WriteLine($"{padding}Value (UTF-8): {Encoding.UTF8.GetString(entry.Data)}");
            //if (entry.Data != null)
            //    Console.WriteLine($"{padding}Value (Unicode): {Encoding.Unicode.GetString(entry.Data)}");
        }

        /// <summary>
        /// Print an RT_MANIFEST resource
        /// </summary>
        private static void PrintResourceRT_MANIFEST(Models.PortableExecutable.ResourceDataEntry entry, int level)
        {
            string padding = new string(' ', (level + 1) * 2);

            Models.PortableExecutable.AssemblyManifest assemblyManifest = null;
            try { assemblyManifest = entry.AsAssemblyManifest(); } catch { }
            if (assemblyManifest == null)
            {
                Console.WriteLine($"{padding}Assembly manifest found, but malformed");
                return;
            }

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
                Console.WriteLine($"{padding}[Assembly Description] Value: {assemblyManifest.Description.Value}");

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

        /// <summary>
        /// Print an UNKNOWN or custom resource
        /// </summary>
        private static void PrintResourceUNKNOWN(Models.PortableExecutable.ResourceDataEntry entry, int level, object resourceType)
        {
            string padding = new string(' ', (level + 1) * 2);

            // Print the type first
            if (resourceType is uint numericType)
                Console.WriteLine($"{padding}Type {(Models.PortableExecutable.ResourceType)numericType} found, not parsed yet");
            else if (resourceType is string stringType)
                Console.WriteLine($"{padding}Type {stringType} found, not parsed yet");
            else
                Console.WriteLine($"{padding}Unknown type {resourceType} found, not parsed yet");

            // Then print the data, if needed
            if (entry.Data[0] == 0x4D && entry.Data[1] == 0x5A)
            {
                Console.WriteLine($"{padding}Data: [Embedded Executable File]"); // TODO: Parse this out and print separately
            }
            else if (entry.Data[0] == 0x4D && entry.Data[1] == 0x53 && entry.Data[2] == 0x46 && entry.Data[3] == 0x54)
            {
                Console.WriteLine($"{padding}Data: [Embedded OLE Library File]"); // TODO: Parse this out and print separately
            }
            else
            {
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (Byte Data): {BitConverter.ToString(entry.Data).Replace('-', ' ')}");
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (ASCII): {Encoding.ASCII.GetString(entry.Data)}");
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (UTF-8): {Encoding.UTF8.GetString(entry.Data)}");
                //if (entry.Data != null)
                //    Console.WriteLine($"{padding}Value (Unicode): {Encoding.Unicode.GetString(entry.Data)}");
            }
        }

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
                .Select(r => r as Models.PortableExecutable.NB10ProgramDatabase)
                .Where(n => n != null)
                .Where(n => n.PdbFileName.Contains(path))
                .Select(n => (object)n);

            var rsdsFound = DebugData.Select(r => r.Value)
                .Select(r => r as Models.PortableExecutable.RSDSProgramDatabase)
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
                
                // If we have CodeView debug data, try to parse it
                if (entry.DebugType == Models.PortableExecutable.DebugType.IMAGE_DEBUG_TYPE_CODEVIEW)
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
        public IEnumerable<Models.PortableExecutable.DialogBoxResource> FindDialogByTitle(string title)
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return Enumerable.Empty<Models.PortableExecutable.DialogBoxResource>();

            return ResourceData.Select(r => r.Value)
                .Select(r => r as Models.PortableExecutable.DialogBoxResource)
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
        public IEnumerable<Models.PortableExecutable.DialogBoxResource> FindDialogBoxByItemTitle(string title)
        {
            // Ensure that we have the resource data cached
            if (ResourceData == null)
                return Enumerable.Empty<Models.PortableExecutable.DialogBoxResource>();

            return ResourceData.Select(r => r.Value)
                .Select(r => r as Models.PortableExecutable.DialogBoxResource)
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
        private void ParseResourceDirectoryTable(Models.PortableExecutable.ResourceDirectoryTable table, List<object> types)
        {
            int totalEntries = table.NumberOfNameEntries + table.NumberOfIDEntries;
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
        private void ParseResourceDirectoryEntry(Models.PortableExecutable.ResourceDirectoryEntry entry, List<object> types)
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
        private void ParseResourceDataEntry(Models.PortableExecutable.ResourceDataEntry entry, List<object> types)
        {
            // Create the key and value objects
            string key = types == null ? $"UNKNOWN_{Guid.NewGuid()}" : string.Join(", ", types);
            object value = entry.Data;

            // If we have a known resource type
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                try
                {
                    switch ((Models.PortableExecutable.ResourceType)resourceType)
                    {
                        case Models.PortableExecutable.ResourceType.RT_CURSOR:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_BITMAP:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_ICON:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_MENU:
                            value = entry.AsMenu();
                            break;
                        case Models.PortableExecutable.ResourceType.RT_DIALOG:
                            value = entry.AsDialogBox();
                            break;
                        case Models.PortableExecutable.ResourceType.RT_STRING:
                            value = entry.AsStringTable();
                            break;
                        case Models.PortableExecutable.ResourceType.RT_FONTDIR:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_FONT:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_ACCELERATOR:
                            value = entry.AsAcceleratorTableResource();
                            break;
                        case Models.PortableExecutable.ResourceType.RT_RCDATA:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_MESSAGETABLE:
                            value = entry.AsMessageResourceData();
                            break;
                        case Models.PortableExecutable.ResourceType.RT_GROUP_CURSOR:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_GROUP_ICON:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_VERSION:
                            _versionInfo = entry.AsVersionInfo();
                            value = _versionInfo;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_DLGINCLUDE:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_PLUGPLAY:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_VXD:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_ANICURSOR:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_ANIICON:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_HTML:
                            value = entry.Data;
                            break;
                        case Models.PortableExecutable.ResourceType.RT_MANIFEST:
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
        /// Get the first section based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public Models.PortableExecutable.SectionHeader GetFirstSection(string name, bool exact = false)
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
        public Models.PortableExecutable.SectionHeader GetLastSection(string name, bool exact = false)
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
        public Models.PortableExecutable.SectionHeader GetSection(int index)
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

            lock(_sourceDataLock)
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