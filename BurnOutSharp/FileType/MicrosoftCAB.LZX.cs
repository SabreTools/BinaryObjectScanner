using System.Collections.Generic;
/// <see href="https://learn.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-patch/cc78752a-b4af-4eee-88cb-01f4d8a4c2bf"/>
/// <see href="https://interoperability.blob.core.windows.net/files/MS-PATCH/%5bMS-PATCH%5d.pdf"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzx.h"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzxc.c"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzxd.c"/>
/// <see href="https://wimlib.net/"/>
/// <see href="http://xavprods.free.fr/lzx/"/>
/// <see href="https://github.com/jhermsmeier/node-lzx"/>
/// <see href="https://github.com/jhermsmeier/node-cabarc"/>
namespace BurnOutSharp.FileType
{
    /// <summary>
    /// 3-bit block type
    /// </summary>
    public enum MSCABLXZBlockType : byte
    {
        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_0 = 0b000,

        /// <summary>
        /// Verbatim block
        /// </summary>
        Verbatim = 0b001,

        /// <summary>
        /// Aligned offset block
        /// </summary>
        AlignedOffset = 0b010,

        /// <summary>
        /// Uncompressed block
        /// </summary>
        Uncompressed = 0b011,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_4 = 0b100,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_5 = 0b101,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_6 = 0b110,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_7 = 0b111,
    }

    public class MSCABLZX
    {
        /// <summary>
        /// The window size determines the number of window subdivisions, or position slots
        /// </summary>
        public static readonly Dictionary<int, int> PositionSlots = new Dictionary<int, int>()
        {
            [128 * 1024] = 34, // 128 KB
            [256 * 1024] = 36, // 256 KB
            [512 * 1024] = 38, // 512 KB
            [1024 * 1024] = 42, // 1 MB
            [2 * 1024 * 1024] = 50, // 2 MB
            [4 * 1024 * 1024] = 66, // 4 MB
            [8 * 1024 * 1024] = 98, // 8 MB
            [16 * 1024 * 1024] = 162, // 16 MB
            [32 * 1024 * 1024] = 290, // 32 MB
        };
    }

