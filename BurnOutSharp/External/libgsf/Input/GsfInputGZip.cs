/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-gzip.c: wrapper to uncompress gzipped input
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
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
using ComponentAce.Compression.Libs.zlib;
using static ComponentAce.Compression.Libs.zlib.zlibConst;
using static LibGSF.GsfUtils;

namespace LibGSF.Input
{
    public class GsfInputGZip : GsfInput
    {
        #region Enums

        [Flags]
        private enum GZIP_HEADER_FLAGS : byte
        {
            /// <summary>
            /// File contains text ?
            /// </summary>
            GZIP_IS_ASCII = 0x01,

            /// <summary>
            /// There is a CRC in the header
            /// </summary>
            GZIP_HEADER_CRC = 0x02,

            /// <summary>
            /// There is an 'extra' field
            /// </summary>
            GZIP_EXTRA_FIELD = 0x04,

            /// <summary>
            /// The original is stored
            /// </summary>
            GZIP_ORIGINAL_NAME = 0x08,

            /// <summary>
            /// There is a comment in the header
            /// </summary>
            GZIP_HAS_COMMENT = 0x10,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Compressed data
        /// </summary>
        public GsfInput Source { get; set; } = null;

        /// <summary>
        /// No header and no trailer.
        /// </summary>
        public bool Raw { get; set; } = false;

        public Exception Err { get; set; } = null;

        public long UncompressedSize { get; set; } = -1;

        public bool StopByteAdded { get; set; }

        public ZStream Stream { get; set; } = new ZStream();

        public byte[] GZippedData { get; set; }

        /// <summary>
        /// CRC32 of uncompressed data
        /// </summary>
        public ulong CRC { get; set; } = 0;

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;

        public long HeaderSize { get; set; }

        public long TrailerSize { get; set; }

        public long SeekSkipped { get; set; } = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInputGZip(GsfInput source = null, bool raw = false, long uncompressedSize = -1)
        {
            Source = source;
            Raw = raw;
            UncompressedSize = uncompressedSize;

            Exception tempErr = null;
            if (Source == null)
                Err = new Exception("Null source");
            else if (Raw && UncompressedSize < 0)
                Err = new Exception("Uncompressed size not set");
            else if (InitZip(ref tempErr) != false)
                Err = tempErr;
        }

        /// <summary>
        /// Adds a reference to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The underlying data source.</param>
        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns></returns>
        public static GsfInputGZip Create(GsfInput source, ref Exception err)
        {
            if (source == null)
                return null;

            GsfInputGZip gzip = new GsfInputGZip(source: source)
            {
                Name = source.Name,
            };

            return gzip;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfInputGZip()
        {
            Source = null;

            if (Stream != null)
                Stream.inflateEnd();

            Err = null;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            GsfInput src_source_copy;
            if (Source != null)
            {
                src_source_copy = Source.Duplicate(ref err);
                if (err != null)
                    return null;
            }
            else
            {
                src_source_copy = null;
            }

            GsfInputGZip dst = new GsfInputGZip(source: src_source_copy, raw: (Source as GsfInputGZip)?.Raw ?? false);

            if (Err != null)
            {
                dst.Err = Err;
            }
            else if (dst.Err != null)
            {
                err = dst.Err;
                return null;
            }

            return dst;
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            if (optional_buffer == null)
            {
                if (BufSize < num_bytes)
                {
                    BufSize = Math.Max(num_bytes, 256);
                    Buf = new byte[BufSize];
                }

                optional_buffer = Buf;
            }

            Stream.next_out = optional_buffer;
            Stream.avail_out = num_bytes;
            while (Stream.avail_out != 0)
            {
                int zerr;
                if (Stream.avail_in == 0)
                {
                    long remain = Source.Remaining();
                    if (remain <= TrailerSize)
                    {
                        if (remain < TrailerSize || StopByteAdded)
                        {
                            Err = new Exception("Truncated source");
                            return null;
                        }

                        // zlib requires an extra byte.
                        Stream.avail_in = 1;
                        GZippedData = new byte[0];
                        StopByteAdded = true;
                    }
                    else
                    {
                        int n = (int)Math.Min(remain - TrailerSize, 4096);

                        GZippedData = Source.Read(n, null);
                        if (GZippedData == null)
                        {
                            Err = new Exception("Failed to read from source");
                            return null;
                        }

                        Stream.avail_in = n;
                    }

                    Stream.next_in = GZippedData;
                }

                zerr = Stream.inflate(Z_NO_FLUSH);
                if (zerr != Z_OK)
                {
                    if (zerr != Z_STREAM_END)
                        return null;

                    // Premature end of stream.
                    if (Stream.avail_out != 0)
                        return null;
                }
            }

            // TODO: Enable CRC32 calculation
            //CRC = crc32(CRC, optional_buffer, (uint)(Stream.next_out - optional_buffer));
            return optional_buffer;
        }

        // Global flag -- we don't want one per stream.
        private static bool warned = false;

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence)
        {            
            long pos = offset;

            // Note, that pos has already been sanity checked.
            switch (whence)
            {
                case SeekOrigin.Begin: break;
                case SeekOrigin.Current: pos += CurrentOffset; break;
                case SeekOrigin.End: pos += Size; break;
                default: return true;
            }

            if (pos < CurrentOffset)
            {
                if (Source.Seek(HeaderSize, SeekOrigin.Begin))
                    return true;

                // TODO: Enable CRC32 calculation
                //CRC = crc32(0L, Z_null, 0);
                Stream.avail_in = 0;
                if (Stream.inflateInit() != Z_OK)
                    return true;

                CurrentOffset = 0;
            }

            if (SeekEmulate(pos))
                return true;

            SeekSkipped += pos;
            if (!warned &&
                SeekSkipped != pos && // Don't warn for single seek.
                SeekSkipped >= 1000000)
            {
                warned = true;
                Console.Error.WriteLine("Seeking in gzipped streams is awfully slow.");
            }

            return false;
        }

