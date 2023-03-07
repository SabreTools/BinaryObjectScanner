namespace BinaryObjectScanner.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    public sealed class DirectoryHeader
    {
        /// <summary>
        /// Always 0x00000004
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Cache ID.
        /// </summary>
        public uint CacheID;

        /// <summary>
        /// NCF file version.
        /// </summary>
        public uint LastVersionPlayed;

        /// <summary>
        /// Number of items in the directory.
        /// </summary>
        public uint ItemCount;

        /// <summary>
        /// Number of files in the directory.
        /// </summary>
        public uint FileCount;

        /// <summary>
        /// Always 0x00008000.  Data per checksum?
        /// </summary>
        public uint ChecksumDataLength;

        /// <summary>
        /// Size of lpNCFDirectoryEntries & lpNCFDirectoryNames & lpNCFDirectoryInfo1Entries & lpNCFDirectoryInfo2Entries & lpNCFDirectoryCopyEntries & lpNCFDirectoryLocalEntries in bytes.
        /// </summary>
        public uint DirectorySize;

        /// <summary>
        /// Size of the directory names in bytes.
        /// </summary>
        public uint NameSize;

        /// <summary>
        /// Number of Info1 entires.
        /// </summary>
        public uint Info1Count;

        /// <summary>
        /// Number of files to copy.
        /// </summary>
        public uint CopyCount;

        /// <summary>
        /// Number of files to keep local.
        /// </summary>
        public uint LocalCount;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}
