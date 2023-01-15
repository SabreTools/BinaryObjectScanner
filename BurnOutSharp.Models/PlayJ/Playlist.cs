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
        public PlaylistHeader Header { get; set; }

        /// <summary>
        /// Embedded audio files / headers
        /// </summary>
        public AudioFile[] AudioFiles { get; set; }
    }
}