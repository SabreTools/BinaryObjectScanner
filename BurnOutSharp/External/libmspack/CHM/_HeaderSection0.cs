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
    internal class _HeaderSection0
    {
        #region Fields

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0000</remarks>
        public uint Unknown1 { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0004</remarks>
        public uint Unknown2 { get; private set; }

        /// <summary>
        /// The length of the CHM helpfile, in bytes.
        /// </summary>
        /// <remarks>0x0008</remarks>
        public long FileLength { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0010</remarks>
        public uint Unknown3 { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0014</remarks>
        public uint Unknown4 { get; private set; }

        /// <summary>
        /// Total size of the header section 0 in bytes
        /// </summary>
        public const int Size = 0x0018;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _HeaderSection0() { }

        /// <summary>
        /// Create a _HeaderSection0 from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _HeaderSection0 headerSection)
        {
            headerSection = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            headerSection = new _HeaderSection0();

            headerSection.Unknown1 = BitConverter.ToUInt32(buffer, 0x0000);
            headerSection.Unknown2 = BitConverter.ToUInt32(buffer, 0x0004);
            headerSection.FileLength = BitConverter.ToInt64(buffer, 0x0008);

            // TODO: Is this supposed to be <= 0?
            if (headerSection.FileLength != 0)
                return Error.MSPACK_ERR_DATAFORMAT;

            headerSection.Unknown3 = BitConverter.ToUInt32(buffer, 0x0010);
            headerSection.Unknown4 = BitConverter.ToUInt32(buffer, 0x0014);

            return Error.MSPACK_ERR_OK;
        }
    }
}
