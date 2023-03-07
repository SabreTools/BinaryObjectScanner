namespace BinaryObjectScanner.Models.PlayJ
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0xFF, 0x9D, 0x53, 0x4B };

        public const uint SignatureUInt32 = 0x4B539DFF;
    }
}