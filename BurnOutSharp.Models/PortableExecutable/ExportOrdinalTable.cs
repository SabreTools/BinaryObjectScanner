namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The export ordinal table is an array of 16-bit unbiased indexes into the export address table.
    /// Ordinals are biased by the Ordinal Base field of the export directory table. In other words,
    /// the ordinal base must be subtracted from the ordinals to obtain true indexes into the export
    /// address table.
    /// 
    /// The export name pointer table and the export ordinal table form two parallel arrays that are
    /// separated to allow natural field alignment. These two tables, in effect, operate as one table,
    /// in which the Export Name Pointer column points to a public (exported) name and the Export
    /// Ordinal column gives the corresponding ordinal for that public name. A member of the export
    /// name pointer table and a member of the export ordinal table are associated by having the same
    /// position (index) in their respective arrays.
    /// 
    /// Thus, when the export name pointer table is searched and a matching string is found at position
    /// i, the algorithm for finding the symbol's RVA and biased ordinal is:
    /// 
    ///     i = Search_ExportNamePointerTable(name);
    ///     ordinal = ExportOrdinalTable[i];
    /// 
    ///     rva = ExportAddressTable[ordinal];
    ///     biased_ordinal = ordinal + OrdinalBase;
    /// 
    /// When searching for a symbol by(biased) ordinal, the algorithm for finding the symbol's RVA
    /// and name is:
    /// 
    ///     ordinal = biased_ordinal - OrdinalBase;
    ///     i = Search_ExportOrdinalTable(ordinal);
    /// 
    ///     rva = ExportAddressTable[ordinal];
    ///     name = ExportNameTable[i];
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class ExportOrdinalTable
    {
        /// <summary>
        /// An array of 16-bit unbiased indexes into the export address table
        /// </summary>
        public ushort[] Indexes;
    }
}
