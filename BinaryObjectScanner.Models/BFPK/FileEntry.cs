namespace BinaryObjectScanner.Models.BFPK
{
    /// <summary>
    /// File entry
    /// </summary>
    /// <see cref="https://forum.xentax.com/viewtopic.php?t=5102"/>
    public sealed class FileEntry
    {
        /// <summary>
        /// Name size
        /// </summary>
        public int NameSize;

        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Uncompressed size
        /// </summary>
        public int UncompressedSize;

        /// <summary>
        /// Offset
        /// </summary>
        public int Offset;

        /// <summary>
        /// Compressed size
        /// </summary>
        public int CompressedSize;
    }
}
