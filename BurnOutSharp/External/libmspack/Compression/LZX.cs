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

namespace LibMSPackSharp.Compression
{
    public class LZX
    {
        #region LZX compression / decompression definitions

        // Some constants defined by the LZX specification
        public const int LZX_MIN_MATCH = 2;
        public const int LZX_MAX_MATCH = 257;
        public const int LZX_NUM_CHARS = 256;

        public const int LZX_PRETREE_NUM_ELEMENTS = 20;
        public const int LZX_ALIGNED_NUM_ELEMENTS = 8;   // Aligned offset tree #elements
        public const int LZX_NUM_PRIMARY_LENGTHS = 7;   // This one missing from spec!
        public const int LZX_NUM_SECONDARY_LENGTHS = 249; // Length tree #elements

        // LZX huffman defines: tweak tablebits as desired

        public const int LZX_PRETREE_MAXSYMBOLS = LZX_PRETREE_NUM_ELEMENTS;
        public const byte LZX_PRETREE_TABLEBITS = 6;
        public const int LZX_MAINTREE_MAXSYMBOLS = LZX_NUM_CHARS + 290 * 8;
        public const byte LZX_MAINTREE_TABLEBITS = 12;
        public const int LZX_LENGTH_MAXSYMBOLS = LZX_NUM_SECONDARY_LENGTHS + 1;
        public const byte LZX_LENGTH_TABLEBITS = 12;
        public const int LZX_ALIGNED_MAXSYMBOLS = LZX_ALIGNED_NUM_ELEMENTS;
        public const byte LZX_ALIGNED_TABLEBITS = 7;
        public const int LZX_LENTABLE_SAFETY = 64;  // Table decoding overruns are allowed

        public const int LZX_FRAME_SIZE = 32768; // The size of a frame in LZX

        #endregion

        #region LZX static data tables

        /* LZX static data tables:
         *
         * LZX uses 'position slots' to represent match offsets.  For every match,
         * a small 'position slot' number and a small offset from that slot are
         * encoded instead of one large offset.
         *
         * The number of slots is decided by how many are needed to encode the
         * largest offset for a given window size. This is easy when the gap between
         * slots is less than 128Kb, it's a linear relationship. But when extra_bits
         * reaches its limit of 17 (because LZX can only ensure reading 17 bits of
         * data at a time), we can only jump 128Kb at a time and have to start
         * using more and more position slots as each window size doubles.
         *
         * position_base[] is an index to the position slot bases
         *
         * extra_bits[] states how many bits of offset-from-base data is needed.
         *
         * They are calculated as follows:
         * extra_bits[i] = 0 where i < 4
         * extra_bits[i] = floor(i/2)-1 where i >= 4 && i < 36
         * extra_bits[i] = 17 where i >= 36
         * position_base[0] = 0
         * position_base[i] = position_base[i-1] + (1 << extra_bits[i-1])
         */

        private static readonly uint[] position_slots = new uint[11]
        {
            30, 32, 34, 36, 38, 42, 50, 66, 98, 162, 290
        };

        private static readonly byte[] extra_bits = new byte[36]
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
            9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16
        };

