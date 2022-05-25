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
    public partial class LZX
    {
        /// <summary>
        /// Allocates and initialises LZX decompression state for decoding an LZX
        /// stream.
        ///
        /// This routine uses system.alloc() to allocate memory. If memory
        /// allocation fails, or the parameters to this function are invalid,
        /// null is returned.
        /// </summary>
        /// <param name="system">A SystemImpl structure used to read from the input stream and write to the output stream, also to allocate and free memory.</param>
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
        /// a pointer to an initialised LZX structure, or null if
        /// there was not enough memory or parameters to the function were wrong.
        /// </returns>
        public static LZX Init(SystemImpl system, FileStream input, FileStream output, int window_bits, int reset_interval, int input_buffer_size, long output_length, bool is_delta)
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
            LZX lzx = new LZX()
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
        /// <param name="system">
        /// an mspack_system implementation to use with the
        /// input param. Only read() will be called.
        /// </param>
        /// <param name="input"> an input file handle to read reference data using system.read().</param>
        /// <param name="length">The length of the reference data. Cannot be longer than the LZX window size.</param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error SetReferenceData(SystemImpl system, FileStream input, uint length)
        {
            if (!IsDelta)
            {
                Console.WriteLine("Only LZX DELTA streams support reference data");
                return Error.MSPACK_ERR_ARGS;
            }

            if (Offset != 0)
            {
                Console.WriteLine("Too late to set reference data after decoding starts");
                return Error.MSPACK_ERR_ARGS;
            }

            if (length > WindowSize)
            {
                Console.WriteLine($"Reference length ({length}) is longer than the window");
                return Error.MSPACK_ERR_ARGS;
            }

            if (length > 0 && (system == null || input == null))
            {
                Console.WriteLine("Length > 0 but no system or input");
                return Error.MSPACK_ERR_ARGS;
            }

            ReferenceDataSize = length;
            if (length > 0)
            {
                // Copy reference data
                int pos = (int)(WindowSize - length);
                int bytes = system.Read(input, Window, pos, (int)length);

                // Length can't be more than 2^25, so no signedness problem
                if (bytes < (int)length)
                    return Error.MSPACK_ERR_READ;
            }

            ReferenceDataSize = length;
            return Error.MSPACK_ERR_OK;
        }

        // See description of outputLength in Init()
        public void SetOutputLength(long outputLength)
        {
            if (outputLength > 0)
                Length = outputLength;
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
        public Error Decompress(long out_bytes)
        {
            int warned = 0;
            byte[] buf = new byte[12];

            // Easy answers
            if (out_bytes < 0)
                return Error.MSPACK_ERR_ARGS;

            if (Error != Error.MSPACK_ERR_OK)
                return Error;

            // Flush out any stored-up bytes before we begin
            int leftover_bytes = OutputEnd - OutputPointer;
            if (leftover_bytes > out_bytes)
                leftover_bytes = (int)out_bytes;

            if (leftover_bytes != 0)
            {
                try { System.Write(OutputFileHandle, OutputIsE8 ? E8Buffer : Window, OutputPointer, leftover_bytes); }
                catch { return Error = Error.MSPACK_ERR_WRITE; }

                OutputPointer += leftover_bytes;
                Offset += leftover_bytes;
                out_bytes -= leftover_bytes;
            }

            if (out_bytes == 0)
                return Error.MSPACK_ERR_OK;

            uint end_frame = (uint)((Offset + out_bytes) / LZX_FRAME_SIZE) + 1;

            while (Frame < end_frame)
            {
                // Have we reached the reset interval? (if there is one?)
                if (ResetInterval != 0 && ((Frame % ResetInterval) == 0))
                {
                    if (BlockRemaining != 0)
                    {
                        // This is a file format error, we can make a best effort to extract what we can
                        Console.WriteLine($"{BlockRemaining} bytes remaining at reset interval");
                        if (warned == 0)
                        {
                            System.Message(null, "WARNING; invalid reset interval detected during LZX decompression");
                            warned++;
                        }
                    }

                    // Re-read the intel header and reset the huffman lengths
                    ResetState();
                }

                // LZX DELTA format has chunk_size, not present in LZX format
                if (IsDelta)
                {
                    ENSURE_BITS(16);
                    REMOVE_BITS_MSB(16);
                }

                // Calculate size of frame: all frames are 32k except the final frame
                // which is 32kb or less. this can only be calculated when Length
                // has been filled in.
                uint frame_size = LZX_FRAME_SIZE;
                if (Length != 0 && (Length - Offset) < frame_size)
                    frame_size = (uint)(Length - Offset);

                // Decode until one more frame is available
                int bytes_todo = (int)(FramePosition + frame_size - WindowPosition);
                while (bytes_todo > 0)
                {
                    ReadBlockHeader(buf);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    // Decode more of the block:
                    int this_run = Math.Min(BlockRemaining, bytes_todo);

                    // Assume we decode exactly this_run bytes, for now
                    bytes_todo -= this_run;
                    BlockRemaining -= this_run;

                    // Decode at least this_run bytes
                    switch (BlockType)
                    {
                        case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                        case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                            DecompressBlock(ref this_run);
                            if (Error != Error.MSPACK_ERR_OK)
                                return Error;

                            // If the literal 0xE8 is anywhere in the block...
                            if (MAINTREE_len[0xE8] != 0)
                                IntelStarted = true;

                            break;

                        case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                            // As this_run is limited not to wrap a frame, this also means it
                            // won't wrap the window (as the window is a multiple of 32k)
                            int rundest = WindowPosition;
                            WindowPosition += this_run;
                            while (this_run > 0)
                            {
                                int i = InputEnd - InputPointer;
                                if (i == 0)
                                {
                                    READ_IF_NEEDED();
                                    if (Error != Error.MSPACK_ERR_OK)
                                        return Error;
                                }
                                else
                                {
                                    if (i > this_run)
                                        i = this_run;

                                    Array.Copy(InputBuffer, InputPointer, Window, rundest, i);

                                    rundest += i;
                                    InputPointer += i;
                                    this_run -= i;
                                }
                            }

                            // Realign if this was an odd-sized UNCOMPRESSED block
                            if (InputFileHandle.Position != InputFileHandle.Length - 1 && (BlockLength & 1) != 0)
                            {
                                READ_IF_NEEDED();
                                if (Error != Error.MSPACK_ERR_OK)
                                    return Error;

                                InputPointer++;
                            }

                            // Because we can't assume otherwise
                            IntelStarted = true;

                            break;

                        default:
                            return Error = Error.MSPACK_ERR_DECRUNCH; // Might as well
                    }

                    // Did the final match overrun our desired this_run length?
                    if (this_run < 0)
                    {
                        if ((uint)(-this_run) > BlockRemaining)
                        {
                            Console.WriteLine($"Overrun went past end of block by {-this_run} ({BlockRemaining} remaining)");
                            return Error = Error.MSPACK_ERR_DECRUNCH;
                        }

                        BlockRemaining -= -this_run;
                    }
                }

                // Streams don't extend over frame boundaries
                if ((WindowPosition - FramePosition) != frame_size)
                {
                    Console.WriteLine($"Decode beyond output frame limits! {WindowPosition - FramePosition} != {frame_size}");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Re-align input bitstream
                if (BitsLeft > 0)
                    ENSURE_BITS(16);
                if ((BitsLeft & 15) != 0)
                    REMOVE_BITS_MSB(BitsLeft & 15);

                // Check that we've used all of the previous frame first
                if (OutputPointer != OutputEnd)
                {
                    Console.WriteLine($"{OutputEnd - OutputPointer} avail bytes, new {frame_size} frame");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Does this intel block _really_ need decoding?
                if (IntelStarted)
                {
                    UndoE8Preprocessing(frame_size);
                }
                else
                {
                    OutputIsE8 = false;
                    OutputPointer = (int)FramePosition;
                }

                OutputEnd = (int)(OutputPointer + frame_size);

                // Write a frame
                int new_out_bytes = (int)((out_bytes < frame_size) ? out_bytes : frame_size);
                try { System.Write(OutputFileHandle, OutputIsE8 ? E8Buffer : Window, OutputPointer, new_out_bytes); }
                catch { return Error = Error.MSPACK_ERR_WRITE; }

                OutputPointer += new_out_bytes;
                Offset += new_out_bytes;
                out_bytes -= new_out_bytes;

                // Advance frame start position
                FramePosition += frame_size;
                Frame++;

                // Wrap window / frame position pointers
                if (WindowPosition == WindowSize)
                    WindowPosition = 0;
                if (FramePosition == WindowSize)
                    FramePosition = 0;

            }

            if (out_bytes != 0)
            {
                Console.WriteLine("Bytes left to output");
                return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return Error.MSPACK_ERR_OK;
        }

        private Error BUILD_TABLE(ushort[] table, byte[] lengths, int tablebits, int maxsymbols)
        {
            if (!MakeDecodeTableMSB(maxsymbols, tablebits, lengths, table))
            {
                Console.WriteLine($"Failed to build table");
                return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        private Error BUILD_TABLE_MAYBE_EMPTY()
        {
            LENGTH_empty = 0;
            if (!MakeDecodeTableMSB(LZX_LENGTH_MAXSYMBOLS, LZX_LENGTH_TABLEBITS, LENGTH_len, LENGTH_table))
            {
                for (int i = 0; i < LZX_LENGTH_MAXSYMBOLS; i++)
                {
                    if (LENGTH_len[i] > 0)
                    {
                        Console.WriteLine("Failed to build table");
                        return Error = Error.MSPACK_ERR_DECRUNCH;
                    }
                }

                // Empty tree - allow it, but don't decode symbols with it
                LENGTH_empty = 1;
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        private Error READ_LENGTHS(byte[] lengths, uint first, uint last)
        {
            if (ReadLens(lengths, first, last) != Error.MSPACK_ERR_OK)
                return Error;

            return Error = Error.MSPACK_ERR_OK;
        }

        private Error Copy(uint match_offset, int match_len, ref int this_run)
        {
            // Copy match
            int rundest = WindowPosition;

            // Does match offset wrap the window?
            if (match_offset > WindowPosition)
            {
                if (match_offset > Offset && (match_offset - WindowPosition) > ReferenceDataSize)
                {
                    Console.WriteLine("Match offset beyond LZX stream");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // j = length from match offset to end of window
                int j = (int)(match_offset - WindowPosition);
                if (j > (int)WindowSize)
                {
                    Console.WriteLine("Match offset beyond window boundaries");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
                }

                int runsrc = (int)(WindowSize - j);
                if (j < match_len)
                {
                    // If match goes over the window edge, do two copy runs
                    Array.Copy(Window, runsrc, Window, rundest, j);
                    runsrc = 0;
                }

                Array.Copy(Window, runsrc, Window, rundest, match_len);
            }
            else
            {
                int runsrc = (int)(rundest - match_offset);
                Array.Copy(Window, runsrc, Window, rundest, match_len);
            }

            this_run -= match_len;
            WindowPosition += match_len;
            return Error = Error.MSPACK_ERR_OK;
        }

        private Error DecodeMatch(int main_element, ref int this_run)
        {
            // The main element is offset by 256 because values under 256 indicate a
            // literal value.
            main_element -= LZX_NUM_CHARS;

            // The length header consists of the lower 3 bits of the main element.
	        // The position slot is the rest of it.
            int length_header = main_element & LZX_NUM_PRIMARY_LENGTHS;
            int position_slot = main_element >> 3;

            // If the length_header is less than LZX_NUM_PRIMARY_LENS (= 7), it
            // gives the match length as the offset from LZX_MIN_MATCH_LEN.
            // Otherwise, the length is given by an additional symbol encoded using
            // the length tree, offset by 9 (LZX_MIN_MATCH_LEN + LZX_NUM_PRIMARY_LENS)
            int match_len = LZX_MIN_MATCH + length_header;
            if (length_header == LZX_NUM_PRIMARY_LENGTHS)
            {
                if (LENGTH_empty != 0)
                {
                    Console.WriteLine("LENGTH symbol needed but tree is empty");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
                }

                match_len += (int)READ_HUFFSYM_MSB(LENGTH_table, LENGTH_len, LZX_LENGTH_TABLEBITS, LZX_LENGTH_MAXSYMBOLS);
            }

            // If the position_slot is 0, 1, or 2, the match offset is retrieved
            // from the LRU queue.  Otherwise, the match offset is not in the LRU queue.
            uint match_offset;
            if (position_slot < 2)
            {
                // Note: This isn't a real LRU queue, since using the R2 offset
                // doesn't bump the R1 offset down to R2.  This quirk allows all
                // 3 recent offsets to be handled by the same code.  (For R0,
                // the swap is a no-op.)
                match_offset = R[position_slot];
                R[position_slot] = R[0];
                R[0] = match_offset;
            }
            else
            {
                // Otherwise, the offset was not encoded as one the offsets in
                // the queue.  Depending on the position slot, there is a
                // certain number of extra bits that need to be read to fully
                // decode the match offset.

                // Look up the number of extra bits that need to be read.
                int num_extra_bits = LZXExtraBits[position_slot];
                long verbatim_bits, aligned_bits;

                // For aligned blocks, if there are at least 3 extra bits, the
                // actual number of extra bits is 3 less, and they encode a
                // number of 8-byte words that are added to the offset; there
                // is then an additional symbol read using the aligned tree that
                // specifies the actual byte alignment.
                if (BlockType == LZXBlockType.LZX_BLOCKTYPE_ALIGNED && num_extra_bits >= 3)
                {
                    // There is an error in the LZX "specification" at this
                    // point; it indicates that a Huffman symbol is to be
                    // read only if num_extra_bits is greater than 3, but
                    // actually it is if num_extra_bits is greater than or
                    // equal to 3.  (Note that in the case with
                    // num_extra_bits == 3, the assignment to verbatim_bits
                    // will just set it to 0. )
                    verbatim_bits = READ_BITS_MSB(num_extra_bits - 3);
                    verbatim_bits <<= 3;
                    aligned_bits = READ_HUFFSYM_MSB(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS);
                }
                else
                {
                    // For non-aligned blocks, or for aligned blocks with
                    // less than 3 extra bits, the extra bits are added
                    // directly to the match offset, and the correction for
                    // the alignment is taken to be 0.
                    verbatim_bits = READ_BITS_MSB(num_extra_bits);
                    aligned_bits = 0;
                }

                // Calculate the match offset.
                match_offset = (uint)(LZXPositionBase[position_slot] + verbatim_bits + aligned_bits + 2); // LZX_OFFSET_OFFSET

                // Update the LRU queue.
                R[2] = R[1];
                R[1] = R[0];
                R[0] = match_offset;
            }

            // LZX DELTA uses max match length to signal even longer match
            if (length_header == LZX_MAX_MATCH && IsDelta)
            {
                int extra_len;

                // 4 entry huffman tree
                ENSURE_BITS(3);

                // '0' . 8 extra length bits
                if (PEEK_BITS_MSB(1) == 0)
                {
                    REMOVE_BITS_MSB(1);
                    extra_len = (int)READ_BITS_MSB(8);
                }

                // '10' . 10 extra length bits + 0x100
                else if (PEEK_BITS_MSB(2) == 2)
                {
                    REMOVE_BITS_MSB(2);
                    extra_len = (int)READ_BITS_MSB(10);
                    extra_len += 0x100;
                }

                // '110' . 12 extra length bits + 0x500
                else if (PEEK_BITS_MSB(3) == 6)
                {
                    REMOVE_BITS_MSB(3);
                    extra_len = (int)READ_BITS_MSB(12);
                    extra_len += 0x500;
                }

                // '111' . 15 extra length bits
                else
                {
                    REMOVE_BITS_MSB(3);
                    extra_len = (int)READ_BITS_MSB(15);
                }

                length_header += extra_len;
            }

            if ((WindowPosition + match_len) > WindowSize)
            {
                Console.WriteLine("Match ran over window wrap");
                return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            Copy(match_offset, match_len, ref this_run);
            return Error;
        }

        private Error DecompressBlock(ref int this_run)
        {
            while (this_run > 0)
            {
                int main_element = (int)READ_HUFFSYM_MSB(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                if (main_element < LZX_NUM_CHARS)
                {
                    // Literal: 0 to LZX_NUM_CHARS-1
                    Window[WindowPosition++] = (byte)main_element;
                    this_run--;
                }
                else
                {
                    DecodeMatch(main_element, ref this_run);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;
                }
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        private Error ReadBlockHeader(byte[] buffer)
        {
            ENSURE_BITS(4);

            // Read block type (3 bits) and block length (24 bits)
            byte block_type = (byte)READ_BITS_MSB(3);
            BlockType = (LZXBlockType)block_type;

            // Read the block size
            int block_size;
            if (READ_BITS_MSB(1) == 1)
            {
                block_size = LZX_FRAME_SIZE;
            }
            else
            {
                int tmp;
                block_size = 0;

                tmp = (int)READ_BITS_MSB(8);
                block_size |= tmp;
                tmp = (int)READ_BITS_MSB(8);
                block_size <<= 8;
                block_size |= tmp;

                if (WindowSize >= 65536)
                {
                    tmp = (int)READ_BITS_MSB(8);
                    block_size <<= 8;
                    block_size |= tmp;
                }
            }

            BlockRemaining = BlockLength = block_size;
            Console.WriteLine($"New block t {BlockType} len {BlockLength}");

            // Read individual block headers
            switch (BlockType)
            {
                case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                    // Read lengths of and build aligned huffman decoding tree
                    for (byte i = 0; i < 8; i++)
                    {
                        ALIGNED_len[i] = (byte)READ_BITS_MSB(3);
                    }

                    BUILD_TABLE(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    // Read lengths of and build main huffman decoding tree
                    READ_LENGTHS(MAINTREE_len, 0, 256);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    READ_LENGTHS(MAINTREE_len, 256, LZX_NUM_CHARS + NumOffsets);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    // Read lengths of and build lengths huffman decoding tree
                    READ_LENGTHS(LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE_MAYBE_EMPTY();
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    break;

                case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                    // Read lengths of and build main huffman decoding tree
                    READ_LENGTHS(MAINTREE_len, 0, 256);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    READ_LENGTHS(MAINTREE_len, 256, LZX_NUM_CHARS + NumOffsets);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    // If the literal 0xE8 is anywhere in the block...
                    if (MAINTREE_len[0xE8] != 0)
                        IntelStarted = true;

                    // Read lengths of and build lengths huffman decoding tree
                    READ_LENGTHS(LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS);
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    BUILD_TABLE_MAYBE_EMPTY();
                    if (Error != Error.MSPACK_ERR_OK)
                        return Error;

                    break;

                case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                    // Read 1-16 (not 0-15) bits to align to bytes
                    if (BitsLeft == 0)
                        ENSURE_BITS(16);

                    BitsLeft = 0; BitBuffer = 0;

                    // Read 12 bytes of stored R[0] / R[1] / R[2] values
                    for (int rundest = 0, k = 0; k < 12; k++)
                    {
                        READ_IF_NEEDED();
                        if (Error != Error.MSPACK_ERR_OK)
                            return Error;

                        buffer[rundest++] = InputBuffer[InputPointer++];
                    }

                    // TODO: uint[] R should be a part of a state object
                    R[0] = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
                    R[1] = (uint)(buffer[4] | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24));
                    R[2] = (uint)(buffer[8] | (buffer[9] << 8) | (buffer[10] << 16) | (buffer[11] << 24));

                    break;

                default:
                    Console.WriteLine($"Bad block type: {BlockType}");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        private Error ReadLens(byte[] lens, uint first, uint last)
        {
            // Read lengths for pretree (20 symbols, lengths stored in fixed 4 bits) 
            for (int i = 0; i < LZX_PRETREE_MAXSYMBOLS; i++)
            {
                uint y = (uint)READ_BITS_MSB(4);
                PRETREE_len[i] = (byte)y;
            }

            BUILD_TABLE(PRETREE_table, PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS);
            if (Error != Error.MSPACK_ERR_OK)
                return Error;

            for (uint lensPtr = first; lensPtr < last;)
            {
                uint num_zeroes, num_same;
                int tree_code = (int)READ_HUFFSYM_MSB(PRETREE_table, PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS);
                switch (tree_code)
                {
                    // Code = 17, run of ([read 4 bits]+4) zeros
                    case 17:
                        num_zeroes = (uint)READ_BITS_MSB(4);
                        num_zeroes += 4;
                        while (num_zeroes-- != 0)
                        {
                            lens[lensPtr++] = 0;
                        }

                        break;

                    // Code = 18, run of ([read 5 bits]+20) zeros
                    case 18:
                        num_zeroes = (uint)READ_BITS_MSB(5);
                        num_zeroes += 20;
                        while (num_zeroes-- != 0)
                        {
                            lens[lensPtr++] = 0;
                        }

                        break;

                    // Code = 19, run of ([read 1 bit]+4) [read huffman symbol]
                    case 19:
                        num_same = (uint)READ_BITS_MSB(1);
                        num_same += 4;

                        tree_code = (int)READ_HUFFSYM_MSB(PRETREE_table, PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS);
                        tree_code = lens[lensPtr] - tree_code;
                        if (tree_code < 0)
                            tree_code += 17;

                        while (num_same-- != 0)
                        {
                            lens[lensPtr++] = (byte)tree_code;
                        }

                        break;

                    // Code = 0 to 16, delta current length entry
                    default:
                        tree_code = lens[lensPtr] - tree_code;
                        if (tree_code < 0)
                            tree_code += 17;

                        lens[lensPtr++] = (byte)tree_code;
                        break;
                }
            }

            return Error.MSPACK_ERR_OK;
        }

        private void ResetState()
        {
            R[0] = 1;
            R[1] = 1;
            R[2] = 1;
            HeaderRead = 0;
            BlockRemaining = 0;
            BlockType = LZXBlockType.LZX_BLOCKTYPE_INVALID0;

            // Initialise tables to 0 (because deltas will be applied to them)
            for (int i = 0; i < LZX_MAINTREE_MAXSYMBOLS; i++)
            {
                MAINTREE_len[i] = 0;
            }

            for (int i = 0; i < LZX_LENGTH_MAXSYMBOLS; i++)
            {
                LENGTH_len[i] = 0;
            }
        }

        private void UndoE8Preprocessing(uint frame_size)
        {
            if (frame_size > 10)
            {
                // Finish any bytes that weren't processed by the vectorized implementation.
                int start = WindowPosition;
                int p8_end = (int)(WindowPosition + frame_size - 10);
                do
                {
                    if (Window[WindowPosition] == 0xe8)
                    {
                        int abs_offset, rel_offset;

                        // XXX: This assumes unaligned memory accesses are okay.
                        abs_offset = BitConverter.ToInt32(Window, WindowPosition + 1);
                        if (abs_offset >= 0)
                        {
                            if (abs_offset < 12_000_000)
                            {
                                // "good translation"
                                rel_offset = abs_offset - start;
                                Array.Copy(BitConverter.GetBytes(rel_offset), 0, Window, WindowPosition + 1, 4);
                            }
                        }
                        else
                        {
                            if (abs_offset >= -start)
                            {
                                // "compensating translation"
                                rel_offset = abs_offset + 12_000_000;
                                Array.Copy(BitConverter.GetBytes(rel_offset), 0, Window, WindowPosition + 1, 4);
                            }
                        }

                        WindowPosition += 5;
                    }
                    else
                    {
                        WindowPosition++;
                    }
                } while (WindowPosition < p8_end);
            }


            int data = 0;
            int dataend = (int)(frame_size - 10);
            int curpos = (int)Offset;
            int filesize = IntelFileSize;
            int abs_off, rel_off;

            // Copy e8 block to the e8 buffer and tweak if needed
            OutputIsE8 = true;
            OutputPointer = data;
            Array.Copy(Window, FramePosition, E8Buffer, data, frame_size);

            while (data < dataend)
            {
                if (E8Buffer[data++] != 0xE8)
                {
                    curpos++;
                    continue;
                }

                abs_off = E8Buffer[data + 0] | (E8Buffer[data + 1] << 8) | (E8Buffer[data + 2] << 16) | (E8Buffer[data + 3] << 24);
                if ((abs_off >= -curpos) && (abs_off < filesize))
                {
                    rel_off = (abs_off >= 0) ? abs_off - curpos : abs_off + filesize;
                    E8Buffer[data + 0] = (byte)rel_off;
                    E8Buffer[data + 1] = (byte)(rel_off >> 8);
                    E8Buffer[data + 2] = (byte)(rel_off >> 16);
                    E8Buffer[data + 3] = (byte)(rel_off >> 24);
                }

                data += 4;
                curpos += 5;
            }
        }
    }
}
