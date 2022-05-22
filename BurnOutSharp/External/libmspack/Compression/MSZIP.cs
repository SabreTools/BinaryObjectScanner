/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * The deflate method was created by Phil Katz. MSZIP is equivalent to the
 * deflate method.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;
using System.IO;
using static LibMSPackSharp.Compression.Constants;

namespace LibMSPackSharp.Compression
{
    public class MSZIP
    {
        #region MSZIP (deflate) compression / (inflate) decompression definitions

        public const int MSZIP_FRAME_SIZE = 32768; // Size of LZ history window
        public const int MSZIP_LITERAL_MAXSYMBOLS = 288; // literal/length huffman tree
        public const int MSZIP_LITERAL_TABLEBITS = 9;
        public const int MSZIP_DISTANCE_MAXSYMBOLS = 32; // Distance huffman tree
        public const int MSZIP_DISTANCE_TABLEBITS = 6;

        // If there are less direct lookup entries than symbols, the longer
        // code pointers will be <= maxsymbols. This must not happen, or we
        // will decode entries badly

        //public const int MSZIP_LITERAL_TABLESIZE = (MSZIP_LITERAL_MAXSYMBOLS * 4);
        public const int MSZIP_LITERAL_TABLESIZE = ((1 << MSZIP_LITERAL_TABLEBITS) + (MSZIP_LITERAL_MAXSYMBOLS * 2));

        //public const int MSZIP_DISTANCE_TABLESIZE = (MSZIP_DISTANCE_MAXSYMBOLS * 4);
        public const int MSZIP_DISTANCE_TABLESIZE = ((1 << MSZIP_DISTANCE_TABLEBITS) + (MSZIP_DISTANCE_MAXSYMBOLS * 2));

        /// <summary>
        /// Match lengths for literal codes 257.. 285
        /// </summary>
        private static readonly ushort[] lit_lengths = new ushort[29]
        {
          3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27,
          31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258
        };

        /// <summary>
        /// Match offsets for distance codes 0 .. 29
        /// </summary>
        private static readonly ushort[] dist_offsets = new ushort[30]
        {
          1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385,
          513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577
        };

        /// <summary>
        /// Extra bits required for literal codes 257.. 285
        /// </summary>
        private static readonly byte[] lit_extrabits = new byte[29]
        {
          0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2,
          2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
        };

        /// <summary>
        /// Extra bits required for distance codes 0 .. 29
        /// </summary>
        private static readonly byte[] dist_extrabits = new byte[30]
        {
          0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6,
          6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
        };

        /// <summary>
        /// The order of the bit length Huffman code lengths
        /// </summary>
        private static readonly byte[] bitlen_order = new byte[19]
        {
          16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
        };

        #endregion

        /// <summary>
        /// Allocates MS-ZIP decompression stream for decoding the given stream.
        /// 
        /// - uses system.alloc() to allocate memory
        /// 
        /// - returns null if not enough memory
        /// 
        /// - input_buffer_size is how many bytes to use as an input bitstream buffer
        /// 
        /// - if RepairMode is non-zero, errors in decompression will be skipped
        ///   and 'holes' left will be filled with zero bytes. This allows at least
        ///   a partial recovery of erroneous data.
        /// </summary>
        public static MSZIPDStream Init(SystemImpl system, FileStream input, FileStream output, int input_buffer_size, bool repair_mode)
        {
            if (system == null)
                return null;

            // Round up input buffer size to multiple of two
            input_buffer_size = (input_buffer_size + 1) & -2;
            if (input_buffer_size < 2)
                return null;

            // Allocate decompression state
            return new MSZIPDStream
            {
                // Allocate input buffer
                InputBuffer = new byte[input_buffer_size],

                // Initialise decompression state
                Sys = system,
                Input = input,
                Output = output,
                InputBufferSize = (uint)input_buffer_size,
                InputEnd = 0,
                Error = Error.MSPACK_ERR_OK,
                RepairMode = repair_mode,
                FlushWindow = FlushWindow,

                InputPointer = 0,
                InputLength = 0,
                OutputPointer = 0,
                OutputLength = 0,
                BitBuffer = 0,
                BitsLeft = 0,
            };
        }

