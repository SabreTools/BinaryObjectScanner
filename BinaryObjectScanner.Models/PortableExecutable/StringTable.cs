namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Represents the organization of data in a file-version resource. It contains language
    /// and code page formatting information for the strings specified by the Children member.
    /// A code page is an ordered character set.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/stringtable"/>
    public sealed class StringTable
    {
        /// <summary>
        /// The length, in bytes, of this StringTable structure, including all structures
        /// indicated by the Children member.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// This member is always equal to zero.
        /// </summary>
        public ushort ValueLength;

        /// <summary>
        /// The type of data in the version resource.
        /// </summary>
        public VersionResourceType ResourceType;

        /// <summary>
        /// An 8-digit hexadecimal number stored as a Unicode string. The four most significant
        /// digits represent the language identifier. The four least significant digits represent
        /// the code page for which the data is formatted. Each Microsoft Standard Language
        /// identifier contains two parts: the low-order 10 bits specify the major language,
        /// and the high-order 6 bits specify the sublanguage.
        /// </summary>
        public string Key;

        /// <summary>
        /// As many zero words as necessary to align the Children member on a 32-bit boundary.
        /// </summary>
        public ushort Padding;

        /// <summary>
        /// An array of one or more StringData structures.
        /// </summary>
        public StringData[] Children;
    }
}
