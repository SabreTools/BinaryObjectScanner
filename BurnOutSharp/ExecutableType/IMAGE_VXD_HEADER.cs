using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_VXD_HEADER
    {
        public ushort e32_magic;
        public byte e32_border;
        public byte e32_worder;
        public uint e32_level;
        public ushort e32_cpu;
        public ushort e32_os;
        public uint e32_ver;
        public uint e32_mflags;
        public uint e32_mpages;
        public uint e32_startobj;
        public uint e32_eip;
        public uint e32_stackobj;
        public uint e32_esp;
        public uint e32_pagesize;
        public uint e32_lastpagesize;
        public uint e32_fixupsize;
        public uint e32_fixupsum;
        public uint e32_ldrsize;
        public uint e32_ldrsum;
        public uint e32_objtab;
        public uint e32_objcnt;
        public uint e32_objmap;
        public uint e32_itermap;
        public uint e32_rsrctab;
        public uint e32_rsrccnt;
        public uint e32_restab;
        public uint e32_enttab;
        public uint e32_dirtab;
        public uint e32_dircnt;
        public uint e32_fpagetab;
        public uint e32_frectab;
        public uint e32_impmod;
        public uint e32_impmodcnt;
        public uint e32_impproc;
        public uint e32_pagesum;
        public uint e32_datapage;
        public uint e32_preload;
        public uint e32_nrestab;
        public uint e32_cbnrestab;
        public uint e32_nressum;
        public uint e32_autodata;
        public uint e32_debuginfo;
        public uint e32_debuglen;
        public uint e32_instpreload;
        public uint e32_instdemand;
        public uint e32_heapsize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] e32_res3; // [12]
        public uint e32_winresoff;
        public uint e32_winreslen;
        public ushort e32_devid;
        public ushort e32_ddkver;
    }
}
