/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-stdio.c: stdio based input
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
using LibGSF.Output;

namespace LibGSF.Input
{
    public class GsfInputStdio : GsfInput
    {
        #region Properties

        public FileStream File { get; set; } = null;

        public string Filename { get; set; } = null;

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;

        public bool KeepOpen { get; set; } = false;

        #endregion

        #region Functions

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfInputStdio()
        {
            if (File != null && !KeepOpen)
                File.Close();
        }

        public static GsfInput MakeLocalCopy(FileStream stream, string filename, ref Exception err)
        {
            GsfOutputMemory output = GsfOutputMemory.Create();

            while (true)
            {
                byte[] buf = new byte[4096];
                int nread = stream.Read(buf, 0, buf.Length);

                if (nread > 0)
                {
                    if (!output.Write(nread, buf))
                    {
                        err = new Exception($"{(filename != null ? filename : "?")}: not a regular file");
                        output.Close();
                        return null;
                    }
                }
                else if (nread == 0)
                {
                    break;
                }
                else
                {
                    err = new Exception($"{(filename != null ? filename : "?")}: not a regular file");
                    output.Close();
                    return null;
                }
            }

            GsfInput copy = GsfInputMemory.Clone(output.Buffer, output.Capacity);

            output.Close();

            if (filename != null)
                copy.SetNameFromFilename(filename);

            return copy;
        }

        /// <param name="filename">In UTF-8.</param>
        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>A new file or null.</returns>
        public static GsfInput Create(string filename, ref Exception err)
        {
            if (filename == null)
                return null;

            FileStream file;
            try
            {
                file = System.IO.File.OpenRead(filename);
            }
            catch (Exception ex)
            {
                err = ex;
                return null;
            }

            FileInfo fstat = new FileInfo(filename);
            if (!fstat.Attributes.HasFlag(FileAttributes.Normal))
            {
                GsfInput res = MakeLocalCopy(file, filename, ref err);
                file.Close();
                return res;
            }

            long size = fstat.Length;
            GsfInputStdio input = new GsfInputStdio
            {
                File = file,
                Filename = filename,
                Buf = null,
                BufSize = 0,
                KeepOpen = false,
                Size = size,
                ModTime = fstat.LastWriteTime,
            };

            input.SetNameFromFilename(filename);

            return input;
        }

        /// <summary>
        /// Assumes ownership of <paramref name="file"/> when succeeding.  If <paramref name="keep_open"/> is true,
        /// ownership reverts to caller when the GsfInput is closed.
        /// </summary>
        /// <param name="filename">The filename corresponding to <paramref name="file"/>.</param>
        /// <param name="file">An existing stdio <see cref="FileStream"/></param>
        /// <param name="keep_open">Should <paramref name="file"/> be closed when the wrapper is closed</param>
        /// <returns>
        /// A new GsfInput wrapper for <paramref name="file"/>.  Note that if the file is not
        /// seekable, this function will make a local copy of the entire file.
        /// </returns>
        public static GsfInput Create(string filename, FileStream file, bool keep_open)
        {
            if (filename == null)
                return null;
            if (file == null)
                return null;

            FileInfo st = new FileInfo(filename);
            if (!st.Attributes.HasFlag(FileAttributes.Normal))
            {
                Exception err = null;
                return MakeLocalCopy(file, filename, ref err);
            }

            GsfInputStdio stdio = new GsfInputStdio
            {
                File = file,
                KeepOpen = keep_open,
                Filename = filename,
                Size = st.Length,
            };

            stdio.SetNameFromFilename(filename);
            return stdio;
        }

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err) => Create(Filename, ref err);

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            if (File == null)
                return null;

            if (optional_buffer == null)
            {
                if (BufSize < num_bytes)
                {
                    BufSize = num_bytes;
                    Buf = new byte[BufSize];
                }

                optional_buffer = Buf;
            }

            int total_read = 0;
            while (total_read < num_bytes)
            {
                int nread = File.Read(optional_buffer, total_read, num_bytes - total_read);
                total_read += nread;
                if (total_read < num_bytes && File.Position >= File.Length)
                    return null;
            }

            return optional_buffer;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence)
        {
            if (File == null)
                return true;

            try
            {
                long pos = File.Seek(offset, whence);
                return pos != offset;
            }
            catch
            {
                return true;
            }
        }

        #endregion
    }
}
