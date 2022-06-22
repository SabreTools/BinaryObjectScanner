﻿/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-outfile-msole.c:
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
 * Foundation, Outc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibGSF.Input;
using static LibGSF.GsfMSOleImpl;
using static LibGSF.GsfUtils;

namespace LibGSF.Output
{
    public class GsfOutfileMSOleBlock
    {
        public int Shift { get; set; }

        public int Size { get; set; }
    }

    public class GsfOutfileMSOle : GsfOutfile
    {
        #region Constants

        // The most common values
        private const int OLE_DEFAULT_THRESHOLD = 0x1000;
        private const int OLE_DEFAULT_SB_SHIFT = 6;
        private const int OLE_DEFAULT_BB_SHIFT = 9;
        private const long OLE_DEFAULT_BB_SIZE = (1u << OLE_DEFAULT_BB_SHIFT);
        private const long OLE_DEFAULT_SB_SIZE = (1u << OLE_DEFAULT_SB_SHIFT);

        // Globals to support variable OLE sector size.

        /// <summary>
        /// 512 and 4096 bytes are the only known values for sector size on
        /// Win2k/XP platforms.  Attempts to create OLE files on Win2k/XP with
        /// other values	using StgCreateStorageEx() fail with invalid parameter.
        /// This code has been tested with 128,256,512,4096,8192	sizes for
        /// libgsf read/write.  Interoperability with MS OLE32.DLL has been
        /// tested with 512 and 4096 block size for filesizes up to 2GB.
        /// </summary>
        private const int ZERO_PAD_BUF_SIZE = 4096;

        private const uint CHUNK_SIZE = 1024u;

        private static readonly byte[] default_header =
        {
            /* 0x00 */	0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1,
            /* 0x08 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            /* 0x18 */	0x3e, 0x00, 0x03, 0x00, 0xfe, 0xff, 0x09, 0x00,
            /* 0x20 */	0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            /* 0x28 */	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            /* 0x30 */	0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00,
            /* 0x38 */	0x00, 0x10, 0x00, 0x00 /* 0x3c-0x4b: filled on close */
	    };

        #endregion

        #region Enums

        public enum MSOleOutfileType
        {
            MSOLE_DIR,
            MSOLE_SMALL_BLOCK,
            MSOLE_BIG_BLOCK
        }

        #endregion

        #region Properties

        public GsfOutfile Parent { get; set; }

        public GsfOutput Sink { get; set; } = null;

        public GsfOutfileMSOle Root { get; set; } = null;

        public GsfMSOleSortingKey Key { get; set; }

        public MSOleOutfileType Type { get; set; } = MSOleOutfileType.MSOLE_DIR;

        public int FirstBlock { get; set; }

        public int Blocks { get; set; }

        public int ChildIndex { get; set; }

        public GsfOutfileMSOleBlock BigBlock { get; set; }

        public GsfOutfileMSOleBlock SmallBlock { get; set; }

        #region Union (Content)

        #region Struct (Dir)

        public List<GsfOutfileMSOle> Content_Dir_Children { get; set; } = null;

        /// <summary>
        /// Only valid for the root, ordered
        /// </summary>
        public List<GsfOutfileMSOle> Content_Dir_RootOrder { get; set; } = null;

        #endregion

        #region Struct (SmallBlock)

        public byte[] Content_SmallBlock_Buf { get; set; }

        #endregion

        #region Struct (BigBlock)

