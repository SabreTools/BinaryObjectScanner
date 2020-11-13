using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_OPTIONAL_HEADER32
    {
        /* Standard fields */

        public ushort Magic; /* 0x10b or 0x107 */    /* 0x00 */
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;      /* 0x10 */
        public uint BaseOfCode;
        public uint BaseOfData;

        /* NT additional fields */

        public uint ImageBase;
        public uint SectionAlignment;     /* 0x20 */
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;     /* 0x30 */
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;         /* 0x40 */
        public ushort Subsystem;
        public DllCharacteristics DllCharacteristics;
        public uint SizeOfStackReserve;
        public uint SizeOfStackCommit;
        public uint SizeOfHeapReserve;        /* 0x50 */
        public uint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory; /* 0x60, [IMAGE_NUMBEROF_DIRECTORY_ENTRIES] */
        /* 0xE0 */
    }
}
