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
    [Flags]
    public enum CompressionType : ushort
    {
        COMPTYPE_MASK = 0x000f,

        COMPTYPE_NONE = 0x0000,
        COMPTYPE_MSZIP = 0x0001,
        COMPTYPE_QUANTUM = 0x0002,
        COMPTYPE_LZX = 0x0003,
    }

    [Flags]
    public enum FileAttributes : byte
    {
        /// <summary>
        /// Indicates the file is write protected.
        /// </summary>
        MSCAB_ATTRIB_RDONLY = 0x01,

        /// <summary>
        /// Indicates the file is hidden.
        /// </summary>
        MSCAB_ATTRIB_HIDDEN = 0x02,

        /// <summary>
        /// Indicates the file is a operating system file.
        /// </summary>
        MSCAB_ATTRIB_SYSTEM = 0x04,

        /// <summary>
        /// Indicates the file is "archived".
        /// </summary>
        MSCAB_ATTRIB_ARCH = 0x20,

        /// <summary>
        /// Indicates the file is an executable program.
        /// </summary>
        MSCAB_ATTRIB_EXEC = 0x40,

        /// <summary>
        /// Indicates the filename is in UTF8 format rather than ISO-8859-1.
        /// </summary>
        MSCAB_ATTRIB_UTF_NAME = 0x80,
    }

    [Flags]
    public enum FileFlags : ushort
    {
        CONTINUED_FROM_PREV = 0xFFFD,
        CONTINUED_TO_NEXT = 0xFFFE,
        CONTINUED_PREV_AND_NEXT = 0xFFFF,
    }

    [Flags]
    public enum HeaderFlags : ushort
    {
        /// <summary>
        /// Indicates the cabinet is part of a cabinet set, and has a predecessor cabinet.
        /// </summary>
        MSCAB_HDR_PREVCAB = 0x0001,

        /// <summary>
        /// Indicates the cabinet is part of a cabinet set, and has a successor cabinet.
        /// </summary>
        MSCAB_HDR_NEXTCAB = 0x0002,

        /// <summary>
        /// Indicates the cabinet has reserved header space.
        /// </summary>
        MSCAB_HDR_RESV = 0x0004,
    }

    public enum Parameters
    {
        /// <summary>
        /// Search buffer size.
        /// </summary>
        MSCABD_PARAM_SEARCHBUF = 0,

        /// <summary>
        /// Repair MS-ZIP streams?
        /// </summary>
        MSCABD_PARAM_FIXMSZIP = 1,

        /// <summary>
        /// Size of decompression buffer
        /// </summary>
        MSCABD_PARAM_DECOMPBUF = 2,

        /// <summary>
        /// salvage data from bad cabinets?
        /// If enabled, open() will skip file with bad folder indices or filenames
        /// rather than reject the whole cabinet, and extract() will limit rather than
        /// reject files with invalid offsets and lengths, and bad data block checksums
        /// will be ignored. Available only in CAB decoder version 2 and above.
        /// </summary>
        MSCABD_PARAM_SALVAGE = 3,
    }
}
