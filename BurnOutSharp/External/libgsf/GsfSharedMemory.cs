/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-shared-memory.c:
 *
 * Copyright (C) 2002-2006 Morten Welinder (terra@diku.dk)
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

namespace LibGSF
{
    public class GsfSharedMemory
    {
        #region Properties

        public byte[] Buf { get; set; } = null;

        public long Size { get; set; }

        public bool NeedsFree { get; set; }

        public bool NeedsUnmap { get; set; }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfSharedMemory() { }

        public static GsfSharedMemory Create(byte[] buf, long size, bool needs_free)
        {
            return new GsfSharedMemory
            {
                Buf = buf,
                Size = size,
                NeedsFree = needs_free,
                NeedsUnmap = false,
            };
        }

        public static GsfSharedMemory CreateMemoryMapped(byte[] buf, long size)
        {
            int msize = (int)size;
            if (msize != size)
            {
                Console.Error.WriteLine("Memory buffer size too large");
                return null;
            }
            else
            {
                GsfSharedMemory mem = Create(buf, size, false);
                mem.NeedsUnmap = true;
                return mem;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfSharedMemory()
        {
            if (Buf != null)
            {
                if (NeedsFree)
                    Buf = null;
                //else if (NeedsUnmap)
                    //UnmapViewOfFile(mem.buf);
            }
        }

        #endregion
    }
}
