namespace BurnOutSharp.Models.MicrosoftCabinet.MSZIP
{
    /// <summary>
    /// Compression with dynamic Huffman codes (BTYPE=10)
    /// </summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc1951"/>
    public class DynamicHuffmanCompressedBlock : CompressedBlock
    {
        /// <inheritdoc/>
        public override int[] LiteralLengths { get; set; }

        /// <inheritdoc/>
        public override int[] DistanceCodes { get; set; }
    }
}