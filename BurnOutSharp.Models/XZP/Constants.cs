namespace BurnOutSharp.Models.XZP
{
    public static class Constants
    {
        public static readonly byte[] HeaderSignatureBytes = new byte[] { 0x70, 0x69, 0x5a, 0x78 };

        public const string HeaderSignatureString = "piZx";

        public const uint HeaderSignatureUInt32 = 0x785a6970;

        public static readonly byte[] FooterSignatureBytes = new byte[] { 0x74, 0x46, 0x7a, 0x58 };

        public const string FooterSignatureString = "tFzX";

        public const uint FooterSignatureUInt32 = 0x587a4674;
    }
}