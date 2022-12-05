namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Type or Offset field entry is a WORD (2 bytes).
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class BaseRelocationTypeOffsetFieldEntry
    {
        /// <summary>
        /// Stored in the high 4 bits of the WORD, a value that indicates the type
        /// of base relocation to be applied. For more information, see <see cref="BaseRelocationTypes"/>
        /// </summary>
        public BaseRelocationTypes BaseRelocationType;

        /// <summary>
        /// Stored in the remaining 12 bits of the WORD, an offset from the starting
        /// address that was specified in the Page RVA field for the block. This
        /// offset specifies where the base relocation is to be applied.
        /// </summary>
        public ushort Offset;
    }
}
