/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System.IO;

namespace LibMSPackSharp
{
    public abstract class BaseDecompressState
    {
        /// <summary>
        /// System wrapper for I/O operations
        /// </summary>
        public SystemImpl System { get; set; }

        /// <summary>
        /// Persistent error state of the state
        /// </summary>
        public Error Error { get; set; }

        /// <summary>
        /// Input file handle
        /// </summary>
        public FileStream InputFileHandle { get; set; }

        /// <summary>
        /// Output file handle
        /// </summary>
        public FileStream OutputFileHandle { get; set; }
    }
}
