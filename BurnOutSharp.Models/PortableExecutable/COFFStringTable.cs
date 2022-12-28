namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Immediately following the COFF symbol table is the COFF string table. The
    /// position of this table is found by taking the symbol table address in the
    /// COFF header and adding the number of symbols multiplied by the size of a symbol.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class COFFStringTable
    {
        /// <summary>
        /// At the beginning of the COFF string table are 4 bytes that contain the
        /// total size (in bytes) of the rest of the string table. This size includes
        /// the size field itself, so that the value in this location would be 4 if no
        /// strings were present.
        /// </summary>
        public uint TotalSize;

        /// <summary>
        /// Following the size are null-terminated strings that are pointed to by symbols
        /// in the COFF symbol table.
        /// </summary>
        public string[] Strings;
    }
}
