/* This file is part of libmspack.
 * (C) 2003-2013 Stuart Caie.
 *
 * The LZX method was created by Jonathan Forbes and Tomi Poutanen, adapted
 * by Microsoft Corporation.
 *
 * libmspack is free software { get; set; } you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    public class LZXDStream : CompressionStream
    {
        /// <summary>
        /// Number of bytes actually output
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Overall decompressed length of stream
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Decoding window
        /// </summary>
        public byte[] Window { get; set; }

        /// <summary>
        /// Window size
        /// </summary>
        public uint WindowSize { get; set; }

        /// <summary>
        /// LZX DELTA reference data size
        /// </summary>
        public uint ReferenceDataSize { get; set; }

        /// <summary>
        /// Number of match_offset entries in table
        /// </summary>
        public uint NumOffsets { get; set; }

        /// <summary>
        /// Decompression offset within window
        /// </summary>
        public uint WindowPosition { get; set; }

        /// <summary>
        /// Current frame offset within in window
        /// </summary>
        public uint FramePosition { get; set; }

        /// <summary>
        /// The number of 32kb frames processed
        /// </summary>
        public uint Frame { get; set; }

        /// <summary>
        /// Which frame do we reset the compressor?
        /// </summary>
        public uint ResetInterval { get; set; }

        /// <summary>
        /// For the LRU offset system
        /// </summary>
        public uint R0 { get; set; }

        /// <summary>
        /// For the LRU offset system
        /// </summary>
        public uint R1 { get; set; }

        /// <summary>
        /// For the LRU offset system
        /// </summary>
        public uint R2 { get; set; }

        /// <summary>
        /// Uncompressed length of this LZX block
        /// </summary>
        public uint BlockLength { get; set; }

        /// <summary>
        /// Uncompressed bytes still left to decode
        /// </summary>
        public uint BlockRemaining { get; set; }

        /// <summary>
        /// Magic header value used for transform
        /// </summary>
        public int IntelFileSize { get; set; }

        /// <summary>
        /// Has intel E8 decoding started?
        /// </summary>
        public bool IntelStarted { get; set; }

        /// <summary>
        /// Type of the current block
        /// </summary>
        public LZXBlockType BlockType { get; set; }

        /// <summary>
        /// Have we started decoding at all yet?
        /// </summary>
        public byte HeaderRead { get; set; }

        /// <summary>
        /// Does stream follow LZX DELTA spec?
        /// </summary>
        public bool IsDelta { get; set; }

        #region Huffman code lengths

        public byte[] PRETREE_len { get; set; } = new byte[LZX.LZX_PRETREE_MAXSYMBOLS + LZX.LZX_LENTABLE_SAFETY];
        public byte[] MAINTREE_len { get; set; } = new byte[LZX.LZX_MAINTREE_MAXSYMBOLS + LZX.LZX_LENTABLE_SAFETY];
        public byte[] LENGTH_len { get; set; } = new byte[LZX.LZX_LENGTH_MAXSYMBOLS + LZX.LZX_LENTABLE_SAFETY];
        public byte[] ALIGNED_len { get; set; } = new byte[LZX.LZX_ALIGNED_MAXSYMBOLS + LZX.LZX_LENTABLE_SAFETY];

        #endregion

        #region Huffman decoding tables

        public ushort[] PRETREE_table { get; set; } = new ushort[(1 << LZX.LZX_PRETREE_TABLEBITS) + (LZX.LZX_PRETREE_MAXSYMBOLS * 2)];
        public ushort[] MAINTREE_table { get; set; } = new ushort[(1 << LZX.LZX_MAINTREE_TABLEBITS) + (LZX.LZX_MAINTREE_MAXSYMBOLS * 2)];
        public ushort[] LENGTH_table { get; set; } = new ushort[(1 << LZX.LZX_LENGTH_TABLEBITS) + (LZX.LZX_LENGTH_MAXSYMBOLS * 2)];
        public ushort[] ALIGNED_table { get; set; } = new ushort[(1 << LZX.LZX_ALIGNED_TABLEBITS) + (LZX.LZX_ALIGNED_MAXSYMBOLS * 2)];

        #endregion
        
        public byte LENGTH_empty { get; set; }

        // This is used purely for doing the intel E8 transform
        public byte[] e8_buf { get; set; } = new byte[LZX.LZX_FRAME_SIZE];
    }
}
