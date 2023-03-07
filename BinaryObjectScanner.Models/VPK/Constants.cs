namespace BinaryObjectScanner.Models.VPK
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x34, 0x12, 0xaa, 0x55 };

        public static readonly string SignatureString = System.Text.Encoding.ASCII.GetString(SignatureBytes);

        public const uint SignatureUInt32 = 0x55aa1234;

        /// <summary>
        /// Index indicating that there is no archive
        /// </summary>
        public const int HL_VPK_NO_ARCHIVE = 0x7fff;

        /// <summary>
        /// Length of a VPK checksum in bytes
        /// </summary>
        public const int HL_VPK_CHECKSUM_LENGTH = 0x00008000;
    }
}