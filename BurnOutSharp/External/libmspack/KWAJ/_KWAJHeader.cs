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
    internal class _KWAJHeader
    {
        #region Fields

        /// <summary>
        /// "KWAA"
        /// </summary>
        /// <remarks>0x00</remarks>
        public uint Signature1 { get; private set; }

        /// <summary>
        /// Signature extension
        /// </summary>
        /// <remarks>0x04</remarks>
        public uint Signature2 { get; private set; }

        /// <summary>
        /// The compression type
        /// </summary>
        /// <remarks>0x08</remarks>
        public CompressionType CompressionType { get; private set; }

        /// <summary>
        /// The offset in the file where the compressed data stream begins
        /// </summary>
        /// <remarks>0x0a</remarks>
        public ushort DataOffset { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public OptionalHeaderFlag Flags { get; private set; }

        /// <summary>
        /// Total size of the KWAJ header in bytes
        /// </summary>
        public const int Size = 0x0e;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _KWAJHeader() { }

        /// <summary>
        /// Create a _KWAJHeader from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _KWAJHeader header)
        {
            header = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            header = new _KWAJHeader();

            header.Signature1 = BitConverter.ToUInt32(buffer, 0x00);
            if (header.Signature1 != 0x4A41574B)
                return Error.MSPACK_ERR_SIGNATURE;

            header.Signature2 = BitConverter.ToUInt32(buffer, 0x04);
            if (header.Signature1 != 0xD127F088)
                return Error.MSPACK_ERR_SIGNATURE;

            header.CompressionType = (CompressionType)BitConverter.ToUInt16(buffer, 0x08);
            header.DataOffset = BitConverter.ToUInt16(buffer, 0x0a);
            header.Flags = (OptionalHeaderFlag)BitConverter.ToUInt16(buffer, 0x0c);

            return Error.MSPACK_ERR_OK;
        }
    }
}
