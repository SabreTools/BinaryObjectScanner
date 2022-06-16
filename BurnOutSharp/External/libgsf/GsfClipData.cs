/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-clip-data.c: clipboard data
 *
 * Copyright (C) 2006 Novell Inc
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2.1 of the GNU Lesser General Public
 * License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.IO;
using static LibGSF.GsfUtils;

namespace LibGSF
{
    #region Enums

    // TODO: Can this be made internal?
    public enum GsfClipFormat
    {
        /// <summary>
        /// Windows clipboard format
        /// </summary>
        GSF_CLIP_FORMAT_WINDOWS_CLIPBOARD = -1,

        /// <summary>
        /// Macintosh clipboard format
        /// </summary>
        GSF_CLIP_FORMAT_MACINTOSH_CLIPBOARD = -2,

        /// <summary>
        /// GUID that contains a format identifier
        /// </summary>
        GSF_CLIP_FORMAT_GUID = -3,

        /// <summary>
        /// No clipboard data
        /// </summary>
        GSF_CLIP_FORMAT_NO_DATA = 0,

        /// <summary>
        /// Custom clipboard format
        /// </summary>
        /// <remarks>
        /// In the file it's actually any positive integer
        /// </remarks>
        GSF_CLIP_FORMAT_CLIPBOARD_FORMAT_NAME = 1,

        /// <summary>
        /// Unknown clipboard type or invalid data
        /// </summary>
        /// <remarks>
        /// This is our own value for unknown types or invalid data
        /// </remarks>
        GSF_CLIP_FORMAT_UNKNOWN
    }

    // TODO: Can this be made internal?
    public enum GsfClipFormatWindows
    {
        /// <summary>
        /// Our own value
        /// </summary>
        GSF_CLIP_FORMAT_WINDOWS_ERROR = -1,

        /// <summary>
        /// Our own value
        /// </summary>
        GSF_CLIP_FORMAT_WINDOWS_UNKNOWN = -2,

        /// <summary>
        /// CF_METAFILEPICT
        /// </summary>
        GSF_CLIP_FORMAT_WINDOWS_METAFILE = 3,

        /// <summary>
        /// CF_DIB
        /// </summary>
        GSF_CLIP_FORMAT_WINDOWS_DIB = 8,

        /// <summary>
        /// CF_ENHMETAFILE
        /// </summary>
        GSF_CLIP_FORMAT_WINDOWS_ENHANCED_METAFILE = 14
    }

    #endregion

    // TODO: Can this be made internal?
    public class GsfClipData
    {
        #region Properties

        public GsfClipFormat Format { get; private set; }

        public GsfBlob DataBlob { get; private set; }

        #endregion

        #region Private Classes

        private class FormatOffsetPair
        {
            public GsfClipFormatWindows Format { get; set; }

            public long Offset { get; set; }
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfClipData() { }

        /// <summary>
        /// Creates a new GsfClipData object.  This function acquires a reference to the
        /// <paramref name="data_blob"/>, so you should unref the blob on your own if you no longer need it
        /// directly.
        /// </summary>
        /// <param name="format">Format for the data inside the <paramref name="data_blob"/></param>
        /// <param name="data_blob">Object which holds the binary contents for the GsfClipData</param>
        /// <returns>A newly-created GsfClipData.</returns>
        public static GsfClipData Create(GsfClipFormat format, GsfBlob data_blob)
        {
            if (data_blob == null)
                return null;

            return new GsfClipData
            {
                Format = format,
                DataBlob = data_blob,
            };
        }

        #endregion

        #region Functions

        /// <summary>
        /// Queries the Windows clipboard data format for a GsfClipData.  The <paramref name="clip_data"/> must
        /// have been created with #GSF_CLIP_FORMAT_WINDOWS_CLIPBOARD.
        /// </summary>
        /// <param name="error">Location to store error, or NULL</param>
        /// <returns>A GsfClipFormatWindows value.</returns>
        public GsfClipFormatWindows GetWindowsClipboardFormat(ref Exception error)
        {
            if (error == null)
                return GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ERROR;
            if (Format != GsfClipFormat.GSF_CLIP_FORMAT_WINDOWS_CLIPBOARD)
                return GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ERROR;

            long size = DataBlob.Size;
            if (size < 4)
            {
                error = new InvalidDataException("The clip_data is in Windows clipboard format, but it is smaller than the required 4 bytes.");
                return GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ERROR;
            }

            byte[] data = DataBlob.Data;
            uint value = GSF_LE_GET_GUINT32(data, 0);

            switch (value)
            {
                case (uint)GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_METAFILE:
                    return CheckFormatWindows(GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_METAFILE, "Windows Metafile format", size, ref error);

                case (uint)GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_DIB:
                case 2: /* CF_BITMAP */
                    return CheckFormatWindows(GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_DIB, "Windows DIB or BITMAP format", size, ref error);

                case (uint)GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ENHANCED_METAFILE:
                    return CheckFormatWindows(GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ENHANCED_METAFILE, "Windows Enhanced Metafile format", size, ref error);

                default:
                    return GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_UNKNOWN;
            }
        }

