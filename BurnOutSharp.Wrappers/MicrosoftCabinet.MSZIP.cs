using System;
using System.Collections.Generic;
using BurnOutSharp.Utilities;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace BurnOutSharp.Wrappers
{
    public partial class MicrosoftCabinet : WrapperBase
    {
        #region Constants

        /// <summary>
        /// Maximum Huffman code bit count
        /// </summary>
        private const int MAX_BITS = 16;

        #endregion

        #region Properties

        /// <summary>
        /// Match lengths for literal codes 257..285
        /// </summary>
        /// <remarks>Each value here is the lower bound for lengths represented</remarks>
        private static Dictionary<int, int> LiteralLengths
        {
            get
            {
                // If we have cached length mappings, use those
                if (_literalLengths != null)
                    return _literalLengths;

                // Otherwise, build it from scratch
                _literalLengths = new Dictionary<int, int>
                {
                    [257] = 3,
                    [258] = 4,
                    [259] = 5,
                    [260] = 6,
                    [261] = 7,
                    [262] = 8,
                    [263] = 9,
                    [264] = 10,
                    [265] = 11, // 11,12
                    [266] = 13, // 13,14
                    [267] = 15, // 15,16
                    [268] = 17, // 17,18
                    [269] = 19, // 19-22
                    [270] = 23, // 23-26
                    [271] = 27, // 27-30
                    [272] = 31, // 31-34
                    [273] = 35, // 35-42
                    [274] = 43, // 43-50
                    [275] = 51, // 51-58
                    [276] = 59, // 59-66
                    [277] = 67, // 67-82
                    [278] = 83, // 83-98
                    [279] = 99, // 99-114
                    [280] = 115, // 115-130
                    [281] = 131, // 131-162
                    [282] = 163, // 163-194
                    [283] = 195, // 195-226
                    [284] = 227, // 227-257
                    [285] = 258,
                };

                return _literalLengths;
            }
        }

        /// <summary>
        /// Extra bits for literal codes 257..285
        /// </summary>
        private static Dictionary<int, int> LiteralExtraBits
        {
            get
            {
                // If we have cached bit mappings, use those
                if (_literalExtraBits != null)
                    return _literalExtraBits;

                // Otherwise, build it from scratch
                _literalExtraBits = new Dictionary<int, int>();

                // Literal Value 257 - 264, 0 bits
                for (int i = 257; i < 265; i++)
                    _literalExtraBits[i] = 0;

                // Literal Value 265 - 268, 1 bit
                for (int i = 265; i < 269; i++)
                    _literalExtraBits[i] = 1;

                // Literal Value 269 - 272, 2 bits
                for (int i = 269; i < 273; i++)
                    _literalExtraBits[i] = 2;

                // Literal Value 273 - 276, 3 bits
                for (int i = 273; i < 277; i++)
                    _literalExtraBits[i] = 3;

                // Literal Value 277 - 280, 4 bits
                for (int i = 277; i < 281; i++)
                    _literalExtraBits[i] = 4;

                // Literal Value 281 - 284, 5 bits
                for (int i = 281; i < 285; i++)
                    _literalExtraBits[i] = 5;

                // Literal Value 285, 0 bits
                _literalExtraBits[285] = 0;

                return _literalExtraBits;
            }
        }

        /// <summary>
        /// Match offsets for distance codes 0..29
        /// </summary>
        /// <remarks>Each value here is the lower bound for lengths represented</remarks>
        public static readonly int[] DistanceOffsets = new int[30]
        {
            1, 2, 3, 4, 5, 7, 9, 13, 17, 25,
            33, 49, 65, 97, 129, 193, 257, 385, 513, 769,
            1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577,
        };

        /// <summary>
        /// Extra bits for distance codes 0..29
        /// </summary>
        private static readonly int[] DistanceExtraBits = new int[30]
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3,
            4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
            9, 9, 10, 10, 11, 11, 12, 12, 13, 13,
        };

        /// <summary>
        /// The order of the bit length Huffman code lengths
        /// </summary>
        private static readonly int[] BitLengthOrder = new int[19]
        {
            16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15,
        };

        #endregion

        #region Instance Variables

        /// <summary>
        /// Match lengths for literal codes 257..285
        /// </summary>
        private static Dictionary<int, int> _literalLengths = null;

        /// <summary>
        /// Extra bits for literal codes 257..285
        /// </summary>
        private static Dictionary<int, int> _literalExtraBits = null;

        #endregion

        #region Parsing

        /// <summary>
        /// Read the block header from the block data, if possible
        /// </summary>
        /// <param name="data">BitStream representing the block</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>Filled block header on success, null on error</returns>
        private static Models.Compression.MSZIP.BlockHeader AsBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.Compression.MSZIP.BlockHeader();

            header.Signature = data.ReadAlignedUInt16();
            if (header.Signature != 0x4B43)
                return null;

            return header;
        }

        /// <summary>
        /// Read the deflate block header from the block data, if possible
        /// </summary>
        /// <param name="data">Byte array representing the block</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>Filled deflate block header on success, null on error</returns>
        private static Models.Compression.MSZIP.DeflateBlockHeader AsDeflateBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.Compression.MSZIP.DeflateBlockHeader();

            header.BFINAL = data.ReadBits(1)[0];
            header.BTYPE = (Models.Compression.MSZIP.CompressionType)data.ReadBits(2).AsByte();

            return header;
        }

        /// <summary>
        /// Read the block header from the block data, if possible
        /// </summary>
        /// <param name="data">Byte array representing the block</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>Filled dynamic Huffman compressed block header on success, null on error</returns>
        private static Models.Compression.MSZIP.DynamicHuffmanCompressedBlockHeader AsDynamicHuffmanCompressedBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.Compression.MSZIP.DynamicHuffmanCompressedBlockHeader();

            // # of Literal/Length codes - 257
            ushort HLIT = (ushort)(data.ReadBits(5).AsUInt16() + 257);

            // # of Distance codes - 1
            byte HDIST = (byte)(data.ReadBits(5).AsByte() + 1);

            // HCLEN, # of Code Length codes - 4
            byte HCLEN = (byte)(data.ReadBits(4).AsByte() + 4);

            // (HCLEN + 4) x 3 bits: code lengths for the code length
            //  alphabet given just above
            // 
            //  These code lengths are interpreted as 3-bit integers
            //  (0-7); as above, a code length of 0 means the
            //  corresponding symbol (literal/ length or distance code
            //  length) is not used.
            int[] bitLengths = new int[19];
            for (byte i = 0; i < HCLEN; i++)
                bitLengths[BitLengthOrder[i]] = data.ReadBits(3).AsByte();

            // Code length Huffman code
            int[] bitLengthTable = CreateTable(19, 7, bitLengths, 1 << 7);

            // HLIT + 257 code lengths for the literal/length alphabet,
            //  encoded using the code length Huffman code
            header.LiteralLengths = BuildHuffmanTree(data, HLIT, bitLengths, bitLengthTable);

            // HDIST + 1 code lengths for the distance alphabet,
            //  encoded using the code length Huffman code
            header.DistanceCodes = BuildHuffmanTree(data, HDIST, bitLengths, bitLengthTable);

            return header;
        }

        /// <summary>
        /// Read the block header from the block data, if possible
        /// </summary>
        /// <param name="data">Byte array representing the block</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>Filled non-compressed block header on success, null on error</returns>
        private static Models.Compression.MSZIP.NonCompressedBlockHeader AsNonCompressedBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.Compression.MSZIP.NonCompressedBlockHeader();

            header.LEN = data.ReadAlignedUInt16();
            header.NLEN = data.ReadAlignedUInt16();
            if (header.LEN != (~header.NLEN & 0xFFFF))
                return null;

            return header;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// The alphabet for code lengths is as follows
        /// </summary>
        private static int[] BuildHuffmanTree(BitStream data, ushort codeCount, int[] bitLengths, int[] decodingTable)
        {
            // Setup the huffman tree
            int[] tree = new int[codeCount];

            // Setup the loop variables
            int lastCode = 0, repeatLength = 0;
            for (int i = 0; i < codeCount; i++)
            {
                // TODO: Fix so we only read the number of bits we need
                int nextCode = data.ReadBits(7).AsUInt16();
                int symbol = decodingTable[nextCode];
                if (bitLengths[symbol] > 7)
                    _ = data.ReadBits(decodingTable[symbol] - 7);

                // Represent code lengths of 0 - 15
                if (symbol > 0 && symbol <= 15)
                {
                    lastCode = symbol;
                    tree[i] = symbol;
                }

                // Copy the previous code length 3 - 6 times.
                // The next 2 bits indicate repeat length (0 = 3, ... , 3 = 6)
                // Example: Codes 8, 16 (+2 bits 11), 16 (+2 bits 10) will expand to 12 code lengths of 8 (1 + 6 + 5)
                else if (symbol == 16)
                {
                    repeatLength = data.ReadBits(2).AsByte();
                    repeatLength += 2;
                    symbol = lastCode;
                }

                // Repeat a code length of 0 for 3 - 10 times.
                // (3 bits of length)
                else if (symbol == 17)
                {
                    repeatLength = data.ReadBits(3).AsByte();
                    repeatLength += 3;
                    symbol = 0;
                }

                // Repeat a code length of 0 for 11 - 138 times
                // (7 bits of length)
                else if (symbol == 18)
                {
                    repeatLength = data.ReadBits(7).AsByte();
                    repeatLength += 11;
                    symbol = 0;
                }

                // Everything else
                else
                {
                    throw new ArgumentOutOfRangeException();
                }

                // If we had a repeat length
                for (; repeatLength > 0; repeatLength--)
                {
                    tree[i++] = symbol;
                }
            }

            return tree;
        }

        /// <summary>
        /// This function was originally coded by David Tritscher.
        /// 
        /// It builds a fast huffman decoding table from a canonical huffman code lengths table.
        /// </summary>
        /// <param name="maxSymbols">Total number of symbols in this huffman tree.</param>
        /// <param name="bitCount">Any symbols with a code length of bitCount or less can be decoded in one lookup of the table.</param>
        /// <param name="lengths">A table to get code lengths from [0 to maxSymbols-1]</param>
        /// <returns>The table with decoded symbols and pointers.</returns>
        /// <see href="https://github.com/mnadareski/LibMSPackSharp/blob/master/LibMSPackSharp/Compression/CompressionStream.ReadHuff.cs"/>
        private static int[] CreateTable(int maxSymbols, int bitCount, int[] lengths, int distanceSize)
        {
            int[] table = new int[distanceSize];

            ushort sym, next_symbol;
            uint leaf, fill;
            uint reverse;
            byte bit_num;
            uint pos = 0; // The current position in the decode table
            uint table_mask = (uint)1 << bitCount;
            uint bit_mask = table_mask >> 1; // Don't do 0 length codes

            // Fill entries for codes short enough for a direct mapping
            for (bit_num = 1; bit_num <= bitCount; bit_num++)
            {
                for (sym = 0; sym < maxSymbols; sym++)
                {
                    if (lengths[sym] != bit_num)
                        continue;

                    // Reverse the significant bits
                    fill = (uint)lengths[sym];
                    reverse = pos >> (int)(bitCount - fill);
                    leaf = 0;

                    do
                    {
                        leaf <<= 1;
                        leaf |= reverse & 1;
                        reverse >>= 1;
                    } while (--fill > 0);

                    if ((pos += bit_mask) > table_mask)
                        return null; // Table overrun

                    // Fill all possible lookups of this symbol with the symbol itself
                    fill = bit_mask;
                    next_symbol = (ushort)(1 << bit_num);

                    do
                    {
                        table[leaf] = sym;
                        leaf += next_symbol;
                    } while (--fill > 0);
                }

                bit_mask >>= 1;
            }

            // Exit with success if table is now complete
            if (pos == table_mask)
                return table;

            // Mark all remaining table entries as unused
            for (sym = (ushort)pos; sym < table_mask; sym++)
            {
                reverse = sym;
                leaf = 0;
                fill = (uint)bitCount;

                do
                {
                    leaf <<= 1;
                    leaf |= reverse & 1;
                    reverse >>= 1;
                } while (--fill > 0);

                table[leaf] = 0xFFFF;
            }

            // next_symbol = base of allocation for long codes
            next_symbol = ((table_mask >> 1) < maxSymbols) ? (ushort)maxSymbols : (ushort)(table_mask >> 1);

            // Give ourselves room for codes to grow by up to 16 more bits.
            // codes now start at bit bitCount+16 and end at (bitCount+16-codelength)
            pos <<= 16;
            table_mask <<= 16;
            bit_mask = 1 << 15;

            for (bit_num = (byte)(bitCount + 1); bit_num <= MAX_BITS; bit_num++)
            {
                for (sym = 0; sym < maxSymbols; sym++)
                {
                    if (lengths[sym] != bit_num)
                        continue;
                    if (pos >= table_mask)
                        return null; // Table overflow

                    // leaf = the first bitCount of the code, reversed
                    reverse = pos >> 16;
                    leaf = 0;
                    fill = (uint)bitCount;

                    do
                    {
                        leaf <<= 1;
                        leaf |= reverse & 1;
                        reverse >>= 1;
                    } while (--fill > 0);

                    for (fill = 0; fill < (bit_num - bitCount); fill++)
                    {
                        // If this path hasn't been taken yet, 'allocate' two entries
                        if (table[leaf] == 0xFFFF)
                        {
                            table[(next_symbol << 1)] = 0xFFFF;
                            table[(next_symbol << 1) + 1] = 0xFFFF;
                            table[leaf] = (ushort)next_symbol++;
                        }

                        // Follow the path and select either left or right for next bit
                        leaf = (uint)(table[leaf] << 1);
                        if (((pos >> (15 - (int)fill)) & 1) != 0)
                            leaf++;
                    }

                    table[leaf] = sym;
                    pos += bit_mask;
                }

                bit_mask >>= 1;
            }

            // Full table?
            return pos == table_mask ? table : null;
        }

        #endregion

        #region Folders

        /// <summary>
        /// Decompress MSZIP data
        /// </summary>
        protected byte[] DecompressMSZIPData(byte[] data)
        {
            // Inflater inflater = new Inflater(noHeader: true);
            // inflater.SetInput(data);
            // byte[] outputData = new byte[data.Length * 4];
            // int read = inflater.Inflate(outputData);
            // return outputData.AsSpan(0, read).ToArray();

            // Create the bitstream to read from
            var dataStream = new BitStream(data);

            // Get the block header
            var blockHeader = AsBlockHeader(dataStream);
            if (blockHeader == null)
                return null;

            // Create the output byte array
            List<byte> decodedBytes = new List<byte>();

            // Create the loop variable block
            Models.Compression.MSZIP.DeflateBlockHeader deflateBlockHeader;

            do
            {
                deflateBlockHeader = AsDeflateBlockHeader(dataStream);

                // We should never get a reserved block
                if (deflateBlockHeader.BTYPE == Models.Compression.MSZIP.CompressionType.Reserved)
                    throw new InvalidOperationException();

                // If stored with no compression
                if (deflateBlockHeader.BTYPE == Models.Compression.MSZIP.CompressionType.NoCompression)
                {
                    // Skip any remaining bits in current partially processed byte
                    dataStream.DiscardBuffer();

                    // Read the block header
                    deflateBlockHeader.BlockDataHeader = AsNonCompressedBlockHeader(dataStream);

                    // Copy LEN bytes of data to output
                    var header = deflateBlockHeader.BlockDataHeader as Models.Compression.MSZIP.NonCompressedBlockHeader;
                    ushort length = header.LEN;
                    decodedBytes.AddRange(dataStream.ReadAlignedBytes(length));
                }

                // Otherwise
                else
                {
                    // If compressed with dynamic Huffman codes read representation of code trees
                    switch (deflateBlockHeader.BTYPE)
                    {
                        case Models.Compression.MSZIP.CompressionType.FixedHuffman:
                            deflateBlockHeader.BlockDataHeader = new Models.Compression.MSZIP.FixedHuffmanCompressedBlockHeader();
                            break;
                        case Models.Compression.MSZIP.CompressionType.DynamicHuffman:
                            deflateBlockHeader.BlockDataHeader = AsDynamicHuffmanCompressedBlockHeader(dataStream);
                            break;
                    }

                    var header = deflateBlockHeader.BlockDataHeader as Models.Compression.MSZIP.CompressedBlockHeader;

                    // 9 bits per entry, 288 max symbols
                    int[] literalDecodeTable = CreateTable(288, 9, header.LiteralLengths, (1 << 9) + (288 * 2));

                    // 6 bits per entry, 32 max symbols
                    int[] distanceDecodeTable = CreateTable(32, 6, header.DistanceCodes, (1 << 6) + (32 * 2));

                    // Loop until end of block code recognized
                    while (true)
                    {
                        // Decode literal/length value from input stream
                        int symbol = literalDecodeTable[dataStream.ReadBits(7).AsUInt16()];

                        // Copy value (literal byte) to output stream
                        if (symbol < 256)
                        {
                            decodedBytes.Add((byte)symbol);
                        }
                        // End of block (256)
                        else if (symbol == 256)
                        {
                            break;
                        }
                        else
                        {
                            // Decode distance from input stream
                            ulong length = dataStream.ReadBits(LiteralExtraBits[symbol]).AsUInt64();
                            length += (ulong)LiteralLengths[symbol];

                            int code = distanceDecodeTable[length];

                            ulong distance = dataStream.ReadBits(DistanceExtraBits[code]).AsUInt64();
                            distance += (ulong)DistanceOffsets[code];


                            // Move backwards distance bytes in the output
                            // stream, and copy length bytes from this
                            // position to the output stream.
                        }
                    }
                }
            } while (!deflateBlockHeader.BFINAL);

            /*
             Note that a duplicated string reference may refer to a string
             in a previous block; i.e., the backward distance may cross one
             or more block boundaries.  However a distance cannot refer past
             the beginning of the output stream.  (An application using a
             preset dictionary might discard part of the output stream; a
             distance can refer to that part of the output stream anyway)
             Note also that the referenced string may overlap the current
             position; for example, if the last 2 bytes decoded have values
             X and Y, a string reference with <length = 5, distance = 2>
             adds X,Y,X,Y,X to the output stream.
            */

            return decodedBytes.ToArray();
        }

        #endregion
    }
}
