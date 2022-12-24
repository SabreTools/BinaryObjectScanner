namespace BurnOutSharp.Models.NCF
{
    /// <summary>
    /// Half-Life No Cache File
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Directory header data
        /// </summary>
        public DirectoryHeader DirectoryHeader { get; set; }

        /// <summary>
        /// Directory entries data
        /// </summary>
        public DirectoryEntry[] DirectoryEntries { get; set; }

        /// <summary>
        /// Directory names data
        /// </summary>
        public string DirectoryNames { get; set; }

        /// <summary>
        /// Directory info 1 entries data
        /// </summary>
        public DirectoryInfo1Entry[] DirectoryInfo1Entries { get; set; }

        /// <summary>
        /// Directory info 2 entries data
        /// </summary>
        public DirectoryInfo2Entry[] DirectoryInfo2Entries { get; set; }

        /// <summary>
        /// Directory copy entries data
        /// </summary>
        public DirectoryCopyEntry[] DirectoryCopyEntries { get; set; }

        /// <summary>
        /// Directory local entries data
        /// </summary>
        public DirectoryLocalEntry[] DirectoryLocalEntries { get; set; }

        /// <summary>
        /// Unknown header data
        /// </summary>
        public UnknownHeader UnknownHeader { get; set; }

        /// <summary>
        /// Unknown entries data
        /// </summary>
        public UnknownEntry[] UnknownEntries { get; set; }

        /// <summary>
        /// Checksum header data
        /// </summary>
        public ChecksumHeader ChecksumHeader { get; set; }

        /// <summary>
        /// Checksum map header data
        /// </summary>
        public ChecksumMapHeader ChecksumMapHeader { get; set; }

        /// <summary>
        /// Checksum map entries data
        /// </summary>
        public ChecksumMapEntry[] ChecksumMapEntries { get; set; }

        /// <summary>
        /// Checksum entries data
        /// </summary>
        public ChecksumEntry[] ChecksumEntries { get; set; }
    }
}
