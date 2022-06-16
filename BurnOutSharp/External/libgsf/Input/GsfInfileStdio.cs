/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-infile-stdio.c: read a directory tree
 *
 * Copyright (C) 2004-2006 Novell, Inc.
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

namespace LibGSF.Input
{
    public class GsfInfileStdio : GsfInfile
    {
        #region Properties

        public string Root { get; set; } = null;

        public List<string> Children { get; set; } = new List<string>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInfileStdio() { }

        /// <param name="root">In locale dependent encoding</param>
        /// <param name="err">Optionally null</param>
        /// <returns>A new file or null.</returns>
        public static GsfInfileStdio Create(string root, ref Exception err)
        {
            if (!Directory.Exists(root))
                return null;

            GsfInfileStdio ifs = new GsfInfileStdio
            {
                Root = root,
            };

            foreach (string child in Directory.EnumerateFileSystemEntries(root))
            {
                ifs.Children.Add(child);
            }

            ifs.SetNameFromFilename(root);
            return ifs;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfInfileStdio()
        {
            Root = null;
            Children.Clear();
            Children = null;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            GsfInfileStdio dst = new GsfInfileStdio
            {
                Root = Root,
            };

            dst.Children.AddRange(Children);

            return dst;
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0) => null;

        /// <inheritdoc/>
        public override string NameByIndex(int i) => i < Children.Count ? Children[i] : null;

        /// <inheritdoc/>
        public override GsfInput ChildByIndex(int i, ref Exception error)
        {
            string name = NameByIndex(i);
            return name != null ? OpenChild(name, ref error) : null;
        }

        /// <inheritdoc/>
        public override GsfInput ChildByName(string name, ref Exception error)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                string child = Children[i];
                if (child.Equals(name))
                    return OpenChild(name, ref error);
            }

            return null;
        }

        /// <inheritdoc/>
        public override int NumChildren() => Children.Count;

        #endregion

        #region Utilities

        private GsfInput OpenChild(string name, ref Exception err)
        {
            string path = Path.Combine(Root, name);

            GsfInput child;
            if (Directory.Exists(path))
                child = GsfInfileStdio.Create(path, ref err);
            else
                child = GsfInputStdio.Create(path, ref err);

            return child;
        }

        #endregion
    }
}
