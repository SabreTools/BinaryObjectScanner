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

using System;
using System.Linq;
using static LibMSPackSharp.Compression.Constants;

namespace LibMSPackSharp.Compression
{
    /* This header defines macros that read data streams by
    * the individual bits
    *
    * INIT_BITS         initialises bitstream state in state structure
    * STORE_BITS        stores bitstream state in state structure
    * RESTORE_BITS      restores bitstream state from state structure
    * ENSURE_BITS(n)    ensure there are at least N bits in the bit buffer
    * READ_BITS(var,n)  takes N bits from the buffer and puts them in var
    * PEEK_BITS(n)      extracts without removing N bits from the bit buffer
    * REMOVE_BITS(n)    removes N bits from the bit buffer
    *
    * READ_BITS simply calls ENSURE_BITS, PEEK_BITS and REMOVE_BITS,
    * which means it's limited to reading the number of bits you can
    * ensure at any one time. It also fails if asked to read zero bits.
    * If you need to read zero bits, or more bits than can be ensured in
    * one go, use READ_MANY_BITS instead.
    *
    * These macros have variable names baked into them, so to use them
    * you have to define some macros:
    * - BITS_TYPE: the type name of your state structure
    * - BITS_VAR: the variable that points to your state structure
    * - define BITS_ORDER_MSB if bits are read from the MSB, or
    *   define BITS_ORDER_LSB if bits are read from the LSB
    * - READ_BYTES: some code that reads more data into the bit buffer,
    *   it should use READ_IF_NEEDED (calls read_input if the byte buffer
    *   is empty), then INJECT_BITS(data,n) to put data from the byte
    *   buffer into the bit buffer.
    *
    * You also need to define some variables and structure members:
    * - byte[] i_ptr;    // current position in the byte buffer
    * - byte[] i_end;    // end of the byte buffer
    * - uint bit_buffer; // the bit buffer itself
    * - uint bits_left;  // number of bits remaining
    *
    * If you use read_input() and READ_IF_NEEDED, they also expect these
    * structure members:
    * - struct mspack_system *sys;  // to access sys->read()
    * - uint error;         // to record/return read errors
    * - byte input_end;    // to mark reaching the EOF
    * - byte[] inbuf;       // the input byte buffer
    * - uint inbuf_size;    // the size of the input byte buffer
    *
    * Your READ_BYTES implementation should read data from *i_ptr and
    * put them in the bit buffer. READ_IF_NEEDED will call read_input()
    * if i_ptr reaches i_end, and will fill up inbuf and set i_ptr to
    * the start of inbuf and i_end to the end of inbuf.
    *
    * If you're reading in MSB order, the routines work by using the area
    * beyond the MSB and the LSB of the bit buffer as a free source of
    * zeroes when shifting. This avoids having to mask any bits. So we
    * have to know the bit width of the bit buffer variable. We use
    * <limits.h> and CHAR_BIT to find the size of the bit buffer in bits.
    *
    * If you are reading in LSB order, bits need to be masked. Normally
    * this is done by computing the mask: N bits are masked by the value
    * (1<<N)-1). However, you can define BITS_LSB_TABLE to use a lookup
    * table instead of computing this. This adds two new macros,
    * PEEK_BITS_T and READ_BITS_T which work the same way as PEEK_BITS
    * and READ_BITS, except they use this lookup table. This is useful if
    * you need to look up a number of bits that are only known at
    * runtime, so the bit mask can't be turned into a constant by the
    * compiler.
    * The bit buffer datatype should be at least 32 bits wide: it must be
    * possible to ENSURE_BITS(17), so it must be possible to add 16 new bits
    * to the bit buffer when the bit buffer already has 1 to 15 bits left.
    */
    public abstract partial class CompressionStream : BaseDecompressState
    {
        #region Common

        /// <summary>
        /// Initialises bitstream state in state structure
        /// </summary>
        public void INIT_BITS()
        {
            InputPointer = 0;
            InputEnd = 0;
            BitBuffer = 0;
            BitsLeft = 0;
            EndOfInput = 0;
        }

        /// <summary>
        /// Ensure there are at least N bits in the bit buffer
        /// </summary>
        public void ENSURE_BITS(int nbits)
        {
            while (BitsLeft < nbits)
            {
                READ_BYTES();
                if (Error != Error.MSPACK_ERR_OK)
                    return;
            }
        }

