/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-outfile-stdio.c: A directory tree wrapper for Outfile
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
using System.IO;

namespace LibGSF.Output
{
    public class GsfOutfileStdio : GsfOutfile
    {
        #region Properties

        public string Root { get; set; } = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutfileStdio() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root">Root directory in utf8.</param>
        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>A new outfile or null.</returns>
        public static GsfOutfileStdio Create(string root, ref Exception err)
        {
            if (!Directory.Exists(root))
            {
                try
                {
                    Directory.CreateDirectory(root);
                }
                catch (Exception ex)
                {
                    err = new Exception($"{root}: {ex.Message}");
                    return null;
                }
            }

            GsfOutfileStdio ofs = new GsfOutfileStdio
            {
                Root = root,
            };

            ofs.SetNameFromFilename(root);
            return ofs;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        public override GsfOutput NewChild(string name, bool is_dir)
        {
            string path = Path.Combine(Root, name);

            Exception err = null;

            GsfOutput child;
            if (is_dir)
                child = Create(path, ref err);
            else
                child = GsfOutputStdio.Create(path, ref err);

            return child;
        }

        /// <inheritdoc/>
        protected override bool CloseImpl() => true;

        #endregion
    }
}
