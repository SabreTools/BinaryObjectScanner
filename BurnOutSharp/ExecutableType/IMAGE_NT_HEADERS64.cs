using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_NT_HEADERS64
    {
        public uint Signature;
        public IMAGE_FILE_HEADER FileHeader;
        public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
    }
}
