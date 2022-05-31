/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    internal static class Constants
    {
        #region readbits.h

        /// <summary>
        /// Bit width of a UInt32 bit buffer
        /// </summary>
        public const int BITBUF_WIDTH = 4 * CHAR_BIT;

        /// <summary>
        /// Number of bits in a character
        /// </summary>
        internal const int CHAR_BIT = 8;

        // lsb_bit_mask[n] = (1 << n) - 1
        internal static readonly ushort[] LSBBitMask = new ushort[17]
        {
            0x0000, 0x0001, 0x0003, 0x0007, 0x000f, 0x001f, 0x003f, 0x007f, 0x00ff,
            0x01ff, 0x03ff, 0x07ff, 0x0fff, 0x1fff, 0x3fff, 0x7fff, 0xffff
        };

        #endregion

        #region readhuff.h

        /// <summary>
        /// Maximum bits in a Huffman code
        /// </summary>
        public const int HUFF_MAXBITS = 16;

        #endregion

        #region LZSS

        /// <summary>
        /// Size of an LZSS window
        /// </summary>
        public const int LZSS_WINDOW_SIZE = 4096;

        /// <summary>
        /// LZSS window fill byte
        /// </summary>
        public const byte LZSS_WINDOW_FILL = 0x20;

        #endregion

        #region LZX

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

        #region LZX static data tables

        /// <summary>
        /// LZX uses 'position slots' to represent match offsets.  For every match,
        /// a small 'position slot' number and a small offset from that slot are
        /// encoded instead of one large offset.
        /// 
        /// The number of slots is decided by how many are needed to encode the
        /// largest offset for a given window size. This is easy when the gap between
        /// slots is less than 128Kb, it's a linear relationship. But when extra_bits
        /// reaches its limit of 17 (because LZX can only ensure reading 17 bits of
        /// data at a time), we can only jump 128Kb at a time and have to start
        /// using more and more position slots as each window size doubles.
        /// </summary>
        public static readonly uint[] LZXPositionSlots = new uint[11]
        {
            30, 32, 34, 36, 38, 42, 50, 66, 98, 162, 290
        };

        /// <summary>
        /// An index to the position slot bases
        /// </summary>
        /// <remarks>
        /// Calculated as follows:
        /// LZXPositionBase[0] = 0
        /// LZXPositionBase[i] = LZXPositionBase[i - 1] + (1 << ExtraBits(i - 1))
        /// </remarks>
        public static readonly uint[] LZXPositionBase = new uint[290]
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

        #endregion

        #endregion

        #region MSZIP

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
        public static readonly ushort[] LiteralLengths = new ushort[29]
        {
          3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27,
          31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258
        };

        /// <summary>
        /// Match offsets for distance codes 0 .. 29
        /// </summary>
        public static readonly ushort[] DistanceOffsets = new ushort[30]
        {
          1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385,
          513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577
        };

        /// <summary>
        /// Extra bits required for literal codes 257.. 285
        /// </summary>
        public static readonly byte[] LiteralExtraBits = new byte[29]
        {
          0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2,
          2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
        };

        /// <summary>
        /// Extra bits required for distance codes 0 .. 29
        /// </summary>
        public static readonly byte[] DistanceExtraBits = new byte[30]
        {
          0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6,
          6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
        };

        /// <summary>
        /// The order of the bit length Huffman code lengths
        /// </summary>
        public static readonly byte[] BitLengthOrder = new byte[19]
        {
          16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
        };

        #endregion

        #region QTM

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

        public static readonly uint[] QTMPositionBase = new uint[42]
        {
          0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 384, 512, 768,
          1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576, 32768, 49152,
          65536, 98304, 131072, 196608, 262144, 393216, 524288, 786432, 1048576, 1572864
        };

        public static readonly byte[] QTMExtraBits = new byte[42]
        {
          0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10,
          11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 18, 18, 19, 19
        };

        public static readonly byte[] QTMLengthBase = new byte[27]
        {
          0, 1, 2, 3, 4, 5, 6, 8, 10, 12, 14, 18, 22, 26,
          30, 38, 46, 54, 62, 78, 94, 110, 126, 158, 190, 222, 254
        };

        public static readonly byte[] QTMLengthExtra = new byte[27]
        {
          0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
          3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
        };

        #endregion
    }
}
