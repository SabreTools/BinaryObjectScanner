namespace BurnOutSharp.Models.Compression.MSZIP
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    public static class Constants
    {
        /// <summary>
        /// Window size
        /// </summary>
        public const ushort ZIPWSIZE = 0x8000;

        /// <summary>
        /// Bits in base literal/length lookup table
        /// </summary>
        public const int ZIPLBITS = 9;

        /// <summary>
        /// Bits in base distance lookup table
        /// </summary>
        public const int ZIPDBITS = 6;

        /// <summary>
        /// Maximum bit length of any code
        /// </summary>
        public const int ZIPBMAX = 16;

        /// <summary>
        /// Maximum number of codes in any set
        /// </summary>
        public const int ZIPN_MAX = 288;

        #region THOSE_ZIP_CONSTS

        /// <summary>
        /// Order of the bit length code lengths
        /// </summary>
        public static readonly byte[] BitLengthOrder = new byte[]
        {
            16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
        };

        /// <summary>
        /// Copy lengths for literal codes 257..285
        /// </summary>
        public static readonly ushort[] CopyLengths = new ushort[]
        {
            3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51,
            59, 67, 83, 99, 115, 131, 163, 195, 227, 258, 0, 0
        };

        /// <summary>
        /// Extra bits for literal codes 257..285
        /// </summary>
        /// <remarks>99 == invalid</remarks>
        public static readonly ushort[] LiteralExtraBits = new ushort[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4,
            4, 5, 5, 5, 5, 0, 99, 99
        };

        /// <summary>
        /// Copy offsets for distance codes 0..29
        /// </summary>
        public static readonly ushort[] CopyOffsets = new ushort[]
        {
            1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385,
            513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577
        };

        /// <summary>
        /// Extra bits for distance codes
        /// </summary>
        public static readonly ushort[] DistanceExtraBits = new ushort[]
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10,
            10, 11, 11, 12, 12, 13, 13
        };

        /// <summary>
        /// And'ing with Zipmask[n] masks the lower n bits
        /// </summary>
        public static readonly ushort[] BitMasks = new ushort[17]
        {
            0x0000, 0x0001, 0x0003, 0x0007, 0x000f, 0x001f, 0x003f, 0x007f, 0x00ff,
            0x01ff, 0x03ff, 0x07ff, 0x0fff, 0x1fff, 0x3fff, 0x7fff, 0xffff
        };

        #endregion
    }
}