﻿using System;

namespace BurnOutSharp.Models.LinearExecutable
{
    public enum ByteOrder : byte
    {
        /// <summary>
        /// little-endian
        /// </summary>
        LE = 0x00,

        /// <summary>
        /// big-endian
        /// </summary>
        /// <remarks>non-zero</remarks>
        BE = 0x01,
    }

    public enum CPUType : ushort
    {
        /// <summary>
        /// Intel 80286 or upwardly compatible
        /// </summary>
        Intel80286 = 0x01,

        /// <summary>
        /// Intel 80386 or upwardly compatible
        /// </summary>
        Intel80386 = 0x02,

        /// <summary>
        /// Intel 80486 or upwardly compatible
        /// </summary>
        Intel80486 = 0x03,

        /// <summary>
        /// Intel 80586 or upwardly compatible
        /// </summary>
        Intel80586 = 0x04,

        /// <summary>
        /// Intel i860 (N10) or compatible
        /// </summary>
        Inteli860 = 0x20,

        /// <summary>
        /// Intel "N11" or compatible
        /// </summary>
        IntelN11 = 0x21,

        /// <summary>
        /// MIPS Mark I (R2000, R3000) or compatible
        /// </summary>
        MIPSMarkI = 0x40,

        /// <summary>
        /// MIPS Mark II ( R6000 ) or compatible
        /// </summary>
        MIPSMarkII = 0x41,

        /// <summary>
        /// MIPS Mark III ( R4000 ) or compatible
        /// </summary>
        MIPSMarkIII = 0x42,
    }

    [Flags]
    public enum ModuleFlags : uint
    {
        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved0 = 0x00000001,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved1 = 0x00000002,

        /// <summary>
        /// Per-Process Library Initialization.
        /// </summary>
        /// <remarks>
        /// The setting of this bit requires the EIP Object # and EIP fields
        /// to have valid values. If the EIP Object # and EIP fields are
        /// valid and this bit is NOT set, then Global Library Initialization
        /// is assumed. Setting this bit for an EXE file is invalid. 
        /// </remarks>
        Initialization = 0x00000004,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved3 = 0x00000008,

        /// <summary>
        /// Internal fixups for the module have been applied. 
        /// </summary>
        /// <remarks>
        /// The setting of this bit in a Linear Executable Module indicates that
        /// each object of the module has a preferred load address specified in
        /// the Object Table Reloc Base Addr. If the module's objects can not be
        /// loaded at these preferred addresses, then the relocation records that
        /// have been retained in the file data will be applied. 
        /// </remarks>
        InternalFixupsApplied = 0x00000010,

        /// <summary>
        /// External fixups for the module have been applied.
        /// </summary>
        ExternalFixupsApplied = 0x00000020,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved6 = 0x00000040,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved7 = 0x00000080,

        /// <summary>
        /// Incompatible with PM windowing.
        /// </summary>
        IncompatibleWithPMWindowing = 0x00000100,

        /// <summary>
        /// Incompatible with PM windowing.
        /// </summary>
        CompatibleWithPMWindowing = 0x00000200,

        /// <summary>
        /// Uses PM windowing API.
        /// </summary>
        UsesPMWindowing = 0x00000300,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved10 = 0x00000400,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved11 = 0x00000800,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved12 = 0x00001000,

        /// <summary>
        /// Module is not loadable.
        /// </summary>
        /// <remarks>
        /// When the 'Module is not loadable' flag is set, it indicates that
        /// either errors were detected at link time or that the module is
        /// being incrementally linked and therefore can't be loaded. 
        /// </remarks>
        ModuleNotLoadable = 0x00002000,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved14 = 0x00004000,

        /// <summary>
        /// Module type mask.
        /// </summary>
        ModuleTypeMask = 0x00038000,

        /// <summary>
        /// Program module.
        /// </summary>
        /// <remarks>
        /// A module can not contain dynamic links to other modules that have
        /// the 'program module' type. 
        /// </remarks>
        ProgramModule = 0x00000000,

        /// <summary>
        /// Library module.
        /// </summary>
        LibraryModule = 0x00008000,

        /// <summary>
        /// Protected Memory Library module.
        /// </summary>
        ProtectedMemoryLibraryModule = 0x00018000,

        /// <summary>
        /// Physical Device Driver module.
        /// </summary>
        PhysicalDeviceDriverModule = 0x00020000,

        /// <summary>
        /// Virtual Device Driver module.
        /// </summary>
        VirtualDeviceDriverModule = 0x00028000,

