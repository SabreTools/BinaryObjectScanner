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

namespace LibMSPackSharp.Compression
{
    public abstract partial class CompressionStream : BaseDecompressState
    {
        public byte[] InputBuffer { get; set; }

        public uint InputBufferSize { get; set; }

        /// <summary>
        /// i_ptr
        /// </summary>
        public int InputPointer { get; set; }

        /// <summary>
        /// i_end
        /// </summary>
        public int InputEnd { get; set; }

        /// <summary>
        /// o_ptr
        /// </summary>
        public int OutputPointer { get; set; }

        /// <summary>
        /// o_end
        /// </summary>
        public int OutputEnd { get; set; }

        /// <summary>
        /// bit_buffer
        /// </summary>
        public uint BitBuffer { get; set; }

        /// <summary>
        /// bits_left
        /// </summary>
        public int BitsLeft { get; set; }

        /// <summary>
        /// Have we reached the end of input?
        /// </summary>
        public int EndOfInput { get; set; }
    }
}
