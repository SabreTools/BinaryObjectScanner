/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.CHM
{
    public enum Parameters
    {
        /// <summary>
        /// Sets the "timestamp" of the CHM file
        /// generated. This is not a timestamp, see mschmd_header::timestamp
        /// for a description. If this timestamp is 0, generate() will use its
        /// own algorithm for making a unique ID, based on the lengths and
        /// names of files in the CHM itself. Defaults to 0, any value between
        /// 0 and (2^32)-1 is valid.
        /// </summary>
        MSCHMC_PARAM_TIMESTAMP = 0,

        /// <summary>
        /// Sets the "language" of the CHM file
        /// generated.  This is not the language used in the CHM file, but the
        /// language setting of the user who ran the HTMLHelp compiler. It
        /// defaults to 0x0409. The valid range is between 0x0000 and 0x7F7F.
        /// </summary>
        MSCHMC_PARAM_LANGUAGE = 1,

        /// <summary>
        /// Sets the size of the LZX history window,
        /// which is also the interval at which the compressed data stream can be
        /// randomly accessed. The value is not a size in bytes, but a power of
        /// two. The default value is 16 (which makes the window 2^16 bytes, or
        /// 64 kilobytes), the valid range is from 15 (32 kilobytes) to 21 (2
        /// megabytes).
        /// </summary>
        MSCHMC_PARAM_LZXWINDOW = 2,

        /// <summary>
        /// Sets the "density" of quick reference
        /// entries stored at the end of directory listing chunk. Each chunk is
        /// 4096 bytes in size, and contains as many file entries as there is
        /// room for. At the other end of the chunk, a list of "quick reference"
        /// pointers is included. The offset of every 'N'th file entry is given a
        /// quick reference, where N = (2 ^ density) + 1.The default density is
        /// 2. The smallest density is 0 (N = 2), the maximum is 10 (N = 1025). As
        /// each file entry requires at least 5 bytes, the maximum number of
        /// entries in a single chunk is roughly 800, so the maximum value 10
        /// can be used to indicate there are no quickrefs at all.
        /// </summary>
        MSCHMC_PARAM_DENSITY = 3,

        /// <summary>
        /// Sets whether or not to include quick lookup
        /// index chunk(s), in addition to normal directory listing chunks. A
        /// value of zero means no index chunks will be created, a non-zero value
        /// means index chunks will be created. The default is zero, "don't
        /// create an index".
        /// </summary>
        MSCHMC_PARAM_INDEX = 4,
    }

    public enum SectionType
    {
        /// <summary>
        /// end of CHM file list
        /// </summary>
        MSCHMC_ENDLIST = 0,

        /// <summary>
        /// this file is in the Uncompressed section
        /// </summary>
        MSCHMC_UNCOMP = 1,

        /// <summary>
        /// this file is in the MSCompressed section
        /// </summary>
        MSCHMC_MSCOMP = 2,
    }
}
