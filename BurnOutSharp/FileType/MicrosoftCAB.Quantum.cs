using System.Runtime.InteropServices;

/// <see href="http://www.russotto.net/quantumcomp.html"/>
/// <see href="https://handwiki.org/wiki/Software:Quantum_compression"/>
/// <see href="https://archive.org/details/datacompressionc00salo_251/page/n206/mode/2up"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/qtm.h"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/qtmc.c"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/qtmd.c"/>
namespace BurnOutSharp.FileType
{
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
        /// LZ model, 5+ character matches, 27 entries, start at symbol 0
        /// </summary>
        SELECTOR_6_LENGTH = 7,
    }

    #region LZ Compression Tables

    public static class QuantumConstants
    {
        /// <summary>
        /// Base position for each position slot (0..41)
        /// Used by selectors 4, 5, and 6
        /// </summary>
        public static readonly uint[] PositionBaseTable = new uint[]
        {
            0x000000, 0x000001, 0x000002, 0x000003, 0x000004, 0x000006, 0x000008, 0x00000c,
            0x000010, 0x000018, 0x000020, 0x000030, 0x000040, 0x000060, 0x000080, 0x0000c0,
            0x000100, 0x000180, 0x000200, 0x000300, 0x000400, 0x000600, 0x000800, 0x000c00,
            0x001000, 0x001800, 0x002000, 0x003000, 0x004000, 0x006000, 0x008000, 0x00c000,
            0x010000, 0x018000, 0x020000, 0x030000, 0x040000, 0x060000, 0x080000, 0x0c0000,
            0x100000, 0x180000,
        };

        /// <summary>
        /// Extra bits for each position slot (0..41)
        /// Used by selectors 4, 5, and 6
        /// </summary>
        public static readonly int[] PositionExtraBitsTable = new int[]
        {
            0,  0,  0,  0,  1,  1,  2,  2,
            3,  3,  4,  4,  5,  5,  6,  6,
            7,  7,  8,  8,  9,  9, 10, 10,
            11, 11, 12, 12, 13, 13, 14, 14,
            15, 15, 16, 16, 17, 17, 18, 18,
            19, 19,
        };

        /// <summary>
        /// Base length for each length slot (0..26)
        /// Used by selector 6
        /// </summary>
        public static readonly byte[] LengthBaseTable = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x08,
            0x0a, 0x0c, 0x0e, 0x12, 0x16, 0x1a, 0x1e, 0x26,
            0x2e, 0x36, 0x3e, 0x4e, 0x5e, 0x6e, 0x7e, 0x9e,
            0xbe, 0xde, 0xfe
        };

        /// <summary>
        /// Extra bits for each length slot (0..26)
        /// Used by selector 6
        /// </summary>
        public static readonly int[] LengthExtraBitsTable = new int[]
        {
            0, 0, 0, 0, 0, 0, 1, 1,
            1, 1, 2, 2, 2, 2, 3, 3,
            3, 3, 4, 4, 4, 4, 5, 5,
            5, 5, 0,
        };

        /// <summary>
        /// Number of position slots for (tsize - 10)
        /// </summary>
        public static readonly int[] NumberOfPositionSlots = new int[]
        {
            20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42,
        };
    }

    #endregion

    /// <summary>
    /// Quantum archive file structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class QuantumArchive
    {
        /// <summary>
        /// Quantum signature: 0x44 0x53
        /// </summary>
        public ushort Signature;

        /// <summary>
        /// Quantum major version number
        /// </summary>
        public byte MajorVersion;

        /// <summary>
        /// Quantum minor version number
        /// </summary>
        public byte MinorVersion;

        /// <summary>
        /// Number of files within this archive
        /// </summary>
        public ushort FileCount;

        /// <summary>
        /// Table size required for decompression
        /// </summary>
        public byte TableSize;

        /// <summary>
        /// Compression flags
        /// </summary>
        public byte CompressionFlags;

        /// <summary>
        /// This is immediately followed by the list of files
        /// </summary>
        public QuantumFileDescriptor[] FileList;

        // Immediately following the list of files is the compressed data. 
    }

    /// <remarks>
    /// Strings are prefixed with their length. If the length is less than
    /// 128 then it is stored directly in one byte. If it is greater than 127
    /// then the high bit of the first byte is set to 1 and the remaining
    /// fifteen bits contain the actual length in big-endian format. 
    /// </remarks>
    public class QuantumFileDescriptor
    {
        /// <summary>
        /// File name, variable length string, not zero-terminated
        /// </summary>
        public string FileName;

        /// <summary>
        /// Comment field, variable length string, not zero-terminated
        /// </summary>
        public string CommentField;

        /// <summary>
        /// Fully expanded file size in bytes
        /// </summary>
        public uint ExpandedFileSize;

        /// <summary>
        /// File time (DOS format) 
        /// </summary>
        public ushort FileTime;

        /// <summary>
        /// File date (DOS format) 
        /// </summary>
        public ushort FileDate;
    }

    public static class QuantumCompressor
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

        public static int GetSymbol(QuantumModel model)
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

    public class QuantumModelSymbol
    {
        public ushort Symbol { get; private set; }

        public ushort CumulativeFrequency { get; private set; }
    }

    public class QuantumModel
    {
        public int Entries { get; set; }

        public QuantumModelSymbol[] Symbols { get; set; }
    }
}
