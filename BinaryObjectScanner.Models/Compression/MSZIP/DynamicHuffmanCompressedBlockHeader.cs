namespace BinaryObjectScanner.Models.Compression.MSZIP
{
    /// <summary>
    /// Compression with dynamic Huffman codes (BTYPE=10)
    /// </summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc1951"/>
    public class DynamicHuffmanCompressedBlockHeader
    {
        /// <summary>
        /// Huffman code lengths for the literal / length alphabet
        /// </summary>
        public int[] LiteralLengths;

        /// <summary>
        /// Huffman distance codes for the literal / length alphabet
        /// </summary>
        public int[] DistanceCodes;
    }
}