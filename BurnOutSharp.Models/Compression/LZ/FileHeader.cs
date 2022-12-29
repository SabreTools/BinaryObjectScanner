namespace BurnOutSharp.Models.Compression.LZ
{
    /// <summary>
    /// Format of first 14 byte of LZ compressed file
    /// </summary>
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/kernel32/lzexpand.c"/>
    public sealed class FileHeaader
    {
        public string Magic;

        public byte CompressionType;

        public char LastChar;

        public uint RealLength;
    }
}