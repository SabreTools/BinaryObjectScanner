namespace BinaryObjectScanner.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio header / CDS entry header
    /// </summary>
    /// <remarks>V1 and V2 variants exist</remarks>
    public abstract class AudioHeader
    {
        /// <summary>
        /// Signature (0x4B539DFF)
        /// </summary>
        public uint Signature;

        /// <summary>
        /// Version
        /// </summary>
        public uint Version;

        // Header-specific data goes here

        /// <summary>
        /// Length of the track name
        /// </summary>
        public ushort TrackLength;

        /// <summary>
        /// Track name (not null-terminated)
        /// </summary>
        public string Track;

        /// <summary>
        /// Length of the artist name
        /// </summary>
        public ushort ArtistLength;

        /// <summary>
        /// Artist name (not null-terminated)
        /// </summary>
        public string Artist;

        /// <summary>
        /// Length of the album name
        /// </summary>
        public ushort AlbumLength;

        /// <summary>
        /// Album name (not null-terminated)
        /// </summary>
        public string Album;

        /// <summary>
        /// Length of the writer name
        /// </summary>
        public ushort WriterLength;

        /// <summary>
        /// Writer name (not null-terminated)
        /// </summary>
        public string Writer;

        /// <summary>
        /// Length of the publisher name
        /// </summary>
        public ushort PublisherLength;

        /// <summary>
        /// Publisher name (not null-terminated)
        /// </summary>
        public string Publisher;

        /// <summary>
        /// Length of the label name
        /// </summary>
        public ushort LabelLength;

        /// <summary>
        /// Label name (not null-terminated)
        /// </summary>
        public string Label;

        /// <summary>
        /// Length of the comments
        /// </summary>
        /// <remarks>Optional field only in some samples</remarks>
        public ushort CommentsLength;

        /// <summary>
        /// Comments (not null-terminated)
        /// </summary>
        /// <remarks>Optional field only in some samples</remarks>
        public string Comments;
    }
}