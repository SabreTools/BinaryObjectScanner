using System;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#dll-characteristics
    [Flags]
    public enum DllCharacteristics : ushort
    {
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved1                                       = 0x0001,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved2                                       = 0x0002,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved4                                       = 0x0004,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved8                                       = 0x0008,

        /// <summary>
        /// Image can handle a high entropy 64-bit virtual address space.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_HIGH_ENTROPY_VA        = 0x0020,

        /// <summary>
        /// DLL can be relocated at load time.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE           = 0x0040,

        /// <summary>
        /// Code Integrity checks are enforced.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY        = 0x0080,

        /// <summary>
        /// Image is NX compatible.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NX_COMPAT              = 0x0100,

        /// <summary>
        /// Isolation aware, but do not isolate the image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION           = 0x0200,

        /// <summary>
        /// Does not use structured exception (SE) handling.
        /// No SE handler may be called in this image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_SEH                 = 0x0400,

        /// <summary>
        /// Do not bind the image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_BIND                = 0x0800,

        /// <summary>
        /// Image must execute in an AppContainer.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_APPCONTAINER           = 0x1000,

        /// <summary>
        /// A WDM driver.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER             = 0x2000,

        /// <summary>
        /// Image supports Control Flow Guard.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_GUARD_CF               = 0x4000,

        /// <summary>
        /// Terminal Server aware.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE  = 0x8000,
    }

    // https://docs.microsoft.com/en-us/windows/win32/api/verrsrc/ns-verrsrc-vs_fixedfileinfo
    [Flags]
    public enum FileInfoFileFlags : uint
    {
        /// <summary>
        /// The file contains debugging information or is compiled with debugging features enabled.
        /// </summary>
        VS_FF_DEBUG         = 0x00000001,

        /// <summary>
        /// The file's version structure was created dynamically; therefore, some of the members in this structure may be empty or incorrect. This flag should never be set in a file's VS_VERSIONINFO data.
        /// </summary>
        VS_FF_INFOINFERRED  = 0x00000010,

        /// <summary>
        /// The file has been modified and is not identical to the original shipping file of the same version number.
        /// </summary>
        VS_FF_PATCHED       = 0x00000004,

        /// <summary>
        /// The file is a development version, not a commercially released product.
        /// </summary>
        VS_FF_PRERELEASE    = 0x00000002,

        /// <summary>
        /// The file was not built using standard release procedures. If this flag is set, the StringFileInfo structure should contain a PrivateBuild entry.
        /// </summary>
        VS_FF_PRIVATEBUILD  = 0x00000008,

        /// <summary>
        /// The file was built by the original company using standard release procedures but is a variation of the normal file of the same version number. If this flag is set, the StringFileInfo structure should contain a SpecialBuild entry.
        /// </summary>
        VS_FF_SPECIALBUILD  = 0x00000020,
    }

    // https://docs.microsoft.com/en-us/windows/win32/api/verrsrc/ns-verrsrc-vs_fixedfileinfo
    public enum FileInfoFileType : uint
    {
        /// <summary>
        /// The file contains an application.
        /// </summary>
        VFT_APP         = 0x00000001,

        /// <summary>
        /// The file contains a DLL.
        /// </summary>
        VFT_DLL         = 0x00000002,

        /// <summary>
        /// The file contains a device driver. If dwFileType is VFT_DRV, dwFileSubtype contains a more specific description of the driver.
        /// </summary>
        VFT_DRV         = 0x00000003,

        /// <summary>
        /// The file contains a font. If dwFileType is VFT_FONT, dwFileSubtype contains a more specific description of the font file.
        /// </summary>
        VFT_FONT        = 0x00000004,

        /// <summary>
        /// The file contains a static-link library.
        /// </summary>
        VFT_STATIC_LIB  = 0x00000007,

        /// <summary>
        /// The file type is unknown to the system.
        /// </summary>
        VFT_UNKNOWN     = 0x00000000,

        /// <summary>
        /// The file contains a virtual device.
        /// </summary>
        VFT_VXD         = 0x00000005,
    }

    // https://docs.microsoft.com/en-us/windows/win32/api/verrsrc/ns-verrsrc-vs_fixedfileinfo
    public enum FileInfoFileSubtype : uint
    {
        #region VFT_DRV

        /// <summary>
        /// The file contains a communications driver.
        /// </summary>
        VFT2_DRV_COMM               = 0x0000000A,

        /// <summary>
        /// The file contains a display driver.
        /// </summary>
        VFT2_DRV_DISPLAY            = 0x00000004,

        /// <summary>
        /// The file contains an installable driver.
        /// </summary>
        VFT2_DRV_INSTALLABLE        = 0x00000008,

        /// <summary>
        /// The file contains a keyboard driver.
        /// </summary>
        VFT2_DRV_KEYBOARD           = 0x00000002,

        /// <summary>
        /// The file contains a language driver.
        /// </summary>
        VFT2_DRV_LANGUAGE           = 0x00000003,

        /// <summary>
        /// The file contains a mouse driver.
        /// </summary>
        VFT2_DRV_MOUSE              = 0x00000005,

        /// <summary>
        /// The file contains a network driver.
        /// </summary>
        VFT2_DRV_NETWORK            = 0x00000006,

        /// <summary>
        /// The file contains a printer driver.
        /// </summary>
        VFT2_DRV_PRINTER            = 0x00000001,

        /// <summary>
        /// The file contains a sound driver.
        /// </summary>
        VFT2_DRV_SOUND              = 0x00000009,

        /// <summary>
        /// The file contains a system driver.
        /// </summary>
        VFT2_DRV_SYSTEM             = 0x00000007,

        /// <summary>
        /// The file contains a versioned printer driver.
        /// </summary>
        VFT2_DRV_VERSIONED_PRINTER  = 0x0000000C,

        #endregion

        #region VFT_FONT

        /// <summary>
        /// The file contains a raster font.
        /// </summary>
        VFT2_FONT_RASTER            = 0x00000001,

        /// <summary>
        /// The file contains a TrueType font.
        /// </summary>
        VFT2_FONT_TRUETYPE          = 0x00000003,

        /// <summary>
        /// The file contains a vector font.
        /// </summary>
        VFT2_FONT_VECTOR            = 0x00000002,

        #endregion

        /// <summary>
        /// The driver type is unknown by the system.
        /// The font type is unknown by the system.
        /// </summary>
        VFT2_UNKNOWN                = 0x00000000,
    }

    // https://docs.microsoft.com/en-us/windows/win32/api/verrsrc/ns-verrsrc-vs_fixedfileinfo
    [Flags]
    public enum FileInfoOS : uint
    {
        /// <summary>
        /// The file was designed for MS-DOS.
        /// </summary>
        VOS_DOS             = 0x00010000,

        /// <summary>
        /// The file was designed for Windows NT.
        /// </summary>
        VOS_NT              = 0x00040000,

        /// <summary>
        /// The file was designed for 16-bit Windows.
        /// </summary>
        VOS__WINDOWS16      = 0x00000001,

        /// <summary>
        /// The file was designed for 32-bit Windows.
        /// </summary>
        VOS__WINDOWS32      = 0x00000004,

        /// <summary>
        /// The file was designed for 16-bit OS/2.
        /// </summary>
        VOS_OS216           = 0x00020000,

        /// <summary>
        /// The file was designed for 32-bit OS/2.
        /// </summary>
        VOS_OS232           = 0x00030000,

        /// <summary>
        /// The file was designed for 16-bit Presentation Manager.
        /// </summary>
        VOS__PM16           = 0x00000002,

        /// <summary>
        /// The file was designed for 32-bit Presentation Manager.
        /// </summary>
        VOS__PM32           = 0x00000003,

        /// <summary>
        /// The operating system for which the file was designed is unknown to the system.
        /// </summary>
        VOS_UNKNOWN         = 0x00000000,

        /// <summary>
        /// The file was designed for 16-bit Windows running on MS-DOS.
        /// </summary>
        VOS_DOS_WINDOWS16   = 0x00010001,

        /// <summary>
        /// The file was designed for 32-bit Windows running on MS-DOS.
        /// </summary>
        VOS_DOS_WINDOWS32   = 0x00010004,

        /// <summary>
        /// The file was designed for Windows NT.
        /// </summary>
        VOS_NT_WINDOWS32    = 0x00040004,

        /// <summary>
        /// The file was designed for 16-bit Presentation Manager running on 16-bit OS/2.
        /// </summary>
        VOS_OS216_PM16      = 0x00020002,

        /// <summary>
        /// The file was designed for 32-bit Presentation Manager running on 32-bit OS/2.
        /// </summary>
        VOS_OS232_PM32      = 0x00030003,
    }

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#optional-header-data-directories-image-only
    public enum ImageDirectory : byte
    {
        /// <summary>
        /// The export table address and size. (.edata)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_EXPORT            = 0,

        /// <summary>
        /// The import table address and size. (.idata)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_IMPORT            = 1,

        /// <summary>
        /// The resource table address and size. (.rsrc)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_RESOURCE          = 2,

        /// <summary>
        /// The exception table address and size. (.pdata)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_EXCEPTION         = 3,

        /// <summary>
        /// The attribute certificate table address and size.
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_SECURITY          = 4,

        /// <summary>
        /// The base relocation table address and size. (.reloc)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_BASERELOC         = 5,

        /// <summary>
        /// The debug data starting address and size. (.debug)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_DEBUG             = 6,

        /// <summary>
        /// Reserved, must be 0
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_ARCHITECTURE      = 7,

        /// <summary>
        /// The RVA of the value to be stored in the global pointer register.
        /// The size member of this structure must be set to zero.
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_GLOBALPTR         = 8,

        /// <summary>
        /// The thread local storage (TLS) table address and size. (.tls)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_TLS               = 9,

        /// <summary>
        /// The load configuration table address and size.
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG       = 10,

        /// <summary>
        /// The bound import table address and size.
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT      = 11,

        /// <summary>
        /// The import address table address and size.
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_IAT               = 12,

        /// <summary>
        /// The delay import descriptor address and size.
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT      = 13,

        /// <summary>
        /// The CLR runtime header address and size. (.cormeta)
        /// </summary>
        IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR    = 14,

        /// <summary>
        /// Reserved, must be zero
        /// </summary>
        IMAGE_DIRECTORY_RESERVED                = 15,
    }

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#characteristics
    [Flags]
    public enum ImageObjectCharacteristics : ushort
    {
        /// <summary>
        /// Image only, Windows CE, and Microsoft Windows NT and later.
        /// This indicates that the file does not contain base relocations and must therefore be loaded at its preferred base address.
        /// If the base address is not available, the loader reports an error.
        /// The default behavior of the linker is to strip base relocations from executable (EXE) files.
        /// </summary>
        IMAGE_FILE_RELOCS_STRIPPED          = 0x0001,

        /// <summary>
        /// Image only. This indicates that the image file is valid and can be run.
        /// If this flag is not set, it indicates a linker error.
        /// </summary>
        IMAGE_FILE_EXECUTABLE_IMAGE         = 0x0002,

        /// <summary>
        /// COFF line numbers have been removed.
        /// This flag is deprecated and should be zero.
        /// </summary>
        [Obsolete]
        IMAGE_FILE_LINE_NUMS_STRIPPED       = 0x0004,

        /// <summary>
        /// COFF symbol table entries for local symbols have been removed.
        /// This flag is deprecated and should be zero.
        /// </summary>
        [Obsolete]
        IMAGE_FILE_LOCAL_SYMS_STRIPPED      = 0x0008,

        /// <summary>
        /// Obsolete. Aggressively trim working set.
        /// This flag is deprecated for Windows 2000 and later and must be zero.
        /// </summary>
        [Obsolete]
        IMAGE_FILE_AGGRESSIVE_WS_TRIM       = 0x0010,

        /// <summary>
        /// Application can handle > 2-GB addresses.
        /// </summary>
        IMAGE_FILE_LARGE_ADDRESS_AWARE      = 0x0020,
        
        /// <summary>
        /// This flag is reserved for future use.
        /// </summary>
        Reserved64                          = 0x0040,

        /// <summary>
        /// Little endian: the least significant bit (LSB) precedes the most significant bit (MSB) in memory.
        /// This flag is deprecated and should be zero.
        /// </summary>
        [Obsolete]
        IMAGE_FILE_BYTES_REVERSED_LO        = 0x0080,

        /// <summary>
        /// Machine is based on a 32-bit-word architecture.
        /// </summary>
        IMAGE_FILE_32BIT_MACHINE            = 0x0100,

        /// <summary>
        /// Debugging information is removed from the image file.
        /// </summary>
        IMAGE_FILE_DEBUG_STRIPPED           = 0x0200,

        /// <summary>
        /// If the image is on removable media, fully load it and copy it to the swap file.
        /// </summary>
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP  = 0x0400,

        /// <summary>
        /// If the image is on network media, fully load it and copy it to the swap file.
        /// </summary>
        IMAGE_FILE_NET_RUN_FROM_SWAP        = 0x0800,

        /// <summary>
        /// The image file is a system file, not a user program.
        /// </summary>
        IMAGE_FILE_SYSTEM                   = 0x1000,

        /// <summary>
        /// The image file is a dynamic-link library (DLL).
        /// Such files are considered executable files for almost all purposes, although they cannot be directly run.
        /// </summary>
        IMAGE_FILE_DLL                      = 0x2000,

        /// <summary>
        /// The file should be run only on a uniprocessor machine.
        /// </summary>
        IMAGE_FILE_UP_SYSTEM_ONLY           = 0x4000,

        /// <summary>
        /// Big endian: the MSB precedes the LSB in memory.
        /// This flag is deprecated and should be zero.
        /// </summary>
        [Obsolete]
        IMAGE_FILE_BYTES_REVERSED_HI        = 0x8000,
    }

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#machine-types
    public enum MachineType : ushort
    {
        /// <summary>
        /// The content of this field is assumed to be applicable to any machine type
        /// </summary>
        IMAGE_FILE_MACHINE_UNKNOWN      = 0x0000,

        /// <summary>
        /// Matsushita AM33
        /// </summary>
        IMAGE_FILE_MACHINE_AM33         = 0x01d3,

        /// <summary>
        /// x64
        /// </summary>
        IMAGE_FILE_MACHINE_AMD64        = 0x8664,

        /// <summary>
        /// ARM little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARM          = 0x01c0,

        /// <summary>
        /// ARM64 little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARM64        = 0xaa64,

        /// <summary>
        /// ARM Thumb-2 little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARMNT        = 0x01c4,

        /// <summary>
        /// EFI byte code
        /// </summary>
        IMAGE_FILE_MACHINE_EBC          = 0x0ebc,

        /// <summary>
        /// Intel 386 or later processors and compatible processors
        /// </summary>
        IMAGE_FILE_MACHINE_I386         = 0x014c,

        /// <summary>
        /// Intel Itanium processor family
        /// </summary>
        IMAGE_FILE_MACHINE_IA64         = 0x0200,

        /// <summary>
        /// Mitsubishi M32R little endian
        /// </summary>
        IMAGE_FILE_MACHINE_M32R         = 0x9041,

        /// <summary>
        /// MIPS16
        /// </summary>
        IMAGE_FILE_MACHINE_MIPS16       = 0x0266,

        /// <summary>
        /// MIPS with FPU
        /// </summary>
        IMAGE_FILE_MACHINE_MIPSFPU      = 0x0366,

        /// <summary>
        /// MIPS16 with FPU
        /// </summary>
        IMAGE_FILE_MACHINE_MIPSFPU16    = 0x0466,

        /// <summary>
        /// Power PC little endian
        /// </summary>
        IMAGE_FILE_MACHINE_POWERPC      = 0x01f0,

        /// <summary>
        /// Power PC with floating point support
        /// </summary>
        IMAGE_FILE_MACHINE_POWERPCFP    = 0x01f1,

        /// <summary>
        /// MIPS little endian
        /// </summary>
        IMAGE_FILE_MACHINE_R4000        = 0x0166,

        /// <summary>
        /// RISC-V 32-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV32      = 0x5032,

        /// <summary>
        /// RISC-V 64-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV64      = 0x5064,

        /// <summary>
        /// RISC-V 128-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV128     = 0x5128,

        /// <summary>
        /// Hitachi SH3
        /// </summary>
        IMAGE_FILE_MACHINE_SH3          = 0x01a2,

        /// <summary>
        /// Hitachi SH3 DSP
        /// </summary>
        IMAGE_FILE_MACHINE_SH3DSP       = 0x01a3,

        /// <summary>
        /// Hitachi SH4
        /// </summary>
        IMAGE_FILE_MACHINE_SH4          = 0x01a6,

        /// <summary>
        /// Hitachi SH5
        /// </summary>
        IMAGE_FILE_MACHINE_SH5          = 0x01a8,

        /// <summary>
        /// Thumb
        /// </summary>
        IMAGE_FILE_MACHINE_THUMB        = 0x01c2,

        /// <summary>
        /// MIPS little-endian WCE v2
        /// </summary>
        IMAGE_FILE_MACHINE_WCEMIPSV2    = 0x0169,
    }

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#optional-header-image-only
    public enum OptionalHeaderType : ushort
    {
        /// <summary>
        /// ROM image
        /// </summary>
        ROMImage    = 0x107,

        /// <summary>
        /// Normal executable file
        /// </summary>
        PE32        = 0x10b,

        /// <summary>
        /// PE32+ images allow for a 64-bit address space while limiting the image size to 2 gigabytes.
        /// </summary>
        PE32Plus    = 0x20b,
    }

    /// http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm
    [Flags]
    public enum ResourceTableEntryFlags : ushort
    {
        /// <summary>
        /// Resource is not fixed.
        /// </summary>
        MOVEABLE = 0x0010,

        /// <summary>
        /// Resource can be shared.
        /// </summary>
        PURE = 0x0020,
        
        /// <summary>
        /// Resource is preloaded.
        /// </summary>
        PRELOAD = 0x0040,
    }

    /// <summary>
    /// Predefined Resource Types
    /// </summary>
    public enum ResourceTypes : ushort
    {
        RT_CURSOR = 1,
        RT_BITMAP = 2,
        RT_ICON = 3,
        RT_MENU = 4,
        RT_DIALOG = 5,
        RT_STRING = 6,
        RT_FONTDIR = 7,
        RT_FONT = 8,
        RT_ACCELERATOR = 9,
        RT_RCDATA = 10,
        RT_MESSAGELIST = 11, // RT_MESSAGETABLE
        RT_GROUP_CURSOR = 12,
        RT_RESERVED_1 = 13, // Undefined
        RT_GROUP_ICON = 14,
        RT_RESERVED_2 = 15, // Undefined
        RT_VERSION = 16,
        RT_DLGINCLUDE = 17,
        RT_PLUGPLAY = 19,
        RT_VXD = 20,
        RT_ANICURSOR = 21,

        RT_NEWRESOURCE = 0x2000,
        RT_NEWBITMAP = (RT_BITMAP |RT_NEWRESOURCE),
        RT_NEWMENU = (RT_MENU |RT_NEWRESOURCE),
        RT_NEWDIALOG = (RT_DIALOG |RT_NEWRESOURCE),
        RT_ERROR = 0x7fff,
    }

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#section-flags
    [Flags]
    public enum SectionCharacteristics : uint
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved0                               = 0x00000000,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved1                               = 0x00000001,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved2                               = 0x00000002,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved4                               = 0x00000004,
        
        /// <summary>
        /// The section should not be padded to the next boundary.
        /// This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES.
        /// This is valid only for object files.
        /// </summary>
        IMAGE_SCN_TYPE_NO_PAD                   = 0x00000008,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved16                              = 0x00000010,
        
        /// <summary>
        /// The section contains executable code.
        /// </summary>
        IMAGE_SCN_CNT_CODE                      = 0x00000020,
        
        /// <summary>
        /// The section contains initialized data.
        /// </summary>
        IMAGE_SCN_CNT_INITIALIZED_DATA          = 0x00000040,
        
        /// <summary>
        /// The section contains uninitialized data.
        /// </summary>
        IMAGE_SCN_CNT_UNINITIALIZED_DATA        = 0x00000080,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_LNK_OTHER                     = 0x00000100,
        
        /// <summary>
        /// The section contains comments or other information.
        /// The .drectve section has this type.
        /// This is valid for object files only.
        /// </summary>
        IMAGE_SCN_LNK_INFO                      = 0x00000200,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved1024                            = 0x00000400,
        
        /// <summary>
        /// The section will not become part of the image.
        /// This is valid only for object files.
        /// </summary>
        IMAGE_SCN_LNK_REMOVE                    = 0x00000800,
        
        /// <summary>
        /// The section contains COMDAT data.
        /// This is valid only for object files.
        /// </summary>
        IMAGE_SCN_LNK_COMDAT                    = 0x00001000,
        
        /// <summary>
        /// The section contains data referenced through the global pointer (GP).
        /// </summary>
        IMAGE_SCN_GPREL                         = 0x00008000,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_PURGEABLE                 = 0x00010000,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_16BIT                     = 0x00020000,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_LOCKED                    = 0x00040000,
        
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_PRELOAD                   = 0x00080000,
        
        /// <summary>
        /// Align data on a 1-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_1BYTES                  = 0x00100000,
        
        /// <summary>
        /// Align data on a 2-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_2BYTES                  = 0x00200000,
        
        /// <summary>
        /// Align data on a 4-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_4BYTES                  = 0x00300000,
        
        /// <summary>
        /// Align data on a 8-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_8BYTES                  = 0x00400000,
        
        /// <summary>
        /// Align data on a 16-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_16BYTES                 = 0x00500000,
        
        /// <summary>
        /// Align data on a 32-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_32BYTES                 = 0x00600000,
        
        /// <summary>
        /// Align data on a 64-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_64BYTES                 = 0x00700000,
        
        /// <summary>
        /// Align data on a 128-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_128BYTES                = 0x00800000,
        
        /// <summary>
        /// Align data on a 256-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_256BYTES                = 0x00900000,
        
        /// <summary>
        /// Align data on a 512-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_512BYTES                = 0x00A00000,
        
        /// <summary>
        /// Align data on a 1024-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_1024BYTES               = 0x00B00000,
        
        /// <summary>
        /// Align data on a 2048-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_2048BYTES               = 0x00C00000,
        
        /// <summary>
        /// Align data on a 4096-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_4096BYTES               = 0x00D00000,
        
        /// <summary>
        /// Align data on a 8192-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_8192BYTES               = 0x00E00000,
        
        /// <summary>
        /// The section contains extended relocations.
        /// </summary>
        IMAGE_SCN_LNK_NRELOC_OVFL               = 0x01000000,
        
        /// <summary>
        /// The section can be discarded as needed.
        /// </summary>
        IMAGE_SCN_MEM_DISCARDABLE               = 0x02000000,
        
        /// <summary>
        /// The section cannot be cached.
        /// </summary>
        IMAGE_SCN_MEM_NOT_CACHED                = 0x04000000,
        
        /// <summary>
        /// The section is not pageable.
        /// </summary>
        IMAGE_SCN_MEM_NOT_PAGED                 = 0x08000000,
        
        /// <summary>
        /// The section can be shared in memory.
        /// </summary>
        IMAGE_SCN_MEM_SHARED                    = 0x10000000,
        
        /// <summary>
        /// The section can be executed as code.
        /// </summary>
        IMAGE_SCN_MEM_EXECUTE                   = 0x20000000,
        
        /// <summary>
        /// The section can be read.
        /// </summary>
        IMAGE_SCN_MEM_READ                      = 0x40000000,
        
        /// <summary>
        /// The section can be written to.
        /// </summary>
        IMAGE_SCN_MEM_WRITE                     = 0x80000000,
    }
    
    /// http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm
    [Flags]
    public enum SegmentTableEntryFlags : ushort
    {
        /// <summary>
        /// Segment-type field.
        /// </summary>
        TYPE = 0x0007,
        
        /// <summary>
        /// Code-segment type.
        /// </summary>
        CODE = 0x0000,
        
        /// <summary>
        /// Data-segment type.
        /// </summary>
        DATA = 0x0001,
        
        /// <summary>
        /// Iterated segment flag
        /// </summary>
        ITER = 0x0008,
        
        /// <summary>
        /// Segment is not fixed.
        /// </summary>
        MOVEABLE = 0x0010,
        
        /// <summary>
        /// Shared segment flag
        /// </summary>
        SHARED = 0x0020,
        
        /// <summary>
        /// For compatibility
        /// </summary>
        PURE = 0x0020,
        
        /// <summary>
        /// Segment will be preloaded; read-only if this is a data segment.
        /// </summary>
        PRELOAD = 0x0040,
        
        /// <summary>
        /// Execute-only (code segment), or read-only (data segment)
        /// </summary>
        EXRD = 0x0080,
        
        /// <summary>
        /// Set if segment has relocation records.
        /// </summary>
        RELOCINFO = 0x0100,
        
        /// <summary>
        /// Conforming segment
        /// </summary>
        CONFORM = 0x0200,
        
        /// <summary>
        /// I/O privilege level (286 DPL bits)
        /// </summary>
        DPL = 0x0C00,
        
        /// <summary>
        /// Left shift count for SEGDPL field
        /// </summary>
        SHIFTDPL = 10,
        
        /// <summary>
        /// Discard priority.
        /// </summary>
        DISCARD = 0x1000,
        
        /// <summary>
        /// 32-bit code segment
        /// </summary>
        NS32BIT = 0x2000,
        
        /// <summary>
        /// Huge memory segment, length of segment and minimum allocation size are in segment sector units
        /// </summary>
        HUGE = 0x4000,
    }

    [Flags]
    public enum TargetOperatingSystems : byte
    {
        /// <summary>
        /// Unknown (any "new-format" OS)
        /// </summary>
        NE_UNKNOWN = 0x0,
        
        /// <summary>
        /// Microsoft/IBM OS/2 (default) 
        /// </summary>
        NE_OS2 = 0x1,
        
        /// <summary>
        /// Microsoft Windows
        /// </summary>
        NE_WINDOWS = 0x2,
        
        /// <summary>
        /// Microsoft MS-DOS 4.x
        /// </summary>
        NE_DOS4 = 0x3,
        
        /// <summary>
        /// Windows 386
        /// </summary>
        NE_WIN386 = 0x4,
    }

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#windows-subsystem
    public enum WindowsSubsystem : ushort
    {
        /// <summary>
        /// An unknown subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_UNKNOWN                     = 0,

        /// <summary>
        /// Device drivers and native Windows processes
        /// </summary>
        IMAGE_SUBSYSTEM_NATIVE                      = 1,

        /// <summary>
        /// The Windows graphical user interface (GUI) subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_GUI                 = 2,

        /// <summary>
        /// The Windows character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_CUI                 = 3,

        /// <summary>
        /// The OS/2 character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_OS2_CUI                     = 5,

        /// <summary>
        /// The Posix character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_POSIX_CUI                   = 7,

        /// <summary>
        /// Native Win9x driver
        /// </summary>
        IMAGE_SUBSYSTEM_NATIVE_WINDOWS              = 8,

        /// <summary>
        /// Windows CE
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI              = 9,

        /// <summary>
        /// An Extensible Firmware Interface (EFI) application
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_APPLICATION             = 10,

        /// <summary>
        /// An EFI driver with boot services
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER     = 11,

        /// <summary>
        /// An EFI driver with run-time services
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER          = 12,

        /// <summary>
        /// An EFI ROM image
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_ROM                     = 13,

        /// <summary>
        /// XBOX
        /// </summary>
        IMAGE_SUBSYSTEM_XBOX                        = 14,

        /// <summary>
        /// Windows boot application.
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION    = 16,
    }
}