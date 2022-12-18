using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Utilities;

namespace BurnOutSharp.Wrappers
{
    public class MicrosoftCabinet : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Signature"/>
        public uint Signature => _cabinet.Header.Signature;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved1"/>
        public uint Reserved1 => _cabinet.Header.Reserved1;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetSize"/>
        public uint CabinetSize => _cabinet.Header.CabinetSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved2"/>
        public uint Reserved2 => _cabinet.Header.Reserved2;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FilesOffset"/>
        public uint FilesOffset => _cabinet.Header.FilesOffset;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved3"/>
        public uint Reserved3 => _cabinet.Header.Reserved3;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.VersionMinor"/>
        public byte VersionMinor => _cabinet.Header.VersionMinor;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.VersionMajor"/>
        public byte VersionMajor => _cabinet.Header.VersionMajor;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FolderCount"/>
        public ushort FolderCount => _cabinet.Header.FolderCount;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FileCount"/>
        public ushort FileCount => _cabinet.Header.FileCount;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FileCount"/>
        public Models.MicrosoftCabinet.HeaderFlags Flags => _cabinet.Header.Flags;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.SetID"/>
        public ushort SetID => _cabinet.Header.SetID;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetIndex"/>
        public ushort CabinetIndex => _cabinet.Header.CabinetIndex;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.HeaderReservedSize"/>
        public ushort HeaderReservedSize => _cabinet.Header.HeaderReservedSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FolderReservedSize"/>
        public byte FolderReservedSize => _cabinet.Header.FolderReservedSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DataReservedSize"/>
        public byte DataReservedSize => _cabinet.Header.DataReservedSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.ReservedData"/>
        public byte[] ReservedData => _cabinet.Header.ReservedData;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetPrev"/>
        public string CabinetPrev => _cabinet.Header.CabinetPrev;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DiskPrev"/>
        public string DiskPrev => _cabinet.Header.DiskPrev;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetNext"/>
        public string CabinetNext => _cabinet.Header.CabinetNext;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DiskNext"/>
        public string DiskNext => _cabinet.Header.DiskNext;

        #endregion

        #region Folders

        /// <inheritdoc cref="Models.MicrosoftCabinet.Cabinet.Folders"/>
        public Models.MicrosoftCabinet.CFFOLDER[] Folders => _cabinet.Folders;

        #endregion

        #region Files

        /// <inheritdoc cref="Models.MicrosoftCabinet.Cabinet.Files"/>
        public Models.MicrosoftCabinet.CFFILE[] Files => _cabinet.Files;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the cabinet
        /// </summary>
        private Models.MicrosoftCabinet.Cabinet _cabinet;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private MicrosoftCabinet() { }

        /// <summary>
        /// Create a Microsoft Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static MicrosoftCabinet Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Microsoft Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static MicrosoftCabinet Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = Builder.MicrosoftCabinet.ParseCabinet(data);
            if (cabinet == null)
                return null;

            var wrapper = new MicrosoftCabinet
            {
                _cabinet = cabinet,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Checksumming

        /// <summary>
        /// The computation and verification of checksums found in CFDATA structure entries cabinet files is
        /// done by using a function described by the following mathematical notation. When checksums are
        /// not supplied by the cabinet file creating application, the checksum field is set to 0 (zero). Cabinet
        /// extracting applications do not compute or verify the checksum if the field is set to 0 (zero).
        /// </summary>
        private static uint ChecksumData(byte[] data)
        {
            uint[] C = new uint[4]
            {
                S(data, 1, data.Length),
                S(data, 2, data.Length),
                S(data, 3, data.Length),
                S(data, 4, data.Length),
            };

            return C[0] ^ C[1] ^ C[2] ^ C[3];
        }

        /// <summary>
        /// Individual algorithmic step
        /// </summary>
        private static uint S(byte[] a, int b, int x)
        {
            int n = a.Length;

            if (x < 4 && b > n % 4)
                return 0;
            else if (x < 4 && b <= n % 4)
                return a[n - b + 1];
            else // if (x >= 4)
                return a[n - x + b] ^ S(a, b, x - 4);
        }
        
        #endregion

        #region Compression

        #region LZX

        // TODO: Implement LZX decompression

        #endregion

        #region MSZIP

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
        private static Models.MicrosoftCabinet.MSZIP.BlockHeader AsBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.MicrosoftCabinet.MSZIP.BlockHeader();
        
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
        private static Models.MicrosoftCabinet.MSZIP.DeflateBlockHeader AsDeflateBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.MicrosoftCabinet.MSZIP.DeflateBlockHeader();
        
            header.BFINAL = data.ReadBits(1)[0];
            header.BTYPE = (Models.MicrosoftCabinet.DeflateCompressionType)data.ReadBits(2).AsByte();

            return header;
        }

