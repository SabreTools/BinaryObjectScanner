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
    public class DataBlockHeader
    {
        #region Fields

        /// <summary>
        /// CRC32 checksum of the data
        /// </summary>
        public uint Checksum { get; private set; }

        /// <summary>
        /// Compressed size of the data
        /// </summary>
        public ushort CompressedSize { get; private set; }

        /// <summary>
        /// Uncompressed size of the data
        /// </summary>
        public ushort UncompressedSize { get; private set; }

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private DataBlockHeader() { }

        /// <summary>
        /// Constructor
        /// </summary>
        public static Error Create(byte[] data, out DataBlockHeader header)
        {
            header = null;
            if (data == null || data.Length < 0x08)
                return Error.MSPACK_ERR_READ;

            header = new DataBlockHeader();

            header.Checksum = BitConverter.ToUInt32(data, 0x00);
            header.CompressedSize = BitConverter.ToUInt16(data, 0x04);
            header.UncompressedSize = BitConverter.ToUInt16(data, 0x06);

            return Error.MSPACK_ERR_OK;
        }
    }
}
