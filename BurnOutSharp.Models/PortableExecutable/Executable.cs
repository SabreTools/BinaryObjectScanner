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

        // TODO: Left off at https://learn.microsoft.com/en-us/windows/win32/debug/pe-format#section-number-values
    }
}
