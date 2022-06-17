/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-infile-zip.c :
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
 *                    	   Tambet Ingo   (tambet@ximian.com)
 * Copyright (C) 2014 Morten Welinder (terra@gnome.org)
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
using static LibGSF.GsfUtils;
using static LibGSF.GsfZipImpl;

namespace LibGSF.Input
{
    // TODO: Can this be made internal?
    public class ZipInfo
    {
        #region Properties

        public uint Entries { get; set; }

        public long DirPos { get; set; }

        public List<GsfZipDirectoryEntry> DirectoryEntries { get; set; }

        public GsfZipVDir VDir { get; set; }

        public int RefCount { get; set; }

        #endregion

        #region Functions

        public ZipInfo Ref()
        {
            RefCount++;
            return this;
        }

        public void Unref()
        {
            if (RefCount-- != 1)
                return;

            VDir.Free(false);
            for (int i = 0; i < DirectoryEntries.Count; i++)
            {
                DirectoryEntries[i].Free();
            }

            DirectoryEntries.Clear();
            DirectoryEntries = null;
        }

        #endregion
    }

    public class GsfInfileZip : GsfInfile
    {
        #region Properties

        public GsfInput Source { get; set; } = null;

        public ZipInfo Info { get; set; } = null;

        public bool Zip64 { get; set; } = false;

        public GsfZipVDir VDir { get; set; } = null;

        public ZStream Stream { get; set; } = null;

        public long RestLen { get; set; } = 0;

        public long CRestLen { get; set; } = 0;

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;

        public long SeekSkipped { get; set; } = 0;

        public Exception Err { get; set; } = null;

        public GsfInfileZip DupParent { get; set; }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInfileZip(GsfInfileZip dupParent = null)
        {
            DupParent = dupParent;
            if (DupParent != null)
            {
                // Special call from PartiallyDuplicate.
                Exception err = null;
                Source = DupParent.Source.Duplicate(ref err);
                Err = err;

                Info = DupParent.Info.Ref();
                Zip64 = DupParent.Zip64;
                DupParent = null;
            }
            else
            {
                if (!InitInfo())
                    VDir = Info.VDir;
            }
        }