        private static readonly uint[] position_base = new uint[290]
        {
            0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 384, 512,
            768, 1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576, 32768,
            49152, 65536, 98304, 131072, 196608, 262144, 393216, 524288, 655360,
            786432, 917504, 1048576, 1179648, 1310720, 1441792, 1572864, 1703936,
            1835008, 1966080, 2097152, 2228224, 2359296, 2490368, 2621440, 2752512,
            2883584, 3014656, 3145728, 3276800, 3407872, 3538944, 3670016, 3801088,
            3932160, 4063232, 4194304, 4325376, 4456448, 4587520, 4718592, 4849664,
            4980736, 5111808, 5242880, 5373952, 5505024, 5636096, 5767168, 5898240,
            6029312, 6160384, 6291456, 6422528, 6553600, 6684672, 6815744, 6946816,
            7077888, 7208960, 7340032, 7471104, 7602176, 7733248, 7864320, 7995392,
            8126464, 8257536, 8388608, 8519680, 8650752, 8781824, 8912896, 9043968,
            9175040, 9306112, 9437184, 9568256, 9699328, 9830400, 9961472, 10092544,
            10223616, 10354688, 10485760, 10616832, 10747904, 10878976, 11010048,
            11141120, 11272192, 11403264, 11534336, 11665408, 11796480, 11927552,
            12058624, 12189696, 12320768, 12451840, 12582912, 12713984, 12845056,
            12976128, 13107200, 13238272, 13369344, 13500416, 13631488, 13762560,
            13893632, 14024704, 14155776, 14286848, 14417920, 14548992, 14680064,
            14811136, 14942208, 15073280, 15204352, 15335424, 15466496, 15597568,
            15728640, 15859712, 15990784, 16121856, 16252928, 16384000, 16515072,
            16646144, 16777216, 16908288, 17039360, 17170432, 17301504, 17432576,
            17563648, 17694720, 17825792, 17956864, 18087936, 18219008, 18350080,
            18481152, 18612224, 18743296, 18874368, 19005440, 19136512, 19267584,
            19398656, 19529728, 19660800, 19791872, 19922944, 20054016, 20185088,
            20316160, 20447232, 20578304, 20709376, 20840448, 20971520, 21102592,
            21233664, 21364736, 21495808, 21626880, 21757952, 21889024, 22020096,
            22151168, 22282240, 22413312, 22544384, 22675456, 22806528, 22937600,
            23068672, 23199744, 23330816, 23461888, 23592960, 23724032, 23855104,
            23986176, 24117248, 24248320, 24379392, 24510464, 24641536, 24772608,
            24903680, 25034752, 25165824, 25296896, 25427968, 25559040, 25690112,
            25821184, 25952256, 26083328, 26214400, 26345472, 26476544, 26607616,
            26738688, 26869760, 27000832, 27131904, 27262976, 27394048, 27525120,
            27656192, 27787264, 27918336, 28049408, 28180480, 28311552, 28442624,
            28573696, 28704768, 28835840, 28966912, 29097984, 29229056, 29360128,
            29491200, 29622272, 29753344, 29884416, 30015488, 30146560, 30277632,
            30408704, 30539776, 30670848, 30801920, 30932992, 31064064, 31195136,
            31326208, 31457280, 31588352, 31719424, 31850496, 31981568, 32112640,
            32243712, 32374784, 32505856, 32636928, 32768000, 32899072, 33030144,
            33161216, 33292288, 33423360
        };

        private static void ResetState(LZXDStream lzx)
        {
            lzx.R0 = 1;
            lzx.R1 = 1;
            lzx.R2 = 1;
            lzx.HeaderRead = 0;
            lzx.BlockRemaining = 0;
            lzx.BlockType = LZXBlockType.LZX_BLOCKTYPE_INVALID0;

            // Initialise tables to 0 (because deltas will be applied to them)
            for (int i = 0; i < LZX_MAINTREE_MAXSYMBOLS; i++)
            {
                lzx.MAINTREE_len[i] = 0;
            }

            for (int i = 0; i < LZX_LENGTH_MAXSYMBOLS; i++)
            {
                lzx.LENGTH_len[i] = 0;
            }
        }

        #endregion

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
            uint window_size = (uint)(1 << window_bits);
            LZXDStream lzx;

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
            lzx = new LZXDStream()
            {
                // Allocate decompression window and input buffer
                Window = new byte[window_size],
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
                NumOffsets = position_slots[window_bits - 15] << 3,
                IsDelta = is_delta,

                // e8_buf
                OutputPointer = 0,
                OutputEnd = 0,
            };

            ResetState(lzx);
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
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
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

