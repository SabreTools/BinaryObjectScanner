namespace BurnOutSharp.Models.MicrosoftCabinet
{
    /// <summary>
    /// Base class for compressed blocks
    /// </summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc1951"/>
    public abstract class MSZIPCompressedBlock : IMSZIPBlockData
    {
        /// <summary>
        /// Huffman code lengths for the literal / length alphabet
        /// </summary>
        public abstract int[] LiteralLengths { get; set; }

        /// <summary>
        /// Huffman distance codes for the literal / length alphabet
        /// </summary>
        public abstract int[] DistanceCodes { get; set; }
    }
}