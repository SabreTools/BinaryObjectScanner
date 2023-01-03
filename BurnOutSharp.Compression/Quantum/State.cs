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
            0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 384, 512, 768,
            1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576, 32768, 49152,
            65536, 98304, 131072, 196608, 262144, 393216, 524288, 786432, 1048576, 1572864
        };

        /// <summary>
        /// How many bits of offset-from-base data is needed
        /// </summary>
        public byte[] ExtraBits = new byte[42]
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10,
            11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 18, 18, 19, 19
        };

        /// <summary>
        /// An index to the position slot bases [Selector 6]
        /// </summary>
        public byte[] LengthBases = new byte[27]
        {
            0, 1, 2, 3, 4, 5, 6, 8, 10, 12, 14, 18, 22, 26,
            30, 38, 46, 54, 62, 78, 94, 110, 126, 158, 190, 222, 254
        };

        /// <summary>
        /// How many bits of offset-from-base data is needed [Selector 6]
        /// </summary>
        public byte[] LengthExtraBits = new byte[27]
        {
            0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
            3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
        };

        #endregion
    }
}