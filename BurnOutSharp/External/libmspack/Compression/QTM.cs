/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * The Quantum method was created by David Stafford, adapted by Microsoft
 * Corporation.
 *
 * This decompressor is based on an implementation by Matthew Russotto, used
 * with permission.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

/* Quantum decompression implementation */

/* This decompressor was researched and implemented by Matthew Russotto. It
 * has since been tidied up by Stuart Caie. More information can be found at
 * http://www.speakeasy.org/~russotto/quantumcomp.html
 */

using System;
using System.IO;

namespace LibMSPackSharp.Compression
{
    public class QTM
    {
        public const int QTM_FRAME_SIZE = 32768;

        /* Quantum static data tables:
         *
         * Quantum uses 'position slots' to represent match offsets.  For every
         * match, a small 'position slot' number and a small offset from that slot
         * are encoded instead of one large offset.
         *
         * position_base[] is an index to the position slot bases
         *
         * extra_bits[] states how many bits of offset-from-base data is needed.
         *
         * length_base[] and length_extra[] are equivalent in function, but are
         * used for encoding selector 6 (variable length match) match lengths,
         * instead of match offsets.
         *
         * They are generated with the following code:
         *   uint i, offset;
         *   for (i = 0, offset = 0; i < 42; i++) {
         *     position_base[i] = offset;
         *     extra_bits[i] = ((i < 2) ? 0 : (i - 2)) >> 1;
         *     offset += 1 << extra_bits[i];
         *   }
         *   for (i = 0, offset = 0; i < 26; i++) {
         *     length_base[i] = offset;
         *     length_extra[i] = (i < 2 ? 0 : i - 2) >> 2;
         *     offset += 1 << length_extra[i];
         *   }
         *   length_base[26] = 254; length_extra[26] = 0;
         */

        private static readonly uint[] position_base = new uint[42]
            {
          0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 384, 512, 768,
          1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576, 32768, 49152,
          65536, 98304, 131072, 196608, 262144, 393216, 524288, 786432, 1048576, 1572864
        };

        private static readonly byte[] extra_bits = new byte[42]
        {
          0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10,
          11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 18, 18, 19, 19
        };

        private static readonly byte[] length_base = new byte[27]
            {
          0, 1, 2, 3, 4, 5, 6, 8, 10, 12, 14, 18, 22, 26,
          30, 38, 46, 54, 62, 78, 94, 110, 126, 158, 190, 222, 254
        };

        private static readonly byte[] length_extra = new byte[27]
            {
          0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
          3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
        };

        /// <summary>
        /// allocates Quantum decompression state for decoding the given stream.
        /// 
        ///  - returns null if window_bits is outwith the range 10 to 21 (inclusive).
        ///  - uses system.alloc() to allocate memory
        ///  - returns null if not enough memory
        ///  - window_bits is the size of the Quantum window, from 1Kb(10) to 2Mb(21).
        ///  - input_buffer_size is the number of bytes to use to store bitstream data.
        /// </summary>
        public static QTMDStream Init(SystemImpl system, FileStream input, FileStream output, int window_bits, int input_buffer_size)
        {
            uint window_size = (uint)(1 << window_bits);

            if (system == null)
                return null;

            // Quantum supports window sizes of 2^10 (1Kb) through 2^21 (2Mb)
            if (window_bits < 10 || window_bits > 21)
                return null;

            // Round up input buffer size to multiple of two
            input_buffer_size = (input_buffer_size + 1) & -2;
            if (input_buffer_size < 2)
                return null;

            // Allocate decompression state
            QTMDStream qtm = new QTMDStream()
            {
                // Allocate decompression window and input buffer
                Window = new byte[window_size],
                InputBuffer = new byte[input_buffer_size],

                // Initialise decompression state
                System = system,
                InputFileHandle = input,
                OutputFileHandle = output,
                InputBufferSize = (uint)input_buffer_size,
                WindowSize = window_size,
                WindowPosition = 0,
                FrameTODO = QTM_FRAME_SIZE,
                HeaderRead = 0,
                Error = Error.MSPACK_ERR_OK,

                InputPointer = 0,
                InputEnd = 0,
                OutputPointer = 0,
                OutputEnd = 0,
                EndOfInput = 0,
                BitsLeft = 0,
                BitBuffer = 0,
            };

            // Initialise arithmetic coding models
            // - model 4    depends on window size, ranges from 20 to 24
            // - model 5    depends on window size, ranges from 20 to 36
            // - model 6pos depends on window size, ranges from 20 to 42

            int i = window_bits * 2;
            InitModel(qtm.Model0, qtm.Model0Symbols, 0, 64);
            InitModel(qtm.Model1, qtm.Model1Symbols, 64, 64);
            InitModel(qtm.Model2, qtm.Model2Symbols, 128, 64);
            InitModel(qtm.Model3, qtm.Model3Symbols, 192, 64);
            InitModel(qtm.Model4, qtm.Model4Symbols, 0, (i > 24) ? 24 : i);
            InitModel(qtm.Model5, qtm.Model5Symbols, 0, (i > 36) ? 36 : i);
            InitModel(qtm.Model6, qtm.Model6Symbols, 0, i);
            InitModel(qtm.Model6Len, qtm.Model6LenSymbols, 0, 27);
            InitModel(qtm.Model7, qtm.Model7Symbols, 0, 7);

            // All ok
            return qtm;
        }

