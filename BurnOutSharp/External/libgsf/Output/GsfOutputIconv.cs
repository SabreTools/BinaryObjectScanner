/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-iconv.c: wrapper to convert character sets.
 *
 * Copyright (C) 2005-2006 Morten Welinder (terra@gnome.org)
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

namespace LibGSF.Output
{
    public class GsfOutputIconv : GsfOutput
    {
        #region Constants

        public const int BUF_SIZE = 0x400;

        #endregion

        #region Properties

        public GsfOutput Sink { get; set; }

        public Encoding InputCharset { get; set; }

        public Encoding OutputCharset { get; set; }

        /// <summary>
        /// Either null or a UTF-8 string (representable in the target encoding)
        /// to convert and output in place of characters that cannot be represented
        /// in the target encoding. null means use \u1234 or \U12345678 format.
        /// </summary>
        public string Fallback { get; set; }

        public byte[] Buf { get; set; } = new byte[BUF_SIZE];

        public int BufLen { get; set; } = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutputIconv() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sink">The underlying data source.</param>
        /// <param name="dst">The target character set.</param>
        /// <param name="src">he source character set.</param>
        /// <returns>A new GsfOutput object or null.</returns>
        /// <remarks>Adds a reference to <paramref name="sink"/>.</remarks>
        public static GsfOutputIconv Create(GsfOutput sink, Encoding dst, Encoding src)
        {
            if (sink == null)
                return null;

            if (dst == null)
                dst = Encoding.UTF8;
            if (src == null)
                src = Encoding.UTF8;

            return new GsfOutputIconv
            {
                Sink = sink,
                InputCharset = src,
                OutputCharset = dst,
            };
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
            if (data == null)
                return false;

            int dataPtr = 0; // data[0]
            while (num_bytes > 0)
            {
                if (Error != null)
                    return false;

                if (BufLen == BUF_SIZE)
                {
                    Flush(false);
                }
                else
                {
                    int count = Math.Min(BUF_SIZE - BufLen, num_bytes);
                    Array.Copy(data, dataPtr, Buf, BufLen, count);
                    BufLen += count;
                    num_bytes -= count;
                    dataPtr += count;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => false;

        /// <inheritdoc/>
        protected override bool CloseImpl()
        {
            if (Error != null)
                return true;

            return Flush(true);
        }

        #endregion

        #region Utilities

        private bool Flush(bool must_empty)
        {
            if (Error != null)
                return false;

            if (BufLen <= 0)
                return true;

            bool ok = true;

            byte[] data = Encoding.Convert(InputCharset, OutputCharset, Buf, 0, BufLen);
            if (data == null || data.Length <= 0)
            {
                Error = new Exception("Failed to convert string");
                ok = false;
            }
            else if (!Sink.Write(data.Length, data))
            {
                Error = new Exception("Failed to write");
                ok = false;
            }
            else
            {
                BufLen = 0;
                ok = true;
            }

            return ok && (!must_empty || BufLen == 0);
        }

        #endregion
    }
}
