namespace BurnOutSharp.Models.CFB
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
    
        public const ulong SignatureUInt64 = 0xE11AB1A1E011CFD0;
    }
}