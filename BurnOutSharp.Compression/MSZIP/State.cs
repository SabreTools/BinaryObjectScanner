using static BurnOutSharp.Models.Compression.MSZIP.Constants;

namespace BurnOutSharp.Compression.MSZIP
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    public class State
    {
        /// <summary>
        /// Current offset within the window
        /// </summary>
        public uint WindowPosition;

        /// <summary>
        /// Bit buffer
        /// </summary>
        public uint BitBuffer;

        /// <summary>
        /// Bits in bit buffer
        /// </summary>
        public uint BitCount;

        /// <summary>
        /// Literal/length and distance code lengths
        /// </summary>
        public uint[] Lengths = new uint[288 + 32];

        /// <summary>
        /// Bit length count table
        /// </summary>
        public uint[] Counts = new uint[ZIPBMAX + 1];

        /// <summary>
        /// Memory for l[-1..ZIPBMAX-1]
        /// </summary>
        public int[] LengthMemory = new int[ZIPBMAX + 1];

        /// <summary>
        /// Table stack
        /// </summary>
        public HuffmanNode[] TableStack = new HuffmanNode[ZIPBMAX];

        /// <summary>
        /// Values in order of bit length
        /// </summary>
        public uint[] Values = new uint[ZIPN_MAX];

        /// <summary>
        /// Bit offsets, then code stack
        /// </summary>
        public uint[] BitOffsets = new uint[ZIPBMAX + 1];

        /// <remarks>byte*</remarks>
        public int InputPosition;
    }
}