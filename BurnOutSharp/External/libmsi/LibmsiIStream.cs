/*
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301, USA
 */

using System;
using System.IO;
using LibGSF.Input;

namespace LibMSI
{
    internal class LibmsiIStream : Stream
    {
        #region Properties

        public GsfInput Input { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiIStream() { }

        public static LibmsiIStream Create(GsfInput input)
        {
            Exception err = null;
            GsfInput dup = input.Duplicate(ref err);
            if (dup == null)
                return null;

            return new LibmsiIStream
            {
                Input = dup,
            };
        }

        #endregion

        #region Functions

        public override long Seek(long offset, SeekOrigin type) => (Input?.Seek(offset, type) ?? false) ? offset : -1;

        public override int Read(byte[] buffer, int offset, int count)
        {
            int remaining = (int)(Input?.Remaining() ?? 0);
            if (remaining == 0)
                return 0;

            count = Math.Min(count, remaining);
            if (Input.Read(count, buffer, offset) == null)
                return -1;

            return count;
        }

        public override void Close() { }

        #endregion

        #region Stream Implementation

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => Input.Size;

        public override long Position { get => Input.CurrentOffset; set => Input.CurrentOffset = value; }

        public override void Flush() { }

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        #endregion
    }
}