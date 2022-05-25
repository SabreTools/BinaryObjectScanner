/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    /// <summary>
    /// The "not compressed" method decompressor
    /// </summary>
    public partial class None : BaseDecompressState
    {
        public byte[] Buffer { get; set; }

        public int BufferSize { get; set; }
    }
}
