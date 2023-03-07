namespace BinaryObjectScanner.Models.GCF
{
    /// <remarks>
    /// Part of version 5 but not version 6.
    /// </remarks>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class BlockEntryMapHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Index of the first block entry.
        /// </summary>
        public uint FirstBlockEntryIndex;

        /// <summary>
        /// Index of the last block entry.
        /// </summary>
        public uint LastBlockEntryIndex;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}