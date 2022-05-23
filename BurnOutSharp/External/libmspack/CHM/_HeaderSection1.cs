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
    internal class _HeaderSection1
    {
        #region Fields

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0000</remarks>
        public uint Signature { get; private set; }

        /// <summary>
        /// UNKNOWN
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
        /// The size of each PMGL/PMGI chunk, in bytes.
        /// </summary>
        /// <remarks>0x0010</remarks>
        public uint ChunkSize { get; private set; }

        /// <summary>
        /// The "density" of the quick-reference section in PMGL/PMGI chunks.
        /// </summary>
        /// <remarks>0x0014</remarks>
        public uint Density { get; private set; }

        /// <summary>
        /// The depth of the index tree.
        /// </summary>
        /// <remarks>
        /// 0x0018
        /// 
        /// - if 1, there are no PMGI chunks, only PMGL chunks.
        /// - if 2, there is 1 PMGI chunk. All chunk indices point to PMGL chunks.
        /// - if 3, the root PMGI chunk points to secondary PMGI chunks, which in turn point to PMGL chunks.
        /// - and so on...
        /// </remarks>
        public uint Depth { get; private set; }

        /// <summary>
        /// The number of the root PMGI chunk.
        /// </summary>
        /// <remarks>
        /// 0x001C
        /// 
        /// If there is no index in the CHM helpfile, this will be 0xFFFFFFFF.
        /// </remarks>
        public uint IndexRoot { get; private set; }

        /// <summary>
        /// The number of the first PMGL chunk. Usually zero.
        /// Available only in CHM decoder version 2 and above.
        /// </summary>
        /// <remarks>0x0020</remarks>
        public uint FirstPMGL { get; private set; }

        /// <summary>
        /// The number of the last PMGL chunk. Usually num_chunks-1.
        /// Available only in CHM decoder version 2 and above.
        /// </summary>
        /// <remarks>0x0024</remarks>
        public uint LastPMGL { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0028</remarks>
        public uint Unknown2 { get; private set; }

        /// <summary>
        /// The number of PMGL/PMGI directory chunks in this CHM helpfile.
        /// </summary>
        /// <remarks>0x002C</remarks>
        public uint NumChunks { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0030</remarks>
        public uint LanguageID { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0034</remarks>
        public Guid GUID { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0044</remarks>
        public uint Unknown3 { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0048</remarks>
        public uint Unknown4 { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x004C</remarks>
        public uint Unknown5 { get; private set; }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        /// <remarks>0x0050</remarks>
        public uint Unknown6 { get; private set; }

        /// <summary>
        /// Total size of the header section 1 in bytes
        /// </summary>
        public const int Size = 0x0054;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private _HeaderSection1() { }

        /// <summary>
        /// Create a _HeaderSection1 from a byte array, if possible
        /// </summary>
        public static Error Create(byte[] buffer, out _HeaderSection1 headerSection)
        {
            headerSection = null;
            if (buffer == null || buffer.Length < Size)
                return Error.MSPACK_ERR_READ;

            headerSection = new _HeaderSection1();

            headerSection.Signature = BitConverter.ToUInt32(buffer, 0x0000);
            headerSection.Version = BitConverter.ToUInt32(buffer, 0x0004);
            headerSection.HeaderLen = BitConverter.ToUInt32(buffer, 0x0008);
            headerSection.Unknown1 = BitConverter.ToUInt32(buffer, 0x000C);
            headerSection.ChunkSize = BitConverter.ToUInt32(buffer, 0x0010);
            headerSection.Density = BitConverter.ToUInt32(buffer, 0x0014);
            headerSection.Depth = BitConverter.ToUInt32(buffer, 0x0018);
            headerSection.IndexRoot = BitConverter.ToUInt32(buffer, 0x001C);
            headerSection.FirstPMGL = BitConverter.ToUInt32(buffer, 0x0020);
            headerSection.LastPMGL = BitConverter.ToUInt32(buffer, 0x0024);
            headerSection.Unknown2 = BitConverter.ToUInt32(buffer, 0x0028);
            headerSection.NumChunks = BitConverter.ToUInt32(buffer, 0x002C);
            headerSection.LanguageID = BitConverter.ToUInt32(buffer, 0x0030);
            headerSection.GUID = new Guid(new ReadOnlySpan<byte>(buffer, 0x0034, 0x10).ToArray());
            headerSection.Unknown3 = BitConverter.ToUInt32(buffer, 0x0044);
            headerSection.Unknown4 = BitConverter.ToUInt32(buffer, 0x0048);
            headerSection.Unknown5 = BitConverter.ToUInt32(buffer, 0x004C);
            headerSection.Unknown6 = BitConverter.ToUInt32(buffer, 0x0050);

            return Error.MSPACK_ERR_OK;
        }
    }
}
