/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System.IO;
using LibMSPackSharp.Compression;

namespace LibMSPackSharp.CHM
{
    public class DecompressState
    {
        /// <summary>
        /// CHM file being decompressed
        /// </summary>
        public Header Header { get; set; }

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
        public LZXDStream State { get; set; }

        /// <summary>
        /// Special I/O code for decompressor
        /// </summary>
        public SystemImpl Sys { get; set; }

        /// <summary>
        /// Input file handle
        /// </summary>
        public FileStream InputFileHandle { get; set; }

        /// <summary>
        /// Output file handle
        /// </summary>
        public FileStream OutputFileHandle { get; set; }
    }
}
