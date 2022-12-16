namespace BurnOutSharp.Models.MicrosoftCabinet.LZX
{
    /// <summary>
    /// Following the generic block header, an uncompressed block begins with 1 to 16 bits of zero padding
    /// to align the bit buffer on a 16-bit boundary. At this point, the bitstream ends and a byte stream
    /// begins. Following the zero padding, new 32-bit values for R0, R1, and R2 are output in little-endian
    /// form, followed by the uncompressed data bytes themselves. Finally, if the uncompressed data length
    /// is odd, one extra byte of zero padding is encoded to realign the following bitstream.
    /// 
    /// Then the bitstream of byte-swapped 16-bit integers resumes for the next Block Type field (if there
    /// are subsequent blocks).
    /// 
    /// The decoded R0, R1, and R2 values are used as initial repeated offset values to decode the
    /// subsequent compressed block if present.
    /// </summary>
    /// <see href="https://interoperability.blob.core.windows.net/files/MS-PATCH/%5bMS-PATCH%5d.pdf"/>
    public class UncompressedBlock
    {
        /// <summary>
        /// Generic block header
        /// </summary>
        public BlockHeader Header;

        /// <summary>
        /// Padding to align following field on 16-bit boundary
        /// </summary>
        /// <remarks>Bits have a value of zero</remarks>
        public ushort PaddingBits;

        /// <summary>
        /// Least significant to most significant byte (little-endian DWORD ([MS-DTYP]))
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public uint R0;

        /// <summary>
        /// Least significant to most significant byte (little-endian DWORD)
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public uint R1;

        /// <summary>
        /// Least significant to most significant byte (little-endian DWORD)
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public uint R2;

        /// <summary>
        /// Can use the direct memcpy function, as specified in [IEEE1003.1]
        /// </summary>
        /// <remarks>Encoded directly in the byte stream, not in the bitstream of byte-swapped 16-bit words</remarks>
        public byte[] RawDataBytes;

        /// <summary>
        /// Only if uncompressed size is odd
        /// </summary>
        public byte AlignmentByte;
    }
}