        /// <summary>
        /// Decompresses, or decompresses more of, a Quantum stream.
        /// 
        /// - out_bytes of data will be decompressed and the function will return
        ///   with an MSPACK_ERR_OK return code.
        ///
        /// - decompressing will stop as soon as out_bytes is reached. if the true
        ///   amount of bytes decoded spills over that amount, they will be kept for
        ///   a later invocation of qtmd_decompress().
        ///
        /// - the output bytes will be passed to the system.write() function given in
        ///   qtmd_init(), using the output file handle given in qtmd_init(). More
        ///   than one call may be made to system.write()
        ///
        /// - Quantum will read input bytes as necessary using the system.read()
        ///   function given in qtmd_init(), using the input file handle given in
        ///   qtmd_init(). This will continue until system.read() returns 0 bytes,
        ///   or an error.
        /// </summary>
        public static Error Decompress(object o, long out_bytes)
        {
            QTMDStream qtm = o as QTMDStream;
            if (qtm == null)
                return Error.MSPACK_ERR_ARGS;

            uint frame_todo, frame_end, window_posn, match_offset, range;
            byte[] window;
            int i_ptr, i_end, runsrc, rundest;
            int i, j, selector, extra, sym, match_length;
            ushort H, L, C, symf;

            uint bit_buffer;
            int bits_left;

            // Easy answers
            if (qtm == null || (out_bytes < 0))
                return Error.MSPACK_ERR_ARGS;

            if (qtm.Error != Error.MSPACK_ERR_OK)
                return qtm.Error;

            // Flush out any stored-up bytes before we begin
            i = qtm.OutputEnd - qtm.OutputPointer;
            if (i > out_bytes)
                i = (int)out_bytes;

            if (i != 0)
            {
                if (qtm.System.Write(qtm.OutputFileHandle, qtm.Window, qtm.OutputPointer, i) != i)
                    return qtm.Error = Error.MSPACK_ERR_WRITE;

                qtm.OutputPointer += i;
                out_bytes -= i;
            }

            if (out_bytes == 0)
                return Error.MSPACK_ERR_OK;

            // Restore local state
            qtm.RESTORE_BITS(out i_ptr, out i_end, out bit_buffer, out bits_left);
            window = qtm.Window;
            window_posn = qtm.WindowPosition;
            frame_todo = qtm.FrameTODO;
            H = qtm.High;
            L = qtm.Low;
            C = qtm.Current;

            // While we do not have enough decoded bytes in reserve
            while ((qtm.OutputEnd - qtm.OutputPointer) < out_bytes)
            {
                // Read header if necessary. Initialises H, L and C
                if (qtm.HeaderRead != 0)
                {
                    H = 0xFFFF;
                    L = 0;

                    //READ_BITS(C, 16)
                    {
                        //ENSURE_BITS(16)
                        {
                            while (bits_left < (16))
                            {
                                //READ_BYTES
                                {
                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                        return qtm.Error;

                                    byte b0 = qtm.InputBuffer[i_ptr++];

                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                        return qtm.Error;

                                    byte b1 = qtm.InputBuffer[i_ptr++];
                                    qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                }
                            }
                        }

                        (C) = (ushort)qtm.PEEK_BITS_MSB(16, bit_buffer);
                        qtm.REMOVE_BITS_MSB(16, ref bit_buffer, ref bits_left);
                    }

                    qtm.HeaderRead = 1;
                }

                // Decode more, up to the number of bytes needed, the frame boundary,
                // or the window boundary, whichever comes first
                frame_end = (uint)(window_posn + (out_bytes - (qtm.OutputEnd - qtm.OutputPointer)));
                if ((window_posn + frame_todo) < frame_end)
                    frame_end = window_posn + frame_todo;
                if (frame_end > qtm.WindowSize)
                    frame_end = qtm.WindowSize;

                while (window_posn < frame_end)
                {
                    //GET_SYMBOL(qtm.Model7, var)
                    {
                        range = (uint)((H - L) & 0xFFFF) + 1;
                        symf = (ushort)(((((C - L + 1) * qtm.Model7.Syms[0].CumulativeFrequency) - 1) / range) & 0xFFFF);

                        for (i = 1; i < qtm.Model7.Entries; i++)
                        {
                            if (qtm.Model7.Syms[i].CumulativeFrequency <= symf)
                                break;
                        }

                        (selector) = qtm.Model7.Syms[i - 1].Sym;

                        range = (uint)(H - L) + 1;
                        symf = qtm.Model7.Syms[0].CumulativeFrequency;
                        H = (ushort)(L + ((qtm.Model7.Syms[i - 1].CumulativeFrequency * range) / symf) - 1);
                        L = (ushort)(L + ((qtm.Model7.Syms[i].CumulativeFrequency * range) / symf));

                        do
                        {
                            qtm.Model7.Syms[--i].CumulativeFrequency += 8;
                        } while (i > 0);

                        if (qtm.Model7.Syms[0].CumulativeFrequency > 3800)
                            UpdateModel(qtm.Model7);

                        while (true)
                        {
                            if ((L & 0x8000) != (H & 0x8000))
                            {
                                if ((L & 0x4000) != 0 && (H & 0x4000) == 0)
                                {
                                    // Underflow case
                                    C ^= 0x4000;
                                    L &= 0x3FFF;
                                    H |= 0x4000;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            L <<= 1;
                            H = (ushort)((H << 1) | 1);

                            //ENSURE_BITS(1)
                            {
                                while (bits_left < (1))
                                {
                                    //READ_BYTES
                                    {
                                        qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                        if (qtm.Error != Error.MSPACK_ERR_OK)
                                            return qtm.Error;

                                        byte b0 = qtm.InputBuffer[i_ptr++];

                                        qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                        if (qtm.Error != Error.MSPACK_ERR_OK)
                                            return qtm.Error;

                                        byte b1 = qtm.InputBuffer[i_ptr++];
                                        qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                    }
                                }
                            }

                            C = (ushort)((C << 1) | (qtm.PEEK_BITS_MSB(1, bit_buffer)));
                            qtm.REMOVE_BITS_MSB(1, ref bit_buffer, ref bits_left);
                        }
                    }

                    if (selector < 4)
                    {
                        // Literal byte
                        QTMDModel mdl;
                        switch (selector)
                        {
                            case 0:
                                mdl = qtm.Model0;
                                break;

                            case 1:
                                mdl = qtm.Model1;
                                break;

                            case 2:
                                mdl = qtm.Model2;
                                break;

                            case 3:
                            default:
                                mdl = qtm.Model3;
                                break;
                        }

                        //GET_SYMBOL(mdl, sym)
                        {
                            range = (uint)((H - L) & 0xFFFF) + 1;
                            symf = (ushort)(((((C - L + 1) * mdl.Syms[0].CumulativeFrequency) - 1) / range) & 0xFFFF);

                            for (i = 1; i < mdl.Entries; i++)
                            {
                                if (mdl.Syms[i].CumulativeFrequency <= symf)
                                    break;
                            }

                            (sym) = mdl.Syms[i - 1].Sym;

                            range = (uint)(H - L) + 1;
                            symf = mdl.Syms[0].CumulativeFrequency;
                            H = (ushort)(L + ((mdl.Syms[i - 1].CumulativeFrequency * range) / symf) - 1);
                            L = (ushort)(L + ((mdl.Syms[i].CumulativeFrequency * range) / symf));

                            do
                            {
                                mdl.Syms[--i].CumulativeFrequency += 8;
                            } while (i > 0);

                            if (mdl.Syms[0].CumulativeFrequency > 3800)
                                UpdateModel(mdl);

                            while (true)
                            {
                                if ((L & 0x8000) != (H & 0x8000))
                                {
                                    if ((L & 0x4000) != 0 && (H & 0x4000) == 0)
                                    {
                                        // Underflow case
                                        C ^= 0x4000;
                                        L &= 0x3FFF;
                                        H |= 0x4000;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                L <<= 1;
                                H = (ushort)((H << 1) | 1);

                                //ENSURE_BITS(1)
                                {
                                    while (bits_left < (1))
                                    {
                                        //READ_BYTES
                                        {
                                            qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                            if (qtm.Error != Error.MSPACK_ERR_OK)
                                                return qtm.Error;

                                            byte b0 = qtm.InputBuffer[i_ptr++];

                                            qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                            if (qtm.Error != Error.MSPACK_ERR_OK)
                                                return qtm.Error;

                                            byte b1 = qtm.InputBuffer[i_ptr++];
                                            qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                        }
                                    }
                                }

                                C = (ushort)((C << 1) | (qtm.PEEK_BITS_MSB(1, bit_buffer)));
                                qtm.REMOVE_BITS_MSB(1, ref bit_buffer, ref bits_left);
                            }
                        }

                        window[window_posn++] = (byte)sym;
                        frame_todo--;
                    }
                    else
                    {
                        // Match repeated string
                        switch (selector)
                        {
                            // Selector 4 = fixed length match (3 bytes)
                            case 4:
                                //GET_SYMBOL(qtm.Model4, sym)
                                {
                                    range = (uint)((H - L) & 0xFFFF) + 1;
                                    symf = (ushort)(((((C - L + 1) * qtm.Model4.Syms[0].CumulativeFrequency) - 1) / range) & 0xFFFF);

                                    for (i = 1; i < qtm.Model4.Entries; i++)
                                    {
                                        if (qtm.Model4.Syms[i].CumulativeFrequency <= symf)
                                            break;
                                    }

                                    (sym) = qtm.Model4.Syms[i - 1].Sym;

                                    range = (uint)(H - L) + 1;
                                    symf = qtm.Model4.Syms[0].CumulativeFrequency;
                                    H = (ushort)(L + ((qtm.Model4.Syms[i - 1].CumulativeFrequency * range) / symf) - 1);
                                    L = (ushort)(L + ((qtm.Model4.Syms[i].CumulativeFrequency * range) / symf));

                                    do
                                    {
                                        qtm.Model4.Syms[--i].CumulativeFrequency += 8;
                                    } while (i > 0);

                                    if (qtm.Model4.Syms[0].CumulativeFrequency > 3800)
                                        UpdateModel(qtm.Model4);

                                    while (true)
                                    {
                                        if ((L & 0x8000) != (H & 0x8000))
                                        {
                                            if ((L & 0x4000) != 0 && (H & 0x4000) == 0)
                                            {
                                                // Underflow case
                                                C ^= 0x4000;
                                                L &= 0x3FFF;
                                                H |= 0x4000;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        L <<= 1;
                                        H = (ushort)((H << 1) | 1);

                                        //ENSURE_BITS(1)
                                        {
                                            while (bits_left < (1))
                                            {
                                                //READ_BYTES
                                                {
                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b0 = qtm.InputBuffer[i_ptr++];

                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b1 = qtm.InputBuffer[i_ptr++];
                                                    qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                                }
                                            }
                                        }

                                        C = (ushort)((C << 1) | (qtm.PEEK_BITS_MSB(1, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(1, ref bit_buffer, ref bits_left);
                                    }
                                }

                                //READ_MANY_BITS(extra, extra_bits[sym])
                                {
                                    byte needed = (byte)(extra_bits[sym]), bitrun;
                                    (extra) = 0;
                                    while (needed > 0)
                                    {
                                        if ((bits_left) <= (CompressionStream.BITBUF_WIDTH - 16))
                                        {
                                            //READ_BYTES
                                            {
                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b0 = qtm.InputBuffer[i_ptr++];

                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b1 = qtm.InputBuffer[i_ptr++];
                                                qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                            }
                                        }

                                        bitrun = (byte)((bits_left < needed) ? bits_left : needed);
                                        (extra) = (int)(((extra) << bitrun) | (qtm.PEEK_BITS_MSB(bitrun, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(bitrun, ref bit_buffer, ref bits_left);
                                        needed -= bitrun;
                                    }
                                }

                                match_offset = (uint)(position_base[sym] + extra + 1);
                                match_length = 3;
                                break;

                            // Selector 5 = fixed length match (4 bytes)
                            case 5:
                                //GET_SYMBOL(qtm.Model5, sym)
                                {
                                    range = (uint)((H - L) & 0xFFFF) + 1;
                                    symf = (ushort)(((((C - L + 1) * qtm.Model5.Syms[0].CumulativeFrequency) - 1) / range) & 0xFFFF);

                                    for (i = 1; i < qtm.Model5.Entries; i++)
                                    {
                                        if (qtm.Model5.Syms[i].CumulativeFrequency <= symf)
                                            break;
                                    }

                                    (sym) = qtm.Model5.Syms[i - 1].Sym;

                                    range = (uint)(H - L) + 1;
                                    symf = qtm.Model5.Syms[0].CumulativeFrequency;
                                    H = (ushort)(L + ((qtm.Model5.Syms[i - 1].CumulativeFrequency * range) / symf) - 1);
                                    L = (ushort)(L + ((qtm.Model5.Syms[i].CumulativeFrequency * range) / symf));

                                    do
                                    {
                                        qtm.Model5.Syms[--i].CumulativeFrequency += 8;
                                    } while (i > 0);

                                    if (qtm.Model5.Syms[0].CumulativeFrequency > 3800)
                                        UpdateModel(qtm.Model5);

                                    while (true)
                                    {
                                        if ((L & 0x8000) != (H & 0x8000))
                                        {
                                            if ((L & 0x4000) != 0 && (H & 0x4000) == 0)
                                            {
                                                // Underflow case
                                                C ^= 0x4000;
                                                L &= 0x3FFF;
                                                H |= 0x4000;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        L <<= 1;
                                        H = (ushort)((H << 1) | 1);

                                        //ENSURE_BITS(1)
                                        {
                                            while (bits_left < (1))
                                            {
                                                //READ_BYTES
                                                {
                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b0 = qtm.InputBuffer[i_ptr++];

                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b1 = qtm.InputBuffer[i_ptr++];
                                                    qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                                }
                                            }
                                        }

                                        C = (ushort)((C << 1) | (qtm.PEEK_BITS_MSB(1, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(1, ref bit_buffer, ref bits_left);
                                    }
                                }

                                //READ_MANY_BITS(extra, extra_bits[sym])
                                {
                                    byte needed = (byte)(extra_bits[sym]), bitrun;
                                    (extra) = 0;
                                    while (needed > 0)
                                    {
                                        if ((bits_left) <= (CompressionStream.BITBUF_WIDTH - 16))
                                        {
                                            //READ_BYTES
                                            {
                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b0 = qtm.InputBuffer[i_ptr++];

                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b1 = qtm.InputBuffer[i_ptr++];
                                                qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                            }
                                        }

                                        bitrun = (byte)((bits_left < needed) ? bits_left : needed);
                                        (extra) = (int)(((extra) << bitrun) | (qtm.PEEK_BITS_MSB(bitrun, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(bitrun, ref bit_buffer, ref bits_left);
                                        needed -= bitrun;
                                    }
                                }

                                match_offset = (uint)(position_base[sym] + extra + 1);
                                match_length = 4;
                                break;

                            // Selector 6 = variable length match
                            case 6:
                                //GET_SYMBOL(qtm.Model6Len, sym)
                                {
                                    range = (uint)((H - L) & 0xFFFF) + 1;
                                    symf = (ushort)(((((C - L + 1) * qtm.Model6Len.Syms[0].CumulativeFrequency) - 1) / range) & 0xFFFF);

                                    for (i = 1; i < qtm.Model6Len.Entries; i++)
                                    {
                                        if (qtm.Model6Len.Syms[i].CumulativeFrequency <= symf)
                                            break;
                                    }

                                    (sym) = qtm.Model6Len.Syms[i - 1].Sym;

                                    range = (uint)(H - L) + 1;
                                    symf = qtm.Model6Len.Syms[0].CumulativeFrequency;
                                    H = (ushort)(L + ((qtm.Model6Len.Syms[i - 1].CumulativeFrequency * range) / symf) - 1);
                                    L = (ushort)(L + ((qtm.Model6Len.Syms[i].CumulativeFrequency * range) / symf));

                                    do
                                    {
                                        qtm.Model6Len.Syms[--i].CumulativeFrequency += 8;
                                    } while (i > 0);

                                    if (qtm.Model6Len.Syms[0].CumulativeFrequency > 3800)
                                        UpdateModel(qtm.Model6Len);

                                    while (true)
                                    {
                                        if ((L & 0x8000) != (H & 0x8000))
                                        {
                                            if ((L & 0x4000) != 0 && (H & 0x4000) == 0)
                                            {
                                                // Underflow case
                                                C ^= 0x4000;
                                                L &= 0x3FFF;
                                                H |= 0x4000;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        L <<= 1;
                                        H = (ushort)((H << 1) | 1);

                                        //ENSURE_BITS(1)
                                        {
                                            while (bits_left < (1))
                                            {
                                                //READ_BYTES
                                                {
                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b0 = qtm.InputBuffer[i_ptr++];

                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b1 = qtm.InputBuffer[i_ptr++];
                                                    qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                                }
                                            }
                                        }

                                        C = (ushort)((C << 1) | (qtm.PEEK_BITS_MSB(1, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(1, ref bit_buffer, ref bits_left);
                                    }
                                }

                                //READ_MANY_BITS(extra, length_extra[sym])
                                {
                                    byte needed = (byte)(length_extra[sym]), bitrun;
                                    (extra) = 0;
                                    while (needed > 0)
                                    {
                                        if ((bits_left) <= (CompressionStream.BITBUF_WIDTH - 16))
                                        {
                                            //READ_BYTES
                                            {
                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b0 = qtm.InputBuffer[i_ptr++];

                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b1 = qtm.InputBuffer[i_ptr++];
                                                qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                            }
                                        }

                                        bitrun = (byte)((bits_left < needed) ? bits_left : needed);
                                        (extra) = (int)(((extra) << bitrun) | (qtm.PEEK_BITS_MSB(bitrun, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(bitrun, ref bit_buffer, ref bits_left);
                                        needed -= bitrun;
                                    }
                                }

                                match_length = length_base[sym] + extra + 5;

                                //GET_SYMBOL(qtm.Model6, sym)
                                {
                                    range = (uint)((H - L) & 0xFFFF) + 1;
                                    symf = (ushort)(((((C - L + 1) * qtm.Model6.Syms[0].CumulativeFrequency) - 1) / range) & 0xFFFF);

                                    for (i = 1; i < qtm.Model6.Entries; i++)
                                    {
                                        if (qtm.Model6.Syms[i].CumulativeFrequency <= symf)
                                            break;
                                    }

                                    (sym) = qtm.Model6.Syms[i - 1].Sym;

                                    range = (uint)(H - L) + 1;
                                    symf = qtm.Model6.Syms[0].CumulativeFrequency;
                                    H = (ushort)(L + ((qtm.Model6.Syms[i - 1].CumulativeFrequency * range) / symf) - 1);
                                    L = (ushort)(L + ((qtm.Model6.Syms[i].CumulativeFrequency * range) / symf));

                                    do
                                    {
                                        qtm.Model6.Syms[--i].CumulativeFrequency += 8;
                                    } while (i > 0);

                                    if (qtm.Model6.Syms[0].CumulativeFrequency > 3800)
                                        UpdateModel(qtm.Model6);

                                    while (true)
                                    {
                                        if ((L & 0x8000) != (H & 0x8000))
                                        {
                                            if ((L & 0x4000) != 0 && (H & 0x4000) == 0)
                                            {
                                                // Underflow case
                                                C ^= 0x4000;
                                                L &= 0x3FFF;
                                                H |= 0x4000;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        L <<= 1;
                                        H = (ushort)((H << 1) | 1);

                                        //ENSURE_BITS(1)
                                        {
                                            while (bits_left < (1))
                                            {
                                                //READ_BYTES
                                                {
                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b0 = qtm.InputBuffer[i_ptr++];

                                                    qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                    if (qtm.Error != Error.MSPACK_ERR_OK)
                                                        return qtm.Error;

                                                    byte b1 = qtm.InputBuffer[i_ptr++];
                                                    qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                                }
                                            }
                                        }

                                        C = (ushort)((C << 1) | (qtm.PEEK_BITS_MSB(1, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(1, ref bit_buffer, ref bits_left);
                                    }
                                }

                                //READ_MANY_BITS(extra, extra_bits[sym])
                                {
                                    byte needed = (byte)(extra_bits[sym]), bitrun;
                                    (extra) = 0;
                                    while (needed > 0)
                                    {
                                        if ((bits_left) <= (CompressionStream.BITBUF_WIDTH - 16))
                                        {
                                            //READ_BYTES
                                            {
                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b0 = qtm.InputBuffer[i_ptr++];

                                                qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                                if (qtm.Error != Error.MSPACK_ERR_OK)
                                                    return qtm.Error;

                                                byte b1 = qtm.InputBuffer[i_ptr++];
                                                qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                            }
                                        }

                                        bitrun = (byte)((bits_left < needed) ? bits_left : needed);
                                        (extra) = (int)(((extra) << bitrun) | (qtm.PEEK_BITS_MSB(bitrun, bit_buffer)));
                                        qtm.REMOVE_BITS_MSB(bitrun, ref bit_buffer, ref bits_left);
                                        needed -= bitrun;
                                    }
                                }

                                match_offset = (uint)(position_base[sym] + extra + 1);
                                break;

                            default:
                                // Should be impossible, model7 can only return 0-6
                                Console.WriteLine($"Got {selector} from selector");
                                return qtm.Error = Error.MSPACK_ERR_DECRUNCH;
                        }

                        rundest = (int)window_posn;
                        frame_todo -= (uint)match_length;

                        // Does match destination wrap the window? This situation is possible
                        // where the window size is less than the 32k frame size, but matches
                        // must not go beyond a frame boundary
                        if ((window_posn + match_length) > qtm.WindowSize)
                        {
                            // Copy first part of match, before window end
                            i = (int)(qtm.WindowSize - window_posn);
                            j = (int)(window_posn - match_offset);

                            while (i-- != 0)
                            {
                                window[rundest++] = window[j++ & (qtm.WindowSize - 1)];
                            }

                            // Flush currently stored data
                            i = (int)(qtm.WindowSize - qtm.OutputPointer);

                            // This should not happen, but if it does then this code
                            // can't handle the situation (can't flush up to the end of
                            // the window, but can't break out either because we haven't
                            // finished writing the match). Bail out in this case
                            if (i > out_bytes)
                            {
                                Console.WriteLine($"During window-wrap match; {i} bytes to flush but only need {out_bytes}");
                                return qtm.Error = Error.MSPACK_ERR_DECRUNCH;
                            }

                            if (qtm.System.Write(qtm.OutputFileHandle, window, qtm.OutputPointer, i) != i)
                                return qtm.Error = Error.MSPACK_ERR_WRITE;

                            out_bytes -= i;
                            qtm.OutputPointer = 0;
                            qtm.OutputEnd = 0;

                            // Copy second part of match, after window wrap
                            rundest = 0;
                            i = (int)(match_length - (qtm.WindowSize - window_posn));
                            while (i-- != 0)
                            {
                                window[rundest++] = window[j++ & (qtm.WindowSize - 1)];
                            }

                            window_posn = (uint)(window_posn + match_length - qtm.WindowSize);

                            break; // Because "window_posn < frame_end" has now failed
                        }
                        else
                        {
                            // Normal match - output won't wrap window or frame end
                            i = match_length;

                            // Does match _offset_ wrap the window?
                            if (match_offset > window_posn)
                            {
                                // j = length from match offset to end of window
                                j = (int)(match_offset - window_posn);
                                if (j > (int)qtm.WindowSize)
                                {
                                    Console.WriteLine("Match offset beyond window boundaries");
                                    return qtm.Error = Error.MSPACK_ERR_DECRUNCH;
                                }

                                runsrc = (int)(qtm.WindowSize - j);
                                if (j < i)
                                {
                                    // If match goes over the window edge, do two copy runs
                                    i -= j;
                                    while (j-- > 0)
                                    {
                                        window[rundest++] = window[runsrc++];
                                    }

                                    runsrc = 0;
                                }

                                while (i-- > 0)
                                {
                                    window[rundest++] = window[runsrc++];
                                }
                            }
                            else
                            {
                                runsrc = (int)(rundest - match_offset);
                                while (i-- > 0)
                                {
                                    window[rundest++] = window[runsrc++];
                                }
                            }

                            window_posn += (uint)match_length;
                        }
                    }
                }

                qtm.OutputEnd = (int)window_posn;

                // If we subtracted too much from frame_todo, it will
                // wrap around past zero and go above its max value
                if (frame_todo > QTM_FRAME_SIZE)
                {
                    Console.WriteLine("Overshot frame alignment");
                    return qtm.Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Another frame completed?
                if (frame_todo == 0)
                {
                    // Re-align input
                    if ((bits_left & 7) != 0)
                        qtm.REMOVE_BITS_MSB(bits_left & 7, ref bit_buffer, ref bits_left);

                    // Special Quantum hack -- cabd.c injects a trailer byte to allow the
                    // decompressor to realign itself. CAB Quantum blocks, unlike LZX
                    // blocks, can have anything from 0 to 4 trailing null bytes.
                    do
                    {
                        //READ_BITS(i, 8)
                        {
                            //ENSURE_BITS(8)
                            {
                                while (bits_left < (8))
                                {
                                    //READ_BYTES
                                    {
                                        qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                        if (qtm.Error != Error.MSPACK_ERR_OK)
                                            return qtm.Error;

                                        byte b0 = qtm.InputBuffer[i_ptr++];

                                        qtm.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                        if (qtm.Error != Error.MSPACK_ERR_OK)
                                            return qtm.Error;

                                        byte b1 = qtm.InputBuffer[i_ptr++];
                                        qtm.INJECT_BITS_MSB((b0 << 8) | b1, 16, ref bit_buffer, ref bits_left);
                                    }
                                }
                            }

                            (i) = (int)qtm.PEEK_BITS_MSB(8, bit_buffer);
                            qtm.REMOVE_BITS_MSB(8, ref bit_buffer, ref bits_left);
                        }
                    } while (i != 0xFF);

                    qtm.HeaderRead = 0;

                    frame_todo = QTM_FRAME_SIZE;
                }

                // Window wrap?
                if (window_posn == qtm.WindowSize)
                {
                    // Flush all currently stored data
                    i = (qtm.OutputEnd - qtm.OutputPointer);

                    // Break out if we have more than enough to finish this request
                    if (i >= out_bytes)
                        break;

                    if (qtm.System.Write(qtm.OutputFileHandle, window, qtm.OutputPointer, i) != i)
                        return qtm.Error = Error.MSPACK_ERR_WRITE;

                    out_bytes -= i;
                    qtm.OutputPointer = 0;
                    qtm.OutputEnd = 0;
                    window_posn = 0;
                }

            }

            if (out_bytes != 0)
            {
                i = (int)out_bytes;

                if (qtm.System.Write(qtm.OutputFileHandle, window, qtm.OutputPointer, i) != i)
                    return qtm.Error = Error.MSPACK_ERR_WRITE;

                qtm.OutputPointer += i;
            }

            // Store local state
            qtm.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);
            qtm.WindowPosition = window_posn;
            qtm.FrameTODO = frame_todo;
            qtm.High = H;
            qtm.Low = L;
            qtm.Current = C;

            return Error.MSPACK_ERR_OK;
        }

        private static void UpdateModel(QTMDModel model)
        {
            QTMDModelSym tmp;
            int i, j;

            if (--model.ShiftsLeft != 0)
            {
                for (i = model.Entries - 1; i >= 0; i--)
                {
                    // -1, not -2; the 0 entry saves this
                    model.Syms[i].CumulativeFrequency >>= 1;
                    if (model.Syms[i].CumulativeFrequency <= model.Syms[i + 1].CumulativeFrequency)
                        model.Syms[i].CumulativeFrequency = (ushort)(model.Syms[i + 1].CumulativeFrequency + 1);
                }
            }
            else
            {
                model.ShiftsLeft = 50;
                for (i = 0; i < model.Entries; i++)
                {
                    // No -1, want to include the 0 entry

                    // This converts CumFreqs into frequencies, then shifts right
                    model.Syms[i].CumulativeFrequency -= model.Syms[i + 1].CumulativeFrequency;
                    model.Syms[i].CumulativeFrequency++; // Avoid losing things entirely
                    model.Syms[i].CumulativeFrequency >>= 1;
                }

                // Now sort by frequencies, decreasing order -- this must be an
                // inplace selection sort, or a sort with the same (in)stability
                // characteristics
                for (i = 0; i < model.Entries - 1; i++)
                {
                    for (j = i + 1; j < model.Entries; j++)
                    {
                        if (model.Syms[i].CumulativeFrequency < model.Syms[j].CumulativeFrequency)
                        {
                            tmp = model.Syms[i];
                            model.Syms[i] = model.Syms[j];
                            model.Syms[j] = tmp;
                        }
                    }
                }

                // Then convert frequencies back to CumFreq
                for (i = model.Entries - 1; i >= 0; i--)
                {
                    model.Syms[i].CumulativeFrequency += model.Syms[i + 1].CumulativeFrequency;
                }
            }
        }

        private static void InitModel(QTMDModel model, QTMDModelSym[] syms, int start, int len)
        {
            model.ShiftsLeft = 4;
            model.Entries = len;
            model.Syms = syms;

            for (int i = 0; i <= len; i++)
            {
                // Actual symbol
                syms[i].Sym = (ushort)(start + i);

                // Current frequency of that symbol
                syms[i].CumulativeFrequency = (ushort)(len - i);
            }
        }
    }
}
