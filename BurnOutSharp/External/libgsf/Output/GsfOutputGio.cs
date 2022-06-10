/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-gio.c:
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

namespace LibGSF.Output
{
    public class GsfOutputGio : GsfOutput
    {
        #region Properties

        public Stream Stream { get; set; } = null;

        public bool CanSeek { get; set; } = false;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutputGio() { }

        /// <param name="file">An existing GFile</param>
        /// <returns>A new GsfOutputGio or null</returns>
        public static GsfOutputGio Create(string file)
        {
            Exception err = null;
            return Create(file, ref err);
        }

        /// <param name="file">An existing GFile</param>
        /// <returns>A new GsfOutputGio or null</returns>
        public static GsfOutputGio Create(string file, ref Exception err)
        {
            if (file == null)
                return null;

            try
            {
                Stream stream = File.OpenWrite(file);
                return new GsfOutputGio
                {
                    Stream = stream,
                    CanSeek = CanSeekSafe(stream),
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfOutputGio() => Close();

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool CloseImpl()
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
            if (Stream == null)
                return false;

            if (num_bytes <= 0)
                return true;

            try
            {
                Stream.Write(data, 0, num_bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence)
        {
            if (Stream == null)
                return false;
            if (CanSeek)
                return false;

            try
            {
                Stream.Seek(offset, whence); ;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Utilities

        private static bool CanSeekSafe(Stream stream) => stream?.CanSeek ?? false;

        #endregion
    }
}
