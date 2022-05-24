/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System.IO;
using LibMSPackSharp.KWAJ;
using static LibMSPackSharp.Constants;

namespace LibMSPackSharp.Compression
{
    /// <summary>
    /// In the KWAJ LZH format, there is no special 'eof' marker, it just
    /// ends. Depending on how many bits are left in the final byte when
    /// the stream ends, that might be enough to start another literal or
    /// match. The only easy way to detect that we've come to an end is to
    /// guard all bit-reading. We allow fake bits to be read once we reach
    /// the end of the stream, but we check if we then consumed any of
    /// those fake bits, after doing the READ_BITS / READ_HUFFSYM. This
    /// isn't how the default ReadInput works (it simply lets
    /// 2 fake bytes in then stops), so we implement our own.
    /// </summary>
    public class LZHKWAJ
    {
        public static LZHKWAJStream Init(SystemImpl sys, FileStream input, FileStream output)
        {
            if (sys == null || input == null || output == null)
                return null;

            return new LZHKWAJStream()
            {
                System = sys,
                InputFileHandle = input,
                OutputFileHandle = output,
            };
        }

        public static Error Decompress(LZHKWAJStream lzh)
        {
            uint bit_buffer;
            int bits_left, i;
            ushort sym;
            int i_ptr, i_end, lit_run = 0;
            int j, pos = 0, len, offset;
            Error err;
            uint[] types = new uint[6];

            // Reset global state

            //INIT_BITS
            {
                lzh.InputPointer = 0;
                lzh.InputEnd = 0;
                lzh.BitBuffer = 0;
                lzh.BitsLeft = 0;
                lzh.EndOfInput = 0;
            }

            //RESTORE_BITS
            {
                i_ptr = lzh.InputPointer;
                i_end = lzh.InputEnd;
                bit_buffer = lzh.BitBuffer;
                bits_left = lzh.BitsLeft;
            }

            for (i = 0; i < LZSS.LZSS_WINDOW_SIZE; i++)
            {
                lzh.Window[i] = LZSS.LZSS_WINDOW_FILL;
            }

            // Read 6 encoding types (for byte alignment) but only 5 are needed
            for (i = 0; i < 6; i++)
            {
                //READ_BITS_SAFE(types[i], 4)
                {
                    //READ_BITS(types[i], 4)
                    {
                        //ENSURE_BITS(nbits)
                        {
                            while (bits_left < (4))
                            {
                                //READ_BYTES
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                            return err;

                                        i_ptr = lzh.InputPointer;
                                        i_end = lzh.InputEnd;
                                    }

                                    //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                    {
                                        bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                        bits_left += (8);
                                    }
                                }
                            }
                        }

                        (types[i]) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (4))); //PEEK_BITS(4)

