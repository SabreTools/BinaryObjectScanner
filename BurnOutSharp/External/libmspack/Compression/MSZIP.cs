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
            MSZIPDStream zip = new MSZIPDStream();

            // Allocate input buffer
            zip.InputBuffer = new byte[input_buffer_size];

            // Initialise decompression state
            zip.Sys = system;
            zip.Input = input;
            zip.Output = output;
            zip.InputBufferSize = (uint)input_buffer_size;
            zip.InputEnd = 0;
            zip.Error = Error.MSPACK_ERR_OK;
            zip.RepairMode = repair_mode;
            zip.FlushWindow = FlushWindow;

            zip.InputPointer = zip.InputLength = 0;
            zip.OutputPointer = zip.OutputLength = 0;
            zip.BitBuffer = 0; zip.BitsLeft = 0;

            return zip;
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
            uint bit_buffer = 0, bits_left = 0;
            int i_ptr = 0, i_end = 0;

            int i, state, error;

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
                zip.RESTORE_BITS(ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                // Skip to next read 'CK' header
                i = (int)(bits_left & 7);
                zip.REMOVE_BITS(i, ref bits_left, ref bit_buffer, msb: false); // Align to bytestream
                state = 0;

                do
                {
                    zip.READ_BITS(ref i, 8, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                    if (zip.Error != Error.MSPACK_ERR_OK)
                        return zip.Error;

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
                zip.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);

                if ((error = (int)Inflate(zip)) != 0)
                {
                    Console.WriteLine($"inflate error {(InflateErrorCode)error}");
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
                        return zip.Error = (error > 0) ? (Error)error : Error.MSPACK_ERR_DECRUNCH;
                    }
                }
                zip.OutputPointer = 0;
                zip.OutputLength = zip.BytesOutput;

                // Write a frame
                i = (out_bytes < zip.BytesOutput) ? (int)out_bytes : zip.BytesOutput;
                if (zip.Output is FileStream)
                {
                    if (SystemImpl.DefaultSystem.Write(zip.Output, zip.Window, zip.OutputPointer, i) != i)
                        return zip.Error = Error.MSPACK_ERR_WRITE;
                }
                else
                {
                    if (zip.Sys.Write(zip.Output, zip.Window, zip.OutputPointer, i) != i)
                        return zip.Error = Error.MSPACK_ERR_WRITE;
                }

                // mspack errors (i.e. read errors) are fatal and can't be recovered
                if (error > 0 && zip.RepairMode)
                    return (Error)error;

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
            uint bit_buffer = 0, bits_left = 0;
            int i_ptr = 0, i_end = 0;

            int i = 0, error, block_len = 0;

            // Unpack blocks until block_len == 0
            for (; ; )
            {
                zip.RESTORE_BITS(ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                // Align to bytestream, read block_len
                i = (int)(bits_left & 7);
                zip.REMOVE_BITS(i, ref bits_left, ref bit_buffer, msb: false);
                zip.READ_BITS(ref block_len, 8, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return zip.Error;

                zip.READ_BITS(ref i, 8, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return zip.Error;

                block_len |= i << 8;

                if (block_len == 0)
                    break;

                // Read "CK" header
                zip.READ_BITS(ref i, 8, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return zip.Error;
                if (i != 'C')
                    return Error.MSPACK_ERR_DATAFORMAT;

                zip.READ_BITS(ref i, 8, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return zip.Error;
                if (i != 'K')
                    return Error.MSPACK_ERR_DATAFORMAT;

                // Inflate block
                zip.WindowPosition = 0;
                zip.BytesOutput = 0;
                zip.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);
                if ((error = (int)Inflate(zip)) != 0)
                {
                    Console.WriteLine($"inflate error {(InflateErrorCode)error}");
                    return zip.Error = (error > 0) ? (Error)error : Error.MSPACK_ERR_DECRUNCH;
                }

                // Write inflated block
                if (zip.Sys.Write(zip.Output, zip.Window, 0, zip.BytesOutput) != zip.BytesOutput)
                    return zip.Error = Error.MSPACK_ERR_WRITE;
            }

            return Error.MSPACK_ERR_OK;
        }

        private static InflateErrorCode ReadLens(MSZIPDStream zip)
        {
            // For the bit buffer and huffman decoding
            uint bit_buffer = 0, bits_left = 0;
            int i_ptr = 0, i_end = 0;

            // Bitlen Huffman codes -- immediate lookup, 7 bit max code length
            ushort[] bl_table = new ushort[1 << 7];
            byte[] bl_len = new byte[19];

            byte[] lens = new byte[MSZIP_LITERAL_MAXSYMBOLS + MSZIP_DISTANCE_MAXSYMBOLS];
            int lit_codes = 0, dist_codes = 0, code, last_code = 0, bitlen_codes = 0, i, run = 0;

            zip.RESTORE_BITS(ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

            // Read the number of codes
            zip.READ_BITS(ref lit_codes, 5, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
            if (zip.Error != Error.MSPACK_ERR_OK)
                return InflateErrorCode.INF_ERR_BITOVERRUN;

            lit_codes += 257;
            zip.READ_BITS(ref dist_codes, 5, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
            if (zip.Error != Error.MSPACK_ERR_OK)
                return InflateErrorCode.INF_ERR_BITOVERRUN;

            dist_codes += 1;
            zip.READ_BITS(ref bitlen_codes, 4, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
            if (zip.Error != Error.MSPACK_ERR_OK)
                return InflateErrorCode.INF_ERR_BITOVERRUN;

            bitlen_codes += 4;

            if (lit_codes > MSZIP_LITERAL_MAXSYMBOLS)
                return InflateErrorCode.INF_ERR_SYMLENS;
            if (dist_codes > MSZIP_DISTANCE_MAXSYMBOLS)
                return InflateErrorCode.INF_ERR_SYMLENS;

            // Read in the bit lengths in their unusual order
            for (i = 0; i < bitlen_codes; i++)
            {
                int blLenTemp = bl_len[bitlen_order[i]];
                zip.READ_BITS(ref blLenTemp, 3, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return InflateErrorCode.INF_ERR_BITOVERRUN;

                bl_len[bitlen_order[i]] = (byte)blLenTemp;
            }

            while (i < 19)
            {
                bl_len[bitlen_order[i++]] = 0;
            }

            // Create decoding table with an immediate lookup
            if (!MSZIPDStream.MakeDecodeTable(19, 7, bl_len, bl_table, msb: false))
                return InflateErrorCode.INF_ERR_BITLENTBL;

            // Read literal / distance code lengths
            for (i = 0; i < (lit_codes + dist_codes); i++)
            {
                // Single-level huffman lookup
                zip.ENSURE_BITS(7, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return InflateErrorCode.INF_ERR_BITBUF;

                code = bl_table[zip.PEEK_BITS(7, bit_buffer, msb: false)];
                zip.REMOVE_BITS(bl_len[code], ref bits_left, ref bit_buffer, msb: false);

                if (code < 16)
                {
                    lens[i] = (byte)(last_code = code);
                }
                else
                {
                    switch (code)
                    {
                        case 16:
                            zip.READ_BITS(ref run, 2, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                            if (zip.Error != Error.MSPACK_ERR_OK)
                                return InflateErrorCode.INF_ERR_BITOVERRUN;

                            run += 3;
                            code = last_code;
                            break;

                        case 17:
                            zip.READ_BITS(ref run, 3, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                            if (zip.Error != Error.MSPACK_ERR_OK)
                                return InflateErrorCode.INF_ERR_BITOVERRUN;

                            run += 3;
                            code = 0;
                            break;

                        case 18:
                            zip.READ_BITS(ref run, 7, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                            if (zip.Error != Error.MSPACK_ERR_OK)
                                return InflateErrorCode.INF_ERR_BITOVERRUN;

                            run += 11;
                            code = 0;
                            break;

                        default:
                            Console.WriteLine($"bad code!: {code}");
                            return InflateErrorCode.INF_ERR_BADBITLEN;
                    }

                    if ((i + run) > (lit_codes + dist_codes))
                        return InflateErrorCode.INF_ERR_BITOVERRUN;

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

            zip.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);
            return 0;
        }

        /// <summary>
        /// A clean implementation of RFC 1951 / inflate
        /// </summary>
        private static InflateErrorCode Inflate(MSZIPDStream zip)
        {
            int last_block = 0, block_type = 0, distance = 0, length = 0, this_run, i;

            // For the bit buffer and huffman decoding
            uint bit_buffer = 0, bits_left = 0;
            int i_ptr = 0, i_end = 0;
            InflateErrorCode err;

            zip.RESTORE_BITS(ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

            do
            {
                // Read in last block bit
                zip.READ_BITS(ref last_block, 1, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return InflateErrorCode.INF_ERR_BITOVERRUN;

                // Read in block type
                zip.READ_BITS(ref block_type, 2, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                if (zip.Error != Error.MSPACK_ERR_OK)
                    return InflateErrorCode.INF_ERR_BITOVERRUN;

                if (block_type == 0)
                {
                    // Uncompressed block
                    byte[] lens_buf = new byte[4];

                    // Go to byte boundary
                    i = (int)(bits_left & 7);
                    zip.REMOVE_BITS(i, ref bits_left, ref bit_buffer, msb: false);

                    // Read 4 bytes of data, emptying the bit-buffer if necessary
                    for (i = 0; (bits_left >= 8); i++)
                    {
                        if (i == 4)
                            return InflateErrorCode.INF_ERR_BITBUF;

                        lens_buf[i] = (byte)zip.PEEK_BITS(8, bit_buffer, msb: false);
                        zip.REMOVE_BITS(8, ref bits_left, ref bit_buffer, msb: false);
                    }

                    if (bits_left != 0)
                        return InflateErrorCode.INF_ERR_BITBUF;

                    while (i < 4)
                    {
                        zip.READ_IF_NEEDED(ref i_ptr, ref i_end);
                        if (zip.Error != Error.MSPACK_ERR_OK)
                            return InflateErrorCode.INF_ERR_BITBUF;

                        lens_buf[i++] = zip.InputBuffer[i_ptr++];
                    }

                    // Get the length and its complement
                    length = lens_buf[0] | (lens_buf[1] << 8);
                    i = lens_buf[2] | (lens_buf[3] << 8);
                    if (length != (~i & 0xFFFF))
                        return InflateErrorCode.INF_ERR_COMPLEMENT;

                    // Read and copy the uncompressed data into the window
                    while (length > 0)
                    {
                        zip.READ_IF_NEEDED(ref i_ptr, ref i_end);
                        if (zip.Error != Error.MSPACK_ERR_OK)
                            return InflateErrorCode.INF_ERR_BITBUF;

                        this_run = length;
                        if (this_run > (uint)(i_end - i_ptr))
                            this_run = i_end - i_ptr;

                        if (this_run > (MSZIP_FRAME_SIZE - zip.WindowPosition))
                            this_run = (int)(MSZIP_FRAME_SIZE - zip.WindowPosition);

                        Array.Copy(zip.InputBuffer, i_ptr, zip.Window, (int)zip.WindowPosition, this_run);
                        zip.WindowPosition += (uint)this_run;
                        i_ptr += this_run;
                        length -= this_run;

                        // FLUSH_IF_NEEDED
                        if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                        {
                            if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                return InflateErrorCode.INF_ERR_FLUSH;

                            zip.WindowPosition = 0;
                        }
                    }
                }
                else if ((block_type == 1) || (block_type == 2))
                {
                    // Huffman-compressed LZ77 block
                    uint match_posn, code = 0;

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
                        zip.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);

                        if ((i = (int)ReadLens(zip)) != 0)
                            return (InflateErrorCode)i;

                        zip.RESTORE_BITS(ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                    }

                    // Now huffman lengths are read for either kind of block, 
                    // create huffman decoding tables
                    if (!MSZIPDStream.MakeDecodeTable(MSZIP_LITERAL_MAXSYMBOLS, MSZIP_LITERAL_TABLEBITS, zip.LITERAL_len, zip.LITERAL_table, msb: false))
                    {
                        // TODO: Figure out why this always gets hit
                        //return InflateErrorCode.INF_ERR_LITERALTBL;
                    }

                    if (!MSZIPDStream.MakeDecodeTable(MSZIP_DISTANCE_MAXSYMBOLS, MSZIP_DISTANCE_TABLEBITS, zip.DISTANCE_len, zip.DISTANCE_table, msb: false))
                    {
                        // TODO: Figure out why this always gets hit
                        //return InflateErrorCode.INF_ERR_DISTANCETBL;
                    }

                    // Decode forever until end of block code
                    for (; ; )
                    {
                        if ((err = (InflateErrorCode)zip.READ_HUFFSYM(zip.LITERAL_table, ref code, MSZIP_LITERAL_TABLEBITS, zip.LITERAL_len, MSZIP_LITERAL_MAXSYMBOLS, ref i, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: false)) != InflateErrorCode.INF_ERR_OK)
                            return err;

                        if (code < 256)
                        {
                            zip.Window[zip.WindowPosition++] = (byte)code;

                            // FLUSH_IF_NEEDED
                            if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                            {
                                if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                    return InflateErrorCode.INF_ERR_FLUSH;

                                zip.WindowPosition = 0;
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
                                return InflateErrorCode.INF_ERR_LITCODE; // Codes 286-287 are illegal

                            zip.READ_BITS_T(ref length, lit_extrabits[code], ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                            if (zip.Error != Error.MSPACK_ERR_OK)
                                return InflateErrorCode.INF_ERR_LITCODE;

                            length += lit_lengths[code];

                            if ((err = (InflateErrorCode)zip.READ_HUFFSYM(zip.DISTANCE_table, ref code, MSZIP_DISTANCE_TABLEBITS, zip.DISTANCE_len, MSZIP_DISTANCE_MAXSYMBOLS, ref i, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: false)) != 0)
                                return err;

                            if (code >= 30)
                                return InflateErrorCode.INF_ERR_DISTCODE;

                            zip.READ_BITS_T(ref distance, dist_extrabits[code], ref i_ptr, ref i_end, ref bit_buffer, ref bits_left, msb: false);
                            if (zip.Error != Error.MSPACK_ERR_OK)
                                return InflateErrorCode.INF_ERR_DISTCODE;

                            distance += dist_offsets[code];

                            // Match position is window position minus distance. If distance
                            // is more than window position numerically, it must 'wrap
                            // around' the frame size.
                            match_posn = (uint)(((distance > zip.WindowPosition) ? MSZIP_FRAME_SIZE : 0) + zip.WindowPosition - distance);

                            // Copy match
                            if (length < 12)
                            {
                                // Short match, use slower loop but no loop setup code
                                while (length-- != 0)
                                {
                                    zip.Window[zip.WindowPosition++] = zip.Window[match_posn++];
                                    match_posn &= MSZIP_FRAME_SIZE - 1;

                                    // FLUSH_IF_NEEDED
                                    if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                                    {
                                        if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                            return InflateErrorCode.INF_ERR_FLUSH;

                                        zip.WindowPosition = 0;
                                    }
                                }
                            }
                            else
                            {
                                // Longer match, use faster loop but with setup expense
                                int runsrc, rundest;
                                do
                                {
                                    this_run = length;
                                    if ((match_posn + this_run) > MSZIP_FRAME_SIZE)
                                        this_run = MSZIP_FRAME_SIZE - (int)match_posn;

                                    if ((zip.WindowPosition + this_run) > MSZIP_FRAME_SIZE)
                                        this_run = MSZIP_FRAME_SIZE - (int)zip.WindowPosition;

                                    rundest = (int)zip.WindowPosition;
                                    zip.WindowPosition += (uint)this_run;
                                    runsrc = (int)match_posn;
                                    match_posn += (uint)this_run;
                                    length -= this_run;

                                    while (this_run-- != 0)
                                    {
                                        zip.Window[rundest++] = zip.Window[runsrc++];
                                    }

                                    if (match_posn == MSZIP_FRAME_SIZE)
                                        match_posn = 0;

                                    // FLUSH_IF_NEEDED
                                    if (zip.WindowPosition == MSZIP_FRAME_SIZE)
                                    {
                                        if (zip.FlushWindow(zip, MSZIP_FRAME_SIZE) != Error.MSPACK_ERR_OK)
                                            return InflateErrorCode.INF_ERR_FLUSH;

                                        zip.WindowPosition = 0;
                                    }
                                } while (length > 0);
                            }
                        }
                    }
                }
                else
                {
                    // block_type == 3 -- bad block type
                    return InflateErrorCode.INF_ERR_BLOCKTYPE;
                }
            } while (last_block == 0);

            // Flush the remaining data
            if (zip.WindowPosition != 0)
            {
                if (zip.FlushWindow(zip, zip.WindowPosition) != Error.MSPACK_ERR_OK)
                    return InflateErrorCode.INF_ERR_FLUSH;
            }

            zip.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);

            // Return success
            return InflateErrorCode.INF_ERR_OK;
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
                Console.WriteLine($"overflow: {data_flushed} bytes flushed, total is now {zip.BytesOutput}");
                return Error.MSPACK_ERR_ARGS;
            }

            return Error.MSPACK_ERR_OK;
        }
    }
}
