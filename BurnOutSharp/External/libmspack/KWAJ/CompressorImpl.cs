/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.KWAJ
{
    public class CompressorImpl : Compressor
    {
        public SystemImpl System { get; set; }

        // TODO

        /// <remarks>
        /// !!! MATCH THIS TO NUM OF PARAMS IN MSPACK.H !!!
        /// </remarks>
        public int[] Param { get; set; } = new int[2];
        
        public Error Error { get; set; }
    }
}