        /// <summary>
        /// Per-process Library Termination.
        /// </summary>
        /// <remarks>
        /// The setting of this bit requires the EIP Object # and EIP fields
        /// to have valid values. If the EIP Object # and EIP fields are
        /// valid and this bit is NOT set, then Global Library Termination
        /// is assumed. Setting this bit for an EXE file is invalid. 
        /// </remarks>
        PerProcessLibraryTermination = 0x40000000,
    }

    [Flags]
    public enum ObjectFlags : ushort
    {
        /// <summary>
        /// Readable Object.
        /// </summary>
        ReadableObject = 0x0001,

        /// <summary>
        /// Writable Object.
        /// </summary>
        WritableObject = 0x0002,

        /// <summary>
        /// Executable Object.
        /// </summary>
        ExecutableObject = 0x0004,

        // The readable, writable and executable flags provide support for all possible
        // protections. In systems where all of these protections are not supported,
        // the loader will be responsible for making the appropriate protection match
        // for the system. 

        /// <summary>
        /// Resource Object.
        /// </summary>
        ResourceObject = 0x0008,

        /// <summary>
        /// Discardable Object.
        /// </summary>
        DiscardableObject = 0x0010,

        /// <summary>
        /// Object is Shared.
        /// </summary>
        Shared = 0x0020,

        /// <summary>
        /// Object has Preload Pages.
        /// </summary>
        HasPreloadPages = 0x0040,

        /// <summary>
        /// Object has Invalid Pages.
        /// </summary>
        HasInvalidPages = 0x0080,

        /// <summary>
        /// Object has Zero Filled Pages.
        /// </summary>
        HasZeroFilledPages = 0x0100,

        /// <summary>
        /// Object is Resident (valid for VDDs, PDDs only).
        /// </summary>
        Resident = 0x0200,

        /// <summary>
        /// Object is Resident & Contiguous (VDDs, PDDs only).
        /// </summary>
        ResidentAndContiguous = 0x0300,

        /// <summary>
        /// Object is Resident & 'long-lockable' (VDDs, PDDs only).
        /// </summary>
        ResidentAndLongLockable = 0x0400,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        Reserved = 0x0800,

        /// <summary>
        /// 16:16 Alias Required (80x86 Specific).
        /// </summary>
        AliasRequired = 0x1000,

        /// <summary>
        /// Big/Default Bit Setting (80x86 Specific).
        /// </summary>
        BitSetting = 0x2000,

        // The 'big/default' bit, for data segments, controls the setting of the
        // Big bit in the segment descriptor. (The Big bit, or B-bit, determines
        // whether ESP or SP is used as the stack pointer.) For code segments,
        // this bit controls the setting of the Default bit in the segment
        // descriptor. (The Default bit, or D-bit, determines whether the default
        // word size is 32-bits or 16-bits. It also affects the interpretation of
        // the instruction stream.) 

        /// <summary>
        /// Object is conforming for code (80x86 Specific).
        /// </summary>
        Conforming = 0x4000,

        /// <summary>
        /// Object I/O privilege level (80x86 Specific). Only used for 16:16 Alias Objects.
        /// </summary>
        PrivilegeLevel = 0x8000,
    }

    [Flags]
    public enum ObjectPageFlags : ushort
    {
        /// <summary>
        /// Legal Physical Page in the module (Offset from Preload Page Section).
        /// </summary>
        LegalPhysicalPage = 0x0000,

        /// <summary>
        /// Iterated Data Page (Offset from Iterated Data Pages Section).
        /// </summary>
        IteratedDataPage = 0x0001,

        /// <summary>
        /// Invalid Page (zero).
        /// </summary>
        InvalidPage = 0x0002,

        /// <summary>
        /// Zero Filled Page (zero).
        /// </summary>
        ZeroFilledPage = 0x0003,

        /// <summary>
        /// Range of Pages.
        /// </summary>
        RangeOfPages = 0x0004,
    }

    public enum OperatingSystem : ushort
    {
        /// <summary>
        /// Unknown (any "new-format" OS) 
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// OS/2 (default)
        /// </summary>
        OS2 = 0x01,

        /// <summary>
        /// Windows
        /// </summary>
        Windows = 0x02,

        /// <summary>
        /// DOS 4.x
        /// </summary>
        DOS4x = 0x03,

        /// <summary>
        /// Windows 386
        /// </summary>
        Windows386 = 0x04,
    }

    public enum WordOrder : byte
    {
        /// <summary>
        /// little-endian
        /// </summary>
        LE = 0x00,

        /// <summary>
        /// big-endian
        /// </summary>
        /// <remarks>non-zero</remarks>
        BE = 0x01,
    }
}
