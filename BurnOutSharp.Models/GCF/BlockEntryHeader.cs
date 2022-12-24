namespace BurnOutSharp.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class BlockEntryHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Number of data blocks that point to data.
        /// </summary>
        public uint BlocksUsed;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy3;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy4;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}