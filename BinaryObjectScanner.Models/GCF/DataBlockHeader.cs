namespace BinaryObjectScanner.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class DataBlockHeader
    {
        /// <summary>
        /// GCF file version.  This field is not part of all file versions.
        /// </summary>
        public uint LastVersionPlayed;

        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Size of each data block in bytes.
        /// </summary>
        public uint BlockSize;

        /// <summary>
        /// Offset to first data block.
        /// </summary>
        public uint FirstBlockOffset;

        /// <summary>
        /// Number of data blocks that contain data.
        /// </summary>
        public uint BlocksUsed;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}