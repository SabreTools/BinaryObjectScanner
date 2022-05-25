/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using LibMSPackSharp.Compression;

namespace LibMSPackSharp.CAB
{
    public class DecompressState : BaseDecompressState
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
        /// Type of compression used by folder
        /// </summary>
        public CompressionType CompressionType { get; set; }

        /// <summary>
        /// Decompressor state
        /// </summary>
        public BaseDecompressState DecompressorState { get; set; }

        /// <summary>
        /// Cabinet where input data comes from
        /// </summary>
        public Cabinet InputCabinet { get; set; }

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
        public byte[] Input { get; set; } = new byte[Constants.CAB_INPUTBUF];

        /// <summary>
        /// Decompressor code
        /// </summary>
        public Error Decompress(object o, long bytes)
        {
            switch (CompressionType & CompressionType.COMPTYPE_MASK)
            {
                case CompressionType.COMPTYPE_NONE:
                    return (o as None)?.Decompress(bytes) ?? Error.MSPACK_ERR_ARGS;

                case CompressionType.COMPTYPE_MSZIP:
                    return (o as MSZIP)?.Decompress(bytes) ?? Error.MSPACK_ERR_ARGS;

                case CompressionType.COMPTYPE_QUANTUM:
                    return (o as QTM)?.Decompress(bytes) ?? Error.MSPACK_ERR_ARGS;

                case CompressionType.COMPTYPE_LZX:
                    return (o as LZX)?.Decompress(bytes) ?? Error.MSPACK_ERR_ARGS;

                default:
                    return Error = Error.MSPACK_ERR_DATAFORMAT;
            }
        }
    }
}
