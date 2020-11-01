using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BurnOutSharp.ExecutableType
{
    internal enum DirectoryEntries
    {
        IMAGE_DIRECTORY_ENTRY_EXPORT = 0,
        IMAGE_DIRECTORY_ENTRY_IMPORT = 1,
        IMAGE_DIRECTORY_ENTRY_RESOURCE = 2,
        IMAGE_DIRECTORY_ENTRY_EXCEPTION = 3,
        IMAGE_DIRECTORY_ENTRY_SECURIT = 4,
        IMAGE_DIRECTORY_ENTRY_BASERELOC = 5,
        IMAGE_DIRECTORY_ENTRY_DEBUG = 6,
        IMAGE_DIRECTORY_ENTRY_COPYRIGHT = 7,
        IMAGE_DIRECTORY_ENTRY_GLOBALPTR = 8, // (MIPS GP)
        IMAGE_DIRECTORY_ENTRY_TLS = 9,
        IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG = 10,
        IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT = 11,
        IMAGE_DIRECTORY_ENTRY_IAT = 12, // Import Address Table
        IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT = 13,
        IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14,
    }

    [Flags]
    internal enum DllCharacteristics : ushort
    {
        IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE = 0x0040,
        IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
        IMAGE_DLLCHARACTERISTICS_NX_COMPAT = 0x0100,
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
        IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
        IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000,
    }

    [Flags]
    internal enum FileCharacteristics : ushort
    {
        IMAGE_FILE_RELOCS_STRIPPED = 0x0001, /* No relocation info */
        IMAGE_FILE_EXECUTABLE_IMAGE = 0x0002,
        IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004,
        IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008,
        IMAGE_FILE_AGGRESIVE_WS_TRIM = 0x0010,
        IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020,
        IMAGE_FILE_16BIT_MACHINE = 0x0040,
        IMAGE_FILE_BYTES_REVERSED_LO = 0x0080,
        IMAGE_FILE_32BIT_MACHINE = 0x0100,
        IMAGE_FILE_DEBUG_STRIPPED = 0x0200,
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 0x0400,
        IMAGE_FILE_NET_RUN_FROM_SWAP = 0x0800,
        IMAGE_FILE_SYSTEM = 0x1000,
        IMAGE_FILE_DLL = 0x2000,
        IMAGE_FILE_UP_SYSTEM_ONLY = 0x4000,
        IMAGE_FILE_BYTES_REVERSED_HI = 0x8000,
    }

    internal enum MachineSettings : ushort
    {
        IMAGE_FILE_MACHINE_UNKNOWN = 0,
        IMAGE_FILE_MACHINE_TARGET_HOST = 0x0001,
        IMAGE_FILE_MACHINE_I860 = 0x014d,
        IMAGE_FILE_MACHINE_I386 = 0x014c,
        IMAGE_FILE_MACHINE_R3000 = 0x0162,
        IMAGE_FILE_MACHINE_R4000 = 0x0166,
        IMAGE_FILE_MACHINE_R10000 = 0x0168,
        IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x0169,
        IMAGE_FILE_MACHINE_ALPHA = 0x0184,
        IMAGE_FILE_MACHINE_SH3 = 0x01a2,
        IMAGE_FILE_MACHINE_SH3DSP = 0x01a3,
        IMAGE_FILE_MACHINE_SH3E = 0x01a4,
        IMAGE_FILE_MACHINE_SH4 = 0x01a6,
        IMAGE_FILE_MACHINE_SH5 = 0x01a8,
        IMAGE_FILE_MACHINE_ARM = 0x01c0,
        IMAGE_FILE_MACHINE_THUMB = 0x01c2,
        IMAGE_FILE_MACHINE_ARMNT = 0x01c4,
        IMAGE_FILE_MACHINE_ARM64 = 0xaa64,
        IMAGE_FILE_MACHINE_AM33 = 0x01d3,
        IMAGE_FILE_MACHINE_POWERPC = 0x01f0,
        IMAGE_FILE_MACHINE_POWERPCFP = 0x01f1,
        IMAGE_FILE_MACHINE_IA64 = 0x0200,
        IMAGE_FILE_MACHINE_MIPS16 = 0x0266,
        IMAGE_FILE_MACHINE_ALPHA64 = 0x0284,
        IMAGE_FILE_MACHINE_MIPSFPU = 0x0366,
        IMAGE_FILE_MACHINE_MIPSFPU16 = 0x0466,
        IMAGE_FILE_MACHINE_AXP64 = 0x0284,
        IMAGE_FILE_MACHINE_TRICORE = 0x0520,
        IMAGE_FILE_MACHINE_CEF = 0x0cef,
        IMAGE_FILE_MACHINE_EBC = 0x0ebc,
        IMAGE_FILE_MACHINE_AMD64 = 0x8664,
        IMAGE_FILE_MACHINE_M32R = 0x9041,
        IMAGE_FILE_MACHINE_CEE = 0xc0ee,
    }

    [Flags]
    internal enum SectionCharacteristics : uint
    {
        IMAGE_SCN_TYPE_REG = 0x00000000, // Reserved
        IMAGE_SCN_TYPE_DSECT = 0x00000001, // Reserved
        IMAGE_SCN_TYPE_NOLOAD = 0x00000002, // Reserved
        IMAGE_SCN_TYPE_GROUP = 0x00000004, // Reserved
        IMAGE_SCN_TYPE_NO_PAD = 0x00000008, // Reserved
        IMAGE_SCN_TYPE_COPY = 0x00000010, // Reserved

        IMAGE_SCN_CNT_CODE = 0x00000020,
        IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040,
        IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080,

        IMAGE_SCN_LNK_OTHER = 0x00000100,
        IMAGE_SCN_LNK_INFO = 0x00000200,
        IMAGE_SCN_TYPE_OVER = 0x00000400, // Reserved
        IMAGE_SCN_LNK_REMOVE = 0x00000800,
        IMAGE_SCN_LNK_COMDAT = 0x00001000,

        /* 						0x00002000 - Reserved */
        IMAGE_SCN_MEM_PROTECTED = 0x00004000, // Obsolete
        IMAGE_SCN_MEM_FARDATA = 0x00008000,

        IMAGE_SCN_MEM_SYSHEAP = 0x00010000, // Obsolete
        IMAGE_SCN_MEM_PURGEABLE = 0x00020000,
        IMAGE_SCN_MEM_16BIT = 0x00020000,
        IMAGE_SCN_MEM_LOCKED = 0x00040000,
        IMAGE_SCN_MEM_PRELOAD = 0x00080000,

        IMAGE_SCN_ALIGN_1BYTES = 0x00100000,
        IMAGE_SCN_ALIGN_2BYTES = 0x00200000,
        IMAGE_SCN_ALIGN_4BYTES = 0x00300000,
        IMAGE_SCN_ALIGN_8BYTES = 0x00400000,
        IMAGE_SCN_ALIGN_16BYTES = 0x00500000, // Default
        IMAGE_SCN_ALIGN_32BYTES = 0x00600000,
        IMAGE_SCN_ALIGN_64BYTES = 0x00700000,
        IMAGE_SCN_ALIGN_128BYTES = 0x00800000,
        IMAGE_SCN_ALIGN_256BYTES = 0x00900000,
        IMAGE_SCN_ALIGN_512BYTES = 0x00A00000,
        IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000,
        IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000,
        IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000,
        IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000,
        /* 						0x00F00000 - Unused */
        IMAGE_SCN_ALIGN_MASK = 0x00F00000,

        IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,


        IMAGE_SCN_MEM_DISCARDABLE = 0x02000000,
        IMAGE_SCN_MEM_NOT_CACHED = 0x04000000,
        IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,
        IMAGE_SCN_MEM_SHARED = 0x10000000,
        IMAGE_SCN_MEM_EXECUTE = 0x20000000,
        IMAGE_SCN_MEM_READ = 0x40000000,
        IMAGE_SCN_MEM_WRITE = 0x80000000,
    }

    internal enum Subsystem : ushort
    {
        IMAGE_SUBSYSTEM_UNKNOWN = 0,
        IMAGE_SUBSYSTEM_NATIVE = 1,
        IMAGE_SUBSYSTEM_WINDOWS_GUI = 2, // Windows GUI subsystem
        IMAGE_SUBSYSTEM_WINDOWS_CUI = 3, // Windows character subsystem
        IMAGE_SUBSYSTEM_OS2_CUI = 5,
        IMAGE_SUBSYSTEM_POSIX_CUI = 7,
        IMAGE_SUBSYSTEM_NATIVE_WINDOWS = 8, // native Win9x driver
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9, // Windows CE subsystem
        IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
        IMAGE_SUBSYSTEM_EFI_ROM = 13,
        IMAGE_SUBSYSTEM_XBOX = 14,
        IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION = 16,
    }

    internal static class Constants
    {
        public const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;     /* MZ   */
        public const ushort IMAGE_OS2_SIGNATURE = 0x454E;     /* NE   */
        public const ushort IMAGE_OS2_SIGNATURE_LE = 0x454C;     /* LE   */
        public const ushort IMAGE_OS2_SIGNATURE_LX = 0x584C;     /* LX */
        public const ushort IMAGE_VXD_SIGNATURE = 0x454C;     /* LE   */
        public const uint IMAGE_NT_SIGNATURE = 0x00004550; /* PE00 */

        public const int IMAGE_SIZEOF_FILE_HEADER = 20;
        public const int IMAGE_SIZEOF_ROM_OPTIONAL_HEADER = 56;
        public const int IMAGE_SIZEOF_STD_OPTIONAL_HEADER = 28;
        public const int IMAGE_SIZEOF_NT_OPTIONAL32_HEADER = 224;
        public const int IMAGE_SIZEOF_NT_OPTIONAL64_HEADER = 240;
        public const int IMAGE_SIZEOF_SHORT_NAME = 8;
        public const int IMAGE_SIZEOF_SECTION_HEADER = 40;
        public const int IMAGE_SIZEOF_SYMBOL = 18;
        public const int IMAGE_SIZEOF_AUX_SYMBOL = 18;
        public const int IMAGE_SIZEOF_RELOCATION = 10;
        public const int IMAGE_SIZEOF_BASE_RELOCATION = 8;
        public const int IMAGE_SIZEOF_LINENUMBER = 6;
        public const int IMAGE_SIZEOF_ARCHIVE_MEMBER_HDR = 60;

        // Possible Magic values
        public const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        public const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b;
        public const ushort IMAGE_ROM_OPTIONAL_HDR_MAGIC = 0x107;

        public const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
    }

    internal class IMAGE_DOS_HEADER
    {
        ushort e_magic;      /* 00: MZ Header signature */
        ushort e_cblp;       /* 02: Bytes on last page of file */
        ushort e_cp;         /* 04: Pages in file */
        ushort e_crlc;       /* 06: Relocations */
        ushort e_cparhdr;    /* 08: Size of header in paragraphs */
        ushort e_minalloc;   /* 0a: Minimum extra paragraphs needed */
        ushort e_maxalloc;   /* 0c: Maximum extra paragraphs needed */
        ushort e_ss;         /* 0e: Initial (relative) SS value */
        ushort e_sp;         /* 10: Initial SP value */
        ushort e_csum;       /* 12: Checksum */
        ushort e_ip;         /* 14: Initial IP value */
        ushort e_cs;         /* 16: Initial (relative) CS value */
        ushort e_lfarlc;     /* 18: File address of relocation table */
        ushort e_ovno;       /* 1a: Overlay number */
        ushort[] e_res;      /* 1c: Reserved words [4] */
        ushort e_oemid;      /* 24: OEM identifier (for e_oeminfo) */
        ushort e_oeminfo;    /* 26: OEM information; e_oemid specific */
        ushort[] e_res2;     /* 28: Reserved words [10] */
        uint e_lfanew;       /* 3c: Offset to extended header */
    }

    internal class IMAGE_OS2_HEADER
    {
        ushort ne_magic;           /* 00 NE signature 'NE' */
        byte ne_ver;               /* 02 Linker version number */
        byte ne_rev;               /* 03 Linker revision number */
        ushort ne_enttab;          /* 04 Offset to entry table relative to NE */
        ushort ne_cbenttab;        /* 06 Length of entry table in bytes */
        uint ne_crc;               /* 08 Checksum */
        ushort ne_flags;           /* 0c Flags about segments in this file */
        ushort ne_autodata;        /* 0e Automatic data segment number */
        ushort ne_heap;            /* 10 Initial size of local heap */
        ushort ne_stack;           /* 12 Initial size of stack */
        uint ne_csip;              /* 14 Initial CS:IP */
        uint ne_sssp;              /* 18 Initial SS:SP */
        ushort ne_cseg;            /* 1c # of entries in segment table */
        ushort ne_cmod;            /* 1e # of entries in module reference tab. */
        ushort ne_cbnrestab;       /* 20 Length of nonresident-name table     */
        ushort ne_segtab;          /* 22 Offset to segment table */
        ushort ne_rsrctab;         /* 24 Offset to resource table */
        ushort ne_restab;          /* 26 Offset to resident-name table */
        ushort ne_modtab;          /* 28 Offset to module reference table */
        ushort ne_imptab;          /* 2a Offset to imported name table */
        uint ne_nrestab;           /* 2c Offset to nonresident-name table */
        ushort ne_cmovent;         /* 30 # of movable entry points */
        ushort ne_align;           /* 32 Logical sector alignment shift count */
        ushort ne_cres;            /* 34 # of resource segments */
        byte ne_exetyp;            /* 36 Flags indicating target OS */
        byte ne_flagsothers;       /* 37 Additional information flags */
        ushort ne_pretthunks;      /* 38 Offset to return thunks */
        ushort ne_psegrefbytes;    /* 3a Offset to segment ref. bytes */
        ushort ne_swaparea;        /* 3c Reserved by Microsoft */
        ushort ne_expver;          /* 3e Expected Windows version number */
    }

    internal class IMAGE_VXD_HEADER
    {
        ushort e32_magic;
        byte e32_border;
        byte e32_worder;
        uint e32_level;
        ushort e32_cpu;
        ushort e32_os;
        uint e32_ver;
        uint e32_mflags;
        uint e32_mpages;
        uint e32_startobj;
        uint e32_eip;
        uint e32_stackobj;
        uint e32_esp;
        uint e32_pagesize;
        uint e32_lastpagesize;
        uint e32_fixupsize;
        uint e32_fixupsum;
        uint e32_ldrsize;
        uint e32_ldrsum;
        uint e32_objtab;
        uint e32_objcnt;
        uint e32_objmap;
        uint e32_itermap;
        uint e32_rsrctab;
        uint e32_rsrccnt;
        uint e32_restab;
        uint e32_enttab;
        uint e32_dirtab;
        uint e32_dircnt;
        uint e32_fpagetab;
        uint e32_frectab;
        uint e32_impmod;
        uint e32_impmodcnt;
        uint e32_impproc;
        uint e32_pagesum;
        uint e32_datapage;
        uint e32_preload;
        uint e32_nrestab;
        uint e32_cbnrestab;
        uint e32_nressum;
        uint e32_autodata;
        uint e32_debuginfo;
        uint e32_debuglen;
        uint e32_instpreload;
        uint e32_instdemand;
        uint e32_heapsize;
        byte[] e32_res3; // [12]
        uint e32_winresoff;
        uint e32_winreslen;
        ushort e32_devid;
        ushort e32_ddkver;
    }

    internal class IMAGE_FILE_HEADER
    {
        ushort Machine;
        ushort NumberOfSections;
        uint TimeDateStamp;
        uint PointerToSymbolTable;
        uint NumberOfSymbols;
        ushort SizeOfOptionalHeader;
        ushort Characteristics;
    }

    internal class IMAGE_DATA_DIRECTORY
    {
        uint VirtualAddress;
        uint Size;
    }

    internal class IMAGE_OPTIONAL_HEADER64
    {
        ushort Magic; /* 0x20b */
        byte MajorLinkerVersion;
        byte MinorLinkerVersion;
        uint SizeOfCode;
        uint SizeOfInitializedData;
        uint SizeOfUninitializedData;
        uint AddressOfEntryPoint;
        uint BaseOfCode;
        ulong ImageBase;
        uint SectionAlignment;
        uint FileAlignment;
        ushort MajorOperatingSystemVersion;
        ushort MinorOperatingSystemVersion;
        ushort MajorImageVersion;
        ushort MinorImageVersion;
        ushort MajorSubsystemVersion;
        ushort MinorSubsystemVersion;
        uint Win32VersionValue;
        uint SizeOfImage;
        uint SizeOfHeaders;
        uint CheckSum;
        ushort Subsystem;
        ushort DllCharacteristics;
        ulong SizeOfStackReserve;
        ulong SizeOfStackCommit;
        ulong SizeOfHeapReserve;
        ulong SizeOfHeapCommit;
        uint LoaderFlags;
        uint NumberOfRvaAndSizes;
        IMAGE_DATA_DIRECTORY[] DataDirectory; // [IMAGE_NUMBEROF_DIRECTORY_ENTRIES]
    }

    internal class IMAGE_NT_HEADERS64
    {
        uint Signature;
        IMAGE_FILE_HEADER FileHeader;
        IMAGE_OPTIONAL_HEADER64 OptionalHeader;
    }

    internal class IMAGE_OPTIONAL_HEADER32
    {
        /* Standard fields */

        ushort Magic; /* 0x10b or 0x107 */    /* 0x00 */
        byte MajorLinkerVersion;
        byte MinorLinkerVersion;
        uint SizeOfCode;
        uint SizeOfInitializedData;
        uint SizeOfUninitializedData;
        uint AddressOfEntryPoint;      /* 0x10 */
        uint BaseOfCode;
        uint BaseOfData;

        /* NT additional fields */

        uint ImageBase;
        uint SectionAlignment;     /* 0x20 */
        uint FileAlignment;
        ushort MajorOperatingSystemVersion;
        ushort MinorOperatingSystemVersion;
        ushort MajorImageVersion;
        ushort MinorImageVersion;
        ushort MajorSubsystemVersion;     /* 0x30 */
        ushort MinorSubsystemVersion;
        uint Win32VersionValue;
        uint SizeOfImage;
        uint SizeOfHeaders;
        uint CheckSum;         /* 0x40 */
        ushort Subsystem;
        ushort DllCharacteristics;
        uint SizeOfStackReserve;
        uint SizeOfStackCommit;
        uint SizeOfHeapReserve;        /* 0x50 */
        uint SizeOfHeapCommit;
        uint LoaderFlags;
        uint NumberOfRvaAndSizes;
        IMAGE_DATA_DIRECTORY[] DataDirectory; /* 0x60, [IMAGE_NUMBEROF_DIRECTORY_ENTRIES] */
        /* 0xE0 */
    }

    internal class IMAGE_NT_HEADERS32
    {
        uint Signature; /* "PE"\0\0 */   /* 0x00 */
        IMAGE_FILE_HEADER FileHeader;       /* 0x04 */
        IMAGE_OPTIONAL_HEADER32 OptionalHeader;	/* 0x18 */
    }

    internal class IMAGE_SECTION_HEADER
    {
        byte[] Name; // [IMAGE_SIZEOF_SHORT_NAME];
        uint PhysicalAddressOrVirtualSize; // Misc
        uint VirtualAddress;
        uint SizeOfRawData;
        uint PointerToRawData;
        uint PointerToRelocations;
        uint PointerToLinenumbers;
        ushort NumberOfRelocations;
        ushort NumberOfLinenumbers;
        uint Characteristics;
    }
}
