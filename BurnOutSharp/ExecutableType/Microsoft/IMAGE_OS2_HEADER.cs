using System.Runtime.InteropServices;

// Converted from https://github.com/wine-mirror/wine/blob/master/include/winnt.h
namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_OS2_HEADER
    {
        public ushort ne_magic;           /* 00 NE signature 'NE' */
        public byte ne_ver;               /* 02 Linker version number */
        public byte ne_rev;               /* 03 Linker revision number */
        public ushort ne_enttab;          /* 04 Offset to entry table relative to NE */
        public ushort ne_cbenttab;        /* 06 Length of entry table in bytes */
        public uint ne_crc;               /* 08 Checksum */
        public ushort ne_flags;           /* 0c Flags about segments in this file */
        public ushort ne_autodata;        /* 0e Automatic data segment number */
        public ushort ne_heap;            /* 10 Initial size of local heap */
        public ushort ne_stack;           /* 12 Initial size of stack */
        public uint ne_csip;              /* 14 Initial CS:IP */
        public uint ne_sssp;              /* 18 Initial SS:SP */
        public ushort ne_cseg;            /* 1c # of entries in segment table */
        public ushort ne_cmod;            /* 1e # of entries in module reference tab. */
        public ushort ne_cbnrestab;       /* 20 Length of nonresident-name table     */
        public ushort ne_segtab;          /* 22 Offset to segment table */
        public ushort ne_rsrctab;         /* 24 Offset to resource table */
        public ushort ne_restab;          /* 26 Offset to resident-name table */
        public ushort ne_modtab;          /* 28 Offset to module reference table */
        public ushort ne_imptab;          /* 2a Offset to imported name table */
        public uint ne_nrestab;           /* 2c Offset to nonresident-name table */
        public ushort ne_cmovent;         /* 30 # of movable entry points */
        public ushort ne_align;           /* 32 Logical sector alignment shift count */
        public ushort ne_cres;            /* 34 # of resource segments */
        public byte ne_exetyp;            /* 36 Flags indicating target OS */
        public byte ne_flagsothers;       /* 37 Additional information flags */
        public ushort ne_pretthunks;      /* 38 Offset to return thunks */
        public ushort ne_psegrefbytes;    /* 3a Offset to segment ref. bytes */
        public ushort ne_swaparea;        /* 3c Reserved by Microsoft */
        public ushort ne_expver;          /* 3e Expected Windows version number */
    }
}
