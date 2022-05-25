/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System.IO;

namespace LibMSPackSharp.Compression
{
    public partial class None
    {
        public static None Init(SystemImpl sys, FileStream input, FileStream output, int bufsize)
        {
            return new None()
            {
                System = sys,
                InputFileHandle = input,
                OutputFileHandle = output,
                Buffer = new byte[bufsize],
                BufferSize = bufsize,
            };
        }

        public Error Decompress(long bytes)
        {
            int run;
            while (bytes > 0)
            {
                run = (bytes > BufferSize) ? BufferSize : (int)bytes;

                if (System.Read(InputFileHandle, Buffer, 0, run) != run)
                    return Error.MSPACK_ERR_READ;

                if (System.Write(OutputFileHandle, Buffer, 0, run) != run)
                    return Error.MSPACK_ERR_WRITE;

                bytes -= run;
            }
            return Error.MSPACK_ERR_OK;
        }
    }
}
