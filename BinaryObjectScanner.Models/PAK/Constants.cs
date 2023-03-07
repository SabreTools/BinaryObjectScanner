namespace BinaryObjectScanner.Models.PAK
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x50, 0x41, 0x43, 0x4b };

        public const string SignatureString = "PACK";

        public const uint SignatureUInt32 = 0x4b434150;
    }
}