/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System.IO;
using static LibMSPackSharp.Constants;
using static LibMSPackSharp.Compression.Constants;

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
            int i;
            int lit_run = 0;
            int j, pos = 0, len, offset;
            uint[] types = new uint[6];

            // Reset global state
            lzh.INIT_BITS();
            BufferState state = lzh.RESTORE_BITS();

            for (i = 0; i < LZSS_WINDOW_SIZE; i++)
            {
                lzh.Window[i] = LZSS_WINDOW_FILL;
            }

            // Read 6 encoding types (for byte alignment) but only 5 are needed
            for (i = 0; i < 6; i++)
            {
                types[i] = (uint)lzh.READ_BITS_SAFE(4, state);
                if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                    return Error.MSPACK_ERR_OK;
            }

            // Read huffman table symbol lengths and build huffman trees
            BUILD_TREE(lzh, types[0], lzh.MATCHLEN1_table, lzh.MATCHLEN1_len, KWAJ_TABLEBITS, KWAJ_MATCHLEN1_SYMS, state);
            BUILD_TREE(lzh, types[1], lzh.MATCHLEN2_table, lzh.MATCHLEN2_len, KWAJ_TABLEBITS, KWAJ_MATCHLEN2_SYMS, state);
            BUILD_TREE(lzh, types[2], lzh.LITLEN_table, lzh.LITLEN_len, KWAJ_TABLEBITS, KWAJ_LITLEN_SYMS, state);
            BUILD_TREE(lzh, types[3], lzh.OFFSET_table, lzh.OFFSET_len, KWAJ_TABLEBITS, KWAJ_OFFSET_SYMS, state);
            BUILD_TREE(lzh, types[4], lzh.LITERAL_table, lzh.LITERAL_len, KWAJ_TABLEBITS, KWAJ_LITERAL_SYMS, state);

            while (lzh.EndOfInput == 0)
            {
                if (lit_run != 0)
                {
                    len = (int)lzh.READ_HUFFSYM_SAFE(lzh.MATCHLEN2_table, lzh.MATCHLEN2_len, KWAJ_MATCHLEN2_TBLSIZE, KWAJ_MATCHLEN2_SYMS, state);
                    if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;
                }
                else
                {
                    len = (int)lzh.READ_HUFFSYM_SAFE(lzh.MATCHLEN1_table, lzh.MATCHLEN1_len, KWAJ_MATCHLEN1_TBLSIZE, KWAJ_MATCHLEN1_SYMS, state);
                    if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;
                }

                if (len > 0)
                {
                    len += 2;
                    lit_run = 0; // Not the end of a literal run

                    j = (int)lzh.READ_HUFFSYM_SAFE(lzh.OFFSET_table, lzh.OFFSET_len, KWAJ_OFFSET_TBLSIZE, KWAJ_OFFSET_SYMS, state);
                    if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    offset = j << 6;

                    j = (int)lzh.READ_BITS_SAFE(6, state);
                    if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    offset |= j;

                    // Copy match as output and into the ring buffer
                    while (len-- > 0)
                    {
                        lzh.Window[pos] = lzh.Window[(pos + 4096 - offset) & 4095];
                        lzh.WRITE_BYTE(pos);
                        if (lzh.Error != Error.MSPACK_ERR_OK)
                            return lzh.Error;

                        pos++;
                        pos &= 4095;
                    }
                }
                else
                {
                    len = (int)lzh.READ_HUFFSYM_SAFE(lzh.LITLEN_table, lzh.LITLEN_len, KWAJ_LITLEN_TBLSIZE, KWAJ_LITLEN_SYMS, state);
                    if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    len++;
                    lit_run = (len == 32) ? 0 : 1; // End of a literal run?
                    while (len-- > 0)
                    {
                        j = (int)lzh.READ_HUFFSYM_SAFE(lzh.LITERAL_table, lzh.LITERAL_len, KWAJ_LITERAL_TBLSIZE, KWAJ_LITERAL_SYMS, state);
                        if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        // Copy as output and into the ring buffer
                        lzh.Window[pos] = (byte)j;

                        lzh.WRITE_BYTE(pos);
                        if (lzh.Error != Error.MSPACK_ERR_OK)
                            return lzh.Error;

                        pos++; pos &= 4095;
                    }
                }
            }

            return Error.MSPACK_ERR_OK;
        }

        private static Error BUILD_TREE(LZHKWAJStream lzh, uint type, ushort[] table, byte[] lengths, int tablebits, int maxsymbols, BufferState state)
        {
            lzh.STORE_BITS(state);

            Error err = ReadLens(lzh, type, (uint)maxsymbols, lzh.MATCHLEN1_len);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            BufferState temp = lzh.RESTORE_BITS();
            state.InputPointer = temp.InputPointer;
            state.InputEnd = temp.InputEnd;
            state.BitBuffer = temp.BitBuffer;
            state.BitsLeft = temp.BitsLeft;

            if (!CompressionStream.MakeDecodeTableMSB(maxsymbols, tablebits, lengths, table))
                return Error.MSPACK_ERR_DATAFORMAT;

            return Error.MSPACK_ERR_OK;
        }

        private static Error ReadLens(LZHKWAJStream lzh, uint type, uint numsyms, byte[] lens)
        {
            uint i, c, sel;

            BufferState state = lzh.RESTORE_BITS();

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
                    c = (uint)lzh.READ_BITS_SAFE(4, state);
                    if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        sel = (uint)lzh.READ_BITS_SAFE(1, state);
                        if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        if (sel == 0)
                        {
                            lens[i] = (byte)c;
                        }
                        else
                        {
                            sel = (uint)lzh.READ_BITS_SAFE(1, state);
                            if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                                return Error.MSPACK_ERR_OK;

                            if (sel == 0)
                            {
                                lens[i] = (byte)++c;
                            }
                            else
                            {
                                c = (uint)lzh.READ_BITS_SAFE(4, state);
                                if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                                    return Error.MSPACK_ERR_OK;

                                lens[i] = (byte)c;
                            }
                        }
                    }

                    break;

                case 2:
                    c = (uint)lzh.READ_BITS_SAFE(4, state);
                    if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        sel = (uint)lzh.READ_BITS_SAFE(2, state);
                        if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        if (sel == 3)
                        {
                            c = (uint)lzh.READ_BITS_SAFE(4, state);
                            if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                                return Error.MSPACK_ERR_OK;
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
                        c = (uint)lzh.READ_BITS_SAFE(4, state);
                        if (lzh.Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        lens[i] = (byte)c;
                    }

                    break;
            }

            lzh.STORE_BITS(state);

            return Error.MSPACK_ERR_OK;
        }
    }
}
