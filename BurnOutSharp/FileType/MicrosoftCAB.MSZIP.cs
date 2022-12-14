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
    public enum DeflateCompressionType : byte
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

    public class MSZIPDeflate
    {
        /*
        3.2.5. Compressed blocks (length and distance codes)

         As noted above, encoded data blocks in the "deflate" format
         consist of sequences of symbols drawn from three conceptually
         distinct alphabets: either literal bytes, from the alphabet of
         byte values(0..255), or<length, backward distance> pairs,
         where the length is drawn from(3..258) and the distance is
         drawn from(1..32,768).  In fact, the literal and length
         alphabets are merged into a single alphabet(0..285), where
         values 0..255 represent literal bytes, the value 256 indicates
         end-of-block, and values 257..285 represent length codes
         (possibly in conjunction with extra bits following the symbol
         code) as follows:

                 Extra Extra               Extra
            Code Bits Length(s) Code Bits Lengths Code Bits Length(s)
            ---- ---- ------     ---- ---- -------   ---- ---- -------
             257   0     3       267   1   15,16     277   4   67-82
             258   0     4       268   1   17,18     278   4   83-98
             259   0     5       269   2   19-22     279   4   99-114
             260   0     6       270   2   23-26     280   4  115-130
             261   0     7       271   2   27-30     281   5  131-162
             262   0     8       272   2   31-34     282   5  163-194
             263   0     9       273   3   35-42     283   5  195-226
             264   0    10       274   3   43-50     284   5  227-257
             265   1  11,12      275   3   51-58     285   0    258
             266   1  13,14      276   3   59-66

         The extra bits should be interpreted as a machine integer
         stored with the most-significant bit first, e.g., bits 1110
         represent the value 14.

                  Extra Extra               Extra
             Code Bits Dist  Code Bits   Dist Code Bits Distance
             ---- ---- ----  ---- ----  ------    ---- ---- --------
               0   0    1     10   4     33-48    20    9   1025-1536
               1   0    2     11   4     49-64    21    9   1537-2048
               2   0    3     12   5     65-96    22   10   2049-3072
               3   0    4     13   5     97-128   23   10   3073-4096
               4   1   5,6    14   6    129-192   24   11   4097-6144
               5   1   7,8    15   6    193-256   25   11   6145-8192
               6   2   9-12   16   7    257-384   26   12  8193-12288
               7   2  13-16   17   7    385-512   27   12 12289-16384
               8   3  17-24   18   8    513-768   28   13 16385-24576
               9   3  25-32   19   8   769-1024   29   13 24577-32768
        */
    }

    public class MSZIPDeflateBlock
    {
        /*
        3.2.3. Details of block format

         Each block of compressed data begins with 3 header bits
         containing the following data:

            first bit       BFINAL
            next 2 bits     BTYPE

         Note that the header bits do not necessarily begin on a byte
         boundary, since a block does not necessarily occupy an integral
         number of bytes.

         BFINAL is set if and only if this is the last block of the data
         set.

         BTYPE specifies how the data are compressed, as follows:

            00 - no compression
            01 - compressed with fixed Huffman codes
            10 - compressed with dynamic Huffman codes
            11 - reserved (error)

         The only difference between the two compressed cases is how the
         Huffman codes for the literal/length and distance alphabets are
         defined.

         In all cases, the decoding algorithm for the actual data is as
         follows:

            do
               read block header from input stream.
               if stored with no compression
                  skip any remaining bits in current partially
                     processed byte
                  read LEN and NLEN (see next section)
                  copy LEN bytes of data to output
               otherwise
                  if compressed with dynamic Huffman codes
                     read representation of code trees (see
                        subsection below)
                  loop (until end of block code recognized)
                     decode literal/length value from input stream
                     if value < 256
                        copy value (literal byte) to output stream
                     otherwise
                        if value = end of block (256)
                           break from loop
                        otherwise (value = 257..285)
                           decode distance from input stream

                           move backwards distance bytes in the output
                           stream, and copy length bytes from this
                           position to the output stream.
                  end loop
            while not last block

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

         We now specify each compression method in turn.
        */
    }

    public class MSZIPNonCompressedBlock
    {
        /*
        3.2.4. Non-compressed blocks (BTYPE=00)

         Any bits of input up to the next byte boundary are ignored.
         The rest of the block consists of the following information:

              0   1   2   3   4...
            +---+---+---+---+================================+
            |  LEN  | NLEN  |... LEN bytes of literal data...|
            +---+---+---+---+================================+

         LEN is the number of data bytes in the block.  NLEN is the
         one's complement of LEN.
        */
    }

    public class MSZIPFixedHuffmanCompressedBlock
    {
        /*
        3.2.6. Compression with fixed Huffman codes (BTYPE = 01)

         The Huffman codes for the two alphabets are fixed, and are not
         represented explicitly in the data.The Huffman code lengths
         for the literal / length alphabet are:

                   Lit Value    Bits        Codes
                   -------- - ---------
                     0 - 143     8          00110000 through
                                            10111111
                   144 - 255     9          110010000 through
                                            111111111
                   256 - 279     7          0000000 through
                                            0010111
                   280 - 287     8          11000000 through
                                            11000111
        */
    }

    public class MSZIPDynamicHuffmanCompressedBlock
    {
        /*
        3.2.7. Compression with dynamic Huffman codes (BTYPE=10)

         The Huffman codes for the two alphabets appear in the block
         immediately after the header bits and before the actual
         compressed data, first the literal/length code and then the
         distance code.  Each code is defined by a sequence of code
         lengths, as discussed in Paragraph 3.2.2, above.  For even
         greater compactness, the code length sequences themselves are
         compressed using a Huffman code.  The alphabet for code lengths
         is as follows:

               0 - 15: Represent code lengths of 0 - 15
                   16: Copy the previous code length 3 - 6 times.
                       The next 2 bits indicate repeat length
                             (0 = 3, ... , 3 = 6)
                          Example:  Codes 8, 16 (+2 bits 11),
                                    16 (+2 bits 10) will expand to
                                    12 code lengths of 8 (1 + 6 + 5)
                   17: Repeat a code length of 0 for 3 - 10 times.
                       (3 bits of length)
                   18: Repeat a code length of 0 for 11 - 138 times
                       (7 bits of length)

         A code length of 0 indicates that the corresponding symbol in
         the literal/length or distance alphabet will not occur in the
         block, and should not participate in the Huffman code
         construction algorithm given earlier.  If only one distance
         code is used, it is encoded using one bit, not zero bits; in
         this case there is a single code length of one, with one unused
         code.  One distance code of zero bits means that there are no
         distance codes used at all (the data is all literals).

         We can now define the format of the block:

               5 Bits: HLIT, # of Literal/Length codes - 257 (257 - 286)
               5 Bits: HDIST, # of Distance codes - 1        (1 - 32)
               4 Bits: HCLEN, # of Code Length codes - 4     (4 - 19)


               (HCLEN + 4) x 3 bits: code lengths for the code length
                  alphabet given just above, in the order: 16, 17, 18,
                  0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15

                  These code lengths are interpreted as 3-bit integers
                  (0-7); as above, a code length of 0 means the
                  corresponding symbol (literal/length or distance code
                  length) is not used.

               HLIT + 257 code lengths for the literal/length alphabet,
                  encoded using the code length Huffman code

               HDIST + 1 code lengths for the distance alphabet,
                  encoded using the code length Huffman code

               The actual compressed data of the block,
                  encoded using the literal/length and distance Huffman
                  codes

               The literal/length symbol 256 (end of data),
                  encoded using the literal/length Huffman code

         The code length repeat codes can cross from HLIT + 257 to the
         HDIST + 1 code lengths.  In other words, all code lengths form
         a single sequence of HLIT + HDIST + 258 values.
        */
    }

    #endregion
}
