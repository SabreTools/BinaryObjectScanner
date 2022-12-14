using System;
using System.Collections;
using System.Collections.Generic;
using BurnOutSharp.Tools;

/// <see href="https://interoperability.blob.core.windows.net/files/MS-MCI/%5bMS-MCI%5d.pdf"/>
/// <see href="https://www.rfc-editor.org/rfc/rfc1951"/>
namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Each MSZIP block MUST consist of a 2-byte MSZIP signature and one or more RFC 1951 blocks. The
    /// 2-byte MSZIP signature MUST consist of the bytes 0x43 and 0x4B. The MSZIP signature MUST be
    /// the first 2 bytes in the MSZIP block.The MSZIP signature is shown in the following packet diagram.       
    /// </summary>
    public class MSZIPBlock
    {
        #region Constants

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string SignatureString = "CK";

        /// <summary>
        /// Signature as an unsigned Int16 value
        /// </summary>
        public const ushort SignatureValue = 0x4B43;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] SignatureBytes = new byte[] { 0x43, 0x4B };

        #endregion

        #region Properties

        /// <summary>
        /// 'CB'
        /// </summary>
        public ushort Signature { get; private set; }

        /// <summary>
        /// Each MSZIP block is the result of a single deflate compression operation, as defined in [RFC1951].
        /// The compressor that performs the compression operation MUST generate one or more RFC 1951
        /// blocks, as defined in [RFC1951]. The number, deflation mode, and type of RFC 1951 blocks in each
        /// MSZIP block is determined by the compressor, as defined in [RFC1951]. The last RFC 1951 block in
        /// each MSZIP block MUST be marked as the "end" of the stream(1), as defined by[RFC1951]
        /// section 3.2.3. Decoding trees MUST be discarded after each RFC 1951 block, but the history buffer
        /// MUST be maintained.Each MSZIP block MUST represent no more than 32 KB of uncompressed data.
        /// 
        /// The maximum compressed size of each MSZIP block is 32 KB + 12 bytes.This enables the MSZIP
        /// block to contain 32 KB of data split between two noncompressed RFC 1951 blocks, each of which
        /// has a value of BTYPE = 00.
        /// </summary>
        public byte[] Data { get; private set; }

        #endregion

        #region Serialization

        public static MSZIPBlock Deserialize(byte[] data)
        {
            if (data == null)
                return null;

            MSZIPBlock block = new MSZIPBlock();
            int dataPtr = 0;

            block.Signature = data.ReadUInt16(ref dataPtr);
            if (block.Signature != SignatureValue)
                return null;

            block.Data = data.ReadBytes(ref dataPtr, data.Length - 2);

            return block;
        }

        #endregion
    }

    #region Deflate Implementation

    /// <summary>
    /// How the data are compressed
    /// </summary>
    public enum MSZIPDeflateCompressionType : byte
    {
        /// <summary>
        /// no compression
        /// </summary>
        NoCompression = 0b00,

        /// <summary>
        /// Compressed with fixed Huffman codes
        /// </summary>
        FixedHuffman = 0b01,

        /// <summary>
        /// Compressed with dynamic Huffman codes
        /// </summary>
        DynamicHuffman = 0b10,

        /// <summary>
        /// Reserved (error)
        /// </summary>
        Reserved = 0b11,
    }

    public class MSZIPDeflateStream
    {
        #region Instance Variables

        /// <summary>
        /// Original data source to read from
        /// </summary>
        private System.IO.Stream _dataStream = null;

        /// <summary>
        /// Current rolling buffer
        /// </summary>
        private byte[] _buffer = null;

        /// <summary>
        /// Current position in the buffer
        /// </summary>
        private int _bufferPointer = -1;

        /// <summary>
        /// Bit buffer to read bits from when necessary
        /// </summary>
        private BitArray _bitBuffer = null;

        /// <summary>
        /// Number of bits left in the buffer
        /// </summary>
        private int _bitsLeft = 0;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MSZIPDeflateStream(System.IO.Stream dataStream)
        {
            _dataStream = dataStream;
        }

        /// <summary>
        /// Read between 0 and 64 bits of data from the stream assuming LSB
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ulong ReadBitsLSB(int numBits)
        {
            // If we are reading an invalid number of bits
            if (numBits < 0 || numBits > 64)
                throw new ArgumentOutOfRangeException();

            // Allocate the bit buffer
            ulong bitBuffer = 0;

            // If the bit buffer has the right number remaining
            if (_bitsLeft >= numBits)
            {
                for (int i = 0; i < numBits; i++)
                {
                    bitBuffer |= _bitBuffer[i + _bitBuffer.Length - _bitsLeft--] ? 1u : 0;
                    bitBuffer <<= 1;
                }

                return bitBuffer;
            }

            // Otherwise, we need to read what we can
            int bitsRemaining = _bitsLeft;
            for (int i = 0; i < bitsRemaining; i++)
            {
                bitBuffer |= _bitBuffer[i + _bitBuffer.Length - _bitsLeft--] ? 1u : 0;
                bitBuffer <<= 1;
            }

            // Fill the bit buffer, if possible
            FillBitBuffer();

            // If we couldn't read anything, throw an exception
            if (_buffer == null)
                throw new IndexOutOfRangeException();

            // Otherwise, read in the remaining bits needed
            for (int i = 0; i < bitsRemaining; i++)
            {
                bitBuffer |= _bitBuffer[i + _bitBuffer.Length - _bitsLeft--] ? 1u : 0;
                bitBuffer <<= 1;
            }

            return bitBuffer;
        }

        /// <summary>
        /// Read between 0 and 64 bits of data from the stream assuming MSB
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ulong ReadBitsMSB(int numBits)
        {
            // If we are reading an invalid number of bits
            if (numBits < 0 || numBits > 64)
                throw new ArgumentOutOfRangeException();

            // Allocate the bit buffer
            ulong bitBuffer = 0;

            // If the bit buffer has the right number remaining
            if (_bitsLeft >= numBits)
            {
                for (int i = 0; i < numBits; i++)
                {
                    bitBuffer |= _bitBuffer[i + _bitsLeft--] ? 1u : 0;
                    bitBuffer <<= 1;
                }

                return bitBuffer;
            }

            // Otherwise, we need to read what we can
            int bitsRemaining = _bitsLeft;
            for (int i = 0; i < bitsRemaining; i++)
            {
                bitBuffer |= _bitBuffer[i + _bitsLeft--] ? 1u : 0;
                bitBuffer <<= 1;
            }

            // Fill the bit buffer, if possible
            FillBitBuffer();

            // If we couldn't read anything, throw an exception
            if (_buffer == null)
                throw new IndexOutOfRangeException();

            // Otherwise, read in the remaining bits needed
            for (int i = 0; i < bitsRemaining; i++)
            {
                bitBuffer |= _bitBuffer[i + _bitsLeft--] ? 1u : 0;
                bitBuffer <<= 1;
            }

            return bitBuffer;
        }

        /// <summary>
        /// Read more than 0 bytes of data from the stream assuming LSB
        /// </summary>
        public byte[] ReadBytesLSB(int numBytes)
        {
            // If we are reading an invalid number of bytes
            if (numBytes < 0)
                throw new ArgumentOutOfRangeException();

            // Allocate the byte buffer
            byte[] byteBuffer = new byte[numBytes];
            int byteBufferPtr = 0;

            // If the bit buffer has the right number remaining
            if (_bitsLeft >= numBytes * 8)
            {
                byte fullBitBuffer = 0;
                for (int i = 0; i < numBytes * 8; i++)
                {
                    fullBitBuffer |= (byte)(_bitBuffer[i + _bitBuffer.Length - _bitsLeft--] ? 1 : 0);
                    if (i % 8 == 7)
                    {
                        byteBuffer[byteBufferPtr++] = fullBitBuffer;
                        fullBitBuffer = 0;
                    }
                    else
                    {
                        fullBitBuffer <<= 1;
                    }
                }

                byteBuffer[byteBufferPtr++] = fullBitBuffer;
                return byteBuffer;
            }

            // Otherwise, we need to read what we can
            int bitsRemaining = _bitsLeft;

            byte bitBuffer = 0;
            for (int i = 0; i < numBytes * 8; i++)
            {
                bitBuffer |= (byte)(_bitBuffer[i + _bitBuffer.Length - _bitsLeft--] ? 1 : 0);
                if (i % 8 == 7)
                {
                    byteBuffer[byteBufferPtr++] = bitBuffer;
                    bitBuffer = 0;
                }
                else
                {
                    bitBuffer <<= 1;
                }
            }

            // Fill the bit buffer, if possible
            FillBitBuffer();

            // If we couldn't read anything, throw an exception
            if (_buffer == null)
                throw new IndexOutOfRangeException();

            // Otherwise, read in the remaining bits needed
            for (int i = 0; i < bitsRemaining; i++)
            {
                bitBuffer |= (byte)(_bitBuffer[i + _bitBuffer.Length - _bitsLeft--] ? 1 : 0);
                if (i % 8 == 7)
                {
                    byteBuffer[byteBufferPtr++] = bitBuffer;
                    bitBuffer = 0;
                }
                else
                {
                    bitBuffer <<= 1;
                }
            }

            byteBuffer[byteBufferPtr++] = bitBuffer;
            return byteBuffer;
        }

        /// <summary>
        /// Read more than 0 bytes of data from the stream assuming MSB
        /// </summary>
        public byte[] ReadBytesMSB(int numBytes)
        {
            // If we are reading an invalid number of bytes
            if (numBytes < 0)
                throw new ArgumentOutOfRangeException();

            // Allocate the byte buffer
            byte[] byteBuffer = new byte[numBytes];
            int byteBufferPtr = 0;

            // If the bit buffer has the right number remaining
            if (_bitsLeft >= numBytes * 8)
            {
                byte fullBitBuffer = 0;
                for (int i = 0; i < numBytes * 8; i++)
                {
                    fullBitBuffer |= (byte)(_bitBuffer[i + _bitsLeft--] ? 1 : 0);
                    if (i % 8 == 7)
                    {
                        byteBuffer[byteBufferPtr++] = fullBitBuffer;
                        fullBitBuffer = 0;
                    }
                    else
                    {
                        fullBitBuffer <<= 1;
                    }
                }

                byteBuffer[byteBufferPtr++] = fullBitBuffer;
                return byteBuffer;
            }

            // Otherwise, we need to read what we can
            int bitsRemaining = _bitsLeft;

            byte bitBuffer = 0;
            for (int i = 0; i < numBytes * 8; i++)
            {
                bitBuffer |= (byte)(_bitBuffer[i + _bitsLeft--] ? 1 : 0);
                if (i % 8 == 7)
                {
                    byteBuffer[byteBufferPtr++] = bitBuffer;
                    bitBuffer = 0;
                }
                else
                {
                    bitBuffer <<= 1;
                }
            }

            // Fill the bit buffer, if possible
            FillBitBuffer();

            // If we couldn't read anything, throw an exception
            if (_buffer == null)
                throw new IndexOutOfRangeException();

            // Otherwise, read in the remaining bits needed
            for (int i = 0; i < bitsRemaining; i++)
            {
                bitBuffer |= (byte)(_bitBuffer[i + _bitsLeft--] ? 1 : 0);
                if (i % 8 == 7)
                {
                    byteBuffer[byteBufferPtr++] = bitBuffer;
                    bitBuffer = 0;
                }
                else
                {
                    bitBuffer <<= 1;
                }
            }

            byteBuffer[byteBufferPtr++] = bitBuffer;
            return byteBuffer;
        }

        /// <summary>
        /// Discard bits in the array up to the next byte boundary
        /// </summary>
        public void DiscardToByteBoundary()
        {
            int bitsToDiscard = _bitsLeft & 7;
            _bitsLeft -= bitsToDiscard;
        }

        /// <summary>
        /// Fill the internal bit buffer from the internal buffer
        /// </summary>
        /// <remarks>Fills up to 4 bytes worth of data at a time</remarks>
        private void FillBitBuffer()
        {
            // If we have 4 bytes left, just create the bit buffer directly
            if (_bufferPointer < _buffer.Length - 4)
            {
                // Read all 4 bytes directly
                byte[] readAllBytes = new ReadOnlySpan<byte>(_buffer, _bufferPointer, 4).ToArray();
                _bufferPointer += 4;

                // Create the new bit buffer
                _bitBuffer = new BitArray(readAllBytes);
                _bitsLeft = 32;
                return;
            }

            // If we have less than 4 bytes left, we need to get creative
            // Create the byte array to hold the data
            byte[] bytes = new byte[4];

            // Read what we can first
            int bytesRemaining = _buffer.Length - _bufferPointer;
            if (bytesRemaining > 0)
            {
                byte[] readBytesRemaining = new ReadOnlySpan<byte>(_buffer, _bufferPointer, bytesRemaining).ToArray();
                Array.Copy(readBytesRemaining, 0, bytes, 0, bytesRemaining);
                _bufferPointer += bytesRemaining;
            }

            // Fill the buffer, if we can
            FillBuffer();

            // If we couldn't read anything, reset the buffer
            if (_buffer == null && bytesRemaining == 4)
            {
                _bitBuffer = null;
                _bitsLeft = 0;
                return;
            }

            // If we don't have anything left, just create a bit array
            if (_buffer == null)
            {
                byte[] readBytesRemaining = new ReadOnlySpan<byte>(bytes, 0, bytesRemaining).ToArray();
                _bitBuffer = new BitArray(readBytesRemaining);
                _bitsLeft = 8 * bytesRemaining;
                return;
            }

            // Otherwise, we want to read in the remaining necessary bytes
            int bytesToRead = 4 - bytesRemaining;
            byte[] bytesRead = new ReadOnlySpan<byte>(_buffer, _bufferPointer, bytesToRead).ToArray();
            _bufferPointer += bytesToRead;

            Array.Copy(bytesRead, 0, bytes, bytesRemaining, bytesToRead);
            _bitBuffer = new BitArray(bytes);
            _bitsLeft = 32;
        }

        /// <summary>
        /// Fill the internal buffer from the original data source
        /// </summary>
        /// <remarks>Reads up to 4096 bytes at a time</remarks>
        private void FillBuffer()
        {
            // Get the amount of bytes to read
            int bytesRemaining = (int)(_dataStream.Length - _dataStream.Position);
            int bytesToRead = Math.Min(bytesRemaining, 4096);

            // If we can't ready any bytes, reset the buffer
            if (bytesToRead == 0)
            {
                _buffer = null;
                _bufferPointer = -1;
                return;
            }

            // Otherwise, read and reset the position
            _buffer = _dataStream.ReadBytes(bytesToRead);
            _bufferPointer = 0;
        }
    }

    public class MSZIPDeflate
    {
        #region Constants

        /// <summary>
        /// Maximum Huffman code bit count
        /// </summary>
        public const int MAX_BITS = 16;

        #endregion

        #region Properties

        /// <summary>
        /// Match lengths for literal codes 257..285
        /// </summary>
        /// <remarks>Each value here is the lower bound for lengths represented</remarks>
        public static Dictionary<int, int> LiteralLengths
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
        public static Dictionary<int, int> LiteralExtraBits
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
        public static readonly int[] DistanceExtraBits = new int[30]
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3,
            4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
            9, 9, 10, 10, 11, 11, 12, 12, 13, 13,
        };

        /// <summary>
        /// The order of the bit length Huffman code lengths
        /// </summary>
        public static readonly int[] BitLengthOrder = new int[19]
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

        /// <summary>
        /// The decoding algorithm for the actual data
        /// </summary>
        public static void Decode(MSZIPDeflateStream data)
        {
            // Create the output byte array
            List<byte> decodedBytes = new List<byte>();

            // Create the loop variable block
            MSZIPDeflateBlock block;

            do
            {
                ulong header = data.ReadBitsLSB(3);
                block = new MSZIPDeflateBlock(header);

                // We should never get a reserved block
                if (block.BTYPE == MSZIPDeflateCompressionType.Reserved)
                    throw new Exception();

                // If stored with no compression
                if (block.BTYPE == MSZIPDeflateCompressionType.NoCompression)
                {
                    // Skip any remaining bits in current partially processed byte
                    data.DiscardToByteBoundary();

                    // Read LEN and NLEN
                    byte[] nonCompressedHeader = data.ReadBytesLSB(4);
                    block.BlockData = new MSZIPNonCompressedBlock(nonCompressedHeader);

                    // Copy LEN bytes of data to output
                    ushort length = ((MSZIPNonCompressedBlock)block.BlockData).LEN;
                    ((MSZIPNonCompressedBlock)block.BlockData).Data = data.ReadBytesLSB(length);
                    decodedBytes.AddRange(((MSZIPNonCompressedBlock)block.BlockData).Data);
                }

                // Otherwise
                else
                {
                    // If compressed with dynamic Huffman codes
                    // read representation of code trees
                    block.BlockData = block.BTYPE == MSZIPDeflateCompressionType.DynamicHuffman
                        ? (IMSZIPBlockData)new MSZIPDynamicHuffmanCompressedBlock(data)
                        : (IMSZIPBlockData)new MSZIPFixedHuffmanCompressedBlock();

                    var compressedBlock = (block.BlockData as MSZIPCompressedBlock);

                    // 9 bits per entry, 288 max symbols
                    int[] literalDecodeTable = CreateTable(compressedBlock.LiteralLengths);

                    // 6 bits per entry, 32 max symbols
                    int[] distanceDecodeTable = CreateTable(compressedBlock.DistanceCodes);

                    // Loop until end of block code recognized
                    while (true)
                    {
                        // Decode literal/length value from input stream
                        int symbol = literalDecodeTable[data.ReadBitsLSB(9)];

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
                            ulong length = data.ReadBitsLSB(LiteralExtraBits[symbol]);
                            length += (ulong)LiteralLengths[symbol];

                            int code = distanceDecodeTable[length];

                            ulong distance = data.ReadBitsLSB(DistanceExtraBits[code]);
                            distance += (ulong)DistanceOffsets[code];

                            // Decode distance from input stream

                            // Move backwards distance bytes in the output
                            // stream, and copy length bytes from this
                            // position to the output stream.
                        }
                    }
                }
            } while (!block.BFINAL);

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
        }

        /// <summary>
        /// Given this rule, we can define the Huffman code for an alphabet
        /// just by giving the bit lengths of the codes for each symbol of
        /// the alphabet in order; this is sufficient to determine the
        /// actual codes.  In our example, the code is completely defined
        /// by the sequence of bit lengths (2, 1, 3, 3).  The following
        /// algorithm generates the codes as integers, intended to be read
        /// from most- to least-significant bit.  The code lengths are
        /// initially in tree[I].Len; the codes are produced in
        /// tree[I].Code.
        /// </summary>
        public static void CreateTable(MSZIPCompressedBlock tree)
        {
            // Count the number of codes for each code length.  Let
            // bl_count[N] be the number of codes of length N, N >= 1.
            var bl_count = new Dictionary<int, int>();
            for (int i = 0; i < tree.LiteralLengths.Length; i++)
            {
                if (!bl_count.ContainsKey(tree.LiteralLengths[i]))
                    bl_count[tree.LiteralLengths[i]] = 0;

                bl_count[tree.LiteralLengths[i]]++;
            }

            // Find the numerical value of the smallest code for each
            // code length:
            var next_code = new Dictionary<int, int>();
            int code = 0;
            bl_count[0] = 0;
            for (int bits = 1; bits <= MAX_BITS; bits++)
            {
                code = (code + bl_count[bits - 1]) << 1;
                next_code[bits] = code;
            }

            // Assign numerical values to all codes, using consecutive
            // values for all codes of the same length with the base
            // values determined at step 2. Codes that are never used
            // (which have a bit length of zero) must not be assigned a
            // value.
            for (int n = 0; n <= tree.LiteralLengths.Length; n++)
            {
                int len = tree.LiteralLengths[n];
                if (len != 0)
                {
                    tree.DistanceCodes[n] = next_code[len];
                    next_code[len]++;
                }
            }
        }

        /// <summary>
        /// Given this rule, we can define the Huffman code for an alphabet
        /// just by giving the bit lengths of the codes for each symbol of
        /// the alphabet in order; this is sufficient to determine the
        /// actual codes.  In our example, the code is completely defined
        /// by the sequence of bit lengths (2, 1, 3, 3).  The following
        /// algorithm generates the codes as integers, intended to be read
        /// from most- to least-significant bit.  The code lengths are
        /// initially in tree[I].Len; the codes are produced in
        /// tree[I].Code.
        /// </summary>
        public static int[] CreateTable(int[] lengths)
        {
            // Count the number of codes for each code length.  Let
            // bl_count[N] be the number of codes of length N, N >= 1.
            var bl_count = new Dictionary<int, int>();
            for (int i = 0; i < lengths.Length; i++)
            {
                if (!bl_count.ContainsKey(lengths[i]))
                    bl_count[lengths[i]] = 0;

                bl_count[lengths[i]]++;
            }

            // Find the numerical value of the smallest code for each
            // code length:
            var next_code = new Dictionary<int, int>();
            int code = 0;
            bl_count[0] = 0;
            for (int bits = 1; bits <= MAX_BITS; bits++)
            {
                code = (code + bl_count[bits - 1]) << 1;
                next_code[bits] = code;
            }

            // Assign numerical values to all codes, using consecutive
            // values for all codes of the same length with the base
            // values determined at step 2. Codes that are never used
            // (which have a bit length of zero) must not be assigned a
            // value.
            int[] distances = new int[lengths.Length];
            for (int n = 0; n <= lengths.Length; n++)
            {
                int len = lengths[n];
                if (len != 0)
                {
                    distances[n] = next_code[len];
                    next_code[len]++;
                }
            }

            return distances;
        }
    }

    public class MSZIPDeflateBlock
    {
        #region Properties

        /// <summary>
        /// Set if and only if this is the last block of the data set.
        /// </summary>
        /// <remarks>Bit 0</remarks>
        public bool BFINAL { get; set; }

        /// <summary>
        /// Specifies how the data are compressed
        /// </summary>
        /// <remarks>Bits 1-2</remarks>
        public MSZIPDeflateCompressionType BTYPE { get; set; }

        /// <summary>
        /// Block data as defined by the compression type
        /// </summary>
        public IMSZIPBlockData BlockData { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MSZIPDeflateBlock(ulong header)
        {
            BFINAL = (header & 0b100) != 0;
            BTYPE = (MSZIPDeflateCompressionType)(header & 0b011);
        }
    }

    /// <summary>
    /// Empty interface defining block types
    /// </summary>
    public interface IMSZIPBlockData { }

    /// <summary>
    /// Non-compressed blocks (BTYPE=00)
    /// </summary>
    public class MSZIPNonCompressedBlock : IMSZIPBlockData
    {
        #region Properties

        /// <summary>
        /// The number of data bytes in the block
        /// </summary>
        /// <remarks>Bytes 0-1</remarks>
        public ushort LEN { get; set; }

        /// <summary>
        /// The one's complement of LEN
        /// </summary>
        /// <remarks>Bytes 2-3</remarks>
        public ushort NLEN { get; set; }

        /// <summary>
        /// <see cref="LEN"/> bytes of literal data
        /// </summary>
        public byte[] Data { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MSZIPNonCompressedBlock(byte[] header)
        {
            // If we have invalid header data
            if (header == null || header.Length < 4)
                throw new ArgumentException();

            int offset = 0;
            LEN = header.ReadUInt16(ref offset);
            NLEN = header.ReadUInt16(ref offset);

            // TODO: Confirm NLEN is 1's compliment of LEN
        }
    }

    /// <summary>
    /// Base class for compressed blocks
    /// </summary>
    public abstract class MSZIPCompressedBlock : IMSZIPBlockData
    {
        /// <summary>
        /// Huffman code lengths for the literal / length alphabet
        /// </summary>
        public abstract int[] LiteralLengths { get; }

        /// <summary>
        /// Huffman distance codes for the literal / length alphabet
        /// </summary>
        public abstract int[] DistanceCodes { get; }
    }

    /// <summary>
    /// Compression with fixed Huffman codes (BTYPE=01)
    /// </summary>
    public class MSZIPFixedHuffmanCompressedBlock : MSZIPCompressedBlock
    {
        #region Properties

        /// <inheritdoc/>
        public override int[] LiteralLengths
        {
            get
            {
                // If we have cached lengths, use those
                if (_literalLengths != null)
                    return _literalLengths;

                // Otherwise, build it from scratch
                _literalLengths = new int[288];

                // Literal Value 0 - 143, 8 bits
                for (int i = 0; i < 144; i++)
                    _literalLengths[i] = 8;

                // Literal Value 144 - 255, 9 bits
                for (int i = 144; i < 256; i++)
                    _literalLengths[i] = 9;

                // Literal Value 256 - 279, 7 bits
                for (int i = 256; i < 280; i++)
                    _literalLengths[i] = 7;

                // Literal Value 280 - 287, 8 bits
                for (int i = 280; i < 288; i++)
                    _literalLengths[i] = 8;

                return _literalLengths;
            }
        }

        /// <inheritdoc/>
        public override int[] DistanceCodes
        {
            get
            {
                // If we have cached distances, use those
                if (_distanceCodes != null)
                    return _distanceCodes;

                // Otherwise, build it from scratch
                _distanceCodes = new int[32];

                // Fixed length, 5 bits
                for (int i = 0; i < 32; i++)
                    _distanceCodes[i] = 5;

                return _distanceCodes;
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Huffman code lengths for the literal / length alphabet
        /// </summary>
        private int[] _literalLengths = null;

        /// <summary>
        /// Huffman distance codes for the literal / length alphabet
        /// </summary>
        private int[] _distanceCodes = null;

        #endregion
    }

    /// <summary>
    /// Compression with dynamic Huffman codes (BTYPE=10)
    /// </summary>
    public class MSZIPDynamicHuffmanCompressedBlock : MSZIPCompressedBlock
    {
        #region Properties

        /// <inheritdoc/>
        public override int[] LiteralLengths { get; } = new int[19];

        /// <inheritdoc/>
        public override int[] DistanceCodes { get; } = new int[19];

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MSZIPDynamicHuffmanCompressedBlock(MSZIPDeflateStream stream)
        {
            // # of Literal/Length codes - 257
            ulong HLIT = stream.ReadBitsLSB(5) + 257;

            // # of Distance codes - 1
            ulong HDIST = stream.ReadBitsLSB(5) + 1;

            // HCLEN, # of Code Length codes - 4
            ulong HCLEN = stream.ReadBitsLSB(5) + 4;

            // (HCLEN + 4) x 3 bits: code lengths for the code length
            //  alphabet given just above
            // 
            //  These code lengths are interpreted as 3-bit integers
            //  (0-7); as above, a code length of 0 means the
            //  corresponding symbol (literal/ length or distance code
            //  length) is not used.
            int[] codeLengthAlphabet = new int[19];
            for (ulong i = 0; i < HCLEN; i++)
                codeLengthAlphabet[MSZIPDeflate.BitLengthOrder[i]] = (int)stream.ReadBitsLSB(3);

            for (ulong i = HCLEN; i < 19; i++)
                codeLengthAlphabet[MSZIPDeflate.BitLengthOrder[i]] = 0;

            // Code length Huffman code
            int[] codeLengthHuffmanCode = MSZIPDeflate.CreateTable(codeLengthAlphabet);

            // HLIT + 257 code lengths for the literal/length alphabet,
            //  encoded using the code length Huffman code
            this.LiteralLengths = BuildHuffmanTree(stream, HLIT, codeLengthHuffmanCode);

            // HDIST + 1 code lengths for the distance alphabet,
            //  encoded using the code length Huffman code
            this.DistanceCodes = BuildHuffmanTree(stream, HDIST, codeLengthHuffmanCode);
        }

        /// <summary>
        /// The alphabet for code lengths is as follows
        /// </summary>
        private int[] BuildHuffmanTree(MSZIPDeflateStream stream, ulong codeCount, int[] codeLengths)
        {
            // Setup the huffman tree
            int[] tree = new int[codeCount];

            // Setup the loop variables
            int lastCode = 0, repeatLength = 0;
            for (ulong i = 0; i < codeCount; i++)
            {
                int code = codeLengths[(int)stream.ReadBitsLSB(7)];

                // Represent code lengths of 0 - 15
                if (code > 0 && code <= 15)
                {
                    lastCode = code;
                    tree[i] = code;
                }

                // Copy the previous code length 3 - 6 times.
                // The next 2 bits indicate repeat length (0 = 3, ... , 3 = 6)
                // Example:  Codes 8, 16 (+2 bits 11), 16 (+2 bits 10) will expand to 12 code lengths of 8 (1 + 6 + 5)
                else if (code == 16)
                {
                    repeatLength = (int)stream.ReadBitsLSB(2);
                    repeatLength += 2;
                    code = lastCode;
                }

                // Repeat a code length of 0 for 3 - 10 times.
                // (3 bits of length)
                else if (code == 17)
                {
                    repeatLength = (int)stream.ReadBitsLSB(3);
                    repeatLength += 3;
                    code = 0;
                }

                // Repeat a code length of 0 for 11 - 138 times
                // (7 bits of length)
                else if (code == 18)
                {
                    repeatLength = (int)stream.ReadBitsLSB(7);
                    repeatLength += 11;
                    code = 0;
                }

                // Everything else
                else
                {
                    throw new ArgumentOutOfRangeException();
                }

                // If we had a repeat length
                for (; repeatLength > 0; repeatLength--)
                {
                    tree[i++] = code;
                }
            }

            return tree;
        }
    }

    #endregion
}
