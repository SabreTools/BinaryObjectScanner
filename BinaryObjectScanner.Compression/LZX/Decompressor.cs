using System;
using BinaryObjectScanner.Compression.LZX;
using static BinaryObjectScanner.Models.Compression.LZX.Constants;
using static BinaryObjectScanner.Models.MicrosoftCabinet.Constants;

namespace BinaryObjectScanner.Compression.LZX
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
    public class Decompressor
    {
        /// <summary>
        /// Initialize an LZX decompressor state
        /// </summary>
        public static bool Init(int window, State state)
        {
            uint wndsize = (uint)(1 << window);
            int posn_slots;

            /* LZX supports window sizes of 2^15 (32Kb) through 2^21 (2Mb) */
            /* if a previously allocated window is big enough, keep it     */
            if (window < 15 || window > 21)
                return false;

            if (state.actual_size < wndsize)
                state.window = null;

            if (state.window == null)
            {
                state.window = new byte[wndsize];
                state.actual_size = wndsize;
            }

            state.window_size = wndsize;

            /* calculate required position slots */
            if (window == 20) posn_slots = 42;
            else if (window == 21) posn_slots = 50;
            else posn_slots = window << 1;

            /*posn_slots=i=0; while (i < wndsize) i += 1 << CAB(extra_bits)[posn_slots++]; */

            state.R0 = state.R1 = state.R2 = 1;
            state.main_elements = (ushort)(LZX_NUM_CHARS + (posn_slots << 3));
            state.header_read = 0;
            state.frames_read = 0;
            state.block_remaining = 0;
            state.block_type = LZX_BLOCKTYPE_INVALID;
            state.intel_curpos = 0;
            state.intel_started = 0;
            state.window_posn = 0;

            /* initialize tables to 0 (because deltas will be applied to them) */
            // memset(state.MAINTREE_len, 0, sizeof(state.MAINTREE_len));
            // memset(state.LENGTH_len, 0, sizeof(state.LENGTH_len));

            return true;
        }

        /// <summary>
        /// Decompress a byte array using a given State
        /// </summary>
        public static bool Decompress(State state, int inlen, byte[] inbuf, int outlen, byte[] outbuf)
        {
            int inpos = 0; // inbuf[0];
            int endinp = inpos + inlen;
            int window = 0; // state.window[0];
            int runsrc, rundest; // byte*

            uint window_posn = state.window_posn;
            uint window_size = state.window_size;
            uint R0 = state.R0;
            uint R1 = state.R1;
            uint R2 = state.R2;

            uint match_offset, i, j, k; /* ijk used in READ_HUFFSYM macro */
            Bits lb = new Bits(); /* used in READ_LENGTHS macro */

            int togo = outlen, this_run, main_element, aligned_bits;
            int match_length, copy_length, length_footer, extra, verbatim_bits;

            INIT_BITSTREAM(out int bitsleft, out uint bitbuf);

            /* read header if necessary */
            if (state.header_read == 0)
            {
                i = j = 0;
                k = READ_BITS(1, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                if (k != 0)
                {
                    i = READ_BITS(16, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    j = READ_BITS(16, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                }

                state.intel_filesize = (int)((i << 16) | j); /* or 0 if not encoded */
                state.header_read = 1;
            }

            /* main decoding loop */
            while (togo > 0)
            {
                /* last block finished, new block expected */
                if (state.block_remaining == 0)
                {
                    if (state.block_type == LZX_BLOCKTYPE_UNCOMPRESSED)
                    {
                        if ((state.block_length & 1) != 0)
                            inpos++; /* realign bitstream to word */

                        INIT_BITSTREAM(out bitsleft, out bitbuf);
                    }

                    state.block_type = (ushort)READ_BITS(3, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    i = READ_BITS(16, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    j = READ_BITS(8, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    state.block_remaining = state.block_length = (i << 8) | j;

                    switch (state.block_type)
                    {
                        case LZX_BLOCKTYPE_ALIGNED:
                            for (i = 0; i < 8; i++)
                            {
                                j = READ_BITS(3, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                state.tblALIGNED_len[i] = (byte)j;
                            }

                            make_decode_table(LZX_ALIGNED_MAXSYMBOLS, LZX_ALIGNED_TABLEBITS, state.tblALIGNED_len, state.tblALIGNED_table);

                            /* rest of aligned header is same as verbatim */
                            goto case LZX_BLOCKTYPE_VERBATIM;

                        case LZX_BLOCKTYPE_VERBATIM:
                            READ_LENGTHS(state.tblMAINTREE_len, 0, 256, lb, state, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                            READ_LENGTHS(state.tblMAINTREE_len, 256, state.main_elements, lb, state, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                            make_decode_table(LZX_MAINTREE_MAXSYMBOLS, LZX_MAINTREE_TABLEBITS, state.tblMAINTREE_len, state.tblMAINTREE_table);
                            if (state.tblMAINTREE_len[0xE8] != 0)
                                state.intel_started = 1;

                            READ_LENGTHS(state.tblLENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS, lb, state, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                            make_decode_table(LZX_LENGTH_MAXSYMBOLS, LZX_LENGTH_TABLEBITS, state.tblLENGTH_len, state.tblLENGTH_table);
                            break;

                        case LZX_BLOCKTYPE_UNCOMPRESSED:
                            state.intel_started = 1; /* because we can't assume otherwise */
                            ENSURE_BITS(16, inbuf, ref inpos, ref bitsleft, ref bitbuf); /* get up to 16 pad bits into the buffer */

                            /* and align the bitstream! */
                            if (bitsleft > 16)
                                inpos -= 2;

                            R0 = (uint)(inbuf[inpos + 0] | (inbuf[inpos + 1] << 8) | (inbuf[inpos + 2] << 16) | (inbuf[inpos + 3] << 24)); inpos += 4;
                            R1 = (uint)(inbuf[inpos + 0] | (inbuf[inpos + 1] << 8) | (inbuf[inpos + 2] << 16) | (inbuf[inpos + 3] << 24)); inpos += 4;
                            R2 = (uint)(inbuf[inpos + 0] | (inbuf[inpos + 1] << 8) | (inbuf[inpos + 2] << 16) | (inbuf[inpos + 3] << 24)); inpos += 4;
                            break;

                        default:
                            return false;
                    }
                }

                /* buffer exhaustion check */
                if (inpos > endinp)
                {
                    /* it's possible to have a file where the next run is less than
                     * 16 bits in size. In this case, the READ_HUFFSYM() macro used
                     * in building the tables will exhaust the buffer, so we should
                     * allow for this, but not allow those accidentally read bits to
                     * be used (so we check that there are at least 16 bits
                     * remaining - in this boundary case they aren't really part of
                     * the compressed data)
                     */
                    if (inpos > (endinp + 2) || bitsleft < 16)
                        return false;
                }

                while ((this_run = (int)state.block_remaining) > 0 && togo > 0)
                {
                    if (this_run > togo) this_run = togo;
                    togo -= this_run;
                    state.block_remaining -= (uint)this_run;

                    /* apply 2^x-1 mask */
                    window_posn &= window_size - 1;

                    /* runs can't straddle the window wraparound */
                    if ((window_posn + this_run) > window_size)
                        return false;

                    switch (state.block_type)
                    {

                        case LZX_BLOCKTYPE_VERBATIM:
                            while (this_run > 0)
                            {
                                main_element = READ_HUFFSYM(state.tblMAINTREE_table, state.tblMAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                if (main_element < LZX_NUM_CHARS)
                                {
                                    /* literal: 0 to LZX_NUM_CHARS-1 */
                                    state.window[window + window_posn++] = (byte)main_element;
                                    this_run--;
                                }
                                else
                                {
                                    /* match: LZX_NUM_CHARS + ((slot<<3) | length_header (3 bits)) */
                                    main_element -= LZX_NUM_CHARS;

                                    match_length = main_element & LZX_NUM_PRIMARY_LENGTHS;
                                    if (match_length == LZX_NUM_PRIMARY_LENGTHS)
                                    {
                                        length_footer = READ_HUFFSYM(state.tblLENGTH_table, state.tblLENGTH_len, LZX_LENGTH_TABLEBITS, LZX_LENGTH_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                        match_length += length_footer;
                                    }

                                    match_length += LZX_MIN_MATCH;
                                    match_offset = (uint)(main_element >> 3);

                                    if (match_offset > 2)
                                    {
                                        /* not repeated offset */
                                        if (match_offset != 3)
                                        {
                                            extra = state.ExtraBits[match_offset];
                                            verbatim_bits = (int)READ_BITS(extra, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                            match_offset = (uint)(state.PositionSlotBases[match_offset] - 2 + verbatim_bits);
                                        }
                                        else
                                        {
                                            match_offset = 1;
                                        }

                                        /* update repeated offset LRU queue */
                                        R2 = R1; R1 = R0; R0 = match_offset;
                                    }
                                    else if (match_offset == 0)
                                    {
                                        match_offset = R0;
                                    }
                                    else if (match_offset == 1)
                                    {
                                        match_offset = R1;
                                        R1 = R0; R0 = match_offset;
                                    }
                                    else /* match_offset == 2 */
                                    {
                                        match_offset = R2;
                                        R2 = R0; R0 = match_offset;
                                    }

                                    rundest = (int)(window + window_posn);
                                    this_run -= match_length;

                                    /* copy any wrapped around source data */
                                    if (window_posn >= match_offset)
                                    {
                                        /* no wrap */
                                        runsrc = (int)(rundest - match_offset);
                                    }
                                    else
                                    {
                                        runsrc = (int)(rundest + (window_size - match_offset));
                                        copy_length = (int)(match_offset - window_posn);
                                        if (copy_length < match_length)
                                        {
                                            match_length -= copy_length;
                                            window_posn += (uint)copy_length;
                                            while (copy_length-- > 0)
                                            {
                                                state.window[rundest++] = state.window[runsrc++];
                                            }

                                            runsrc = window;
                                        }
                                    }

                                    window_posn += (uint)match_length;

                                    /* copy match data - no worries about destination wraps */
                                    while (match_length-- > 0)
                                    {
                                        state.window[rundest++] = state.window[runsrc++];
                                    }
                                }
                            }
                            break;

                        case LZX_BLOCKTYPE_ALIGNED:
                            while (this_run > 0)
                            {
                                main_element = READ_HUFFSYM(state.tblMAINTREE_table, state.tblMAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);

                                if (main_element < LZX_NUM_CHARS)
                                {
                                    /* literal: 0 to LZX_NUM_CHARS-1 */
                                    state.window[window + window_posn++] = (byte)main_element;
                                    this_run--;
                                }
                                else
                                {
                                    /* mverbatim_bitsatch: LZX_NUM_CHARS + ((slot<<3) | length_header (3 bits)) */
                                    main_element -= LZX_NUM_CHARS;

                                    match_length = main_element & LZX_NUM_PRIMARY_LENGTHS;
                                    if (match_length == LZX_NUM_PRIMARY_LENGTHS)
                                    {
                                        length_footer = READ_HUFFSYM(state.tblLENGTH_table, state.tblLENGTH_len, LZX_LENGTH_TABLEBITS, LZX_LENGTH_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                        match_length += length_footer;
                                    }
                                    match_length += LZX_MIN_MATCH;

                                    match_offset = (uint)(main_element >> 3);

                                    if (match_offset > 2)
                                    {
                                        /* not repeated offset */
                                        extra = state.ExtraBits[match_offset];
                                        match_offset = state.PositionSlotBases[match_offset] - 2;
                                        if (extra > 3)
                                        {
                                            /* verbatim and aligned bits */
                                            extra -= 3;
                                            verbatim_bits = (int)READ_BITS(extra, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                            match_offset += (uint)(verbatim_bits << 3);
                                            aligned_bits = READ_HUFFSYM(state.tblALIGNED_table, state.tblALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                            match_offset += (uint)aligned_bits;
                                        }
                                        else if (extra == 3)
                                        {
                                            /* aligned bits only */
                                            aligned_bits = READ_HUFFSYM(state.tblALIGNED_table, state.tblALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                            match_offset += (uint)aligned_bits;
                                        }
                                        else if (extra > 0)
                                        {
                                            /* extra==1, extra==2 */
                                            /* verbatim bits only */
                                            verbatim_bits = (int)READ_BITS(extra, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                                            match_offset += (uint)verbatim_bits;
                                        }
                                        else /* extra == 0 */
                                        {
                                            /* ??? */
                                            match_offset = 1;
                                        }

                                        /* update repeated offset LRU queue */
                                        R2 = R1; R1 = R0; R0 = match_offset;
                                    }
                                    else if (match_offset == 0)
                                    {
                                        match_offset = R0;
                                    }
                                    else if (match_offset == 1)
                                    {
                                        match_offset = R1;
                                        R1 = R0; R0 = match_offset;
                                    }
                                    else /* match_offset == 2 */
                                    {
                                        match_offset = R2;
                                        R2 = R0; R0 = match_offset;
                                    }

                                    rundest = (int)(window + window_posn);
                                    this_run -= match_length;

                                    /* copy any wrapped around source data */
                                    if (window_posn >= match_offset)
                                    {
                                        /* no wrap */
                                        runsrc = (int)(rundest - match_offset);
                                    }
                                    else
                                    {
                                        runsrc = (int)(rundest + (window_size - match_offset));
                                        copy_length = (int)(match_offset - window_posn);
                                        if (copy_length < match_length)
                                        {
                                            match_length -= copy_length;
                                            window_posn += (uint)copy_length;
                                            while (copy_length-- > 0)
                                            {
                                                state.window[rundest++] = state.window[runsrc++];
                                            }

                                            runsrc = window;
                                        }
                                    }

                                    window_posn += (uint)match_length;

                                    /* copy match data - no worries about destination wraps */
                                    while (match_length-- > 0)
                                    {
                                        state.window[rundest++] = state.window[runsrc++];
                                    }
                                }
                            }
                            break;

                        case LZX_BLOCKTYPE_UNCOMPRESSED:
                            if ((inpos + this_run) > endinp)
                                return false;
                            
                            Array.Copy(inbuf, inpos, state.window, window + window_posn, this_run);
                            inpos += this_run;
                            window_posn += (uint)this_run;
                            break;

                        default:
                            return false; /* might as well */
                    }

                }
            }

            if (togo != 0)
                return false;

            Array.Copy(state.window, window + ((window_posn == 0) ? window_size : window_posn) - outlen, outbuf, 0, outlen);

            state.window_posn = window_posn;
            state.R0 = R0;
            state.R1 = R1;
            state.R2 = R2;

            /* intel E8 decoding */
            if ((state.frames_read++ < 32768) && state.intel_filesize != 0)
            {
                if (outlen <= 6 || state.intel_started == 0)
                {
                    state.intel_curpos += outlen;
                }
                else
                {
                    int data = 0; // outbuf[0];
                    int dataend = data + outlen - 10;
                    int curpos = state.intel_curpos;
                    int filesize = state.intel_filesize;
                    int abs_off, rel_off;

                    state.intel_curpos = curpos + outlen;

                    while (data < dataend)
                    {
                        if (outbuf[data++] != 0xE8)
                        {
                            curpos++;
                            continue;
                        }

                        abs_off = outbuf[data + 0] | (outbuf[data + 1] << 8) | (outbuf[data + 2] << 16) | (outbuf[data + 3] << 24);
                        if ((abs_off >= -curpos) && (abs_off < filesize))
                        {
                            rel_off = (abs_off >= 0) ? abs_off - curpos : abs_off + filesize;
                            outbuf[data + 0] = (byte)rel_off;
                            outbuf[data + 1] = (byte)(rel_off >> 8);
                            outbuf[data + 2] = (byte)(rel_off >> 16);
                            outbuf[data + 3] = (byte)(rel_off >> 24);
                        }
                        data += 4;
                        curpos += 5;
                    }
                }
            }
            
            return true;
        }
        /// <summary>
        /// Read and build the Huffman tree from the lengths
        /// </summary>
        private static int ReadLengths(byte[] lengths, uint first, uint last, Bits lb, State state, byte[] inbuf)
        {
            uint x, y;
            uint bitbuf = lb.BitBuffer;
            int bitsleft = lb.BitsLeft;
            int inpos = lb.InputPosition;

            for (x = 0; x < 20; x++)
            {
                y = READ_BITS(4, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                state.tblPRETREE_len[x] = (byte)y;
            }

            make_decode_table(LZX_PRETREE_MAXSYMBOLS, LZX_PRETREE_TABLEBITS, state.tblPRETREE_len, state.tblPRETREE_table);

            for (x = first; x < last;)
            {
                int z = READ_HUFFSYM(state.tblPRETREE_table, state.tblPRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                if (z == 17)
                {
                    y = READ_BITS(4, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    y += 4;
                    while (y-- > 0)
                    {
                        lengths[x++] = 0;
                    }
                }
                else if (z == 18)
                {
                    y = READ_BITS(5, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    y += 20;
                    while (y-- > 0)
                    {
                        lengths[x++] = 0;
                    }
                }
                else if (z == 19)
                {
                    y = READ_BITS(1, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    y += 4;

                    z = READ_HUFFSYM(state.tblPRETREE_table, state.tblPRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                    z = lengths[x] - z;
                    if (z < 0)
                        z += 17;

                    while (y-- > 0)
                    {
                        lengths[x++] = (byte)z;
                    }
                }
                else
                {
                    z = lengths[x] - z;
                    if (z < 0)
                        z += 17;

                    lengths[x++] = (byte)z;
                }
            }

            lb.BitBuffer = bitbuf;
            lb.BitsLeft = bitsleft;
            lb.InputPosition = inpos;
            return 0;
        }

        // Bitstream reading macros (LZX / intel little-endian byte order)
        #region Bitstream Reading Macros

        /*
        * These bit access routines work by using the area beyond the MSB and the
        * LSB as a free source of zeroes. This avoids having to mask any bits.
        * So we have to know the bit width of the bitbuffer variable.
        */

        /// <summary>
        /// Should be used first to set up the system
        /// </summary>
        private static void INIT_BITSTREAM(out int bitsleft, out uint bitbuf)
        {
            bitsleft = 0;
            bitbuf = 0;
        }

        /// <summary>
        /// Ensures there are at least N bits in the bit buffer. It can guarantee
        // up to 17 bits (i.e. it can read in 16 new bits when there is down to
        /// 1 bit in the buffer, and it can read 32 bits when there are 0 bits in
        /// the buffer).
        /// </summary>
        /// <remarks>Quantum reads bytes in normal order; LZX is little-endian order</remarks>
        private static void ENSURE_BITS(int n, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            while (bitsleft < n)
            {
                byte b0 = inpos + 0 < inbuf.Length ? inbuf[inpos + 0] : (byte)0;
                byte b1 = inpos + 1 < inbuf.Length ? inbuf[inpos + 1] : (byte)0;

                bitbuf |= (uint)(((b1 << 8) | b0) << (16 - bitsleft));
                bitsleft += 16;
                inpos += 2;
            }
        }

        /// <summary>
        /// Extracts (without removing) N bits from the bit buffer
        /// </summary>
        private static uint PEEK_BITS(int n, uint bitbuf)
        {
            return bitbuf >> (32 - n);
        }

        /// <summary>
        /// Removes N bits from the bit buffer
        /// </summary>
        private static void REMOVE_BITS(int n, ref int bitsleft, ref uint bitbuf)
        {
            bitbuf <<= n;
            bitsleft -= n;
        }

        /// <summary>
        /// Takes N bits from the buffer and puts them in v.
        /// </summary>
        private static uint READ_BITS(int n, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            uint v = 0;
            if (n > 0)
            {
                ENSURE_BITS(n, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                v = PEEK_BITS(n, bitbuf);
                REMOVE_BITS(n, ref bitsleft, ref bitbuf);
            }

            return v;
        }

        #endregion

        #region Huffman Methods

        /// <summary>
        /// This function was coded by David Tritscher. It builds a fast huffman
        /// decoding table out of just a canonical huffman code lengths table.
        /// </summary>
        /// <param name="nsyms">Total number of symbols in this huffman tree.</param>
        /// <param name="nbits">
        /// Any symbols with a code length of nbits or less can be decoded
        /// in one lookup of the table.
        /// </param>
        /// <param name="length">A table to get code lengths from [0 to syms-1]</param>
        /// <param name="table">The table to fill up with decoded symbols and pointers.</param>
        /// <returns>
        /// OK:    0
        /// error: 1
        /// </returns>
        private static int make_decode_table(uint nsyms, uint nbits, byte[] length, ushort[] table)
        {
            ushort sym;
            uint leaf;
            byte bit_num = 1;
            uint fill;
            uint pos = 0; /* the current position in the decode table */
            uint table_mask = (uint)(1 << (int)nbits);
            uint bit_mask = table_mask >> 1; /* don't do 0 length codes */
            uint next_symbol = bit_mask; /* base of allocation for long codes */

            /* fill entries for codes short enough for a direct mapping */
            while (bit_num <= nbits)
            {
                for (sym = 0; sym < nsyms; sym++)
                {
                    if (length[sym] == bit_num)
                    {
                        leaf = pos;

                        if ((pos += bit_mask) > table_mask) return 1; /* table overrun */

                        /* fill all possible lookups of this symbol with the symbol itself */
                        fill = bit_mask;
                        while (fill-- > 0) table[leaf++] = sym;
                    }
                }
                bit_mask >>= 1;
                bit_num++;
            }

            /* if there are any codes longer than nbits */
            if (pos != table_mask)
            {
                /* clear the remainder of the table */
                for (sym = (ushort)pos; sym < table_mask; sym++) table[sym] = 0;

                /* give ourselves room for codes to grow by up to 16 more bits */
                pos <<= 16;
                table_mask <<= 16;
                bit_mask = 1 << 15;

                while (bit_num <= 16)
                {
                    for (sym = 0; sym < nsyms; sym++)
                    {
                        if (length[sym] == bit_num)
                        {
                            leaf = pos >> 16;
                            for (fill = 0; fill < bit_num - nbits; fill++)
                            {
                                /* if this path hasn't been taken yet, 'allocate' two entries */
                                if (table[leaf] == 0)
                                {
                                    table[(next_symbol << 1)] = 0;
                                    table[(next_symbol << 1) + 1] = 0;
                                    table[leaf] = (ushort)next_symbol++;
                                }
                                /* follow the path and select either left or right for next bit */
                                leaf = (uint)(table[leaf] << 1);
                                if (((pos >> (int)(15 - fill)) & 1) != 0) leaf++;
                            }
                            table[leaf] = sym;

                            if ((pos += bit_mask) > table_mask) return 1; /* table overflow */
                        }
                    }
                    bit_mask >>= 1;
                    bit_num++;
                }
            }

            /* full table? */
            if (pos == table_mask) return 0;

            /* either erroneous table, or all elements are 0 - let's find out. */
            for (sym = 0; sym < nsyms; sym++) if (length[sym] != 0) return 1;
            return 0;
        }

        #endregion

        // Huffman macros
        #region Huffman Macros

        /// <summary>
        /// Decodes one huffman symbol from the bitstream using the stated table and
        /// puts it in v.
        /// </summary>
        private static int READ_HUFFSYM(ushort[] hufftbl, byte[] lentable, int tablebits, int maxsymbols, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            int v = 0, i, j = 0;
            ENSURE_BITS(16, inbuf, ref inpos, ref bitsleft, ref bitbuf);
            if ((i = hufftbl[PEEK_BITS(tablebits, bitbuf)]) >= maxsymbols)
            {
                j = 1 << (32 - tablebits);
                do
                {
                    j >>= 1;
                    i <<= 1;
                    i |= (bitbuf & j) != 0 ? 1 : 0;
                    if (j == 0)
                        throw new System.Exception();
                } while ((i = hufftbl[i]) >= maxsymbols);
            }

            j = lentable[v = i];
            REMOVE_BITS(j, ref bitsleft, ref bitbuf);
            return v;
        }

        /// <summary>
        /// Reads in code lengths for symbols first to last in the given table. The
        /// code lengths are stored in their own special LZX way.
        /// </summary>
        private static bool READ_LENGTHS(byte[] lentable, uint first, uint last, Bits lb, State state, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            lb.BitBuffer = bitbuf;
            lb.BitsLeft = bitsleft;
            lb.InputPosition = inpos;

            if (ReadLengths(lentable, first, last, lb, state, inbuf) != 0)
                return false;

            bitbuf = lb.BitBuffer;
            bitsleft = lb.BitsLeft;
            inpos = lb.InputPosition;
            return true;
        }

        #endregion
    }
}