                        //REMOVE_BITS(4)
                        {
                            bit_buffer <<= (4);
                            bits_left -= (4);
                        }
                    }

                    if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                        return Error.MSPACK_ERR_OK;
                }
            }

            // Read huffman table symbol lengths and build huffman trees

            //BUILD_TREE(MATCHLEN1, types[0])
            {
                //STORE_BITS
                {
                    lzh.InputPointer = i_ptr;
                    lzh.InputEnd = i_end;
                    lzh.BitBuffer = bit_buffer;
                    lzh.BitsLeft = bits_left;
                }

                err = ReadLens(lzh, types[0], KWAJ_MATCHLEN1_SYMS, lzh.MATCHLEN1_len);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                //RESTORE_BITS
                {
                    i_ptr = lzh.InputPointer;
                    i_end = lzh.InputEnd;
                    bit_buffer = lzh.BitBuffer;
                    bits_left = lzh.BitsLeft;
                }

                if (!CompressionStream.MakeDecodeTable(KWAJ_MATCHLEN1_SYMS, KWAJ_TABLEBITS, lzh.MATCHLEN1_len, lzh.MATCHLEN1_table, msb: true))
                    return Error.MSPACK_ERR_DATAFORMAT;
            }

            //BUILD_TREE(MATCHLEN2, types[1])
            {
                //STORE_BITS
                {
                    lzh.InputPointer = i_ptr;
                    lzh.InputEnd = i_end;
                    lzh.BitBuffer = bit_buffer;
                    lzh.BitsLeft = bits_left;
                }

                err = ReadLens(lzh, types[1], KWAJ_MATCHLEN2_SYMS, lzh.MATCHLEN2_len);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                //RESTORE_BITS
                {
                    i_ptr = lzh.InputPointer;
                    i_end = lzh.InputEnd;
                    bit_buffer = lzh.BitBuffer;
                    bits_left = lzh.BitsLeft;
                }

                if (!CompressionStream.MakeDecodeTable(KWAJ_MATCHLEN2_SYMS, KWAJ_TABLEBITS, lzh.MATCHLEN2_len, lzh.MATCHLEN2_table, msb: true))
                    return Error.MSPACK_ERR_DATAFORMAT;
            }

            //BUILD_TREE(LITLEN, types[2])
            {
                //STORE_BITS
                {
                    lzh.InputPointer = i_ptr;
                    lzh.InputEnd = i_end;
                    lzh.BitBuffer = bit_buffer;
                    lzh.BitsLeft = bits_left;
                }

                err = ReadLens(lzh, types[2], KWAJ_LITLEN_SYMS, lzh.LITLEN_len);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                //RESTORE_BITS
                {
                    i_ptr = lzh.InputPointer;
                    i_end = lzh.InputEnd;
                    bit_buffer = lzh.BitBuffer;
                    bits_left = lzh.BitsLeft;
                }

                if (!CompressionStream.MakeDecodeTable(KWAJ_LITLEN_SYMS, KWAJ_TABLEBITS, lzh.LITLEN_len, lzh.LITLEN_table, msb: true))
                    return Error.MSPACK_ERR_DATAFORMAT;
            }

            //BUILD_TREE(OFFSET, types[3])
            {
                //STORE_BITS
                {
                    lzh.InputPointer = i_ptr;
                    lzh.InputEnd = i_end;
                    lzh.BitBuffer = bit_buffer;
                    lzh.BitsLeft = bits_left;
                }

                err = ReadLens(lzh, types[3], KWAJ_OFFSET_SYMS, lzh.OFFSET_len);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                //RESTORE_BITS
                {
                    i_ptr = lzh.InputPointer;
                    i_end = lzh.InputEnd;
                    bit_buffer = lzh.BitBuffer;
                    bits_left = lzh.BitsLeft;
                }

                if (!CompressionStream.MakeDecodeTable(KWAJ_OFFSET_SYMS, KWAJ_TABLEBITS, lzh.OFFSET_len, lzh.OFFSET_table, msb: true))
                    return Error.MSPACK_ERR_DATAFORMAT;
            }

            //BUILD_TREE(LITERAL, types[4])
            {
                //STORE_BITS
                {
                    lzh.InputPointer = i_ptr;
                    lzh.InputEnd = i_end;
                    lzh.BitBuffer = bit_buffer;
                    lzh.BitsLeft = bits_left;
                }

                err = ReadLens(lzh, types[4], KWAJ_LITERAL_SYMS, lzh.LITERAL_len);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                //RESTORE_BITS
                {
                    i_ptr = lzh.InputPointer;
                    i_end = lzh.InputEnd;
                    bit_buffer = lzh.BitBuffer;
                    bits_left = lzh.BitsLeft;
                }

                if (!CompressionStream.MakeDecodeTable(KWAJ_LITERAL_SYMS, KWAJ_TABLEBITS, lzh.LITERAL_len, lzh.LITERAL_table, msb: true))
                    return Error.MSPACK_ERR_DATAFORMAT;
            }

            while (lzh.EndOfInput == 0)
            {
                if (lit_run != 0)
                {
                    //READ_HUFFSYM_SAFE(MATCHLEN2, len)
                    {
                        //READ_HUFFSYM(MATCHLEN2, len)
                        {
                            //ENSURE_BITS(CompressionStream.HUFF_MAXBITS)
                            {
                                while (bits_left < (CompressionStream.HUFF_MAXBITS))
                                {
                                    //READ_BYTES
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                return err;

                                            i_ptr = lzh.InputPointer;
                                            i_end = lzh.InputEnd;
                                        }

                                        //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            sym = lzh.MATCHLEN2_table[(bit_buffer >> (CompressionStream.BITBUF_WIDTH - (KWAJ_TABLEBITS)))]; //PEEK_BITS(TABLEBITS(MATCHLEN2))
                            if (sym >= KWAJ_MATCHLEN2_SYMS)
                            {
                                //HUFF_TRAVERSE(MATCHLEN2)
                                {
                                    i = 1 << (CompressionStream.BITBUF_WIDTH - KWAJ_TABLEBITS);
                                    do
                                    {
                                        if ((i >>= 1) == 0)
                                            return Error.MSPACK_ERR_DATAFORMAT;

                                        sym = lzh.MATCHLEN2_table[(sym << 1) | ((bit_buffer & i) != 0 ? 1 : 0)];
                                    } while (sym >= KWAJ_MATCHLEN2_SYMS);
                                }
                            }

                            (len) = sym;
                            i = lzh.MATCHLEN2_len[sym];

                            //REMOVE_BITS(i)
                            {
                                bit_buffer <<= (i);
                                bits_left -= (i);
                            }
                        }

                        if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                            return Error.MSPACK_ERR_OK;
                    }
                }
                else
                {
                    //READ_HUFFSYM_SAFE(MATCHLEN1, len)
                    {
                        //READ_HUFFSYM(MATCHLEN1, len)
                        {
                            //ENSURE_BITS(CompressionStream.HUFF_MAXBITS)
                            {
                                while (bits_left < (CompressionStream.HUFF_MAXBITS))
                                {
                                    //READ_BYTES
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                return err;

                                            i_ptr = lzh.InputPointer;
                                            i_end = lzh.InputEnd;
                                        }

                                        //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            sym = lzh.MATCHLEN1_table[(bit_buffer >> (CompressionStream.BITBUF_WIDTH - (KWAJ_TABLEBITS)))]; //PEEK_BITS(TABLEBITS(MATCHLEN1))
                            if (sym >= KWAJ_MATCHLEN1_SYMS)
                            {
                                //HUFF_TRAVERSE(MATCHLEN1)
                                {
                                    i = 1 << (CompressionStream.BITBUF_WIDTH - KWAJ_TABLEBITS);
                                    do
                                    {
                                        if ((i >>= 1) == 0)
                                            return Error.MSPACK_ERR_DATAFORMAT;

                                        sym = lzh.MATCHLEN1_table[(sym << 1) | ((bit_buffer & i) != 0 ? 1 : 0)];
                                    } while (sym >= KWAJ_MATCHLEN1_SYMS);
                                }
                            }

                            (len) = sym;
                            i = lzh.MATCHLEN1_len[sym];

                            //REMOVE_BITS(i)
                            {
                                bit_buffer <<= (i);
                                bits_left -= (i);
                            }
                        }

                        if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                            return Error.MSPACK_ERR_OK;
                    }
                }

                if (len > 0)
                {
                    len += 2;
                    lit_run = 0; // Not the end of a literal run

                    //READ_HUFFSYM_SAFE(OFFSET, j)
                    {
                        //READ_HUFFSYM(OFFSET, j)
                        {
                            //ENSURE_BITS(CompressionStream.HUFF_MAXBITS)
                            {
                                while (bits_left < (CompressionStream.HUFF_MAXBITS))
                                {
                                    //READ_BYTES
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                return err;

                                            i_ptr = lzh.InputPointer;
                                            i_end = lzh.InputEnd;
                                        }

                                        //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            sym = lzh.OFFSET_table[(bit_buffer >> (CompressionStream.BITBUF_WIDTH - (KWAJ_TABLEBITS)))]; //PEEK_BITS(TABLEBITS(OFFSET))
                            if (sym >= KWAJ_OFFSET_SYMS)
                            {
                                //HUFF_TRAVERSE(OFFSET)
                                {
                                    i = 1 << (CompressionStream.BITBUF_WIDTH - KWAJ_TABLEBITS);
                                    do
                                    {
                                        if ((i >>= 1) == 0)
                                            return Error.MSPACK_ERR_DATAFORMAT;

                                        sym = lzh.OFFSET_table[(sym << 1) | ((bit_buffer & i) != 0 ? 1 : 0)];
                                    } while (sym >= KWAJ_OFFSET_SYMS);
                                }
                            }

                            (j) = sym;
                            i = lzh.OFFSET_len[sym];

                            //REMOVE_BITS(i)
                            {
                                bit_buffer <<= (i);
                                bits_left -= (i);
                            }
                        }

                        if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                            return Error.MSPACK_ERR_OK;
                    }

                    offset = j << 6;

                    //READ_BITS_SAFE(j, 6)
                    {
                        //READ_BITS(j, 6)
                        {
                            //ENSURE_BITS(6)
                            {
                                while (bits_left < (6))
                                {
                                    //READ_BYTES
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                return err;

                                            i_ptr = lzh.InputPointer;
                                            i_end = lzh.InputEnd;
                                        }

                                        //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            (j) = (int)(bit_buffer >> (CompressionStream.BITBUF_WIDTH - (6))); //PEEK_BITS(6)

                            //REMOVE_BITS(6)
                            {
                                bit_buffer <<= (6);
                                bits_left -= (6);
                            }
                        }

                        if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                            return Error.MSPACK_ERR_OK;
                    }

                    offset |= j;

                    // Copy match as output and into the ring buffer
                    while (len-- > 0)
                    {
                        lzh.Window[pos] = lzh.Window[(pos + 4096 - offset) & 4095];

                        //WRITE_BYTE 
                        {
                            if (lzh.System.Write(lzh.OutputFileHandle, lzh.Window, pos, 1) != 1)
                                return Error.MSPACK_ERR_WRITE;
                        }

                        pos++;
                        pos &= 4095;
                    }
                }
                else
                {
                    //READ_HUFFSYM_SAFE(LITLEN, len)
                    {
                        //READ_HUFFSYM(LITLEN, len)
                        {
                            //ENSURE_BITS(CompressionStream.HUFF_MAXBITS)
                            {
                                while (bits_left < (CompressionStream.HUFF_MAXBITS))
                                {
                                    //READ_BYTES
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                return err;

                                            i_ptr = lzh.InputPointer;
                                            i_end = lzh.InputEnd;
                                        }

                                        //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            sym = lzh.LITLEN_table[(bit_buffer >> (CompressionStream.BITBUF_WIDTH - (KWAJ_TABLEBITS)))]; //PEEK_BITS(TABLEBITS(LITLEN))
                            if (sym >= KWAJ_LITLEN_SYMS)
                            {
                                //HUFF_TRAVERSE(tbl)
                                {
                                    i = 1 << (CompressionStream.BITBUF_WIDTH - KWAJ_TABLEBITS);
                                    do
                                    {
                                        if ((i >>= 1) == 0)
                                            return Error.MSPACK_ERR_DATAFORMAT;

                                        sym = lzh.LITLEN_table[(sym << 1) | ((bit_buffer & i) != 0 ? 1 : 0)];
                                    } while (sym >= KWAJ_LITLEN_SYMS);
                                }
                            }

                            (len) = sym;
                            i = lzh.LITLEN_len[sym];

                            //REMOVE_BITS(i)
                            {
                                bit_buffer <<= (i);
                                bits_left -= (i);
                            }
                        }

                        if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                            return Error.MSPACK_ERR_OK;
                    }

                    len++;
                    lit_run = (len == 32) ? 0 : 1; // End of a literal run?
                    while (len-- > 0)
                    {
                        //READ_HUFFSYM_SAFE(LITERAL, j)
                        {
                            //READ_HUFFSYM(LITERAL, j)
                            {
                                //ENSURE_BITS(CompressionStream.HUFF_MAXBITS)
                                {
                                    while (bits_left < (CompressionStream.HUFF_MAXBITS))
                                    {
                                        //READ_BYTES
                                        {
                                            if (i_ptr >= i_end)
                                            {
                                                if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                    return err;

                                                i_ptr = lzh.InputPointer;
                                                i_end = lzh.InputEnd;
                                            }

                                            //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                sym = lzh.LITERAL_table[(bit_buffer >> (CompressionStream.BITBUF_WIDTH - (KWAJ_TABLEBITS)))]; //PEEK_BITS(TABLEBITS(LITERAL))
                                if (sym >= KWAJ_LITERAL_SYMS)
                                {
                                    //HUFF_TRAVERSE(LITERAL)
                                    {
                                        i = 1 << (CompressionStream.BITBUF_WIDTH - KWAJ_TABLEBITS);
                                        do
                                        {
                                            if ((i >>= 1) == 0)
                                                return Error.MSPACK_ERR_DATAFORMAT;

                                            sym = lzh.LITERAL_table[(sym << 1) | ((bit_buffer & i) != 0 ? 1 : 0)];
                                        } while (sym >= KWAJ_LITERAL_SYMS);
                                    }
                                }

                                (j) = sym;
                                i = lzh.LITERAL_len[sym];

                                //REMOVE_BITS(i)
                                {
                                    bit_buffer <<= (i);
                                    bits_left -= (i);
                                }
                            }

                            if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                                return Error.MSPACK_ERR_OK;
                        }

                        // Copy as output and into the ring buffer
                        lzh.Window[pos] = (byte)j;

                        //WRITE_BYTE 
                        {
                            if (lzh.System.Write(lzh.OutputFileHandle, lzh.Window, pos, 1) != 1)
                                return Error.MSPACK_ERR_WRITE;
                        }

                        pos++; pos &= 4095;
                    }
                }
            }

            return Error.MSPACK_ERR_OK;
        }

        private static Error ReadLens(LZHKWAJStream lzh, uint type, uint numsyms, byte[] lens)
        {
            uint bit_buffer;
            int bits_left;
            int i_ptr, i_end;
            uint i, c, sel;
            Error err;

            //RESTORE_BITS
            {
                i_ptr = lzh.InputPointer;
                i_end = lzh.InputEnd;
                bit_buffer = lzh.BitBuffer;
                bits_left = lzh.BitsLeft;
            }

            switch (type)
            {
                case 0:
                    i = numsyms;
                    c = (uint)((i == 16) ? 4 : (i == 32) ? 5 : (i == 64) ? 6 : (i == 256) ? 8 : 0);
                    for (i = 0; i < numsyms; i++)
                    {
                        lens[i] = (byte)c;
                    }

                    break;

                case 1:
                    //READ_BITS_SAFE(c, 4)
                    {
                        //READ_BITS(val, 4)
                        {
                            //ENSURE_BITS(4)
                            {
                                while (bits_left < (4))
                                {
                                    //READ_BYTES
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                return err;

                                            i_ptr = lzh.InputPointer;
                                            i_end = lzh.InputEnd;
                                        }

                                        //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            (c) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (4))); //PEEK_BITS(4)

                            //REMOVE_BITS(4)
                            {
                                bit_buffer <<= (4);
                                bits_left -= (4);
                            }
                        }

                        if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                            return Error.MSPACK_ERR_OK;
                    }

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        //READ_BITS_SAFE(sel, 1)
                        {
                            //READ_BITS(val, 1)
                            {
                                //ENSURE_BITS(1)
                                {
                                    while (bits_left < (1))
                                    {
                                        //READ_BYTES
                                        {
                                            if (i_ptr >= i_end)
                                            {
                                                if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                    return err;

                                                i_ptr = lzh.InputPointer;
                                                i_end = lzh.InputEnd;
                                            }

                                            //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (sel) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (1))); //PEEK_BITS(1)

                                //REMOVE_BITS(1)
                                {
                                    bit_buffer <<= (1);
                                    bits_left -= (1);
                                }
                            }

                            if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                                return Error.MSPACK_ERR_OK;
                        }

                        if (sel == 0)
                        {
                            lens[i] = (byte)c;
                        }
                        else
                        {
                            //READ_BITS_SAFE(sel, 1)
                            {
                                //READ_BITS(val, 1)
                                {
                                    //ENSURE_BITS(1)
                                    {
                                        while (bits_left < (1))
                                        {
                                            //READ_BYTES
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                        return err;

                                                    i_ptr = lzh.InputPointer;
                                                    i_end = lzh.InputEnd;
                                                }

                                                //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                                {
                                                    bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                                    bits_left += (8);
                                                }
                                            }
                                        }
                                    }

                                    (sel) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (1))); //PEEK_BITS(1)

                                    //REMOVE_BITS(1)
                                    {
                                        bit_buffer <<= (1);
                                        bits_left -= (1);
                                    }
                                }

                                if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                                    return Error.MSPACK_ERR_OK;
                            }

                            if (sel == 0)
                            {
                                lens[i] = (byte)++c;
                            }
                            else
                            {
                                //READ_BITS_SAFE(c, 4)
                                {
                                    //READ_BITS(val, 4)
                                    {
                                        //ENSURE_BITS(4)
                                        {
                                            while (bits_left < (4))
                                            {
                                                //READ_BYTES
                                                {
                                                    if (i_ptr >= i_end)
                                                    {
                                                        if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                            return err;

                                                        i_ptr = lzh.InputPointer;
                                                        i_end = lzh.InputEnd;
                                                    }

                                                    //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                                    {
                                                        bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                                        bits_left += (8);
                                                    }
                                                }
                                            }
                                        }

                                        (c) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (4))); //PEEK_BITS(4)

                                        //REMOVE_BITS(4)
                                        {
                                            bit_buffer <<= (4);
                                            bits_left -= (4);
                                        }
                                    }

                                    if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                                        return Error.MSPACK_ERR_OK;
                                }

                                lens[i] = (byte)c;
                            }
                        }
                    }

                    break;

                case 2:
                    //READ_BITS_SAFE(c, 4)
                    {
                        //READ_BITS(val, 4)
                        {
                            //ENSURE_BITS(4)
                            {
                                while (bits_left < (4))
                                {
                                    //READ_BYTES
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                return err;

                                            i_ptr = lzh.InputPointer;
                                            i_end = lzh.InputEnd;
                                        }

                                        //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            (c) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (4))); //PEEK_BITS(4)

                            //REMOVE_BITS(4)
                            {
                                bit_buffer <<= (4);
                                bits_left -= (4);
                            }
                        }

                        if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                            return Error.MSPACK_ERR_OK;
                    }

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        //READ_BITS_SAFE(sel, 2)
                        {
                            //READ_BITS(sel, 2)
                            {
                                //ENSURE_BITS(2)
                                {
                                    while (bits_left < (2))
                                    {
                                        //READ_BYTES
                                        {
                                            if (i_ptr >= i_end)
                                            {
                                                if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                    return err;

                                                i_ptr = lzh.InputPointer;
                                                i_end = lzh.InputEnd;
                                            }

                                            //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (sel) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (2))); //PEEK_BITS(2)

                                //REMOVE_BITS(2)
                                {
                                    bit_buffer <<= (2);
                                    bits_left -= (2);
                                }
                            }

                            if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                                return Error.MSPACK_ERR_OK;
                        }

                        if (sel == 3)
                        {
                            //READ_BITS_SAFE(c, 4)
                            {
                                //READ_BITS(c, 4)
                                {
                                    //ENSURE_BITS(4)
                                    {
                                        while (bits_left < (4))
                                        {
                                            //READ_BYTES
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                        return err;

                                                    i_ptr = lzh.InputPointer;
                                                    i_end = lzh.InputEnd;
                                                }

                                                //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                                {
                                                    bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                                    bits_left += (8);
                                                }
                                            }
                                        }
                                    }

                                    (c) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (4))); //PEEK_BITS(4)

                                    //REMOVE_BITS(4)
                                    {
                                        bit_buffer <<= (4);
                                        bits_left -= (4);
                                    }
                                }

                                if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                                    return Error.MSPACK_ERR_OK;
                            }
                        }
                        else
                        {
                            c += (uint)((byte)sel - 1);
                        }

                        lens[i] = (byte)c;
                    }

                    break;

                case 3:
                    for (i = 0; i < numsyms; i++)
                    {
                        //READ_BITS_SAFE(c, 4)
                        {
                            //READ_BITS(c, 4)
                            {
                                //ENSURE_BITS(4)
                                {
                                    while (bits_left < (4))
                                    {
                                        //READ_BYTES
                                        {
                                            if (i_ptr >= i_end)
                                            {
                                                if ((err = ReadInput(lzh)) != Error.MSPACK_ERR_OK)
                                                    return err;

                                                i_ptr = lzh.InputPointer;
                                                i_end = lzh.InputEnd;
                                            }

                                            //INJECT_BITS(lzh.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)(lzh.InputBuffer[i_ptr++]) << (CompressionStream.BITBUF_WIDTH - (8) - bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (c) = (bit_buffer >> (CompressionStream.BITBUF_WIDTH - (4))); //PEEK_BITS(4)

                                //REMOVE_BITS(4)
                                {
                                    bit_buffer <<= (4);
                                    bits_left -= (4);
                                }
                            }

                            if (lzh.EndOfInput != 0 && bits_left < lzh.EndOfInput)
                                return Error.MSPACK_ERR_OK;
                        }

                        lens[i] = (byte)c;
                    }

                    break;
            }

            //STORE_BITS
            {
                lzh.InputPointer = i_ptr;
                lzh.InputEnd = i_end;
                lzh.BitBuffer = bit_buffer;
                lzh.BitsLeft = bits_left;
            }

            return Error.MSPACK_ERR_OK;
        }

        private static Error ReadInput(LZHKWAJStream lzh)
        {
            int read;
            if (lzh.EndOfInput != 0)
            {
                lzh.EndOfInput += 8;
                lzh.InputBuffer[0] = 0;
                read = 1;
            }
            else
            {
                read = lzh.System.Read(lzh.InputFileHandle, lzh.InputBuffer, 0, KWAJ_INPUT_SIZE);
                if (read < 0)
                    return Error.MSPACK_ERR_READ;

                if (read == 0)
                {
                    lzh.InputEnd = 8;
                    lzh.InputBuffer[0] = 0;
                    read = 1;
                }
            }

            // Update InputPointer and InputLength
            lzh.InputPointer = 0;
            lzh.InputEnd = read;
            return Error.MSPACK_ERR_OK;
        }
    }
}
