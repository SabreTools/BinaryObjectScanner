namespace BurnOutSharp.Compression.MSZIP
{
    public unsafe struct HuffmanNode
    {
        /// <summary>
        /// Number of extra bits or operation
        /// </summary>
        public byte e;

        /// <summary>
        /// Number of bits in this code or subcode
        /// </summary>
        public byte b;

        #region v

        /// <summary>
        /// Literal, length base, or distance base
        /// </summary>
        public ushort n;

        /// <summary>
        /// Pointer to next level of table
        /// </summary>
        public HuffmanNode* t;

        #endregion
    }
}