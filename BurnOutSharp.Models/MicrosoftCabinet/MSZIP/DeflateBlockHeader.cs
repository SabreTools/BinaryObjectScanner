namespace BurnOutSharp.Models.MicrosoftCabinet.MSZIP
{
    /// <see href="https://www.rfc-editor.org/rfc/rfc1951"/>
    public class DeflateBlockHeader
    {
        /// <summary>
        /// Set if and only if this is the last block of the data set.
        /// </summary>
        /// <remarks>Bit 0</remarks>
        public bool BFINAL { get; set; }

        /// <summary>
        /// Specifies how the data are compressed
        /// </summary>
        /// <remarks>Bits 1-2</remarks>
        public DeflateCompressionType BTYPE { get; set; }

        /// <summary>
        /// Block data as defined by the compression type
        /// </summary>
        public IBlockDataHeader BlockDataHeader { get; set; }
    }
}