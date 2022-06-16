/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-outfile-zip.c: zip archive output.
 *
 * Copyright (C) 2002-2006 Jon K Hellan (hellan@acm.org)
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
 * Foundation, Outc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
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

namespace LibGSF.Output
{
    public class GsfOutfileZip : GsfOutfile
    {
        #region Constants

        public static readonly bool? ZIP_CREATE_DEFAULT_ZIP64 = null;

        public const bool ZIP_ADD_UNIXTIME_FIELD = true;

        #endregion

        #region Properties

        public GsfOutput Sink { get; set; } = null;

        public GsfOutfileZip Root { get; set; } = null;

        public bool? SinkIsSeekable { get; set; } = null;

        public bool? Zip64 { get; set; } = ZIP_CREATE_DEFAULT_ZIP64;

        public string EntryName { get; set; } = null;

        public GsfZipVDir VDir { get; set; } = null;

        /// <summary>
        /// Only valid for the root, ordered
        /// </summary>
        public List<GsfOutfileZip> RootOrder { get; set; } = null;

        public ZStream Stream { get; set; } = null;

        public GsfZipCompressionMethod CompressionMethod { get; set; } = GsfZipCompressionMethod.GSF_ZIP_DEFLATED;

        public int DeflateLevel { get; set; } = Z_DEFAULT_COMPRESSION;

        public bool Writing { get; set; } = false;

        public byte[] Buf { get; set; } = null;

        public int BufSize { get; set; } = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutfileZip() { }

