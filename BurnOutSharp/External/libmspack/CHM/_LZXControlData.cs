/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;
using static LibMSPackSharp.Compression.Constants;

namespace LibMSPackSharp.CHM
{
    internal class _LZXControlData
    {
        #region Fields

        /// <summary>
        /// Length of the control data
        /// </summary>
        /// <remarks>0x0000</remarks>
        public uint Length { get; private set; }

        /// <summary>
        /// "LZXC"
        /// </summary>
        /// <remarks>0x0004</remarks>
        public uint Signature { get; private set; }

        /// <summary>
        /// Control data version
        /// </summary>
        /// <remarks>0x0008</remarks>
        public uint Version { get; private set; }

        /// <summary>
        /// Reset interval
        /// </summary>
        /// <remarks>0x000C</remarks>
        public uint ResetInterval { get; private set; }

        /// <summary>
        /// Window size
        /// </summary>
        /// <remarks>0x0010</remarks>
        public uint WindowSize { get; private set; }

        /// <summary>
        /// Cache size
        /// </summary>
        /// <remarks>0x0014</remarks>
        public uint CacheSize { get; private set; }

        /// <summary>
        /// Cache size
        /// </summary>
        /// <remarks>0x0018</remarks>
        public uint Unknown1 { get; private set; }

        /// <summary>
        /// Total size of the LZX control data in bytes
        /// </summary>
        public const int Size = 0x001C;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _LZXControlData() { }

        /// <summary>
        /// Create a _LZXControlData from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _LZXControlData controlData)
        {
            controlData = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            controlData = new _LZXControlData();

            controlData.Length = BitConverter.ToUInt32(buffer, 0x0000);
            controlData.Signature = BitConverter.ToUInt32(buffer, 0x0004);
            if (controlData.Signature != 0x43585A4C)
                return Error.MSPACK_ERR_SIGNATURE;

            controlData.Version = BitConverter.ToUInt32(buffer, 0x0008);
            switch (controlData.Version)
            {
                case 1:
                    controlData.ResetInterval = BitConverter.ToUInt32(buffer, 0x000C);
                    controlData.WindowSize = BitConverter.ToUInt32(buffer, 0x0010);
                    break;
                case 2:
                    controlData.ResetInterval = BitConverter.ToUInt32(buffer, 0x000C) * LZX_FRAME_SIZE;
                    controlData.WindowSize = BitConverter.ToUInt32(buffer, 0x0010) * LZX_FRAME_SIZE;
                    break;
                default:
                    return Error.MSPACK_ERR_DATAFORMAT;
            }

            controlData.CacheSize = BitConverter.ToUInt32(buffer, 0x0014);
            controlData.Unknown1 = BitConverter.ToUInt32(buffer, 0x0018);

            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Get the number of bits in the window based on the window size
        /// </summary>
        /// <param name="windowBits">Window </param>
        /// <returns>An error code or MSPACK_ERR_OK if all is good</returns>
        public Error GetWindowBits(out int windowBits)
        {
            switch (WindowSize)
            {
                case 0x008000:
                    windowBits = 15;
                    break;

                case 0x010000:
                    windowBits = 16;
                    break;

                case 0x020000:
                    windowBits = 17;
                    break;

                case 0x040000:
                    windowBits = 18;
                    break;

                case 0x080000:
                    windowBits = 19;
                    break;

                case 0x100000:
                    windowBits = 20;
                    break;

                case 0x200000:
                    windowBits = 21;
                    break;

                default:
                    windowBits = -1;
                    return Error.MSPACK_ERR_DATAFORMAT;
            }

            return Error.MSPACK_ERR_OK;
        }
    }
}
