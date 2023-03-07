namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Represents the organization of data in a file-version resource. It contains version
    /// information not dependent on a particular language and code page combination.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/varfileinfo"/>
    public sealed class VarFileInfo
    {
        /// <summary>
        /// The length, in bytes, of the entire VarFileInfo block, including all structures
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
        /// The Unicode string L"VarFileInfo".
        /// </summary>
        public string Key;

        /// <summary>
        /// As many zero words as necessary to align the Children member on a 32-bit boundary.
        /// </summary>
        public ushort Padding;

        /// <summary>
        /// Typically contains a list of languages that the application or DLL supports.
        /// </summary>
        public VarData[] Children;
    }
}
