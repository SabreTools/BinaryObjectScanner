namespace BurnOutSharp.Models.XZP
{
    /// <summary>
    /// XBox Package File
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Directory entries data
        /// </summary>
        public DirectoryEntry[] DirectoryEntries { get; set; }

        /// <summary>
        /// Preload directory entries data
        /// </summary>
        public DirectoryEntry[] PreloadDirectoryEntries { get; set; }

        /// <summary>
        /// Preload directory mappings data
        /// </summary>
        public DirectoryMapping[] PreloadDirectoryMappings { get; set; }

        /// <summary>
        /// Directory items data
        /// </summary>
        public DirectoryItem[] DirectoryItems { get; set; }

        /// <summary>
        /// Footer data
        /// </summary>
        public Footer Footer { get; set; }
    }
}
