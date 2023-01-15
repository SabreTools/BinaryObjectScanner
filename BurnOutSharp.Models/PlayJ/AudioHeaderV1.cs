namespace BurnOutSharp.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio header / CDS entry header (V1)
    /// </summary>
    public sealed class AudioHeaderV1 : AudioHeader
    {
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
    }
}