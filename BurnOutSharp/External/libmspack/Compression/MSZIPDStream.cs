/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * The deflate method was created by Phil Katz. MSZIP is equivalent to the
 * deflate method.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;

namespace LibMSPackSharp.Compression
{
    public class MSZIPDStream : CompressionStream
    {
        #region Fields

        /// <summary>
        /// 32kb history window
        /// </summary>
        public byte[] Window { get; set; } = new byte[MSZIP.MSZIP_FRAME_SIZE];

        /// <summary>
        /// Offset within window
        /// </summary>
        public uint WindowPosition { get; set; }

        /// <summary>
        /// inflate() will call this whenever the window should be emptied.
        /// </summary>
        public Func<MSZIPDStream, uint, Error> FlushWindow;

        public bool RepairMode { get; set; }

        public int BytesOutput { get; set; }

        #region Huffman code lengths

        public byte[] LITERAL_len { get; set; } = new byte[MSZIP.MSZIP_LITERAL_MAXSYMBOLS];
        public byte[] DISTANCE_len { get; set; } = new byte[MSZIP.MSZIP_DISTANCE_MAXSYMBOLS];

        #endregion

        #region Huffman decoding tables

        public ushort[] LITERAL_table { get; set; } = new ushort[MSZIP.MSZIP_LITERAL_TABLESIZE];
        public ushort[] DISTANCE_table { get; set; } = new ushort[MSZIP.MSZIP_DISTANCE_TABLESIZE];

        #endregion

        #endregion

        /// <inheritdoc/>
        public override Error HUFF_ERROR() => Error.INF_ERR_HUFFSYM;

        /// <inheritdoc/>
        public override void READ_BYTES(ref int i_ptr, ref int i_end, ref uint bit_buffer, ref int bits_left)
        {
            READ_IF_NEEDED(ref i_ptr, ref i_end);
            if (Error != Error.MSPACK_ERR_OK)
                return;

            INJECT_BITS_LSB(InputBuffer[i_ptr++], 8, ref bit_buffer, ref bits_left);
        }
    }
}
