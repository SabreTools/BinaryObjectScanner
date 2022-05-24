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
    public partial class LZX : CompressionStream
    {
        /// <inheritdoc/>
        public override void READ_BYTES()
        {
            READ_IF_NEEDED();
            if (Error != Error.MSPACK_ERR_OK)
                return;

            byte b0 = InputBuffer[InputPointer++];

            READ_IF_NEEDED();
            if (Error != Error.MSPACK_ERR_OK)
                return;

            byte b1 = InputBuffer[InputPointer++];
            INJECT_BITS_MSB((b1 << 8) | b0, 16);
        }
    }
}