        // See description of outputLength in lzxd_init()
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

            int match_length, length_footer, extra, verbatim_bits, bytes_todo;
            int this_run, main_element, aligned_bits, j, warned = 0;
            byte[] window, buf = new byte[12];
            int runsrc, rundest;
            uint frame_size, end_frame, match_offset, window_posn;
            uint R0, R1, R2;

            // Easy answers
            if (lzx == null || (out_bytes < 0))
                return Error.MSPACK_ERR_ARGS;

            if (lzx.Error != Error.MSPACK_ERR_OK)
                return lzx.Error;

            // Flush out any stored-up bytes before we begin
            int i = lzx.OutputEnd - lzx.OutputPointer;
            if (i > out_bytes)
                i = (int)out_bytes;

            if (i != 0)
            {
                try { lzx.System.Write(lzx.OutputFileHandle, lzx.e8_buf, lzx.OutputPointer, i); }
                catch { return lzx.Error = Error.MSPACK_ERR_WRITE; }

                lzx.OutputPointer += i;
                lzx.Offset += i;
                out_bytes -= i;
            }

            if (out_bytes == 0)
                return Error.MSPACK_ERR_OK;

            // Restore local state
            lzx.RESTORE_BITS(out int i_ptr, out int i_end, out uint bit_buffer, out int bits_left);
            window = lzx.Window;
            window_posn = lzx.WindowPosition;
            R0 = lzx.R0;
            R1 = lzx.R1;
            R2 = lzx.R2;

