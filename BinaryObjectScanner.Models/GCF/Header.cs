namespace BinaryObjectScanner.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class Header
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint MajorVersion;

        /// <summary>
        /// GCF version number.
        /// </summary>
        public uint MinorVersion;

        /// <summary>
        /// Cache ID
        /// </summary>
        public uint CacheID;

        /// <summary>
        /// Last version played
        /// </summary>
        public uint LastVersionPlayed;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2;

        /// <summary>
        /// Total size of GCF file in bytes.
        /// </summary>
        public uint FileSize;

        /// <summary>
        /// Size of each data block in bytes.
        /// </summary>
        public uint BlockSize;

        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy3;
    }
}