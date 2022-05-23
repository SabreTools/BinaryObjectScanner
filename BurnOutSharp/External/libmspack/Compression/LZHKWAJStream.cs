/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using static LibMSPackSharp.Constants;

namespace LibMSPackSharp.Compression
{
    public class LZHKWAJStream : CompressionStream
    {
        // Huffman code lengths

        public byte[] MATCHLEN1_len { get; set; } = new byte[KWAJ_MATCHLEN1_SYMS];
        public byte[] MATCHLEN2_len { get; set; } = new byte[KWAJ_MATCHLEN2_SYMS];
        public byte[] LITLEN_len { get; set; } = new byte[KWAJ_LITLEN_SYMS];
        public byte[] OFFSET_len { get; set; } = new byte[KWAJ_OFFSET_SYMS];
        public byte[] LITERAL_len { get; set; } = new byte[KWAJ_LITERAL_SYMS];

        // Huffman decoding tables

        public ushort[] MATCHLEN1_table { get; set; } = new ushort[KWAJ_MATCHLEN1_TBLSIZE];
        public ushort[] MATCHLEN2_table { get; set; } = new ushort[KWAJ_MATCHLEN2_TBLSIZE];
        public ushort[] LITLEN_table { get; set; } = new ushort[KWAJ_LITLEN_TBLSIZE];
        public ushort[] OFFSET_table { get; set; } = new ushort[KWAJ_OFFSET_TBLSIZE];
        public ushort[] LITERAL_table { get; set; } = new ushort[KWAJ_LITERAL_TBLSIZE];

        // History window

        public byte[] Window { get; set; } = new byte[LZSS.LZSS_WINDOW_SIZE];
    }
}
