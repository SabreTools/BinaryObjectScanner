namespace BurnOutSharp.Models.WAD
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x57, 0x41, 0x44, 0x33 };

        public const string SignatureString = "WAD3";

        public const uint SignatureUInt32 = 0x33444157;
    }
}