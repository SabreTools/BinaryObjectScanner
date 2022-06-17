/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input.c: interface for used by the ole layer to read raw data
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibGSF.Output;
using static LibGSF.GsfMSOleUtils;
using static LibGSF.GsfUtils;

namespace LibGSF.Input
{
    public abstract class GsfInput : IDisposable
    {
        #region Constants

        private const int GSF_READ_BUFSIZE = 1024 * 4;

        #endregion

        #region Properties

        public long Size { get; protected internal set; }

        public long CurrentOffset { get; protected internal set; }

        public string Name { get; protected internal set; }

        public GsfInfile Container { get; protected internal set; }

        public DateTime? ModTime { get; protected internal set; }

        public GsfOpenPkgRels Relations { get; protected internal set; }

        #endregion

        #region Functions

        public void Dispose()
        {
            Container = null;
            Name = null;
            ModTime = default;
        }

        public void Init()
        {
            Size = 0;
            CurrentOffset = 0;
            Name = null;
            Container = null;
        }

        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>The duplicate</returns>
        public GsfInput Duplicate(ref Exception err)
        {
            GsfInput dst = DupImpl(ref err);
            if (dst != null)
            {
                if (dst.Size != Size)
                {
                    err = new Exception("Duplicate size mismatch");
                    return null;
                }

                if (dst.Seek(CurrentOffset, SeekOrigin.Begin))
                {
                    err = new Exception("Seek failed");
                    return null;
                }

                dst.Name = Name;
                dst.Container = Container;
            }

            return dst;
        }

        /// <summary>
        /// Attempts to open a 'sibling' of input.  The caller is responsible for
        /// managing the resulting object.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>A related GsfInput</returns>
        /// <remarks>
        /// UNIMPLEMENTED BY ANY BACKEND
        /// and it is probably unnecessary.   Container provides
        /// enough power to do what is necessary.
        /// </remarks>
        public GsfInput OpenSibling(string name, ref Exception err) => OpenSiblingImpl(name, ref err);

        /// <summary>
        /// Are we at the end of the file?
        /// </summary>
        /// <returns>True if the input is at the eof.</returns>
        public bool EOF() => CurrentOffset >= Size;

        /// <summary>
        /// Read at least <paramref name="num_bytes"/>.  Does not change the current position if there
        /// is an error.  Will only read if the entire amount can be read.  Invalidates
        /// the buffer associated with previous calls to gsf_input_read.
        /// </summary>
        /// <param name="num_bytes">Number of bytes to read</param>
        /// <param name="optional_buffer">Pointer to destination memory area</param>
        /// <returns>
        /// Pointer to the buffer or null if there is
        /// an error or 0 bytes are requested.
        /// </returns>
        public byte[] Read(int num_bytes, byte[] optional_buffer, int bufferPtr = 0)
        {
            long newpos = CurrentOffset + num_bytes;

            if (newpos <= CurrentOffset || newpos > Size)
                return null;

            byte[] res = ReadImpl(num_bytes, optional_buffer, bufferPtr);
            if (res == null)
                return null;

            CurrentOffset = newpos;
            return res;
        }

        /// <summary>
        /// Read <paramref name="num_bytes"/>.  Does not change the current position if there
        /// is an error.  Will only read if the entire amount can be read.
        /// </summary>
        /// <param name="num_bytes">Number of bytes to read</param>
        /// <param name="bytes_read">Copy of <paramref name="num_bytes"/></param>
        /// <returns>The data read.</returns>
        public byte[] Read0(int num_bytes, out int bytes_read)
        {
            bytes_read = num_bytes;

            if (num_bytes < 0 || (long)num_bytes > Remaining())
                return null;

            byte[] res = new byte[num_bytes];
            if (Read(num_bytes, res) != null)
                return res;

            return null;
        }

        /// <returns>The number of bytes left in the file.</returns>
        public long Remaining() => Size - CurrentOffset;

        /// <summary>
        /// Move the current location in the input stream.
        /// </summary>
        /// <param name="offset">Target offset</param>
        /// <param name="whence">
        /// Determines whether the offset is relative to the beginning or
        /// the end of the stream, or to the current location.
        /// </param>
        /// <returns>True on error.</returns>
        public bool Seek(long offset, SeekOrigin whence)
        {
            long pos = offset;

            switch (whence)
            {
                case SeekOrigin.Begin: break;
                case SeekOrigin.Current: pos += CurrentOffset; break;
                case SeekOrigin.End: pos += Size; break;
                default: return true;
            }

            if (pos < 0 || pos > Size)
                return true;

            // If we go nowhere, just return.  This in particular handles null
            // seeks for streams with no seek method.
            if (pos == CurrentOffset)
                return false;

            if (SeekImpl(offset, whence))
                return true;

            CurrentOffset = pos;
            return false;
        }

