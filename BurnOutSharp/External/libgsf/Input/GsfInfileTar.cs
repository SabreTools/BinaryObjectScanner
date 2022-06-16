/*
 * gsf-infile-tar.c :
 *
 * Copyright (C) 2008 Morten Welinder (terra@gnome.org)
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
 *
 * TODO:
 *   symlinks
 *   hardlinks
 *   weird headers
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibGSF.Input
{
    public class TarChild
    {
        public string Name { get; set; }

        public DateTime? ModTime { get; set; }

        /// <summary>
        /// The location of data
        /// </summary>
        public long Offset { get; set; }

        public long Length { get; set; }

        /// <summary>
        /// The directory object, or null for a data file
        /// </summary>
        public GsfInfileTar Dir { get; set; }
    }

    /// <summary>
    /// Tar header from POSIX 1003.1-1990.
    /// </summary>
    public class TarHeader
    {
        public byte[] Name { get; set; } = new byte[100];               /*   0 */

        public byte[] Mode { get; set; } = new byte[8];                 /* 100 (octal) */

        public byte[] UID { get; set; } = new byte[8];                  /* 108 (octal) */

        public byte[] GID { get; set; } = new byte[8];                  /* 116 (octal) */

        public byte[] Size { get; set; } = new byte[12];                /* 124 (octal) */

        public byte[] MTime { get; set; } = new byte[12];               /* 136 (octal) */

        public byte[] Chksum { get; set; } = new byte[8];               /* 148 (octal) */

        public byte TypeFlag { get; set; }                              /* 156 */

        public byte[] Linkname { get; set; } = new byte[100];           /* 157 */

        public byte[] Magic { get; set; } = new byte[6];                /* 257 */

        public byte[] Version { get; set; } = new byte[2];              /* 263 */

        public byte[] UName { get; set; } = new byte[32];               /* 265 */

        public byte[] GName { get; set; } = new byte[32];               /* 297 */

        public byte[] DevMajor { get; set; } = new byte[8];             /* 329 (octal) */

        public byte[] DevMinor { get; set; } = new byte[8];             /* 337 (octal) */

        public byte[] Prefix { get; set; } = new byte[155];             /* 345 */

        public byte[] Filler { get; set; } = new byte[12];              /* 500 */
    }

    public class GsfInfileTar : GsfInfile
    {
        #region Constants

        private const int HEADER_SIZE = 512; // sizeof(TarHeader);

        private const int BLOCK_SIZE = 512;

        private const string MAGIC_LONGNAME = "././@LongLink";

        #endregion

        #region Properties

        public GsfInput Source { get; set; } = null;

        public List<TarChild> Children { get; set; } = new List<TarChild>();

        public Exception Err { get; set; } = null;

        #endregion

        #region Constructor and Deconstructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInfileTar() { }

        /// <summary>
        /// Opens the root directory of a Tar file.
        /// </summary>
        /// <param name="source">A base GsfInput</param>
        /// <param name="err">An Exception, optionally null</param>
        /// <returns>The new tar file handler</returns>
        /// <remarks>This adds a reference to <paramref name="source"/>.</remarks>
        public static GsfInfileTar Create(GsfInput source, ref Exception err)
        {
            if (source == null)
                return null;

            GsfInfileTar tar = new GsfInfileTar
            {
                Source = GsfInputProxy.Create(source),
            };

            if (tar.Err != null)
            {
                err = tar.Err;
                return null;
            }

            tar.InitInfo();

            return tar;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfInfileTar()
        {
            Source = null;
            Err = null;
            Children.Clear();
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            if (Err != null)
            {
                err = Err;
                return null;
            }

            GsfInfileTar res = new GsfInfileTar
            {
                Source = Source,
            };

            for (int i = 0; i < Children.Count; i++)
            {
                // This copies the structure.
                TarChild c = new TarChild
                {
                    Name = Children[i].Name,
                    ModTime = Children[i].ModTime,
                    Offset = Children[i].Offset,
                    Length = Children[i].Length,
                    Dir = Children[i].Dir,
                };

                res.Children.Add(c);
            }

            return null;
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0) => null;

        /// <inheritdoc/>
        public override bool Seek(long offset, SeekOrigin whence) => false;

        /// <inheritdoc/>
        public override GsfInput ChildByIndex(int i, ref Exception error)
        {
            error = null;

            if (i < 0 || i >= Children.Count)
                return null;

            TarChild c = Children[i];
            if (c.Dir != null)
            {
                return c.Dir;
            }
            else
            {
                GsfInputProxy input = GsfInputProxy.Create(Source, c.Offset, c.Length);
                input.ModTime = c.ModTime;
                input.Name = c.Name;
                return input;
            }
        }

        /// <inheritdoc/>
        public override string NameByIndex(int i)
        {
            if (i < 0 || i >= Children.Count)
                return null;

            return Children[i].Name;
        }

        /// <inheritdoc/>
        public override GsfInput ChildByName(string name, ref Exception error)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                TarChild c = Children[i];
                if (name == c.Name)
                    return ChildByIndex(i, ref error);
            }

            return null;
        }

        /// <inheritdoc/>
        public override int NumChildren() => Children.Count;

        #endregion

        #region Utilities

        private long UnpackOctal(byte[] s, int len)
        {
            // Different specifications differ on what terminating characters
            // are allowed.  It doesn't hurt for us to allow both space and
            // NUL.
            if (len == 0 || (s[len - 1] != 0 && s[len - 1] != ' '))
            {
                Err = new Exception("Invalid tar header");
                return 0;
            }

            len--;

            long res = 0;
            int sPtr = 0; // s[0]
            while (len-- != 0)
            {
                byte c = s[sPtr++];
                if (c < '0' || c > '7')
                {
                    Err = new Exception("Invalid tar header");
                    return 0;
                }

                res = (res << 3) | (c - '0');
            }

            return res;
        }

        private GsfInfileTar CreateDirectory(string name)
        {
            TarChild c = new TarChild
            {
                Offset = 0,
                Length = 0,
                Name = name,
                ModTime = null,
                Dir = new GsfInfileTar
                {
                    Source = Source,
                    Name = name,
                }
            };

            // We set the source here, so gsf_infile_tar_constructor doesn't
            // start reading the tarfile recursively.
            Children.Add(c);

            return c.Dir;
        }

        private GsfInfileTar DirectoryForFile(string name, bool last)
        {
            GsfInfileTar dir = this;
            int s = 0; // name[0]

            while (true)
            {
                int s0 = s;

                // Find a directory component, if any.
                while (true)
                {
                    if (name[s] == 0)
                    {
                        if (last && s != s0)
                            break;
                        else
                            return dir;
                    }

                    // This is deliberately slash-only.
                    if (name[s] == '/')
                        break;

                    s++;
                }

                string dirname = name.Substring(s0, s - s0);
                while (name[s] == '/')
                {
                    s++;
                }

                if (dirname != ".")
                {
                    Exception err = null;
                    GsfInput subdir = ChildByName(dirname, ref err);
                    if (subdir != null)
                        dir = subdir is GsfInfileTar ? (GsfInfileTar)subdir : dir;
                    else
                        dir = dir.CreateDirectory(dirname);
                }
            }
        }

        /// <summary>
        /// Read tar headers and do some sanity checking
        /// along the way.
        /// </summary>
        private void InitInfo()
        {
            long pos0 = Source.CurrentOffset;
            string pending_longname = null;

            TarHeader header;
            TarHeader end = new TarHeader();

            byte[] headerBytes;
            byte[] endBytes = new byte[HEADER_SIZE];

            while (Err == null && (headerBytes = Source.Read(HEADER_SIZE, null)) != null)
            {
                header = new TarHeader();
                Array.Copy(headerBytes, 0, header.Name, 0, 100);
                Array.Copy(headerBytes, 100, header.Mode, 0, 8);
                Array.Copy(headerBytes, 108, header.UID, 0, 8);
                Array.Copy(headerBytes, 116, header.GID, 0, 8);
                Array.Copy(headerBytes, 124, header.Size, 0, 12);
                Array.Copy(headerBytes, 136, header.MTime, 0, 12);
                Array.Copy(headerBytes, 148, header.Chksum, 0, 8);
                header.TypeFlag = headerBytes[156];
                Array.Copy(headerBytes, 157, header.Linkname, 0, 100);
                Array.Copy(headerBytes, 257, header.Magic, 0, 6);
                Array.Copy(headerBytes, 263, header.Version, 0, 2);
                Array.Copy(headerBytes, 265, header.UName, 0, 32);
                Array.Copy(headerBytes, 297, header.GName, 0, 32);
                Array.Copy(headerBytes, 329, header.DevMajor, 0, 8);
                Array.Copy(headerBytes, 337, header.DevMinor, 0, 8);
                Array.Copy(headerBytes, 345, header.Prefix, 0, 155);
                Array.Copy(headerBytes, 500, header.Filler, 0, 12);

                if (header.Filler.Length != end.Filler.Length || !header.Filler.SequenceEqual(end.Filler))
                {
                    Err = new Exception("Invalid tar header");
                    break;
                }

                if (headerBytes.SequenceEqual(endBytes))
                    break;

                string name;
                if (pending_longname != null)
                {
                    name = pending_longname;
                    pending_longname = null;
                }
                else
                {
                    name = Encoding.UTF8.GetString(header.Name);
                }

                long length = UnpackOctal(header.Size, header.Size.Length);
                long offset = Source.CurrentOffset;

                long mtime = UnpackOctal(header.MTime, header.MTime.Length);

                switch (header.TypeFlag)
                {
                    case (byte)'0':
                    case 0:
                        {
                            // Regular file.
                            int n = 0, s; // name[0]

                            // This is deliberately slash-only.
                            while ((s = name.IndexOf('/', n)) != -1)
                            {
                                n = s + 1;
                            }

                            TarChild c = new TarChild
                            {
                                Name = name.Substring(n),
                                ModTime = mtime > 0 ? DateTimeOffset.FromUnixTimeSeconds(mtime).UtcDateTime : (DateTime?)null,
                                Offset = offset,
                                Length = length,
                                Dir = null,
                            };

                            GsfInfileTar dir = DirectoryForFile(name, false);
                            dir.Children.Add(c);
                            break;
                        }

                    case (byte)'5':
                        {
                            // Directory
                            DirectoryForFile(name, true);
                            break;
                        }

                    case (byte)'L':
                        {
                            if (pending_longname != null || name != MAGIC_LONGNAME)
                            {
                                Err = new Exception("Invalid longname header");
                                break;
                            }

                            byte[] n = Source.Read((int)length, null);
                            if (n == null)
                            {
                                Err = new Exception("Failed to read longname"); ;
                                break;
                            }

                            pending_longname = Encoding.UTF8.GetString(n);
                            break;
                        }

                    default:
                        // Other -- ignore
                        break;
                }

                // Round up to block size
                length = (length + (BLOCK_SIZE - 1)) / BLOCK_SIZE * BLOCK_SIZE;

                if (Err == null && Source.Seek(offset + length, SeekOrigin.Begin))
                {
                    Err = new Exception("Seek failed");
                    break;
                }
            }

            if (pending_longname != null)
            {
                if (Err == null)
                    Err = new Exception("Truncated archive");
            }

            if (Err != null)
                Source.Seek(pos0, SeekOrigin.Begin);
        }

        #endregion
    }
}