        #endregion

        #region Utilities

        private bool CheckHeader()
        {
            if (Raw)
            {
                HeaderSize = 0;
                TrailerSize = 0;
            }
            else
            {
                byte[] data;
                uint len;

                // Check signature
                byte[] signature = { 0x1f, 0x8b };
                if ((data = Source.Read(2 + 1 + 1 + 6, null)) == null
                    || !(data[0] == signature[0] && data[1] == signature[1]))
                {
                    return true;
                }

                // zlib constant
                int Z_DEFLATED = 8;

                // Verify flags and compression type
                GZIP_HEADER_FLAGS flags = (GZIP_HEADER_FLAGS)data[3];
                if (data[2] != Z_DEFLATED)
                    return true;

                uint modutime = GSF_LE_GET_GUINT32(data, 4);
                if (modutime != 0)
                {
                    DateTime modtime = DateTimeOffset.FromUnixTimeSeconds(modutime).DateTime;
                    ModTime = modtime;
                }

                // If we have the size, don't bother seeking to the end.
                if (UncompressedSize < 0)
                {
                    // Get the uncompressed size
                    if (Source.Seek(-4, SeekOrigin.End)
                        || (data = Source.Read(4, null)) == null)
                    {
                        return true;
                    }

                    // FIXME, but how?  The size read here is modulo 2^32.
                    UncompressedSize = GSF_LE_GET_GUINT32(data, 0);

                    if (UncompressedSize / 1000 > Source.Size)
                    {
                        Console.Error.WriteLine("Suspiciously well compressed file with better than 1000:1 ratio.\n"
                            + "It is probably truncated or corrupt");
                    }
                }

                if (Source.Seek(2 + 1 + 1 + 6, SeekOrigin.Begin))
                    return true;

                if (flags.HasFlag(GZIP_HEADER_FLAGS.GZIP_EXTRA_FIELD))
                {
                    if ((data = Source.Read(2, null)) == null)
                        return true;

                    len = GSF_LE_GET_GUINT16(data, 0);
                    if (Source.Read((int)len, null) == null)
                        return true;
                }

                if (flags.HasFlag(GZIP_HEADER_FLAGS.GZIP_ORIGINAL_NAME))
                {
                    // Skip over the filename (which is in ISO 8859-1 encoding).
                    do
                    {
                        if ((data = Source.Read(1, null)) == null)
                            return true;
                    } while (data[0] != 0);
                }

                if (flags.HasFlag(GZIP_HEADER_FLAGS.GZIP_HAS_COMMENT))
                {
                    // Skip over the comment (which is in ISO 8859-1 encoding).
                    do
                    {
                        if ((data = Source.Read(1, null)) == null)
                            return true;
                    } while (data[0] != 0);
                }

                if (flags.HasFlag(GZIP_HEADER_FLAGS.GZIP_HEADER_CRC) && (data = Source.Read(2, null)) == null)
                    return true;

                HeaderSize = Source.CurrentOffset;

                // the last 8 bytes are the crc and size.
                TrailerSize = 8;
            }

            Size = UncompressedSize;

            if (Source.Remaining() < TrailerSize)
                return true;    // No room for payload

            return false;
        }

        private bool InitZip(ref Exception err)
        {
            if (Z_OK != Stream.inflateInit(-15))
            {
                err = new Exception("Unable to initialize zlib");
                return true;
            }

            long cur_pos = Source.CurrentOffset;
            if (Source.Seek(0, SeekOrigin.Begin))
            {
                err = new Exception("Failed to rewind source");
                return true;
            }

            if (CheckHeader() != false)
            {
                err = new Exception("Invalid gzip header");
                if (Source.Seek(cur_pos, SeekOrigin.Begin))
                    Console.Error.WriteLine("attempt to restore position failed ??");

                return true;
            }

            return false;
        }

        #endregion
    }
}
