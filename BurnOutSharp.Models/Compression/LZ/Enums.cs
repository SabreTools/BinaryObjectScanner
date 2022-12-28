namespace BurnOutSharp.Models.Compression.LZ
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/lzexpand.h"/>
    public enum LZERROR
    {
        LZERROR_BADINHANDLE = -1,
        LZERROR_BADOUTHANDLE = -2,
        LZERROR_READ = -3,
        LZERROR_WRITE = -4,
        LZERROR_GLOBALLOC = -5,
        LZERROR_GLOBLOCK = -6,
        LZERROR_BADVALUE = -7,
        LZERROR_UNKNOWNALG = -8,
    }
}