        /// <summary>
        /// Creates the root directory of a Zip file and manages the addition of
        /// children.
        /// </summary>
        /// <param name="sink">A GsfOutput to hold the ZIP file</param>
        /// <param name="err">Location to store error, or null; currently unused.</param>
        /// <returns>The new zip file handler</returns>
        /// <remarks>This adds a reference to <paramref name="sink"/>.</remarks>
        public static GsfOutfileZip Create(GsfOutput sink, ref Exception err)
        {
            if (sink == null)
                return null;

            err = null;
            GsfOutfileZip zip = new GsfOutfileZip { Sink = sink };
            if (zip.EntryName == null)
            {
                zip.VDir = GsfZipVDir.Create(string.Empty, true, null);
                zip.RootOrder = new List<GsfOutfileZip>();
                zip.Root = zip;

                // The names are the same
                zip.Name = zip.Sink.Name;
                zip.Container = null;
            }

            if (zip.ModTime == null)
            {
                DateTime? modtime = DateTime.UtcNow;
                zip.ModTime = modtime;
            }

            return zip;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfOutfileZip()
        {
            // If the closing failed, we might have stuff here.
            DisconnectChildren();

            if (Sink != null)
                SetSink(null);

            if (Stream != null)
                Stream.deflateEnd();

            if (this == Root)
                VDir.Free(true); // Frees vdirs recursively
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => false;

        /// <inheritdoc/>
        protected override bool CloseImpl()
        {
            bool ret;

            // The root dir
            if (this == Root)
            {
                ret = CloseRoot();
            }
            else if (VDir.IsDirectory)
            {
                // Directories: Do nothing. Should change this to actually
                // write dirs which don't have children.
                ret = true;
            }
            else
            {
                ret = CloseStream();
            }

            return ret;
        }

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
            if (VDir == null)
                return false;
            if (VDir.IsDirectory)
                return false;
            if (data == null)
                return false;

            int ret;

            if (!Writing)
            {
                if (!InitWrite())
                    return false;
            }

            GsfZipDirectoryEntry dirent = VDir.DirectoryEntry;

            if (dirent.Zip64 == false &&
                (num_bytes >= uint.MaxValue || CurrentOffset >= (long)(uint.MaxValue - num_bytes)))
            {
                // Uncompressed size field would overflow.
                return false;
            }

            if (CompressionMethod == GsfZipCompressionMethod.GSF_ZIP_DEFLATED)
            {
                Stream.next_in = data;
                Stream.avail_in = num_bytes;

                while (Stream.avail_in > 0)
                {
                    if (Stream.avail_out == 0)
                    {
                        if (!OutputBlock())
                            return false;
                    }

                    ret = Stream.deflate(Z_NO_FLUSH);
                    if (ret != Z_OK)
                        return false;
                }
            }
            else
            {
                if (!Sink.Write(num_bytes, data))
                    return false;

                dirent.CompressedSize += num_bytes;
            }

            // TODO: Enable CRC32 calculation
            //dirent.CRC32 = crc32(dirent.crc32, data, num_bytes);
            dirent.UncompressedSize += num_bytes;

            return true;
        }

        public void RootRegisterChild(GsfOutfileZip child)
        {
            child.Root = this;
            if (!child.VDir.IsDirectory)
                Root.RootOrder.Add(child);
        }

        public void SetSink(GsfOutput sink)
        {
            Sink = sink;
            SinkIsSeekable = null;
        }

        public override GsfOutput NewChild(string name, bool is_dir)
        {
            if (VDir == null)
                return null;
            if (!VDir.IsDirectory)
                return null;
            if (string.IsNullOrEmpty(name))
                return null;

            GsfOutfileZip child = new GsfOutfileZip
            {
                Name = name,
                Zip64 = this.Zip64,
                VDir = GsfZipVDir.Create(name, is_dir, null),
                Container = this,
            };

            if (child.EntryName == null)
            {
                child.VDir = GsfZipVDir.Create(string.Empty, true, null);
                child.RootOrder = new List<GsfOutfileZip>();
                child.Root = child;

                // The names are the same
                child.Name = child.Sink.Name;
                child.Container = null;
            }

            if (child.ModTime == null)
            {
                DateTime? modtime = DateTime.UtcNow;
                child.ModTime = modtime;
            }

            VDir.AddChild(child.VDir);
            Root.RootRegisterChild(child);

            return child;
        }

        #endregion

        #region Utilities

        private void DisconnectChildren()
        {
            if (RootOrder == null)
                return;

            for (int i = 0; i < RootOrder.Count; i++)
            {
                RootOrder[i] = null;
            }

            RootOrder = null;
        }

        /// <summary>
        /// The "mimetype" member is special for ODF.  It cannot have any
        /// extra field (and thus cannot be a zip64 member).  Hardcode
        /// this to help compatibility with libgsf users depending on
        /// past behaviour of zip creation.
        /// 
        /// The flip side is that such a file cannot be 4G+.
        /// </summary>
        /// <returns></returns>
        private static bool SpecialMimetypeDirectoryEntry(GsfZipDirectoryEntry dirent)
        {
            return (dirent.Offset == 0
                && dirent.Zip64 != true
                && dirent.CompressionMethod == GsfZipCompressionMethod.GSF_ZIP_STORED
                && dirent.Name == "mimetype");
        }

        private bool DirectoryEntryWrite(GsfZipDirectoryEntry dirent)
        {
            int nlen = dirent.Name.Length;
            List<byte> extras = new List<byte>(ZIP_DIRENT_SIZE + nlen + 100);
            bool offset_in_zip64 = dirent.Offset >= uint.MaxValue;
            bool zip64_here = (dirent.Zip64 == true || offset_in_zip64);
            byte extract = (byte)(zip64_here ? 45 : 20);  // Unsure if dirent.zip64 is enough

            if (zip64_here)
            {
                byte[] tmp = new byte[8];

                // We could unconditionally store the offset here, but
                // zipinfo has a known bug in which it fails to account
                // for differences in extra fields between the global
                // and the local headers.  So we try to make them the
                // same.

                GSF_LE_SET_GUINT16(tmp, 0, (ushort)ExtraFieldTags.ZIP_DIRENT_EXTRA_FIELD_ZIP64);
                GSF_LE_SET_GUINT16(tmp, 2, (ushort)(2 + (offset_in_zip64 ? 1 : 0) * 8));
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 4).ToArray());
                GSF_LE_SET_GUINT64(tmp, 0, (ulong)dirent.UncompressedSize);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 8).ToArray());
                GSF_LE_SET_GUINT64(tmp, 0, (ulong)dirent.CompressedSize);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 8).ToArray());
                if (offset_in_zip64)
                {
                    GSF_LE_SET_GUINT64(tmp, 0, (ulong)dirent.Offset);
                    extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 8).ToArray());
                }
            }
            else if (dirent.Zip64 == null)
            {
                byte[] tmp = new byte[8];

                // Match the local header.
                GSF_LE_SET_GUINT16(tmp, 0, (ushort)ExtraFieldTags.ZIP_DIRENT_EXTRA_FIELD_IGNORE);
                GSF_LE_SET_GUINT16(tmp, 2, 2 * 8);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 4).ToArray());
                GSF_LE_SET_GUINT64(tmp, 0, 0);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 8).ToArray());
                GSF_LE_SET_GUINT64(tmp, 0, 0);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 8).ToArray());
            }

            if (ZIP_ADD_UNIXTIME_FIELD && dirent.ModifiedTime != null && !SpecialMimetypeDirectoryEntry(dirent))
            {
                // Clearly a year 2038 problem here.
                byte[] tmp = new byte[4];

                GSF_LE_SET_GUINT16(tmp, 0, (ushort)(ExtraFieldTags.ZIP_DIRENT_EXTRA_FIELD_UNIXTIME));
                GSF_LE_SET_GUINT16(tmp, 2, 5);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 4).ToArray());
                tmp[0] = 1;
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 1).ToArray());
                GSF_LE_SET_GUINT32(tmp, 0, (uint)new DateTimeOffset(dirent.ModifiedTime ?? DateTime.UtcNow).ToUnixTimeSeconds());
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 4).ToArray());
            }

            byte[] buf = new byte[ZIP_DIRENT_SIZE];

            GSF_LE_SET_GUINT32(buf, 0, ZIP_DIRENT_SIGNATURE);
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_ENCODER, (ushort)(((int)OSCodes.ZIP_OS_UNIX << 8) + extract));
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_EXTRACT, (ushort)(((int)OSCodes.ZIP_OS_MSDOS << 8) + extract));
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_FLAGS, (ushort)dirent.Flags);
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_COMPR_METHOD, (ushort)dirent.CompressionMethod);
            GSF_LE_SET_GUINT32(buf, ZIP_DIRENT_DOSTIME, dirent.DosTime);
            GSF_LE_SET_GUINT32(buf, ZIP_DIRENT_CRC32, dirent.CRC32);
            GSF_LE_SET_GUINT32(buf, ZIP_DIRENT_CSIZE, (uint)(zip64_here ? uint.MaxValue : dirent.CompressedSize));
            GSF_LE_SET_GUINT32(buf, ZIP_DIRENT_USIZE, (uint)(zip64_here ? uint.MaxValue : dirent.UncompressedSize));
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_NAME_SIZE, (ushort)nlen);
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_EXTRAS_SIZE, (ushort)extras.Count);
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_COMMENT_SIZE, 0);
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_DISKSTART, 0);
            GSF_LE_SET_GUINT16(buf, ZIP_DIRENT_FILE_TYPE, 0);

            // Hardcode file mode 644
            GSF_LE_SET_GUINT32(buf, ZIP_DIRENT_FILE_MODE, 0100644u << 16);
            GSF_LE_SET_GUINT32(buf, ZIP_DIRENT_OFFSET, (uint)(offset_in_zip64 ? uint.MaxValue : dirent.Offset));

            // Stuff everything into buf so we can do just one write.
            List<byte> header = new List<byte>();
            header.AddRange(buf);
            header.AddRange(extras);
            header.AddRange(Encoding.ASCII.GetBytes(dirent.Name));

            return Sink.Write(header.Count, header.ToArray());
        }

        private bool TrailerWrite(int entries, long dirpos, long dirsize)
        {
            byte[] buf = new byte[ZIP_TRAILER_SIZE];

            GSF_LE_SET_GUINT32(buf, 0, ZIP_TRAILER_SIGNATURE);
            GSF_LE_SET_GUINT16(buf, ZIP_TRAILER_ENTRIES, (ushort)Math.Min(entries, ushort.MaxValue));
            GSF_LE_SET_GUINT16(buf, ZIP_TRAILER_TOTAL_ENTRIES, (ushort)Math.Min(entries, ushort.MaxValue));
            GSF_LE_SET_GUINT32(buf, ZIP_TRAILER_DIR_SIZE, (uint)Math.Min(dirsize, uint.MaxValue));
            GSF_LE_SET_GUINT32(buf, ZIP_TRAILER_DIR_POS, (uint)Math.Min(dirpos, uint.MaxValue));

            return Sink.Write(buf.Length, buf);
        }

        private bool Trailer64Write(int entries, long dirpos, long dirsize)
        {
            byte[] buf = new byte[ZIP_TRAILER64_SIZE];
            byte extract = 45;

            GSF_LE_SET_GUINT32(buf, 0, ZIP_TRAILER64_SIGNATURE);
            GSF_LE_SET_GUINT64(buf, ZIP_TRAILER64_RECSIZE, ZIP_TRAILER64_SIZE - 12);
            GSF_LE_SET_GUINT16(buf, ZIP_TRAILER64_ENCODER, (ushort)(((int)OSCodes.ZIP_OS_UNIX << 8) + extract));
            GSF_LE_SET_GUINT16(buf, ZIP_TRAILER64_EXTRACT, (ushort)(((int)OSCodes.ZIP_OS_MSDOS << 8) + extract));
            GSF_LE_SET_GUINT32(buf, ZIP_TRAILER64_DISK, 0);
            GSF_LE_SET_GUINT32(buf, ZIP_TRAILER64_DIR_DISK, 0);
            GSF_LE_SET_GUINT32(buf, ZIP_TRAILER64_ENTRIES, (uint)entries);
            GSF_LE_SET_GUINT32(buf, ZIP_TRAILER64_TOTAL_ENTRIES, (uint)entries);
            GSF_LE_SET_GUINT64(buf, ZIP_TRAILER64_DIR_SIZE, (ulong)dirsize);
            GSF_LE_SET_GUINT64(buf, ZIP_TRAILER64_DIR_POS, (ulong)dirpos);

            return Sink.Write(buf.Length, buf);
        }

        private bool Zip64LocatorWrite(long trailerpos)
        {
            byte[] buf = new byte[ZIP_ZIP64_LOCATOR_SIZE];

            GSF_LE_SET_GUINT32(buf, 0, ZIP_ZIP64_LOCATOR_SIGNATURE);
            GSF_LE_SET_GUINT32(buf, ZIP_ZIP64_LOCATOR_DISK, 0);
            GSF_LE_SET_GUINT64(buf, ZIP_TRAILER64_DIR_POS, (ulong)trailerpos);
            GSF_LE_SET_GUINT32(buf, ZIP_ZIP64_LOCATOR_DISKS, 1);

            return Sink.Write(buf.Length, buf);
        }

        private static int OffsetOrdering(GsfOutfileZip a, GsfOutfileZip b)
        {
            long diff = a.VDir.DirectoryEntry.Offset - b.VDir.DirectoryEntry.Offset;
            return diff < 0 ? -1 : diff > 0 ? +1 : 0;
        }

        private bool CloseRoot()
        {
            bool? zip64 = Zip64;

            // Check that children are closed
            for (int i = 0; i < RootOrder.Count; i++)
            {
                GsfOutfileZip child = RootOrder[i];
                GsfZipDirectoryEntry dirent = child.VDir.DirectoryEntry;
                if (dirent.Zip64 == true)
                    zip64 = true;

                if (!child.IsClosed)
                {
                    Console.Error.WriteLine("Child still open");
                    return false;
                }
            }

            if (true)
            {
                // It is unclear whether we need this.  However, the
                // zipdetails utility gets utterly confused if we do
                // not.
                //
                // If we do not sort, we will use the ordering in which
                // the members were actually being written.  Note, that
                // merely creating the member doesn't count -- it's the
                // actual writing (or closing an empty member) that
                // counts.

                RootOrder.Sort(OffsetOrdering);
            }

            // Write directory
            long dirpos = Sink.CurrentOffset;
            for (int i = 0; i < RootOrder.Count; i++)
            {
                GsfOutfileZip child = RootOrder[i];
                GsfZipDirectoryEntry dirent = child.VDir.DirectoryEntry;
                if (!DirectoryEntryWrite(dirent))
                    return false;
            }

            long dirend = Sink.CurrentOffset;

            if (RootOrder.Count >= ushort.MaxValue || dirend >= uint.MaxValue - ZIP_TRAILER_SIZE)
            {
                // We don't have a choice; force zip64.
                zip64 = true;
            }

            DisconnectChildren();

            if (zip64 == null)
                zip64 = false;

            if (zip64 == true)
            {
                if (!Trailer64Write(RootOrder.Count, dirpos, dirend - dirpos))
                    return false;

                if (!Zip64LocatorWrite(dirend))
                    return false;
            }

            return TrailerWrite(RootOrder.Count, dirpos, dirend - dirpos);
        }

        private void StreamNameWriteToBuf(string res)
        {
            if (this == Root)
                return;

            if (Container != null)
            {
                (Container as GsfOutfileZip).StreamNameWriteToBuf(res);
                if (res.Length != 0)
                {
                    // Forward slash is specified by the format.
                    res += ZIP_NAME_SEPARATOR;
                }
            }

            if (EntryName != null)
                res += EntryName;
        }

        private string StreamNameBuild()
        {
            string str = new string('\0', 80);
            StreamNameWriteToBuf(str);
            return str;
        }

        private static uint ZipTimeMake(DateTime? modtime) => (uint)(modtime ?? DateTime.UtcNow).ToFileTime();

        private GsfZipDirectoryEntry NewDirectoryEntry()
        {
            string name = StreamNameBuild();

            // The spec is a bit vague about the length limit for file names, but
            // clearly we should not go beyond 0xffff.
            if (name.Length < ushort.MaxValue)
            {
                GsfZipDirectoryEntry dirent = GsfZipDirectoryEntry.Create();
                DateTime? modtime = ModTime;

                dirent.Name = name;
                dirent.CompressionMethod = CompressionMethod;

                if (modtime == null)
                    modtime = DateTime.UtcNow;

                dirent.DosTime = ZipTimeMake(modtime);
                dirent.ModifiedTime = modtime;
                dirent.Zip64 = Zip64;

                return dirent;
            }
            else
            {
                return null;
            }
        }

        private bool HeaderWrite()
        {
            byte[] hbuf = new byte[ZIP_HEADER_SIZE];

            GsfZipDirectoryEntry dirent = VDir.DirectoryEntry;
            string name = dirent.Name;
            int nlen = name.Length;

            GSF_LE_SET_GUINT32(hbuf, 0, ZIP_HEADER_SIGNATURE);

            if (SinkIsSeekable == null)
            {
                // We need to figure out if the sink is seekable, but we lack
                // an API to check that.  Instead, write the signature and
                // try to seek back onto it.  If seeking back fails, just
                // don't rewrite it.

                if (!Sink.Write(4, hbuf))
                    return false;

                SinkIsSeekable = Sink.Seek(dirent.Offset, SeekOrigin.Begin);
                if (SinkIsSeekable == false)
                    hbuf = new byte[ZIP_HEADER_SIZE];
            }

            // Now figure out if we need a DDESC record.
            if (SinkIsSeekable == true)
                dirent.Flags &= ~ZIP_DIRENT_FLAGS_HAS_DDESC;
            else
                dirent.Flags |= ZIP_DIRENT_FLAGS_HAS_DDESC;

            bool has_ddesc = (dirent.Flags & ZIP_DIRENT_FLAGS_HAS_DDESC) != 0;
            uint crc32 = has_ddesc ? 0 : dirent.CRC32;
            long csize = has_ddesc ? 0 : dirent.CompressedSize;
            long usize = has_ddesc ? 0 : dirent.UncompressedSize;

            // Determine if we need a real zip64 extra field.  We do so, if
            // - forced
            // - in auto mode, if usize or csize has overflowed
            // - in auto mode, if we use a DDESC
            bool real_zip64 = (dirent.Zip64 == true
                || (dirent.Zip64 == null
                    && (has_ddesc
                        || dirent.UncompressedSize >= uint.MaxValue
                        || dirent.CompressedSize >= uint.MaxValue)));

            byte extract = 20;
            if (real_zip64)
                extract = 45;

            List<byte> extras = new List<byte>(ZIP_HEADER_SIZE + nlen + 100);

            // In the has_ddesc case, we write crc32/size/usize as zero and store
            // the right values in the DDESC record that follows the data.
            //
            // In the !has_ddesc case, we return to the same spot and write the
            // header a second time correcting crc32/size/usize, see
            // see HeaderPatchSizes.  For this reason, we must ensure that
            // the record's length does not depend on the the sizes.
            //
            // In the the has_ddesc case we store zeroes here.  No idea what we
            // were supposed to write.

            // Auto or forced
            if (dirent.Zip64 != false)
            {
                byte[] tmp = new byte[8];
                ExtraFieldTags typ = real_zip64
                    ? ExtraFieldTags.ZIP_DIRENT_EXTRA_FIELD_ZIP64
                    : ExtraFieldTags.ZIP_DIRENT_EXTRA_FIELD_IGNORE;

                GSF_LE_SET_GUINT16(tmp, 0, (ushort)typ);
                GSF_LE_SET_GUINT16(tmp, 2, 2 * 8);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 4).ToArray());
                GSF_LE_SET_GUINT64(tmp, 0, (ulong)usize);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 8).ToArray());
                GSF_LE_SET_GUINT64(tmp, 0, (ulong)csize);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 8).ToArray());
            }

            if (ZIP_ADD_UNIXTIME_FIELD && dirent.ModifiedTime != null && !SpecialMimetypeDirectoryEntry(dirent))
            {
                byte[] tmp = new byte[4];

                // Clearly a year 2038 problem here.
                GSF_LE_SET_GUINT16(tmp, 0, (ushort)ExtraFieldTags.ZIP_DIRENT_EXTRA_FIELD_UNIXTIME);
                GSF_LE_SET_GUINT16(tmp, 2, 5);
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 4).ToArray());
                tmp[0] = 1;
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 1).ToArray());
                GSF_LE_SET_GUINT32(tmp, 0, (uint)new DateTimeOffset(dirent.ModifiedTime ?? DateTime.UtcNow).ToUnixTimeSeconds());
                extras.AddRange(new ReadOnlySpan<byte>(tmp, 0, 4).ToArray());
            }

            GSF_LE_SET_GUINT16(hbuf, ZIP_HEADER_EXTRACT, (ushort)(((int)OSCodes.ZIP_OS_MSDOS << 8) + extract));
            GSF_LE_SET_GUINT16(hbuf, ZIP_HEADER_FLAGS, (ushort)dirent.Flags);
            GSF_LE_SET_GUINT16(hbuf, ZIP_HEADER_COMP_METHOD, (ushort)dirent.CompressionMethod);
            GSF_LE_SET_GUINT32(hbuf, ZIP_HEADER_DOSTIME, dirent.DosTime);
            GSF_LE_SET_GUINT32(hbuf, ZIP_HEADER_CRC32, crc32);
            GSF_LE_SET_GUINT32(hbuf, ZIP_HEADER_CSIZE, (uint)(real_zip64 && !has_ddesc ? uint.MaxValue : csize));
            GSF_LE_SET_GUINT32(hbuf, ZIP_HEADER_USIZE, (uint)(real_zip64 && !has_ddesc ? uint.MaxValue : usize));
            GSF_LE_SET_GUINT16(hbuf, ZIP_HEADER_NAME_SIZE, (ushort)nlen);
            GSF_LE_SET_GUINT16(hbuf, ZIP_HEADER_EXTRAS_SIZE, (ushort)extras.Count);

            // Stuff everything into buf so we can do just one write.
            List<byte> header = new List<byte>();
            header.AddRange(hbuf);
            header.AddRange(extras);
            header.AddRange(Encoding.ASCII.GetBytes(name));

            bool ret = Sink.Write(header.Count, header.ToArray());
            if (real_zip64)
                dirent.Zip64 = true;

            return ret;
        }

        private bool InitWrite()
        {
            int ret;

            if (Root.Writing)
            {
                Console.Error.WriteLine("Already writing to another stream in archive");
                return false;
            }

            if (!Wrap(Sink))
                return false;

            GsfZipDirectoryEntry dirent = NewDirectoryEntry();
            if (dirent == null)
            {
                Unwrap(Sink);
                return false;
            }

            dirent.Offset = Sink.CurrentOffset;
            if (SpecialMimetypeDirectoryEntry(dirent))
                dirent.Zip64 = false;

            VDir.DirectoryEntry = dirent;

            HeaderWrite();
            Writing = true;
            Root.Writing = true;

            // TODO: Enable CRC32 calculation
            // dirent.CRC32 = crc32(0L, Z_null, 0);
            if (CompressionMethod == GsfZipCompressionMethod.GSF_ZIP_DEFLATED)
            {
                if (Stream == null)
                    Stream = new ZStream();

                ret = Stream.inflateInit(-15);
                if (ret != Z_OK)
                    return false;

                ret = Stream.deflateParams(DeflateLevel, Z_DEFAULT_STRATEGY);
                if (ret != Z_OK)
                    return false;

                if (Buf == null)
                {
                    BufSize = ZIP_BUF_SIZE;
                    Buf = new byte[BufSize];
                }

                Stream.next_out = Buf;
                Stream.avail_out = BufSize;
            }

            return true;
        }

        private bool OutputBlock()
        {
            int num_bytes = BufSize - Stream.avail_out;
            GsfZipDirectoryEntry dirent = VDir.DirectoryEntry;

            if (!Sink.Write(num_bytes, Buf))
                return false;

            dirent.CompressedSize += num_bytes;
            if (dirent.Zip64 == false && dirent.CompressedSize >= uint.MaxValue)
                return false;

            Stream.next_out = Buf;
            Stream.avail_out = BufSize;

            return true;
        }

        private bool Flush()
        {
            int zret;
            do
            {
                zret = Stream.deflate(Z_FINISH);
                if (zret == Z_OK || (zret == Z_BUF_ERROR && Stream.avail_out == 0))
                {
                    //  In this case Z_OK or Z_BUF_ERROR means more buffer space is needed
                    if (!OutputBlock())
                        return false;
                }
            } while (zret == Z_OK || zret == Z_BUF_ERROR);

            if (zret != Z_STREAM_END)
                return false;

            if (!OutputBlock())
                return false;

            return true;
        }

        /// <summary>
        /// Write the per stream data descriptor
        /// </summary>
        private bool DataDescriptorWrite()
        {
            byte[] buf = new byte[Math.Max(ZIP_DDESC_SIZE, ZIP_DDESC64_SIZE)];
            GsfZipDirectoryEntry dirent = VDir.DirectoryEntry;
            int size;

            // Documentation says signature is not official.

            if (dirent.Zip64 != false)
            {
                GSF_LE_SET_GUINT32(buf, 0, ZIP_DDESC64_SIGNATURE);
                GSF_LE_SET_GUINT32(buf, ZIP_DDESC64_CRC32, dirent.CRC32);
                GSF_LE_SET_GUINT64(buf, ZIP_DDESC64_CSIZE, (ulong)dirent.CompressedSize);
                GSF_LE_SET_GUINT64(buf, ZIP_DDESC64_USIZE, (ulong)dirent.UncompressedSize);
                size = ZIP_DDESC64_SIZE;
            }
            else
            {
                GSF_LE_SET_GUINT32(buf, 0, ZIP_DDESC_SIGNATURE);
                GSF_LE_SET_GUINT32(buf, ZIP_DDESC_CRC32, dirent.CRC32);
                GSF_LE_SET_GUINT32(buf, ZIP_DDESC_CSIZE, (uint)dirent.CompressedSize);
                GSF_LE_SET_GUINT32(buf, ZIP_DDESC_USIZE, (uint)dirent.UncompressedSize);
                size = ZIP_DDESC_SIZE;
            }

            if (!Sink.Write(size, buf))
                return false;

            return true;
        }

        private bool HeaderPatchSizes()
        {
            GsfZipDirectoryEntry dirent = VDir.DirectoryEntry;
            long pos = Sink.CurrentOffset;

            // Rewrite the header in the same location again.
            bool ok = (Sink.Seek(dirent.Offset, SeekOrigin.Begin)
                && HeaderWrite()
                && Sink.Seek(pos, SeekOrigin.Begin));

            if (ok && dirent.Zip64 == null)
            {
                // We just wrote the final header.  Since we still are in
                // auto-mode, the header did not use a real zip64 extra
                // field.  Hence we don't need such a field.
                dirent.Zip64 = false;
            }

            return ok;
        }

        private bool CloseStream()
        {
            if (!Writing)
            {
                if (!InitWrite())
                    return false;
            }

            if (CompressionMethod == GsfZipCompressionMethod.GSF_ZIP_DEFLATED)
            {
                if (!Flush())
                    return false;
            }

            if ((VDir.DirectoryEntry.Flags & ZIP_DIRENT_FLAGS_HAS_DDESC) != 0)
            {
                // Write data descriptor
                if (!DataDescriptorWrite())
                    return false;
            }
            else
            {
                // Write crc, sizes
                if (!HeaderPatchSizes())
                    return false;
            }

            Root.Writing = false;

            bool result = Unwrap(Sink);

            // Free unneeded memory
            if (Stream != null)
            {
                Stream.deflateEnd();
                Stream = null;
                Buf = null;
            }

            return result;
        }

        #endregion
    }
}
