/*
 *	  NEWEXE.H (C) Copyright Microsoft Corp 1984-1987
 *
 *	  Data structure definitions for the OS/2 & Windows
 *	  executable file format.
 *
 *	  Modified by IVS on 24-Jan-1991 for Resource DeCompiler
 *	  (C) Copyright IVS 1991
 *
 *    http://csn.ul.ie/~caolan/pub/winresdump/winresdump/newexe.h
 */

using System;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    internal enum ExecutableType
    {
        Unknown,
        NE,
        PE,
    }

    /// <summary>
    /// Format of NE_FLAGS(x):
    /// 
    /// p                   Not-a-process
    ///  x                  Unused
    ///   e                 Errors in image
    ///    x                Unused
    ///     b               Bound as family app
    ///      ttt            Application type
    ///         f           Floating-point instructions
    ///          3          386 instructions
    ///           2         286 instructions
    ///            0        8086 instructions
    ///             P       Protected mode only
    ///              p      Per-process library initialization
    ///               i     Instance data
    ///                s    Solo data
    /// </summary>
    [Flags]
    internal enum NeFlags : ushort
    {
        /// <summary>
        /// Not a process
        /// </summary>
        NENOTP = 0x8000,
        
        /// <summary>
        /// Errors in image
        /// </summary>
        NEIERR = 0x2000,
        
        /// <summary>
        /// Bound as family app
        /// </summary>
        NEBOUND = 0x0800,
        
        /// <summary>
        /// Application type mask
        /// </summary>
        NEAPPTYP = 0x0700,
        
        /// <summary>
        /// Not compatible with P.M. Windowing
        /// </summary>
        NENOTWINCOMPAT = 0x0100,
        
        /// <summary>
        /// Compatible with P.M. Windowing
        /// </summary>
        NEWINCOMPAT = 0x0200,
        
        /// <summary>
        /// Uses P.M. Windowing API
        /// </summary>
        NEWINAPI = 0x0300,
        
        /// <summary>
        /// Floating-point instructions
        /// </summary>
        NEFLTP = 0x0080,
        
        /// <summary>
        /// 386 instructions
        /// </summary>
        NEI386 = 0x0040,
        
        /// <summary>
        /// 286 instructions
        /// </summary>
        NEI286 = 0x0020,
        
        /// <summary>
        /// 8086 instructions
        /// </summary>
        NEI086 = 0x0010,
        
        /// <summary>
        /// Runs in protected mode only
        /// </summary>
        NEPROT = 0x0008,
        
        /// <summary>
        /// Per-Process Library Initialization
        /// </summary>
        NEPPLI = 0x0004,
        
        /// <summary>
        /// Instance data
        /// </summary>
        NEINST = 0x0002,
        
        /// <summary>
        /// Solo data
        /// </summary>
        NESOLO = 0x0001,
    }

    /// <summary>
    ///  Format of NR_FLAGS(x):
    ///
    ///  xxxxx       Unused
    ///       a      Additive fixup
    ///        rr    Reference type
    /// </summary>
    [Flags]
    internal enum NrFlags : byte
    {
        /// <summary>
        /// Additive fixup
        /// </summary>
        NRADD = 0x04,
        
        /// <summary>
        /// Reference type mask
        /// </summary>
        NRRTYP = 0x03,
        
        /// <summary>
        /// Internal reference
        /// </summary>
        NRRINT = 0x00,
        
        /// <summary>
        /// Import by ordinal
        /// </summary>
        NRRORD = 0x01,
        
        /// <summary>
        /// Import by name
        /// </summary>
        NRRNAM = 0x02,
        
        /// <summary>
        /// Operating system fixup
        /// </summary>
        NRROSF = 0x03,
    }

    /// <summary>
    ///  Format of NR_STYPE(x):
    ///
    ///  xxxxx       Unused
    ///       sss    Source type
    ////
    /// </summary>
    [Flags]
    internal enum NrStype : byte
    {
        /// <summary>
        /// Source type mask
        /// </summary>
        NRSTYP = 0x0f,
        
        /// <summary>
        /// lo byte
        /// </summary>
        NRSBYT = 0x00,
        
        /// <summary>
        /// 16-bit segment
        /// </summary>
        NRSSEG = 0x02,
        
        /// <summary>
        /// 32-bit pointer
        /// </summary>
        NRSPTR = 0x03,
        
        /// <summary>
        /// 16-bit offset
        /// </summary>
        NRSOFF = 0x05,
        
        /// <summary>
        /// 48-bit pointer
        /// </summary>
        NRSPTR48 = 0x0B,
        
        /// <summary>
        /// 32-bit offset
        /// </summary>
        NRSOFF32 = 0x0D,
    }

    /// <summary>
    /// Format of NS_FLAGS(x)
    /// 
    /// x                   Unused
    ///  h                  Huge segment
    ///   c                 32-bit code segment
    ///    d                Discardable segment
    ///     DD              I/O privilege level (286 DPL bits)
    ///       c             Conforming segment
    ///        r            Segment has relocations
    ///         e           Execute/read only
    ///          p          Preload segment
    ///           P         Pure segment
    ///            m        Movable segment
    ///             i       Iterated segment
    ///              ttt    Segment type
    /// </summary>
    [Flags]
    internal enum NsFlags : ushort
    {
        /// <summary>
        /// Segment type mask
        /// </summary>
        NSTYPE = 0x0007,
        
        /// <summary>
        /// Code segment
        /// </summary>
        NSCODE = 0x0000,
        
        /// <summary>
        /// Data segment
        /// </summary>
        NSDATA = 0x0001,
        
        /// <summary>
        /// Iterated segment flag
        /// </summary>
        NSITER = 0x0008,
        
        /// <summary>
        /// Movable segment flag
        /// </summary>
        NSMOVE = 0x0010,
        
        /// <summary>
        /// Shared segment flag
        /// </summary>
        NSSHARED = 0x0020,
        
        /// <summary>
        /// For compatibility
        /// </summary>
        NSPURE = 0x0020,
        
        /// <summary>
        /// Preload segment flag
        /// </summary>
        NSPRELOAD = 0x0040,
        
        /// <summary>
        /// Execute-only (code segment), or read-only (data segment)
        /// </summary>
        NSEXRD = 0x0080,
        
        /// <summary>
        /// Segment has relocations
        /// </summary>
        NSRELOC = 0x0100,
        
        /// <summary>
        /// Conforming segment
        /// </summary>
        NSCONFORM = 0x0200,
        
        /// <summary>
        /// I/O privilege level (286 DPL bits)
        /// </summary>
        NSDPL = 0x0C00,
        
        /// <summary>
        /// Left shift count for SEGDPL field
        /// </summary>
        SHIFTDPL = 10,
        
        /// <summary>
        /// Segment is discardable
        /// </summary>
        NSDISCARD = 0x1000,
        
        /// <summary>
        /// 32-bit code segment
        /// </summary>
        NS32BIT = 0x2000,
        
        /// <summary>
        /// Huge memory segment, length of segment and minimum allocation size are in segment sector units
        /// </summary>
        NSHUGE = 0x4000,
    }

    /// <summary>
    /// Predefined Resource Types
    /// </summary>
    internal enum ResourceTypes : ushort
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

    [Flags]
    internal enum SectionCharacteristics : uint
    {
        CodeSection = 0x00000020,
        InitializedDataSection = 0x00000040,
        UninitializedDataSection = 0x00000080,
        SectionCannotBeCached = 0x04000000,
        SectionIsNotPageable = 0x08000000,
        SectionIsShared = 0x10000000,
        ExecutableSection = 0x20000000,
        ReadableSection = 0x40000000,
        WritableSection = 0x80000000,
    }

    [Flags]
    internal enum TargetOperatingSystems : byte
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
}