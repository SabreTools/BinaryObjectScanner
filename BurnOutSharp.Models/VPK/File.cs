namespace BurnOutSharp.Models.VPK
{
    /// <summary>
    /// Valve Package File
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Extended header data
        /// </summary>
        public ExtendedHeader ExtendedHeader { get; set; }

        /// <summary>
        /// Archive hashes data
        /// </summary>
        public ArchiveHash[] ArchiveHashes { get; set; }

        /// <summary>
        /// Directory items data
        /// </summary>
        public DirectoryItem[] DirectoryItems { get; set; }
    }
}
