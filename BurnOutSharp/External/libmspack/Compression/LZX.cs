/* This file is part of libmspack.
 * (C) 2003-2013 Stuart Caie.
 *
 * The LZX method was created by Jonathan Forbes and Tomi Poutanen, adapted
 * by Microsoft Corporation.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

/* Microsoft's LZX document (in cab-sdk.exe) and their implementation
 * of the com.ms.util.cab Java package do not concur.
 *
 * In the LZX document, there is a table showing the correlation between
 * window size and the number of position slots. It states that the 1MB
 * window = 40 slots and the 2MB window = 42 slots. In the implementation,
 * 1MB = 42 slots, 2MB = 50 slots. The actual calculation is 'find the
 * first slot whose position base is equal to or more than the required
 * window size'. This would explain why other tables in the document refer
 * to 50 slots rather than 42.
 *
 * The constant NUM_PRIMARY_LENGTHS used in the decompression pseudocode
 * is not defined in the specification.
 *
 * The LZX document does not state the uncompressed block has an
 * uncompressed length field. Where does this length field come from, so
 * we can know how large the block is? The implementation has it as the 24
 * bits following after the 3 blocktype bits, before the alignment
 * padding.
 *
 * The LZX document states that aligned offset blocks have their aligned
 * offset huffman tree AFTER the main and length trees. The implementation
 * suggests that the aligned offset tree is BEFORE the main and length
 * trees.
 *
 * The LZX document decoding algorithm states that, in an aligned offset
 * block, if an extra_bits value is 1, 2 or 3, then that number of bits
 * should be read and the result added to the match offset. This is
 * correct for 1 and 2, but not 3, where just a huffman symbol (using the
 * aligned tree) should be read.
 *
 * Regarding the E8 preprocessing, the LZX document states 'No translation
 * may be performed on the last 6 bytes of the input block'. This is
 * correct.  However, the pseudocode provided checks for the *E8 leader*
 * up to the last 6 bytes. If the leader appears between -10 and -7 bytes
 * from the end, this would cause the next four bytes to be modified, at
 * least one of which would be in the last 6 bytes, which is not allowed
 * according to the spec.
 *
 * The specification states that the huffman trees must always contain at
 * least one element. However, many CAB files contain blocks where the
 * length tree is completely empty (because there are no matches), and
 * this is expected to succeed.
 *
 * The errors in LZX documentation appear have been corrected in the
 * new documentation for the LZX DELTA format.
 *
 *     http://msdn.microsoft.com/en-us/library/cc483133.aspx
 *
 * However, this is a different format, an extension of regular LZX.
 * I have noticed the following differences, there may be more:
 *
 * The maximum window size has increased from 2MB to 32MB. This also
 * increases the maximum number of position slots, etc.
 *
 * If the match length is 257 (the maximum possible), this signals
 * a further length decoding step, that allows for matches up to
 * 33024 bytes long.
 *
 * The format now allows for "reference data", supplied by the caller.
 * If match offsets go further back than the number of bytes
 * decompressed so far, that is them accessing the reference data.
 */

using System;
using System.IO;
using static LibMSPackSharp.Compression.Constants;

