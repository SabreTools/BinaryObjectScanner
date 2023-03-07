using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.MoPaQ
{
    /// <summary>
    /// The HET table is present if the HetTablePos64 member of MPQ header is
    /// set to nonzero. This table can fully replace hash table. Depending on
    /// MPQ size, the pair of HET&BET table can be more efficient than Hash&Block
    /// table. HET table can be encrypted and compressed.
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class HetTable
    {
        // TODO: Extract this out and make in common between HET and BET
        #region Common Table Headers

        /// <summary>
        /// 'HET\x1A'
        /// </summary>
        public string Signature;

        /// <summary>
        /// Version. Seems to be always 1
        /// </summary>
        public uint Version;

        /// <summary>
        /// Size of the contained table
        /// </summary>
        public uint DataSize;

        #endregion

        /// <summary>
        /// Size of the entire hash table, including the header (in bytes)
        /// </summary>
        public uint TableSize;

        /// <summary>
        /// Maximum number of files in the MPQ
        /// </summary>
        public uint MaxFileCount;

        /// <summary>
        /// Size of the hash table (in bytes)
        /// </summary>
        public uint HashTableSize;

        /// <summary>
        /// Effective size of the hash entry (in bits)
        /// </summary>
        public uint HashEntrySize;

        /// <summary>
        /// Total size of file index (in bits)
        /// </summary>
        public uint TotalIndexSize;

        /// <summary>
        /// Extra bits in the file index
        /// </summary>
        public uint IndexSizeExtra;

        /// <summary>
        /// Effective size of the file index (in bits)
        /// </summary>
        public uint IndexSize;

        /// <summary>
        /// Size of the block index subtable (in bytes)
        /// </summary>
        public uint BlockTableSize;

        /// <summary>
        /// HET hash table. Each entry is 8 bits.
        /// </summary>
        /// <remarks>Size is derived from HashTableSize</remarks>
        public byte[] HashTable;

        /// <summary>
        /// Array of file indexes. Bit size of each entry is taken from dwTotalIndexSize.
        /// Table size is taken from dwHashTableSize.
        /// </summary>
        public byte[][] FileIndexes;
    }
}
