namespace BurnOutSharp.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio header / CDS entry header (V2)
    /// </summary>
    public sealed class AudioHeaderV2 : AudioHeader
    {
        /// <summary>
        /// Unknown (Always 0x00000001)
        /// </summary>
        public uint Unknown1;

        /// <summary>
        /// Unknown (Always 0x00000001)
        /// </summary>
        public uint Unknown2;

        /// <summary>
        /// Unknown (Always 0x00000000)
        /// </summary>
        public uint Unknown3;

        /// <summary>
        /// Unknown (Always 0x00000003)
        /// </summary>
        public uint Unknown4;

        /// <summary>
        /// Unknown (Always 0x00000001)
        /// </summary>
        public uint Unknown5;

        /// <summary>
        /// Unknown (Always 0x00000000)
        /// </summary>
        public uint Unknown6;

        /// <summary>
        /// Offset to unknown block 1, relative to the track ID
        /// </summary>
        public uint UnknownOffset1;

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown7;

        /// <summary>
        /// Unknown (Always 0x00000004)
        /// </summary>
        public uint Unknown8;

        /// <summary>
        /// Unknown (Always 0x00000002)
        /// </summary>
        public uint Unknown9;

        /// <summary>
        /// Offset to unknown block 1, relative to the track ID
        /// </summary>
        /// <remarks>Always identical to <see cref="UnknownOffset1"/>?</remarks>
        public uint UnknownOffset2;

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown10;

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown11;

        /// <summary>
        /// Unknown (Always 0x0000005)
        /// </summary>
        public uint Unknown12;

        /// <summary>
        /// Unknown (Always 0x0000009)
        /// </summary>
        public uint Unknown13;

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown14;

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown15;

        /// <summary>
        /// Unknown (Always 0x0000000)
        /// </summary>
        public uint Unknown16;

        /// <summary>
        /// Unknown (Always 0x00000007)
        /// </summary>
        public uint Unknown17;

        /// <summary>
        /// Download track ID
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint TrackID;

        /// <summary>
        /// Track year -- UNCONFIRMED
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint Year;

        /// <summary>
        /// Track number
        /// </summary>
        public uint TrackNumber;

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown18;
    }
}