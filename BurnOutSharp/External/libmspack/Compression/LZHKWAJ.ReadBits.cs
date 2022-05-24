/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using static LibMSPackSharp.Constants;

namespace LibMSPackSharp.Compression
{
    public partial class LZHKWAJ : CompressionStream
    {
        /// <inheritdoc/>
        public override void READ_BYTES()
        {
            if (InputPointer >= InputEnd)
            {
                ReadInput();
                if (Error != Error.MSPACK_ERR_OK)
                    return;

                InputPointer = InputPointer;
                InputEnd = InputEnd;
            }

            INJECT_BITS_MSB(InputBuffer[InputPointer++], 8);
        }

        /// <inheritdoc/>
        protected override void ReadInput()
        {
            int read;
            if (EndOfInput != 0)
            {
                EndOfInput += 8;
                InputBuffer[0] = 0;
                read = 1;
            }
            else
            {
                read = System.Read(InputFileHandle, InputBuffer, 0, KWAJ_INPUT_SIZE);
                if (read < 0)
                {
                    Error = Error.MSPACK_ERR_READ;
                    return;
                }

                if (read == 0)
                {
                    InputEnd = 8;
                    InputBuffer[0] = 0;
                    read = 1;
                }
            }

            // Update InputPointer and InputLength
            InputPointer = 0;
            InputEnd = read;
        }
    }
}
