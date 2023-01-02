using BurnOutSharp.Models.Compression.Quantum;

namespace BurnOutSharp.Compression.Quantum
{
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
        /// Model for Model 4
        /// </summary>
        public Model Model4;

        /// <summary>
        /// Model for Model 5
        /// </summary>
        public Model Model5;

        /// <summary>
        /// Model for Model 6 Position
        /// </summary>
        public Model Model6Position;

        /// <summary>
        /// Model for Model 6 Length
        /// </summary>
        public Model Model6Length;

        /// <summary>
        /// Model for Model 7
        /// </summary>
        public Model Model7;

        /// <summary>
        /// Model for Model 7, Submodel 00
        /// </summary>
        public Model Model7Submodel00;

        /// <summary>
        /// Model for Model 7, Submodel 40
        /// </summary>
        public Model Model7Submodel40;

        /// <summary>
        /// Model for Model 7, Submodel 80
        /// </summary>
        public Model Model7Submodel80;

        /// <summary>
        /// Model for Model 7, Submodel C0
        /// </summary>
        public Model Model7SubmodelC0;

        #endregion

        #region Symbol Tables

        /// <summary>
        /// Symbol table for Model 4
        /// </summary>
        public ModelSymbol[] Model4Symbols = new ModelSymbol[0x18 + 1];

        /// <summary>
        /// Symbol table for Model 5
        /// </summary>
        public ModelSymbol[] Model5Symbols = new ModelSymbol[0x24 + 1];

        /// <summary>
        /// Symbol table for Model 6 Position
        /// </summary>
        public ModelSymbol[] Model6PositionSymbols = new ModelSymbol[0x2a + 1];

        /// <summary>
        /// Symbol table for Model 6 Length
        /// </summary>
        public ModelSymbol[] Model6LengthSymbols = new ModelSymbol[0x1b + 1];

        /// <summary>
        /// Symbol table for Model 7
        /// </summary>
        public ModelSymbol[] Model7Symbols = new ModelSymbol[7 + 1];

        /// <summary>
        /// Symbol table for Model 7, Submodel 00
        /// </summary>
        public ModelSymbol[] Model7Submodel00Symbols = new ModelSymbol[0x40 + 1];

        /// <summary>
        /// Symbol table for Model 7, Submodel 40
        /// </summary>
        public ModelSymbol[] Model7Submodel40Symbols = new ModelSymbol[0x40 + 1];

        /// <summary>
        /// Symbol table for Model 7, Submodel 80
        /// </summary>
        public ModelSymbol[] Model7Submodel80Symbols = new ModelSymbol[0x40 + 1];

        /// <summary>
        /// Symbol table for Model 7, Submodel C0
        /// </summary>
        public ModelSymbol[] Model7SubmodelC0Symbols = new ModelSymbol[0x40 + 1];

        #endregion
    
        #region Decompression Tables

        /// <summary>
        /// XXXXX
        /// </summary>
        public byte[] q_length_base = new byte[27];

        /// <summary>
        /// XXXXX
        /// </summary>
        public byte[] q_length_extra = new byte[27];

        /// <summary>
        /// XXXXX
        /// </summary>
        public byte[] q_extra_bits = new byte[42];

        /// <summary>
        /// XXXXX
        /// </summary>
        public uint[] q_position_base = new uint[42];

        #endregion
    }
}