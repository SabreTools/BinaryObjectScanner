using BurnOutSharp.Models.Compression.LZX;
using static BurnOutSharp.Models.Compression.LZX.Constants;

namespace BurnOutSharp.Compression.LZX
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
    public class Decompressor
    {
        /// <summary>
        /// Decompress a byte array using a given State
        /// </summary>
        public static bool Decompress(State state, int inlen, byte[] inbuf, int outlen, byte[] outbuf)
        {

            // TODO: Finish implementation
            return false;
        }

        /// <summary>
        /// Read and build the Huffman tree from the lengths
        /// </summary>
        private static int ReadLengths(byte[] lengths, uint first, uint last, Bits lb, State state, byte[] inbuf)
        {
            uint i, j, x, y;
            int z;

            uint bitbuf = lb.BitBuffer;
            int bitsleft = lb.BitsLeft;
            int inpos = lb.InputPosition;
            ushort[] hufftbl;

            for (x = 0; x < 20; x++)
            {
                y = READ_BITS(4, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                state.tblPRETREE_len[x] = (byte)y;
            }

            make_decode_table(LZX_PRETREE_MAXSYMBOLS, LZX_PRETREE_TABLEBITS, state.tblPRETREE_len, state.tblPRETREE_table);

            for (x = first; x < last;)
            {
                z = READ_HUFFSYM(state.tblPRETREE_table, state.tblPRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS, inbuf, ref inpos, ref bitsleft, ref bitbuf);
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
                bitbuf |= (uint)(((inbuf[inpos + 1] << 8) | inbuf[inpos + 0]) << (16 - bitsleft));
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