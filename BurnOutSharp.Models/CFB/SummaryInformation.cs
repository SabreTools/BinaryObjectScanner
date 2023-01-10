using System;

namespace BurnOutSharp.Models.CFB
{
    /// <see href=""/>
    public sealed class SummaryInformation
    {
        /// <summary>
        /// This field MUST be set to 0xFFFE. This field is a byte order mark for
        /// all integer fields, specifying little-endian byte order.
        /// </summary>
        public ushort ByteOrder;

        /// <summary>
        /// 26 bytes of unknown data
        /// </summary>
        public byte[] Unknown1;

        /// <summary>
        /// Format ID, should be <see cref="Constants.FMTID_SummaryInformation"/>
        /// </summary>
        public Guid FormatID;

        /// <summary>
        /// 16 bytes of unknown data
        /// </summary>
        public byte[] Unknown2;

        /// <summary>
        /// Location of the section
        /// </summary>
        public uint Offset;

        /// <summary>
        /// Section count(?)
        /// </summary>
        public uint SectionCount;

        /// <summary>
        /// Property count
        /// </summary>
        public uint PropertyCount;

        // Followed by an array of variants
    }
}