    public class MSCABLZXHeader
    {
        /*
        2.2 Header

        2.2.1 Chunk Size

        The LZXD compressor emits chunks of compressed data. A chunk represents exactly 32 KB of
        uncompressed data until the last chunk in the stream, which can represent less than 32 KB. To
        ensure that an exact number of input bytes represent an exact number of output bytes for each
        chunk, after each 32 KB of uncompressed data is represented in the output compressed bitstream, the
        output bitstream is padded with up to 15 bits of zeros to realign the bitstream on a 16-bit boundary
        (even byte boundary) for the next 32 KB of data. This results in a compressed chunk of a byte-aligned
        size. The compressed chunk could be smaller than 32 KB or larger than 32 KB if the data is
        incompressible when the chunk is not the last one.

        The LZXD engine encodes a compressed, chunk-size prefix field preceding each compressed chunk in
        the compressed byte stream. The compressed, chunk-size prefix field is a byte aligned, little-endian,
        16-bit field. The chunk prefix chain could be followed in the compressed stream without
        decompressing any data. The next chunk prefix is at a location computed by the absolute byte offset
        location of this chunk prefix plus 2 (for the size of the chunk-size prefix field) plus the current chunk
        size.

        2.2.2 E8 Call Translation

        E8 call translation is an optional feature that can be used when the data to compress contains x86
        instruction sequences. E8 translation operates as a preprocessing stage before compressing each
        chunk, and the compressed stream header contains a bit that indicates whether the decoder shall
        reverse the translation as a postprocessing step after decompressing each chunk.

        The x86 instruction beginning with a byte value of 0xE8 is followed by a 32-bit, little-endian relative
        displacement to the call target. When E8 call translation is enabled, the following preprocessing steps
        are performed on the uncompressed input before compression (assuming little-endian byte ordering):

        Let chunk_offset refer to the total number of uncompressed bytes preceding this chunk.

        Let E8_file_size refer to the caller-specified value given to the compressor or decoded from the header
        of the compressed stream during decompression.

        The following example shows how E8 translation is performed for each 32-KB chunk of uncompressed
        data (or less than 32 KB if last chunk to compress).

            if (( chunk_offset < 0x40000000 ) && ( chunk_size > 10 ))
                for ( i = 0; i < (chunk_size – 10); i++ )
                    if ( chunk_byte[ i ] == 0xE8 )
                        long current_pointer = chunk_offset + i;
                        long displacement = chunk_byte[ i+1 ] |
                        chunk_byte[ i+2 ] << 8 |
                        chunk_byte[ i+3 ] << 16 |
                        chunk_byte[ i+4 ] << 24;
                        long target = current_pointer + displacement;
                        if (( target >= 0 ) && ( target < E8_file_size+current_pointer))
                            if ( target >= E8_file_size )
                                target = displacement – E8_file_size;
                            endif
                            chunk_byte[ i+1 ] = (byte)( target );
                            chunk_byte[ i+2 ] = (byte)( target >> 8 );
                            chunk_byte[ i+3 ] = (byte)( target >> 16 );
                            chunk_byte[ i+4 ] = (byte)( target >> 24 );
                        endif
                        i += 4;
                    endif
                endfor
            endif

        After decompression, the E8 scanning algorithm is the same. The following example shows how E8
        translation reversal is performed.

        long value = chunk_byte[ i+1 ] |
        chunk_byte[ i+2 ] << 8 |
        chunk_byte[ i+3 ] << 16 |
        chunk_byte[ i+4 ] << 24;
        if (( value >= -current_pointer ) && ( value < E8_file_size ))
            if ( value >= 0 )
                displacement = value – current_pointer;
            else
                displacement = value + E8_file_size;
            endif
            chunk_byte[ i+1 ] = (byte)( displacement );
            chunk_byte[ i+2 ] = (byte)( displacement >> 8 );
            chunk_byte[ i+3 ] = (byte)( displacement >> 16 );
            chunk_byte[ i+4 ] = (byte)( displacement >> 24 );
        endif

        The first bit in the first chunk in the LZXD bitstream (following the 2-byte, chunk-size prefix described
        in section 2.2.1) indicates the presence or absence of two 16-bit fields immediately following the
        single bit. If the bit is set, E8 translation is enabled for all the following chunks in the stream using the
        32-bit value derived from the two 16-bit fields as the E8_file_size provided to the compressor when E8
        translation was enabled. Note that E8_file_size is completely independent of the length of the
        uncompressed data. E8 call translation is disabled after the 32,768th chunk (after 1 gigabyte (GB) of
        uncompressed data).

        Field                       Comments                    Size
        ----------------------------------------------------------------
        E8 translation              0-disabled, 1-enabled       1 bit
        Translation size high word  Only present if enabled     0 or 16 bits
        Translation size low word   Only present if enabled     0 or 16 bits
        */
    }

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
    public class MSCABLZXBlockHeader
    {
        /// <remarks>3 bits</remarks>
        public MSCABLXZBlockType BlockType;

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

        /*
        2.3.2 Block Data

        2.3.2.3 Aligned Offset Block

        An aligned offset block is identical to the verbatim block except for the presence of the aligned offset
        tree preceding the other trees.

        Entry                                           Comments                    Size
        ----------------------------------------------------------------------------------
        Aligned offset tree                             8 elements, 3 bits each     24 bits
        Pretree for first 256 elements of main tree     20 elements, 4 bits each    80 bits
        Path lengths of first 256 elements of main tree Encoded using pretree       Variable
        Pretree for remainder of main tree              20 elements, 4 bits each    80 bits
        Path lengths of remaining elements of main tree Encoded using pretree       Variable
        Pretree for length tree                         20 elements, 4 bits each    80 bits
        Path lengths of elements in length tree         Encoded using pretree       Variable
        Token sequence (matches and literals)           Specified in section 2.6    Variable
        */
    }

