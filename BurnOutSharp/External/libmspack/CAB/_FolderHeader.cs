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
    internal class _FolderHeader
    {
        #region Fields

        /// <summary>
        /// Cabinet offset of first datablock
        /// </summary>
        /// <remarks>0x00</remarks>
        public uint DataOffset { get; internal set; }

        /// <summary>
        /// The total number of data blocks used by this folder. This includes
        /// data blocks present in other files, if this folder spans more than
        /// one cabinet.
        /// </summary>
        /// <remarks>0x04</remarks>
        public ushort NumBlocks { get; internal set; }

        /// <summary>
        /// The compression format used by this folder.
        /// 
        /// The macro MSCABD_COMP_METHOD() should be used on this field to get
        /// the algorithm used. The macro MSCABD_COMP_LEVEL() should be used to get
        /// the "compression level".
        /// </summary>
        /// <remarks>0x06</remarks>
        public CompressionType CompType { get; private set; }

        /// <summary>
        /// Size of the Folder header in bytes
        /// </summary>
        public const int Size = 0x08;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _FolderHeader() { }

        /// <summary>
        /// Create a _Folder from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _FolderHeader header)
        {
            header = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            header = new _FolderHeader();

            header.DataOffset = BitConverter.ToUInt32(buffer, 0x00);
            header.NumBlocks = BitConverter.ToUInt16(buffer, 0x04);
            header.CompType = (CompressionType)BitConverter.ToUInt16(buffer, 0x06);

            return Error.MSPACK_ERR_OK;
        }
    }
}