        /// <param name="filename">The (fs-sys encoded) filename</param>
        /// <returns>True if the assignment was ok.</returns>
        public bool SetNameFromFilename(string filename)
        {
            string name = null;
            if (filename != null)
            {
                byte[] filenameBytes = Encoding.Unicode.GetBytes(filename);
                filenameBytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, filenameBytes);
                name = Encoding.UTF8.GetString(filenameBytes);
            }

            Name = name;
            return true;
        }

        /// <summary>
        /// Emulate forward seeks by reading.
        /// </summary>
        /// <param name="pos">Absolute position to seek to</param>
        /// <returns>True if the emulation failed.</returns>
        public bool SeekEmulate(long pos)
        {
            if (pos < CurrentOffset)
                return true;

            while (pos > CurrentOffset)
            {
                long readcount = Math.Min(pos - CurrentOffset, 8192);
                if (Read((int)readcount, null) == null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Copy the contents from input to <paramref name="output"/> from their respective
        /// current positions. So if you want to be sure to copy *everything*,
        /// make sure to call input.Seek(0, SeekOrigin.Begin) and
        /// output.Seek(0, SeekOrigin.Begin) first, if applicable.
        /// </summary>
        /// <param name="output">A non-null GsfOutput</param>
        /// <returns>True on success</returns>
        public bool Copy(GsfOutput output)
        {
            if (output == null)
                return false;

            bool success = true;
            long remaining;
            while (success && (remaining = Remaining()) > 0)
            {
                long toread = Math.Min(remaining, GSF_READ_BUFSIZE);
                byte[] buffer = Read((int)toread, null);
                if (buffer != null && buffer.Length != 0)
                    success = output.Write((int)toread, buffer);
                else
                    success = false;
            }

            return success;
        }

        /// <summary>
        /// This functions takes ownership of the incoming reference and yields a
        /// new one as its output.
        /// </summary>
        /// <returns>
        /// A stream equivalent to the source stream,
        /// but uncompressed if the source was compressed.
        /// </returns>
        public GsfInput Uncompress()
        {
            long cur_offset = CurrentOffset;
            byte[] header = new byte[4];

            if (Seek(0, SeekOrigin.Begin))
                goto error;

            // Read header up front, so we avoid extra seeks in tests.
            if (Read(4, header) == null)
                goto error;

            // Let's try gzip.
            {
                byte[] gzip_sig = new byte[2] { 0x1f, 0x8b };
                if (header.Take(2).SequenceEqual(gzip_sig))
                {
                    Exception err = null;
                    GsfInput res = GsfInputGZip.Create(null, ref err);
                    if (res != null)
                        return res.Uncompress();
                }
            }

            // Let's try bzip.
            {
                byte[] bzip_sig = new byte[3] { (byte)'B', (byte)'Z', (byte)'h' };
                if (header.Take(3).SequenceEqual(bzip_sig))
                {
                    Exception err = null;
                    GsfInput res = GsfInputMemory.CreateFromBzip(null, ref err);
                    if (res != null)
                        return res.Uncompress();
                }
            }

        // Other methods go here.

        error:
            Seek(cur_offset, SeekOrigin.Begin);
            return this;
        }

        #endregion

        #region GIO

        internal void SetNameFromFile(string file)
        {
            if (!File.Exists(file))
                return;

            FileInfo info = new FileInfo(file);
            SetNameFromFilename(info.Name);
        }

        #endregion

        #region MS-OLE

        /// <summary>
        /// Decompresses an LZ compressed stream.
        /// </summary>
        /// <param name="offset">Offset into it for start byte of compresse stream</param>
        /// <returns>A GByteArray that the caller is responsible for freeing</returns>
        internal byte[] InflateMSOLE(long offset)
        {
            uint pos = 0;
            uint shift;
            byte[] flag = new byte[1];
            byte[] buffer = new byte[VBA_COMPRESSION_WINDOW];
            byte[] tmp = new byte[2];
            bool clean = true;

            if (Seek(offset, SeekOrigin.Begin))
                return null;

            byte[] res = new byte[0];

            // Explaination from libole2/ms-ole-vba.c

            // The first byte is a flag byte.  Each bit in this byte
            // determines what the next byte is.  If the bit is zero,
            // the next byte is a character.  Otherwise the  next two
            // bytes contain the number of characters to copy from the
            // umcompresed buffer and where to copy them from (offset,
            // length).

            while (Read(1, flag) != null)
            {
                for (uint mask = 1; mask < 0x100; mask <<= 1)
                {
                    if ((flag[0] & mask) != 0)
                    {
                        if ((tmp = Read(2, null)) == null)
                            break;

                        uint win_pos = pos % VBA_COMPRESSION_WINDOW;
                        if (win_pos <= 0x80)
                        {
                            if (win_pos <= 0x20)
                                shift = (uint)((win_pos <= 0x10) ? 12 : 11);
                            else
                                shift = (uint)((win_pos <= 0x40) ? 10 : 9);
                        }
                        else
                        {
                            if (win_pos <= 0x200)
                                shift = (uint)((win_pos <= 0x100) ? 8 : 7);
                            else if (win_pos <= 0x800)
                                shift = (uint)((win_pos <= 0x400) ? 6 : 5);
                            else
                                shift = 4;
                        }

                        ushort token = GSF_LE_GET_GUINT16(tmp, 0);
                        ushort len = (ushort)((token & ((1 << (int)shift) - 1)) + 3);
                        uint distance = (uint)(token >> (int)shift);
                        clean = true;

                        if (distance >= pos)
                        {
                            Console.Error.WriteLine("Corrupted compressed stream");
                            break;
                        }

                        for (uint i = 0; i < len; i++)
                        {
                            uint srcpos = (pos - distance - 1) % VBA_COMPRESSION_WINDOW;
                            byte c = buffer[srcpos];
                            buffer[pos++ % VBA_COMPRESSION_WINDOW] = c;
                        }
                    }
                    else
                    {
                        if ((pos != 0) && ((pos % VBA_COMPRESSION_WINDOW) == 0) && clean)
                        {
                            Read(2, null);
                            clean = false;

                            List<byte> temp = new List<byte>(res);
                            temp.AddRange(buffer);
                            res = temp.ToArray();
                            break;
                        }

                        if (Read(1, buffer, (int)(pos % VBA_COMPRESSION_WINDOW)) != null)
                            pos++;

                        clean = true;
                    }
                }
            }

            if ((pos % VBA_COMPRESSION_WINDOW) != 0)
            {
                List<byte> temp = new List<byte>(res);
                temp.AddRange(buffer.Skip((int)(pos % VBA_COMPRESSION_WINDOW)));
                res = temp.ToArray();
            }

            return res;
        }

        #endregion

        #region MS-VBA

        /// <summary>
        /// Decompresses VBA stream.
        /// </summary>
        /// <param name="offset">Offset into it for start byte of compressed stream</param>
        /// <param name="size">Size of the returned array</param>
        /// <param name="add_null_terminator">Whenever add or not null at the end of array</param>
        /// <returns>A pointer to byte array</returns>
        internal byte[] InflateMSVBA(long offset, out int size, bool add_null_terminator)
        {
            size = 0;

            byte[] sig = new byte[1];
            Read(1, sig);
            if (sig[0] != 1) // Should start with 0x01
                return null;

            offset++;

            List<byte> res = new List<byte>();

            long length = Size;
            while (offset < length)
            {
                byte[] tmp = Read(2, null);
                if (tmp == null)
                    break;

                ushort chunk_hdr = GSF_LE_GET_GUINT16(tmp, 0);
                offset += 2;

                GsfInput chunk;
                if (0xB000 == (chunk_hdr & 0xF000) && (chunk_hdr & 0xFFF) > 0 && (length - offset < 4094))
                {
                    if (length < offset + (chunk_hdr & 0xFFF))
                        break;

                    chunk = GsfInputProxy.Create(this, offset, (long)(chunk_hdr & 0xFFF) + 1);
                    offset += (chunk_hdr & 0xFFF) + 1;
                }
                else
                {
                    if (length < offset + 4094)
                    {
                        chunk = GsfInputProxy.Create(this, offset, length - offset);
                        offset = length;
                    }
                    else
                    {
                        chunk = GsfInputProxy.Create(this, offset, 4094);
                        offset += 4094;
                    }
                }

                if (chunk != null)
                {
                    byte[] tmpres = chunk.InflateMSOLE(0);
                    Seek(offset, SeekOrigin.Current);
                    res.AddRange(tmpres);
                }
            }

            if (res == null)
                return null;
            if (add_null_terminator)
                res.Add(0x00);

            size = res.Count;

            return res.ToArray();
        }

        #endregion

        #region Virtual Functions

        protected virtual GsfInput DupImpl(ref Exception err) => null;

        protected virtual GsfInput OpenSiblingImpl(string name, ref Exception err) => null;

        protected virtual byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int bufferPtr = 0) => null;

        protected virtual bool SeekImpl(long offset, SeekOrigin whence) => false;

        #endregion
    }
}
