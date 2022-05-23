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
    internal class _PMGHeader
    {
        #region Fields

        /// <summary>
        /// "PMGL" or "PMGI"
        /// </summary>
        /// <remarks>0x0000</remarks>
        public uint Signature { get; private set; }

        /// <summary>
        /// Quick reference size
        /// </summary>
        /// <remarks>0x0004</remarks>
        public uint QuickRefSize { get; private set; }

        /// <summary>
        /// Number of entries in a PMGI chunk
        /// </summary>
        /// <remarks>
        /// 0x0008
        /// 
        /// Unused in PMGL
        /// </remarks>
        public uint PMGIEntries { get; private set; }

        /// <summary>
        /// Previous chunk ID
        /// </summary>
        /// <remarks>
        /// 0x000C
        /// 
        /// Does not exist in PMGI
        /// </remarks>
        public uint PrevChunk { get; private set; }

        /// <summary>
        /// Next chunk ID
        /// </summary>
        /// <remarks>
        /// 0x0010
        /// 
        /// Does not exist in PMGI
        /// </remarks>
        public uint NextChunk { get; private set; }

        /// <summary>
        /// Number of entries in a PMGL chunk
        /// </summary>
        /// <remarks>
        /// 0x0014
        /// 
        /// Does not exist in PMGI
        /// </remarks>
        public uint PMGLEntries { get; private set; }

        /// <summary>
        /// Total size of the PMGI header in bytes
        /// </summary>
        public const int PMGISize = 0x000C;

        /// <summary>
        /// Total size of the PMGL header in bytes
        /// </summary>
        public const int PMGLSize = 0x0014;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _PMGHeader() { }

        /// <summary>
        /// Create a _PMGHeader from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _PMGHeader header)
        {
            header = null;
            if (buffer == null || buffer.Length < PMGISize)
                return Error.MSPACK_ERR_READ;

            header = new _PMGHeader();

            header.Signature = BitConverter.ToUInt32(buffer, 0x0000);
            header.QuickRefSize = BitConverter.ToUInt32(buffer, 0x0004);
            header.PMGIEntries = BitConverter.ToUInt32(buffer, 0x0008);

            if (buffer.Length >= PMGLSize)
            {
                header.PrevChunk = BitConverter.ToUInt32(buffer, 0x000C);
                header.NextChunk = BitConverter.ToUInt32(buffer, 0x0010);
                header.PMGLEntries = BitConverter.ToUInt32(buffer, 0x0014);
            }

            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Determines if a PMG chunk is PMGL or PMGI
        /// </summary>
        /// <returns>True for PMGL and false of PMGI</returns>
        public bool IsPMGL() => Signature == 0x4C474D50;
    }
}
