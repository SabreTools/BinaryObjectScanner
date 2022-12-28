namespace BurnOutSharp.Models.BFPK
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x42, 0x46, 0x50, 0x4b };

        public const string SignatureString = "BFPK";

        public const uint SignatureUInt32 = 0x4b504642;
    }
}