/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using LibMSPackSharp.Compression;

namespace LibMSPackSharp.KWAJ
{
    public class InternalStream : CompressionStream
    {
        // Huffman code lengths

        public byte[] MATCHLEN1_len { get; set; } = new byte[Implementation.KWAJ_MATCHLEN1_SYMS];
        public byte[] MATCHLEN2_len { get; set; } = new byte[Implementation.KWAJ_MATCHLEN2_SYMS];
        public byte[] LITLEN_len { get; set; } = new byte[Implementation.KWAJ_LITLEN_SYMS];
        public byte[] OFFSET_len { get; set; } = new byte[Implementation.KWAJ_OFFSET_SYMS];
        public byte[] LITERAL_len { get; set; } = new byte[Implementation.KWAJ_LITERAL_SYMS];

        // Huffman decoding tables

        public ushort[] MATCHLEN1_table { get; set; } = new ushort[Implementation.KWAJ_MATCHLEN1_TBLSIZE];
        public ushort[] MATCHLEN2_table { get; set; } = new ushort[Implementation.KWAJ_MATCHLEN2_TBLSIZE];
        public ushort[] LITLEN_table { get; set; } = new ushort[Implementation.KWAJ_LITLEN_TBLSIZE];
        public ushort[] OFFSET_table { get; set; } = new ushort[Implementation.KWAJ_OFFSET_TBLSIZE];
        public ushort[] LITERAL_table { get; set; } = new ushort[Implementation.KWAJ_LITERAL_TBLSIZE];

        // History window

        public byte[] Window { get; set; } = new byte[LZSS.LZSS_WINDOW_SIZE];

        public override Error READ_BYTES(ref int i_ptr, ref int i_end, ref uint bitsLeft, ref uint bitBuffer)
        {
            Error error = Error.MSPACK_ERR_OK;
            if (i_ptr >= i_end)
            {
                if ((error = Implementation.LZHReadInput(this)) != Error.MSPACK_ERR_OK)
                    return error;

                i_ptr = InputPointer;
                i_end = InputLength;
            }

            INJECT_BITS(InputBuffer[i_ptr++], 8, ref bitsLeft, ref bitBuffer);
            return error;
        }

        public override int HUFF_ERROR() => (int)Error.MSPACK_ERR_DATAFORMAT;
    }
}
