/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;

namespace LibMSPackSharp.CHM
{
    internal class _LZXResetTable
    {
        #region Fields

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0000</remarks>
        public uint Unknown1 { get; private set; }

        /// <summary>
        /// Number of entries in the table
        /// </summary>
        /// <remarks>0x0004</remarks>
        public uint NumEntries { get; private set; }

        /// <summary>
        /// Size of each entry
        /// </summary>
        /// <remarks>0x0008</remarks>
        public uint EntrySize { get; private set; }

        /// <summary>
        /// Table offset
        /// </summary>
        /// <remarks>0x000C</remarks>
        public uint TableOffset { get; private set; }

        /// <summary>
        /// Uncompressed length
        /// </summary>
        /// <remarks>0x0010</remarks>
        public long UncompressedLength { get; private set; }

        /// <summary>
        /// Compressed length
        /// </summary>
        /// <remarks>0x0018</remarks>
        public long CompressedLength { get; private set; }

        /// <summary>
        /// Frame length
        /// </summary>
        /// <remarks>0x0020</remarks>
        public long FrameLength { get; private set; }

        /// <summary>
        /// Total size of the LZX reset table in bytes
        /// </summary>
        public const int Size = 0x0028;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _LZXResetTable() { }

        /// <summary>
        /// Create a _LZXControlData from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _LZXResetTable resetTable)
        {
            resetTable = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            resetTable = new _LZXResetTable();

            resetTable.Unknown1 = BitConverter.ToUInt32(buffer, 0x0000);
            resetTable.NumEntries = BitConverter.ToUInt32(buffer, 0x0004);
            resetTable.EntrySize = BitConverter.ToUInt32(buffer, 0x0008);
            resetTable.TableOffset = BitConverter.ToUInt32(buffer, 0x000C);
            resetTable.UncompressedLength = BitConverter.ToInt64(buffer, 0x0010);
            resetTable.CompressedLength = BitConverter.ToInt64(buffer, 0x0018);
            resetTable.FrameLength = BitConverter.ToInt64(buffer, 0x0020);

            return Error.MSPACK_ERR_OK;
        }
    }
}
