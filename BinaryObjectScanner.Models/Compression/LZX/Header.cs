namespace BinaryObjectScanner.Models.Compression.LZX
{
    public class Header
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
}