namespace BinaryObjectScanner.Models.BFPK
{
    /// <summary>
    /// BFPK custom archive format
    /// </summary>
    /// <see cref="https://forum.xentax.com/viewtopic.php?t=5102"/>
    public sealed class Archive
    {
        /// <summary>
        /// Header
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Files
        /// </summary>
        public FileEntry[] Files { get; set; }
    }
}
