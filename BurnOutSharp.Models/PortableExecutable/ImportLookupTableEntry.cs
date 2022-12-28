namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// An import lookup table is an array of 32-bit numbers for PE32 or an array of
    /// 64-bit numbers for PE32+. Each entry uses the bit-field format that is described
    /// in the following table. In this format, bit 31 is the most significant bit for
    /// PE32 and bit 63 is the most significant bit for PE32+. The collection of these
    /// entries describes all imports from a given DLL. The last entry is set to zero
    /// (NULL) to indicate the end of the table.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class ImportLookupTableEntry
    {
        /// <summary>
        /// If this bit is set, import by ordinal. Otherwise, import by name. Bit is
        /// masked as 0x80000000 for PE32, 0x8000000000000000 for PE32+. 
        /// </summary>
        /// <remarks>Bit 31/63</remarks>
        public bool OrdinalNameFlag;

        /// <summary>
        /// A 16-bit ordinal number. This field is used only if the Ordinal/Name Flag
        /// bit field is 1 (import by ordinal). Bits 30-15 or 62-15 must be 0.
        /// </summary>
        /// <remarks>Bits 15-0</remarks>
        public ushort OrdinalNumber;

        /// <summary>
        /// A 31-bit RVA of a hint/name table entry. This field is used only if the
        /// Ordinal/Name Flag bit field is 0 (import by name). For PE32+ bits 62-31
        /// must be zero.
        /// </summary>
        /// <remarks>Bits 30-0</remarks>
        public uint HintNameTableRVA;
    }
}
