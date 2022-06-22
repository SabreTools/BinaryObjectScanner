/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-structured_blob.c : Utility storage to blob in/out a tree of data
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
using LibGSF.Output;

namespace LibGSF.Input
{
    public class GsfStructuredBlob : GsfInfile
    {
        #region Properties

        public GsfSharedMemory Data { get; set; } = null;

        public GsfStructuredBlob[] Children { get; set; } = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfStructuredBlob() { }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfStructuredBlob()
        {
            if (Data != null)
                Data = null;

            if (Children != null)
                Children = null;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            GsfStructuredBlob dst = new GsfStructuredBlob();

            if (Data != null)
                dst.Data = Data;

            if (Children != null)
            {
                dst.Children = new GsfStructuredBlob[Children.Length];
                Array.Copy(Children, dst.Children, Children.Length);
            }

            return dst;
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            byte[] src = Data.Buf;
            if (src == null)
                return null;

            if (optional_buffer == null)
                optional_buffer = new byte[num_bytes];

            Array.Copy(src, CurrentOffset, optional_buffer, 0, num_bytes);
            return optional_buffer;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => false;

        /// <inheritdoc/>
        public override int NumChildren() => Children != null ? Children.Length : -1;

        /// <inheritdoc/>
        public override string NameByIndex(int i)
        {
            if (Children == null)
                return null;

            if (i < 0 || i >= Children.Length)
                return null;

            return Children[i].Name;
        }

        /// <inheritdoc/>
        public override GsfInput ChildByIndex(int i, ref Exception error)
        {
            if (Children == null)
                return null;

            if (i < 0 || i >= Children.Length)
                return null;

            return Children[i];
        }

        /// <inheritdoc/>
        public override GsfInput ChildByName(string name, ref Exception error)
        {
            if (Children == null)
                return null;

            for (int i = 0; i < Children.Length; i++)
            {
                GsfInput child = Children[i];
                if (child != null && child.Name == name)
                    return child.Duplicate(ref error);
            }

            return null;
        }

        /// <summary>
        /// Create a tree of binary blobs with unknown content from a GsfInput or
        /// #GsfInfile and store it in a newly created #GsfStructuredBlob.
        /// </summary>
        /// <param name="input">An input (potentially a GsfInfile) holding the blob</param>
        /// <returns>A new GsfStructuredBlob object which the caller is responsible for.</returns>
        public static GsfStructuredBlob Read(GsfInput input)
        {
            if (input == null)
                return null;

            GsfStructuredBlob blob = new GsfStructuredBlob();
            long content_size = input.Remaining();
            if (content_size > 0)
            {
                byte[] buf = new byte[content_size];
                input.Read((int)content_size, buf);
                blob.Data = GsfSharedMemory.Create(buf, content_size, true);
            }

            blob.Name = input.Name;

            if (input is GsfInfile infile)
            {
                int i = infile.NumChildren();
                if (i > 0)
                {
                    Exception err = null;
                    blob.Children = new GsfStructuredBlob[i];
                    while (i-- > 0)
                    {
                        GsfInput child = infile.ChildByIndex(i, ref err);
                        GsfStructuredBlob child_blob = null;
                        if (child != null)
                            child_blob = Read(child);

                        blob.Children[i] = child_blob;
                    }
                }
            }

            return blob;
        }

        /// <summary>
        /// Dumps structured blob @blob onto the <paramref name="container"/>.  Will fail if the output is
        /// not an Outfile and blob has multiple streams.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Write(GsfOutfile container)
        {
            if (container == null)
                return false;

            bool has_kids = (Children != null && Children.Length > 0);
            GsfOutput output = container.NewChild(Name, has_kids);
            if (has_kids)
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    GsfStructuredBlob child_blob = Children[i];
                    if (!child_blob.Write(output as GsfOutfile))
                        return false;
                }
            }

            if (Data != null)
                output.Write((int)Data.Size, Data.Buf);

            output.Close();
            return true;
        }

        #endregion
    }
}
