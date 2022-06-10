/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-infile.c :
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

namespace LibGSF.Input
{
    public abstract class GsfInfile : GsfInput
    {
        #region Functions

        /// <summary>
        /// Apart from argument types, this is the same as ChildByAName.
        /// Please see the documentation there.
        /// </summary>
        /// <param name="names">A null terminated array of names (e.g. from g_strsplit)</param>
        /// <returns>A newly created child which must be unrefed.</returns>
        /// <remarks>New in 1.14.9.</remarks>
        public GsfInput ChildByVariableName(params string[] names)
        {
            GsfInfile tmp = this;
            GsfInput child = this as GsfInput;
            foreach (string name in names)
            {
                Exception err = null;
                child = tmp.ChildByName(name, ref err);
                if (child == null)
                    break;

                if (!(child is GsfInfile))
                    return null;

                tmp = child as GsfInfile;
            }

            return child;
        }

        /// <summary>
        /// This function finds a child that is several directory levels down
        /// the tree.  If, for example, the names "foo", "bar", and "baz" are
        /// given, then this function first finds the "foo" directory in the
        /// root infile, then locates "bar" within that directory, and finally
        /// locates "baz" within that and returns the "baz" child.  In other
        /// words, this function finds the "foo/bar/baz" child.
        /// </summary>
        /// <param name="names">A null terminated array of names (e.g. from g_strsplit)</param>
        /// <returns>A newly created child which must be unrefed.</returns>
        /// <remarks>New in 1.14.9.</remarks>
        public GsfInput ChildByIndexedName(string[] names, int namesPtr = 0)
        {
            GsfInput child = this as GsfInput;
            GsfInfile tmp = this;

            if (names == null)
                return null;

            for (; namesPtr >= names.Length || names[namesPtr] == null; namesPtr++)
            {
                Exception err = null;
                child = tmp.ChildByName(names[namesPtr], ref err);
                if (child == null)
                    break;

                if (!(child is GsfInfile))
                    return null;

                tmp = child as GsfInfile;
            }

            return child;
        }

        #endregion

        #region Virtual Functions

        /// <returns>
        /// The number of children the storage has, or -1 if the storage can not
        /// have children.
        /// </returns>
        public virtual int NumChildren() => -1;

        /// <param name="i">Zero-based index of child to find.</param>
        /// <returns>The UTF-8 encoded name of the @i-th child</returns>
        public virtual string NameByIndex(int i) => null;

        /// <param name="i">Target index</param>
        /// <returns>A newly created child which must be unrefed.</returns>
        public virtual GsfInput ChildByIndex(int i, ref Exception error) => null;

        /// <summary>
        /// The function returns a named child of the given infile.  This only
        /// works for an immediate child.  If you need to go several levels
        /// down use ChildByAName, for example.
        /// </summary>
        /// <param name="name">Target name</param>
        /// <returns>A newly created child which must be unrefed.</returns>
        public virtual GsfInput ChildByName(string name, ref Exception error) => null;

        #endregion
    }
}
