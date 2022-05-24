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

using static LibMSPackSharp.Compression.Constants;

namespace LibMSPackSharp.Compression
{
    public abstract partial class CompressionStream : BaseDecompressState
    {
        #region Common

        /// <summary>
        /// Per compression error code for decoding failure
        /// </summary>
        public abstract Error HUFF_ERROR();

        #endregion

        #region MSB

        /// <summary>
        /// Decodes the next huffman symbol from the input bitstream into var.
        /// Do not use this macro on a table unless build_decode_table() succeeded.
        /// </summary>
        public long READ_HUFFSYM_MSB(ushort[] table, byte[] lengths, int tablebits, int maxsymbols)
        {
            ENSURE_BITS(HUFF_MAXBITS);
            ushort sym = table[PEEK_BITS_MSB(tablebits)];
            if (sym >= maxsymbols)
                HUFF_TRAVERSE_MSB(ref sym, table, tablebits, maxsymbols);

            REMOVE_BITS_MSB(lengths[sym]);
            return sym;
        }

        /// <summary>
        /// Traverse for a single symbol
        /// </summary>
        private void HUFF_TRAVERSE_MSB(ref ushort sym, ushort[] table, int tablebits, int maxsymbols)
        {
            int i = 1 << (BITBUF_WIDTH - tablebits);
            do
            {
                if ((i >>= 1) == 0)
                {
                    Error = HUFF_ERROR();
                    return;
                }

                sym = table[(sym << 1) | ((BitBuffer & i) != 0 ? 1 : 0)];
            } while (sym >= maxsymbols);
        }