            end_frame = (uint)((lzx.Offset + out_bytes) / LZX_FRAME_SIZE) + 1;

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
                    ResetState(lzx);
                    R0 = lzx.R0;
                    R1 = lzx.R1;
                    R2 = lzx.R2;
                }

                // LZX DELTA format has chunk_size, not present in LZX format
                if (lzx.IsDelta)
                {
                    lzx.ENSURE_BITS(16, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                    lzx.REMOVE_BITS_MSB(16, ref bit_buffer, ref bits_left);
                }

                // Calculate size of frame: all frames are 32k except the final frame
                // which is 32kb or less. this can only be calculated when lzx.Length
                // has been filled in.
                frame_size = LZX_FRAME_SIZE;
                if (lzx.Length != 0 && (lzx.Length - lzx.Offset) < frame_size)
                    frame_size = (uint)(lzx.Length - lzx.Offset);

                // Decode until one more frame is available
                bytes_todo = (int)(lzx.FramePosition + frame_size - window_posn);
                while (bytes_todo > 0)
                {
                    // Initialise new block, if one is needed
                    if (lzx.BlockRemaining == 0)
                    {
                        // Realign if previous block was an odd-sized UNCOMPRESSED block
                        if ((lzx.BlockType == LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED) && (lzx.BlockLength & 1) != 0)
                        {
                            lzx.READ_IF_NEEDED(ref i_ptr, ref i_end);
                            if (lzx.Error != Error.MSPACK_ERR_OK)
                                return lzx.Error;

                            i_ptr++;
                        }

                        // Read block type (3 bits) and block length (24 bits)

                        // THIS IS NOT 3 BECAUSE OF OTHER CODE I FOUND
                        lzx.BlockType = (LZXBlockType)lzx.READ_BITS_MSB(3, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                        // Read header if necessary
                        if (lzx.HeaderRead == 0)
                        {
                            // Read 1 bit. if bit=0, intel_filesize = 0.
                            // if bit=1, read intel filesize (32 bits)
                            j = 0;
                            i = (int)lzx.READ_BITS_MSB(1, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                            if (i != 0)
                            {
                                i = (int)lzx.READ_BITS_MSB(16, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                j = (int)lzx.READ_BITS_MSB(16, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                            }

                            lzx.IntelFileSize = (i << 16) | j;
                            lzx.HeaderRead = 1;
                        }

                        i = (int)lzx.READ_BITS_MSB(16, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                        j = (int)lzx.READ_BITS_MSB(8, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                        lzx.BlockRemaining = lzx.BlockLength = (uint)((i << 8) | j);
                        // Console.WriteLine($"New block t {lzx.BlockType} len {lzx.BlockLength}");

                        // Read individual block headers
                        switch (lzx.BlockType)
                        {
                            case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                                // Read lengths of and build aligned huffman decoding tree
                                for (i = 0; i < 8; i++)
                                {
                                    j = (int)lzx.READ_BITS_MSB(16, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                    lzx.ALIGNED_len[i] = (byte)j;
                                }

                                BUILD_TABLE(lzx, lzx.ALIGNED_table, lzx.ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                // Read lengths of and build main huffman decoding tree
                                READ_LENGTHS(lzx, lzx.MAINTREE_len, 0, 256, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                READ_LENGTHS(lzx, lzx.MAINTREE_len, 256, LZX_NUM_CHARS + lzx.NumOffsets, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                BUILD_TABLE(lzx, lzx.MAINTREE_table, lzx.MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                // If the literal 0xE8 is anywhere in the block...
                                if (lzx.MAINTREE_len[0xE8] != 0)
                                    lzx.IntelStarted = true;

                                // Read lengths of and build lengths huffman decoding tree
                                READ_LENGTHS(lzx, lzx.LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                BUILD_TABLE_MAYBE_EMPTY(lzx);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                break;

                            case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                                // Read lengths of and build main huffman decoding tree
                                READ_LENGTHS(lzx, lzx.MAINTREE_len, 0, 256, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                READ_LENGTHS(lzx, lzx.MAINTREE_len, 256, LZX_NUM_CHARS + lzx.NumOffsets, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                BUILD_TABLE(lzx, lzx.MAINTREE_table, lzx.MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                // If the literal 0xE8 is anywhere in the block...
                                if (lzx.MAINTREE_len[0xE8] != 0)
                                    lzx.IntelStarted = true;

                                // Read lengths of and build lengths huffman decoding tree
                                READ_LENGTHS(lzx, lzx.LENGTH_len, 0, LZX_NUM_SECONDARY_LENGTHS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                BUILD_TABLE_MAYBE_EMPTY(lzx);
                                if (lzx.Error != Error.MSPACK_ERR_OK)
                                    return lzx.Error;

                                break;

                            case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                                // Because we can't assume otherwise
                                lzx.IntelStarted = true;

                                // Read 1-16 (not 0-15) bits to align to bytes
                                if (bits_left == 0)
                                    lzx.ENSURE_BITS(16, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                                bits_left = 0; bit_buffer = 0;

                                // Read 12 bytes of stored R0 / R1 / R2 values
                                for (rundest = 0, i = 0; i < 12; i++)
                                {
                                    lzx.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                    if (lzx.Error != Error.MSPACK_ERR_OK)
                                        return lzx.Error;

                                    buf[rundest++] = lzx.InputBuffer[i_ptr++];
                                }

                                R0 = (uint)(buf[0] | (buf[1] << 8) | (buf[2] << 16) | (buf[3] << 24));
                                R1 = (uint)(buf[4] | (buf[5] << 8) | (buf[6] << 16) | (buf[7] << 24));
                                R2 = (uint)(buf[8] | (buf[9] << 8) | (buf[10] << 16) | (buf[11] << 24));

                                break;

                            default:
                                Console.WriteLine("Bad block type");
                                return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                        }
                    }

                    // Decode more of the block:
                    // run = min(what's available, what's needed)
                    this_run = (int)lzx.BlockRemaining;
                    if (this_run > bytes_todo)
                        this_run = bytes_todo;

                    // Assume we decode exactly this_run bytes, for now
                    bytes_todo -= this_run;
                    lzx.BlockRemaining -= (uint)this_run;

                    // Decode at least this_run bytes
                    switch (lzx.BlockType)
                    {
                        case LZXBlockType.LZX_BLOCKTYPE_ALIGNED:
                        case LZXBlockType.LZX_BLOCKTYPE_VERBATIM:
                            while (this_run > 0)
                            {
                                main_element = (int)lzx.READ_HUFFSYM_MSB(lzx.MAINTREE_table, lzx.MAINTREE_len, LZX_MAINTREE_TABLEBITS, LZX_MAINTREE_MAXSYMBOLS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                if (main_element < LZX_NUM_CHARS)
                                {
                                    // Literal: 0 to LZX_NUM_CHARS-1
                                    window[window_posn++] = (byte)main_element;
                                    this_run--;
                                }
                                else
                                {
                                    // Match: LZX_NUM_CHARS + ((slot<<3) | length_header (3 bits))
                                    main_element -= LZX_NUM_CHARS;

                                    // Get match length
                                    match_length = main_element & LZX_NUM_PRIMARY_LENGTHS;
                                    if (match_length == LZX_NUM_PRIMARY_LENGTHS)
                                    {
                                        if (lzx.LENGTH_empty != 0)
                                        {
                                            Console.WriteLine("LENGTH symbol needed but tree is empty");
                                            return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                                        }

                                        length_footer = (int)lzx.READ_HUFFSYM_MSB(lzx.LENGTH_table, lzx.LENGTH_len, LZX_LENGTH_TABLEBITS, LZX_LENGTH_MAXSYMBOLS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                        match_length += length_footer;
                                    }

                                    match_length += LZX_MIN_MATCH;

                                    // Get match offset
                                    switch ((match_offset = (uint)(main_element >> 3)))
                                    {
                                        case 0:
                                            match_offset = R0;
                                            break;

                                        case 1:
                                            match_offset = R1;
                                            R1 = R0;
                                            R0 = match_offset;
                                            break;

                                        case 2:
                                            match_offset = R2;
                                            R2 = R0;
                                            R0 = match_offset;
                                            break;

                                        default:
                                            if (lzx.BlockType == LZXBlockType.LZX_BLOCKTYPE_VERBATIM)
                                            {
                                                if (match_offset == 3)
                                                {
                                                    match_offset = 1;
                                                }
                                                else
                                                {
                                                    extra = (match_offset >= 36) ? 17 : extra_bits[match_offset];
                                                    verbatim_bits = (int)lzx.READ_BITS_MSB(extra, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                                    match_offset = (uint)(position_base[match_offset] - 2 + verbatim_bits);
                                                }
                                            }

                                            // LZX_BLOCKTYPE_ALIGNED
                                            else
                                            {
                                                extra = (match_offset >= 36) ? 17 : extra_bits[match_offset];
                                                match_offset = position_base[match_offset] - 2;

                                                // >3: verbatim and aligned bits
                                                if (extra > 3)
                                                {
                                                    extra -= 3;
                                                    verbatim_bits = (int)lzx.READ_BITS_MSB(extra, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                                    match_offset += (uint)(verbatim_bits << 3);

                                                    aligned_bits = (int)lzx.READ_HUFFSYM_MSB(lzx.ALIGNED_table, lzx.ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                                    match_offset += (uint)aligned_bits;
                                                }

                                                // 3: aligned bits only
                                                else if (extra == 3)
                                                {
                                                    aligned_bits = (int)lzx.READ_HUFFSYM_MSB(lzx.ALIGNED_table, lzx.ALIGNED_len, LZX_ALIGNED_TABLEBITS, LZX_ALIGNED_MAXSYMBOLS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                                    match_offset += (uint)aligned_bits;
                                                }

                                                // 1-2: verbatim bits only
                                                else if (extra > 0)
                                                {
                                                    verbatim_bits = (int)lzx.READ_BITS_MSB(extra, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                                    match_offset += (uint)verbatim_bits;
                                                }

                                                // 0: not defined in LZX specification!
                                                else
                                                {
                                                    match_offset = 1;
                                                }
                                            }

                                            // Update repeated offset LRU queue
                                            R2 = R1; R1 = R0; R0 = match_offset;
                                            break;
                                    }

                                    // LZX DELTA uses max match length to signal even longer match
                                    if (match_length == LZX_MAX_MATCH && lzx.IsDelta)
                                    {
                                        int extra_len;

                                        // 4 entry huffman tree
                                        lzx.ENSURE_BITS(3, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                                        // '0' . 8 extra length bits
                                        if (lzx.PEEK_BITS_MSB(1, bit_buffer) == 0)
                                        {
                                            lzx.REMOVE_BITS_MSB(1, ref bit_buffer, ref bits_left);
                                            extra_len = (int)lzx.READ_BITS_MSB(8, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                        }

                                        // '10' . 10 extra length bits + 0x100
                                        else if (lzx.PEEK_BITS_MSB(2, bit_buffer) == 2)
                                        {
                                            lzx.REMOVE_BITS_MSB(2, ref bit_buffer, ref bits_left);
                                            extra_len = (int)lzx.READ_BITS_MSB(10, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                            extra_len += 0x100;
                                        }

                                        // '110' . 12 extra length bits + 0x500
                                        else if (lzx.PEEK_BITS_MSB(3, bit_buffer) == 6)
                                        {
                                            lzx.REMOVE_BITS_MSB(3, ref bit_buffer, ref bits_left);
                                            extra_len = (int)lzx.READ_BITS_MSB(12, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                            extra_len += 0x500;
                                        }

                                        // '111' . 15 extra length bits
                                        else
                                        {
                                            lzx.REMOVE_BITS_MSB(3, ref bit_buffer, ref bits_left);
                                            extra_len = (int)lzx.READ_BITS_MSB(15, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                                        }

                                        match_length += extra_len;
                                    }

                                    if ((window_posn + match_length) > lzx.WindowSize)
                                    {
                                        Console.WriteLine("Match ran over window wrap");
                                        return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                                    }

                                    // Copy match
                                    rundest = (int)window_posn;
                                    i = match_length;

                                    // Does match offset wrap the window?
                                    if (match_offset > window_posn)
                                    {
                                        if (match_offset > lzx.Offset && (match_offset - window_posn) > lzx.ReferenceDataSize)
                                        {
                                            Console.WriteLine("Match offset beyond LZX stream");
                                            return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                                        }

                                        // j = length from match offset to end of window
                                        j = (int)(match_offset - window_posn);
                                        if (j > (int)lzx.WindowSize)
                                        {
                                            Console.WriteLine("Match offset beyond window boundaries");
                                            return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                                        }

                                        runsrc = (int)(lzx.WindowSize - j);
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

                                    this_run -= match_length;
                                    window_posn += (uint)match_length;
                                }
                            }

                            break;

                        case LZXBlockType.LZX_BLOCKTYPE_UNCOMPRESSED:
                            // As this_run is limited not to wrap a frame, this also means it
                            // won't wrap the window (as the window is a multiple of 32k)
                            rundest = (int)window_posn;
                            window_posn += (uint)this_run;
                            while (this_run > 0)
                            {
                                if ((i = i_end - i_ptr) == 0)
                                {
                                    lzx.READ_IF_NEEDED(ref i_ptr, ref i_end);
                                    if (lzx.Error != Error.MSPACK_ERR_OK)
                                        return lzx.Error;
                                }
                                else
                                {
                                    if (i > this_run)
                                        i = this_run;

                                    Array.Copy(lzx.InputBuffer, i_ptr, window, rundest, i);

                                    rundest += i;
                                    i_ptr += i;
                                    this_run -= i;
                                }
                            }
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

                        lzx.BlockRemaining -= (uint)-this_run;
                    }
                }

                // Streams don't extend over frame boundaries
                if ((window_posn - lzx.FramePosition) != frame_size)
                {
                    Console.WriteLine($"Decode beyond output frame limits! {window_posn - lzx.FramePosition} != {frame_size}");
                    return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Re-align input bitstream
                if (bits_left > 0)
                    lzx.ENSURE_BITS(16, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                if ((bits_left & 15) != 0)
                    lzx.REMOVE_BITS_MSB(bits_left & 15, ref bit_buffer, ref bits_left);

                // Check that we've used all of the previous frame first
                if (lzx.OutputPointer != lzx.OutputEnd)
                {
                    Console.WriteLine($"{lzx.OutputEnd - lzx.OutputPointer} avail bytes, new {frame_size} frame");
                    return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                }

                // Does this intel block _really_ need decoding?
                if (lzx.IntelStarted && lzx.IntelFileSize != 0 && (lzx.Frame < 32768) && (frame_size > 10))
                {
                    int data = 0;
                    int dataend = (int)(frame_size - 10);
                    int curpos = (int)lzx.Offset;
                    int filesize = lzx.IntelFileSize;
                    int abs_off, rel_off;

                    // Copy e8 block to the e8 buffer and tweak if needed
                    lzx.OutputPointer = data;
                    Array.Copy(lzx.Window, lzx.FramePosition, lzx.e8_buf, data, frame_size);

                    while (data < dataend)
                    {
                        if (lzx.e8_buf[data++] != 0xE8)
                        {
                            curpos++;
                            continue;
                        }

                        abs_off = lzx.e8_buf[data + 0] | (lzx.e8_buf[data + 1] << 8) | (lzx.e8_buf[data + 2] << 16) | (lzx.e8_buf[data + 3] << 24);
                        if ((abs_off >= -curpos) && (abs_off < filesize))
                        {
                            rel_off = (abs_off >= 0) ? abs_off - curpos : abs_off + filesize;
                            lzx.e8_buf[data + 0] = (byte)rel_off;
                            lzx.e8_buf[data + 1] = (byte)(rel_off >> 8);
                            lzx.e8_buf[data + 2] = (byte)(rel_off >> 16);
                            lzx.e8_buf[data + 3] = (byte)(rel_off >> 24);
                        }

                        data += 4;
                        curpos += 5;
                    }

                    lzx.OutputEnd = (int)(lzx.OutputPointer + frame_size);

                    // Write a frame
                    i = (int)((out_bytes < frame_size) ? out_bytes : frame_size);
                    try { lzx.System.Write(lzx.OutputFileHandle, lzx.e8_buf, lzx.OutputPointer, i); }
                    catch { return lzx.Error = Error.MSPACK_ERR_WRITE; }
                }
                else
                {
                    lzx.OutputPointer = (int)lzx.FramePosition;
                    lzx.OutputEnd = (int)(lzx.OutputPointer + frame_size);

                    // Write a frame
                    i = (int)((out_bytes < frame_size) ? out_bytes : frame_size);
                    try { lzx.System.Write(lzx.OutputFileHandle, lzx.Window, lzx.OutputPointer, i); }
                    catch { return lzx.Error = Error.MSPACK_ERR_WRITE; }
                }

                lzx.OutputPointer += i;
                lzx.Offset += i;
                out_bytes -= i;

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
            lzx.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);
            lzx.WindowPosition = window_posn;
            lzx.R0 = R0;
            lzx.R1 = R1;
            lzx.R2 = R2;

            return Error.MSPACK_ERR_OK;
        }

        private static Error BUILD_TABLE(LZXDStream lzx, ushort[] table, byte[] lengths, int tablebits, int maxsymbols)
        {
            if (!CompressionStream.MakeDecodeTableMSB(maxsymbols, tablebits, lengths, table))
            {
                Console.WriteLine($"Failed to build table");
                return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
            }

            return lzx.Error = Error.MSPACK_ERR_OK;
        }

        private static Error BUILD_TABLE_MAYBE_EMPTY(LZXDStream lzx)
        {
            lzx.LENGTH_empty = 0;
            if (!CompressionStream.MakeDecodeTableMSB(LZX_LENGTH_MAXSYMBOLS, LZX_LENGTH_TABLEBITS, lzx.LENGTH_len, lzx.LENGTH_table))
            {
                for (int i = 0; i < LZX_LENGTH_MAXSYMBOLS; i++)
                {
                    if (lzx.LENGTH_len[i] > 0)
                    {
                        Console.WriteLine("Failed to build table");
                        return lzx.Error = Error.MSPACK_ERR_DECRUNCH;
                    }
                }

                // Empty tree - allow it, but don't decode symbols with it
                lzx.LENGTH_empty = 1;
            }

            return lzx.Error = Error.MSPACK_ERR_OK;
        }

        private static Error READ_LENGTHS(LZXDStream lzx, byte[] lengths, uint first, uint last, ref int i_ptr, ref int i_end, ref uint bit_buffer, ref int bits_left)
        {
            lzx.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);

            if (ReadLens(lzx, lengths, first, last) != Error.MSPACK_ERR_OK)
                return lzx.Error;

            lzx.RESTORE_BITS(out i_ptr, out i_end, out bit_buffer, out bits_left);
            return lzx.Error = Error.MSPACK_ERR_OK;
        }

        private static Error ReadLens(LZXDStream lzx, byte[] lens, uint first, uint last)
        {
            uint x, y;
            int z;

            lzx.RESTORE_BITS(out int i_ptr, out int i_end, out uint bit_buffer, out int bits_left);

            // Read lengths for pretree (20 symbols, lengths stored in fixed 4 bits) 
            for (x = 0; x < 20; x++)
            {
                y = (uint)lzx.READ_BITS_MSB(4, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                lzx.PRETREE_len[x] = (byte)y;
            }

            BUILD_TABLE(lzx, lzx.PRETREE_table, lzx.PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS);
            if (lzx.Error != Error.MSPACK_ERR_OK)
                return lzx.Error;

            for (x = first; x < last;)
            {
                z = (int)lzx.READ_HUFFSYM_MSB(lzx.PRETREE_table, lzx.PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                // Code = 17, run of ([read 4 bits]+4) zeros
                if (z == 17)
                {
                    y = (uint)lzx.READ_BITS_MSB(4, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                    y += 4;
                    while (y-- != 0)
                    {
                        lens[x++] = 0;
                    }
                }

                // Code = 18, run of ([read 5 bits]+20) zeros
                else if (z == 18)
                {
                    y = (uint)lzx.READ_BITS_MSB(5, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                    y += 20;
                    while (y-- != 0)
                    {
                        lens[x++] = 0;
                    }
                }

                // Code = 19, run of ([read 1 bit]+4) [read huffman symbol]
                else if (z == 19)
                {
                    y = (uint)lzx.READ_BITS_MSB(1, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);
                    y += 4;
                    z = (int)lzx.READ_HUFFSYM_MSB(lzx.PRETREE_table, lzx.PRETREE_len, LZX_PRETREE_TABLEBITS, LZX_PRETREE_MAXSYMBOLS, ref i_ptr, ref i_end, ref bit_buffer, ref bits_left);

                    z = lens[x] - z;
                    if (z < 0)
                        z += 17;

                    while (y-- != 0)
                    {
                        lens[x++] = (byte)z;
                    }
                }

                // Code = 0 to 16, delta current length entry
                else
                {
                    z = lens[x] - z;
                    if (z < 0)
                        z += 17;

                    lens[x++] = (byte)z;
                }
            }

            lzx.STORE_BITS(i_ptr, i_end, bit_buffer, bits_left);

            return Error.MSPACK_ERR_OK;
        }
    }
}
