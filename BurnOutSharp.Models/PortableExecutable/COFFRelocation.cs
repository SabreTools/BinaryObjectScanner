using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Object files contain COFF relocations, which specify how the section data
    /// should be modified when placed in the image file and subsequently loaded
    /// into memory.
    /// 
    /// Image files do not contain COFF relocations, because all referenced symbols
    /// have already been assigned addresses in a flat address space. An image
    /// contains relocation information in the form of base relocations in the
    /// .reloc section (unless the image has the IMAGE_FILE_RELOCS_STRIPPED attribute).
    /// 
    /// For each section in an object file, an array of fixed-length records holds
    /// the section's COFF relocations. The position and length of the array are
    /// specified in the section header.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Sequential)]
    public class COFFRelocation
    {
        /// <summary>
        /// The address of the item to which relocation is applied. This is the
        /// offset from the beginning of the section, plus the value of the
        /// section's RVA/Offset field. See Section Table (Section Headers).
        /// For example, if the first byte of the section has an address of 0x10,
        /// the third byte has an address of 0x12.
        /// </summary>
        public uint VirtualAddress;

        /// <summary>
        /// A zero-based index into the symbol table. This symbol gives the address
        /// that is to be used for the relocation. If the specified symbol has section
        /// storage class, then the symbol's address is the address with the first
        /// section of the same name.
        /// </summary>
        public uint SymbolTableIndex;

        /// <summary>
        /// A value that indicates the kind of relocation that should be performed.
        /// Valid relocation types depend on machine type.
        /// </summary>
        public RelocationType TypeIndicator;
    }
}
