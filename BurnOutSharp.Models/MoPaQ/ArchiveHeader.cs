using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.MoPaQ
{
    /// <summary>
    /// MoPaQ archive header
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ArchiveHeader
    {
        #region V1 Properties

        /// <summary>
        /// The MPQ archive signature
        /// </summary>
        public uint Signature;

        /// <summary>
        /// Size of the archive header
        /// </summary>
        public uint HeaderSize;

        /// <summary>
        /// Size of MPQ archive
        /// </summary>
        /// <remarks>
        /// This field is deprecated in the Burning Crusade MoPaQ format, and the size of the archive
        /// is calculated as the size from the beginning of the archive to the end of the hash table,
        /// block table, or extended block table (whichever is largest).
        /// </remarks>
        public uint ArchiveSize;

        /// <summary>
        /// 0 = Format 1 (up to The Burning Crusade)
        /// 1 = Format 2 (The Burning Crusade and newer)
        /// 2 = Format 3 (WoW - Cataclysm beta or newer)
        /// 3 = Format 4 (WoW - Cataclysm beta or newer)
        /// </summary>
        public FormatVersion FormatVersion;

        /// <summary>
        /// Power of two exponent specifying the number of 512-byte disk sectors in each logical sector
        /// in the archive. The size of each logical sector in the archive is 512 * 2 ^ BlockSize.
        /// </summary>
        public ushort BlockSize;

        /// <summary>
        /// Offset to the beginning of the hash table, relative to the beginning of the archive.
        /// </summary>
        public uint HashTablePosition;

        /// <summary>
        /// Offset to the beginning of the block table, relative to the beginning of the archive.
        /// </summary>
        public uint BlockTablePosition;

        /// <summary>
        /// Number of entries in the hash table. Must be a power of two, and must be less than 2^16 for
        /// the original MoPaQ format, or less than 2^20 for the Burning Crusade format.
        /// </summary>
        public uint HashTableSize;

        /// <summary>
        /// Number of entries in the block table
        /// </summary>
        public uint BlockTableSize;

        #endregion

        #region V2 Properties

        /// <summary>
        /// Offset to the beginning of array of 16-bit high parts of file offsets.
        /// </summary>
        public ulong HiBlockTablePosition;

        /// <summary>
        /// High 16 bits of the hash table offset for large archives.
        /// </summary>
        public ushort HashTablePositionHi;

        /// <summary>
        /// High 16 bits of the block table offset for large archives.
        /// </summary>
        public ushort BlockTablePositionHi;

        #endregion

        #region V3 Properties

        /// <summary>
        /// 64-bit version of the archive size
        /// </summary>
        public ulong ArchiveSizeLong;

        /// <summary>
        /// 64-bit position of the BET table
        /// </summary>
        public ulong BetTablePosition;

        /// <summary>
        /// 64-bit position of the HET table
        /// </summary>
        public ulong HetTablePosition;

        #endregion

        #region V4 Properties

        /// <summary>
        /// Compressed size of the hash table
        /// </summary>
        public ulong HashTableSizeLong;

        /// <summary>
        /// Compressed size of the block table
        /// </summary>
        public ulong BlockTableSizeLong;

        /// <summary>
        /// Compressed size of the hi-block table
        /// </summary>
        public ulong HiBlockTableSize;

        /// <summary>
        /// Compressed size of the HET block
        /// </summary>
        public ulong HetTableSize;

        /// <summary>
        /// Compressed size of the BET block
        /// </summary>
        public ulong BetTablesize;

        /// <summary>
        /// Size of raw data chunk to calculate MD5.
        /// </summary>
        /// <remarks>MD5 of each data chunk follows the raw file data.</remarks>
        public uint RawChunkSize;

        /// <summary>
        /// MD5 of the block table before decryption
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        public byte[] BlockTableMD5;

        /// <summary>
        /// MD5 of the hash table before decryption
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        public byte[] HashTableMD5;

        /// <summary>
        /// MD5 of the hi-block table
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        public byte[] HiBlockTableMD5;

        /// <summary>
        /// MD5 of the BET table before decryption
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        public byte[] BetTableMD5;

        /// <summary>
        /// MD5 of the HET table before decryption
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        public byte[] HetTableMD5;

        /// <summary>
        /// MD5 of the MPQ header from signature to (including) HetTableMD5
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        public byte[] MpqHeaderMD5;

        #endregion
    }
}
