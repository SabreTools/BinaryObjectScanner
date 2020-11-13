using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_NT_HEADERS32
    {
        public uint Signature; /* "PE"\0\0 */   /* 0x00 */
        public IMAGE_FILE_HEADER FileHeader;       /* 0x04 */
        public IMAGE_OPTIONAL_HEADER32 OptionalHeader;	/* 0x18 */
    }
}
