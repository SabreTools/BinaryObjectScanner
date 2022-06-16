/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-memory.c:
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
using System.Linq;

namespace LibGSF.Input
{
    public partial class GsfInputMemory : GsfInput
    {
        #region Properties

        public GsfSharedMemory Shared { get; set; } = null;

        #endregion

        #region Functions

        /// <summary>
        /// </summary>
        /// <param name="buf">The input bytes</param>
        /// <param name="length">The length of <paramref name="buf"/></param>
        /// <param name="needs_free">Whether you want this memory to be free'd at object destruction</param>
        /// <returns>A new GsfInputMemory</returns>
        public static GsfInputMemory Create(byte[] buf, long length, bool needs_free)
        {
            GsfInputMemory mem = new GsfInputMemory();
            mem.Shared = GsfSharedMemory.Create(buf, length, needs_free);
            mem.Size = length;
            return mem;
        }

        /// <param name="buf">The input bytes</param>
        /// <param name="length">The length of <paramref name="buf"/></param>
        /// <returns>A new GsfInputMemory</returns>
        public static GsfInputMemory Clone(byte[] buf, long length)
        {
            if (buf == null || length < 0)
                return null;

            GsfInputMemory mem = new GsfInputMemory();
            byte[] cpy = new byte[Math.Max(1, length)];
            if (buf.Length > 0)
                Array.Copy(buf, cpy, length);

            mem.Shared = GsfSharedMemory.Create(cpy, length, true);
            mem.Size = length;
            return mem;
        }

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            return new GsfInputMemory
            {
                Shared = Shared,
                Size = Shared.Size,
            };
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            byte[] src = Shared.Buf;
            if (src == null)
                return null;

            if (optional_buffer != null)
            {
                Array.Copy(src, CurrentOffset, optional_buffer, 0, num_bytes);
                return optional_buffer;
            }
            else
            {
                return new ReadOnlySpan<byte>(src, (int)CurrentOffset, num_bytes).ToArray();
            }
        }

        /// <inheritdoc/>
        public override bool Seek(long offset, SeekOrigin whence) => false;

        #endregion
    }

    public class GsfInputMemoryMap : GsfInputMemory
    {
        private const int PROT_READ = 0x1;

        /// <param name="filename">The file on disk that you want to mmap</param>
        /// <param name="err">An Exception</param>
        /// <returns>A new GsfInputMemory</returns>
        public GsfInputMemoryMap Create(string filename, ref Exception err)
        {
            FileStream fd;
            try
            {
                fd = File.OpenRead(filename);
            }
            catch (Exception ex)
            {
                err = ex;
                return null;
            }

            FileInfo st = new FileInfo(filename);
            if (!st.Attributes.HasFlag(FileAttributes.Normal))
            {
                err = new Exception($"{filename}: Is not a regular file");
                fd.Close();
                return null;
            }

            long size = fd.Length;
            byte[] buf = new byte[size];
            fd.Read(buf, 0, (int)size);

            GsfInputMemoryMap mem = new GsfInputMemoryMap
            {
                Shared = GsfSharedMemory.CreateMemoryMapped(buf, size),
                Size = size,
                Name = filename,
                ModTime = st.LastWriteTime,
            };

            fd.Close();

            return mem;
        }
    }
}
