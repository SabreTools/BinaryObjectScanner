namespace BurnOutSharp.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    public sealed class ChecksumMapEntry
    {
        /// <summary>
        /// Number of checksums.
        /// </summary>
        public uint ChecksumCount;

        /// <summary>
        /// Index of first checksum.
        /// </summary>
        public uint FirstChecksumIndex;
    }
}
