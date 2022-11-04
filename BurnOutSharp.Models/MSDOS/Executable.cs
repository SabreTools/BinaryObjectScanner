namespace BurnOutSharp.Models.MSDOS
{
    /// <summary>
    /// The MS-DOS EXE format, also known as MZ after its signature (the initials of Microsoft engineer Mark Zbykowski),
    /// was introduced with MS-DOS 2.0 (version 1.0 only sported the simple COM format). It is designed as a relocatable
    /// executable running under real mode. As such, only DOS and Windows 9x can use this format natively, but there are
    /// several free DOS emulators (e.g., DOSBox) that support it and that run under various operating systems (e.g.,
    /// Linux, Amiga, Windows NT, etc.). Although they can exist on their own, MZ executables are embedded in all NE, LE,
    /// and PE executables, usually as stubs so that when they are ran under DOS, they display a warning.
    /// </summary>
    /// <see href="https://wiki.osdev.org/MZ"/>
    public class Executable
    {
        /// <summary>
        /// MS-DOS executable header
        /// </summary>
        public ExecutableHeader Header { get; set; }

        /// <summary>
        /// After loading the executable into memory, the program loader goes through
        /// every entry in relocation table. For each relocation entry, the loader
        /// adds the start segment address into word value pointed to by the
        /// segment:offset pair. So, for example, a relocation entry 0001:001A will
        /// make the loader add start segment address to the value at offset
        /// 1*0x10+0x1A=0x2A within the program data.
        /// </summary>
        public RelocationEntry[] RelocationTable { get; set; }
    }
}
