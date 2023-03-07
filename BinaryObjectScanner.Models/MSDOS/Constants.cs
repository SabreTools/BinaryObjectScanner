namespace BinaryObjectScanner.Models.MSDOS
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x4d, 0x5a };

        public const string SignatureString = "MZ";

        public const ushort SignatureUInt16 = 0x5a4d;
    }
}