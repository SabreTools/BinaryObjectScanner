namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Represents the organization of data in a file-version resource. It is the root
    /// structure that contains all other file-version information structures.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/vs-versioninfo"/>
    public sealed class VersionInfo
    {
        /// <summary>
        /// The length, in bytes, of the VS_VERSIONINFO structure. This length does not
        /// include any padding that aligns any subsequent version resource data on a
        /// 32-bit boundary.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// The length, in bytes, of the Value member. This value is zero if there is no
        /// Value member associated with the current version structure.
        /// </summary>
        public ushort ValueLength;

        /// <summary>
        /// The type of data in the version resource. This member is 1 if the version resource
        /// contains text data and 0 if the version resource contains binary data.
        /// </summary>
        public VersionResourceType ResourceType;

        /// <summary>
        /// The Unicode string L"VS_VERSION_INFO".
        /// </summary>
        public string Key;

        /// <summary>
        /// Contains as many zero words as necessary to align the Value member on a 32-bit boundary.
        /// </summary>
        public ushort Padding1;

        /// <summary>
        /// Arbitrary data associated with this VS_VERSIONINFO structure. The ValueLength member
        /// specifies the length of this member; if ValueLength is zero, this member does not exist.
        /// </summary>
        public FixedFileInfo Value;

        /// <summary>
        /// As many zero words as necessary to align the Children member on a 32-bit boundary.
        /// These bytes are not included in wValueLength. This member is optional.
        /// </summary>
        public ushort Padding2;

        /// <summary>
        /// The StringFileInfo structure to store user-defined string information data.
        /// </summary>
        public StringFileInfo StringFileInfo;

        /// <summary>
        /// The VarFileInfo structure to store language information data.
        /// </summary>
        public VarFileInfo VarFileInfo;
    }
}
