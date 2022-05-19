/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * LZSS is a derivative of LZ77 and was created by James Storer and
 * Thomas Szymanski in 1982. Haruhiko Okumura wrote a very popular C
 * implementation.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    public class LZSS
    {
        #region LZSS compression / decompression definitions

        public const int LZSS_WINDOW_SIZE = 4096;
        public const byte LZSS_WINDOW_FILL = 0x20;

        #endregion

        /// <summary>
        /// Decompresses an LZSS stream.
        /// 
        /// Input bytes will be read in as necessary using the system.read()
        /// function with the input file handle given.This will continue until
        /// system.read() returns 0 bytes, or an error.Errors will be passed
        /// out of the function as MSPACK_ERR_READ errors. Input streams should
        /// convey an "end of input stream" by refusing to supply all the bytes
        /// that LZSS asks for when they reach the end of the stream, rather
        /// than return an error code.
        /// 
        /// Output bytes will be passed to the system.write() function, using
        /// the output file handle given.More than one call may be made to
        /// system.write().
        /// 
        /// As EXPAND.EXE (SZDD/KWAJ), Microsoft Help and QBasic have slightly
        /// different encodings for the control byte and matches, a "mode"
        /// parameter is allowed, to choose the encoding.
        /// </summary>
        /// <param name="system">
        /// an mspack_system structure used to read from
        /// the input stream and write to the output
        /// stream, also to allocate and free memory.
        /// </param>
        /// <param name="input">an input stream with the LZSS data.</param>
        /// <param name="output">an output stream to write the decoded data to.</param>
        /// <param name="inputBufferSize">
        /// the number of bytes to use as an input
        /// bitstream buffer.
        /// </param>
        /// <param name="mode">one of LZSSMode values</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public static Error Decompress(SystemImpl system, object input, object output, int inputBufferSize, LZSSMode mode)
        {
            uint i, c, mpos, len;
            int read;

            // Check parameters
            if (system == null
                || inputBufferSize < 1
                || (mode != LZSSMode.LZSS_MODE_EXPAND && mode != LZSSMode.LZSS_MODE_MSHELP && mode != LZSSMode.LZSS_MODE_QBASIC))
            {
                return Error.MSPACK_ERR_ARGS;
            }

            // Allocate memory
            byte[] window = new byte[LZSS_WINDOW_SIZE + inputBufferSize];

            // Initialise decompression
            int inbuf = LZSS_WINDOW_SIZE;
            for (i = 0; i < LZSS_WINDOW_SIZE; i++)
            {
                window[i] = LZSS_WINDOW_FILL;
            }

            uint pos = LZSS_WINDOW_SIZE - (uint)((mode == LZSSMode.LZSS_MODE_QBASIC) ? 18 : 16);
            uint invert = (uint)((mode == LZSSMode.LZSS_MODE_MSHELP) ? ~0 : 0);
            int iPtr = 0, iEnd = 0;

            // Loop forever; exit condition is in ENSURE_BYTES macro
            for (; ; )
            {
                //ENSURE_BYTES
                if (iPtr >= iEnd)
                {
                    read = system.Read(input, window, inbuf, inputBufferSize);
                    if (read <= 0)
                    {
                        system.Free(window);
                        return (read < 0) ? Error.MSPACK_ERR_READ : Error.MSPACK_ERR_OK;
                    }

                    iPtr = 0;
                    iEnd = read;
                }

                c = window[iPtr++] ^ invert;
                for (i = 0x01; (i & 0xFF) != 0; i <<= 1)
                {
                    if (c != 0 & i != 0)
                    {
                        // Literal
                        //ENSURE_BYTES
                        if (iPtr >= iEnd)
                        {
                            read = system.Read(input, window, inbuf, inputBufferSize);
                            if (read <= 0)
                            {
                                system.Free(window);
                                return (read < 0) ? Error.MSPACK_ERR_READ : Error.MSPACK_ERR_OK;
                            }

                            iPtr = 0;
                            iEnd = read;
                        }

                        window[pos] = window[iPtr++];

                        //ENSURE_BYTES
                        if (iPtr >= iEnd)
                        {
                            read = system.Read(input, window, inbuf, inputBufferSize);
                            if (read <= 0)
                            {
                                system.Free(window);
                                return (read < 0) ? Error.MSPACK_ERR_READ : Error.MSPACK_ERR_OK;
                            }

                            iPtr = 0;
                            iEnd = read;
                        }

                        pos++; pos &= LZSS_WINDOW_SIZE - 1;
                    }
                    else
                    {
                        // Match
                        //ENSURE_BYTES
                        if (iPtr >= iEnd)
                        {
                            read = system.Read(input, window, inbuf, inputBufferSize);
                            if (read <= 0)
                            {
                                system.Free(window);
                                return (read < 0) ? Error.MSPACK_ERR_READ : Error.MSPACK_ERR_OK;
                            }

                            iPtr = 0;
                            iEnd = read;
                        }

                        mpos = window[iPtr++];

                        //ENSURE_BYTES
                        if (iPtr >= iEnd)
                        {
                            read = system.Read(input, window, inbuf, inputBufferSize);
                            if (read <= 0)
                            {
                                system.Free(window);
                                return (read < 0) ? Error.MSPACK_ERR_READ : Error.MSPACK_ERR_OK;
                            }

                            iPtr = 0;
                            iEnd = read;
                        }

                        mpos |= (uint)(window[iPtr] & 0xF0) << 4;
                        len = (uint)(window[iPtr++] & 0x0F) + 3;
                        while (len-- != 0)
                        {
                            window[pos] = window[mpos];

                            //WRITE_BYTE;
                            if (system.Write(output, window, (int)pos, 1) != 1)
                            {
                                system.Free(window);
                                return Error.MSPACK_ERR_WRITE;
                            }

                            pos++;
                            pos &= LZSS_WINDOW_SIZE - 1;
                            mpos++;
                            mpos &= LZSS_WINDOW_SIZE - 1;
                        }
                    }
                }
            }
        }
    }
}
