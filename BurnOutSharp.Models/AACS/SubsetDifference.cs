namespace BurnOutSharp.Models.AACS
{
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    public sealed class SubsetDifference
    {
        /// <summary>
        /// The mask for u is given by the first byte. That byte is
        /// treated as a number, the number of low-order 0-bits in
        /// the mask. For example, the value 0x01 denotes a mask of
        /// 0xFFFFFFFE; value 0x0A denotes a mask of 0xFFFFFC00.
        /// </summary>
        public byte Mask;

        /// <summary>
        /// The last 4 bytes are the uv number, most significant
        /// byte first.
        /// </summary>
        public uint Number;
    }
}