namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The base relocation table contains entries for all base relocations in
    /// the image. The Base Relocation Table field in the optional header data
    /// directories gives the number of bytes in the base relocation table. For
    /// more information, see Optional Header Data Directories (Image Only).
    /// The base relocation table is divided into blocks. Each block represents
    /// the base relocations for a 4K page. Each block must start on a 32-bit boundary.
    /// 
    /// The loader is not required to process base relocations that are resolved by
    /// the linker, unless the load image cannot be loaded at the image base that is
    /// specified in the PE header.
    /// 
    /// To apply a base relocation, the difference is calculated between the preferred
    /// base address and the base where the image is actually loaded. If the image is
    /// loaded at its preferred base, the difference is zero and thus the base
    /// relocations do not have to be applied.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class BaseRelocationBlock
    {
        /// <summary>
        /// The image base plus the page RVA is added to each offset to create
        /// the VA where the base relocation must be applied.
        /// </summary>
        public uint PageRVA;

        /// <summary>
        /// The total number of bytes in the base relocation block, including
        /// the Page RVA and Block Size fields and the Type/Offset fields that
        /// follow.
        /// </summary>
        public uint BlockSize;

        /// <summary>
        /// The Block Size field is then followed by any number of Type or Offset
        /// field entries. Each entry is a WORD (2 bytes) and has the following
        /// structure:
        /// 
        /// 4 bits - Type -    Stored in the high 4 bits of the WORD, a value
        ///                    that indicates the type of base relocation to be
        ///                    applied. For more information, see <see cref="BaseRelocationTypes"/>
        /// 12 bits - Offset - Stored in the remaining 12 bits of the WORD, an
        ///                    offset from the starting address that was specified
        ///                    in the Page RVA field for the block. This offset
        ///                    specifies where the base relocation is to be applied. 
        /// </summary>
        public BaseRelocationTypeOffsetFieldEntry[] TypeOffsetFieldEntries;
    }
}
