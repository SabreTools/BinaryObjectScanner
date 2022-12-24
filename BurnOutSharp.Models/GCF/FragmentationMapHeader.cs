namespace BurnOutSharp.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class FragmentationMapHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// The index of the first unused fragmentation map entry.
        /// </summary>
        public uint FirstUnusedEntry;

        /// <summary>
        /// The block entry terminator; 0 = 0x0000ffff or 1 = 0xffffffff.
        /// </summary>
        public uint Terminator;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}