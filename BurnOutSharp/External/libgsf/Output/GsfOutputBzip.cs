/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-bzip.c: wrapper to compress to bzipped output
 *
 * Copyright (C) 2003-2006 Dom Lachowicz (cinamod@hotmail.com)
 *               2002-2006 Jon K Hellan (hellan@acm.org)
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
    // TODO: Implement BZIP writing
    public class GsfOutputBzip : GsfOutput
    {
        #region Constants

        private const int BZ_BUFSIZE = 1024;

        #endregion

        #region Properties

#if BZIP2
        /// <summary>
        /// Compressed data
        /// </summary>
        public GsfOutput Sink { get; set; } = null;

        public bz_stream Stream { get; set; } = new bz_stream();

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutputBzip() { }

        /// <param name="sink">The underlying data source.</param>
        /// <returns>A new file or null.</returns>
        /// <remarks>Adds a reference to <paramref name="sink"/>.</remarks>
        public static GsfOutputBzip Create(GsfOutput sink, ref Exception err)
        {
#if BZIP2
            if (sink == null)
                return null;

            GsfOutputBzip bzip = new GsfOutputBzip
            {
                Sink = sink,
            };

            if (!InitBzip(ref err))
                return null;

            return bzip;
#else
            err = new Exception("BZ2 support not enabled");
            return null;
#endif
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
#if BZIP2
            if (data == null)
                return false;

            Stream.next_in  = data;
            Stream.avail_in = num_bytes;

            while (Stream.avail_in > 0)
            {
                if (Stream.avail_out == 0)
                {
                    if (!OutputBlock())
                        return false;
                }

                int zret = BZ2_bzCompress(&bzip.stream, BZ_RUN);
                if (zret != BZ_RUN_OK)
                {
                    Console.Error.WriteLine($"Unexpected error code {zret} from bzlib during compression.");
                    return false;
                }
            }

            if (Stream.avail_out == 0)
            {
                if (!OutputBlock())
                    return false;
            }

            return true;
#else
            return false;
#endif
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => false;

        protected override bool CloseImpl()
        {
#if BZIP2
            bool rt = Flush();
            BZ2_bzCompressEnd(Stream);

            return rt;
#else
            return false;
#endif
        }

        #endregion

        #region Utilities

#if BZIP2
        private bool InitBzip(ref Exception err)
        {
            int ret = BZ2_bzCompressInit(Stream, 6, 0, 0);

            if (ret != BZ_OK)
            {
                err = new Exception("Unable to initialize BZ2 library");
                return false;
            }

            if (Buf == null)
            {
                BufSize = BZ_BUFSIZE;
                Buf = new byte[BufSize];
            }

            Stream.next_out = Buf;
            Stream.avail_out = BufSize;

            return true;
        }

        private bool OutputBlock()
        {
            int num_bytes = BufSize - Stream.avail_out;

            if (!Sink.Write(num_bytes, Buf))
                return false;

            Stream.next_out = Buf;
            Stream.avail_out = BufSize;

            return true;
        }

        private bool Flush()
        {
            int zret;

            do
            {
                zret = BZ2_bzCompress(Stream, BZ_FINISH);
                if (zret == BZ_FINISH_OK)
                {
                    //  In this case BZ_FINISH_OK means more buffer space needed
                    if (!OutputBlock())
                        return false;
                }
            } while (zret == BZ_FINISH_OK);

            if (zret != BZ_STREAM_END)
            {
                Console.Error.WriteLine($"Unexpected error code {zret} from bzlib during compression.");
                return false;
            }

            if (!OutputBlock())
                return false;

            return true;
        }
#endif

        #endregion
    }
}
