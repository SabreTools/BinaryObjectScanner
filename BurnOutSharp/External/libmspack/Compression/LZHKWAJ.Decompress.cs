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
    public partial class LZHKWAJ
    {
        public static LZHKWAJ Init(SystemImpl sys, FileStream input, FileStream output)
        {
            if (sys == null || input == null || output == null)
                return null;

            return new LZHKWAJ()
            {
                System = sys,
                InputFileHandle = input,
                OutputFileHandle = output,
            };
        }

        public Error Decompress()
        {
            int i;
            int lit_run = 0;
            int j, pos = 0, len, offset;
            uint[] types = new uint[6];

            // Reset global state
            INIT_BITS();

            for (i = 0; i < LZSS_WINDOW_SIZE; i++)
            {
                Window[i] = LZSS_WINDOW_FILL;
            }

            // Read 6 encoding types (for byte alignment) but only 5 are needed
            for (i = 0; i < 6; i++)
            {
                types[i] = (uint)READ_BITS_SAFE(4);
                if (Error == Error.MSPACK_ERR_NOMEMORY)
                    return Error.MSPACK_ERR_OK;
            }

            // Read huffman table symbol lengths and build huffman trees
            BUILD_TREE(types[0], MATCHLEN1_table, MATCHLEN1_len, KWAJ_TABLEBITS, KWAJ_MATCHLEN1_SYMS);
            BUILD_TREE(types[1], MATCHLEN2_table, MATCHLEN2_len, KWAJ_TABLEBITS, KWAJ_MATCHLEN2_SYMS);
            BUILD_TREE(types[2], LITLEN_table, LITLEN_len, KWAJ_TABLEBITS, KWAJ_LITLEN_SYMS);
            BUILD_TREE(types[3], OFFSET_table, OFFSET_len, KWAJ_TABLEBITS, KWAJ_OFFSET_SYMS);
            BUILD_TREE(types[4], LITERAL_table, LITERAL_len, KWAJ_TABLEBITS, KWAJ_LITERAL_SYMS);

            while (EndOfInput == 0)
            {
                if (lit_run > 0)
                {
                    len = (int)READ_HUFFSYM_SAFE(MATCHLEN2_table, MATCHLEN2_len, KWAJ_MATCHLEN2_TBLSIZE, KWAJ_MATCHLEN2_SYMS);
                    if (Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;
                }
                else
                {
                    len = (int)READ_HUFFSYM_SAFE(MATCHLEN1_table, MATCHLEN1_len, KWAJ_MATCHLEN1_TBLSIZE, KWAJ_MATCHLEN1_SYMS);
                    if (Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;
                }

                if (len > 0)
                {
                    len += 2;
                    lit_run = 0; // Not the end of a literal run

                    j = (int)READ_HUFFSYM_SAFE(OFFSET_table, OFFSET_len, KWAJ_OFFSET_TBLSIZE, KWAJ_OFFSET_SYMS);
                    if (Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    offset = j << 6;

                    j = (int)READ_BITS_SAFE(6);
                    if (Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    offset |= j;

                    // Copy match as output and into the ring buffer
                    while (len-- > 0)
                    {
                        Window[pos] = Window[(pos + 4096 - offset) & 4095];
                        WRITE_BYTE(pos);
                        if (Error != Error.MSPACK_ERR_OK)
                            return Error;

                        pos++;
                        pos &= 4095;
                    }
                }
                else
                {
                    len = (int)READ_HUFFSYM_SAFE(LITLEN_table, LITLEN_len, KWAJ_LITLEN_TBLSIZE, KWAJ_LITLEN_SYMS);
                    if (Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    len++;
                    lit_run = (len == 32) ? 0 : 1; // End of a literal run?
                    while (len-- > 0)
                    {
                        j = (int)READ_HUFFSYM_SAFE(LITERAL_table, LITERAL_len, KWAJ_LITERAL_TBLSIZE, KWAJ_LITERAL_SYMS);
                        if (Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        // Copy as output and into the ring buffer
                        Window[pos] = (byte)j;

                        WRITE_BYTE(pos);
                        if (Error != Error.MSPACK_ERR_OK)
                            return Error;

                        pos++; pos &= 4095;
                    }
                }
            }

            return Error.MSPACK_ERR_OK;
        }

        private Error BUILD_TREE(uint type, ushort[] table, byte[] lengths, int tablebits, int maxsymbols)
        {
            Error err = ReadLens(type, (uint)maxsymbols, MATCHLEN1_len);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            if (!CompressionStream.MakeDecodeTableMSB(maxsymbols, tablebits, lengths, table))
                return Error.MSPACK_ERR_DATAFORMAT;

            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Safely read bits from the buffer
        /// </summary>
        private long READ_BITS_SAFE(int nbits)
        {
            long val = READ_BITS_MSB(nbits);
            if (EndOfInput != 0 && BitsLeft < EndOfInput)
                Error = Error.MSPACK_ERR_NOMEMORY;
            else
                Error = Error.MSPACK_ERR_OK;

            return val;
        }

        /// <summary>
        /// Safely read a symbol from a Huffman tree
        /// </summary>
        private long READ_HUFFSYM_SAFE(ushort[] table, byte[] lengths, int tablebits, int maxsymbols)
        {
            long val = READ_HUFFSYM_MSB(table, lengths, tablebits, maxsymbols);
            if (EndOfInput != 0 && BitsLeft < EndOfInput)
                Error = Error.MSPACK_ERR_NOMEMORY;
            else
                Error = Error.MSPACK_ERR_OK;

            return val;
        }

        /// <summary>
        /// Write a single byte to the output stream
        /// </summary>
        private void WRITE_BYTE(int pos)
        {
            if (System.Write(OutputFileHandle, Window, pos, 1) != 1)
                Error = Error.MSPACK_ERR_WRITE;
        }

        private Error ReadLens(uint type, uint numsyms, byte[] lens)
        {
            uint i, c, sel;

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
                    c = (uint)READ_BITS_SAFE(4);
                    if (Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        sel = (uint)READ_BITS_SAFE(1);
                        if (Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        if (sel == 0)
                        {
                            lens[i] = (byte)c;
                        }
                        else
                        {
                            sel = (uint)READ_BITS_SAFE(1);
                            if (Error == Error.MSPACK_ERR_NOMEMORY)
                                return Error.MSPACK_ERR_OK;

                            if (sel == 0)
                            {
                                lens[i] = (byte)++c;
                            }
                            else
                            {
                                c = (uint)READ_BITS_SAFE(4);
                                if (Error == Error.MSPACK_ERR_NOMEMORY)
                                    return Error.MSPACK_ERR_OK;

                                lens[i] = (byte)c;
                            }
                        }
                    }

                    break;

                case 2:
                    c = (uint)READ_BITS_SAFE(4);
                    if (Error == Error.MSPACK_ERR_NOMEMORY)
                        return Error.MSPACK_ERR_OK;

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        sel = (uint)READ_BITS_SAFE(2);
                        if (Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        if (sel == 3)
                        {
                            c = (uint)READ_BITS_SAFE(4);
                            if (Error == Error.MSPACK_ERR_NOMEMORY)
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
                        c = (uint)READ_BITS_SAFE(4);
                        if (Error == Error.MSPACK_ERR_NOMEMORY)
                            return Error.MSPACK_ERR_OK;

                        lens[i] = (byte)c;
                    }

                    break;
            }

            return Error.MSPACK_ERR_OK;
        }
    }
}
