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
    public class None
    {
        public static NoneState Init(SystemImpl sys, FileStream input, FileStream output, int bufsize)
        {
            return new NoneState()
            {
                Sys = sys,
                Input = input,
                Output = output,
                Buffer = new byte[bufsize],
                BufferSize = bufsize,
            };
        }

        public static LibMSPackSharp.Error Decompress(object s, long bytes)
        {
            NoneState state = (NoneState)s;
            if (state == null)
                return LibMSPackSharp.Error.MSPACK_ERR_ARGS;

            int run;
            while (bytes > 0)
            {
                run = (bytes > state.BufferSize) ? state.BufferSize : (int)bytes;

                if (state.Sys.Read(state.Input, state.Buffer, 0, run) != run)
                    return LibMSPackSharp.Error.MSPACK_ERR_READ;

                if (state.Sys.Write(state.Output, state.Buffer, 0, run) != run)
                    return LibMSPackSharp.Error.MSPACK_ERR_WRITE;

                bytes -= run;
            }
            return LibMSPackSharp.Error.MSPACK_ERR_OK;
        }
    }
}