    /// <summary>
    /// Following the generic block header, an uncompressed block begins with 1 to 16 bits of zero padding
    /// to align the bit buffer on a 16-bit boundary. At this point, the bitstream ends and a byte stream
    /// begins. Following the zero padding, new 32-bit values for R0, R1, and R2 are output in little-endian
    /// form, followed by the uncompressed data bytes themselves. Finally, if the uncompressed data length
    /// is odd, one extra byte of zero padding is encoded to realign the following bitstream.
    /// 
    /// Then the bitstream of byte-swapped 16-bit integers resumes for the next Block Type field (if there
    /// are subsequent blocks).
    /// 
    /// The decoded R0, R1, and R2 values are used as initial repeated offset values to decode the
    /// subsequent compressed block if present.
    /// </summary>
    public class MSCABLZXUncompressedBlock
    {
        /// <summary>
        /// Generic block header
        /// </summary>
        public MSCABLZXBlockHeader Header;

        /// <summary>
        /// Padding to align following field on 16-bit boundary
        /// </summary>
        /// <remarks>Bits have a value of zero</remarks>
        public ushort PaddingBits;

        /// <summary>
        /// Least significant to most significant byte (little-endian DWORD ([MS-DTYP]))
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public uint R0;

        /// <summary>
        /// Least significant to most significant byte (little-endian DWORD)
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public uint R1;

        /// <summary>
        /// Least significant to most significant byte (little-endian DWORD)
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public uint R2;

        /// <summary>
        /// Can use the direct memcpy function, as specified in [IEEE1003.1]
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public byte[] RawDataBytes;

        /// <summary>
        /// Only if uncompressed size is odd
        /// </summary>
        public byte AlignmentByte;
    }

    /// <summary>
    /// The fields of a verbatim block that follow the generic block header
    /// </summary>
    public class MSCABLZXVerbatimBlock
    {
        /// <summary>
        /// Generic block header
        /// </summary>
        public MSCABLZXBlockHeader Header;

        /// <summary>
        /// Pretree for first 256 elements of main tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeFirst256;

        /// <summary>
        /// Path lengths of first 256 elements of main tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsFirst256;

        /// <summary>
        /// Pretree for remainder of main tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeRemainder;

        /// <summary>
        /// Path lengths of remaining elements of main tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsRemainder;

        /// <summary>
        /// Pretree for length tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeLengthTree;

        /// <summary>
        /// Path lengths of elements in length tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsLengthTree;

        // Entry                                    Comments                    Size
        // ---------------------------------------------------------------------------------------
        // Token sequence (matches and literals)    Specified in section 2.6    Variable
    }

    /// <summary>
    /// An aligned offset block is identical to the verbatim block except for the presence of the aligned offset
    /// tree preceding the other trees.
    /// </summary>
    public class MSCABLZXAlignedOffsetBlock
    {
        /// <summary>
        /// Generic block header
        /// </summary>
        public MSCABLZXBlockHeader Header;

        /// <summary>
        /// Aligned offset tree
        /// </summary>
        /// <remarks>8 elements, 3 bits each</remarks>
        public byte[] AlignedOffsetTree;

        /// <summary>
        /// Pretree for first 256 elements of main tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeFirst256;

        /// <summary>
        /// Path lengths of first 256 elements of main tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsFirst256;

        /// <summary>
        /// Pretree for remainder of main tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeRemainder;

        /// <summary>
        /// Path lengths of remaining elements of main tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsRemainder;

        /// <summary>
        /// Pretree for length tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeLengthTree;

        /// <summary>
        /// Path lengths of elements in length tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsLengthTree;

        // Entry                                    Comments                    Size
        // ---------------------------------------------------------------------------------------
        // Token sequence (matches and literals)    Specified in section 2.6    Variable
    }
}
