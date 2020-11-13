using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress;
        public uint Size;
    }
}
