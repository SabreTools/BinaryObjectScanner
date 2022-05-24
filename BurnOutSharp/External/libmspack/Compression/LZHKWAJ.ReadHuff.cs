/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    public partial class LZHKWAJ : CompressionStream
    {
        /// <inheritdoc/>
        public override Error HUFF_ERROR() => Error.MSPACK_ERR_DATAFORMAT;
    }
}
