/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.CHM
{
    public class Constants
    {
        // _PMGHeader
        internal const int pmgl_Signature = 0x0000;
        internal const int pmgl_QuickRefSize = 0x0004;
        internal const int pmgl_PMGIEntries = 0x0008; // Unknown1 in PMGL
        internal const int pmgl_PrevChunk = 0x000C; // Not in PMGI
        internal const int pmgl_NextChunk = 0x0010; // Not in PMGI
        internal const int pmgl_PMGLEntries = 0x0014; // Not in PMGI
        internal const int pmgl_headerSIZEOF = 0x0014;
        internal const int pmgi_headerSIZEOF = 0x000C;

        // _LZXControlData
        internal const int lzxcd_Length = 0x0000;
        internal const int lzxcd_Signature = 0x0004;
        internal const int lzxcd_Version = 0x0008;
        internal const int lzxcd_ResetInterval = 0x000C;
        internal const int lzxcd_WindowSize = 0x0010;
        internal const int lzxcd_CacheSize = 0x0014;
        internal const int lzxcd_Unknown1 = 0x0018;
        internal const int lzxcd_SIZEOF = 0x001C;

        // _LZXResetTable
        internal const int lzxrt_Unknown1 = 0x0000;
        internal const int lzxrt_NumEntries = 0x0004;
        internal const int lzxrt_EntrySize = 0x0008;
        internal const int lzxrt_TableOffset = 0x000C;
        internal const int lzxrt_UncompLen = 0x0010;
        internal const int lzxrt_CompLen = 0x0018;
        internal const int lzxrt_FrameLen = 0x0020;
        internal const int lzxrt_Entries = 0x0028;
        internal const int lzxrt_headerSIZEOF = 0x0028;

        // Filenames of the system files used for decompression.
        // - Content and ControlData are essential.
        // - ResetTable is preferred, but SpanInfo can be used if not available
        internal const string ContentName = "::DataSpace/Storage/MSCompressed/Content";
        internal const string ControlName = "::DataSpace/Storage/MSCompressed/ControlData";
        internal const string SpanInfoName = "::DataSpace/Storage/MSCompressed/SpanInfo";
        internal const string ResetTableName = "::DataSpace/Storage/MSCompressed/Transform/{7FC28940-9D31-11D0-9B27-00A0C91E9C7C}/InstanceData/ResetTable";
    }
}
