namespace BurnOutSharp.Models.PlayJ
{
    /// <summary>
    /// PlayJ playlist file
    /// </summary>
    public sealed class Playlist
    {
        /// <summary>
        /// Playlist header
        /// </summary>
        public PlaylistHeader PlaylistHeader { get; set; }

        /// <summary>
        /// Entry headers
        /// </summary>
        public EntryHeader[] EntryHeaders { get; set; }
    }
}