        /// <summary>
        /// Read from the input if the buffer is empty
        /// </summary>
        public void READ_IF_NEEDED()
        {
            if (InputPointer >= InputEnd)
                ReadInput();
        }

        /// <summary>
        /// Read bytes from the input into the bit buffer
        /// </summary>
        public abstract void READ_BYTES();

        /// <summary>
        /// Read an input stream and fill the buffer
        /// </summary>
        protected virtual void ReadInput()
        {
            int read = System.Read(InputFileHandle, InputBuffer, 0, (int)InputBufferSize);
            if (read < 0)
            {
                Error = Error.MSPACK_ERR_READ;
                return;
            }

            // We might overrun the input stream by asking for bits we don't use,
            // so fake 2 more bytes at the end of input
            if (read == 0)
            {
                if (EndOfInput != 0)
                {
                    Console.WriteLine("Out of input bytes");
                    Error = Error.MSPACK_ERR_READ;
                    return;
                }
                else
                {
                    read = 2;
                    InputBuffer[0] = InputBuffer[1] = 0;
                    EndOfInput = 1;
                }
            }

            // Update i_ptr and i_end
            InputPointer = 0;
            InputEnd = read;
        }

        #endregion

        #region MSB

        /// <summary>
        /// Inject data into the bit buffer
        /// </summary>
        public void INJECT_BITS_MSB(int bitdata, int nbits)
        {
            BitBuffer |= ((uint)bitdata << (BITBUF_WIDTH - nbits - BitsLeft));
            BitsLeft += nbits;
        }

        /// <summary>
        /// Extracts without removing N bits from the bit buffer
        /// </summary>
        public long PEEK_BITS_MSB(int nbits) => (BitBuffer >> (BITBUF_WIDTH - (nbits)));

        /// <summary>
        /// Takes N bits from the buffer and puts them in var
        /// </summary>
        public long READ_BITS_MSB(int nbits)
        {
            ENSURE_BITS(nbits);
            if (Error != Error.MSPACK_ERR_OK)
                return -1;

            long temp = PEEK_BITS_MSB(nbits);

            REMOVE_BITS_MSB(nbits);
            return temp;
        }

        /// <summary>
        /// Read multiple bits and put them in var
        /// </summary>
        public long READ_MANY_BITS_MSB(int nbits)
        {
            byte needed = (byte)(nbits), bitrun;
            long temp = 0;
            while (needed > 0)
            {
                if (BitsLeft <= (BITBUF_WIDTH - 16))
                {
                    READ_BYTES();
                    if (Error != Error.MSPACK_ERR_OK)
                        return -1;
                }

                bitrun = (byte)((BitsLeft < needed) ? BitsLeft : needed);
                temp = (temp << bitrun) | PEEK_BITS_MSB(bitrun);
                REMOVE_BITS_MSB(bitrun);
                needed -= bitrun;
            }

            return temp;
        }

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
        /// Inject data into the bit buffer
        /// </summary>
        public void INJECT_BITS_LSB(int bitdata, int nbits)
        {
            BitBuffer |= (uint)(bitdata << BitsLeft);
            BitsLeft += nbits;
        }

        /// <summary>
        /// Extracts without removing N bits from the bit buffer
        /// </summary>
        public long PEEK_BITS_LSB(int nbits) => (BitBuffer & ((1 << (nbits)) - 1));

        /// <summary>
        /// Extracts without removing N bits from the bit buffer using a bit mask
        /// </summary>
        public long PEEK_BITS_T_LSB(int nbits) => BitBuffer & LSBBitMask[(nbits)];

        /// <summary>
        /// Takes N bits from the buffer and puts them in var
        /// </summary>
        public long READ_BITS_LSB(int nbits)
        {
            ENSURE_BITS(nbits);
            if (Error != Error.MSPACK_ERR_OK)
                return -1;

            long temp = PEEK_BITS_LSB(nbits);

            REMOVE_BITS_LSB(nbits);
            return temp;
        }

        /// <summary>
        /// Takes N bits from the buffer and puts them in var using a bit mask
        /// </summary>
        public long READ_BITS_T_LSB(int nbits)
        {
            ENSURE_BITS(nbits);
            if (Error != Error.MSPACK_ERR_OK)
                return -1;

            long temp = PEEK_BITS_T_LSB(nbits);

            REMOVE_BITS_LSB(nbits);
            return temp;
        }

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
