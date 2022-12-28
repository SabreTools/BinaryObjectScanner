namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Represents the organization of data in a file-version resource. It typically contains a
    /// list of language and code page identifier pairs that the version of the application or
    /// DLL supports.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/var-str"/>
    public sealed class VarData
    {
        /// <summary>
        /// The length, in bytes, of the Var structure.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// The size, in words, of the Value member.
        /// </summary>
        public ushort ValueLength;

        /// <summary>
        /// The type of data in the version resource.
        /// </summary>
        public VersionResourceType ResourceType;

        /// <summary>
        /// The Unicode string L"Translation".
        /// </summary>
        public string Key;

        /// <summary>
        /// As many zero words as necessary to align the Value member on a 32-bit boundary.
        /// </summary>
        public ushort Padding;

        /// <summary>
        /// An array of one or more values that are language and code page identifier pairs.
        /// 
        /// If you use the Var structure to list the languages your application or DLL supports
        /// instead of using multiple version resources, use the Value member to contain an array
        /// of DWORD values indicating the language and code page combinations supported by this
        /// file. The low-order word of each DWORD must contain a Microsoft language identifier,
        /// and the high-order word must contain the IBM code page number. Either high-order or
        /// low-order word can be zero, indicating that the file is language or code page
        /// independent. If the Var structure is omitted, the file will be interpreted as both
        /// language and code page independent.
        /// </summary>
        public uint[] Value;
    }
}
