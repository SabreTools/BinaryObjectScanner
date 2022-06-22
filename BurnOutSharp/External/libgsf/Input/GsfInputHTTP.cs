/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-http.c: retrieves input via HTTP
 *
 * Copyright (C) 2006 Michael Lawrence (lawremi@iastate.edu)
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
using System.Net;
using LibGSF.Output;

namespace LibGSF.Input
{
    public class GsfInputHTTP : GsfInput
    {
        #region Properties

        public string Url { get; private set; } = null;

        public string ContentType { get; private set; } = null;

        public Stream ResponseData { get; private set; } = null;

        public byte[] Buffer { get; private set; } = null;

        public int BufferSize { get; private set; } = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInputHTTP() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">A string containing the URL to retrieve</param>
        /// <param name="error">Holds any errors encountered when establishing the HTTP connection</param>
        /// <returns>An open HTTP connection, ready for reading.</returns>
        public static GsfInput Create(string url, ref Exception error)
        {
            if (url == null)
                return null;

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream ctx = response.GetResponseStream();

            string content_type = response.ContentType;

            // Always make a local copy
            // see https://bugzilla.gnome.org/show_bug.cgi?id=724970
            GsfInput input = MakeLocalCopy(ctx);
            if (input != null)
            {
                input.Name = url;
                return input;
            }

            GsfInputHTTP obj = new GsfInputHTTP
            {
                Url = url,
                ContentType = content_type,
                Size = ctx.Length,
                ResponseData = ctx,
            };

            return obj;
        }

        ~GsfInputHTTP()
        {
            Url = null;
            ContentType = null;

            if (ResponseData != null)
            {
                ResponseData.Close();
                ResponseData = null;
            }

            Buffer = null;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err) => Create(Url, ref err);

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            int nread;
            int total_read;

            if (optional_buffer == null)
            {
                if (BufferSize < num_bytes)
                {
                    BufferSize = num_bytes;
                    Buffer = new byte[BufferSize];
                }

                optional_buffer = Buffer;
            }

            int ptr = 0;
            for (total_read = 0; total_read < num_bytes; total_read += nread)
            {
                nread = ResponseData.Read(optional_buffer, ptr, num_bytes - total_read);
                if (nread <= 0)
                    return null;

                ptr += nread;
            }

            return optional_buffer;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => false;

        #endregion

        #region Utilities

        private static GsfInput MakeLocalCopy(Stream ctx)
        {
            GsfOutputMemory output = GsfOutputMemory.Create();

            GsfInput copy;
            while (true)
            {
                byte[] buf = new byte[4096];
                int nread;

                nread = ctx.Read(buf, 0, buf.Length);

                if (nread > 0)
                {
                    if (!output.Write(nread, buf))
                    {
                        copy = null;
                        output.Close();
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
                    return copy;
                }
            }

            copy = GsfInputMemory.Clone(output.Buffer, output.Capacity);
            output.Close();
            return copy;
        }

        #endregion
    }
}
