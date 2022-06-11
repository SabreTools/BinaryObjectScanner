/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-gio.c:
 *
 * Copyright (C) 2007 Dom Lachowicz <cinamod@hotmail.com>
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
using LibGSF.Output;

namespace LibGSF.Input
{
    public class GsfInputGio : GsfInput
    {
        #region Properties

        public string File { get; set; } = null;

        public Stream Stream { get; set; } = null;

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInputGio() { }

        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>A new GsfInputGio or null</returns>
        public static GsfInput Create(string file, ref Exception err)
        {
            if (file == null || !System.IO.File.Exists(file))
                return null;

            long filesize;

            Stream stream;
            try
            {
                stream = System.IO.File.OpenRead(file);
            }
            catch (Exception ex)
            {
                err = ex;
                return null;
            }

            if (true)
            {
                // see https://bugzilla.gnome.org/show_bug.cgi?id=724970
                return MakeLocalCopy(file, stream);
            }

            if (!stream.CanSeek)
                return MakeLocalCopy(file, stream);

            {
                FileInfo info = new FileInfo(file);
                filesize = info.Length;
            }

            GsfInputGio input = new GsfInputGio
            {
                Size = filesize,
                Stream = stream,
                File = file,
                Buf = null,
                BufSize = 0,
            };

            input.SetNameFromFile(file);
            return input;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfInputGio()
        {
            Stream.Close();
            Stream = null;

            File = null;

            if (Buf != null)
            {
                Buf = null;
                BufSize = 0;
            }
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            if (File != null)
                return null;

            return Create(File, ref err);
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            if (Stream == null)
                return null;

            int total_read = 0;

            if (optional_buffer == null)
            {
                if (BufSize < num_bytes)
                {
                    BufSize = num_bytes;
                    Buf = new byte[BufSize];
                }

                optional_buffer = Buf;
            }

            while (total_read < num_bytes)
            {
                int try_to_read = Math.Min(int.MaxValue, num_bytes - total_read);
                int nread = Stream.Read(optional_buffer, total_read, try_to_read);
                if (nread > 0)
                {
                    total_read += nread;
                }
                else
                {
                    // Getting zero means EOF which isn't supposed to
                    // happen.   Negative means error.
                    return null;
                }
            }

            return optional_buffer;
        }

        /// <inheritdoc/>
        public override bool Seek(long offset, SeekOrigin whence)
        {
            if (Stream == null)
                return true;

            if (!Stream.CanSeek)
                return true;

            try
            {

                Stream.Seek(offset, whence);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Utilities

        private static GsfInput MakeLocalCopy(string file, Stream stream)
        {
            GsfInput copy;
            GsfOutput output = GsfOutputMemory.Create();
            while (true)
            {
                byte[] buf = new byte[4096];
                int nread = stream.Read(buf, 0, buf.Length);
                if (nread > 0)
                {
                    if (!output.Write(nread, buf))
                    {
                        copy = null;
                        output.Close();
                        stream.Close();
                        return copy;
                    }
                }
                else if (nread == 0)
                {
                    break;
                }
                else
                {
                    copy = null;
                    output.Close();
                    stream.Close();
                    return copy;
                }
            }

            copy = GsfInputMemory.Clone((output as GsfOutputMemory).Buffer, output.CurrentSize);

            if (copy != null)
            {
                if (System.IO.File.Exists(file))
                {
                    FileInfo info = new FileInfo(file);
                    copy.Name = info.Name;
                }
            }

            output.Close();
            stream.Close();
            copy.SetNameFromFile(file);

            return copy;
        }

        #endregion
    }
}
