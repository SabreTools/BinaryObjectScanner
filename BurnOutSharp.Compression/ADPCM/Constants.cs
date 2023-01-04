namespace BurnOutSharp.Compression.ADPCM
{
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/adpcm/adpcm.h"/>
    public static class Constants
    {
        public const int MAX_ADPCM_CHANNEL_COUNT = 2;

        public const byte INITIAL_ADPCM_STEP_INDEX = 0x2C;

        #region Tables necessary for decompression

        public static readonly int[] NextStepTable =
        {
            -1, 0, -1, 4, -1, 2, -1, 6,
            -1, 1, -1, 5, -1, 3, -1, 7,
            -1, 1, -1, 5, -1, 3, -1, 7,
            -1, 2, -1, 4, -1, 6, -1, 8
        };

        public static readonly int[] StepSizeTable =
        {
            7,     8,     9,    10,     11,    12,    13,    14,
            16,    17,    19,    21,     23,    25,    28,    31,
            34,    37,    41,    45,     50,    55,    60,    66,
            73,    80,    88,    97,    107,   118,   130,   143,
            157,   173,   190,   209,    230,   253,   279,   307,
            337,   371,   408,   449,    494,   544,   598,   658,
            724,   796,   876,   963,   1060,  1166,  1282,  1411,
            1552,  1707,  1878,  2066,   2272,  2499,  2749,  3024,
            3327,  3660,  4026,  4428,   4871,  5358,  5894,  6484,
            7132,  7845,  8630,  9493,  10442, 11487, 12635, 13899,
            15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794,
            32767
        };

        #endregion

        #region ADPCM decompression present in Starcraft I BETA

        public static readonly uint[] adpcm_values_2 = { 0x33, 0x66 };
        public static readonly uint[] adpcm_values_3 = { 0x3A, 0x3A, 0x50, 0x70 };
        public static readonly uint[] adpcm_values_4 = { 0x3A, 0x3A, 0x3A, 0x3A, 0x4D, 0x66, 0x80, 0x9A };
        public static readonly uint[] adpcm_values_6 =
        {
            0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A, 0x3A,
            0x46, 0x53, 0x60, 0x6D, 0x7A, 0x86, 0x93, 0xA0, 0xAD, 0xBA, 0xC6, 0xD3, 0xE0, 0xED, 0xFA, 0x106
        };

        #endregion
    }
}