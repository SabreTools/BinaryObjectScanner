namespace BinaryObjectScanner.Models.Compression.LZX
{
    /// <summary>
    /// 3-bit block type
    /// </summary>
    public enum BlockType : byte
    {
        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_0 = 0b000,

        /// <summary>
        /// Verbatim block
        /// </summary>
        Verbatim = 0b001,

        /// <summary>
        /// Aligned offset block
        /// </summary>
        AlignedOffset = 0b010,

        /// <summary>
        /// Uncompressed block
        /// </summary>
        Uncompressed = 0b011,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_4 = 0b100,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_5 = 0b101,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_6 = 0b110,

        /// <summary>
        /// Not valid
        /// </summary>
        INVALID_7 = 0b111,
    }
}
