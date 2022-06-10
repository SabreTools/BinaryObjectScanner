/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-proxy.c: proxy object (with its own current position)
 *
 * Copyright (C) 2004-2006 Morten Welinder (terra@gnome.org)
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

namespace LibGSF.Input
{
    public class GsfInputProxy : GsfInput
    {
        #region Properties

        public GsfInput Source { get; set; } = null;

        public long Offset { get; set; } = 0;

        #endregion

        #region Functions

        /// <summary>
        /// This creates a new proxy to a section of the given source.  The new
        /// object will have its own current position, but any operation on it
        /// can change the source's position.
        /// 
        /// If a proxy to a proxy is created, the intermediate proxy is short-
        /// circuited.
        /// 
        /// This function will ref the source.
        /// </summary>
        /// <param name="source">The underlying data source.</param>
        /// <param name="offset">Offset into source for start of section.</param>
        /// <param name="size">Length of section.</param>
        /// <returns>A new input object.</returns>
        public static GsfInputProxy Create(GsfInput source, long offset, long size)
        {
            if (source == null)
                return null;
            if (offset < 0)
                return null;

            long source_size = source.Size;
            if (offset > source_size)
                return null;
            if (size > source_size - offset)
                return null;

            GsfInputProxy proxy = new GsfInputProxy
            {
                Offset = offset,
                Size = size,
                Name = source.Name,
            };

            // Short-circuit multiple proxies.
            if (source is GsfInputProxy proxy_source)
            {
                proxy.Offset += proxy_source.Offset;
                source = proxy_source.Source;
            }

            proxy.Source = source;
            return proxy;
        }

        /// <summary>
        /// This creates a new proxy to the entire, given input source.  See
        /// Create for details.
        /// </summary>
        /// <param name="source">The underlying data source.</param>
        /// <returns>A new input object.</returns>
        public static GsfInput Create(GsfInput source) => Create(source, 0, source.Size);

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err) => Create(this);

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            // Seek to our position in the source.
            if (Source.Seek(Offset + CurrentOffset, SeekOrigin.Begin))
                return null;

            // Read the data.
            return Source.Read(num_bytes, optional_buffer, bufferPtr);
        }

        /// <inheritdoc/>
        public override bool Seek(long offset, SeekOrigin whence) => false;

        #endregion
    }
}
