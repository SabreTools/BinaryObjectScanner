using BurnOutSharp.Models.Compression.MSZIP;
using static BurnOutSharp.Models.Compression.MSZIP.Constants;

namespace BurnOutSharp.Compression.MSZIP
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
    public class Decompressor
    {
        /// <summary>
        /// Decompress a byte array using a given State
        /// </summary>
        public static bool Decompress(State state, int inlen, byte[] inbuf, int outlen, byte[] outbuf)
        {
            state.InputPosition = 0; // inbuf[0];
            state.BitBuffer = state.BitCount = state.WindowPosition = 0;
            if (outlen > ZIPWSIZE)
                return false;

            //  CK = Chris Kirmse, official Microsoft purloiner
            if (inbuf[state.InputPosition + 0] != 0x43 || inbuf[state.InputPosition + 1] != 0x48)
                return false;

            state.InputPosition += 2;

            int lastBlockFlag = 0;
            do
            {
                if (InflateBlock(ref lastBlockFlag, state, inbuf, outbuf) != 0)
                    return false;
            } while (lastBlockFlag == 0);

            // Return success
            return true;
        }

        /// <summary>
        /// Decompress a deflated block
        /// </summary>
        private static uint InflateBlock(ref int e, State state, byte[] inbuf, byte[] outbuf)
        {
            // Make local bit buffer
            uint bitBuffer = state.BitBuffer;
            uint bitCount = state.BitCount;

            // Read the deflate block header
            var header = new DeflateBlockHeader();

            // Read in last block bit
            ZIPNEEDBITS(1, state, inbuf, ref bitBuffer, ref bitCount);
            header.BFINAL = (e = (int)(bitBuffer & 1)) != 0;
            ZIPDUMPBITS(1, ref bitBuffer, ref bitCount);

            // Read in block type
            ZIPNEEDBITS(2, state, inbuf, ref bitBuffer, ref bitCount);
            header.BTYPE = (CompressionType)(bitBuffer & 3);
            ZIPDUMPBITS(2, ref bitBuffer, ref bitCount);

            // Restore the global bit buffer
            state.BitBuffer = bitBuffer;
            state.BitCount = bitCount;

            // Inflate that block type
            switch (header.BTYPE)
            {
                case CompressionType.NoCompression:
                    return (uint)DecompressStored(state, inbuf, outbuf);
                case CompressionType.FixedHuffman:
                    return (uint)DecompressFixed(state, inbuf, outbuf);
                case CompressionType.DynamicHuffman:
                    return (uint)DecompressDynamic(state, inbuf, outbuf);

                // Bad block type
                case CompressionType.Reserved:
                default:
                    return 2;
            }
        }

        /// <summary>
        /// "Decompress" a stored block
        /// </summary>
        private static int DecompressStored(State state, byte[] inbuf, byte[] outbuf)
        {
            // Make local copies of globals
            uint bitBuffer = state.BitBuffer;
            uint bitCount = state.BitCount;
            uint windowPosition = state.WindowPosition;

            // Go to byte boundary
            int n = (int)(bitCount & 7);
            ZIPDUMPBITS(n, ref bitBuffer, ref bitCount);

            // Read the stored block header
            var header = new NonCompressedBlockHeader();

            // Get the length and its compliment
            ZIPNEEDBITS(16, state, inbuf, ref bitBuffer, ref bitCount);
            header.LEN = (ushort)(bitBuffer & 0xffff);
            ZIPDUMPBITS(16, ref bitBuffer, ref bitCount);

            ZIPNEEDBITS(16, state, inbuf, ref bitBuffer, ref bitCount);
            header.NLEN = (ushort)(bitBuffer & 0xffff);

            if (header.LEN != (~header.NLEN & 0xffff))
                return 1; // Error in compressed data

            ZIPDUMPBITS(16, ref bitBuffer, ref bitCount);

            // Read and output the compressed data
            while (n-- > 0)
            {
                ZIPNEEDBITS(8, state, inbuf, ref bitBuffer, ref bitCount);
                outbuf[windowPosition++] = (byte)bitBuffer;
                ZIPDUMPBITS(8, ref bitBuffer, ref bitCount);
            }

            // Restore the globals from the locals
            state.WindowPosition = windowPosition;
            state.BitBuffer = bitBuffer;
            state.BitCount = bitCount;

            return 0;
        }

        /// <summary>
        /// Decompress a block originally compressed with fixed Huffman codes
        /// </summary>
        private static int DecompressFixed(State state, byte[] inbuf, byte[] outbuf)
        {
            // Create the block header
            var header = new FixedHuffmanCompressedBlockHeader();

            // Assign the literal lengths
            state.Lengths = header.LiteralLengths;
            var literalLengthTable = new HuffmanNode[30];
            int literalLengthBitCount = 7;

            // Build the literal length tree
            int i = BuildHuffmanTree(state.Lengths, 0, 30, 0, CopyOffsets, LiteralExtraBits, ref literalLengthTable, ref literalLengthBitCount, state);
            if (i != 0)
                return i;

            // Assign the distance codes
            state.Lengths = header.DistanceCodes;
            var distanceCodeTable = new HuffmanNode[30];
            int distanceCodeBitCount = 5;

            // Build the distance code tree
            i = BuildHuffmanTree(state.Lengths, 0, 30, 0, CopyOffsets, DistanceExtraBits, ref distanceCodeTable, ref distanceCodeBitCount, state);
            if (i != 0)
                return i;

            // Decompress until an end-of-block code
            return InflateCodes(literalLengthTable, distanceCodeTable, literalLengthBitCount, distanceCodeBitCount, state, inbuf, outbuf);
        }

        /// <summary>
        /// Decompress a block originally compressed with dynamic Huffman codes
        /// </summary>
        /// INCORRECT IMPLEMENTATION TO SATISFY COMPILER FOR NOW
        private static int DecompressDynamic(State state, byte[] inbuf, byte[] outbuf)
        {

            // TODO: Finish implementation
            return 0;
        }

        /// <summary>
        /// Build a Huffman tree from a set of lengths
        /// </summary>
        /// INCORRECT IMPLEMENTATION TO SATISFY COMPILER FOR NOW
        private static int BuildHuffmanTree(uint[] b, int bi, uint n, uint s, ushort[] d, ushort[] e, ref HuffmanNode[] t, ref int m, State state)
        {
            
            // TODO: Finish implementation
            return 0;
        }

        /// <summary>
        /// Inflate codes into Huffman trees
        /// </summary>
        /// INCORRECT IMPLEMENTATION TO SATISFY COMPILER FOR NOW
        private static int InflateCodes(HuffmanNode[] tl, HuffmanNode[] td, int bl, int bd, State state, byte[] inbuf, byte[] outbuf)
        {

            // TODO: Finish implementation
            return 0;
        }

        /// <summary>
        /// Free a single huffman node
        /// </summary>
        /// <remarks>No-op because of garbage collection</remarks>
        private static void Free(HuffmanNode node) { }

        #region Macros

        private static void ZIPNEEDBITS(int n, State state, byte[] inbuf, ref uint bitBuffer, ref uint bitCount)
        {
            while (bitCount < n)
            {
                int c = inbuf[state.InputPosition++];
                bitBuffer |= (uint)(c << (int)bitCount);
                bitCount += 8;
            }
        }

        private static void ZIPDUMPBITS(int n, ref uint bitBuffer, ref uint bitCount)
        {
            bitBuffer >>= n;
            bitCount -= (uint)n;
        }

        #endregion
    }
}