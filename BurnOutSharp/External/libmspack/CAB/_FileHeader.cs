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
    internal class _FileHeader
    {
        #region Fields

        /// <summary>
        /// The uncompressed length of the file, in bytes.
        /// </summary>
        /// <remarks>0x00</remarks>
        public uint UncompressedSize { get; private set; }

        /// <summary>
        /// The uncompressed offset of this file in its folder.
        /// </summary>
        /// <remarks>0x04</remarks>
        public uint FolderOffset { get; private set; }

        /// <summary>
        /// Internal index of the folder
        /// </summary>
        /// <remarks>0x08</remarks>
        public FileFlags FolderIndex { get; internal set; }

        /// <summary>
        /// File's last modified date, day field.
        /// </summary>
        /// <remarks>0x0A</remarks>
        public byte LastModifiedDateDay { get; set; }

        /// <summary>
        /// File's last modified date, month field.
        /// </summary>
        /// <remarks>0x0A</remarks>
        public byte LastModifiedDateMonth { get; set; }

        /// <summary>
        /// File's last modified date, year field.
        /// </summary>
        /// <remarks>0x0A</remarks>
        public int LastModifiedDateYear { get; set; }

        /// <summary>
        /// File's last modified time, hour field.
        /// </summary>
        /// <remarks>0x0C</remarks>
        public byte LastModifiedTimeHour { get; set; }

        /// <summary>
        /// File's last modified time, minute field.
        /// </summary>
        /// <remarks>0x0C</remarks>
        public byte LastModifiedTimeMinute { get; set; }

        /// <summary>
        /// File's last modified time, second field.
        /// </summary>
        /// <remarks>0x0C</remarks>
        public byte LastModifiedTimeSecond { get; set; }

        /// <summary>
        /// File attributes.
        /// </summary>
        /// <remarks>0x0E</remarks>
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// Size of the File header in bytes
        /// </summary>
        public const int Size = 0x10;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _FileHeader() { }

        /// <summary>
        /// Create a _FileHeader from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _FileHeader header)
        {
            header = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            header = new _FileHeader();

            header.UncompressedSize = BitConverter.ToUInt32(buffer, 0x00);
            header.FolderOffset = BitConverter.ToUInt32(buffer, 0x04);
            header.FolderIndex = (FileFlags)BitConverter.ToUInt16(buffer, 0x08);

            // Get date
            ushort x = BitConverter.ToUInt16(buffer, 0x0A);
            header.LastModifiedDateDay = (byte)(x & 0x1F);
            header.LastModifiedDateMonth = (byte)((x >> 5) & 0xF);
            header.LastModifiedDateYear = (x >> 9) + 1980;

            // Get time
            x = BitConverter.ToUInt16(buffer, 0x0C);
            header.LastModifiedTimeHour = (byte)(x >> 11);
            header.LastModifiedTimeMinute = (byte)((x >> 5) & 0x3F);
            header.LastModifiedTimeSecond = (byte)((x << 1) & 0x3E);

            header.Attributes = (FileAttributes)BitConverter.ToUInt16(buffer, 0x0E);

            return Error.MSPACK_ERR_OK;
        }
    }
}
