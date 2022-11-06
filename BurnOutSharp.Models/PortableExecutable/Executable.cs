namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The following list describes the Microsoft PE executable format, with the
    /// base of the image header at the top. The section from the MS-DOS 2.0
    /// Compatible EXE Header through to the unused section just before the PE header
    /// is the MS-DOS 2.0 Section, and is used for MS-DOS compatibility only.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class Executable
    {
        /// <summary>
        /// MS-DOS executable stub
        /// </summary>
        public MSDOS.Executable Stub { get; set; }

        /// <summary>
        /// After the MS-DOS stub, at the file offset specified at offset 0x3c, is a 4-byte
        /// signature that identifies the file as a PE format image file. This signature is "PE\0\0"
        /// (the letters "P" and "E" followed by two null bytes).
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// COFF file header
        /// </summary>
        public COFFFileHeader COFFFileHeader { get; set; }

        /// <summary>
        /// Optional header
        /// </summary>
        public OptionalHeader OptionalHeader { get; set; }

        /// <summary>
        /// Section table
        /// </summary>
        public SectionHeader[] SectionTable { get; set; }

        /// <summary>
        /// COFF symbol table
        /// </summary>
        public COFFSymbolTableEntry[] COFFSymbolTable { get; set; }

        /// <summary>
        /// COFF string table
        /// </summary>
        public COFFStringTable COFFStringTable { get; set; }

        /// <summary>
        /// Attribute certificate table
        /// </summary>
        public AttributeCertificateTableEntry[] AttributeCertificateTable { get; set; }

        /// <summary>
        /// Delay-load directory table
        /// </summary>
        public DelayLoadDirectoryTableEntry[] DelayLoadDirectoryTable { get; set; }

        // TODO: Left off at "The .cormeta Section (Object Only)"

        // TODO: Implement and/or document the following non-modeled parts:
        // - Grouped Sections (Object Only)
        // - Certificate Data
        // - Delay Import Address Table
        // - Delay Import Name Table
        // - Delay Bound Import Address Table
        // - Delay Unload Import Address Table
        // - The .debug Section
        // - IMAGE_DEBUG_TYPE_FPO
        // - .debug$F (Object Only)
        // - .debug$S (Object Only)
        // - .debug$P (Object Only)
        // - .debug$T (Object Only)
        // - The .drectve Section (Object Only)
        // - The .edata Section (Image Only)
        // - Export Name Pointer Table
        // - Export Ordinal Table
        // - Export Name Table
        // - The .idata Section
        // - Import Lookup Table [has model, but bit-based]
        // - Import Address Table
        // - The .pdata Section [Multiple formats per entry]
        // - TLS Callback Functions
        // - The .rsrc Section
        // - The .cormeta Section (Object Only)
        // - The .sxdata Section

        // TODO: Determine if "Archive (Library) File Format" is worth modelling
    }
}
