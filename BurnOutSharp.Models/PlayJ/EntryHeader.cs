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

        // Unknown V2 data block (lady.plj)
        // ---------------------------
        // 0x08     0x00000001      Unknown
        // 0x0C     0x00000001      Unknown
        // 0x10     0x00000000      Unknown
        // 0x14     0x00000003      Unknown
        // 0x18     0x00000001      Unknown
        // 0x1C     0x00000000      Unknown
        // 0x20     0x00000066      Unknown
        // 0x24     0xF02F4372      Unknown
        // 0x28     0x00000004      Unknown
        // 0x2C     0x00000002      Unknown
        // 0x30     0x00000066      Unknown
        // 0x34     0x00005933      Unknown
        // 0x38     0xB462688A      Unknown
        // 0x3C     0x00000005      Unknown (Track number?)
        // 0x40     0x00000009      Unknown (Total tracks?)
        // 0x44     0x00005999      Unknown
        // 0x48     0x0032FF38      Unknown
        // 0x4C     0x00000000      Unknown
        // 0x50     0x00000007      Unknown
        // 0x54     0x0001897D      Track ID
        // 0x58     0xFFFFFFFF      Unknown (Year?)
        // 0x5C     0x00000005      Unknown (Track number?)
        // 0x60     0x00033068      Unknown

        // In the third block:
        //      lady.plj has 0x00000002 and references "ad006376_5.dat" after

        #endregion
    }
}