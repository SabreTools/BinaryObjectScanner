namespace BurnOutSharp.Models.MicrosoftCabinet.LZX
{
    /// <summary>
    /// An LZXD block represents a sequence of compressed data that is encoded with the same set of
    /// Huffman trees, or a sequence of uncompressed data. There can be one or more LZXD blocks in a
    /// compressed stream, each with its own set of Huffman trees. Blocks do not have to start or end on a
    /// chunk boundary; blocks can span multiple chunks, or a single chunk can contain multiple blocks. The
    /// number of chunks is related to the size of the data being compressed, while the number of blocks is
    /// related to how well the data is compressed. The Block Type field, as specified in section 2.3.1.1,
    /// indicates which type of block follows, and the Block Size field, as specified in section 2.3.1.2,
    /// indicates the number of uncompressed bytes represented by the block. Following the generic block
    /// header is a type-specific header that describes the remainder of the block.
    /// </summary>
    /// <see href="https://interoperability.blob.core.windows.net/files/MS-PATCH/%5bMS-PATCH%5d.pdf"/>
    public class BlockHeader
    {
        /// <remarks>3 bits</remarks>
        public LZXBlockType BlockType;

        /// <summary>
        /// Block size is the high 8 bits of 24
        /// </summary>
        /// <remarks>8 bits</remarks>
        public byte BlockSizeMSB;

        /// <summary>
        /// Block size is the middle 8 bits of 24
        /// </summary>
        /// <remarks>8 bits</remarks>
        public byte BlockSizeByte2;

        /// <summary>
        /// Block size is the low 8 bits of 24
        /// </summary>
        /// <remarks>8 bits</remarks>
        public byte BlocksizeLSB;
    }
}