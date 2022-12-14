namespace BurnOutSharp.FileType
{
    #region TEMPORARY AREA FOR QUANTUM COMPRESSION FORMAT

    // See http://www.russotto.net/quantumcomp.html for details about implementation

    internal enum SelectorModel
    {
        /// <summary>
        /// Literal model, 64 entries, start at symbol 0
        /// </summary>
        SELECTOR_0 = 0,

        /// <summary>
        /// Literal model, 64 entries, start at symbol 64
        /// </summary>
        SELECTOR_1 = 1,

        /// <summary>
        /// Literal model, 64 entries, start at symbol 128
        /// </summary>
        SELECTOR_2 = 2,

        /// <summary>
        /// Literal model, 64 entries, start at symbol 192
        /// </summary>
        SELECTOR_3 = 3,

        /// <summary>
        /// LZ model, 3 character matches, max 24 entries, start at symbol 0
        /// </summary>
        SELECTOR_4 = 4,

        /// <summary>
        /// LZ model, 4 character matches, max 36 entries, start at symbol 0
        /// </summary>
        SELECTOR_5 = 5,

        /// <summary>
        /// LZ model, 5+ character matches, max 42 entries, start at symbol 0
        /// </summary>
        SELECTOR_6_POSITION = 6,

        /// <summary>
        /// LZ model, 5+ character matches, max 27 entries, start at symbol 0
        /// </summary>
        SELECTOR_6_LENGTH = 7,
    }

    #region LZ Compression Tables

    internal static class QuantumConstants
    {
        internal static readonly uint[] PositionBaseTable = new uint[]
        {
                0x00000,  0x00001, 0x00002, 0x00003, 0x00004, 0x00006, 0x00008, 0x0000c,
                0x00010,  0x00018, 0x00020, 0x00030, 0x00040, 0x00060, 0x00080, 0x000c0,
                0x00100,  0x00180, 0x00200, 0x00300, 0x00400, 0x00600, 0x00800, 0x00c00,
                0x01000,  0x01800, 0x02000, 0x03000, 0x04000, 0x06000, 0x08000, 0x0c000,
                0x10000,  0x18000, 0x20000, 0x30000, 0x40000, 0x60000, 0x80000, 0xc0000,
            0x100000, 0x180000,
        };

        internal static readonly int[] PositionExtraBitsTable = new int[]
        {
                0,  0,  0,  0,  1,  1,  2,  2,
                3,  3,  4,  4,  5,  5,  6,  6,
                7,  7,  8,  8,  9,  9, 10, 10,
            11, 11, 12, 12, 13, 13, 14, 14,
            15, 15, 16, 16, 17, 17, 18, 18,
            19, 19,
        };

        internal static readonly byte[] LengthBaseTable = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x08,
            0x0a, 0x0c, 0x0e, 0x12, 0x16, 0x1a, 0x1e, 0x26,
            0x2e, 0x36, 0x3e, 0x4e, 0x5e, 0x6e, 0x7e, 0x9e,
            0xbe, 0xde, 0xfe
        };

        internal static readonly int[] LengthExtraBitsTable = new int[]
        {
            0, 0, 0, 0, 0, 0, 1, 1,
            1, 1, 2, 2, 2, 2, 3, 3,
            3, 3, 4, 4, 4, 4, 5, 5,
            5, 5, 0,
        };

        /// <summary>
        /// Number of position slots for (tsize - 10)
        /// </summary>
        internal static readonly int[] NumberOfPositionSlots = new int[]
        {
            20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42,
        };
    }

    #endregion

    internal static class QuantumCompressor
    {
        // TODO: Determine how these values are set
        private static uint CS_C = 0;
        private static uint CS_H = 0;
        private static uint CS_L = 0;

        /// <summary>
        /// Get frequency from code
        /// </summary>
        public static ushort GetFrequency(ushort totfreq)
        {
            uint range = ((CS_H - CS_L) & 0xFFFF) + 1;
            uint freq = ((CS_C - CS_L + 1) * totfreq - 1) / range;
            return (ushort)(freq & 0xFFFF);
        }

        /// <summary>
        /// The decoder renormalization loop
        /// </summary>
        public static int GetCode(int cumfreqm1, int cumfreq, int totfreq)
        {
            uint range = (CS_H - CS_L) + 1;
            CS_H = CS_L + (uint)((cumfreqm1 * range) / totfreq) - 1;
            CS_L = CS_L + (uint)((cumfreq * range) / totfreq);

            while (true)
            {
                if ((CS_L & 0x8000) != (CS_H & 0x8000))
                {
                    if ((CS_L & 0x4000) != 0 && (CS_H & 0x4000) == 0)
                    {
                        // Underflow case
                        CS_C ^= 0x4000;
                        CS_L &= 0x3FFF;
                        CS_H |= 0x4000;
                    }
                    else
                    {
                        break;
                    }
                }

                CS_L <<= 1;
                CS_H = (CS_H << 1) | 1;
                CS_C = (CS_C << 1) | 0; // TODO: Figure out what `getbit()` is and replace the placeholder `0`
            }

            // TODO: Figure out what is supposed to return here
            return 0;
        }

        public static int GetSymbol(Model model)
        {
            int freq = GetFrequency(model.Symbols[0].CumulativeFrequency);

            int i = 1;
            for (; i < model.Entries; i++)
            {
                if (model.Symbols[i].CumulativeFrequency <= freq)
                    break;
            }

            int sym = model.Symbols[i - 1].Symbol;

            GetCode(model.Symbols[i - 1].CumulativeFrequency, model.Symbols[i].CumulativeFrequency, model.Symbols[0].CumulativeFrequency);

            // TODO: Figure out what `update_model` does
            //update_model(model, i);

            return sym;
        }
    }

    internal class ModelSymbol
    {
        public ushort Symbol { get; private set; }

        public ushort CumulativeFrequency { get; private set; }
    }

    internal class Model
    {
        public int Entries { get; private set; }

        public ModelSymbol[] Symbols { get; private set; }
    }

    #endregion

}