namespace LibMSPackSharp.Compression
{
    public class LZX
    {
        /// <summary>
        /// Allocates and initialises LZX decompression state for decoding an LZX
        /// stream.
        ///
        /// This routine uses system.alloc() to allocate memory. If memory
        /// allocation fails, or the parameters to this function are invalid,
        /// null is returned.
        /// </summary>
        /// <param name="system">
        /// an mspack_system structure used to read from
        /// the input stream and write to the output
        /// stream, also to allocate and free memory.
        /// </param>
        /// <param name="input">an input stream with the LZX data.</param>
        /// <param name="output">an output stream to write the decoded data to.</param>
        /// <param name="window_bits">
        /// the size of the decoding window, which must be
        /// between 15 and 21 inclusive for regular LZX
        /// data, or between 17 and 25 inclusive for
        /// LZX DELTA data.</param>
        /// <param name="reset_interval">
        /// the interval at which the LZX bitstream is
        /// reset, in multiples of LZX frames (32678
        /// bytes), e.g. a value of 2 indicates the input
        /// stream resets after every 65536 output bytes.
        /// A value of 0 indicates that the bitstream never
        /// resets, such as in CAB LZX streams.
        /// </param>
        /// <param name="input_buffer_size">
        /// the number of bytes to use as an input
        /// bitstream buffer.
        /// </param>
        /// <param name="output_length">
        /// the length in bytes of the entirely
        /// decompressed output stream, if known in
        /// advance. It is used to correctly perform the
        /// Intel E8 transformation, which must stop 6
        /// bytes before the very end of the
        /// decompressed stream. It is not otherwise used
        /// or adhered to. If the full decompressed
        /// length is known in advance, set it here.
        /// If it is NOT known, use the value 0, and call
        /// lzxd_set_outputLength() once it is
        /// known. If never set, 4 of the final 6 bytes
        /// of the output stream may be incorrect.
        /// </param>
        /// <param name="is_delta">
        /// should be zero for all regular LZX data,
        /// non-zero for LZX DELTA encoded data.
        /// </param>
        /// <returns>
        /// a pointer to an initialised LZXDStream structure, or null if
        /// there was not enough memory or parameters to the function were wrong.
        /// </returns>
        public static LZXDStream Init(SystemImpl system, FileStream input, FileStream output, int window_bits, int reset_interval, int input_buffer_size, long output_length, bool is_delta)
        {
            if (system == null)
                return null;

            // LZX DELTA window sizes are between 2^17 (128KiB) and 2^25 (32MiB),
            // regular LZX windows are between 2^15 (32KiB) and 2^21 (2MiB)
            if (is_delta)
            {
                if (window_bits < 17 || window_bits > 25)
                    return null;
            }
            else
            {
                if (window_bits < 15 || window_bits > 21)
                    return null;
            }

            if (reset_interval < 0 || output_length < 0)
            {
                Console.WriteLine("Reset interval or output length < 0");
                return null;
            }

            // Round up input buffer size to multiple of two
            input_buffer_size = (input_buffer_size + 1) & -2;
            if (input_buffer_size < 2)
                return null;

            // Allocate decompression state
            LZXDStream lzx = new LZXDStream()
            {
                // Allocate decompression window and input buffer
                Window = new byte[1 << window_bits],
                InputBuffer = new byte[input_buffer_size],

                System = system,
                InputFileHandle = input,
                OutputFileHandle = output,
                Offset = 0,
                Length = output_length,

                InputBufferSize = (uint)input_buffer_size,
                WindowSize = (uint)(1 << window_bits),
                ReferenceDataSize = 0,
                WindowPosition = 0,
                FramePosition = 0,
                Frame = 0,
                ResetInterval = (uint)reset_interval,
                IntelFileSize = 0,
                IntelStarted = false,
                Error = Error.MSPACK_ERR_OK,
                NumOffsets = LZXPositionSlots[window_bits - 15] << 3,
                IsDelta = is_delta,

                OutputPointer = 0,
                OutputEnd = 0,
                OutputIsE8 = true,
            };

            lzx.ResetState();
            lzx.INIT_BITS();

            return lzx;
        }

        /// <summary>
        /// Reads LZX DELTA reference data into the window and allows
        /// lzxd_decompress() to reference it.
        ///
        /// Call this before the first call to lzxd_decompress().
        /// </summary>
        /// <param name="lzx">the LZX stream to apply this reference data to</param>
        /// <param name="system">
        /// an mspack_system implementation to use with the
        /// input param. Only read() will be called.
        /// </param>
        /// <param name="input"> an input file handle to read reference data using system.read().</param>
        /// <param name="length">
        /// the length of the reference data. Cannot be longer
        /// than the LZX window size.
        /// </param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public static Error SetReferenceData(LZXDStream lzx, SystemImpl system, FileStream input, uint length)
        {
            if (lzx == null)
                return Error.MSPACK_ERR_ARGS;

            if (!lzx.IsDelta)
            {
                Console.WriteLine("Only LZX DELTA streams support reference data");
                return Error.MSPACK_ERR_ARGS;
            }

            if (lzx.Offset != 0)
            {
                Console.WriteLine("Too late to set reference data after decoding starts");
                return Error.MSPACK_ERR_ARGS;
            }

            if (length > lzx.WindowSize)
            {
                Console.WriteLine($"Reference length ({length}) is longer than the window");
                return Error.MSPACK_ERR_ARGS;
            }

            if (length > 0 && (system == null || input == null))
            {
                Console.WriteLine("Length > 0 but no system or input");
                return Error.MSPACK_ERR_ARGS;
            }

            lzx.ReferenceDataSize = length;
            if (length > 0)
            {
                // Copy reference data
                int pos = (int)(lzx.WindowSize - length);
                int bytes = system.Read(input, lzx.Window, pos, (int)length);

                // Length can't be more than 2^25, so no signedness problem
                if (bytes < (int)length)
                    return Error.MSPACK_ERR_READ;
            }

            lzx.ReferenceDataSize = length;
            return Error.MSPACK_ERR_OK;
        }

        // See description of outputLength in Init()
        public static void SetOutputLength(LZXDStream lzx, long outputLength)
        {
            if (lzx != null && outputLength > 0)
                lzx.Length = outputLength;
        }

