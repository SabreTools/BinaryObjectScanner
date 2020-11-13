using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_OPTIONAL_HEADER64
    {
        public ushort Magic; /* 0x20b */
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public ulong ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public DllCharacteristics DllCharacteristics;
        public ulong SizeOfStackReserve;
        public ulong SizeOfStackCommit;
        public ulong SizeOfHeapReserve;
        public ulong SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory; // [IMAGE_NUMBEROF_DIRECTORY_ENTRIES]
    }
}
