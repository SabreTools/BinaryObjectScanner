namespace BurnOutSharp.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
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