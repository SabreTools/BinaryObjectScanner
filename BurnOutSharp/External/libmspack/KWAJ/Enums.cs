/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;

namespace LibMSPackSharp.KWAJ
{
    public enum CompressionType
    {
        /// <summary>
        /// no compression.
        /// </summary>
        MSKWAJ_COMP_NONE = 0,

        /// <summary>
        /// no compression, 0xFF XOR "encryption".
        /// </summary>
        MSKWAJ_COMP_XOR = 1,

        /// <summary>
        /// LZSS (same method as SZDD)
        /// </summary>
        MSKWAJ_COMP_SZDD = 2,

        /// <summary>
        /// LZ+Huffman compression
        /// </summary>
        MSKWAJ_COMP_LZH = 3,

        /// <summary>
        /// MSZIP
        /// </summary>
        MSKWAJ_COMP_MSZIP = 4,
    }

    [Flags]
    public enum OptionalHeaderFlag : ushort
    {
        /// <summary>
        /// decompressed file length is included
        /// </summary>
        MSKWAJ_HDR_HASLENGTH = 0x01,

        /// <summary>
        /// unknown 2-byte structure is included
        /// </summary>
        MSKWAJ_HDR_HASUNKNOWN1 = 0x02,

        /// <summary>
        /// unknown multi-sized structure is included
        /// </summary>
        MSKWAJ_HDR_HASUNKNOWN2 = 0x04,

        /// <summary>
        /// file name (no extension) is included
        /// </summary>
        MSKWAJ_HDR_HASFILENAME = 0x08,

        /// <summary>
        /// file extension is included
        /// </summary>
        MSKWAJ_HDR_HASFILEEXT = 0x10,

        /// <summary>
        /// extra text is included
        /// </summary>
        MSKWAJ_HDR_HASEXTRATEXT = 0x20,
    }

    public enum Parameters
    {
        /// <summary>
        /// Compression type
        /// </summary>
        MSKWAJC_PARAM_COMP_TYPE = 0,

        /// <summary>
        /// Include the length of the uncompressed file in the header?
        /// </summary>
        MSKWAJC_PARAM_INCLUDE_LENGTH = 1,
    }
}
