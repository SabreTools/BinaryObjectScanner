namespace BurnOutSharp.Models.SGA
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x5f, 0x41, 0x52, 0x43, 0x48, 0x49, 0x56, 0x45 };

        public const string SignatureString = "_ARCHIVE";

        public const ulong SignatureUInt64 = 0x455649484352415f;

        /// <summary>
        /// Length of a SGA checksum in bytes
        /// </summary>
        public const int HL_SGA_CHECKSUM_LENGTH = 0x00008000;
    }
}