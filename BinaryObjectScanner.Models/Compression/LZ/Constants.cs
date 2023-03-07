namespace BinaryObjectScanner.Models.Compression.LZ
{
    public static class Constants
    {
        public const int GETLEN	= 2048;

        public const int LZ_MAGIC_LEN = 8;

        public const int LZ_HEADER_LEN = 14;

        public static readonly byte[] MagicBytes = new byte[] { 0x53, 0x5a, 0x44, 0x44, 0x88, 0xf0, 0x27, 0x33 };

        public static readonly string MagicString = System.Text.Encoding.ASCII.GetString(MagicBytes);

        public const ulong MagicUInt64 = 0x3327f08844445a53;

        public const int LZ_TABLE_SIZE = 0x1000;

        public const int MAX_LZSTATES = 16;

        public const int LZ_MIN_HANDLE = 0x400;
    }
}