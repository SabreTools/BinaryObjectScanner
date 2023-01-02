namespace BurnOutSharp.Compression.MSZIP
{
    public class HuffmanNode
    {
        /// <summary>
        /// Number of extra bits or operation
        /// </summary>
        public byte ExtraBits;

        /// <summary>
        /// Number of bits in this code or subcode
        /// </summary>
        public byte BitLength;

        #region v

        /// <summary>
        /// Literal, length base, or distance base
        /// </summary>
        public ushort Base;

        /// <summary>
        /// Pointer to next level of table
        /// </summary>
        public HuffmanNode[] NextLevel;

        #endregion
    }
}