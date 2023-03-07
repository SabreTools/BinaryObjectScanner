namespace BinaryObjectScanner.Models.N3DS
{
    public static class Constants
    {
        // ExeFS
        public static readonly byte[] CodeSegmentName = new byte[] { 0x2e, 0x63, 0x6f, 0x64, 0x65, 0x00, 0x00, 0x00 }; // .code\0\0\0

        // NCCH
        public const string NCCHMagicNumber = "NCCH";

        // NCSD
        public const string NCSDMagicNumber = "NCSD";

        // RomFS
        public const string RomFSMagicNumber = "IVFC";
        public const uint RomFSSecondMagicNumber = 0x10000;

        // Setup Keys and IVs
        public static byte[] PlainCounter = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public static byte[] ExefsCounter = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public static byte[] RomfsCounter = new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public const int CXTExtendedDataHeaderLength = 0x800;
    }
}
