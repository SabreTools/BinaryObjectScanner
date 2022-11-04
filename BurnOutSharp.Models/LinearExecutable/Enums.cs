using System;

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
    public enum InformationBlockFlag : uint
    {
        /// <summary>
        /// Initialization (Only for DLL):
        /// 0 	Global
        /// 1 	Per-Process
        /// </summary>
        Initialization = 1 << 2,

        /// <summary>
        /// No internal fixup in exe image
        /// </summary>
        NoInternalFixup = 1 << 4,

        /// <summary>
        /// No internal fixup in exe image
        /// </summary>
        NoExternalFixup = 1 << 5,

        // TODO: Figure out this block of flags
        // 8, 9, 10 all have the same note:
        // 0 	Unknown
        // 1 	Incompatible with PM windowing
        // 2 	Compatible with PM windowing
        // 3 	Uses PM windowing API

        /// <summary>
        /// Module not loadable
        /// </summary>
        ModuleNotLoadable = 1 << 13,

        /// <summary>
        /// Module is DLL rather then program
        /// </summary>
        IsDLL = 1 << 15,

        // Bits 16-31 are all reserved
    }

    public enum OperatingSystem : ushort
    {
        /// <summary>
        /// OS/2
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
