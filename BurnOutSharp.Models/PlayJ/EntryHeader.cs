namespace BurnOutSharp.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio header / CDS entry header
    /// </summary>
    /// <remarks>This is only valid for V1 right now</remarks>
    public sealed class EntryHeader
    {
        /// <summary>
        /// Signature (0x4B539DFF)
        /// </summary>
        public uint Signature;

        /// <summary>
        /// Version
        /// </summary>
        public uint Version;

        /// <summary>
        /// Download track ID
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint TrackID;

        /// <summary>
        /// Offset to unknown data block 1
        /// </summary>
        public uint UnknownOffset1;

        /// <summary>
        /// Offset to unknown data block 2
        /// </summary>
        public uint UnknownOffset2;

        /// <summary>
        /// Offset to unknown data block 3
        /// </summary>
        public uint UnknownOffset3;

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Always 0x00000001</remarks>
        public uint Unknown1;

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Typically 0x00000001 in download titles</remarks>
        public uint Unknown2;

        /// <summary>
        /// Track year
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint Year;

        /// <summary>
        /// Track number
        /// </summary>
        public byte TrackNumber;

        /// <summary>
        /// Subgenre
        /// </summary>
        public Subgenre Subgenre;

        /// <summary>
        /// Track duration in seconds
        /// </summary>
        public uint Duration;

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

        #region V2 Notes

        // Header Layout (V2) [WIP]
        // ------------------------------------------------------------------------------
        // 0x00                                 Signature                       UInt32
        // 0x04                                 Version                         UInt32 [0x0000000A]
        // 0x08                                 UNKNOWN                         byte[36]
        // 0x2A                                 Track Length                    UInt16
        // 0x2C                                 Track                           String
        // 0x2C+TL                              Artist Length                   UInt16
        // 0x2E+TL                              Artist                          String
        // 0x2E+TL+TAL                          Album Length                    UInt16
        // 0x30+TL+TAL                          Album                           String
        // 0x30+TL+TAL+AL                       Writer Length                   UInt16
        // 0x32+TL+TAL+AL                       Writer                          String
        // 0x32+TL+TAL+AL+WL                    Publisher Length                UInt16
        // 0x34+TL+TAL+AL+WL                    Publisher                       String
        // 0x34+TL+TAL+AL+WL+PL                 Label Length                    UInt16
        // 0x36+TL+TAL+AL+WL+PL                 Label                           String

        // In the third block:
        //      lady.plj has 0x00000002 and references "ad006376_5.dat" after

        #endregion
    }
}