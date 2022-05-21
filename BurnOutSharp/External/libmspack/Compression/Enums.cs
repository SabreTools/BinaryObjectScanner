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