        /// <summary>
        /// Opens the root directory of a Zip file.
        /// </summary>
        /// <param name="source">A base GsfInput</param>
        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>The new zip file handler</returns>
        /// <remarks>This adds a reference to <paramref name="source"/>.</remarks>
        public static GsfInfileZip Create(GsfInput source, ref Exception err)
        {
            if (source == null)
                return null;

            GsfInfileZip zip = new GsfInfileZip
            {
                Source = source,
            };

            if (zip.Err != null)
            {
                err = zip.Err;
                return null;
            }

            return zip;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfInfileZip()
        {
            if (Info != null)
            {
                Info.Unref();
                Info = null;
            }

            if (Stream != null)
            {
                Stream.inflateEnd();
                Stream = null;
            }

            Buf = null;

            SetSource(null);
            Err = null;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err)
        {
            GsfInfileZip dst = PartiallyDuplicate(ref err);
            if (dst == null)
                return null;

            dst.VDir = VDir;

            if (dst.VDir.DirectoryEntry != null && dst.ChildInit(ref err))
                return null;

            return dst;
        }

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            GsfZipVDir vdir = VDir;
            long pos;

            if (RestLen < num_bytes)
                return null;

            switch (vdir.DirectoryEntry.CompressionMethod)
            {
                case GsfZipCompressionMethod.GSF_ZIP_STORED:
                    RestLen -= num_bytes;
                    pos = VDir.DirectoryEntry.DataOffset + CurrentOffset;
                    if (Source.Seek(pos, SeekOrigin.Begin))
                        return null;

                    return Source.Read(num_bytes, optional_buffer);

                case GsfZipCompressionMethod.GSF_ZIP_DEFLATED:
                    if (optional_buffer == null)
                    {
                        if (BufSize < num_bytes)
                        {
                            BufSize = Math.Max(num_bytes, 256);
                            Buf = new byte[BufSize];
                        }

                        optional_buffer = Buf;
                    }

                    Stream.avail_out = num_bytes;
                    Stream.next_out = optional_buffer;

                    do
                    {
                        int err;
                        long startlen;

                        if (CRestLen > 0 && Stream.avail_in == 0)
                            if (!UpdateStreamInput())
                                break;

                        startlen = Stream.total_out;
                        err = Stream.inflate(Z_NO_FLUSH);

                        if (err == Z_STREAM_END)
                            RestLen = 0;
                        else if (err == Z_OK)
                            RestLen -= (Stream.total_out - startlen);
                        else
                            return null;  // Error, probably corrupted

                    } while (RestLen != 0 && Stream.avail_out != 0);

                    return optional_buffer;

                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Global flag -- we don't want one per stream.
        /// </summary>
        private static bool warned = false;

        /// <inheritdoc/>
        public override bool Seek(long offset, SeekOrigin whence)
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

            if (Stream != null)
            {
                Stream.inflateEnd();
                Stream = new ZStream();
            }

            Exception err = null;
            if (ChildInit(ref err))
            {
                Console.Error.WriteLine("Failure initializing zip child");
                return true;
            }

            CurrentOffset = 0;
            if (SeekEmulate(pos))
                return true;

            SeekSkipped += pos;
            if (!warned && SeekSkipped != pos && SeekSkipped >= 1000000)
            {
                warned = true;
                Console.Error.WriteLine("Seeking in zip child streams is awfully slow.");
            }

            return false;
        }

        /// <inheritdoc/>
        public override GsfInput ChildByIndex(int i, ref Exception error)
        {
            GsfZipVDir child_vdir = VDir.ChildByIndex(i);
            if (child_vdir != null)
                return NewChild(child_vdir, ref error);

            return null;
        }

        /// <inheritdoc/>
        public override string NameByIndex(int i)
        {
            GsfZipVDir child_vdir = VDir.ChildByIndex(i);
            if (child_vdir != null)
                return child_vdir.Name;

            return null;
        }

        /// <inheritdoc/>
        public override GsfInput ChildByName(string name, ref Exception error)
        {
            GsfZipVDir child_vdir = VDir.ChildByName(name);
            if (child_vdir != null)
                return NewChild(child_vdir, ref error);

            return null;
        }

        /// <inheritdoc/>
        public override int NumChildren()
        {
            if (VDir == null)
                return -1;

            if (!VDir.IsDirectory)
                return -1;

            return VDir.Children.Count;
        }

        #endregion

        #region Utilities

        private static DateTime? MakeModTime(uint dostime)
        {
            if (dostime == 0)
            {
                return null;
            }
            else
            {
                int year = (int)(dostime >> 25) + 1980;
                int month = (int)(dostime >> 21) & 0x0f;
                int day = (int)(dostime >> 16) & 0x1f;
                int hour = (int)(dostime >> 11) & 0x0f;
                int minute = (int)(dostime >> 5) & 0x3f;
                int second = (int)(dostime & 0x1f) * 2;

                DateTime modtime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

                return modtime;
            }
        }

        private long FindTrailer(uint sig, int size)
        {
            byte sig1 = (byte)(sig & 0xff);

            long filesize = Source.Size;
            if (filesize < size)
                return -1;

            long trailer_offset = filesize;
            long maplen = filesize & (ZIP_BUF_SIZE - 1);
            if (maplen == 0)
                maplen = ZIP_BUF_SIZE;

            long offset = filesize - maplen; // Offset is now BUFSIZ aligned

            while (true)
            {
                if (Source.Seek(offset, SeekOrigin.Begin))
                    return -1;

                byte[] data = Source.Read((int)maplen, null);
                if (data == null)
                    return -1;

                int p = 0; // data[0]

                for (int s = (int)(p + maplen - 1); (s >= p); s--, trailer_offset--)
                {
                    if (data[s] == sig1 && p + maplen - 1 - s > size - 2 && GSF_LE_GET_GUINT32(data, s) == sig)
                        return --trailer_offset;
                }

                // Not found in currently mapped block, so update it if
                // there is some room in before. The requirements are..
                // (a) mappings should overlap so that trailer can cross BUFSIZ-boundary
                // (b) trailer cannot be farther away than 64K from fileend

                // Outer loop cond
                if (offset <= 0)
                    return -1;

                // Outer loop step
                offset -= ZIP_BUF_SIZE / 2;
                maplen = Math.Min(filesize - offset, ZIP_BUF_SIZE);
                trailer_offset = offset + maplen;

                if (filesize - offset > 64 * 1024)
                    return -1;
            }
        }

        private static byte[] DirectoryEntryExtraField(byte[] extra, int extraPtr, int elen, ExtraFieldTags typ, out uint pflen)
        {
            while (true)
            {
                if (elen == 0)
                {
                    pflen = 0;
                    return null;
                }

                if (elen < 4)
                {
                    pflen = 0;
                    return null;
                }

                ExtraFieldTags ftyp = (ExtraFieldTags)GSF_LE_GET_GUINT16(extra, extraPtr);
                uint flen = GSF_LE_GET_GUINT16(extra, extraPtr + 2);
                if (flen > elen - 4)
                {
                    pflen = 0;
                    return null;
                }

                extraPtr += 4;
                elen -= 4;
                if (ftyp == typ)
                {
                    // Found the extended data.
                    pflen = flen;
                    return extra;
                }

                extraPtr += (int)flen;
                elen -= (int)flen;
            }
        }

        private GsfZipDirectoryEntry NewDirectoryEntry(ref long offset)
        {
            byte[] header = new byte[ZIP_DIRENT_SIZE];

            // Read fixed-length part of data and check the header
            byte[] data = header;
            if (Source.Seek(offset, SeekOrigin.Begin)
                || Source.Read(ZIP_DIRENT_SIZE, header) != null
                || GSF_LE_GET_GUINT32(data, 0) != ZIP_DIRENT_SIGNATURE)
            {
                return null;
            }

            ushort name_len    = GSF_LE_GET_GUINT16(header, ZIP_DIRENT_NAME_SIZE);
            ushort extras_len  = GSF_LE_GET_GUINT16(header, ZIP_DIRENT_EXTRAS_SIZE);
            ushort comment_len = GSF_LE_GET_GUINT16(header, ZIP_DIRENT_COMMENT_SIZE);
            int vlen = name_len + extras_len + comment_len;

            // Read variable part
            byte[] variable = Source.Read(vlen, null);
            if (variable == null && vlen > 0)
                return null;

            byte[] extra = DirectoryEntryExtraField(variable, name_len, extras_len, ExtraFieldTags.ZIP_DIRENT_EXTRA_FIELD_ZIP64, out uint elen);
            int extraPtr = 0; // extra[0];
            bool zip64 = (extra != null);

            uint flags                                  = GSF_LE_GET_GUINT32(header, ZIP_DIRENT_FLAGS);
            GsfZipCompressionMethod compression_method  = (GsfZipCompressionMethod)GSF_LE_GET_GUINT16(header, ZIP_DIRENT_COMPR_METHOD);
            uint dostime                                = GSF_LE_GET_GUINT32(header, ZIP_DIRENT_DOSTIME);
            uint crc32                                  = GSF_LE_GET_GUINT32(header, ZIP_DIRENT_CRC32);
            ulong csize                                 = GSF_LE_GET_GUINT32(header, ZIP_DIRENT_CSIZE);
            ulong usize                                 = GSF_LE_GET_GUINT32(header, ZIP_DIRENT_USIZE);
            ulong off                                   = GSF_LE_GET_GUINT32(header, ZIP_DIRENT_OFFSET);
            uint disk_start                             = GSF_LE_GET_GUINT16(header, ZIP_DIRENT_DISKSTART);

            if (usize == 0xffffffffu && elen >= 8)
            {
                usize = GSF_LE_GET_GUINT64(extra, extraPtr);
                extraPtr += 8;
                elen -= 8;
            }
            if (csize == 0xffffffffu && elen >= 8)
            {
                csize = GSF_LE_GET_GUINT64(extra, extraPtr);
                extraPtr += 8;
                elen -= 8;
            }
            if (off == 0xffffffffu && elen >= 8)
            {
                off = GSF_LE_GET_GUINT64(extra, extraPtr);
                extraPtr += 8;
                elen -= 8;
            }
            if (disk_start == 0xffffu && elen >= 4)
            {
                disk_start = GSF_LE_GET_GUINT32(extra, extraPtr);
                extraPtr += 4;
                elen -= 4;
            }

            byte[] name = new byte[name_len + 1];
            Array.Copy(variable, name, name_len);
            name[name_len] = 0x00;

            GsfZipDirectoryEntry dirent = GsfZipDirectoryEntry.Create();
            dirent.Name = Encoding.UTF8.GetString(name);

            dirent.Flags                = (int)flags;
            dirent.CompressionMethod    = compression_method;
            dirent.CRC32                = crc32;
            dirent.CompressedSize       = (long)csize;
            dirent.UncompressedSize     = (long)usize;
            dirent.Offset               = (long)off;
            dirent.DosTime              = dostime;
            dirent.Zip64                = zip64;

            //g_print("%s = 0x%x @ %" GSF_OFF_T_FORMAT "\n", name, off, *offset);

            offset += ZIP_DIRENT_SIZE + vlen;

            return dirent;
        }

        /// <summary>
        /// Returns a partial duplicate.
        /// </summary>
        private GsfInfileZip PartiallyDuplicate(ref Exception err)
        {
            GsfInfileZip dst = new GsfInfileZip(this);

            if (dst.Err != null)
            {
                err = dst.Err;
                return null;
            }

            return dst;
        }

        /// <summary>
        /// Read zip headers and do some sanity checking
        /// along the way.
        /// </summary>
        /// <returns>True on error setting Err.</returns>
        private bool ReadDirectoryEntries()
        {
            // Find and check the trailing header
            long offset = FindTrailer(ZIP_TRAILER_SIGNATURE, ZIP_TRAILER_SIZE);
            if (offset < ZIP_ZIP64_LOCATOR_SIZE || Source.Seek(offset - ZIP_ZIP64_LOCATOR_SIZE, SeekOrigin.Begin))
            {
                Err = new Exception("Broken zip file structure");
                return true;
            }

            byte[] locator = Source.Read(ZIP_TRAILER_SIZE + ZIP_ZIP64_LOCATOR_SIZE, null);
            if (locator == null)
            {
                Err = new Exception("Broken zip file structure");
                return true;
            }

            int data = ZIP_ZIP64_LOCATOR_SIZE; // locator[0] + ZIP_ZIP64_LOCATOR_SIZE

            ulong entries = GSF_LE_GET_GUINT16(locator, data + ZIP_TRAILER_ENTRIES);
            ulong dir_pos = GSF_LE_GET_GUINT32(locator, data + ZIP_TRAILER_DIR_POS);

            if (GSF_LE_GET_GUINT32(locator, 0) == ZIP_ZIP64_LOCATOR_SIGNATURE)
            {
                Zip64 = true;

                data = 0; // locator[0]
                uint disk = GSF_LE_GET_GUINT32(locator, data + ZIP_ZIP64_LOCATOR_DISK);
                long zip64_eod_offset = (long)GSF_LE_GET_GUINT64(locator, data + ZIP_ZIP64_LOCATOR_OFFSET);
                uint disks = GSF_LE_GET_GUINT32(locator, data + ZIP_ZIP64_LOCATOR_DISKS);

                if (disk != 0 || disks != 1)
                {
                    Err = new Exception("Broken zip file structure");
                    return true;
                }

                if (Source.Seek((int)zip64_eod_offset, SeekOrigin.Begin))
                {
                    Err = new Exception("Broken zip file structure");
                    return true;
                }

                locator = Source.Read(ZIP_TRAILER64_SIZE, null);
                if (locator == null || GSF_LE_GET_GUINT32(locator, data) != ZIP_TRAILER64_SIGNATURE)
                {
                    Err = new Exception("Broken zip file structure");
                    return true;
                }

                entries = GSF_LE_GET_GUINT64(locator, data + ZIP_TRAILER64_ENTRIES);
                dir_pos = GSF_LE_GET_GUINT64(locator, data + ZIP_TRAILER64_DIR_POS);
            }

            Info = new ZipInfo()
            {
                DirectoryEntries    = new List<GsfZipDirectoryEntry>(),
                RefCount            = 1,
                Entries             = (uint)entries,
                DirPos              = (long)dir_pos,
            };

            // Read the directory
            uint i = 0;
            for (offset = (long)dir_pos; i < entries; i++)
            {
                GsfZipDirectoryEntry d = NewDirectoryEntry(ref offset);
                if (d == null)
                {
                    Err = new Exception("Error reading zip dirent");
                    return true;
                }

                Info.DirectoryEntries.Add(d);
            }

            return false;
        }

        private void BuildVirtualDirectories()
        {
            Info.VDir = GsfZipVDir.Create(string.Empty, true, null);
            for (int i = 0; i < Info.DirectoryEntries.Count; i++)
            {
                GsfZipDirectoryEntry dirent = Info.DirectoryEntries[i];
                Info.VDir.Insert(dirent.Name, dirent);
            }
        }

        /// <summary>
        /// Read zip headers and do some sanity checking
        /// along the way.
        /// </summary>
        /// <returns>True on error setting Err.</returns>
        private bool InitInfo()
        {
            bool ret = ReadDirectoryEntries();
            if (ret != false)
                return ret;

            BuildVirtualDirectories();

            return false;
        }

        /// <returns>Returns true on error</returns>
        private bool ChildInit(ref Exception errmsg)
        {
            byte[] data = null;

            GsfZipDirectoryEntry dirent = VDir.DirectoryEntry;

            // Skip local header
            // Should test tons of other info, but trust that those are correct

            string err = null;
            if (Source.Seek(dirent.Offset, SeekOrigin.Begin))
            {
                err = "Error seeking to zip header";
            }
            else if ((data = Source.Read(ZIP_HEADER_SIZE, null)) == null)
            {
                err = "Error reading zip header";
            }
            else if (GSF_LE_GET_GUINT32(data, 0) != ZIP_HEADER_SIGNATURE)
            {
                err = "Error incorrect zip header";
                Console.Error.WriteLine($"Header is 0x{GSF_LE_GET_GUINT32(data, 0):x}");
                Console.Error.WriteLine($"Expected 0x{ZIP_HEADER_SIGNATURE:x}");
            }

            if (err != null)
            {
                errmsg = new Exception(err);
                return true;
            }

            uint name_len =   GSF_LE_GET_GUINT16(data, ZIP_HEADER_NAME_SIZE);
            uint extras_len = GSF_LE_GET_GUINT16(data, ZIP_HEADER_EXTRAS_SIZE);

            dirent.DataOffset = dirent.Offset + ZIP_HEADER_SIZE + name_len + extras_len;
            RestLen =  dirent.UncompressedSize;
            CRestLen = dirent.CompressedSize;

            if (dirent.CompressionMethod != GsfZipCompressionMethod.GSF_ZIP_STORED)
            {
                if (Stream == null)
                    Stream = new ZStream();

                int errno = Stream.inflateInit(-15);
                if (errno != Z_OK)
                {
                    errmsg = new Exception("Problem uncompressing stream");
                    return true;
                }
            }

            return false;
        }

        private bool UpdateStreamInput()
        {
            if (CRestLen == 0)
                return false;

            uint read_now = (uint)Math.Min(CRestLen, ZIP_BLOCK_SIZE);

            long pos = VDir.DirectoryEntry.DataOffset + Stream.total_in;
            if (Source.Seek(pos, SeekOrigin.Begin))
                return false;

            byte[] data = Source.Read((int)read_now, null);
            if (data == null)
                return false;

            CRestLen -= read_now;
            Stream.next_in = data;    // next input byte
            Stream.avail_in = (int)read_now;  // number of bytes available at next_in

            return true;
        }

        private GsfInput NewChild(GsfZipVDir vdir, ref Exception err)
        {
            GsfZipDirectoryEntry dirent = vdir.DirectoryEntry;
            GsfInfileZip child = PartiallyDuplicate(ref err);

            if (child == null)
                return null;

            child.Name = vdir.Name;
            child.Container = this;
            child.VDir = vdir;

            if (dirent != null)
            {
                child.Size = dirent.UncompressedSize;
                if (dirent.DosTime != 0)
                {
                    DateTime? modtime = MakeModTime(dirent.DosTime);
                    child.ModTime = modtime;
                }

                if (child.ChildInit(ref err) != false)
                    return null;
            }
            else
            {
                child.Size = 0;
            }

            return child;
        }

        private void SetSource(GsfInput src)
        {
            if (src != null)
                src = GsfInputProxy.Create(src);

            Source = src;
        }

        #endregion
    }
}
