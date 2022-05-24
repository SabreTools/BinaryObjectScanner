/* This file is part of libmspack.
 * (C) 2003-2013 Stuart Caie.
 *
 * The LZX method was created by Jonathan Forbes and Tomi Poutanen, adapted
 * by Microsoft Corporation.
 *
 * libmspack is free software { get; set; } you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;
using static LibMSPackSharp.Compression.Constants;

namespace LibMSPackSharp.Compression
{
    public class LZXDStream : CompressionStream
    {
        #region Fields

        /// <summary>
        /// Number of bytes actually output
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Overall decompressed length of stream
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Decoding window
        /// </summary>
        public byte[] Window { get; set; }

        /// <summary>
        /// Window size
        /// </summary>
        public uint WindowSize { get; set; }

        /// <summary>
        /// LZX DELTA reference data size
        /// </summary>
        public uint ReferenceDataSize { get; set; }

        /// <summary>
        /// Number of match_offset entries in table
        /// </summary>
        public uint NumOffsets { get; set; }

        /// <summary>
        /// Decompression offset within window
        /// </summary>
        public int WindowPosition { get; set; }

        /// <summary>
        /// Current frame offset within in window
        /// </summary>
        public uint FramePosition { get; set; }

        /// <summary>
        /// The number of 32kb frames processed
        /// </summary>
        public uint Frame { get; set; }

        /// <summary>
        /// Which frame do we reset the compressor?
        /// </summary>
        public uint ResetInterval { get; set; }

        /// <summary>
        /// For the LRU offset system
        /// </summary>
        public uint[] R { get; set; } = new uint[3];

        /// <summary>
        /// Uncompressed length of this LZX block
        /// </summary>
        public int BlockLength { get; set; }

        /// <summary>
        /// Uncompressed bytes still left to decode
        /// </summary>
        public int BlockRemaining { get; set; }

        /// <summary>
        /// Magic header value used for transform
        /// </summary>
        public int IntelFileSize { get; set; }

        /// <summary>
        /// Has intel E8 decoding started?
        /// </summary>
        public bool IntelStarted { get; set; }

        /// <summary>
        /// Type of the current block
        /// </summary>
        public LZXBlockType BlockType { get; set; }

        /// <summary>
        /// Have we started decoding at all yet?
        /// </summary>
        public byte HeaderRead { get; set; }

        /// <summary>
        /// Does stream follow LZX DELTA spec?
        /// </summary>
        public bool IsDelta { get; set; }

        #region Huffman code lengths

        public byte[] PRETREE_len { get; set; } = new byte[LZX_PRETREE_MAXSYMBOLS + LZX_LENTABLE_SAFETY];
        public byte[] MAINTREE_len { get; set; } = new byte[LZX_MAINTREE_MAXSYMBOLS + LZX_LENTABLE_SAFETY];
        public byte[] LENGTH_len { get; set; } = new byte[LZX_LENGTH_MAXSYMBOLS + LZX_LENTABLE_SAFETY];
        public byte[] ALIGNED_len { get; set; } = new byte[LZX_ALIGNED_MAXSYMBOLS + LZX_LENTABLE_SAFETY];

        #endregion

        #region Huffman decoding tables

        public ushort[] PRETREE_table { get; set; } = new ushort[(1 << LZX_PRETREE_TABLEBITS) + (LZX_PRETREE_MAXSYMBOLS * 2)];
        public ushort[] MAINTREE_table { get; set; } = new ushort[(1 << LZX_MAINTREE_TABLEBITS) + (LZX_MAINTREE_MAXSYMBOLS * 2)];
        public ushort[] LENGTH_table { get; set; } = new ushort[(1 << LZX_LENGTH_TABLEBITS) + (LZX_LENGTH_MAXSYMBOLS * 2)];
        public ushort[] ALIGNED_table { get; set; } = new ushort[(1 << LZX_ALIGNED_TABLEBITS) + (LZX_ALIGNED_MAXSYMBOLS * 2)];

        #endregion
        
        public byte LENGTH_empty { get; set; }

        // This is used purely for doing the intel E8 transform
        public byte[] E8Buffer { get; set; } = new byte[LZX_FRAME_SIZE];

        /// <summary>
        /// Is the output pointer referring to E8?
        /// </summary>
        public bool OutputIsE8 { get; set; }

        #endregion

        #region Specialty Methods

        public Error DecompressBlock(byte[] window, ref int window_posn, ref int this_run, ref uint[] R, BufferState state)
        {
            while (this_run > 0)
            {
                int main_element = (int)READ_HUFFSYM_MSB(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS, state);
                if (main_element < LZX_NUM_CHARS)
                {
                    // Literal: 0 to LZX_NUM_CHARS-1
                    window[window_posn++] = (byte)main_element;
                    this_run--;
                }
                else
                {
                    // Match: LZX_NUM_CHARS + ((slot<<3) | length_header (3 bits))
                    main_element -= LZX_NUM_CHARS;

                    // Get match length
                    int match_length = main_element & LZX_NUM_PRIMARY_LENGTHS;
                    if (match_length == LZX_NUM_PRIMARY_LENGTHS)
                    {
                        if (LENGTH_empty != 0)
                        {
                            Console.WriteLine("LENGTH symbol needed but tree is empty");
                            return Error = Error.MSPACK_ERR_DECRUNCH;
                        }

                        int length_footer = (int)READ_HUFFSYM_MSB(LENGTH_table, LENGTH_len, LZX_LENGTH_TABLEBITS, LZX_LENGTH_MAXSYMBOLS, state);
                        match_length += length_footer;
                    }

                    match_length += LZX_MIN_MATCH;

                    // Get match offset
                    uint match_offset = (uint)(main_element >> 3);
                    switch (match_offset)
                    {
                        case 0:
                            match_offset = R[0];
                            break;

                        case 1:
                            match_offset = R[1];
                            R[1] = R[0];
                            R[0] = match_offset;
                            break;

                        case 2:
                            match_offset = R[2];
                            R[2] = R[0];
                            R[0] = match_offset;
                            break;

                        default:
                            if (BlockType == LZXBlockType.LZX_BLOCKTYPE_VERBATIM)
                            {
                                if (match_offset == 3)
                                {
                                    match_offset = 1;
                                }
                                else
                                {
                                    int extra = (match_offset >= 36) ? 17 : LZXExtraBits[match_offset];
                                    int verbatim_bits = (int)READ_BITS_MSB(extra, state);
                                    match_offset = (uint)(LZXPositionBase[match_offset] - 2 + verbatim_bits);
                                }
                            }

                            // LZX_BLOCKTYPE_ALIGNED
                            else
                            {
                                int extra = (match_offset >= 36) ? 17 : LZXExtraBits[match_offset];
                                match_offset = LZXPositionBase[match_offset] - 2;

                                // >3: verbatim and aligned bits
                                if (extra > 3)
                                {
                                    extra -= 3;
                                    int verbatim_bits = (int)READ_BITS_MSB(extra, state);
                                    match_offset += (uint)(verbatim_bits << 3);

                                    int aligned_bits = (int)READ_HUFFSYM_MSB(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS, state);
                                    match_offset += (uint)aligned_bits;
                                }

                                // 3: aligned bits only
                                else if (extra == 3)
                                {
                                    int aligned_bits = (int)READ_HUFFSYM_MSB(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS, state);
                                    match_offset += (uint)aligned_bits;
                                }

                                // 1-2: verbatim bits only
                                else if (extra > 0)
                                {
                                    int verbatim_bits = (int)READ_BITS_MSB(extra, state);
                                    match_offset += (uint)verbatim_bits;
                                }

                                // 0: not defined in LZX specification!
                                else
                                {
                                    match_offset = 1;
                                }
                            }

                            // Update repeated offset LRU queue
                            R[2] = R[1]; R[1] = R[0]; R[0] = match_offset;
                            break;
                    }

                    // LZX DELTA uses max match length to signal even longer match
                    if (match_length == LZX_MAX_MATCH && IsDelta)
                    {
                        int extra_len;

                        // 4 entry huffman tree
                        ENSURE_BITS(3, state);

                        // '0' . 8 extra length bits
                        if (PEEK_BITS_MSB(1, state.BitBuffer) == 0)
                        {
                            state.REMOVE_BITS_MSB(1);
                            extra_len = (int)READ_BITS_MSB(8, state);
                        }

                        // '10' . 10 extra length bits + 0x100
                        else if (PEEK_BITS_MSB(2, state.BitBuffer) == 2)
                        {
                            state.REMOVE_BITS_MSB(2);
                            extra_len = (int)READ_BITS_MSB(10, state);
                            extra_len += 0x100;
                        }

                        // '110' . 12 extra length bits + 0x500
                        else if (PEEK_BITS_MSB(3, state.BitBuffer) == 6)
                        {
                            state.REMOVE_BITS_MSB(3);
                            extra_len = (int)READ_BITS_MSB(12, state);
                            extra_len += 0x500;
                        }

                        // '111' . 15 extra length bits
                        else
                        {
                            state.REMOVE_BITS_MSB(3);
                            extra_len = (int)READ_BITS_MSB(15, state);
                        }

                        match_length += extra_len;
                    }

                    if ((window_posn + match_length) > WindowSize)
                    {
                        Console.WriteLine("Match ran over window wrap");
                        return Error = Error.MSPACK_ERR_DECRUNCH;
                    }

                    // Copy match
                    int rundest = window_posn;
                    int i = match_length;

                    // Does match offset wrap the window?
                    if (match_offset > window_posn)
                    {
                        if (match_offset > Offset && (match_offset - window_posn) > ReferenceDataSize)
                        {
                            Console.WriteLine("Match offset beyond LZX stream");
                            return Error = Error.MSPACK_ERR_DECRUNCH;
                        }

                        // j = length from match offset to end of window
                        int j = (int)(match_offset - window_posn);
                        if (j > (int)WindowSize)
                        {
                            Console.WriteLine("Match offset beyond window boundaries");
                            return Error = Error.MSPACK_ERR_DECRUNCH;
                        }

                        int runsrc = (int)(WindowSize - j);
                        if (j < i)
                        {
                            // If match goes over the window edge, do two copy runs
                            i -= j;
                            while (j-- > 0)
                            {
                                window[rundest++] = window[runsrc++];
                            }

                            runsrc = 0;
                        }

                        while (i-- > 0)
                        {
                            window[rundest++] = window[runsrc++];
                        }
                    }
                    else
                    {
                        int runsrc = (int)(rundest - match_offset);
                        while (i-- > 0)
                        {
                            window[rundest++] = window[runsrc++];
                        }
                    }

                    this_run -= match_length;
                    window_posn += match_length;
                }
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        public Error ReadBlockHeader(byte[] buffer, ref uint[] R, BufferState state)
        {
            ENSURE_BITS(4, state);

            // Read block type (3 bits) and block length (24 bits)
            byte block_type = (byte)READ_BITS_MSB(3, state);
            BlockType = (LZXBlockType)block_type;

            // Read the block size
            int block_size;
            if (READ_BITS_MSB(1, state) == 1)
            {
                block_size = LZX_FRAME_SIZE;
            }
            else
            {
                int tmp;
                block_size = 0;

                tmp = (int)READ_BITS_MSB(8, state);
                block_size |= tmp;
                tmp = (int)READ_BITS_MSB(8, state);
                block_size <<= 8;
                block_size |= tmp;

                if (WindowSize >= 65536)
                {
                    tmp = (int)READ_BITS_MSB(8, state);
                    block_size <<= 8;
                    block_size |= tmp;
                }
            }

            BlockRemaining = BlockLength = block_size;
            Console.WriteLine($"New block t {BlockType} len {BlockLength}");

            // Read individual block headers
            switch (BlockType)
            {
                case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                    // Read lengths of and build aligned huffman decoding tree
                    for (byte i = 0; i < 8; i++)
                    {
                        ALIGNED_len[i] = (byte)READ_BITS_MSB(3, state);
                    }

                    BUILD_TABLE(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    // Read lengths of and build main huffman decoding tree
                    READ_LENGTHS(MAINTREE_len, 0, 256, state);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    READ_LENGTHS(MAINTREE_len, 256, LZX_NUM_CHARS + NumOffsets, state);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    // Read lengths of and build lengths huffman decoding tree
                    READ_LENGTHS(LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS, state);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE_MAYBE_EMPTY();
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    break;

                case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                    // Read lengths of and build main huffman decoding tree
                    READ_LENGTHS(MAINTREE_len, 0, 256, state);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    READ_LENGTHS(MAINTREE_len, 256, LZX_NUM_CHARS + NumOffsets, state);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    // If the literal 0xE8 is anywhere in the block...
                    if (MAINTREE_len[0xE8] != 0)
                        IntelStarted = true;

                    // Read lengths of and build lengths huffman decoding tree
                    READ_LENGTHS(LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS, state);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE_MAYBE_EMPTY();
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    break;

                case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                    // Read 1-16 (not 0-15) bits to align to bytes
                    if (state.BitsLeft == 0)
                        ENSURE_BITS(16, state);

                    state.BitsLeft = 0; state.BitBuffer = 0;

                    // Read 12 bytes of stored R[0] / R[1] / R[2] values
                    for (int rundest = 0, k = 0; k < 12; k++)
                    {
                        READ_IF_NEEDED(state);
                        if (Error != Error.MSPACK_ERR_OK)
                            return Error;

                        buffer[rundest++] = InputBuffer[state.InputPointer++];
                    }

                    // TODO: uint[] R should be a part of a state object
                    R[0] = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
                    R[1] = (uint)(buffer[4] | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24));
                    R[2] = (uint)(buffer[8] | (buffer[9] << 8) | (buffer[10] << 16) | (buffer[11] << 24));

                    break;

                default:
                    Console.WriteLine($"Bad block type: {BlockType}");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        public Error ReadLens(byte[] lens, uint first, uint last)
        {
            BufferState state = RESTORE_BITS();

            // Read lengths for pretree (20 symbols, lengths stored in fixed 4 bits) 
            for (int i = 0; i < LZX_PRETREE_MAXSYMBOLS; i++)
            {
                uint y = (uint)READ_BITS_MSB(4, state);
                PRETREE_len[i] = (byte)y;
            }

            BUILD_TABLE(PRETREE_table, PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS);
            if (Error != Error.MSPACK_ERR_OK)
                return Error;

            for (uint lensPtr = first; lensPtr < last;)
            {
                uint num_zeroes, num_same;
                int tree_code = (int)READ_HUFFSYM_MSB(PRETREE_table, PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS, state);
                switch (tree_code)
                {
                    // Code = 17, run of ([read 4 bits]+4) zeros
                    case 17:
                        num_zeroes = (uint)READ_BITS_MSB(4, state);
                        num_zeroes += 4;
                        while (num_zeroes-- != 0)
                        {
                            lens[lensPtr++] = 0;
                        }

                        break;

                    // Code = 18, run of ([read 5 bits]+20) zeros
                    case 18:
                        num_zeroes = (uint)READ_BITS_MSB(5, state);
                        num_zeroes += 20;
                        while (num_zeroes-- != 0)
                        {
                            lens[lensPtr++] = 0;
                        }

                        break;

                    // Code = 19, run of ([read 1 bit]+4) [read huffman symbol]
                    case 19:
                        num_same = (uint)READ_BITS_MSB(1, state);
                        num_same += 4;

                        tree_code = (int)READ_HUFFSYM_MSB(PRETREE_table, PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS, state);
                        tree_code = lens[lensPtr] - tree_code;
                        if (tree_code < 0)
                            tree_code += 17;

                        while (num_same-- != 0)
                        {
                            lens[lensPtr++] = (byte)tree_code;
                        }

                        break;

                    // Code = 0 to 16, delta current length entry
                    default:
                        tree_code = lens[lensPtr] - tree_code;
                        if (tree_code < 0)
                            tree_code += 17;

                        lens[lensPtr++] = (byte)tree_code;
                        break;
                }
            }

            STORE_BITS(state);

            return Error.MSPACK_ERR_OK;
        }

        public void ResetState()
        {
            R[0] = 1;
            R[1] = 1;
            R[2] = 1;
            HeaderRead = 0;
            BlockRemaining = 0;
            BlockType = LZXBlockType.LZX_BLOCKTYPE_INVALID0;

            // Initialise tables to 0 (because deltas will be applied to them)
            for (int i = 0; i < LZX_MAINTREE_MAXSYMBOLS; i++)
            {
                MAINTREE_len[i] = 0;
            }

            for (int i = 0; i < LZX_LENGTH_MAXSYMBOLS; i++)
            {
                LENGTH_len[i] = 0;
            }
        }

        public void UndoE8Preprocessing(uint frame_size)
        {
            int data = 0;
            int dataend = (int)(frame_size - 10);
            int curpos = (int)Offset;
            int filesize = IntelFileSize;
            int abs_off, rel_off;

            // Copy e8 block to the e8 buffer and tweak if needed
            OutputIsE8 = true;
            OutputPointer = data;
            Array.Copy(Window, FramePosition, E8Buffer, data, frame_size);

            while (data < dataend)
            {
                if (E8Buffer[data++] != 0xE8)
                {
                    curpos++;
                    continue;
                }

                abs_off = E8Buffer[data + 0] | (E8Buffer[data + 1] << 8) | (E8Buffer[data + 2] << 16) | (E8Buffer[data + 3] << 24);
                if ((abs_off >= -curpos) && (abs_off < filesize))
                {
                    rel_off = (abs_off >= 0) ? abs_off - curpos : abs_off + filesize;
                    E8Buffer[data + 0] = (byte)rel_off;
                    E8Buffer[data + 1] = (byte)(rel_off >> 8);
                    E8Buffer[data + 2] = (byte)(rel_off >> 16);
                    E8Buffer[data + 3] = (byte)(rel_off >> 24);
                }

                data += 4;
                curpos += 5;
            }
        }

        private Error BUILD_TABLE(ushort[] table, byte[] lengths, int tablebits, int maxsymbols)
        {
            if (!MakeDecodeTableMSB(maxsymbols, tablebits, lengths, table))
            {
                Console.WriteLine($"Failed to build table");
                return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        private Error BUILD_TABLE_MAYBE_EMPTY()
        {
            LENGTH_empty = 0;
            if (!MakeDecodeTableMSB(LZX_LENGTH_MAXSYMBOLS, LZX_LENGTH_TABLEBITS, LENGTH_len, LENGTH_table))
            {
                for (int i = 0; i < LZX_LENGTH_MAXSYMBOLS; i++)
                {
                    if (LENGTH_len[i] > 0)
                    {
                        Console.WriteLine("Failed to build table");
                        return Error = Error.MSPACK_ERR_DECRUNCH;
                    }
                }

                // Empty tree - allow it, but don't decode symbols with it
                LENGTH_empty = 1;
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        private Error READ_LENGTHS(byte[] lengths, uint first, uint last, BufferState state)
        {
            STORE_BITS(state);

            if (ReadLens(lengths, first, last) != Error.MSPACK_ERR_OK)
                return Error;

            BufferState temp = RESTORE_BITS();
            state.InputPointer = temp.InputPointer;
            state.InputEnd = temp.InputEnd;
            state.BitBuffer = temp.BitBuffer;
            state.BitsLeft = temp.BitsLeft;

            return Error = Error.MSPACK_ERR_OK;
        }

        #endregion

        /// <inheritdoc/>
        public override Error HUFF_ERROR() => Error.MSPACK_ERR_DECRUNCH;

        /// <inheritdoc/>
        public override void READ_BYTES(BufferState state)
        {
            READ_IF_NEEDED(state);
            if (Error != Error.MSPACK_ERR_OK)
                return;

            byte b0 = InputBuffer[state.InputPointer++];

            READ_IF_NEEDED(state);
            if (Error != Error.MSPACK_ERR_OK)
                return;

            byte b1 = InputBuffer[state.InputPointer++];
            INJECT_BITS_MSB((b1 << 8) | b0, 16, state);
        }
    }
}
