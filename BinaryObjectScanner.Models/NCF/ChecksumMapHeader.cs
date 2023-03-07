namespace BinaryObjectScanner.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    public sealed class ChecksumMapHeader
    {
        /// <summary>
        /// Always 0x14893721
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy1;

        /// <summary>
        /// Number of items.
        /// </summary>
        public uint ItemCount;

        /// <summary>
        /// Number of checksums.
        /// </summary>
        public uint ChecksumCount;
    }
}
