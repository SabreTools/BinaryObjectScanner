using BurnOutSharp.Models.Compression.Quantum;

namespace BurnOutSharp.Compression.Quantum
{
    /// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/qtmd.c"/>
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    public class State
    {
        /// <summary>
        /// The actual decoding window
        /// </summary>
        public byte[] Window;

        /// <summary>
        /// Window size (1Kb through 2Mb)
        /// </summary>
        public uint WindowSize;

        /// <summary>
        /// Window size when it was first allocated
        /// </summary>
        public uint ActualSize;

        /// <summary>
        /// Current offset within the window
        /// </summary>
        public uint WindowPosition;

        #region Models

        /// <summary>
        /// Symbol table for selector model
        /// </summary>
        public ModelSymbol[] SelectorModelSymbols = new ModelSymbol[7 + 1];

        /// <summary>
        /// Model for selector values
        /// </summary>
        public Model SelectorModel;

        /// <summary>
        /// Model for Selector 0
        /// </summary>
        public Model Model0;

        /// <summary>
        /// Model for Selector 1
        /// </summary>
        public Model Model1;

        /// <summary>
        /// Model for Selector 2
        /// </summary>
        public Model Model2;

        /// <summary>
        /// Model for Selector 3
        /// </summary>
        public Model Model3;

        /// <summary>
        /// Model for Selector 4
        /// </summary>
        public Model Model4;

        /// <summary>
        /// Model for Selector 5
        /// </summary>
        public Model Model5;

        /// <summary>
        /// Model for Selector 6 Position
        /// </summary>
        public Model Model6Position;

        /// <summary>
        /// Model for Selector 6 Length
        /// </summary>
        public Model Model6Length;

        #endregion

        #region Symbol Tables

        /// <summary>
        /// Symbol table for Selector 0
        /// </summary>
        public ModelSymbol[] Model0Symbols = new ModelSymbol[0x40 + 1];

        /// <summary>
        /// Symbol table for Selector 1
        /// </summary>
        public ModelSymbol[] Model1Symbols = new ModelSymbol[0x40 + 1];

        /// <summary>
        /// Symbol table for Selector 2
        /// </summary>
        public ModelSymbol[] Model2Symbols = new ModelSymbol[0x40 + 1];

        /// <summary>
        /// Symbol table for Selector 3
        /// </summary>
        public ModelSymbol[] Model3Symbols = new ModelSymbol[0x40 + 1];

        /// <summary>
        /// Symbol table for Selector 4
        /// </summary>
        public ModelSymbol[] Model4Symbols = new ModelSymbol[0x18 + 1];

        /// <summary>
        /// Symbol table for Selector 5
        /// </summary>
        public ModelSymbol[] Model5Symbols = new ModelSymbol[0x24 + 1];

        /// <summary>
        /// Symbol table for Selector 6 Position
        /// </summary>
        public ModelSymbol[] Model6PositionSymbols = new ModelSymbol[0x2a + 1];

        /// <summary>
        /// Symbol table for Selector 6 Length
        /// </summary>
        public ModelSymbol[] Model6LengthSymbols = new ModelSymbol[0x1b + 1];

        #endregion

        #region Decompression Tables

        /// <summary>
        /// An index to the position slot bases
        /// </summary>
        public uint[] PositionSlotBases = new uint[42]
        {
            0x00000, 0x00001, 0x00002, 0x00003, 0x00004, 0x00006, 0x00008, 0x0000c,
            0x00010, 0x00018, 0x00020, 0x00030, 0x00040, 0x00060, 0x00080, 0x000c0,
            0x00100, 0x00180, 0x00200, 0x00300, 0x00400, 0x00600, 0x00800, 0x00c00,
            0x01000, 0x01800, 0x02000, 0x03000, 0x04000, 0x06000, 0x08000, 0x0c000,
            0x10000, 0x18000, 0x20000, 0x30000, 0x40000, 0x60000, 0x80000, 0xc0000,
            0x100000, 0x180000
        };

        /// <summary>
        /// How many bits of offset-from-base data is needed
        /// </summary>
        public byte[] ExtraBits = new byte[42]
        {
            0,  0,  0,  0,  1,  1,  2,  2,
            3,  3,  4,  4,  5,  5,  6,  6,
            7,  7,  8,  8,  9,  9, 10, 10,
            11, 11, 12, 12, 13, 13, 14, 14,
            15, 15, 16, 16, 17, 17, 18, 18,
            19, 19
        };

        /// <summary>
        /// An index to the position slot bases [Selector 6]
        /// </summary>
        public byte[] LengthBases = new byte[27]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x08,
            0x0a, 0x0c, 0x0e, 0x12, 0x16, 0x1a, 0x1e, 0x26,
            0x2e, 0x36, 0x3e, 0x4e, 0x5e, 0x6e, 0x7e, 0x9e,
            0xbe, 0xde, 0xfe
        };

        /// <summary>
        /// How many bits of offset-from-base data is needed [Selector 6]
        /// </summary>
        public byte[] LengthExtraBits = new byte[27]
        {
            0,  0,  0,  0,  0,  0,  1,  1,
            1,  1,  2,  2,  2,  2,  3,  3,
            3,  3,  4,  4,  4,  4,  5,  5,
            5,  5,  0
        };

        #endregion

        #region Decompression State

        /// <summary>
        /// Bit buffer to persist between runs
        /// </summary>
        public uint BitBuffer = 0;

        /// <summary>
        /// Bits remaining to persist between runs
        /// </summary>
        public int BitsLeft = 0;

        #endregion
    }
}