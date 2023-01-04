using static BurnOutSharp.Models.Compression.LZX.Constants;

namespace BurnOutSharp.Compression.LZX
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
    }
}