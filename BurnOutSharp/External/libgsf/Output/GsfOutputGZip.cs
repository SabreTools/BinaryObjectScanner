/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-gzip.c: wrapper to compress to gzipped output. See rfc1952.
 *
 * Copyright (C) 2002-2006 Jon K Hellan (hellan@acm.org)
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using static ComponentAce.Compression.Libs.zlib.zlibConst;

namespace LibGSF.Output
{
    public class GsfOutputGZip : GsfOutput
    {
        #region Constants

        /// <summary>
        /// GZip flag byte - The original is stored
        /// </summary>
        private const byte GZIP_ORIGINAL_NAME = 0x08;

        #endregion

        #region Properties

        /// <summary>
        /// Compressed data
        /// </summary>
        public GsfOutput Sink { get; set; } = null;

        /// <summary>
        /// No header and no trailer.
        /// </summary>
        public bool Raw { get; set; }

        /// <summary>
        /// zlib compression level
        /// </summary>
        public int DeflateLevel { get; set; } = Z_DEFAULT_COMPRESSION;

        public ZStream Stream { get; set; }

        /// <summary>
        /// CRC32 of uncompressed data
        /// </summary>
        public uint CRC { get; set; } = 0;

        public int ISize { get; set; } = 0;

        public bool Setup { get; set; } = false;

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutputGZip() { }

        /// <param name="sink">The underlying data source.</param>
        /// <param name="err">Optionally null.</param>
        /// <returns>A new file or null</returns>
        /// <remarks>Adds a reference to <paramref name="sink"/>.</remarks>
        public static GsfOutputGZip Create(GsfOutput sink, ref Exception err)
        {
            if (sink == null)
                return null;

            GsfOutputGZip output = new GsfOutputGZip
            {
                Sink = sink,
            };

            if (output.Error != null)
            {
                err = output.Error;
                return null;
            }

            return output;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfOutputGZip()
        {
            // FIXME: check for error?
            Stream.deflateEnd();
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
            if (data == null)
                return false;

            // Write header, if needed
            SetupImpl();

            Stream.next_in = data;
            Stream.avail_in = num_bytes;

            while (Stream.avail_in > 0)
            {
                if (Stream.avail_out == 0)
                {
                    if (!OutputBlock())
                        return false;
                }

                int zret = Stream.deflate(Z_NO_FLUSH);
                if (zret != Z_OK)
                {
                    Error = new Exception("Unexpected compression failure");
                    Console.Error.WriteLine($"Unexpected error code {zret} from zlib during compression.");
                    return false;
                }
            }

            // TODO: Enable CRC32 calculation
            //CRC = crc32(gzip.crc, data, num_bytes);
            ISize += num_bytes;

            if (Stream.avail_out == 0)
            {
                if (!OutputBlock())
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => false;

        /// <inheritdoc/>
        protected override bool CloseImpl()
        {
            // Just in case nothing was ever written
            SetupImpl();

            if (Error != null)
            {
                if (!Flush())
                    return false;

                if (!Raw)
                {
                    List<byte> buf = new List<byte>();

                    buf.AddRange(BitConverter.GetBytes((uint)(CRC)));
                    buf.AddRange(BitConverter.GetBytes((uint)(ISize)));
                    if (!Sink.Write(8, buf.ToArray()))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Utilities

        private bool InitGZip()
        {
            int ret = Stream.deflateInit(DeflateLevel);
            if (ret != Z_OK)
                return false;

            ret = Stream.deflateParams(DeflateLevel, Z_DEFAULT_STRATEGY);
            if (ret != Z_OK)
                return false;

            if (Buf == null)
            {
                BufSize = 0x100;
                Buf = new byte[BufSize];
            }

            Stream.next_out = Buf;
            Stream.avail_out = BufSize;

            return true;
        }

        private bool OutputHeader()
        {
            List<byte> buf = new List<byte>(3 + 1 + 4 + 2);

            DateTime? modtime = ModTime;
            ulong mtime = (ulong)(modtime != null ? new DateTimeOffset(modtime.Value).ToUnixTimeSeconds() : 0);

            string name = Sink.Name;
            // FIXME: What to do about gz extension ... ?
            int nlen = 0;  // name ? strlen (name) : 0;

            buf.AddRange(new byte[] { 0x1f, 0x8b, 0x08 });

            if (nlen > 0)
                buf.Add(GZIP_ORIGINAL_NAME);

            buf.AddRange(BitConverter.GetBytes((uint)(mtime)));
            buf.Add(3); // UNIX
            bool ret = Sink.Write(buf.Count, buf.ToArray());
            if (ret && name != null && nlen > 0)
                ret = Sink.Write(nlen, Encoding.ASCII.GetBytes(name));

            return ret;
        }

        private void SetupImpl()
        {
            if (Setup)
                return;

            if (!InitGZip())
                Error = new Exception("Failed to initialize zlib structure");
            else if (!Raw && !OutputHeader())
                Error = new Exception("Failed to write gzip header");

            Setup = true;
        }

        private bool OutputBlock()
        {
            int num_bytes = BufSize - Stream.avail_out;
            if (!Sink.Write(num_bytes, Buf))
            {
                Error = new Exception("Failed to write");
                return false;
            }

            Stream.next_out = Buf;
            Stream.avail_out = BufSize;

            return true;
        }

        private bool Flush()
        {
            int zret;

            do
            {
                zret = Stream.deflate(Z_FINISH);
                if (zret == Z_OK)
                {
                    //  In this case Z_OK means more buffer space needed
                    if (!OutputBlock())
                        return false;
                }
            } while (zret == Z_OK);

            if (zret != Z_STREAM_END)
            {
                Error = new Exception("Unexpected compression failure");
                Console.Error.WriteLine($"Unexpected error code {zret} from zlib during compression.");
                return false;
            }

            if (!OutputBlock())
                return false;

            return true;
        }

        #endregion
    }
}
