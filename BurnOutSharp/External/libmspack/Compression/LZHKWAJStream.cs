/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using static LibMSPackSharp.Constants;
using static LibMSPackSharp.Compression.Constants;

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

        public byte[] Window { get; set; } = new byte[LZSS_WINDOW_SIZE];

        #endregion

        #region Specialty Methods

        /* In the KWAJ LZH format, there is no special 'eof' marker, it just
         * ends. Depending on how many bits are left in the final byte when
         * the stream ends, that might be enough to start another literal or
         * match. The only easy way to detect that we've come to an end is to
         * guard all bit-reading. We allow fake bits to be read once we reach
         * the end of the stream, but we check if we then consumed any of
         * those fake bits, after doing the READ_BITS / READ_HUFFSYM. This
         * isn't how the default readbits.h read_input() works (it simply lets
         * 2 fake bytes in then stops), so we implement our own.
         */

        /// <summary>
        /// Safely read bits from the buffer
        /// </summary>
        public long READ_BITS_SAFE(int nbits, BufferState state)
        {
            long val = READ_BITS_MSB(nbits, state);
            if (EndOfInput != 0 && BufferState.BitsLeft < EndOfInput)
                Error = Error.MSPACK_ERR_NOMEMORY;
            else
                Error = Error.MSPACK_ERR_OK;

            return val;
        }

        /// <summary>
        /// Safely read a symbol from a Huffman tree
        /// </summary>
        public long READ_HUFFSYM_SAFE(ushort[] table, byte[] lengths, int tablebits, int maxsymbols, BufferState state)
        {
            long val = READ_HUFFSYM_MSB(table, lengths, tablebits, maxsymbols, state);
            if (EndOfInput != 0 && BufferState.BitsLeft < EndOfInput)
                Error = Error.MSPACK_ERR_NOMEMORY;
            else
                Error = Error.MSPACK_ERR_OK;

            return val;
        }

        /// <summary>
        /// Write a single byte to the output stream
        /// </summary>
        public void WRITE_BYTE(int pos)
        {
            if (System.Write(OutputFileHandle, Window, pos, 1) != 1)
                Error = Error.MSPACK_ERR_WRITE;
        }

        #endregion

        /// <inheritdoc/>
        public override Error HUFF_ERROR() => Error.MSPACK_ERR_DATAFORMAT;

        /// <inheritdoc/>
        public override void READ_BYTES(BufferState state)
        {
            if (state.InputPointer >= state.InputEnd)
            {
                ReadInput();
                if (Error != Error.MSPACK_ERR_OK)
                    return;

                state.InputPointer = BufferState.InputPointer;
                state.InputEnd = BufferState.InputEnd;
            }

            INJECT_BITS_MSB(InputBuffer[state.InputPointer++], 8, state);
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
                    BufferState.InputEnd = 8;
                    InputBuffer[0] = 0;
                    read = 1;
                }
            }

            // Update InputPointer and InputLength
            BufferState.InputPointer = 0;
            BufferState.InputEnd = read;
        }
    }
}
