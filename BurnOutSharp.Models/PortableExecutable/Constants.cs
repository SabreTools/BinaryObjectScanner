namespace BurnOutSharp.Models.PortableExecutable
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x50, 0x45, 0x00, 0x00 };

        public const string SignatureString = "PE\0\0";

        public const uint SignatureUInt32 = 0x00004550;
    }
}