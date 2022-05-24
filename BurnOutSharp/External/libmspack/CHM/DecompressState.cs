/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using LibMSPackSharp.Compression;

namespace LibMSPackSharp.CHM
{
    public class DecompressState : BaseDecompressState
    {
        /// <summary>
        /// CHM file being decompressed
        /// </summary>
        public CHM Header { get; set; }

        /// <summary>
        /// Uncompressed offset within folder
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Offset in input file
        /// </summary>
        public long InOffset { get; set; }

        /// <summary>
        /// LZX decompressor state
        /// </summary>
        public LZX State { get; set; }
    }
}