        /// <summary>
        /// Decompresses entire or partial LZX streams.
        ///
        /// The number of bytes of data that should be decompressed is given as the
        /// out_bytes parameter. If more bytes are decoded than are needed, they
        /// will be kept over for a later invocation.
        ///
        /// The output bytes will be passed to the system.write() function given in
        /// lzxd_init(), using the output file handle given in lzxd_init(). More than
        /// one call may be made to system.write().
        /// Input bytes will be read in as necessary using the system.read()
        /// function given in lzxd_init(), using the input file handle given in
        /// lzxd_init().  This will continue until system.read() returns 0 bytes,
        /// or an error. Errors will be passed out of the function as
        /// MSPACK_ERR_READ errors.  Input streams should convey an "end of input
        /// stream" by refusing to supply all the bytes that LZX asks for when they
        /// reach the end of the stream, rather than return an error code.
        ///
        /// If any error code other than MSPACK_ERR_OK is returned, the stream
        /// should be considered unusable and lzxd_decompress() should not be
        /// called again on this stream.
        /// </summary>
        /// <param name="o">LZX decompression state, as allocated by lzxd_init().</param>
        /// <param name="out_bytes">the number of bytes of data to decompress.</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public static Error Decompress(object o, long out_bytes)
        {
            LZXDStream lzx = o as LZXDStream;
            if (lzx == null)
                return Error.MSPACK_ERR_ARGS;

            int warned = 0;
            byte[] buf = new byte[12];

            // Easy answers
            if (lzx == null || (out_bytes < 0))
                return Error.MSPACK_ERR_ARGS;

            if (lzx.Error != Error.MSPACK_ERR_OK)
                return lzx.Error;

            // Flush out any stored-up bytes before we begin
            int leftover_bytes = lzx.OutputEnd - lzx.OutputPointer;
            if (leftover_bytes > out_bytes)
                leftover_bytes = (int)out_bytes;

            if (leftover_bytes != 0)
            {
                try { lzx.System.Write(lzx.OutputFileHandle, lzx.OutputIsE8 ? lzx.E8Buffer : lzx.Window, lzx.OutputPointer, leftover_bytes); }
                catch { return lzx.Error = Error.MSPACK_ERR_WRITE; }

                lzx.OutputPointer += leftover_bytes;
                lzx.Offset += leftover_bytes;
                out_bytes -= leftover_bytes;
            }

            if (out_bytes == 0)
                return Error.MSPACK_ERR_OK;

            // Restore local state
            BufferState state = lzx.RESTORE_BITS();
            byte[] window = lzx.Window;
            int window_posn = lzx.WindowPosition;
            uint[] R = lzx.R;

            uint end_frame = (uint)((lzx.Offset + out_bytes) / LZX_FRAME_SIZE) + 1;

            while (lzx.Frame < end_frame)
            {
                // Have we reached the reset interval? (if there is one?)
                if (lzx.ResetInterval != 0 && ((lzx.Frame % lzx.ResetInterval) == 0))
                {
                    if (lzx.BlockRemaining != 0)
                    {
                        // This is a file format error, we can make a best effort to extract what we can
                        Console.WriteLine($"{lzx.BlockRemaining} bytes remaining at reset interval");
                        if (warned == 0)
                        {
                            lzx.System.Message(null, "WARNING; invalid reset interval detected during LZX decompression");
                            warned++;
                        }
                    }

                    // Re-read the intel header and reset the huffman lengths
                    lzx.ResetState();
                    R = lzx.R;
                }

                // LZX DELTA format has chunk_size, not present in LZX format
                if (lzx.IsDelta)
                {
                    lzx.ENSURE_BITS(16, state);
                    state.REMOVE_BITS_MSB(16);
                }

                //// Read header if necessary
                //if (lzx.HeaderRead == 0)
                //{
                //    // Read 1 bit. If bit=0, intel_filesize = 0.
                //    // If bit=1, read intel filesize (32 bits)
                //    int j = 0;
                //    int i = (int)lzx.READ_BITS_MSB(1, state);

                //    if (i != 0)
                //    {
                //        i = (int)lzx.READ_BITS_MSB(16, state);
                //        j = (int)lzx.READ_BITS_MSB(16, state);
                //    }

                //    lzx.IntelFileSize = (i << 16) | j;
                //    lzx.HeaderRead = 1;
                //}

                // Calculate size of frame: all frames are 32k except the final frame
                // which is 32kb or less. this can only be calculated when lzx.Length
                // has been filled in.
                uint frame_size = LZX_FRAME_SIZE;
                if (lzx.Length != 0 && (lzx.Length - lzx.Offset) < frame_size)
                    frame_size = (uint)(lzx.Length - lzx.Offset);

                // Decode until one more frame is available
                int bytes_todo = (int)(lzx.FramePosition + frame_size - window_posn);
                while (bytes_todo > 0)
                {
                    // Realign if previous block was an odd-sized UNCOMPRESSED block
                    if ((lzx.BlockType == LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED) && (lzx.BlockLength & 1) != 0)
                    {
                        lzx.READ_IF_NEEDED(state);
                        if (lzx.Error != Error.MSPACK_ERR_OK)
                            return lzx.Error;

                        state.InputPointer++;
                    }

                    lzx.ReadBlockHeader(buf, ref R, state);
                    if (lzx.Error != Error.MSPACK_ERR_OK)
                        return lzx.Error;

                    // Decode more of the block:
                    int this_run = Math.Min(lzx.BlockRemaining, bytes_todo);

                    // Assume we decode exactly this_run bytes, for now
                    bytes_todo -= this_run;
                    lzx.BlockRemaining -= this_run;

                    // Decode at least this_run bytes
                    switch (lzx.BlockType)
                    {
                        case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                        case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                            lzx.DecompressBlock(window, ref window_posn, ref this_run, ref R, state);
                            if (lzx.Error != Error.MSPACK_ERR_OK)
                                return lzx.Error;

                            // If the literal 0xE8 is anywhere in the block...
                            if (lzx.MAINTREE_len[0xE8] != 0)
                                lzx.IntelStarted = true;

                            break;

                        case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                            // As this_run is limited not to wrap a frame, this also means it
                            // won't wrap the window (as the window is a multiple of 32k)
                            int rundest = window_posn;
                            window_posn += this_run;
                            while (this_run > 0)
                            {
                                int i = state.InputEnd - state.InputPointer;
                                if (i == 0)
                                {
                                    lzx.READ_IF_NEEDED(state);
                                    if (lzx.Error != Error.MSPACK_ERR_OK)
                                        return lzx.Error;
                                }
                                else
                                {
                                    if (i > this_run)
                                        i = this_run;

                                    Array.Copy(lzx.InputBuffer, state.InputPointer, window, rundest, i);

                                    rundest += i;
                                    state.InputPointer += i;
                                    this_run -= i;
                                }
                            }

                            // Because we can't assume otherwise
                            lzx.IntelStarted = true;

                            break;

                        default:
                            return lzx.Error = Error.MSPACK_ERR_DECRUNCH; // Might as well
                    }

                    // Did the final match overrun our desired this_run length?
                    if (this_run < 0)
                    {
                        if ((uint)(-this_run) > lzx.BlockRemaining)
                        {
                            Console.WriteLine($"Overrun went past end of block by {-this_run} ({lzx.BlockRemaining} remaining)");
                            return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                        }

                        lzx.BlockRemaining -= -this_run;
                    }
                }

                // Streams don't extend over frame boundaries
                if ((window_posn - lzx.FramePosition) != frame_size)
                {
                    Console.WriteLine($"Decode beyond output frame limits! {window_posn - lzx.FramePosition} != {frame_size}");
                    return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Re-align input bitstream
                if (state.BitsLeft > 0)
                    lzx.ENSURE_BITS(16, state);
                if ((state.BitsLeft & 15) != 0)
                    state.REMOVE_BITS_MSB(state.BitsLeft & 15);

                // Check that we've used all of the previous frame first
                if (lzx.OutputPointer != lzx.OutputEnd)
                {
                    Console.WriteLine($"{lzx.OutputEnd - lzx.OutputPointer} avail bytes, new {frame_size} frame");
                    return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Does this intel block _really_ need decoding?
                if (lzx.IntelStarted && lzx.IntelFileSize != 0 && (lzx.Frame < 32768) && (frame_size > 10))
                {
                    lzx.UndoE8Preprocessing(frame_size);
                }
                else
                {
                    lzx.OutputIsE8 = false;
                    lzx.OutputPointer = (int)lzx.FramePosition;
                }

                lzx.OutputEnd = (int)(lzx.OutputPointer + frame_size);

                // Write a frame
                int new_out_bytes = (int)((out_bytes < frame_size) ? out_bytes : frame_size);
                try { lzx.System.Write(lzx.OutputFileHandle, lzx.OutputIsE8 ? lzx.E8Buffer : lzx.Window, lzx.OutputPointer, new_out_bytes); }
                catch { return lzx.Error = Error.MSPACK_ERR_WRITE; }

                lzx.OutputPointer += new_out_bytes;
                lzx.Offset += new_out_bytes;
                out_bytes -= new_out_bytes;

                // Advance frame start position
                lzx.FramePosition += frame_size;
                lzx.Frame++;

                // Wrap window / frame position pointers
                if (window_posn == lzx.WindowSize)
                    window_posn = 0;
                if (lzx.FramePosition == lzx.WindowSize)
                    lzx.FramePosition = 0;

            }

            if (out_bytes != 0)
            {
                Console.WriteLine("Bytes left to output");
                return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
            }

            // Store local state
            lzx.STORE_BITS(state);
            lzx.WindowPosition = window_posn;
            lzx.R = R;

            return Error.MSPACK_ERR_OK;
        }
    }
}
