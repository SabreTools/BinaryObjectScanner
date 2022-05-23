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
    internal class _HeaderSectionTable
    {
        #region Regular Table

        /// <summary>
        /// Header section 0 offset
        /// </summary>
        /// <remarks>0x0000</remarks>
        public long OffsetHS0 { get; private set; }

        /// <summary>
        /// Header section 0 length
        /// </summary>
        /// <remarks>0x0008</remarks>
        public long LengthHS0 { get; private set; }

        /// <summary>
        /// Header section 1 offset
        /// The file offset of the first PMGL/PMGI directory chunk.
        /// </summary>
        /// <remarks>
        /// 0x0010
        /// 
        /// This is internally settable because it has to be corrected in some cases
        /// </remarks>
        public long OffsetHS1 { get; internal set; }

        /// <summary>
        /// Header section 1 length
        /// </summary>
        /// <remarks>0x0018</remarks>
        public long LengthHS1 { get; private set; }

        /// <summary>
        /// Total size of the version 1 and 2 header section table in bytes
        /// </summary>
        public const int Size = 0x0020;

        #endregion

        #region Version 3 Table

        /// <summary>
        /// Header section 0 offset correct
        /// </summary>
        /// <remarks>0x0020</remarks>
        public long OffsetCS0 { get; private set; }

        /// <summary>
        /// Total size of the version 3 header section table in bytes
        /// </summary>
        public const int V3Size = 0x0028;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _HeaderSectionTable() { }

        /// <summary>
        /// Create a _HeaderSectionTable from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _HeaderSectionTable headerSectionTable)
        {
            headerSectionTable = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            headerSectionTable = new _HeaderSectionTable();

            headerSectionTable.OffsetHS0 = BitConverter.ToInt64(buffer, 0x0000);
            headerSectionTable.LengthHS0 = BitConverter.ToInt64(buffer, 0x0008);
            headerSectionTable.OffsetHS1 = BitConverter.ToInt64(buffer, 0x0010);
            headerSectionTable.LengthHS1 = BitConverter.ToInt64(buffer, 0x0018);

            if (buffer.Length >= V3Size)
            {
                headerSectionTable.OffsetCS0 = BitConverter.ToInt64(buffer, 0x0020);
            }

            // OffsetCS0 does not exist in version 1 or 2 CHM files.
            // The offset will be corrected later, once HS1 is read.
            // TODO: Are these supposed to be == 0?
            if (headerSectionTable.OffsetHS0 != 0 || headerSectionTable.OffsetHS1 != 0 || headerSectionTable.OffsetCS0 != 0)
                return Error.MSPACK_ERR_DATAFORMAT;

            return Error.MSPACK_ERR_OK;
        }
    }
}
