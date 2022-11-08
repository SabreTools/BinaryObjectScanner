using System;

namespace BurnOutSharp.Models.NewExecutable
{
    [Flags]
    public enum FixedSegmentEntryFlag : byte
    {
        /// <summary>
        /// Set if the entry is exported.
        /// </summary>
        Exported = 0x01,

        /// <summary>
        /// Set if the entry uses a global (shared) data segments.
        /// </summary>
        /// <remarks>
        /// The first assembly-language instruction in the
        /// entry point prologue must be "MOV AX,data
        /// segment number". This may be set only for
        /// SINGLEDATA library modules.
        /// </remarks>
        Global = 0x02,
    }

    [Flags]
    public enum HeaderFlag : ushort
    {
        #region Program Flags

        NOAUTODATA = 0x0000,

        /// <summary>
        /// Shared automatic data segment
        /// </summary>
        SINGLEDATA = 0x0001,

        /// <summary>
        /// Instanced automatic data segment
        /// </summary>
        MULTIPLEDATA = 0x0002,

        /// <summary>
        /// Global initialization
        /// </summary>
        GlobalInitialization = 0x0004,

        /// <summary>
        /// Protected mode only
        /// </summary>
        ProtectedModeOnly = 0x0008,

        /// <summary>
        /// 8086 instructions
        /// </summary>
        Instructions8086 = 0x0010,

        /// <summary>
        /// 80286 instructions
        /// </summary>
        Instructions80286 = 0x0020,

        /// <summary>
        /// 80386 instructions
        /// </summary>
        Instructions80386 = 0x0040,

        /// <summary>
        /// 80x87 instructions
        /// </summary>
        Instructions80x87 = 0x0080,

        #endregion

        #region Application Flags

        /// <summary>
        /// Full screen (not aware of Windows/P.M. API)
        /// </summary>
        FullScreen = 0x0100,

        /// <summary>
        /// Compatible with Windows/P.M. API
        /// </summary>
        WindowsPMCompatible = 0x0200,

        /// <summary>
        /// Uses Windows/P.M. API
        /// </summary>
        WindowsPM = 0x0400,

        /// <summary>
        /// OS/2 family application
        /// </summary>
        OS2FamilyApplication = 0x0800,

        /// <summary>
        /// Unknown (Reserved?)
        /// </summary>
        UnknownReserved = 0x1000,

        /// <summary>
        /// Errors detected at link time, module will not load
        /// </summary>
        ErrorsDetectedAtLinkTime = 0x2000,

        /// <summary>
        /// Unknown (non-conforming program)
        /// </summary>
        UnknownNonConforming = 0x4000,

        /// <summary>
        /// Library module.
        /// The SS:SP information is invalid, CS:IP points
        /// to an initialization procedure that is called
        /// with AX equal to the module handle. This
        /// initialization procedure must perform a far
        /// return to the caller, with AX not equal to
        /// zero to indicate success, or AX equal to zero
        /// to indicate failure to initialize. DS is set
        /// to the library's data segment if the
        /// SINGLEDATA flag is set. Otherwise, DS is set
        /// to the caller's data segment.
        /// </summary>
        /// <remarks>
        /// A program or DLL can only contain dynamic
        /// links to executable files that have this
        /// library module flag set. One program cannot
        /// dynamic-link to another program.
        /// </remarks>
        LibraryModule = 0x8000,

        #endregion
    }

    [Flags]
    public enum MoveableSegmentEntryFlag : byte
    {
        /// <summary>
        /// Set if the entry is exported.
        /// </summary>
        Exported = 0x01,

        /// <summary>
        /// Set if the entry uses a global (shared) data segments.
        /// </summary>
        Global = 0x02,
    }

    public enum OperatingSystem : byte
    {
        OS2 = 0x01,
        WINDOWS = 0x02,
        EU_MSDOS4 = 0x03,
        WINDOWS_386 = 0x04,
        BOSS = 0x05,
    }

    [Flags]
    public enum OS2Flag : byte
    {
        /// <summary>
        /// Long filename support
        /// </summary>
        LongFilenameSupport = 0x01,

        /// <summary>
        /// 2.x protected mode
        /// </summary>
        ProtectedMode = 0x02,

        /// <summary>
        /// 2.x proportional fonts
        /// </summary>
        ProportionalFonts = 0x04,

        /// <summary>
        /// Executable has gangload area
        /// </summary>
        HasGangload = 0x08,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0xF0,
    }

    public enum OSFixupType : ushort
    {
        /// <summary>
        /// FIARQQ, FJARQQ
        /// </summary>
        FIARQQ = 0x0001,

        /// <summary>
        /// FISRQQ, FJSRQQ
        /// </summary>
        FISRQQ = 0x0002,

        /// <summary>
        /// FICRQQ, FJCRQQ
        /// </summary>
        FICRQQ = 0x0003,

        FIERQQ = 0x0004,

        FIDRQQ = 0x0005,

        FIWRQQ = 0x0006,
    }

    [Flags]
    public enum RelocationRecordFlag : byte
    {
        TARGET_MASK = 0x03,

        INTERNALREF = 0x00,

        IMPORTORDINAL = 0x01,

        IMPORTNAME = 0x02,

        OSFIXUP = 0x03,

        ADDITIVE = 0x04,
    }

    public enum RelocationRecordSourceType : byte
    {
        SOURCE_MASK = 0x0F,

        LOBYTE = 0x00,

        SEGMENT = 0x02,

        /// <summary>
        /// 32-bit pointer
        /// </summary>
        FAR_ADDR = 0x03,

        /// <summary>
        /// 16-bit offset
        /// </summary>
        OFFSET = 0x05,
    }

    [Flags]
    public enum ResourceTypeResourceFlag : ushort
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

    public enum SegmentEntryType
    {
        /// <summary>
        /// 000h - There is no entry data in an unused bundle. The next bundle
        /// follows this field. This is used by the linker to skip ordinal numbers.
        /// </summary>
        Unused,

        /// <summary>
        /// 001h-0FEh - Segment number for fixed segment entries. A fixed segment
        /// entry is 3 bytes long.
        /// </summary>
        FixedSegment,

        /// <summary>
        /// 0FFH - Moveable segment entries. The entry data contains the segment
        /// number for the entry points. A moveable segment entry is 6 bytes long.
        /// </summary>
        MoveableSegment,
    }

    [Flags]
    public enum SegmentTableEntryFlag : ushort
    {
        /// <summary>
        /// Segment-type field.
        /// </summary>
        TYPE_MASK = 0x0007,

        /// <summary>
        /// Code-segment type.
        /// </summary>
        CODE = 0x0000,

        /// <summary>
        /// Data-segment type.
        /// </summary>
        DATA = 0x0001,

        /// <summary>
        /// Segment is not fixed.
        /// </summary>
        MOVEABLE = 0x0010,

        /// <summary>
        /// Segment will be preloaded; read-only if this is a data segment.
        /// </summary>
        PRELOAD = 0x0040,

        /// <summary>
        /// Set if segment has relocation records.
        /// </summary>
        RELOCINFO = 0x0100,

        /// <summary>
        /// Discard priority.
        /// </summary>
        DISCARD = 0xF000,
    }
}
