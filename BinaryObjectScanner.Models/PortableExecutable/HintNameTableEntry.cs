namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// One hint/name table suffices for the entire import section.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class HintNameTableEntry
    {
        /// <summary>
        /// An index into the export name pointer table. A match is attempted first
        /// with this value. If it fails, a binary search is performed on the DLL's
        /// export name pointer table.
        /// </summary>
        public ushort Hint;

        /// <summary>
        /// An ASCII string that contains the name to import. This is the string that
        /// must be matched to the public name in the DLL. This string is case sensitive
        /// and terminated by a null byte.
        /// </summary>
        public string Name;
    }
}