        /// <summary>
        /// Queries a pointer directly to the clipboard data of a GsfClipData.  The
        /// resulting pointer is not necessarily the same data pointer that was passed to
        /// gsf_blob_new() prior to creating the <paramref name="clip_data"/>.  For example, if the data is
        /// in GSF_CLIP_FORMAT_WINDOWS_CLIPBOARD format, then it will have extra header
        /// bytes in front of the actual metafile data.  This function will skip over
        /// those header bytes if necessary and return a pointer to the "real" data.
        /// </summary>
        /// <param name="ret_size">Location to return the size of the returned data buffer.</param>
        /// <param name="error">Location to store error, or NULL.</param>
        /// <returns>
        /// Pointer to the real clipboard data.  The size in bytes of this
        /// buffer is returned in the <paramref name="ret_size"/> argument.
        /// </returns>
        public byte[] PeekRealData(ref long ret_size, ref Exception error)
        {
            if (error == null)
                return null;

            byte[] data = DataBlob.Data;

            long offset;
            if (Format == GsfClipFormat.GSF_CLIP_FORMAT_WINDOWS_CLIPBOARD)
            {
                GsfClipFormatWindows win_format = GetWindowsClipboardFormat(ref error);
                if (win_format == GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ERROR)
                    return null;

                // GetWindowsClipboardFormat() already did the size checks for us,
                // so we can jump to the offset right away without doing extra checks.

                offset = GetWindowsClipboardDataOffset(win_format);
            }
            else
            {
                offset = 0;
            }

            ret_size = DataBlob.Size - offset;
            return new ReadOnlySpan<byte>(data, (int)offset, (int)ret_size).ToArray();
        }

        #endregion

        #region Utilities

        private static void SetErrorMissingClipboardData(ref Exception error, string format_name, long at_least_size)
        {
            error = new InvalidDataException($"The clip_data is in {format_name}, but it is smaller than at least {at_least_size} bytes");
        }

        private static long GetWindowsClipboardDataOffset(GsfClipFormatWindows format)
        {
            FormatOffsetPair[] pairs =
            {
                new FormatOffsetPair { Format = GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_UNKNOWN, Offset = 4 },
                new FormatOffsetPair { Format = GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_METAFILE, Offset = 12 },
                new FormatOffsetPair { Format = GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_DIB, Offset = 4 },

                // FIXME: does this have a PACKEDMETA in front as well, similar to GSF_CLIP_FORMAT_WINDOWS_METAFILE?
                new FormatOffsetPair { Format = GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ENHANCED_METAFILE, Offset = 4 }
            };

            int num_pairs = pairs.Length;
            for (int i = 0; i < num_pairs; i++)
            {
                if (pairs[i].Format == format)
                    return pairs[i].Offset;
            }

            Console.Error.WriteLine("Should not have reached this point");
            return 0;
        }

        /// <summary>
        /// Checks that the specified blob size matches the expected size for the format.
        /// </summary>
        /// <returns>
        /// The same format if the size is correct, or
        /// GSF_CLIP_FORMAT_WINDOWS_ERROR if the size is too small.
        /// </returns>
        private static GsfClipFormatWindows CheckFormatWindows(GsfClipFormatWindows format, string format_name, long blob_size, ref Exception error)
        {
            long offset = GetWindowsClipboardDataOffset(format);
            if (blob_size <= offset)
            {
                SetErrorMissingClipboardData(ref error, format_name, offset + 1);
                format = GsfClipFormatWindows.GSF_CLIP_FORMAT_WINDOWS_ERROR;
            }

            return format;
        }

        #endregion
    }
}
