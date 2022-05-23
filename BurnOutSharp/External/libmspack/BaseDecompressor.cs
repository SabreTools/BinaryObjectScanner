/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp
{
    /// <summary>
    /// Set of common fields shared by all decompressors
    /// </summary>
    public abstract class BaseDecompressor
    {
        /// <summary>
        /// System wrapper for I/O operations
        /// </summary>
        public SystemImpl System { get; set; }

        /// <summary>
        /// Persistent error state of the decompressor
        /// </summary>
        public Error Error { get; set; }

        /// <summary>
        /// Size of the internal buffer
        /// </summary>
        public int BufferSize { get; set; }
    }
}
