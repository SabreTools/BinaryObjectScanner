using System;
using System.IO;
using System.Linq;
using System.Xml;
using BurnOutSharp.ExecutableType.Microsoft.MZ.Headers;
using BurnOutSharp.ExecutableType.Microsoft.PE.Entries;
using BurnOutSharp.ExecutableType.Microsoft.PE.Headers;
using BurnOutSharp.ExecutableType.Microsoft.PE.Sections;
using BurnOutSharp.ExecutableType.Microsoft.PE.Tables;
using BurnOutSharp.ExecutableType.Microsoft.Resources;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.PE
{
    /// <summary>
    /// The PE file header consists of a Microsoft MS-DOS stub, the PE signature, the COFF file header, and an optional header.
    /// A COFF object file header consists of a COFF file header and an optional header.
    /// In both cases, the file headers are followed immediately by section headers.
    /// </summary>
    public class PortableExecutable
    {
        /// <summary>
        /// Value determining if the executable is initialized or not
        /// </summary>
        public bool Initialized { get; } = false;

        /// <summary>
        /// Source array that the executable was parsed from
        /// </summary>
        private readonly byte[] _sourceArray = null;

        /// <summary>
        /// Source stream that the executable was parsed from
        /// </summary>
        private readonly Stream _sourceStream = null;

        #region Headers

        /// <summary>
        /// The MS-DOS stub is a valid application that runs under MS-DOS.
        /// It is placed at the front of the EXE image.
        /// The linker places a default stub here, which prints out the message "This program cannot be run in DOS mode" when the image is run in MS-DOS.
        /// The user can specify a different stub by using the /STUB linker option.
        /// At location 0x3c, the stub has the file offset to the PE signature.
        /// This information enables Windows to properly execute the image file, even though it has an MS-DOS stub.
        /// This file offset is placed at location 0x3c during linking.
        /// </summary>
        public MSDOSExecutableHeader DOSStubHeader;

        /// <summary>
        /// At the beginning of an object file, or immediately after the signature of an image file, is a standard COFF file header in the following format.
        /// Note that the Windows loader limits the number of sections to 96.
        /// </summary>
        public CommonObjectFileFormatHeader ImageFileHeader;

        /// <summary>
        /// Every image file has an optional header that provides information to the loader.
        /// This header is optional in the sense that some files (specifically, object files) do not have it.
        /// For image files, this header is required.
        /// An object file can have an optional header, but generally this header has no function in an object file except to increase its size.
        /// </summary>
        public OptionalHeader OptionalHeader;

        /// <summary>
        /// Each row of the section table is, in effect, a section header.
        /// This table immediately follows the optional header, if any.
        /// This positioning is required because the file header does not contain a direct pointer to the section table.
        /// Instead, the location of the section table is determined by calculating the location of the first byte after the headers.
        /// Make sure to use the size of the optional header as specified in the file header.
        /// </summary>
        public SectionHeader[] SectionTable;

        #endregion

        #region Tables

        /// <summary>
        /// The .debug section is used in object files to contain compiler-generated debug information and in image files to contain
        /// all of the debug information that is generated.
        /// This section describes the packaging of debug information in object and image files.
        /// </summary>
        public DebugSection DebugDirectory;

        /// <summary>
        /// The export data section, named .edata, contains information about symbols that other images can access through dynamic linking.
        /// Exported symbols are generally found in DLLs, but DLLs can also import symbols.
        /// </summary>
        public ExportDataSection ExportTable;

        /// <summary>
        /// All image files that import symbols, including virtually all executable (EXE) files, have an .idata section.
        /// </summary>
        public ImportDataSection ImportTable;

        /// <summary>
        /// The base relocation table contains entries for all base relocations in the image.
        /// The Base Relocation Table field in the optional header data directories gives the number of bytes in the base relocation table.
        /// </summary>
        public RelocationSection RelocationTable;

        /// <summary>
        /// Resources are indexed by a multiple-level binary-sorted tree structure.
        /// The general design can incorporate 2**31 levels.
        /// By convention, however, Windows uses three levels
        /// </summary>
        public ResourceSection ResourceSection;

        // TODO: Add more and more parts of a standard PE executable, not just the header
        // TODO: Add data directory table information here instead of in IMAGE_OPTIONAL_HEADER

        #endregion

        #region Raw Section Data

        // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#special-sections
        // Here is a list of standard sections that are used in various protections:
        // Y        - .bss          *1 protection       Uninitialized data (free format)
        // X        - .data         14 protections      Initialized data (free format)
        // X        - .edata        *1 protection       Export tables
        // X        - .idata        *1 protection       Import tables
        // X        - .rdata        11 protections      Read-only initialized data
        //          - .rsrc         *1 protection       Resource directory [TODO: Mostly taken care of, last protection needs research]
        // X        - .text         6 protections       Executable code (free format)
        // Y        - .tls          *1 protection       Thread-local storage (object only)
        // 
        // Here is a list of non-standard sections whose contents are read by various protections:
        // X        - CODE          2 protections       SafeDisc, WTM CD Protect
        // X        - .dcrtext      *1 protection       JoWood
        // X        - .grand        *1 protection       CD-Cops / DVD-Cops
        // X        - .init         *1 protection       SolidShield
        //          - .pec2         *1 protection       PE Compact [Unconfirmed]
        //          - .NOS0         *1 protection        UPX (NOS Variant)
        //          - .NOS1         *1 protection        UPX (NOS Variant)
        // X        - .txt2         *1 protection       SafeDisc
        //          - .UPX0         *1 protection        UPX
        //          - .UPX1         *1 protection        UPX
        // 
        // Here is a list of non-standard sections whose data is not read by various protections:
        //          - .brick        1 protection        StarForce
        //          - .cenega       1 protection        Cenega ProtectDVD
        //          - .ext          1 protection        JoWood
        //          - HC09          1 protection        JoWood
        //          - .icd*         1 protection        CodeLock
        //          - .ldr          1 protection        3PLock
        //          - .ldt          1 protection        3PLock
        //          - .nicode       1 protection        Armadillo
        //          - .pec1         1 protection        PE Compact
        //          - .securom      1 protection        SecuROM
        //          - .sforce       1 protection        StarForce
        //          - stxt371       1 protection        SafeDisc
        //          - stxt774       1 protection        SafeDisc
        //          - .vob.pcd      1 protection        VOB ProtectCD
        //          - _winzip_      1 protection        WinZip SFX
        //          - XPROT         1 protection        JoWood
        //
        // *    => Only used by 1 protection so it may be read in by that protection specifically

        /// <summary>
        /// .data/DATA - Initialized data (free format)
        /// </summary>
        public byte[] DataSectionRaw;

        /// <summary>
        /// .edata - Export tables
        /// </summary>
        /// <remarks>Replace with ExportDataSection</remarks>
        public byte[] ExportDataSectionRaw;

        /// <summary>
        /// .idata - Import tables
        /// </summary>
        /// <remarks>Replace with ImportDataSection</remarks>
        public byte[] ImportDataSectionRaw;

        /// <summary>
        /// .rdata - Read-only initialized data
        /// </summary>
        public byte[] ResourceDataSectionRaw;

        /// <summary>
        /// .text - Executable code (free format)
        /// </summary>
        public byte[] TextSectionRaw;

        #endregion

        #region Raw Other Data

        /// <summary>
        /// Data at the entry point of the application
        /// </summary>
        public byte[] EntryPointRaw;

        /// <summary>
        /// Data from the overlay of the application
        /// </summary>
        public byte[] OverlayRaw;

        #endregion

        #region Resources

        /// <summary>
        /// Company name resource string
        /// </summary>
        public string CompanyName { get; private set; }

        /// <summary>
        /// File description resource string
        /// </summary>
        public string FileDescription { get; private set; }

        /// <summary>
        /// File version resource string
        /// </summary>
        public string FileVersion { get; private set; }

        /// <summary>
        /// Internal name resource string
        /// </summary>
        public string InternalName { get; private set; }

        /// <summary>
        /// Legal copyright resource string
        /// </summary>
        public string LegalCopyright { get; private set; }

        /// <summary>
        /// Legal trademarks resource string
        /// </summary>
        public string LegalTrademarks { get; private set; }

        /// <summary>
        /// Description manifest string
        /// </summary>
        public string ManifestDescription { get; private set; }

        /// <summary>
        /// Version manifest string
        /// </summary>
        public string ManifestVersion { get; private set; }

        /// <summary>
        /// Original filename resource string
        /// </summary>
        public string OriginalFileName { get; private set; }

        /// <summary>
        /// Product name resource string
        /// </summary>
        public string ProductName { get; private set; }

        /// <summary>
        /// Product version resource string
        /// </summary>
        public string ProductVersion { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a PortableExecutable object from a stream
        /// </summary>
        /// <param name="stream">Stream representing a file</param>
        /// <remarks>
        /// This constructor assumes that the stream is already in the correct position to start parsing
        /// </remarks>
        public PortableExecutable(Stream stream)
        {
            if (stream == null || !stream.CanRead || !stream.CanSeek)
                return;

            this._sourceStream = stream;
            this.Initialized = Deserialize(stream);
        }

        /// <summary>
        /// Create a PortableExecutable object from a byte array
        /// </summary>
        /// <param name="fileContent">Byte array representing a file</param>
        /// <param name="offset">Positive offset representing the current position in the array</param>
        public PortableExecutable(byte[] fileContent, int offset)
        {
            if (fileContent == null || fileContent.Length == 0 || offset < 0)
                return;

            this._sourceArray = fileContent;
            this.Initialized = Deserialize(fileContent, offset);
        }

        /// <summary>
        /// Deserialize a PortableExecutable object from a stream
        /// </summary>
        /// <param name="stream">Stream representing a file</param>
        private bool Deserialize(Stream stream)
        {
            try
            {
                // Attempt to read the DOS header first
                this.DOSStubHeader = MSDOSExecutableHeader.Deserialize(stream); stream.Seek(this.DOSStubHeader.NewExeHeaderAddr, SeekOrigin.Begin);
                if (this.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                    return false;
                
                // If the new header address is invalid for the file, it's not a PE
                if (this.DOSStubHeader.NewExeHeaderAddr >= stream.Length)
                    return false;

                // Then attempt to read the PE header
                this.ImageFileHeader = CommonObjectFileFormatHeader.Deserialize(stream);
                if (this.ImageFileHeader.Signature != Constants.IMAGE_NT_SIGNATURE)
                    return false;

                // If the optional header is supposed to exist, read that as well
                if (this.ImageFileHeader.SizeOfOptionalHeader > 0)
                    this.OptionalHeader = OptionalHeader.Deserialize(stream);
                
                // Then read in the section table
                this.SectionTable = new SectionHeader[this.ImageFileHeader.NumberOfSections];
                for (int i = 0; i < this.ImageFileHeader.NumberOfSections; i++)
                {
                    this.SectionTable[i] = SectionHeader.Deserialize(stream);
                }

                #region Structured Tables

                // // Debug Section
                // var table = this.GetLastSection(".debug", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     stream.Seek((int)table.PointerToRawData, SeekOrigin.Begin);
                //     this.DebugSection = DebugSection.Deserialize(stream, this.SectionTable);
                // }

                // // Export Table
                // var table = this.GetLastSection(".edata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     stream.Seek((int)table.PointerToRawData, SeekOrigin.Begin);
                //     this.ExportTable = ExportDataSection.Deserialize(stream, this.SectionTable);
                // }

                // // Import Table
                // table = this.GetSection(".idata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     stream.Seek((int)table.PointerToRawData, SeekOrigin.Begin);
                //     this.ImportTable = ImportDataSection.Deserialize(stream, this.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0);
                // }

                // // Relocation Section
                // var table = this.GetLastSection(".reloc", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     stream.Seek((int)table.PointerToRawData, SeekOrigin.Begin);
                //     this.RelocationTable = RelocationSection.Deserialize(stream);
                // }

                // Resource Table
                var table = this.GetLastSection(".rsrc", true);
                if (table != null && table.VirtualSize > 0)
                {
                    stream.Seek((int)table.PointerToRawData, SeekOrigin.Begin);
                    this.ResourceSection = ResourceSection.Deserialize(stream, this.SectionTable);
                }

                #endregion

                #region Freeform Sections

                // Data Section
                this.DataSectionRaw = this.ReadRawSection(".data", force: true, first: false) ?? this.ReadRawSection("DATA", force: true, first: false);

                // Export Table
                this.ExportDataSectionRaw = this.ReadRawSection(".edata", force: true, first: false);

                // Import Table
                this.ImportDataSectionRaw = this.ReadRawSection(".idata", force: true, first: false);

                // Resource Data Section
                this.ResourceDataSectionRaw = this.ReadRawSection(".rdata", force: true, first: false);

                // Text Section
                this.TextSectionRaw = this.ReadRawSection(".text", force: true, first: false);

                #endregion

                #region Freeform Data

                // Entry Point Data
                if (this.OptionalHeader != null && this.OptionalHeader.AddressOfEntryPoint != 0)
                {
                    int entryPointAddress = (int)ConvertVirtualAddress(this.OptionalHeader.AddressOfEntryPoint, SectionTable);
                    this.EntryPointRaw = this.ReadArbitraryRange(entryPointAddress, 1024);
                }

                // Overlay Data
                if (this.SectionTable != null && this.SectionTable.Length > 0)
                {
                    // TODO: Read certificate data separately
                    int overlayOffset = this.SectionTable
                        .Select(sh => (int)(sh.PointerToRawData + sh.VirtualSize))
                        .OrderByDescending(o => o)
                        .First();

                    if (overlayOffset < stream.Length)
                        this.OverlayRaw = this.ReadArbitraryRange(rangeStart: overlayOffset);
                }

                #endregion

                // Populate resources, if possible
                PopulateResourceStrings();
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deserialize a PortableExecutable object from a byte array
        /// </summary>
        /// <param name="fileContent">Byte array representing a file</param>
        /// <param name="offset">Positive offset representing the current position in the array</param>
        private bool Deserialize(byte[] content, int offset)
        {
            try
            {
                // Attempt to read the DOS header first
                this.DOSStubHeader = MSDOSExecutableHeader.Deserialize(content, ref offset);
                offset = this.DOSStubHeader.NewExeHeaderAddr;
                if (this.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                    return false;

                // If the new header address is invalid for the file, it's not a PE
                if (this.DOSStubHeader.NewExeHeaderAddr >= content.Length)
                    return false;

                // Then attempt to read the PE header
                this.ImageFileHeader = CommonObjectFileFormatHeader.Deserialize(content, ref offset);
                if (this.ImageFileHeader.Signature != Constants.IMAGE_NT_SIGNATURE)
                    return false;

                // If the optional header is supposed to exist, read that as well
                if (this.ImageFileHeader.SizeOfOptionalHeader > 0)
                    this.OptionalHeader = OptionalHeader.Deserialize(content, ref offset);

                // Then read in the section table
                this.SectionTable = new SectionHeader[this.ImageFileHeader.NumberOfSections];
                for (int i = 0; i < this.ImageFileHeader.NumberOfSections; i++)
                {
                    this.SectionTable[i] = SectionHeader.Deserialize(content, ref offset);
                }

                #region Structured Tables

                // // Debug Section
                // var table = this.GetLastSection(".debug", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)table.PointerToRawData;
                //     this.DebugSection = DebugSection.Deserialize(content, ref tableAddress, this.SectionTable);
                // }

                // // Export Table
                // var table = this.GetLastSection(".edata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)table.PointerToRawData;
                //     this.ExportTable = ExportDataSection.Deserialize(content, ref tableAddress, this.SectionTable);
                // }

                // // Import Table
                // table = this.GetSection(".idata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)table.PointerToRawData;
                //     this.ImportTable = ImportDataSection.Deserialize(content, ref tableAddress, this.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0);
                // }

                // // Relocation Section
                // var table = this.GetLastSection(".reloc", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)table.PointerToRawData;
                //     this.RelocationTable = RelocationSection.Deserialize(content, ref tableAddress);
                // }

                // Resource Table
                var table = this.GetLastSection(".rsrc", true);
                if (table != null && table.VirtualSize > 0)
                {
                    int tableAddress = (int)table.PointerToRawData;
                    this.ResourceSection = ResourceSection.Deserialize(content, ref tableAddress, this.SectionTable);
                }

                #endregion

                #region Freeform Sections

                // Data Section
                this.DataSectionRaw = this.ReadRawSection(".data", force: true, first: false) ?? this.ReadRawSection("DATA", force: true, first: false);

                // Export Table
                this.ExportDataSectionRaw = this.ReadRawSection(".edata", force: true, first: false);

                // Import Table
                this.ImportDataSectionRaw = this.ReadRawSection(".idata", force: true, first: false);

                // Resource Data Section
                this.ResourceDataSectionRaw = this.ReadRawSection(".rdata", force: true, first: false);

                // Text Section
                this.TextSectionRaw = this.ReadRawSection(".text", force: true, first: false);

                #endregion

                #region Freeform Data

                // Entry Point Data
                if (this.OptionalHeader != null && this.OptionalHeader.AddressOfEntryPoint != 0)
                {
                    int entryPointAddress = (int)ConvertVirtualAddress(this.OptionalHeader.AddressOfEntryPoint, SectionTable);
                    this.EntryPointRaw = this.ReadArbitraryRange(entryPointAddress, 1024);
                }

                // Overlay Data
                if (this.SectionTable != null && this.SectionTable.Length > 0)
                {
                    // TODO: Read certificate data separately
                    int overlayOffset = this.SectionTable
                        .Select(sh => (int)(sh.PointerToRawData + sh.VirtualSize))
                        .OrderByDescending(o => o)
                        .First();

                    if (overlayOffset < content.Length)
                        this.OverlayRaw = this.ReadArbitraryRange(rangeStart: overlayOffset);
                }

                #endregion

                // Populate resources, if possible
                PopulateResourceStrings();
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return false;
            }

            return true;
        }

        #endregion

        #region Resource Helpers

        /// <summary>
        /// Find resource data in a ResourceSection, if possible
        /// </summary>
        /// <param name="dataStart">String to use if checking for data starting with a string</param>
        /// <param name="dataContains">String to use if checking for data contains a string</param>
        /// <param name="dataEnd">String to use if checking for data ending with a string</param>
        /// <returns>Full encoded resource data, null on error</returns>
        public ResourceDataEntry FindResource(string dataStart = null, string dataContains = null, string dataEnd = null)
        {
            if (this.ResourceSection == null)
                return null;

            return FindResourceInTable(this.ResourceSection.ResourceDirectoryTable, dataStart, dataContains, dataEnd);
        }

        /// <summary>
        /// Get the assembly identity node from an embedded manifest
        /// </summary>
        /// <param name="manifestString">String representing the XML document</param>
        /// <returns>Assembly identity node, if possible</returns>
        private XmlElement GetAssemblyNode(string manifestString)
        {
            // An invalid string means we can't read it
            if (string.IsNullOrWhiteSpace(manifestString))
                return null;

            try
            {
                // Load the XML string as a document
                var manifestDoc = new XmlDocument();
                manifestDoc.LoadXml(manifestString);

                // If the XML has no children, it's invalid
                if (!manifestDoc.HasChildNodes)
                    return null;

                // Try to read the assembly node
                return manifestDoc["assembly"];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Find the assembly manifest from a resource section, if possible
        /// </summary>
        /// <returns>Full assembly manifest, null on error</returns>
        private string FindAssemblyManifest() => FindResource(dataContains: "<assembly")?.DataAsUTF8String;

        /// <summary>
        /// Find resource data in a ResourceDirectoryTable, if possible
        /// </summary>
        /// <param name="rdt">ResourceDirectoryTable representing a layer</param>
        /// <param name="dataStart">String to use if checking for data starting with a string</param>
        /// <param name="dataContains">String to use if checking for data contains a string</param>
        /// <param name="dataEnd">String to use if checking for data ending with a string</param>
        /// <returns>Full encoded resource data, null on error</returns>
        private ResourceDataEntry FindResourceInTable(ResourceDirectoryTable rdt, string dataStart, string dataContains, string dataEnd)
        {
            if (rdt == null)
                return null;

            try
            {
                foreach (var rdte in rdt.NamedEntries)
                {
                    if (rdte.IsResourceDataEntry() && rdte.DataEntry != null)
                    {
                        // Ignore if we have a nested executable
                        // TODO: Support nested executables
                        if (rdte.DataEntry.DataAsUTF8String.StartsWith("MZ"))
                            return null;

                        if (dataStart != null && rdte.DataEntry.DataAsUTF8String.StartsWith(dataStart))
                            return rdte.DataEntry;
                        else if (dataContains != null && rdte.DataEntry.DataAsUTF8String.Contains(dataContains))
                            return rdte.DataEntry;
                        else if (dataEnd != null && rdte.DataEntry.DataAsUTF8String.EndsWith(dataStart))
                            return rdte.DataEntry;
                    }
                    else
                    {
                        var manifest = FindResourceInTable(rdte.Subdirectory, dataStart, dataContains, dataEnd);
                        if (manifest != null)
                            return manifest;
                    }
                }

                foreach (var rdte in rdt.IdEntries)
                {
                    if (rdte.IsResourceDataEntry() && rdte.DataEntry != null)
                    {
                        // Ignore if we have a nested executable
                        // TODO: Support nested executables
                        if (rdte.DataEntry.DataAsUTF8String.StartsWith("MZ"))
                            return null;

                        if (dataStart != null && rdte.DataEntry.DataAsUTF8String.StartsWith(dataStart))
                            return rdte.DataEntry;
                        else if (dataContains != null && rdte.DataEntry.DataAsUTF8String.Contains(dataContains))
                            return rdte.DataEntry;
                        else if (dataEnd != null && rdte.DataEntry.DataAsUTF8String.EndsWith(dataStart))
                            return rdte.DataEntry;
                    }
                    else
                    {
                        var manifest = FindResourceInTable(rdte.Subdirectory, dataStart, dataContains, dataEnd);
                        if (manifest != null)
                            return manifest;
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Get the assembly version as determined by an embedded assembly manifest
        /// </summary>
        /// <returns>Description string, null on error</returns>
        private string GetManifestDescription()
        {
            // If we don't have a complete PE executable, just return null
            if (this.ResourceSection == null)
                return null;
            
            // Read in the manifest to a string
            string manifestString = FindAssemblyManifest();
            if (string.IsNullOrWhiteSpace(manifestString))
                return null;

            // Try to read the XML in from the string
            try
            {
                // Try to read the assembly
                var assemblyNode = GetAssemblyNode(manifestString);
                if (assemblyNode == null)
                    return null;

                // Return the content of the description node, if possible
                var descriptionNode = assemblyNode["description"];
                if (descriptionNode == null)
                    return null;
                    
                return descriptionNode.InnerXml;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the assembly version as determined by an embedded assembly manifest
        /// </summary>
        /// <returns>Version string, null on error</returns>
        private string GetManifestVersion()
        {
            // If we don't have a complete PE executable, just return null
            if (this.ResourceSection == null)
                return null;
            
            // Read in the manifest to a string
            string manifestString = FindAssemblyManifest();
            if (string.IsNullOrWhiteSpace(manifestString))
                return null;

            // Try to read the XML in from the string
            try
            {
                // Try to read the assembly
                var assemblyNode = GetAssemblyNode(manifestString);
                if (assemblyNode == null)
                    return null;

                // Try to read the assemblyIdentity
                var assemblyIdentityNode = assemblyNode["assemblyIdentity"];
                if (assemblyIdentityNode == null)
                    return null;
                
                // Return the version attribute, if possible
                return assemblyIdentityNode.GetAttribute("version");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get a resource string from the version info
        /// </summary>
        /// <returns>Original filename string, null on error</returns>
        private string GetResourceString(string key)
        {
            var resourceStrings = GetVersionInfo()?.ChildrenStringFileInfo?.Children?.Children;
            if (resourceStrings == null)
                return null;
            
            var value = resourceStrings.FirstOrDefault(s => s.Key == key);
            if (!string.IsNullOrWhiteSpace(value?.Value))
                return value.Value.Trim(' ', '\0');

            return null;
        }

        /// <summary>
        /// Get the version info object related to file contents, if possible
        /// </summary>
        /// <returns>VersionInfo object on success, null on error</returns>
        private VersionInfo GetVersionInfo()
        {
            // If we don't have a complete PE executable, just return null
            if (this.ResourceSection == null)
                return null;

            // Try to get the matching resource
            var resource = FindResource(dataContains: "V\0S\0_\0V\0E\0R\0S\0I\0O\0N\0_\0I\0N\0F\0O\0");
            if (resource?.Data == null)
                return null;

            try
            {
                int index = 0;
                return VersionInfo.Deserialize(resource.Data, ref index);
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Populate all resource strings
        /// </summary>
        private void PopulateResourceStrings()
        {
            // Standalone resource strings
            this.CompanyName = GetResourceString("CompanyName");
            this.FileDescription = GetResourceString("FileDescription");
            this.FileVersion = GetResourceString("FileVersion")?.Replace(", ", ".");
            this.InternalName = GetResourceString("InternalName");
            this.LegalCopyright = GetResourceString("LegalCopyright");
            this.LegalTrademarks = GetResourceString("LegalTrademarks");
            this.OriginalFileName = GetResourceString("OriginalFileName");
            this.ProductName = GetResourceString("ProductName");
            this.ProductVersion = GetResourceString("ProductVersion")?.Replace(", ", ".");

            // TODO: Make these combined calls more efficient
            // Manifest resource strings
            this.ManifestDescription = GetManifestDescription();
            this.ManifestVersion = GetManifestVersion();
        }

        #endregion

        #region Section Helpers

        /// <summary>
        /// Determine if a section is contained within the section table
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>True if the section is in the executable, false otherwise</returns>
        public bool ContainsSection(string sectionName, bool exact = false)
        {
            // Get all section names first
            string[] sectionNames = GetSectionNames();
            if (sectionNames == null)
                return false;
            
            // If we're checking exactly, return only exact matches
            if (exact)
                return sectionNames.Any(n => n.Equals(sectionName));
            
            // Otherwise, check if section name starts with the value
            else
                return sectionNames.Any(n => n.StartsWith(sectionName));
        }

        /// <summary>
        /// Convert a virtual address to a physical one
        /// </summary>
        /// <param name="virtualAddress">Virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Physical address, 0 on error</returns>
        public static uint ConvertVirtualAddress(uint virtualAddress, SectionHeader[] sections)
        {
            // Loop through all of the sections
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section is invalid, just skip it
                if (sections[i] == null)
                    continue;

                // If the section "starts" at 0, just skip it
                if (sections[i].PointerToRawData == 0)
                    continue;

                // Attempt to derive the physical address from the current section
                var section = sections[i];
                if (virtualAddress >= section.VirtualAddress && virtualAddress <= section.VirtualAddress + section.VirtualSize)
                   return section.PointerToRawData + virtualAddress - section.VirtualAddress;
            }

            return 0;
        }

        /// <summary>
        /// Get the first section based on name, if possible
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public SectionHeader GetFirstSection(string sectionName, bool exact = false)
        {
            // If we have no sections, we can't do anything
            if (SectionTable == null || !SectionTable.Any())
                return null;
            
            // If we're checking exactly, return only exact matches
            if (exact)
                return SectionTable.FirstOrDefault(s => s.NameString.Equals(sectionName));
            
            // Otherwise, check if section name starts with the value
            else
                return SectionTable.FirstOrDefault(s => s.NameString.StartsWith(sectionName));
        }

        /// <summary>
        /// Get the last section based on name, if possible
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public SectionHeader GetLastSection(string sectionName, bool exact = false)
        {
            // If we have no sections, we can't do anything
            if (SectionTable == null || !SectionTable.Any())
                return null;
            
            // If we're checking exactly, return only exact matches (with nulls trimmed)
            if (exact)
                return SectionTable.LastOrDefault(s => s.NameString.Equals(sectionName));
            
            // Otherwise, check if section name starts with the value
            else
                return SectionTable.LastOrDefault(s => s.NameString.StartsWith(sectionName));
        }

        /// <summary>
        /// Get the list of section names
        /// </summary>
        public string[] GetSectionNames()
        {
            // Invalid table means no names are accessible
            if (SectionTable == null || SectionTable.Length == 0)
                return null;
            
            return SectionTable.Select(s => s.NameString).ToArray();
        }

        /// <summary>
        /// Print all sections, including their start and end addresses
        /// </summary>
        public void PrintAllSections()
        {
            foreach (var section in SectionTable)
            {
                string sectionName = section.NameString;
                int sectionAddr = (int)section.PointerToRawData;
                int sectionEnd = sectionAddr + (int)section.VirtualSize;
                Console.WriteLine($"{sectionName}: {sectionAddr} -> {sectionEnd}");
            }
        }

        /// <summary>
        /// Read an arbitrary range from the source
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns></returns>
        public byte[] ReadArbitraryRange(int rangeStart = -1, int length = -1)
        {
            try
            {
                // If we have a source stream, use that
                if (this._sourceStream != null)
                    return ReadArbitraryRangeFromSourceStream(rangeStart, length);

                // If we have a source array, use that
                if (this._sourceArray != null)
                    return ReadArbitraryRangeFromSourceArray(rangeStart, length);

                // Otherwise, return null
                return null;
            }
            catch (Exception ex)
            {
                // TODO: How to handle this differently?
                return null;
            }
        }

        /// <summary>
        /// Read an arbitrary range from the stream source, if possible
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns></returns>
        private byte[] ReadArbitraryRangeFromSourceStream(int rangeStart, int length)
        {
            lock (this._sourceStream)
            {
                int startingIndex = (int)Math.Max(rangeStart, 0);
                int readLength = (int)Math.Min(length == -1 ? length = Int32.MaxValue : length, this._sourceStream.Length);

                long originalPosition = this._sourceStream.Position;
                this._sourceStream.Seek(startingIndex, SeekOrigin.Begin);
                byte[] sectionData = this._sourceStream.ReadBytes(readLength);
                this._sourceStream.Seek(originalPosition, SeekOrigin.Begin);
                return sectionData;
            }
        }

        /// <summary>
        /// Read an arbitrary range from the array source, if possible
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns></returns>
        private byte[] ReadArbitraryRangeFromSourceArray(int rangeStart, int length)
        {
            int startingIndex = (int)Math.Max(rangeStart, 0);
            int readLength = (int)Math.Min(length == -1 ? length = Int32.MaxValue : length, this._sourceArray.Length);

            try
            {
                return this._sourceArray.ReadBytes(ref startingIndex, readLength);
            }
            catch
            {
                // Just absorb errors for now
                // TODO: Investigate why and when this would be hit
                return null;
            }
        }

        /// <summary>
        /// Get the raw bytes from a section, if possible
        /// </summary>
        /// <param name="sectionName">The name of the section to attempt to read</param>
        /// <param name="force">True to force reading the section from the underlying source, false to use cached values, if possible</param>
        /// <param name="first">True to use the first section with a matching name, false to use the last section</param>
        /// <param name="offset">Offset to start reading at, default is 0</param>
        public byte[] ReadRawSection(string sectionName, bool force = false, bool first = true, int offset = 0)
        {
            // Special cases for non-forced, non-offset data
            if (!force && offset == 0)
            {
                switch (sectionName)
                {
                    case ".data":
                        return DataSectionRaw;
                    case ".edata":
                        return ExportDataSectionRaw;
                    case ".idata":
                        return ImportDataSectionRaw;
                    case ".rdata":
                        return ResourceDataSectionRaw;
                    case ".text":
                        return TextSectionRaw;
                }
            }

            // Get the section, if possible
            var section = first ? GetFirstSection(sectionName, true) : GetLastSection(sectionName, true);
            if (section == null)
                return null;

            // Return the raw data from that section
            int rangeStart = (int)(section.PointerToRawData + offset);
            int rangeEnd = (int)(section.VirtualSize - offset);
            return ReadArbitraryRange(rangeStart, rangeEnd);
        }

        #endregion
    }
}