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
    internal class _CHMHeader
    {
        #region Fields

        /// <summary>
        /// "ITSF"
        /// </summary>
        /// <remarks>0x0000</remarks>
        public uint Signature { get; private set; }

        /// <summary>
        /// The version of the CHM file format used in this file.
        /// </summary>
        /// <remarks>0x0004</remarks>
        public uint Version { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0008</remarks>
        public uint HeaderLen { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x000C</remarks>
        public uint Unknown1 { get; private set; }

        /// <summary>
        /// The "timestamp" of the CHM helpfile.
        /// </summary>
        /// <remarks>
        /// 0x0010
        /// 
        /// It is the lower 32 bits of a 64-bit value representing the number of
        /// centiseconds since 1601-01-01 00:00:00 UTC, plus 42. It is not useful
        /// as a timestamp, but it is useful as a semi-unique ID.
        /// </remarks>
        public uint Timestamp { get; private set; }

        /// <summary>
        /// The default Language and Country ID (LCID) of the user who ran the
        /// HTMLHelp Compiler. This is not the language of the CHM file itself.
        /// </summary>
        /// <remarks>0x0014</remarks>
        public uint LanguageID { get; private set; }

        /// <summary>
        /// Header GUID 1
        /// </summary>
        /// <remarks>0x0018</remarks>
        public Guid GUID1 { get; private set; }

        /// <summary>
        /// Header GUID 2
        /// </summary>
        /// <remarks>0x0028</remarks>
        public Guid GUID2 { get; private set; }

        /// <summary>
        /// Total size of the CHM header in bytes
        /// </summary>
        public const int Size = 0x0038;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _CHMHeader() { }

        /// <summary>
        /// Create a _CHMHeader from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _CHMHeader header)
        {
            header = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            header = new _CHMHeader();

            header.Signature = BitConverter.ToUInt32(buffer, 0x0000);
            if (header.Signature != 0x46535449)
                return Error.MSPACK_ERR_SIGNATURE;

            // Expect version less than or equal to 3, but don't validate
            header.Version = BitConverter.ToUInt32(buffer, 0x0004);
            header.HeaderLen = BitConverter.ToUInt32(buffer, 0x0008);
            header.Unknown1 = BitConverter.ToUInt32(buffer, 0x000C);
            header.Timestamp = BitConverter.ToUInt32(buffer, 0x0010);
            header.LanguageID = BitConverter.ToUInt32(buffer, 0x0014);

            header.GUID1 = new Guid(new ReadOnlySpan<byte>(buffer, 0x0018, 0x10).ToArray());
            if (header.GUID1 != Guid.Parse("7C01FD10-7BAA-11D0-9E0C-00A0-C922-E6EC"))
                return Error.MSPACK_ERR_SIGNATURE;
            
            header.GUID2 = new Guid(new ReadOnlySpan<byte>(buffer, 0x0028, 0x10).ToArray());
            if (header.GUID2 != Guid.Parse("7C01FD11-7BAA-11D0-9E0C-00A0-C922-E6EC"))
                return Error.MSPACK_ERR_SIGNATURE;

            return Error.MSPACK_ERR_OK;
        }
    }
}
