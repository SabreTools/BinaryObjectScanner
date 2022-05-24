/* This file is part of libmspack.
 * (C) 2003-2013 Stuart Caie.
 *
 * The LZX method was created by Jonathan Forbes and Tomi Poutanen, adapted
 * by Microsoft Corporation.
 *
 * libmspack is free software { get; set; } you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    public class BufferState
    {
        /// <summary>
        /// i_ptr
        /// </summary>
        public int InputPointer { get; set; }

        /// <summary>
        /// i_end
        /// </summary>
        public int InputEnd { get; set; }

        /// <summary>
        /// bit_buffer
        /// </summary>
        public uint BitBuffer { get; set; }

        /// <summary>
        /// bits_left
        /// </summary>
        public int BitsLeft { get; set; }

        #region Common

        /// <summary>
        /// Initialises bitstream state in state structure
        /// </summary>
        public void Init()
        {
            InputPointer = 0;
            InputEnd = 0;
            BitBuffer = 0;
            BitsLeft = 0;
        }

        #endregion

        #region MSB

        /// <summary>
        /// Removes N bits from the bit buffer
        /// </summary>
        public void REMOVE_BITS_MSB(int nbits)
        {
            BitBuffer <<= nbits;
            BitsLeft -= nbits;
        }

        #endregion

        #region LSB

        /// <summary>
        /// Removes N bits from the bit buffer
        /// </summary>
        public void REMOVE_BITS_LSB(int nbits)
        {
            BitBuffer >>= nbits;
            BitsLeft -= nbits;
        }

        #endregion
    }
}
