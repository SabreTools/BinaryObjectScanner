/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;

namespace LibMSPackSharp.CAB
{
    public class DecompressState
    {
        /// <summary>
        /// Current folder we're extracting from
        /// </summary>
        public Folder Folder { get; set; }

        /// <summary>
        /// Current folder split we're in
        /// </summary>
        public FolderData Data { get; set; }

        /// <summary>
        /// Uncompressed offset within folder
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Which block are we decompressing?
        /// </summary>
        public uint Block { get; set; }

        /// <summary>
        /// Cumulative sum of block output sizes
        /// </summary>
        public long Outlen { get; set; }

        /// <summary>
        /// Special I/O code for decompressor
        /// </summary>
        public SystemImpl Sys { get; set; }

        /// <summary>
        /// Type of compression used by folder
        /// </summary>
        public CompressionType CompressionType { get; set; }

        /// <summary>
        /// Decompressor code
        /// </summary>
        public Func<object, long, Error> Decompress { get; set; }

        /// <summary>
        /// Decompressor state
        /// </summary>
        public object DecompressorState { get; set; }

        /// <summary>
        /// Cabinet where input data comes from
        /// </summary>
        public Cabinet InputCabinet { get; set; }

        /// <summary>
        /// Input file handle
        /// </summary>
        public object InputFileHandle { get; set; }

        /// <summary>
        /// Output file handle
        /// </summary>
        public object OutputFileHandle { get; set; }

        /// <summary>
        /// Input data consumed
        /// </summary>
        public int InputPointer { get; set; }

        /// <summary>
        /// Input data end
        /// </summary>
        public int InputEnd { get; set; }

        /// <summary>
        /// One input block of data
        /// </summary>
        public byte[] Input { get; set; } = new byte[Implementation.CAB_INPUTBUF];
    }
}
