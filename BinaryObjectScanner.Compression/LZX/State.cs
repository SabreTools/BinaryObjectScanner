using static SabreTools.Models.Compression.LZX.Constants;

namespace BinaryObjectScanner.Compression.LZX
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    public class State
    {
        /// <summary>
        /// the actual decoding window
        /// </summary>
        public byte[] window;

        /// <summary>
        /// window size (32Kb through 2Mb)
        /// </summary>
        public uint window_size;

        /// <summary>
        /// window size when it was first allocated
        /// </summary>
        public uint actual_size;

        /// <summary>
        /// current offset within the window
        /// </summary>
        public uint window_posn;

        /// <summary>
        /// for the LRU offset system
        /// </summary>
        public uint R0, R1, R2;

        /// <summary>
        /// number of main tree elements
        /// </summary>
        public ushort main_elements;

        /// <summary>
        /// have we started decoding at all yet?
        /// </summary>
        public int header_read;

        /// <summary>
        /// type of this block
        /// </summary>
        public ushort block_type;

        /// <summary>
        /// uncompressed length of this block
        /// </summary>
        public uint block_length;

        /// <summary>
        /// uncompressed bytes still left to decode
        /// </summary>
        public uint block_remaining;

        /// <summary>
        /// the number of CFDATA blocks processed
        /// </summary>
        public uint frames_read;

        /// <summary>
        /// magic header value used for transform
        /// </summary>
        public int intel_filesize;

        /// <summary>
        /// current offset in transform space
        /// </summary>
        public int intel_curpos;

        /// <summary>
        /// have we seen any translatable data yet?
        /// </summary>
        public int intel_started;

        public ushort[] tblPRETREE_table = new ushort[(1 << LZX_PRETREE_TABLEBITS) + (LZX_PRETREE_MAXSYMBOLS << 1)];
        public byte[] tblPRETREE_len = new byte[LZX_PRETREE_MAXSYMBOLS + LZX_LENTABLE_SAFETY];

        public ushort[] tblMAINTREE_table = new ushort[(1 << LZX_MAINTREE_TABLEBITS) + (LZX_MAINTREE_MAXSYMBOLS << 1)];
        public byte[] tblMAINTREE_len = new byte[LZX_MAINTREE_MAXSYMBOLS + LZX_LENTABLE_SAFETY];

        public ushort[] tblLENGTH_table = new ushort[(1 << LZX_LENGTH_TABLEBITS) + (LZX_LENGTH_MAXSYMBOLS << 1)];
        public byte[] tblLENGTH_len = new byte[LZX_LENGTH_MAXSYMBOLS + LZX_LENTABLE_SAFETY];

        public ushort[] tblALIGNED_table = new ushort[(1 << LZX_ALIGNED_TABLEBITS) + (LZX_ALIGNED_MAXSYMBOLS << 1)];
        public byte[] tblALIGNED_len = new byte[LZX_ALIGNED_MAXSYMBOLS + LZX_LENTABLE_SAFETY];

        #region Decompression Tables

        /// <summary>
        /// An index to the position slot bases
        /// </summary>
        public uint[] PositionSlotBases = new uint[]
        {
            0,       1,       2,       3,       4,       6,       8,      12,
            16,      24,      32,      48,      64,      96,     128,     192,
            256,     384,     512,     768,    1024,    1536,    2048,    3072,
            4096,    6144,    8192,   12288,   16384,   24576,   32768,   49152,
            65536,   98304,  131072,  196608,  262144,  393216,  524288,  655360,
            786432,  917504, 1048576, 1179648, 1310720, 1441792, 1572864, 1703936,
            1835008, 1966080, 2097152
        };

        /// <summary>
        /// How many bits of offset-from-base data is needed
        /// </summary>
        public byte[] ExtraBits = new byte[]
        {
            0,  0,  0,  0,  1,  1,  2,  2,  3,  3,  4,  4,  5,  5,  6,  6,
            7,  7,  8,  8,  9,  9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14,
            15, 15, 16, 16, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17,
            17, 17, 17
        };

        #endregion
    }
}