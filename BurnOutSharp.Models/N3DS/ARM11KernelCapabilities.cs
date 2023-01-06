namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// The kernel capability descriptors are passed to svcCreateProcess.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#ARM11_Kernel_Capabilities"/>
    public sealed class ARM11KernelCapabilities
    {
        /// <summary>
        /// Descriptors
        /// -------------------
        /// Pattern of bits 20-31	Type	Fields
        /// 0b1110xxxxxxxx Interrupt info	
        /// 0b11110xxxxxxx System call mask    Bits 24-26: System call mask table index; Bits 0-23: mask
        /// 0b1111110xxxxx Kernel release version  Bits 8-15: Major version; Bits 0-7: Minor version
        /// 0b11111110xxxx Handle table size   Bits 0-18: size
        /// 0b111111110xxx Kernel flags
        /// 0b11111111100x Map address range   Describes a memory mapping like the 0b111111111110 descriptor, but an entire range rather than a single page is mapped.Another 0b11111111100x descriptor must follow this one to denote the(exclusive) end of the address range to map.
        /// 0b111111111110	Map memory page Bits 0-19: page index to map(virtual address >> 12; the physical address is determined per-page according to Memory layout); Bit 20: Map read-only(otherwise read-write)
        /// 
        /// ARM11 Kernel Flags
        /// -------------------
        /// Bit	Description
        /// 0	Allow debug
        /// 1	Force debug
        /// 2	Allow non-alphanum
        /// 3	Shared page writing
        /// 4	Privilege priority
        /// 5	Allow main() args
        /// 6	Shared device memory
        /// 7	Runnable on sleep
        /// 8-11	Memory type(1: application, 2: system, 3: base)
        /// 12	Special memory
        /// 13	Process has access to CPU core 2 (New3DS only)
        /// </summary>
        public byte[][] Descriptors;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;
    }
}
