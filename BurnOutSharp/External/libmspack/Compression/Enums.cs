/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    public enum InflateErrorCode
    {
        INF_ERR_OK = 0,

        /// <summary>
        /// Unknown block type
        /// </summary>
        INF_ERR_BLOCKTYPE = -1,

        /// <summary>
        /// Block size complement mismatch
        /// </summary>
        INF_ERR_COMPLEMENT = -2,

        /// <summary>
        /// Error from flush_window callback
        /// </summary>
        INF_ERR_FLUSH = -3,

        /// <summary>
        /// Too many bits in bit buffer
        /// </summary>
        INF_ERR_BITBUF = -4,

        /// <summary>
        /// Too many symbols in blocktype 2 header
        /// </summary>
        INF_ERR_SYMLENS = -5,

        /// <summary>
        /// Failed to build bitlens huffman table
        /// </summary>
        INF_ERR_BITLENTBL = -6,

        /// <summary>
        /// Failed to build literals huffman table
        /// </summary>
        INF_ERR_LITERALTBL = -7,

        /// <summary>
        /// Failed to build distance huffman table
        /// </summary>
        INF_ERR_DISTANCETBL = -8,

        /// <summary>
        /// Bitlen RLE code goes over table size
        /// </summary>
        INF_ERR_BITOVERRUN = -9,

        /// <summary>
        /// Invalid bit-length code
        /// </summary>
        INF_ERR_BADBITLEN = -10,

        /// <summary>
        /// Out-of-range literal code
        /// </summary>
        INF_ERR_LITCODE = -11,

        /// <summary>
        /// Out-of-range distance code
        /// </summary>
        INF_ERR_DISTCODE = -12,

        /// <summary>
        /// Somehow, distance is beyond 32k
        /// </summary>
        INF_ERR_DISTANCE = -13,

        /// <summary>
        /// Out of bits decoding huffman symbol
        /// </summary>
        INF_ERR_HUFFSYM = -14,
    }

    public enum LZSSMode
    {
        LZSS_MODE_EXPAND = 0,

        LZSS_MODE_MSHELP = 1,

        LZSS_MODE_QBASIC = 2,
    }

    public enum LZXBlockType : byte
    {
        LZX_BLOCKTYPE_INVALID0 = 0,

        LZX_BLOCKTYPE_VERBATIM = 1,

        LZX_BLOCKTYPE_ALIGNED = 2,

        LZX_BLOCKTYPE_UNCOMPRESSED = 3,

        LZX_BLOCKTYPE_INVALID4 = 4,

        LZX_BLOCKTYPE_INVALID5 = 5,

        LZX_BLOCKTYPE_INVALID6 = 6,

        LZX_BLOCKTYPE_INVALID7 = 7,
    }
}
