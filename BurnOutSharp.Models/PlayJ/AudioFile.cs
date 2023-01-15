namespace BurnOutSharp.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio file / CDS entry
    /// <summary>
    public sealed class AudioFile
    {
        /// <summary>
        /// Header
        /// </summary>
        public EntryHeader Header { get; set; }

        /// <summary>
        /// Unknown block 1
        /// </summary>
        public UnknownBlock1 UnknownBlock1 { get; set; }

        /// <summary>
        /// Value referred to by <see cref="EntryHeader.UnknownOffset2"/>
        /// </summary>
        /// <remarks>Typically 0x00000000</remarks>
        public uint UnknownValue2 { get; set; }

        /// <summary>
        /// Unknown block 3
        /// </summary>
        public UnknownBlock3 UnknownBlock3 { get; set; }
    }
}