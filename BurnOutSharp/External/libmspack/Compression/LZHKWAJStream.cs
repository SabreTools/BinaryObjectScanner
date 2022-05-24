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
        #region Fields

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

        #endregion

        /// <inheritdoc/>
        public override void READ_BYTES(ref int i_ptr, ref int i_end, ref uint bit_buffer, ref int bits_left)
        {
            if (i_ptr >= i_end)
            {
                ReadInput();
                if (Error != Error.MSPACK_ERR_OK)
                    return;

                i_ptr = InputPointer;
                i_end = InputEnd;
            }

            INJECT_BITS_MSB(InputBuffer[i_ptr++], 8, ref bit_buffer, ref bits_left);
        }

        /// <inheritdoc/>
        protected override void ReadInput()
        {
            int read;
            if (EndOfInput != 0)
            {
                EndOfInput += 8;
                InputBuffer[0] = 0;
                read = 1;
            }
            else
            {
                read = System.Read(InputFileHandle, InputBuffer, 0, KWAJ_INPUT_SIZE);
                if (read < 0)
                {
                    Error = Error.MSPACK_ERR_READ;
                    return;
                }

                if (read == 0)
                {
                    InputEnd = 8;
                    InputBuffer[0] = 0;
                    read = 1;
                }
            }

            // Update InputPointer and InputLength
            InputPointer = 0;
            InputEnd = read;
        }
    }
}