        /// <summary>
        /// This function was originally coded by David Tritscher.
        /// 
        /// It builds a fast huffman decoding table from
        /// a canonical huffman code lengths table.
        /// </summary>
        /// <param name="nsyms">total number of symbols in this huffman tree.</param>
        /// <param name="nbits">any symbols with a code length of nbits or less can be decoded in one lookup of the table.</param>
        /// <param name="length">A table to get code lengths from [0 to nsyms-1]</param>
        /// <param name="table">
        /// The table to fill up with decoded symbols and pointers.
        /// Should be ((1<<nbits) + (nsyms*2)) in length.
        /// </param>
        /// <returns>True for OK or false for error</returns>
        public static bool MakeDecodeTableMSB(int nsyms, int nbits, byte[] length, ushort[] table)
        {
            ushort sym, next_symbol;
            uint leaf, fill;
            byte bit_num;
            uint pos = 0; // The current position in the decode table
            uint table_mask = (uint)1 << nbits;
            uint bit_mask = table_mask >> 1; // Don't do 0 length codes

            // Fill entries for codes short enough for a direct mapping
            for (bit_num = 1; bit_num <= nbits; bit_num++)
            {
                for (sym = 0; sym < nsyms; sym++)
                {
                    if (length[sym] != bit_num)
                        continue;

                    leaf = pos;
                    if ((pos += bit_mask) > table_mask)
                        return false; // Table overrun

                    // Fill all possible lookups of this symbol with the symbol itself
                    for (fill = bit_mask; fill-- > 0;)
                    {
                        table[leaf++] = sym;
                    }
                }

                bit_mask >>= 1;
            }

            // Exit with success if table is now complete
            if (pos == table_mask)
                return true;

            // Mark all remaining table entries as unused
            for (sym = (ushort)pos; sym < table_mask; sym++)
            {
                table[sym] = 0xFFFF;
            }

            // next_symbol = base of allocation for long codes
            next_symbol = ((table_mask >> 1) < nsyms) ? (ushort)nsyms : (ushort)(table_mask >> 1);

            // Give ourselves room for codes to grow by up to 16 more bits.
            // codes now start at bit nbits+16 and end at (nbits+16-codelength)
            pos <<= 16;
            table_mask <<= 16;
            bit_mask = 1 << 15;

            for (bit_num = (byte)(nbits + 1); bit_num <= HUFF_MAXBITS; bit_num++)
            {
                for (sym = 0; sym < nsyms; sym++)
                {
                    if (length[sym] != bit_num)
                        continue;
                    if (pos >= table_mask)
                        return false; // Table overflow

                    leaf = pos >> 16;
                    for (fill = 0; fill < (bit_num - nbits); fill++)
                    {
                        // If this path hasn't been taken yet, 'allocate' two entries
                        if (table[leaf] == 0xFFFF)
                        {
                            table[(next_symbol << 1)] = 0xFFFF;
                            table[(next_symbol << 1) + 1] = 0xFFFF;
                            table[leaf] = next_symbol++;
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
            return pos == table_mask;
        }

        #endregion

        #region LSB

        /// <summary>
        /// Decodes the next huffman symbol from the input bitstream into var.
        /// Do not use this macro on a table unless build_decode_table() succeeded.
        /// </summary>
        public long READ_HUFFSYM_LSB(ushort[] table, byte[] lengths, int tablebits, int maxsymbols)
        {
            ENSURE_BITS(HUFF_MAXBITS);
            ushort sym = table[PEEK_BITS_LSB(tablebits)];
            if (sym >= maxsymbols)
                HUFF_TRAVERSE_LSB(ref sym, table, tablebits, maxsymbols);

            REMOVE_BITS_LSB(lengths[sym]);
            return sym;
        }

        /// <summary>
        /// Traverse for a single symbol
        /// </summary>
        private void HUFF_TRAVERSE_LSB(ref ushort sym, ushort[] table, int tablebits, int maxsymbols)
        {
            int i = tablebits - 1;
            do
            {
                if (i++ > HUFF_MAXBITS)
                {
                    Error = HUFF_ERROR();
                    return;
                }

                sym = table[(sym << 1) | ((BitBuffer >> i) & 1)];
            } while (sym >= maxsymbols);
        }

        /// <summary>
        /// This function was originally coded by David Tritscher.
        /// 
        /// It builds a fast huffman decoding table from
        /// a canonical huffman code lengths table.
        /// </summary>
        /// <param name="nsyms">total number of symbols in this huffman tree.</param>
        /// <param name="nbits">any symbols with a code length of nbits or less can be decoded in one lookup of the table.</param>
        /// <param name="length">A table to get code lengths from [0 to nsyms-1]</param>
        /// <param name="table">
        /// The table to fill up with decoded symbols and pointers.
        /// Should be ((1<<nbits) + (nsyms*2)) in length.
        /// </param>
        /// <returns>True for OK or false for error</returns>
        public static bool MakeDecodeTableLSB(int nsyms, int nbits, byte[] length, ushort[] table)
        {
            ushort sym, next_symbol;
            uint leaf, fill;
            uint reverse;
            byte bit_num;
            uint pos = 0; // The current position in the decode table
            uint table_mask = (uint)1 << nbits;
            uint bit_mask = table_mask >> 1; // Don't do 0 length codes

            // Fill entries for codes short enough for a direct mapping
            for (bit_num = 1; bit_num <= nbits; bit_num++)
            {
                for (sym = 0; sym < nsyms; sym++)
                {
                    if (length[sym] != bit_num)
                        continue;

                    // Reverse the significant bits
                    fill = length[sym];
                    reverse = pos >> (int)(nbits - fill);
                    leaf = 0;

                    do
                    {
                        leaf <<= 1;
                        leaf |= reverse & 1;
                        reverse >>= 1;
                    } while (--fill != 0);

                    if ((pos += bit_mask) > table_mask)
                        return false; // Table overrun

                    // Fill all possible lookups of this symbol with the symbol itself
                    fill = bit_mask;
                    next_symbol = (ushort)(1 << bit_num);

                    do
                    {
                        table[leaf] = sym;
                        leaf += next_symbol;
                    } while (--fill != 0);
                }

                bit_mask >>= 1;
            }

            // Exit with success if table is now complete
            if (pos == table_mask)
                return true;

            // Mark all remaining table entries as unused
            for (sym = (ushort)pos; sym < table_mask; sym++)
            {
                reverse = sym;
                leaf = 0;
                fill = (uint)nbits;

                do
                {
                    leaf <<= 1;
                    leaf |= reverse & 1;
                    reverse >>= 1;
                } while (--fill != 0);

                table[leaf] = 0xFFFF;
            }

            // next_symbol = base of allocation for long codes
            next_symbol = ((table_mask >> 1) < nsyms) ? (ushort)nsyms : (ushort)(table_mask >> 1);

            // Give ourselves room for codes to grow by up to 16 more bits.
            // codes now start at bit nbits+16 and end at (nbits+16-codelength)
            pos <<= 16;
            table_mask <<= 16;
            bit_mask = 1 << 15;

            for (bit_num = (byte)(nbits + 1); bit_num <= HUFF_MAXBITS; bit_num++)
            {
                for (sym = 0; sym < nsyms; sym++)
                {
                    if (length[sym] != bit_num)
                        continue;
                    if (pos >= table_mask)
                        return false; // Table overflow

                    // leaf = the first nbits of the code, reversed
                    reverse = pos >> 16;
                    leaf = 0;
                    fill = (uint)nbits;

                    do
                    {
                        leaf <<= 1;
                        leaf |= reverse & 1;
                        reverse >>= 1;
                    } while (--fill != 0);

                    for (fill = 0; fill < (bit_num - nbits); fill++)
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
            return pos == table_mask;
        }

        #endregion
    }
}
