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
        #region Public Functionality

        /// <summary>
        /// Allocates and initialises LZX decompression state for decoding an LZX
        /// stream.
        ///
        /// This routine uses system.alloc() to allocate memory. If memory
        /// allocation fails, or the parameters to this function are invalid,
        /// null is returned.
        /// </summary>
        /// <param name="system">A SystemImpl structure used to read from the input stream and write to the output stream, also to allocate and free memory.</param>
        /// <param name="input">An input stream with the LZX data.</param>
        /// <param name="output">An output stream to write the decoded data to.</param>
        /// <param name="window_bits">The size of the decoding window, which must be between 15 and 21 inclusive for regular LZX data, or between 17 and 25 inclusive for LZX DELTA data.</param>
        /// <param name="reset_interval">
        /// The interval at which the LZX bitstream is reset, in multiples of LZX frames (32678 bytes),
        /// e.g. a value of 2 indicates the input stream resets after every 65536 output bytes.
        /// A value of 0 indicates that the bitstream never resets, such as in CAB LZX streams.
        /// </param>
        /// <param name="input_buffer_size">The number of bytes to use as an input bitstream buffer.</param>
        /// <param name="output_length">
        /// The length in bytes of the entirely decompressed output stream, if known in advance.
        /// It is used to correctly perform the Intel E8 transformation, which must stop 6 bytes before the very end of the decompressed stream.
        /// It is not otherwise used or adhered to. If the full decompressed length is known in advance, set it here.
        /// If it is NOT known, use the value 0, and call lzxd_set_outputLength() once it is known.
        /// If never set, 4 of the final 6 bytes of the output stream may be incorrect
        /// </param>
        /// <param name="is_delta">Should be zero for all regular LZX data, non-zero for LZX DELTA encoded data.</param>
        /// <returns>A pointer to an initialised LZX structure, or null if there was not enough memory or parameters to the function were wrong.</returns>
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
                WriteFromE8 = false,
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
        /// <param name="system">A SystemImpl structure to use with the input param. Only read() will be called.</param>
        /// <param name="input">An input file handle to read reference data using system.read().</param>
        /// <param name="length">The length of the reference data. Cannot be longer than the LZX window size.</param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error SetReferenceData(SystemImpl system, FileStream input, uint length)
        {
            if (!IsDelta)
            {
                Console.WriteLine("Only LZX DELTA streams support reference data");
                return Error.MSPACK_ERR_ARGS;
            }

            if (Offset > 0)
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
        /// <param name="out_bytes">The number of bytes of data to decompress.</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Error Decompress(long out_bytes)
        {
            int match_length, length_footer, extra, verbatim_bits, bytes_todo;
            int this_run, main_element, aligned_bits, j, warned = 0;
            byte[] buf = new byte[12];
            int runsrc, rundest;
            uint frame_size = 0, end_frame, match_offset;

            // Easy answers
            if (out_bytes < 0)
                return Error.MSPACK_ERR_ARGS;
            if (Error != Error.MSPACK_ERR_OK)
                return Error;

            // Flush out any stored-up bytes before we begin
            int i = OutputEnd - OutputPointer;
            if (i > out_bytes)
                i = (int)out_bytes;

            if (i > 0)
            {
                if (System.Write(OutputFileHandle, WriteFromE8 ? E8Buffer : Window, OutputPointer, i) != i)
                    return Error = Error.MSPACK_ERR_WRITE;

                OutputPointer += i;
                Offset        += i;
                out_bytes     -= i;
            }

            if (out_bytes == 0)
                return Error.MSPACK_ERR_OK;

            end_frame = (uint)((Offset + out_bytes) / LZX_FRAME_SIZE) + 1;

            while (Frame < end_frame)
            {
                // Have we reached the reset interval? (if there is one?)
                if (ResetInterval != 0 && ((Frame % ResetInterval) == 0))
                {
                    if (BlockRemaining > 0)
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

                // Read header if necessary
                if (HeaderRead == 0)
                {
                    // Read 1 bit. If bit=0, intel filesize = 0.
                    // If bit=1, read intel filesize (32 bits)
                    j = 0;
                    i = (int)READ_BITS_MSB(1);
                    if (i != 0)
                    {
                        i = (int)READ_BITS_MSB(16);
                        j = (int)READ_BITS_MSB(16);
                    }

                    IntelFileSize = (i << 16) | j;
                    HeaderRead = 1;
                }

                // Calculate size of frame: all frames are 32k except the final frame
                // which is 32kb or less. this can only be calculated when lzx->length
                // has been filled in.
                frame_size = LZX_FRAME_SIZE;
                if (Length != 0 && (Length - Offset) < frame_size)
                    frame_size = (uint)(Length - Offset);

                // Decode until one more frame is available
                bytes_todo = (int)(FramePosition + frame_size - WindowPosition);
                while (bytes_todo > 0)
                {
                    // Initialise new block, if one is needed
                    if (BlockRemaining == 0)
                    {
                        // Realign if previous block was an odd-sized UNCOMPRESSED block
                        if (BlockType == LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED && (BlockLength & 1) != 0)
                        {
                            READ_IF_NEEDED();
                            InputPointer++;
                        }

                        // Read block type (3 bits) and block length (24 bits)
                        BlockType = (LZXBlockType)READ_BITS_MSB(3);
                        i = (int)READ_BITS_MSB(16);
                        j = (int)READ_BITS_MSB(8);
                        BlockRemaining = BlockLength = (i << 8) | j;
                        Console.WriteLine($"New block - type: {BlockType}, length: {BlockLength}");

                        // Read individual block headers
                        switch (BlockType)
                        {
                            case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                                // Read lengths of and build aligned huffman decoding tree
                                for (i = 0; i < 8; i++)
                                {
                                    j = (int)READ_BITS_MSB(3);
                                    ALIGNED_len[i] = (byte)j;
                                }

                                BUILD_TABLE(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS);

                                // Rest of aligned header is same as verbatim

                                // Read lengths of and build main huffman decoding tree
                                ReadLengths(MAINTREE_len, 0, 256);
                                ReadLengths(MAINTREE_len, 256, LZX_NUM_CHARS + NumOffsets);
                                BUILD_TABLE(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);

                                // If the literal 0xE8 is anywhere in the block...
                                if (MAINTREE_len[0xE8] != 0)
                                    IntelStarted = true;

                                // Read lengths of and build lengths huffman decoding tree
                                ReadLengths(LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS);
                                BUILD_TABLE_MAYBE_EMPTY();
                                break;

                            case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                                // Read lengths of and build main huffman decoding tree
                                ReadLengths(MAINTREE_len, 0, 256);
                                ReadLengths(MAINTREE_len, 256, LZX_NUM_CHARS + NumOffsets);
                                BUILD_TABLE(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);

                                // If the literal 0xE8 is anywhere in the block...
                                if (MAINTREE_len[0xE8] != 0)
                                    IntelStarted = true;

                                // Read lengths of and build lengths huffman decoding tree
                                ReadLengths(LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS);
                                BUILD_TABLE_MAYBE_EMPTY();
                                break;

                            case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                                // Because we can't assume otherwise
                                IntelStarted = true;

                                // Read 1-16 (not 0-15) bits to align to bytes
                                if (BitsLeft == 0)
                                    ENSURE_BITS(16);

                                BitsLeft = 0;
                                BitBuffer = 0;

                                // Read 12 bytes of stored R0 / R1 / R2 values
                                for (rundest = 0, i = 0; i < 12; i++)
                                {
                                    READ_IF_NEEDED();
                                    buf[rundest++] = InputBuffer[InputPointer++];
                                }

                                R[0] = (uint)(buf[0] | (buf[1] << 8) | (buf[2]  << 16) | (buf[3] << 24));
                                R[1] = (uint)(buf[4] | (buf[5] << 8) | (buf[6]  << 16) | (buf[7] << 24));
                                R[2] = (uint)(buf[8] | (buf[9] << 8) | (buf[10] << 16) | (buf[11] << 24));
                                break;

                            default:
                                Console.WriteLine($"Bad block type {BlockType}");
                                return Error = Error.MSPACK_ERR_DECRUNCH;
                        }
                    }

                    // Decode more of the block:
                    // run = min(what's available, what's needed)
                    this_run = BlockRemaining;
                    if (this_run > bytes_todo)
                        this_run = bytes_todo;

                    // Assume we decode exactly this_run bytes, for now
                    bytes_todo     -= this_run;
                    BlockRemaining -= this_run;

                    // Decode at least this_run bytes
                    switch (BlockType)
                    {
                        case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                        case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                            while (this_run > 0)
                            {
                                main_element = (int)READ_HUFFSYM_MSB(MAINTREE_table, MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                                if (main_element < LZX_NUM_CHARS)
                                {
                                    // Literal: 0 to LZX_NUM_CHARS-1
                                    Window[WindowPosition++] = (byte)main_element;
                                    this_run--;
                                }
                                else
                                {
                                    // Match: LZX_NUM_CHARS ((slot<<3) | length_header (3 bits))
                                    main_element -= LZX_NUM_CHARS;

                                    // Get match length
                                    match_length = main_element & LZX_NUM_PRIMARY_LENGTHS;
                                    if (match_length == LZX_NUM_PRIMARY_LENGTHS)
                                    {
                                        if (LENGTH_empty)
                                        {
                                            Console.WriteLine("LENGTH symbol needed but tree is empty");
                                            return Error = Error.MSPACK_ERR_DECRUNCH;
                                        }

                                        length_footer = (int)READ_HUFFSYM_MSB(LENGTH_table, LENGTH_len, LZX_LENGTH_TABLEBITS, LZX_LENGTH_MAXSYMBOLS);
                                        match_length += length_footer;
                                    }

                                    match_length += LZX_MIN_MATCH;

                                    // Get match offset
                                    switch ((match_offset = (uint)(main_element >> 3)))
                                    {
                                        case 0:
                                            match_offset = R[0];
                                            break;

                                        case 1:
                                            match_offset = R[1];
                                            R[1] = R[0];
                                            R[0] = match_offset;
                                            break;

                                        case 2:
                                            match_offset = R[2];
                                            R[2] = R[0];
                                            R[0] = match_offset;
                                            break;

                                        default:
                                            if (BlockType == LZXBlockType.LZX_BLOCKTYPE_VERBATIM)
                                            {
                                                if (match_offset == 3)
                                                {
                                                    match_offset = 1;
                                                }
                                                else
                                                {
                                                    extra = ExtraBits(match_offset);
                                                    verbatim_bits = (int)READ_BITS_MSB(extra);
                                                    match_offset = (uint)(PositionBase(match_offset) - 2 + verbatim_bits);
                                                }
                                            }
                                            else // LZX_BLOCKTYPE_ALIGNED 
                                            {
                                                extra = ExtraBits(match_offset);
                                                match_offset = (uint)(PositionBase(match_offset) - 2);
                                                if (extra > 3) // >3: verbatim and aligned bits
                                                {
                                                    extra -= 3;
                                                    verbatim_bits = (int)READ_BITS_MSB(extra);
                                                    match_offset += (uint)(verbatim_bits << 3);
                                                    aligned_bits = (int)READ_HUFFSYM_MSB(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS);
                                                    match_offset += (uint)aligned_bits;
                                                }
                                                else if (extra == 3) // 3: aligned bits only
                                                {
                                                    aligned_bits = (int)READ_HUFFSYM_MSB(ALIGNED_table, ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS);
                                                    match_offset += (uint)aligned_bits;
                                                }
                                                else if (extra > 0) // 1-2: verbatim bits only
                                                {
                                                    verbatim_bits = (int)READ_BITS_MSB(extra);
                                                    match_offset += (uint)verbatim_bits;
                                                }
                                                else // 0: not defined in LZX specification!
                                                {
                                                    match_offset = 1;
                                                }
                                            }

                                            // Update repeated offset LRU queue
                                            R[2] = R[1];
                                            R[1] = R[0];
                                            R[0] = match_offset;
                                            break;
                                    }

                                    // LZX DELTA uses max match length to signal even longer match
                                    if (match_length == LZX_MAX_MATCH && IsDelta)
                                    {
                                        int extra_len = 0;
                                        ENSURE_BITS(3); // 4 entry huffman tree
                                        if (PEEK_BITS_MSB(1) == 0)
                                        {
                                            REMOVE_BITS_MSB(1); // '0' -> 8 extra length bits
                                            extra_len = (int)READ_BITS_MSB(8);
                                        }
                                        else if (PEEK_BITS_MSB(2) == 2)
                                        {
                                            REMOVE_BITS_MSB(2); // '10' -> 10 extra length bits + 0x100
                                            extra_len = (int)READ_BITS_MSB(10);
                                            extra_len += 0x100;
                                        }
                                        else if (PEEK_BITS_MSB(3) == 6)
                                        {
                                            REMOVE_BITS_MSB(3); // '110' -> 12 extra length bits + 0x500
                                            extra_len = (int)READ_BITS_MSB(12);
                                            extra_len += 0x500;
                                        }
                                        else
                                        {
                                            REMOVE_BITS_MSB(3); // '111' -> 15 extra length bits
                                            extra_len = (int)READ_BITS_MSB(15);
                                        }

                                        match_length += extra_len;
                                    }

                                    if ((WindowPosition + match_length) > WindowSize)
                                    {
                                        Console.WriteLine("Match ran over window wrap");
                                        return Error = Error.MSPACK_ERR_DECRUNCH;
                                    }

                                    // TODO: This falls out of sync after (91,3)
                                    // - Official program goes to (94, 3)
                                    // - Ours goes to (95, 3)

                                    // Copy match
                                    rundest = WindowPosition;
                                    i = match_length;

                                    // Does match offset wrap the window?
                                    if (match_offset > WindowPosition)
                                    {
                                        if (match_offset > Offset && (match_offset - WindowPosition) > ReferenceDataSize)
                                        {
                                            Console.WriteLine("Match offset beyond LZX stream");
                                            return Error = Error.MSPACK_ERR_DECRUNCH;
                                        }

                                        // j = length from match offset to end of window
                                        j = (int)match_offset - WindowPosition;
                                        if (j > (int)WindowSize)
                                        {
                                            Console.WriteLine("Match offset beyond window boundaries");
                                            return Error = Error.MSPACK_ERR_DECRUNCH;
                                        }

                                        runsrc = (int)(WindowSize - j);
                                        if (j < i)
                                        {
                                            // If match goes over the window edge, do two copy runs
                                            i -= j;
                                            while (j-- > 0)
                                            {
                                                Window[rundest++] = Window[runsrc++];
                                            }

                                            runsrc = 0;
                                        }

                                        while (i-- > 0)
                                        {
                                            Window[rundest++] = Window[runsrc++];
                                        }
                                    }
                                    else
                                    {
                                        runsrc = rundest - (int)match_offset;
                                        while (i-- > 0)
                                        {
                                            Window[rundest++] = Window[runsrc++];
                                        }
                                    }

                                    this_run       -= match_length;
                                    WindowPosition += match_length;
                                }
                            }

                            break;

                        case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                            // As this_run is limited not to wrap a frame, this also means it
                            // won't wrap the window (as the window is a multiple of 32k)
                            rundest = WindowPosition;
                            WindowPosition += this_run;
                            while (this_run > 0)
                            {
                                if ((i = InputEnd - InputPointer) == 0)
                                {
                                    READ_IF_NEEDED();
                                }
                                else
                                {
                                    if (i > this_run)
                                        i = this_run;

                                    Array.Copy(InputBuffer, InputPointer, Window, rundest, i);
                                    rundest      += 1;
                                    InputPointer += i;
                                    this_run     -= i;
                                }
                            }

                            break;

                        default:
                            return Error = Error.MSPACK_ERR_DECRUNCH; // Might as well
                    }

                    // Did the final match overrun our desired this_run length?
                    if (this_run < 0)
                    {
                        if ((uint)(-this_run) > BlockRemaining)
                        {
                            Console.Write($"Overrun went past end of block by {-this_run} ({BlockRemaining} remaining)");
                            return Error = Error.MSPACK_ERR_DECRUNCH;
                        }

                        BlockRemaining -= -this_run;
                    }
                }

                // Streams don't extend over frame boundaries
                if ((WindowPosition - FramePosition) != frame_size)
                {
                    Console.WriteLine($"Decode beyond output frame limits {WindowPosition - FramePosition} != {frame_size}");
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
                    Console.Write($"{OutputEnd - OutputPointer} avail bytes, new {frame_size} frame");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Does this intel block _really_ need decoding?
                if (IntelStarted && IntelFileSize != 0 && Frame < 32768 && frame_size > 10)
                {
                    int data     = 0;
                    int dataend  = (int)frame_size - 10;
                    int curpos   = (int)Offset;
                    int filesize = IntelFileSize;
                    int abs_off, rel_off;

                    // Copy E8 block to the e8 buffer and tweak if needed
                    WriteFromE8 = true;
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
                else
                {
                    WriteFromE8 = false;
                    OutputPointer = (int)FramePosition;
                }

                OutputEnd = (int)frame_size;

                // Write a frame
                i = (int)(out_bytes < frame_size ? out_bytes : frame_size);
                if (System.Write(OutputFileHandle, WriteFromE8 ? E8Buffer : Window, OutputPointer, i) != i)
                    return Error = Error.MSPACK_ERR_WRITE;

                OutputPointer += i;
                Offset        += i;
                out_bytes     -= i;

                // Advance frame start position
                FramePosition += (uint)frame_size;
                Frame++;

                // Wrap window / frame position pointers
                if (WindowPosition == WindowSize)
                    WindowPosition = 0;
                if (FramePosition == WindowSize)
                    FramePosition = 0;
            }

            if (out_bytes > 0)
            {
                Console.WriteLine($"{out_bytes} bytes left to output");
                return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// States how many bits of offset-from-base data is needed.
        /// </summary>
        private int ExtraBits(uint offset)
        {
            if (offset < 4)
                return 0;
            else if (offset >= 4 && offset < 36)
                return (int)Math.Floor((double)(offset / 2)) - 1;
            else // offset >= 36
                return 17;
        }

        /// <summary>
        /// An index to the position slot bases
        /// </summary>
        private long PositionBase(uint offset)
        {
            if (offset < 0 || offset >= 290)
                return 0;

            return LZXPositionBase[offset];

            // TODO: Replace naieve recursive implementation
            if (offset == 0)
                return 0;
            else
                return PositionBase(offset - 1) + (1 << ExtraBits(offset - 1));
        }

        /// <summary>
        /// Reset the internal state
        /// </summary>
        private void ResetState()
        {
            R[0]           = 1;
            R[1]           = 1;
            R[2]           = 1;
            HeaderRead     = 0;
            BlockRemaining = 0;
            BlockType = LZXBlockType.LZX_BLOCKTYPE_INVALID0;
            WriteFromE8 = false;

            // Initialise tables to 0 (because deltas will be applied to them)
            for (int i = 0; i < LZX_MAINTREE_MAXSYMBOLS; i++)
            {
                MAINTREE_len[i] = 0;
            }

            for (int i = 0; i < LZX_LENGTH_MAXSYMBOLS; i++)
            {
                LENGTH_len[i]   = 0;
            }
        }

        #endregion
    }
}
