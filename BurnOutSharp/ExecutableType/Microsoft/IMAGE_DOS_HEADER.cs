using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_DOS_HEADER
    {
        public ushort e_magic;      /* 00: MZ Header signature */
        public ushort e_cblp;       /* 02: Bytes on last page of file */
        public ushort e_cp;         /* 04: Pages in file */
        public ushort e_crlc;       /* 06: Relocations */
        public ushort e_cparhdr;    /* 08: Size of header in paragraphs */
        public ushort e_minalloc;   /* 0a: Minimum extra paragraphs needed */
        public ushort e_maxalloc;   /* 0c: Maximum extra paragraphs needed */
        public ushort e_ss;         /* 0e: Initial (relative) SS value */
        public ushort e_sp;         /* 10: Initial SP value */
        public ushort e_csum;       /* 12: Checksum */
        public ushort e_ip;         /* 14: Initial IP value */
        public ushort e_cs;         /* 16: Initial (relative) CS value */
        public ushort e_lfarlc;     /* 18: File address of relocation table */
        public ushort e_ovno;       /* 1a: Overlay number */
        public ushort[] e_res;      /* 1c: Reserved words [4] */
        public ushort e_oemid;      /* 24: OEM identifier (for e_oeminfo) */
        public ushort e_oeminfo;    /* 26: OEM information; e_oemid specific */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] e_res2;     /* 28: Reserved words [10] */
        public uint e_lfanew;       /* 3c: Offset to extended header */
    }
}
