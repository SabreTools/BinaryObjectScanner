using static BurnOutSharp.Models.Compression.MSZIP.Constants;

namespace BurnOutSharp.Compression.MSZIP
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    public unsafe class State
    {
        /// <summary>
        /// Current offset within the window
        /// </summary>
        public uint window_posn;

        /// <summary>
        /// Bit buffer
        /// </summary>
        public uint bb;

        /// <summary>
        /// Bits in bit buffer
        /// </summary>
        public uint bk;

        /// <summary>
        /// Literal/length and distance code lengths
        /// </summary>
        public uint[] ll = new uint[288 + 32];

        /// <summary>
        /// Bit length count table
        /// </summary>
        public uint[] c = new uint[ZIPBMAX + 1];

        /// <summary>
        /// Memory for l[-1..ZIPBMAX-1]
        /// </summary>
        public int[] lx = new int[ZIPBMAX + 1];

        /// <summary>
        /// Table stack
        /// </summary>
        public HuffmanNode*[] u = new HuffmanNode*[ZIPBMAX];

        /// <summary>
        /// Values in order of bit length
        /// </summary>
        public uint[] v = new uint[ZIPN_MAX];

        /// <summary>
        /// Bit offsets, then code stack
        /// </summary>
        public uint[] x = new uint[ZIPBMAX + 1];

        /// <remarks>byte*</remarks>
        public byte* inpos;
    }
}