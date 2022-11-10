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
    public class ImportAddressTableEntry
    {
        /// <summary>
        /// 32-bit address of the symbol being imported
        /// </summary>
        public uint Address_PE32;

        /// <summary>
        /// 64-bit address of the symbol being imported
        /// </summary>
        public ulong Address_PE32Plus;
    }
}
