namespace BinaryObjectScanner.Models.PFF
{
    /// <summary>
    /// PFF segment identifier
    /// </summary>
    /// <see href="https://devilsclaws.net/download/file-pff-new-bz2"/>
    public sealed class Segment
    {
        /// <summary>
        /// Deleted flag
        /// </summary>
        public uint Deleted;

        /// <summary>
        /// File location
        /// </summary>
        public uint FileLocation;

        /// <summary>
        /// File size
        /// </summary>
        public uint FileSize;

        /// <summary>
        /// Packed date
        /// </summary>
        public uint PackedDate;

        /// <summary>
        /// File name
        /// </summary>
        public string FileName;

        /// <summary>
        /// Modified date
        /// </summary>
        /// <remarks>Only for versions 3 and 4</remarks>
        public uint ModifiedDate;

        /// <summary>
        /// Compression level
        /// </summary>
        /// <remarks>Only for version 4</remarks>
        public uint CompressionLevel;
    }
}