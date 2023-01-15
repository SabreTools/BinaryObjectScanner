namespace BurnOutSharp.Models.PlayJ
{
    /// <summary>
    /// Data referred to by <see cref="EntryHeader.UnknownOffset1"/>
    /// </summary>
    public sealed class UnknownBlock1
    {
        /// <summary>
        /// Length of the following data block
        /// </summary>
        public ushort Length;

        /// <summary>
        /// Unknown data
        /// </summary>
        public byte[] Data;
    }
}