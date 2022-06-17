
/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-textline.c: textline based input
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
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
using System.Text;

namespace LibGSF.Input
{
    public class GsfInputTextline : GsfInput
    {
        #region Properties

        public GsfInput Source { get; set; } = null;

        public byte[] Remainder { get; set; } = null;

        public int RemainderSize { get; set; } = 0;

        public int MaxLineSize { get; set; } = 512; // An initial guess

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;

        #endregion

        #region Functions

        /// <param name="source">In some combination of ascii and UTF-8</param>
        /// <returns>A new file</returns>
        /// <remarks>This adds a reference to @source.</remarks>
        public static GsfInputTextline Create(GsfInput source)
        {
            if (source == null)
                return null;

            return new GsfInputTextline
            {
                Source = source,
                Buf = null,
                BufSize = 0,
                Size = source.Size,
                Name = source.Name,
            };
        }

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            return new GsfInputTextline()
            {
                Source = this.Source,
                Size = this.Size,
                Name = this.Name,
            };
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            Remainder = null;
            byte[] res = Source.Read(num_bytes, optional_buffer);
            CurrentOffset = Source.CurrentOffset;
            return res;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence)
        {
            Remainder = null;
            bool res = Source.Seek(offset, whence);
            CurrentOffset = Source.CurrentOffset;
            return res;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// A utility routine to read things line by line from the underlying source.
        /// Trailing newlines and carriage returns are stripped, and the resultant buffer
        /// can be edited.
        /// </summary>
        /// <returns>The string read, or null on eof.</returns>
        internal string GetStringASCII() => GetStringUTF8();

        /// <summary>
        /// A utility routine to read things line by line from the underlying source.
        /// Trailing newlines and carriage returns are stripped, and the resultant buffer
        /// can be edited.
        /// </summary>
        /// <returns>The string read, or null on eof.</returns>
        internal string GetStringUTF8()
        {
            int len, ptr, end;
            int count = 0;
            while (true)
            {
                if (Remainder == null || RemainderSize == 0)
                {
                    long remain = Source.Remaining();
                    len = (int)Math.Min(remain, MaxLineSize);

                    Remainder = Source.Read(len, null);
                    if (Remainder == null)
                        return null;

                    RemainderSize = len;
                }

                ptr = 0; // Remainder[0]
                end = ptr + RemainderSize;

                for (; ptr < end; ptr++)
                {
                    if (Remainder[ptr] == '\n' || Remainder[ptr] == '\r')
                        break;
                }

                // Copy the remains into the buffer, grow it if necessary
                len = ptr;
                if (count + len >= BufSize)
                {
                    BufSize = len;
                    Buf = new byte[BufSize + 1];
                }

                if (Buf == null)
                    return null;

                Array.Copy(Remainder, 0, Buf, count, len);
                count += len;

                if (ptr < end)
                {
                    char last = (char)Remainder[ptr];

                    // Eat the trailing eol marker: \n, \r\n, or \r.
                    ptr++;
                    if (ptr >= end && last == '\r')
                    {
                        // Be extra careful, the CR is at the bound
                        if (Source.Remaining() > 0)
                        {
                            if (Source.Read(1, Remainder, ptr) == null)
                                return null;

                            RemainderSize = 1;
                            end = ptr + 1;
                        }
                        else
                        {
                            ptr = end = -1;
                        }
                    }

                    if (ptr != -1 && last == '\r' && Remainder[ptr] == '\n')
                        ptr++;

                    break;
                }
                else if (Source.Remaining() <= 0)
                {
                    ptr = end = -1;
                    break;
                }
                else
                {
                    Remainder = null;
                }
            }

            if (ptr == -1)
            {
                Remainder = null;
                RemainderSize = 0;
            }
            else
            {
                RemainderSize = end - ptr;
            }

            CurrentOffset = Source.CurrentOffset - (Remainder != null ? RemainderSize : 0);
            Buf[count] = 0x00;

            return Encoding.UTF8.GetString(Buf);
        }

        #endregion
    }
}
