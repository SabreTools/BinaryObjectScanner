/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output.c: interface for storing data
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
using System.Text;

namespace LibGSF.Output
{
    public abstract class GsfOutput : IDisposable
    {
        #region Properties

        public long CurrentSize { get; protected set; } = 0;

        public long CurrentOffset { get; protected set; } = 0;

        public string Name { get; protected set; } = null;

        public object WrappedBy { get; protected set; } = null;

        public GsfOutfile Container { get; protected set; } = null;

        public Exception Error { get; protected set; } = null;

        public bool IsClosed { get; protected set; } = false;

        public string PrintFBuf { get; protected set; } = null;

        public int PrintFBufSize { get; protected set; } = 0;

        public DateTime? ModTime { get; protected set; }

        #endregion

        #region Functions

        /// <summary>
        /// Close a stream.
        /// </summary>
        /// <returns>false on error</returns>
        public bool Close()
        {
            if (IsClosed)
                return SetError(0, "<internal>");

            // The implementation will log any errors, but we can never try to
            // close multiple times even on failure.
            bool res = CloseImpl();
            IsClosed = true;
            return res;
        }

        /// <summary>
        /// Reposition in output stream @output. @whence specifies what the offset is
        /// relative to: the beginning of the stream (SeekOrigin.Begin), current position in
        /// the stream (SeekOrigin.Current) or the end of the stream (SeekOrigin.End).
        /// This function is similar to fseek(3)
        /// </summary>
        /// <param name="offset">Relative amount to reposition</param>
        /// <param name="whence">What the offset is relative to.</param>
        /// <returns>false on error.</returns>
        public bool Seek(long offset, SeekOrigin whence)
        {
            long pos = offset;

            switch (whence)
            {
                case SeekOrigin.Begin: break;
                case SeekOrigin.Current: pos += CurrentOffset; break;
                case SeekOrigin.End: pos += CurrentSize; break;
                default:
                    Console.Error.WriteLine($"Invalid seek type {whence}");
                    return false;
            }

            if (pos < 0)
            {
                Console.Error.WriteLine($"Invalid seek position {pos}, which is before the start of the file");
                return false;
            }

            // If we go nowhere, just return.  This in particular handles null
            // seeks for streams with no seek method.
            if (pos == CurrentOffset)
                return true;

            if (SeekImpl(offset, whence))
            {
                // NOTE : it is possible for the current pos to be beyond the
                // end of the file.  The intervening space is not filled with 0
                // until something is written.
                CurrentOffset = pos;
                return true;
            }

            // The implementation should have assigned whatever errors are necessary
            return false;
        }

        /// <summary>
        /// Write <paramref name="num_bytes"/> of <paramref name="data"/> to output.
        /// </summary>
        /// <param name="num_bytes">Number of bytes to write</param>
        /// <param name="data">Data to write.</param>
        /// <returns>%false on error.</returns>
        public bool Write(int num_bytes, byte[] data)
        {
            if (num_bytes == 0)
                return true;

            if (WriteImpl(num_bytes, data))
                return IncrementCurrentOffset(num_bytes);

            // The implementation should have assigned whatever errors are necessary
            return false;
        }

        /// <returns>True if the wrapping succeeded.</returns>
        public bool Wrap(object wrapper)
        {
            if (wrapper == null)
                return false;

            if (WrappedBy != null)
            {
                Console.Error.WriteLine("Attempt to wrap an output that is already wrapped.");
                return false;
            }

            WrappedBy = wrapper;
            return true;
        }

        public bool Unwrap(object wrapper)
        {
            if (WrappedBy != wrapper)
                return false;

            WrappedBy = null;
            return true;
        }

        /// <summary>
        /// Output <paramref name="va"/> to output using the format string <paramref name="format"/>, similar to printf(3)
        /// </summary>
        /// <param name="format">The printf-style format string</param>
        /// <param name="va">The arguments for @format</param>
        /// <returns>True if successful, false if not</returns>
        public bool PrintF(string format, params string[] va) => VPrintF(format, va) >= 0;

        /// <summary>
        /// Output <paramref name="args"/> to output using the format string <paramref name="format"/>, similar to vprintf(3)
        /// </summary>
        /// <param name="format">The printf-style format string</param>
        /// <param name="args">The arguments for @format</param>
        /// <returns>Number of bytes printed, a negative value if not successful</returns>
        public long VPrintF(string format, params string[] args)
        {
            if (format == null)
                return -1;

            long num_bytes = VPrintFImpl(format, args);
            if (num_bytes >= 0)
            {
                if (!IncrementCurrentOffset(num_bytes))
                    return -1;
            }

            return num_bytes;
        }

        /// <summary>
        /// Like fputs, this assumes that the line already ends with a newline
        /// </summary>
        /// <param name="line">Nul terminated string to write</param>
        /// <returns>%true if successful, %false if not</returns>
        public bool PutString(string line)
        {
            if (line == null)
                return false;

            int nbytes = line.Length;
            return Write(nbytes, Encoding.UTF8.GetBytes($"{line}"));
        }

        #endregion

        #region Virtual Functions

        public virtual void Dispose()
        {
            if (!IsClosed)
            {
                Console.Error.WriteLine("Disposing of an unclosed stream");
                Close();
            }

            Container = null;
            Name = null;
            ModTime = null;

            PrintFBuf = null;

            Error = null;
        }

        protected virtual bool CloseImpl() => false;

        protected virtual bool SeekImpl(long offset, SeekOrigin whence) => false;

        protected virtual bool WriteImpl(int num_bytes, byte[] data) => false;

        protected virtual long VPrintFImpl(string format, params string[] args) => -1;

        #endregion

        #region Utilities

        private bool IncrementCurrentOffset(long num_bytes)
        {
            CurrentOffset += num_bytes;
            if (CurrentOffset < num_bytes)
                return SetError(0, "Output size overflow.");

            if (CurrentSize < CurrentOffset)
                CurrentSize = CurrentOffset;

            return true;
        }

        /// <param name="filename">The (fs-sys encoded) filename</param>
        /// <returns> %true if the assignment was ok.</returns>
        /// <remarks>This is a utility routine that should only be used by derived outputs.</remarks>
        protected bool SetNameFromFilename(string filename)
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

        /// <param name="code">The error id</param>
        /// <param name="format">printf style format string</param>
        /// <param name="va">arguments for @format</param>
        /// <returns>Always returns false to facilitate its use.</returns>
        /// <remarks>This is a utility routine that should only be used by derived outputs.</remarks>
        protected bool SetError(int code, string format, params string[] va)
        {
            Error = null;
            if (format != null)
            {
                string message = string.Format(format, va);
                Error = new Exception(message); // TODO: How to include `code`?
            }

            return false;
        }

        #endregion
    }
}
