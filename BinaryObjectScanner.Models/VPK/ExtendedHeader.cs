namespace BinaryObjectScanner.Models.VPK
{
    /// <summary>
    /// Added in version 2.
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    public sealed class ExtendedHeader
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Archive hash length
        /// </summary>
        public uint ArchiveHashLength;

        /// <summary>
        /// Looks like some more MD5 hashes.
        /// </summary>
        public uint ExtraLength;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1;
    }
}