        /// <summary>
        /// Decompresses, or decompresses more of, an MS-ZIP stream.
        ///
        /// - out_bytes of data will be decompressed and the function will return
        ///   with an MSPACK_ERR_OK return code.
        ///
        /// - decompressing will stop as soon as out_bytes is reached. if the true
        ///   amount of bytes decoded spills over that amount, they will be kept for
        ///   a later invocation of mszipd_decompress().
        ///
        /// - the output bytes will be passed to the system.write() function given in
        ///   mszipd_init(), using the output file handle given in mszipd_init(). More
        ///   than one call may be made to system.write()
        ///
        /// - MS-ZIP will read input bytes as necessary using the system.read()
        ///   function given in mszipd_init(), using the input file handle given in
        ///   mszipd_init(). This will continue until system.read() returns 0 bytes,
        ///   or an error.
        /// </summary>
        public static Error Decompress(object o, long out_bytes)
        {
            MSZIPDStream zip = o as MSZIPDStream;
            if (zip == null)
                return Error.MSPACK_ERR_ARGS;

            // For the bit buffer
            uint bit_buffer;
            int bits_left;
            int i_ptr, i_end;

            int i, state;
            Error error;

            // Easy answers
            if (zip == null || (out_bytes < 0))
                return Error.MSPACK_ERR_ARGS;
            if (zip.Error != Error.MSPACK_ERR_OK)
                return zip.Error;

            // Flush out any stored-up bytes before we begin
            i = zip.OutputLength - zip.OutputPointer;
            if (i > out_bytes)
                i = (int)out_bytes;

            if (i != 0)
            {
                if (zip.Sys.Write(zip.Output, zip.Window, zip.OutputPointer, i) != i)
                    return zip.Error = Error.MSPACK_ERR_WRITE;

                zip.OutputPointer += i;
                out_bytes -= i;
            }

            if (out_bytes == 0)
                return Error.MSPACK_ERR_OK;

            while (out_bytes > 0)
            {
                // Unpack another block

                //RESTORE_BITS
                {
                    i_ptr = zip.InputPointer;
                    i_end = zip.InputLength;
                    bit_buffer = zip.BitBuffer;
                    bits_left = zip.BitsLeft;
                }

                // Skip to next read 'CK' header
                i = bits_left & 7;

                // Align to bytestream

                //REMOVE_BITS(i)
                {
                    bit_buffer >>= (i);
                    bits_left -= (i);
                }

                state = 0;
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
                                    //READ_IF_NEEDED
                                    {
                                        if (i_ptr >= i_end)
                                        {
                                            if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                return zip.Error;

                                            i_ptr = zip.InputPointer;
                                            i_end = zip.InputLength;
                                        }
                                    }

                                    //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                    {
                                        bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                        bits_left += (8);
                                    }
                                }
                            }
                        }

                        (i) = (int)(bit_buffer & ((1 << (8)) - 1)); //PEEK_BITS(8)

                        //REMOVE_BITS(8)
                        {
                            bit_buffer >>= (8);
                            bits_left -= (8);
                        }
                    }

                    if (i == 'C')
                        state = 1;
                    else if ((state == 1) && (i == 'K'))
                        state = 2;
                    else
                        state = 0;
                } while (state != 2);

                // Inflate a block, repair and realign if necessary
                zip.WindowPosition = 0;
                zip.BytesOutput = 0;

                //STORE_BITS
                {
                    zip.InputPointer = i_ptr;
                    zip.InputLength = i_end;
                    zip.BitBuffer = bit_buffer;
                    zip.BitsLeft = bits_left;
                }

                if ((error = Inflate(zip)) != Error.MSPACK_ERR_OK)
                {
                    Console.WriteLine($"Inflate error {error}");
                    if (zip.RepairMode)
                    {
                        // Recover partially-inflated buffers
                        if (zip.BytesOutput == 0 && zip.WindowPosition > 0)
                            zip.FlushWindow(zip, zip.WindowPosition);

                        zip.Sys.Message(null, $"MSZIP error, {MSZIP_FRAME_SIZE - zip.BytesOutput} bytes of data lost.");
                        for (i = zip.BytesOutput; i < MSZIP_FRAME_SIZE; i++)
                        {
                            zip.Window[i] = 0x00;
                        }

                        zip.BytesOutput = MSZIP_FRAME_SIZE;
                    }
                    else
                    {
                        return zip.Error = (error > 0) ? error : Error.MSPACK_ERR_DECRUNCH;
                    }
                }

                zip.OutputPointer = 0;
                zip.OutputLength = zip.BytesOutput;

                // Write a frame
                i = (out_bytes < zip.BytesOutput) ? (int)out_bytes : zip.BytesOutput;
                if (zip.Sys.Write(zip.Output, zip.Window, zip.OutputPointer, i) != i)
                    return zip.Error = Error.MSPACK_ERR_WRITE;

                // mspack errors (i.e. read errors) are fatal and can't be recovered
                if ((error > 0) && zip.RepairMode)
                    return error;

                zip.OutputPointer += i;
                out_bytes -= i;
            }

            if (out_bytes != 0)
            {
                Console.WriteLine("bytes left to output");
                return zip.Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Decompresses an entire MS-ZIP stream in a KWAJ file. Acts very much
        /// like mszipd_decompress(), but doesn't take an out_bytes parameter
        /// </summary>
        public static Error DecompressKWAJ(MSZIPDStream zip)
        {
            // For the bit buffer
            uint bit_buffer;
            int bits_left;
            int i_ptr, i_end;

            int i, block_len;
            Error error;

            // Unpack blocks until block_len == 0
            for (; ; )
            {
                //RESTORE_BITS
                {
                    i_ptr = zip.InputPointer;
                    i_end = zip.InputLength;
                    bit_buffer = zip.BitBuffer;
                    bits_left = zip.BitsLeft;
                }

                // Align to bytestream, read block_len
                i = bits_left & 7;

                //REMOVE_BITS(i)
                {
                    bit_buffer >>= (i);
                    bits_left -= (i);
                }

                //READ_BITS(block_len, 8)
                {
                    //ENSURE_BITS(8)
                    {
                        while (bits_left < (8))
                        {
                            //READ_BYTES
                            {
                                //READ_IF_NEEDED
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                            return zip.Error;

                                        i_ptr = zip.InputPointer;
                                        i_end = zip.InputLength;
                                    }
                                }

                                //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                {
                                    bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                    bits_left += (8);
                                }
                            }
                        }
                    }

                    (block_len) = (int)(bit_buffer & ((1 << (8)) - 1)); //PEEK_BITS(8)

                    //REMOVE_BITS(8)
                    {
                        bit_buffer >>= (8);
                        bits_left -= (8);
                    }
                }

                //READ_BITS(i, 8)
                {
                    //ENSURE_BITS(8)
                    {
                        while (bits_left < (8))
                        {
                            //READ_BYTES
                            {
                                //READ_IF_NEEDED
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                            return zip.Error;

                                        i_ptr = zip.InputPointer;
                                        i_end = zip.InputLength;
                                    }
                                }

                                //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                {
                                    bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                    bits_left += (8);
                                }
                            }
                        }
                    }

                    (i) = (int)(bit_buffer & ((1 << (8)) - 1)); //PEEK_BITS(8)

                    //REMOVE_BITS(8)
                    {
                        bit_buffer >>= (8);
                        bits_left -= (8);
                    }
                }
                
                block_len |= i << 8;

                if (block_len == 0)
                    break;

                // Read "CK" header

                //READ_BITS(i, 8)
                {
                    //ENSURE_BITS(8)
                    {
                        while (bits_left < (8))
                        {
                            //READ_BYTES
                            {
                                //READ_IF_NEEDED
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                            return zip.Error;

                                        i_ptr = zip.InputPointer;
                                        i_end = zip.InputLength;
                                    }
                                }

                                //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                {
                                    bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                    bits_left += (8);
                                }
                            }
                        }
                    }

                    (i) = (int)(bit_buffer & ((1 << (8)) - 1)); //PEEK_BITS(8)

                    //REMOVE_BITS(8)
                    {
                        bit_buffer >>= (8);
                        bits_left -= (8);
                    }
                }

                if (i != 'C')
                    return Error.MSPACK_ERR_DATAFORMAT;

                //READ_BITS(i, 8)
                {
                    //ENSURE_BITS(8)
                    {
                        while (bits_left < (8))
                        {
                            //READ_BYTES
                            {
                                //READ_IF_NEEDED
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                            return zip.Error;

                                        i_ptr = zip.InputPointer;
                                        i_end = zip.InputLength;
                                    }
                                }

                                //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                {
                                    bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                    bits_left += (8);
                                }
                            }
                        }
                    }

                    (i) = (int)(bit_buffer & ((1 << (8)) - 1)); //PEEK_BITS(8)

                    //REMOVE_BITS(nbits)
                    {
                        bit_buffer >>= (8);
                        bits_left -= (8);
                    }
                }

                if (i != 'K')
                    return Error.MSPACK_ERR_DATAFORMAT;

                // Inflate block
                zip.WindowPosition = 0;
                zip.BytesOutput = 0;

                //STORE_BITS
                {
                    zip.InputPointer = i_ptr;
                    zip.InputLength = i_end;
                    zip.BitBuffer = bit_buffer;
                    zip.BitsLeft = bits_left;
                }

                if ((error = Inflate(zip)) != Error.MSPACK_ERR_OK)
                {
                    Console.WriteLine($"Inflate error {error}");
                    return zip.Error = (error > 0) ? error : Error.MSPACK_ERR_DECRUNCH;
                }

                // Write inflated block
                try { zip.Sys.Write(zip.Output, zip.Window, 0, zip.BytesOutput); }
                catch { return zip.Error = Error.MSPACK_ERR_WRITE; }
            }

            return Error.MSPACK_ERR_OK;
        }

        private static Error ReadLens(MSZIPDStream zip)
        {
            // For the bit buffer and huffman decoding
            uint bit_buffer;
            int bits_left;
            int i_ptr, i_end;

            // bitlen Huffman codes -- immediate lookup, 7 bit max code length
            ushort[] bl_table = new ushort[(1 << 7)];
            byte[] bl_len = new byte[19];

            byte[] lens = new byte[MSZIP_LITERAL_MAXSYMBOLS + MSZIP_DISTANCE_MAXSYMBOLS];
            uint lit_codes, dist_codes, code, last_code = 0, bitlen_codes, i, run;

            //RESTORE_BITS
            {
                i_ptr = zip.InputPointer;
                i_end = zip.InputLength;
                bit_buffer = zip.BitBuffer;
                bits_left = zip.BitsLeft;
            }

            // Read the number of codes

            //READ_BITS(lit_codes, 5)
            {
                //ENSURE_BITS(5)
                {
                    while (bits_left < (5))
                    {
                        //READ_BYTES
                        {
                            //READ_IF_NEEDED
                            {
                                if (i_ptr >= i_end)
                                {
                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                        return zip.Error;

                                    i_ptr = zip.InputPointer;
                                    i_end = zip.InputLength;
                                }
                            }

                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                            {
                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                bits_left += (8);
                            }
                        }
                    }
                }

                (lit_codes) = (bit_buffer & ((1 << (5)) - 1)); //PEEK_BITS(5)

                //REMOVE_BITS(5)
                {
                    bit_buffer >>= (5);
                    bits_left -= (5);
                }
            }

            lit_codes += 257;

            //READ_BITS_T(dist_codes, 5)
            {
                //ENSURE_BITS(5)
                {
                    while (bits_left < (5))
                    {
                        //READ_BYTES
                        {
                            //READ_IF_NEEDED
                            {
                                if (i_ptr >= i_end)
                                {
                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                        return zip.Error;

                                    i_ptr = zip.InputPointer;
                                    i_end = zip.InputLength;
                                }
                            }

                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                            {
                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                bits_left += (8);
                            }
                        }
                    }
                }

                (dist_codes) = (bit_buffer & lsb_bit_mask[(5)]); //PEEK_BITS_T(5)

                //REMOVE_BITS(5)
                {
                    bit_buffer >>= (5);
                    bits_left -= (5);
                }
            }

            dist_codes += 1;

            //READ_BITS(bitlen_codes, 4)
            {
                //ENSURE_BITS(4)
                {
                    while (bits_left < (4))
                    {
                        //READ_BYTES
                        {
                            //READ_IF_NEEDED
                            {
                                if (i_ptr >= i_end)
                                {
                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                        return zip.Error;

                                    i_ptr = zip.InputPointer;
                                    i_end = zip.InputLength;
                                }
                            }

                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                            {
                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                bits_left += (8);
                            }
                        }
                    }
                }

                (bitlen_codes) = (bit_buffer & ((1 << (4)) - 1)); //PEEK_BITS(4)

                //REMOVE_BITS(nbits)
                {
                    bit_buffer >>= (4);
                    bits_left -= (4);
                }
            }

            bitlen_codes += 4;
            if (lit_codes > MSZIP_LITERAL_MAXSYMBOLS)
                return Error.INF_ERR_SYMLENS;
            if (dist_codes > MSZIP_DISTANCE_MAXSYMBOLS)
                return Error.INF_ERR_SYMLENS;

            // Read in the bit lengths in their unusual order
            for (i = 0; i < bitlen_codes; i++)
            {
                //READ_BITS(bl_len[bitlen_order[i]], 3)
                {
                    //ENSURE_BITS(3)
                    {
                        while (bits_left < (3))
                        {
                            //READ_BYTES
                            {
                                //READ_IF_NEEDED
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                            return zip.Error;

                                        i_ptr = zip.InputPointer;
                                        i_end = zip.InputLength;
                                    }
                                }

                                //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                {
                                    bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                    bits_left += (8);
                                }
                            }
                        }
                    }

                    (bl_len[bitlen_order[i]]) = (byte)(bit_buffer & ((1 << (3)) - 1)); //PEEK_BITS(3)

                    //REMOVE_BITS(3)
                    {
                        bit_buffer >>= (3);
                        bits_left -= (3);
                    }
                }
            }

            while (i < 19) bl_len[bitlen_order[i++]] = 0;

            // Create decoding table with an immediate lookup
            if (CompressionStream.MakeDecodeTable(19, 7, bl_len, bl_table, msb: false))
                return Error.INF_ERR_BITLENTBL;

            // Read literal / distance code lengths */
            for (i = 0; i < (lit_codes + dist_codes); i++)
            {
                // Single-level huffman lookup

                //ENSURE_BITS(7)
                {
                    while (bits_left < (7))
                    {
                        //READ_BYTES
                        {
                            //READ_IF_NEEDED
                            {
                                if (i_ptr >= i_end)
                                {
                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                        return zip.Error;

                                    i_ptr = zip.InputPointer;
                                    i_end = zip.InputLength;
                                }
                            }

                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                            {
                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                bits_left += (8);
                            }
                        }
                    }
                }

                code = bl_table[(bit_buffer & ((1 << (7)) - 1))]; //PEEK_BITS(7)

                //REMOVE_BITS(bl_len[code])
                {
                    bit_buffer >>= (bl_len[code]);
                    bits_left -= (bl_len[code]);
                }

                if (code < 16)
                {
                    lens[i] = (byte)(last_code = code);
                }
                else
                {
                    switch (code)
                    {
                        case 16:
                            //READ_BITS(run, 2)
                            {
                                //ENSURE_BITS(2)
                                {
                                    while (bits_left < (2))
                                    {
                                        //READ_BYTES
                                        {
                                            //READ_IF_NEEDED
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                        return zip.Error;

                                                    i_ptr = zip.InputPointer;
                                                    i_end = zip.InputLength;
                                                }
                                            }

                                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (run) = (bit_buffer & ((1 << (2)) - 1)); //PEEK_BITS(2)

                                //REMOVE_BITS(2)
                                {
                                    bit_buffer >>= (2);
                                    bits_left -= (2);
                                }
                            }

                            run += 3;
                            code = last_code;
                            break;

                        case 17:
                            //READ_BITS(run, 3)
                            {
                                //ENSURE_BITS(3)
                                {
                                    while (bits_left < (3))
                                    {
                                        //READ_BYTES
                                        {
                                            //READ_IF_NEEDED
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                        return zip.Error;

                                                    i_ptr = zip.InputPointer;
                                                    i_end = zip.InputLength;
                                                }
                                            }

                                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (run) = (bit_buffer & ((1 << (3)) - 1)); //PEEK_BITS(3)

                                //REMOVE_BITS(3)
                                {
                                    bit_buffer >>= (3);
                                    bits_left -= (3);
                                }
                            }

                            run += 3;
                            code = 0;
                            break;

                        case 18:
                            //READ_BITS(run, 7)
                            {
                                //ENSURE_BITS(7)
                                {
                                    while (bits_left < (7))
                                    {
                                        //READ_BYTES
                                        {
                                            //READ_IF_NEEDED
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                        return zip.Error;

                                                    i_ptr = zip.InputPointer;
                                                    i_end = zip.InputLength;
                                                }
                                            }

                                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (run) = (bit_buffer & ((1 << (7)) - 1)); //PEEK_BITS(7)

                                //REMOVE_BITS(7)
                                {
                                    bit_buffer >>= (7);
                                    bits_left -= (7);
                                }
                            }

                            run += 11;
                            code = 0;
                            break;

                        default:
                            Console.WriteLine($"Bad code!: {code}");
                            return Error.INF_ERR_BADBITLEN;
                    }

                    if ((i + run) > (lit_codes + dist_codes))
                        return Error.INF_ERR_BITOVERRUN;

                    while (run-- != 0)
                    {
                        lens[i++] = (byte)code;
                    }

                    i--;
                }
            }

            // Copy LITERAL code lengths and clear any remaining
            i = lit_codes;
            Array.Copy(lens, 0, zip.LITERAL_len, 0, i);
            while (i < MSZIP_LITERAL_MAXSYMBOLS)
            {
                zip.LITERAL_len[i++] = 0;
            }

            i = dist_codes;
            Array.Copy(lens, lit_codes, zip.DISTANCE_len, 0, i);
            while (i < MSZIP_DISTANCE_MAXSYMBOLS)
            {
                zip.DISTANCE_len[i++] = 0;
            }

            //STORE_BITS
            {
                zip.InputPointer = i_ptr;
                zip.InputLength = i_end;
                zip.BitBuffer = bit_buffer;
                zip.BitsLeft = bits_left;
            }

            return 0;
        }

        /// <summary>
        /// A clean implementation of RFC 1951 / inflate
        /// </summary>
        private static Error Inflate(MSZIPDStream zip)
        {
            uint last_block, block_type, distance, length, this_run, i;
            Error err;

            // For the bit buffer and huffman decoding
            uint bit_buffer;
            int bits_left;
            ushort sym;
            int i_ptr, i_end;

            //RESTORE_BITS
            {
                i_ptr = zip.InputPointer;
                i_end = zip.InputLength;
                bit_buffer = zip.BitBuffer;
                bits_left = zip.BitsLeft;
            }

            do
            {
                // Read in last block bit

                //READ_BITS(last_block, 1)
                {
                    //ENSURE_BITS(1)
                    {
                        while (bits_left < (1))
                        {
                            //READ_BYTES
                            {
                                //READ_IF_NEEDED
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                            return zip.Error;

                                        i_ptr = zip.InputPointer;
                                        i_end = zip.InputLength;
                                    }
                                }

                                //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                {
                                    bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                    bits_left += (8);
                                }
                            }
                        }
                    }

                    (last_block) = (bit_buffer & ((1 << (1)) - 1)); //PEEK_BITS(1)

                    //REMOVE_BITS(1)
                    {
                        bit_buffer >>= (1);
                        bits_left -= (1);
                    }
                }

                // Read in block type

                //READ_BITS(block_type, 2)
                {
                    //ENSURE_BITS(2)
                    {
                        while (bits_left < (2))
                        {
                            //READ_BYTES
                            {
                                //READ_IF_NEEDED
                                {
                                    if (i_ptr >= i_end)
                                    {
                                        if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                            return zip.Error;

                                        i_ptr = zip.InputPointer;
                                        i_end = zip.InputLength;
                                    }
                                }

                                //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                {
                                    bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                    bits_left += (8);
                                }
                            }
                        }
                    }

                    (block_type) = (bit_buffer & ((1 << (2)) - 1)); //PEEK_BITS(2)

                    //REMOVE_BITS(2)
                    {
                        bit_buffer >>= (2);
                        bits_left -= (2);
                    }
                }

                if (block_type == 0)
                {
                    // Uncompressed block
                    byte[] lens_buf = new byte[4];

                    // Go to byte boundary
                    i = (uint)(bits_left & 7);

                    //REMOVE_BITS(i)
                    {
                        bit_buffer >>= (int)(i);
                        bits_left -= (int)(i);
                    }

                    // Read 4 bytes of data, emptying the bit-buffer if necessary
                    for (i = 0; (bits_left >= 8); i++)
                    {
                        if (i == 4)
                            return Error.INF_ERR_BITBUF;

                        lens_buf[i] = (byte)(bit_buffer & ((1 << (8)) - 1)); //PEEK_BITS(8)

                        //REMOVE_BITS(8)
                        {
                            bit_buffer >>= (8);
                            bits_left -= (8);
                        }
                    }

                    if (bits_left != 0)
                        return Error.INF_ERR_BITBUF;

                    while (i < 4)
                    {
                        //READ_IF_NEEDED
                        {
                            if (i_ptr >= i_end)
                            {
                                if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                    return zip.Error;

                                i_ptr = zip.InputPointer;
                                i_end = zip.InputLength;
                            }
                        }

                        lens_buf[i++] = zip.InputBuffer[i_ptr++];
                    }

                    // Get the length and its complement
                    length = (uint)(lens_buf[0] | (lens_buf[1] << 8));
                    i = (uint)(lens_buf[2] | (lens_buf[3] << 8));
                    if (length != (~i & 0xFFFF))
                        return Error.INF_ERR_COMPLEMENT;

                    // Read and copy the uncompressed data into the window
                    while (length > 0)
                    {
                        //READ_IF_NEEDED
                        {
                            if (i_ptr >= i_end)
                            {
                                if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                    return zip.Error;

                                i_ptr = zip.InputPointer;
                                i_end = zip.InputLength;
                            }
                        }

                        this_run = length;
                        if (this_run > (uint)(i_end - i_ptr))
                            this_run = (uint)(i_end - i_ptr);

                        if (this_run > (MSZIP_FRAME_SIZE - zip.WindowPosition))
                            this_run = MSZIP_FRAME_SIZE - zip.WindowPosition;

                        Array.Copy(zip.InputBuffer, i_ptr, zip.Window, zip.WindowPosition, this_run);

                        zip.WindowPosition += this_run;
                        i_ptr += (int)this_run;
                        length -= this_run;

                        //FLUSH_IF_NEEDED
                        {
                            if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                            {
                                if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                    return Error.INF_ERR_FLUSH;

                                zip.WindowPosition = 0;
                            }
                        }
                    }
                }
                else if ((block_type == 1) || (block_type == 2))
                {
                    // Huffman-compressed LZ77 block
                    uint match_posn, code;

                    if (block_type == 1)
                    {
                        // Block with fixed Huffman codes
                        i = 0;
                        while (i < 144)
                        {
                            zip.LITERAL_len[i++] = 8;
                        }

                        while (i < 256)
                        {
                            zip.LITERAL_len[i++] = 9;
                        }

                        while (i < 280)
                        {
                            zip.LITERAL_len[i++] = 7;
                        }

                        while (i < 288)
                        {
                            zip.LITERAL_len[i++] = 8;
                        }

                        for (i = 0; i < 32; i++)
                        {
                            zip.DISTANCE_len[i] = 5;
                        }
                    }
                    else
                    {
                        // Block with dynamic Huffman codes

                        //STORE_BITS
                        {
                            zip.InputPointer = i_ptr;
                            zip.InputLength = i_end;
                            zip.BitBuffer = bit_buffer;
                            zip.BitsLeft = bits_left;
                        }

                        if ((err = ReadLens(zip)) != Error.MSPACK_ERR_OK)
                            return err;

                        //RESTORE_BITS
                        {
                            i_ptr = zip.InputPointer;
                            i_end = zip.InputLength;
                            bit_buffer = zip.BitBuffer;
                            bits_left = zip.BitsLeft;
                        }
                    }

                    // Now huffman lengths are read for either kind of block, 
                    // create huffman decoding tables
                    if (CompressionStream.MakeDecodeTable(MSZIP_LITERAL_MAXSYMBOLS, MSZIP_LITERAL_TABLEBITS, zip.LITERAL_len, zip.LITERAL_table, msb: false))
                        return Error.INF_ERR_LITERALTBL;

                    if (CompressionStream.MakeDecodeTable(MSZIP_DISTANCE_MAXSYMBOLS, MSZIP_DISTANCE_TABLEBITS, zip.DISTANCE_len, zip.DISTANCE_table, msb: false))
                        return Error.INF_ERR_DISTANCETBL;

                    // Decode forever until end of block code
                    for (; ; )
                    {
                        //READ_HUFFSYM(LITERAL, code)
                        {
                            //ENSURE_BITS(CompressionStream.HUFF_MAXBITS)
                            {
                                while (bits_left < (CompressionStream.HUFF_MAXBITS))
                                {
                                    //READ_BYTES
                                    {
                                        //READ_IF_NEEDED
                                        {
                                            if (i_ptr >= i_end)
                                            {
                                                if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                    return zip.Error;

                                                i_ptr = zip.InputPointer;
                                                i_end = zip.InputLength;
                                            }
                                        }

                                        //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                        {
                                            bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                            bits_left += (8);
                                        }
                                    }
                                }
                            }

                            sym = zip.LITERAL_table[(bit_buffer & ((1 << (MSZIP_LITERAL_TABLEBITS)) - 1))]; //PEEK_BITS(TABLEBITS(LITERAL))
                            if (sym >= MSZIP_LITERAL_MAXSYMBOLS)
                            {
                                //HUFF_TRAVERSE(LITERAL)
                                {
                                    i = MSZIP_LITERAL_TABLEBITS - 1;
                                    do
                                    {
                                        if (i++ > CompressionStream.HUFF_MAXBITS)
                                            return Error.INF_ERR_HUFFSYM;

                                        sym = zip.LITERAL_table[(sym << 1) | ((bit_buffer >> (int)i) & 1)];
                                    } while (sym >= MSZIP_LITERAL_MAXSYMBOLS);
                                }
                            }

                            (code) = sym;
                            i = zip.LITERAL_len[sym];

                            //REMOVE_BITS(i)
                            {
                                bit_buffer >>= (int)(i);
                                bits_left -= (int)(i);
                            }
                        }

                        if (code < 256)
                        {
                            zip.Window[zip.WindowPosition++] = (byte)code;

                            //FLUSH_IF_NEEDED
                            {
                                if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                                {
                                    if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                        return Error.INF_ERR_FLUSH;

                                    zip.WindowPosition = 0;
                                }
                            }
                        }
                        else if (code == 256)
                        {
                            // END OF BLOCK CODE: loop break point
                            break;
                        }
                        else
                        {
                            code -= 257; // Codes 257-285 are matches
                            if (code >= 29)
                                return Error.INF_ERR_LITCODE; // Codes 286-287 are illegal

                            //READ_BITS_T(length, lit_extrabits[code])
                            {
                                //ENSURE_BITS(lit_extrabits[code])
                                {
                                    while (bits_left < (lit_extrabits[code]))
                                    {
                                        //READ_BYTES
                                        {
                                            //READ_IF_NEEDED
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                        return zip.Error;

                                                    i_ptr = zip.InputPointer;
                                                    i_end = zip.InputLength;
                                                }
                                            }

                                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (length) = (bit_buffer & lsb_bit_mask[(lit_extrabits[code])]); //PEEK_BITS_T(lit_extrabits[code])

                                //REMOVE_BITS(lit_extrabits[code])
                                {
                                    bit_buffer >>= (lit_extrabits[code]);
                                    bits_left -= (lit_extrabits[code]);
                                }
                            }

                            length += lit_lengths[code];

                            //READ_HUFFSYM(DISTANCE, var)
                            {
                                //ENSURE_BITS(CompressionStream.HUFF_MAXBITS)
                                {
                                    while (bits_left < (CompressionStream.HUFF_MAXBITS))
                                    {
                                        //READ_BYTES
                                        {
                                            //READ_IF_NEEDED
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                        return zip.Error;

                                                    i_ptr = zip.InputPointer;
                                                    i_end = zip.InputLength;
                                                }
                                            }

                                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                sym = zip.DISTANCE_table[(bit_buffer & ((1 << (MSZIP_DISTANCE_TABLEBITS)) - 1))]; //PEEK_BITS(TABLEBITS(DISTANCE))
                                if (sym >= MSZIP_DISTANCE_MAXSYMBOLS)
                                {
                                    //HUFF_TRAVERSE(DISTANCE)
                                    {
                                        i = MSZIP_DISTANCE_TABLEBITS - 1;
                                        do
                                        {
                                            if (i++ > CompressionStream.HUFF_MAXBITS)
                                                return Error.INF_ERR_HUFFSYM;

                                            sym = zip.DISTANCE_table[(sym << 1) | ((bit_buffer >> (int)i) & 1)];
                                        } while (sym >= MSZIP_DISTANCE_MAXSYMBOLS);
                                    }
                                }

                                (code) = sym;
                                i = zip.DISTANCE_len[sym];

                                //REMOVE_BITS(i)
                                {
                                    bit_buffer >>= (int)(i);
                                    bits_left -= (int)(i);
                                }
                            }

                            if (code >= 30)
                                return Error.INF_ERR_DISTCODE;

                            //READ_BITS_T(distance, dist_extrabits[code])
                            {
                                //ENSURE_BITS(dist_extrabits[code])
                                {
                                    while (bits_left < (dist_extrabits[code]))
                                    {
                                        //READ_BYTES
                                        {
                                            //READ_IF_NEEDED
                                            {
                                                if (i_ptr >= i_end)
                                                {
                                                    if (zip.ReadInput() != Error.MSPACK_ERR_OK)
                                                        return zip.Error;

                                                    i_ptr = zip.InputPointer;
                                                    i_end = zip.InputLength;
                                                }
                                            }

                                            //INJECT_BITS(zip.InputBuffer[i_ptr++], 8)
                                            {
                                                bit_buffer |= (uint)((zip.InputBuffer[i_ptr++]) << bits_left);
                                                bits_left += (8);
                                            }
                                        }
                                    }
                                }

                                (distance) = (bit_buffer & lsb_bit_mask[(dist_extrabits[code])]); //PEEK_BITS_T(dist_extrabits[code])

                                //REMOVE_BITS(dist_extrabits[code])
                                {
                                    bit_buffer >>= (dist_extrabits[code]);
                                    bits_left -= (dist_extrabits[code]);
                                }
                            }

                            distance += dist_offsets[code];

                            // Match position is window position minus distance. If distance
                            // is more than window position numerically, it must 'wrap
                            // around' the frame size.
                            match_posn = (uint)((distance > zip.WindowPosition) ? MSZIP_FRAME_SIZE : 0) + zip.WindowPosition - distance;

                            // Copy match
                            if (length < 12)
                            {
                                // Short match, use slower loop but no loop setup code
                                while (length-- != 0)
                                {
                                    zip.Window[zip.WindowPosition++] = zip.Window[match_posn++];
                                    match_posn &= MSZIP_FRAME_SIZE - 1;

                                    //FLUSH_IF_NEEDED
                                    {
                                        if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                                        {
                                            if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                                return Error.INF_ERR_FLUSH;

                                            zip.WindowPosition = 0;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Longer match, use faster loop but with setup expense */
                                int runsrc, rundest;
                                do
                                {
                                    this_run = length;
                                    if ((match_posn + this_run) > MSZIP_FRAME_SIZE)
                                        this_run = MSZIP_FRAME_SIZE - match_posn;
                                    if ((zip.WindowPosition + this_run) > MSZIP_FRAME_SIZE)
                                        this_run = MSZIP_FRAME_SIZE - zip.WindowPosition;

                                    rundest = (int)zip.WindowPosition;
                                    zip.WindowPosition += this_run;
                                    runsrc = (int)match_posn;
                                    match_posn += this_run;
                                    length -= this_run;
                                    while (this_run-- != 0)
                                    {
                                        zip.Window[rundest++] = zip.Window[runsrc++];
                                    }

                                    if (match_posn == MSZIP_FRAME_SIZE)
                                        match_posn = 0;

                                    //FLUSH_IF_NEEDED
                                    {
                                        if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                                        {
                                            if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                                return Error.INF_ERR_FLUSH;

                                            zip.WindowPosition = 0;
                                        }
                                    }
                                } while (length > 0);
                            }

                        }

                    }
                }
                else
                {
                    // block_type == 3 -- bad block type
                    return Error.INF_ERR_BLOCKTYPE;
                }
            } while (last_block == 0);

            // Flush the remaining data
            if (zip.WindowPosition != 0)
            {
                if (zip.FlushWindow(zip, zip.WindowPosition) != Error.MSPACK_ERR_OK)
                    return Error.INF_ERR_FLUSH;
            }

            //STORE_BITS
            {
                zip.InputPointer = i_ptr;
                zip.InputLength = i_end;
                zip.BitBuffer = bit_buffer;
                zip.BitsLeft = bits_left;
            }

            // Return success
            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// inflate() calls this whenever the window should be flushed. As
        /// MSZIP only expands to the size of the window, the implementation used
        /// simply keeps track of the amount of data flushed, and if more than 32k
        /// is flushed, an error is raised.
        /// </summary>
        private static Error FlushWindow(MSZIPDStream zip, uint data_flushed)
        {
            zip.BytesOutput += (int)data_flushed;
            if (zip.BytesOutput > MSZIP_FRAME_SIZE)
            {
                Console.WriteLine($"Overflow: {data_flushed} bytes flushed, total is now {zip.BytesOutput}");
                return Error.MSPACK_ERR_ARGS;
            }

            return Error.MSPACK_ERR_OK;
        }
    }
}
