namespace BinaryObjectScanner.Models.PlayJ
{
    /// <summary>
    /// Data referred to by <see cref="AudioHeaderV1.UnknownOffset1"/> or <see cref="AudioHeaderV2.UnknownOffset1"/>
    /// </summary>
    public sealed class UnknownBlock1
    {
        /// <summary>
        /// Length of the following data block
        /// </summary>
        public uint Length;

        /// <summary>
        /// Unknown data
        /// </summary>
        public byte[] Data;

        // Notes about Data:
        // - Might be UInt16 offset and UInt16 length pairs
        // - Might be relevant to ad data
        // - Might be relevant to encryption
        // - Highly repeating patterns in the values with only a few differences
    }
}