        /// <summary>
        /// In bytes
        /// </summary>
        public int Content_BigBlock_StartOffset { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// 16 byte GUID used by some apps
        /// </summary>
        public byte[] ClassID { get; set; } = new byte[16];

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutfileMSOle() => MakeSortingName();

        /// <summary>
        /// Creates the root directory of an MS OLE file and manages the addition of
        /// children.
        /// </summary>
        /// <param name="sink">A GsfOutput to hold the OLE2 file.</param>
        /// <param name="bb_size">Size of large blocks.</param>
        /// <param name="sb_size">Size of small blocks.</param>
        /// <returns>The new ole file handler.</returns>
        /// <remarks>This adds a reference to <paramref name="sink"/>.</remarks>
        public static GsfOutfileMSOle CreateFull(GsfOutput sink, int bb_size, int sb_size)
        {
            if (sink == null)
                return null;
            if (sb_size != (1u << ComputeShift(sb_size)))
                return null;
            if (bb_size != (1u << ComputeShift(bb_size)))
                return null;
            if (sb_size > bb_size)
                return null;

            GsfOutfileMSOle ole = new GsfOutfileMSOle
            {
                Sink = sink,
                SmallBlock = new GsfOutfileMSOleBlock
                {
                    Size = sb_size,
                },
                BigBlock = new GsfOutfileMSOleBlock
                {
                    Size = bb_size,
                },
                Container = null,
                Name = sink.Name,
                Type = MSOleOutfileType.MSOLE_DIR,
                Content_Dir_RootOrder = new List<GsfOutfileMSOle>(),
            };

            ole.RegisterChild(ole);

            // Build the header
            byte[] buf = Enumerable.Repeat<byte>(0xFF, MSOleHeader.OLE_HEADER_SIZE).ToArray();

            MSOleHeader header = MSOleHeader.CreateDefault();
            header.BB_SHIFT = (ushort)ole.BigBlock.Shift;
            header.SB_SHIFT = (ushort)ole.SmallBlock.Shift;

            // 4k sector OLE files seen in the wild have version 4
            if (ole.BigBlock.Size == 4096)
                header.MAJOR_VER = 4;

            header.Write(buf, 0);
            sink.Write(MSOleHeader.OLE_HEADER_SIZE, buf);

            // Header must be padded out to BigBlock.Size with zeros
            ole.PadZero();

            return ole;
        }

        /// <summary>
        /// Creates the root directory of an MS OLE file and manages the addition of
        /// children.
        /// </summary>
        /// <param name="sink">A GsfOutput to hold the OLE2 file</param>
        /// <returns>The new ole file handler.</returns>
        /// <remarks>This adds a reference to <paramref name="sink"/>.</remarks>
        public static GsfOutfileMSOle Create(GsfOutput sink) => CreateFull(sink, (int)OLE_DEFAULT_BB_SIZE, (int)OLE_DEFAULT_SB_SIZE);

        ~GsfOutfileMSOle()
        {
            Key = null;

            switch (Type)
            {
                case MSOleOutfileType.MSOLE_DIR:
                    Content_Dir_Children.Clear();
                    Content_Dir_Children = null;
                    if (Content_Dir_RootOrder != null)
                        Console.Error.WriteLine("Finalizing a MSOle Outfile without closing it.");

                    break;

                case MSOleOutfileType.MSOLE_SMALL_BLOCK:
                    Content_SmallBlock_Buf = null;
                    break;

                case MSOleOutfileType.MSOLE_BIG_BLOCK:
                    break;

                default:
                    throw new Exception("This should not be reached");
            }
        }

        #endregion

        #region Functions

        public override void Dispose()
        {
            if (!IsClosed)
                Close();

            Sink = null;
            base.Dispose();
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence)
        {
            switch (whence)
            {
                case SeekOrigin.Begin: break;
                case SeekOrigin.Current: offset += CurrentOffset; break;
                case SeekOrigin.End: offset += CurrentSize; break;
                default: throw new Exception("This should not be reached");
            }

            switch (Type)
            {
                case MSOleOutfileType.MSOLE_DIR:
                    if (offset != 0)
                    {
                        Console.Error.WriteLine("Attempt to seek a directory");
                        return false;
                    }

                    return true;

                case MSOleOutfileType.MSOLE_SMALL_BLOCK:
                    // It is ok to seek past the big block threshold
                    // we don't convert until they _write_ something
                    return true;

                case MSOleOutfileType.MSOLE_BIG_BLOCK:
                    return Sink.Seek(Content_BigBlock_StartOffset + offset, SeekOrigin.Begin);

                default:
                    throw new Exception("This should not be reached");
            }

            return false;
        }

        /// <inheritdoc/>
        protected override bool CloseImpl()
        {
            if (Container == null)
            {
                // The root dir
                bool ok = WriteDirectory();
                HoistError();
                if (!Sink.Close())
                    ok = false;

                return ok;
            }

            if (Type == MSOleOutfileType.MSOLE_BIG_BLOCK)
            {
                Seek(0, SeekOrigin.End);
                PadZero();
                Blocks = CurrentBlock() - FirstBlock;
                return Unwrap(Sink);
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
            if (Type == MSOleOutfileType.MSOLE_DIR)
                return false;

            if (Type == MSOleOutfileType.MSOLE_SMALL_BLOCK)
            {
                if ((CurrentOffset + num_bytes) < OLE_DEFAULT_THRESHOLD)
                {
                    Array.Copy(Content_SmallBlock_Buf, CurrentOffset, data, 0, num_bytes);
                    return true;
                }

                bool ok = Wrap(Sink);
                if (!ok)
                    return false;

                byte[] buf = Content_SmallBlock_Buf;
                Content_SmallBlock_Buf = null;

                long start_offset = Sink.CurrentOffset;
                Content_BigBlock_StartOffset = (int)start_offset;
                if (Content_BigBlock_StartOffset != start_offset)
                {
                    // Check for overflow
                    Console.Error.WriteLine("File too big");
                    return false;
                }

                FirstBlock = CurrentBlock();
                Type = MSOleOutfileType.MSOLE_BIG_BLOCK;

                int wsize = (int)CurrentSize;
                if (wsize != CurrentSize)
                {
                    // Check for overflow
                    Console.Error.WriteLine("File too big");
                    return false;
                }

                Sink.Write(wsize, buf);

                // If we had a seek then we might not be at the right location.
                // This can happen both with a seek beyond the end of file
                // (see bug #14) and with a backward seek.
                Sink.Seek(Content_BigBlock_StartOffset + CurrentOffset, SeekOrigin.Begin);
            }

            if (Type != MSOleOutfileType.MSOLE_BIG_BLOCK)
                return false;

            Sink.Write(num_bytes, data);

            return true;
        }

        /// <inheritdoc/>
        protected override long VPrintFImpl(string format, params string[] args)
        {
            // An optimization.
            if (Type == MSOleOutfileType.MSOLE_BIG_BLOCK)
                return Sink.VPrintF(format, args);

            // In other cases, use the gsf_output_real_vprintf fallback method.
            // (This eventually calls gsf_outfile_msole_write, which will also
            // check that ole.type != MSOLE_DIR.)
            return VPrintF(format, args);
        }

        /// <inheritdoc/>
        public override GsfOutput NewChild(string name, bool is_dir)
        {
            if (Type != MSOleOutfileType.MSOLE_DIR)
                return null;

            GsfOutfileMSOle child = new GsfOutfileMSOle();
            if (is_dir)
            {
                child.Type = MSOleOutfileType.MSOLE_DIR;
                child.Content_Dir_Children = null;
            }
            else
            {
                // Start as small block
                child.Type = MSOleOutfileType.MSOLE_SMALL_BLOCK;
                child.Content_SmallBlock_Buf = new byte[OLE_DEFAULT_THRESHOLD];
            }

            child.Root = Root;
            child.Sink = Sink;

            child.SetSmallBlockSize(SmallBlock.Size);
            child.SetBigBlockSize(BigBlock.Size);

            child.Name = name;
            child.Container = this;

            Content_Dir_Children.Add(child);
            Root.RegisterChild(child);

            return child;
        }

        /// <summary>
        /// Write <paramref name="clsid"/> to the directory associated with ole.
        /// </summary>
        /// <param name="clsid">Identifier (often a GUID in MS Windows apps)</param>
        /// <returns>True on success.</returns>
        public bool SetClassID(byte[] clsid)
        {
            if (Type != MSOleOutfileType.MSOLE_DIR)
                return false;

            Array.Copy(clsid, ClassID, ClassID.Length);
            return true;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the number of times 1 must be shifted left to reach value
        /// </summary>
        private static int ComputeShift(int value)
        {
            int i = 0;
            while ((value >> i) > 1)
                i++;

            return i;
        }

        private void SetBigBlockSize(int size)
        {
            BigBlock.Size = size;
            BigBlock.Shift = ComputeShift(size);
        }

        private void SetSmallBlockSize(int size)
        {
            SmallBlock.Size = size;
            SmallBlock.Shift = ComputeShift(size);
        }

        /// <summary>
        /// Static objects are zero-initialized
        /// </summary>
        private static byte[] zero_buf = new byte[ZERO_PAD_BUF_SIZE];

        /// <summary>
        /// Calculate the block of the current offset in the file.  A useful idiom is to
        /// pad_zero to move to the start of the next block, then get the block number.
        /// This avoids fence post type problems with partial blocks.
        /// </summary>
        private int CurrentBlock() => (int)((Sink.CurrentOffset - MSOleHeader.OLE_HEADER_SIZE) >> BigBlock.Shift);

        private int BytesLeftInBlock()
        {
            // Blocks are multiples of BigBlock.Size (the header is padded out to BigBlock.Size) 
            long r = Sink.CurrentOffset % BigBlock.Size;
            return (int)((r != 0) ? (BigBlock.Size - r) : 0);
        }

        private void PadZero()
        {
            // No need to bounds check.  len will always be less than bb.size, and
            // we already check that zero_buf is big enough at creation
            int len = BytesLeftInBlock();
            if (len > 0)
                Sink.Write(len, zero_buf);
        }

        /// <summary>
        /// Utility routine to generate a BAT for a file known to be sequential and
        /// continuous.
        /// </summary>
        private static void WriteBat(GsfOutput sink, int block, int blocks)
        {
            byte[] buf = new byte[BAT_INDEX_SIZE * CHUNK_SIZE];
            int bufi = 0;

            while (blocks-- > 1)
            {
                block++;

                byte[] temp2 = BitConverter.GetBytes(block);
                Array.Copy(temp2, 0, buf, bufi * BAT_INDEX_SIZE, buf.Length);

                bufi++;
                if (bufi == CHUNK_SIZE)
                {
                    if (bufi != 0)
                        sink.Write(bufi * BAT_INDEX_SIZE, buf);

                    bufi = 0;
                }
            }

            byte[] temp = BitConverter.GetBytes(BAT_MAGIC_END_OF_CHAIN);
            Array.Copy(temp, 0, buf, bufi * BAT_INDEX_SIZE, buf.Length);

            bufi++;
            if (bufi == CHUNK_SIZE)
            {
                if (bufi != 0)
                    sink.Write(bufi * BAT_INDEX_SIZE, buf);

                bufi = 0;
            }

            if (bufi != 0)
                sink.Write(bufi * BAT_INDEX_SIZE, buf);
        }

        private static void WriteConst(GsfOutput sink, uint value, int n)
        {
            byte[] buf = new byte[BAT_INDEX_SIZE * CHUNK_SIZE];
            int bufi = 0;

            while (n-- > 0)
            {
                byte[] temp = BitConverter.GetBytes(value);
                Array.Copy(temp, 0, buf, bufi * BAT_INDEX_SIZE, buf.Length);

                bufi++;
                if (bufi == CHUNK_SIZE)
                {
                    if (bufi != 0)
                        sink.Write(bufi * BAT_INDEX_SIZE, buf);

                    bufi = 0;
                }
            }

            if (bufi != 0)
                sink.Write(bufi * BAT_INDEX_SIZE, buf);

            bufi = 0;
        }

        private void PadBatUnused(int residual)
        {
            WriteConst(Sink, BAT_MAGIC_UNUSED, (BytesLeftInBlock() / BAT_INDEX_SIZE) - residual);
        }

        /// <summary>
        /// Write the metadata (dirents, small block, xbats)
        /// </summary>
        private bool WriteDirectory()
        {
            byte[] buf = new byte[MSOleHeader.OLE_HEADER_SIZE];
            uint next;
            uint xbat_pos;
            int metabat_size = BigBlock.Size / BAT_INDEX_SIZE - 1;
            List<GsfOutfileMSOle> elem = Root.Content_Dir_RootOrder;

            // Write small block data
            int blocks = 0;
            int sb_data_start = CurrentBlock();
            long data_size = Sink.CurrentOffset;

            foreach (GsfOutfileMSOle child in elem)
            {
                if (child.Type == MSOleOutfileType.MSOLE_SMALL_BLOCK)
                {
                    long size = child.CurrentSize;
                    if (size > 0)
                    {
                        child.Blocks = (int)(((size - 1) >> SmallBlock.Shift) + 1);
                        Sink.Write(child.Blocks << SmallBlock.Shift, child.Content_SmallBlock_Buf);
                        child.FirstBlock = blocks;
                        blocks += child.Blocks;
                    }
                    else
                    {
                        child.Blocks = 0;
                        unchecked
                        {
                            child.FirstBlock = (int)BAT_MAGIC_END_OF_CHAIN;
                        }
                    }
                }
            }

            data_size = Sink.CurrentOffset - data_size;
            int sb_data_size = (int)data_size;
            if (sb_data_size != data_size)
            {
                // Check for overflow
                Console.Error.WriteLine("File too big");
                return false;
            }

            PadZero();
            int sb_data_blocks = CurrentBlock() - sb_data_start;

            // Write small block BAT (the meta bat is in a file)
            int sbat_start = CurrentBlock();
            foreach (GsfOutfileMSOle child in elem)
            {
                if (child.Type == MSOleOutfileType.MSOLE_SMALL_BLOCK && child.Blocks > 0)
                    WriteBat(Sink, child.FirstBlock, child.Blocks);
            }

            PadBatUnused(0);
            int num_sbat = CurrentBlock() - sbat_start;

            // Write dirents
            int dirent_start = CurrentBlock();
            for (int i = 0; i < elem.Count; i++)
            {
                GsfOutfileMSOle child = elem[i];

                buf = Enumerable.Repeat<byte>(0, MSOleDirectoryEntry.DIRENT_SIZE).ToArray();

                MSOleDirectoryEntry directoryEntry = MSOleDirectoryEntry.CreateDefault();

                // Hard code 'Root Entry' for the root
                if (i == 0 || child.Name != null)
                {
                    string name = (i == 0) ? "Root Entry" : child.Name;

                    byte[] nameUtf16Bytes = Encoding.UTF8.GetBytes(name);
                    nameUtf16Bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, nameUtf16Bytes);
                    ushort name_len = (ushort)nameUtf16Bytes.Length;
                    if (name_len >= MSOleDirectoryEntry.DIRENT_MAX_NAME_SIZE)
                        name_len = MSOleDirectoryEntry.DIRENT_MAX_NAME_SIZE - 1;

                    directoryEntry.NAME = nameUtf16Bytes;
                    directoryEntry.NAME_LEN = (ushort)(name_len + 1);
                }

                if (child.Root == child)
                {
                    directoryEntry.TYPE_FLAG = DIRENT_TYPE.DIRENT_TYPE_ROOTDIR;
                    directoryEntry.FIRSTBLOCK = (sb_data_size > 0) ? (uint)sb_data_start : BAT_MAGIC_END_OF_CHAIN;
                    directoryEntry.FILE_SIZE = (uint)sb_data_size;
                    directoryEntry.CLSID = child.ClassID;
                }
                else if (child.Type == MSOleOutfileType.MSOLE_DIR)
                {
                    directoryEntry.TYPE_FLAG = DIRENT_TYPE.DIRENT_TYPE_DIR;
                    directoryEntry.FIRSTBLOCK = BAT_MAGIC_END_OF_CHAIN;
                    directoryEntry.FILE_SIZE = 0;
                    directoryEntry.CLSID = child.ClassID;
                }
                else
                {
                    uint size = (uint)child.Parent.CurrentSize;
                    if (size != child.Parent.CurrentSize)
                        Console.Error.WriteLine("File too big");

                    directoryEntry.TYPE_FLAG = DIRENT_TYPE.DIRENT_TYPE_FILE;
                    directoryEntry.FIRSTBLOCK = (uint)child.FirstBlock;
                    directoryEntry.FILE_SIZE = size;
                    directoryEntry.CLSID = new byte[0x10];
                }

                directoryEntry.MODIFY_TIME = (ulong)(child.ModTime?.ToFileTime() ?? 0);

                // Make everything black (red == 0)
                directoryEntry.COLOR = 1;

                GsfOutfileMSOle tmp = child.Container as GsfOutfileMSOle;
                next = MSOleDirectoryEntry.DIRENT_MAGIC_END;
                if (child.Root != child && tmp != null)
                {
                    for (int j = 0; j < tmp.Content_Dir_Children.Count; j++)
                    {
                        GsfOutfileMSOle ptr = tmp.Content_Dir_Children[j];
                        if (ptr != child)
                            continue;

                        if (j + 1 != tmp.Content_Dir_Children.Count && tmp.Content_Dir_Children[j + 1] != null)
                        {
                            GsfOutfileMSOle sibling = tmp.Content_Dir_Children[j + 1];
                            next = (uint)sibling.ChildIndex;
                        }

                        break;
                    }
                }

                // Make linked list rather than tree, only use next
                directoryEntry.PREV = MSOleDirectoryEntry.DIRENT_MAGIC_END;
                directoryEntry.NEXT = next;

                uint child_index = MSOleDirectoryEntry.DIRENT_MAGIC_END;
                if (child.Type == MSOleOutfileType.MSOLE_DIR && child.Content_Dir_Children != null)
                {
                    GsfOutfileMSOle first = child.Content_Dir_Children[0];
                    child_index = (uint)first.ChildIndex;
                }

                directoryEntry.CHILD = child_index;

                directoryEntry.Write(buf, 0);
                Sink.Write(MSOleDirectoryEntry.DIRENT_SIZE, buf);
            }

            PadZero();
            int num_dirent_blocks = CurrentBlock() - dirent_start;

            // Write BAT
            int bat_start = CurrentBlock();
            foreach (GsfOutfileMSOle child in elem)
            {
                if (child.Type == MSOleOutfileType.MSOLE_BIG_BLOCK)
                    WriteBat(Sink, child.FirstBlock, child.Blocks);
            }

            if (sb_data_blocks > 0)
                WriteBat(Sink, sb_data_start, sb_data_blocks);

            if (num_sbat > 0)
                WriteBat(Sink, sbat_start, num_sbat);

            WriteBat(Sink, dirent_start, num_dirent_blocks);

            // List the BAT and meta-BAT blocks in the BAT.  Doing this may
            // increase the size of the bat and hence the metabat, so be
            // prepared to iterate.
            long num_bat = 0;
            long num_xbat = 0;

            while (Sink.Error == null)
            {
                // If we have an error, then the actual size as reported
                // by _tell and .cur_size may be out of sync.  We don't
                // want to loop forever here.

                long i = ((Sink.CurrentSize + BAT_INDEX_SIZE * (num_bat + num_xbat) - MSOleHeader.OLE_HEADER_SIZE - 1) >> BigBlock.Shift) + 1;
                i -= bat_start;
                if (num_bat != i)
                {
                    num_bat = i;
                    continue;
                }
                i = 0;
                if (num_bat > OLE_HEADER_METABAT_SIZE)
                    i = 1 + ((num_bat - OLE_HEADER_METABAT_SIZE - 1)
                         / metabat_size);
                if (num_xbat != i)
                {
                    num_xbat = i;
                    continue;
                }

                break;
            }

            WriteConst(Sink, BAT_MAGIC_BAT, (int)num_bat);
            WriteConst(Sink, BAT_MAGIC_METABAT, (int)num_xbat);
            PadBatUnused(0);

            if (num_xbat > 0)
            {
                xbat_pos = (uint)CurrentBlock();
                blocks = OLE_HEADER_METABAT_SIZE;
            }
            else
            {
                xbat_pos = BAT_MAGIC_END_OF_CHAIN;
                blocks = (int)num_bat;
            }

            // Fix up the header
            if (BigBlock.Size == 4096)
            {
                // Set _cSectDir for 4k sector files
                GSF_LE_SET_GUINT32(buf, 0, (uint)num_dirent_blocks);
                Sink.Seek(OLE_HEADER_CSECTDIR, SeekOrigin.Begin);
                Sink.Write(4, buf);
            }

            GSF_LE_SET_GUINT32(buf, 0, (uint)num_bat);
            GSF_LE_SET_GUINT32(buf, 4, (uint)dirent_start);
            Sink.Seek(OLE_HEADER_NUM_BAT, SeekOrigin.Begin);
            Sink.Write(8, buf);

            GSF_LE_SET_GUINT32(buf, 0x0, (num_sbat > 0) ? (uint)sbat_start : BAT_MAGIC_END_OF_CHAIN);
            GSF_LE_SET_GUINT32(buf, 0x4, (uint)num_sbat);
            GSF_LE_SET_GUINT32(buf, 0x8, xbat_pos);
            GSF_LE_SET_GUINT32(buf, 0xc, (uint)num_xbat);
            Sink.Seek(OLE_HEADER_SBAT_START, SeekOrigin.Begin);
            Sink.Write(0x10, buf);

            // Write initial Meta-BAT
            for (int i = 0; i < blocks; i++)
            {
                GSF_LE_SET_GUINT32(buf, 0, (uint)(bat_start + i));
                Sink.Write(BAT_INDEX_SIZE, buf);
            }

            // Write extended Meta-BAT
            if (num_xbat > 0)
            {
                Sink.Seek(0, SeekOrigin.End);
                for (long i = 0; i++ < num_xbat;)
                {
                    bat_start += blocks;
                    num_bat -= blocks;
                    blocks = (int)((num_bat > metabat_size) ? metabat_size : num_bat);
                    for (int j = 0; j < blocks; j++)
                    {
                        GSF_LE_SET_GUINT32(buf, 0, (uint)(bat_start + j));
                        Sink.Write(BAT_INDEX_SIZE, buf);
                    }

                    if (i == num_xbat)
                    {
                        PadBatUnused(1);
                        xbat_pos = BAT_MAGIC_END_OF_CHAIN;
                    }
                    else
                    {
                        xbat_pos++;
                    }

                    GSF_LE_SET_GUINT32(buf, 0, xbat_pos);
                    Sink.Write(BAT_INDEX_SIZE, buf);
                }
            }

            // Free the children
            Content_Dir_RootOrder = null;

            return true;
        }

        private void HoistError()
        {
            if (Error == null && Sink.Error != null)
                Error = Sink.Error;
        }

        private void RegisterChild(GsfOutfileMSOle child)
        {
            child.Root = this;
            child.ChildIndex = Content_Dir_RootOrder.Count;
            Content_Dir_RootOrder.Add(child);
        }

        private static int NameCompare(GsfOutfileMSOle a, GsfOutfileMSOle b) => GsfInfileMSOle.SortingKeyCompare(a.Key, b.Key);

        private void MakeSortingName()
        {
            Key = GsfMSOleSortingKey.Create(Name);
        }

        #endregion
    }
}
