namespace BinaryObjectScanner.Models.PlayJ
{
    /// <summary>
    /// PlayJ playlist header
    /// </summary>
    public sealed class PlaylistHeader
    {
        /// <summary>
        /// Number of tracks contained within the playlist
        /// </summary>
        public uint TrackCount;

        /// <summary>
        /// 52 bytes of unknown data
        /// </summary>
        public byte[] Data;
    }
}