namespace BinaryObjectScanner.Models.SFFS
{
    /// <summary>
    /// SFFS consists of 2 major parts: the container files that contain the game
    /// content and a filesystem filter driver (sfvfs02.sys) that handles all file-io.
    /// When a game has SFFS'ed files and uses some file-io api like CreateFile, the
    /// SFFS filterdriver sees this request and handles it if needed.that way, SFFS is
    /// totally transparent to the game, since it never knows if the data is coming from
    /// real file API or from the SFFS filterdriver. during SF initialization, the
    /// SF-process registers itself as a SFFS process in a processid list, maintained
    /// from the filterdriver. Part of that registration are the names of the
    /// containerfiles, the process uses and an application key, that is needed to
    /// decrypt headerinfos. Note that SFFS itself is completly vm-free.
    /// </summary>
    /// <see href="https://forum.xentax.com/viewtopic.php?f=21&t=2084"/>
    public sealed class StarForceFileSystem
    {
        /// <summary>
        /// Header
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Files
        /// </summary>
        public FileEntry[] Files { get; set; }

        /// <summary>
        /// File headers
        /// </summary>
        public FileHeader[] FileHeaders { get; set; }
    }
}
