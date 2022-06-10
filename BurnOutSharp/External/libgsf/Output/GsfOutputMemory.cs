/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-memory.c:
 *
 * Copyright (C) 2002-2006 Dom Lachowicz (cinamod@hotmail.com)
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
    public class GsfOutputMemory : GsfOutput
    {
        #region Constants

        public const int MIN_BLOCK = 512;

        public const int MAX_STEP = MIN_BLOCK * 128;

        #endregion

        #region Properties

        public byte[] Buffer { get; set; } = null;

        public int Capacity { get; set; } = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutputMemory() { }

        /// <returns>A new file.</returns>
        public static GsfOutputMemory Create() => new GsfOutputMemory();

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool CloseImpl() => true;

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => true;

        public bool Expand(long needed)
        {
            // If we need >= MAX_STEP, align to a next multiple of MAX_STEP.
            // Since MAX_STEP is probably a power of two, this computation
            // should reduce to "dec, shr, inc, shl", which is probably
            // quicker then branching.

            long capacity = Math.Max(Capacity, MIN_BLOCK);

            if (needed < MAX_STEP)
            {
                while (capacity < needed)
                    capacity *= 2;
            }
            else
            {
                capacity = ((needed - 1) / MAX_STEP + 1) * MAX_STEP;
            }

            // Check for overflow: g_renew() casts its parameters to int.
            int lcapacity = (int)capacity;
            if ((long)lcapacity != capacity || capacity < 0)
            {
                Console.Error.WriteLine("Overflow in Expand");
                return false;
            }

            byte[] tempBuffer = new byte[lcapacity];
            Array.Copy(Buffer, tempBuffer, Buffer.Length);
            Buffer = tempBuffer;
            Capacity = lcapacity;

            return true;
        }

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
            if (Buffer == null)
            {
                Buffer = new byte[MIN_BLOCK];
                Capacity = MIN_BLOCK;
            }

            if (num_bytes + CurrentOffset > Capacity)
            {
                if (!Expand(CurrentOffset + num_bytes))
                    return false;
            }

            Array.Copy(data, 0, Buffer, CurrentOffset, num_bytes);
            return true;
        }

        /// <inheritdoc/>
        protected override long VPrintFImpl(string format, params string[] args)
        {
            if (Buffer != null)
            {
                byte[] temp = Encoding.UTF8.GetBytes(string.Format(format, args));
                long len = Math.Min(temp.Length, Capacity - CurrentOffset);
                Array.Copy(temp, 0, Buffer, CurrentOffset, len);

                // There was insufficient space
                if (temp.Length >= len)
                    len = base.VPrintFImpl(format, args);

                return len;
            }

            return base.VPrintFImpl(format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// The data that has been written to mem.
        /// The caller takes ownership and the buffer belonging to mem is set
        /// to NULL.
        /// </returns>
        public byte[] StealBytes()
        {
            byte[] bytes = Buffer;
            Buffer = null;
            Capacity = 0;
            return bytes;
        }

        #endregion
    }
}
