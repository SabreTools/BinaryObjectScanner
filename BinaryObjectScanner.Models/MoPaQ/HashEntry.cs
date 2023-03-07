using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.MoPaQ
{
    /// <summary>
    /// Hash table is used for searching files by name. The file name is converted to
    /// two 32-bit hash values, which are then used for searching in the table. The size
    /// of the hash table must always be a power of two. Each entry in the hash table
    /// also contains file locale and offset into block table. Size of one entry of hash
    /// table is 16 bytes.
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class HashEntry
    {
        /// <summary>
        /// The hash of the full file name (part A)
        /// </summary>
        public uint NameHashPartA;

        /// <summary>
        /// The hash of the full file name (part B)
        /// </summary>
        public uint NameHashPartB;

        /// <summary>
        /// The language of the file. This is a Windows LANGID data type, and uses the same values.
        /// 0 indicates the default language (American English), or that the file is language-neutral.
        /// </summary>
        public Locale Locale;

        /// <summary>
        /// The platform the file is used for. 0 indicates the default platform.
        /// No other values have been observed.
        /// </summary>
        public ushort Platform;

        /// <summary>
        /// If the hash table entry is valid, this is the index into the block table of the file.
        /// Otherwise, one of the following two values:
        ///  - FFFFFFFFh: Hash table entry is empty, and has always been empty.
        ///               Terminates searches for a given file.
        ///  - FFFFFFFEh: Hash table entry is empty, but was valid at some point (a deleted file).
        ///               Does not terminate searches for a given file.
        /// </summary>
        public uint BlockIndex;
    }
}
