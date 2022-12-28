namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The structure and content of the import address table are identical to those of
    /// the import lookup table, until the file is bound. During binding, the entries in
    /// the import address table are overwritten with the 32-bit (for PE32) or 64-bit
    /// (for PE32+) addresses of the symbols that are being imported. These addresses are
    /// the actual memory addresses of the symbols, although technically they are still
    /// called "virtual addresses." The loader typically processes the binding.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class ImportAddressTableEntry
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
