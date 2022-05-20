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
    internal class _DataBlockHeader
    {
        #region Fields

        /// <summary>
        /// Block CRC32 checksum
        /// </summary>
        /// <remarks>0x00</remarks>
        public uint CheckSum { get; private set; }

        /// <summary>
        /// Compressed size of the data block
        /// </summary>
        /// <remarks>0x04</remarks>
        public ushort CompressedSize { get; private set; }

        /// <summary>
        /// Uncompressed size of the data block
        /// </summary>
        /// <remarks>0x06</remarks>
        public ushort UncompressedSize { get; private set; }

        /// <summary>
        /// Size of the Data Block header in bytes
        /// </summary>
        public const int Size = 0x08;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _DataBlockHeader() { }

        /// <summary>
        /// Create a _DataBlockHeader from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _DataBlockHeader header)
        {
            header = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            header = new _DataBlockHeader();

            header.CheckSum = BitConverter.ToUInt32(buffer, 0x00);
            header.CompressedSize = BitConverter.ToUInt16(buffer, 0x04);
            header.UncompressedSize = BitConverter.ToUInt16(buffer, 0x06);

            return Error.MSPACK_ERR_OK;
        }
    }
}
