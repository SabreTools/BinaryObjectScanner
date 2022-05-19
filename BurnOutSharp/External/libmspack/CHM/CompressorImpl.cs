/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.CHM
{
    public class CompressorImpl : Compressor
    {
        public SystemImpl System { get; set; }

        public string TempFile { get; set; }

        public bool UseTempFile { get; set; }

        public Error Error { get; set; }
    }
}
