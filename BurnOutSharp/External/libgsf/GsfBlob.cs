/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-blob.c: a chunk of data
 *
 * Copyright (C) 2006 Novell Inc
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
    // TODO: Can this be made internal?
    public class GsfBlob
    {
        #region Properties

        public long Size { get; private set; }

        public byte[] Data { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfBlob() { }

        /// <summary>
        /// Creates a new GsfBlob object to hold the specified data.  The blob can then
        /// be used as a facility for reference-counting for the data.  The data is
        /// copied internally, so the blob does not hold references to external chunks
        /// of memory.
        /// </summary>
        /// <param name="size">Size of the data in bytes.</param>
        /// <param name="data_to_copy">Data which will be copied into the blob, or null if <paramref name="size"/> is zero.</param>
        /// <param name="error">Location to store error, or null.</param>
        /// <returns>A newly-created GsfBlob, or null if the data could not be copied.</returns>
        public static GsfBlob Create(long size, byte[] data_to_copy, int dataPtr, ref Exception error)
        {
            if (!((size > 0 && data_to_copy != null) || (size == 0 && data_to_copy == null)))
                return null;
            if (error != null)
                return null;

            byte[] data;
            if (data_to_copy != null)
            {
                data = new byte[size];
                Array.Copy(data_to_copy, dataPtr, data, 0, size);
            }
            else
            {
                data = null;
            }
            
            return new GsfBlob
            {
                Size = size,
                Data = data,
            };
        }

        #endregion
    }
}