        /// <summary>
        /// Read the block header from the block data, if possible
        /// </summary>
        /// <param name="data">Byte array representing the block</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>Filled dynamic Huffman compressed block header on success, null on error</returns>
        private static Models.MicrosoftCabinet.MSZIP.DynamicHuffmanCompressedBlockHeader AsDynamicHuffmanCompressedBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.MicrosoftCabinet.MSZIP.DynamicHuffmanCompressedBlockHeader();

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
            for (ulong i = 0; i < HCLEN; i++)
                bitLengths[BitLengthOrder[i]] = data.ReadBits(3).AsByte();

            // Code length Huffman code
            int[] bitLengthTable = CreateTable(bitLengths);

            // HLIT + 257 code lengths for the literal/length alphabet,
            //  encoded using the code length Huffman code
            header.LiteralLengths = BuildHuffmanTree(data, HLIT, bitLengthTable);

            // HDIST + 1 code lengths for the distance alphabet,
            //  encoded using the code length Huffman code
            header.DistanceCodes = BuildHuffmanTree(data, HDIST, bitLengthTable);

            return header;
        }

        /// <summary>
        /// Read the block header from the block data, if possible
        /// </summary>
        /// <param name="data">Byte array representing the block</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>Filled non-compressed block header on success, null on error</returns>
        private static Models.MicrosoftCabinet.MSZIP.NonCompressedBlockHeader AsNonCompressedBlockHeader(BitStream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            var header = new Models.MicrosoftCabinet.MSZIP.NonCompressedBlockHeader();

            header.LEN = data.ReadAlignedUInt16();
            header.NLEN = data.ReadAlignedUInt16();
            // TODO: Confirm NLEN is 1's compliment of LEN

            return header;
        }

        #endregion

        #region Helpers
        
        /// <summary>
        /// The alphabet for code lengths is as follows
        /// </summary>
        private static int[] BuildHuffmanTree(BitStream data, ushort codeCount, int[] codeLengths)
        {
            // Setup the huffman tree
            int[] tree = new int[codeCount];

            // Setup the loop variables
            int lastCode = 0, repeatLength = 0;
            for (ulong i = 0; i < codeCount; i++)
            {
                int codeLength = codeLengths[data.ReadBits(7).AsUInt16()];
                if (codeLengths[codeLength] > 7)
                    _ = data.ReadBits(codeLengths[codeLength] - 7);

                // Represent code lengths of 0 - 15
                if (codeLength > 0 && codeLength <= 15)
                {
                    lastCode = codeLength;
                    tree[i] = codeLength;
                }

                // Copy the previous code length 3 - 6 times.
                // The next 2 bits indicate repeat length (0 = 3, ... , 3 = 6)
                // Example: Codes 8, 16 (+2 bits 11), 16 (+2 bits 10) will expand to 12 code lengths of 8 (1 + 6 + 5)
                else if (codeLength == 16)
                {
                    repeatLength = data.ReadBits(2).AsByte();
                    repeatLength += 2;
                    codeLength = lastCode;
                }

                // Repeat a code length of 0 for 3 - 10 times.
                // (3 bits of length)
                else if (codeLength == 17)
                {
                    repeatLength = data.ReadBits(3).AsByte();
                    repeatLength += 3;
                    codeLength = 0;
                }

                // Repeat a code length of 0 for 11 - 138 times
                // (7 bits of length)
                else if (codeLength == 18)
                {
                    repeatLength = data.ReadBits(7).AsByte();
                    repeatLength += 11;
                    codeLength = 0;
                }

                // Everything else
                else
                {
                    throw new ArgumentOutOfRangeException();
                }

                // If we had a repeat length
                for (; repeatLength > 0; repeatLength--)
                {
                    tree[i++] = codeLength;
                }
            }

            return tree;
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
        private static int[] CreateTable(int[] lengths)
        {
            // Count the number of codes for each code length.  Let
            // bl_count[N] be the number of codes of length N, N >= 1.
            int[] bl_count = new int[259];
            for (int i = 0; i < lengths.Length; i++)
            {
                bl_count[lengths[i]]++;
            }

            // Find the numerical value of the smallest code for each
            // code length.
            int[] next_code = new int[MAX_BITS + 1];
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
            for (int n = 0; n < lengths.Length; n++)
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

        #endregion

        #endregion

        #region Quantum

        // TODO: Implement Quantum decompression

        #endregion

        #endregion

        #region Folders

        /// <summary>
        /// Get the uncompressed data associated with a folder
        /// </summary>
        /// <param name="folderIndex">Folder index to check</param>
        /// <returns>Byte array representing the data, null on error</returns>
        /// <remarks>All but uncompressed are unimplemented</remarks>
        public byte[] GetUncompressedData(int folderIndex)
        {
            // If we have an invalid folder index
            if (folderIndex < 0 || folderIndex >= Folders.Length)
                return null;

            // Get the folder header
            var folder = Folders[folderIndex];
            if (folder == null)
                return null;

            // If we have invalid data blocks
            if (folder.DataBlocks == null || folder.DataBlocks.Count == 0)
                return null;

            // Store the last decompressed block for MS-ZIP
            byte[] lastDecompressed = null;

            List<byte> data = new List<byte>();
            foreach (var dataBlock in folder.DataBlocks.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
            {
                byte[] decompressed = null;
                switch (folder.CompressionType)
                {
                    case Models.MicrosoftCabinet.CompressionType.TYPE_NONE:
                        decompressed = dataBlock.CompressedData;
                        break;
                    case Models.MicrosoftCabinet.CompressionType.TYPE_MSZIP:
                        decompressed = DecompressMSZIPData(dataBlock.CompressedData);
                        break;
                    case Models.MicrosoftCabinet.CompressionType.TYPE_QUANTUM:
                        // TODO: UNIMPLEMENTED
                        decompressed = dataBlock.CompressedData;
                        break;
                    case Models.MicrosoftCabinet.CompressionType.TYPE_LZX:
                        // TODO: UNIMPLEMENTED
                        decompressed = dataBlock.CompressedData;
                        break;
                    default:
                        return null;
                }

                lastDecompressed = decompressed;
                if (decompressed != null)
                    data.AddRange(decompressed);
            }

            return data.ToArray();
        }

        /// <summary>
        /// Decompress MSZIP data
        /// </summary>
        private byte[] DecompressMSZIPData(byte[] data)
        {
            // Create the bitstream to read from
            var dataStream = new BitStream(data);

            // Get the block header
            var blockHeader = AsBlockHeader(dataStream);
            if (blockHeader == null)
                return null;

            // Create the output byte array
            List<byte> decodedBytes = new List<byte>();

            // Create the loop variable block
            Models.MicrosoftCabinet.MSZIP.DeflateBlockHeader deflateBlockHeader;

            do
            {
                deflateBlockHeader = AsDeflateBlockHeader(dataStream);

                // We should never get a reserved block
                if (deflateBlockHeader.BTYPE == Models.MicrosoftCabinet.DeflateCompressionType.Reserved)
                    throw new Exception();

                // If stored with no compression
                if (deflateBlockHeader.BTYPE == Models.MicrosoftCabinet.DeflateCompressionType.NoCompression)
                {
                    // Skip any remaining bits in current partially processed byte
                    dataStream.DiscardBuffer();

                    // Read the block header
                    deflateBlockHeader.BlockDataHeader = AsNonCompressedBlockHeader(dataStream);

                    // Copy LEN bytes of data to output
                    var header = deflateBlockHeader.BlockDataHeader as Models.MicrosoftCabinet.MSZIP.NonCompressedBlockHeader;
                    ushort length = header.LEN;
                    decodedBytes.AddRange(dataStream.ReadAlignedBytes(length));
                }

                // Otherwise
                else
                {
                    // If compressed with dynamic Huffman codes
                    // read representation of code trees
                    deflateBlockHeader.BlockDataHeader = deflateBlockHeader.BTYPE == Models.MicrosoftCabinet.DeflateCompressionType.DynamicHuffman
                        ? (Models.MicrosoftCabinet.MSZIP.IBlockDataHeader)AsDynamicHuffmanCompressedBlockHeader(dataStream)
                        : (Models.MicrosoftCabinet.MSZIP.IBlockDataHeader)new Models.MicrosoftCabinet.MSZIP.FixedHuffmanCompressedBlockHeader();

                    var header = deflateBlockHeader.BlockDataHeader as Models.MicrosoftCabinet.MSZIP.CompressedBlockHeader;

                    // 9 bits per entry, 288 max symbols
                    int[] literalDecodeTable = CreateTable(header.LiteralLengths);

                    // 6 bits per entry, 32 max symbols
                    int[] distanceDecodeTable = CreateTable(header.DistanceCodes);

                    // Loop until end of block code recognized
                    while (true)
                    {
                        // Decode literal/length value from input stream
                        int symbol = literalDecodeTable[dataStream.ReadBits(9).AsUInt16()];

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

        #region Files

        /// <summary>
        /// Extract a single file to an output directory
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <param name="outputDirectory">Output directory to use for writing</param>
        /// <returns>Byte array representing the data, null on error</returns>
        public bool ExtractFile(int fileIndex, string outputDirectory)
        {
            // If we have an invalid file index
            if (fileIndex < 0 || fileIndex >= Files.Length)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Ensure the directory exists
            Directory.CreateDirectory(outputDirectory);

            // Get the file header
            var file = Files[fileIndex];
            if (file == null || file.FileSize == 0)
                return false;

            // Create the output filename
            string fileName = Path.Combine(outputDirectory, file.Name);

            // Get the file data, if possible
            byte[] fileData = GetFileData(fileIndex);
            if (fileData == null)
                return false;

            // Write the file data
            using (FileStream fs = File.OpenWrite(fileName))
            {
                fs.Write(fileData, 0, fileData.Length);
            }

            return true;
        }

        /// <summary>
        /// Get the DateTime for a particular file index
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <returns>DateTime representing the file time, null on error</returns>
        public DateTime? GetDateTime(int fileIndex)
        {
            // If we have an invalid file index
            if (fileIndex < 0 || fileIndex >= Files.Length)
                return null;

            // Get the file header
            var file = Files[fileIndex];
            if (file == null)
                return null;

            // If we have an invalid DateTime
            if (file.Date == 0 && file.Time == 0)
                return null;

            try
            {
                // Date property
                int year = (file.Date >> 9) + 1980;
                int month = (file.Date >> 5) & 0x0F;
                int day = file.Date & 0x1F;

                // Time property
                int hour = file.Time >> 11;
                int minute = (file.Time >> 5) & 0x3F;
                int second = (file.Time << 1) & 0x3E;

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Get the uncompressed data associated with a file
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <returns>Byte array representing the data, null on error</returns>
        public byte[] GetFileData(int fileIndex)
        {
            // If we have an invalid file index
            if (fileIndex < 0 || fileIndex >= Files.Length)
                return null;

            // Get the file header
            var file = Files[fileIndex];
            if (file == null || file.FileSize == 0)
                return null;

            // Get the parent folder data
            byte[] folderData = GetUncompressedData((int)file.FolderIndex);
            if (folderData == null)
                return null;

            // Get the segment that represents this file
            byte[] fileData = new byte[file.FileSize];
            Array.Copy(folderData, file.FolderStartOffset, fileData, 0, file.FileSize);
            return fileData;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("Microsoft Cabinet Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintFolders();
            PrintFiles();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Reserved 1: {Reserved1}");
            Console.WriteLine($"  Cabinet size: {CabinetSize}");
            Console.WriteLine($"  Reserved 2: {Reserved2}");
            Console.WriteLine($"  Files offset: {FilesOffset}");
            Console.WriteLine($"  Reserved 3: {Reserved3}");
            Console.WriteLine($"  Minor version: {VersionMinor}");
            Console.WriteLine($"  Major version: {VersionMajor}");
            Console.WriteLine($"  Folder count: {FolderCount}");
            Console.WriteLine($"  File count: {FileCount}");
            Console.WriteLine($"  Flags: {Flags}");
            Console.WriteLine($"  Set ID: {SetID}");
            Console.WriteLine($"  Cabinet index: {CabinetIndex}");

            if (Flags.HasFlag(Models.MicrosoftCabinet.HeaderFlags.RESERVE_PRESENT))
            {
                Console.WriteLine($"  Header reserved size: {HeaderReservedSize}");
                Console.WriteLine($"  Folder reserved size: {FolderReservedSize}");
                Console.WriteLine($"  Data reserved size: {DataReservedSize}");
                if (ReservedData == null)
                    Console.WriteLine($"  Reserved data = [NULL]");
                else
                    Console.WriteLine($"  Reserved data = {BitConverter.ToString(ReservedData).Replace("-", " ")}");
            }

            if (Flags.HasFlag(Models.MicrosoftCabinet.HeaderFlags.PREV_CABINET))
            {
                Console.WriteLine($"  Previous cabinet: {CabinetPrev}");
                Console.WriteLine($"  Previous disk: {DiskPrev}");
            }

            if (Flags.HasFlag(Models.MicrosoftCabinet.HeaderFlags.NEXT_CABINET))
            {
                Console.WriteLine($"  Next cabinet: {CabinetNext}");
                Console.WriteLine($"  Next disk: {DiskNext}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print folders information
        /// </summary>
        private void PrintFolders()
        {
            Console.WriteLine("  Folders:");
            Console.WriteLine("  -------------------------");
            if (FolderCount == 0 || Folders == null || Folders.Length == 0)
            {
                Console.WriteLine("  No folders");
            }
            else
            {
                for (int i = 0; i < Folders.Length; i++)
                {
                    var entry = Folders[i];
                    Console.WriteLine($"  Folder {i}");
                    Console.WriteLine($"    Cab start offset = {entry.CabStartOffset}");
                    Console.WriteLine($"    Data count = {entry.DataCount}");
                    Console.WriteLine($"    Compression type = {entry.CompressionType}");
                    if (entry.ReservedData == null)
                        Console.WriteLine($"    Reserved data = [NULL]");
                    else
                        Console.WriteLine($"    Reserved data = {BitConverter.ToString(entry.ReservedData).Replace("-", " ")}");
                    Console.WriteLine();

                    Console.WriteLine("    Data Blocks");
                    Console.WriteLine("    -------------------------");
                    if (entry.DataBlocks == null || entry.DataBlocks.Count == 0)
                    {
                        Console.WriteLine("    No data blocks");
                    }
                    else
                    {
                        foreach (var block in entry.DataBlocks)
                        {
                            Console.WriteLine($"    Data Block at offset {block.Key}");
                            Console.WriteLine($"      Checksum = {block.Value.Checksum}");
                            Console.WriteLine($"      Compressed size = {block.Value.CompressedSize}");
                            Console.WriteLine($"      Uncompressed size = {block.Value.UncompressedSize}");
                            if (block.Value.ReservedData == null)
                                Console.WriteLine($"      Reserved data = [NULL]");
                            else
                                Console.WriteLine($"      Reserved data = {BitConverter.ToString(block.Value.ReservedData).Replace("-", " ")}");
                            //Console.WriteLine($"      Compressed data = {BitConverter.ToString(block.Value.CompressedData).Replace("-", " ")}");
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print files information
        /// </summary>
        private void PrintFiles()
        {
            Console.WriteLine("  Files:");
            Console.WriteLine("  -------------------------");
            if (FileCount == 0 || Files == null || Files.Length == 0)
            {
                Console.WriteLine("  No files");
            }
            else
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    var entry = Files[i];
                    Console.WriteLine($"  File {i}");
                    Console.WriteLine($"    File size = {entry.FileSize}");
                    Console.WriteLine($"    Folder start offset = {entry.FolderStartOffset}");
                    Console.WriteLine($"    Folder index = {entry.FolderIndex}");
                    Console.WriteLine($"    Date = {entry.Date}");
                    Console.WriteLine($"    Time = {entry.Time}");
                    Console.WriteLine($"    Attributes = {entry.Attributes}");
                    Console.WriteLine($"    Name = {entry.Name}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}
