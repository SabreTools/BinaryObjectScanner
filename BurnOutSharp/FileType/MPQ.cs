using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;
using StormLibSharp;

namespace BurnOutSharp.FileType
{
    public class MPQ : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte?[] { 0x4d, 0x50, 0x51, 0x1a }))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the mpq file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (MpqArchive mpqArchive = new MpqArchive(file, FileAccess.Read))
                {
                    // Try to open the listfile
                    string listfile = null;
                    MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");
                   
                    // If we can't read the listfile, we just return
                    if (!listStream.CanRead)
                        return null;

                    // Read the listfile in for processing
                    using (StreamReader sr = new StreamReader(listStream))
                    {
                        listfile = sr.ReadToEnd();
                    }

                    // Split the listfile by newlines
                    string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                    // Loop over each entry
                    foreach (string sub in listfileLines)
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(tempPath, sub);
                            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                            mpqArchive.ExtractFile(sub, tempFile);
                        }
                        catch (Exception ex)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine(ex);
                        }
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }

        // http://zezula.net/en/mpq/mpqformat.html
        #region TEMPORARY AREA FOR MPQ FORMAT

        /// <summary>
        /// MPQ (MoPaQ) is an archive format developed by Blizzard Entertainment,
        /// purposed for storing data files, images, sounds, music and videos for
        /// their games. The name MoPaQ comes from the author of the format,
        /// Mike O'Brien (Mike O'brien PaCK).
        /// </summary>
        internal class MoPaQArchive
        {
            #region Constants

            #region Header Sizes

            public const int HeaderVersion1Size = 0x20;

            public const int HeaderVersion2Size = 0x2C;

            public const int HeaderVersion3Size = 0x44;

            public const int HeaderVersion4Size = 0xD0;

            #endregion

            #region Signatures

            /// <summary>
            /// Human-readable signature
            /// </summary>
            public static readonly string SignatureString = $"MPQ{(char)0x1A}";

            /// <summary>
            /// Signature as an unsigned Int32 value
            /// </summary>
            public const uint SignatureValue = 0x1A51504D;

            /// <summary>
            /// Signature as a byte array
            /// </summary>
            public static readonly byte[] SignatureBytes = new byte[] { 0x4D, 0x50, 0x51, 0x1A };

            #endregion

            #endregion

            #region Properties

            // Data before archive, ignored

            /// <summary>
            /// MPQ User Data (optional)
            /// </summary>
            public MoPaQUserData UserData { get; private set; }

            /// <summary>
            /// MPQ Header (required)
            /// </summary>
            public MoPaQArchiveHeader ArchiveHeader { get; private set; }

            // Files (optional)
            // Special files (optional)

            /// <summary>
            /// HET Table (optional)
            /// </summary>
            public MoPaQHetTable HetTable { get; private set; }

            /// <summary>
            /// BET Table (optional)
            /// </summary>
            public MoPaQBetTable BetTable { get; private set; }

            /// <summary>
            /// Hash Table (optional)
            /// </summary>
            public MoPaQHashEntry[] HashTable { get; private set; }

            /// <summary>
            /// Block Table (optional)
            /// </summary>
            public MoPaQBlockEntry[] BlockTable { get; private set; }

            /// <summary>
            /// Hi-Block Table (optional)
            /// </summary>
            /// <remarks>
            /// Since World of Warcraft - The Burning Crusade, Blizzard extended
            /// the MPQ format to support archives larger than 4GB. The hi-block
            /// table holds the higher 16-bits of the file position in the MPQ.
            /// Hi-block table is plain array of 16-bit values. This table is
            /// not encrypted.
            /// </remarks>
            public short[] HiBlockTable { get; private set; }

            // Strong digital signature

            #endregion
        }

        /// <summary>
        /// MPQ User Data are optional, and is commonly used in custom maps for
        /// Starcraft II. If MPQ User Data header is present, it contains an offset,
        /// from where the MPQ header should be searched.
        /// </summary>
        internal class MoPaQUserData
        {
            #region Constants

            public const int Size = 0x10;

            /// <summary>
            /// Human-readable signature
            /// </summary>
            public static readonly string SignatureString = $"MPQ{(char)0x1B}";

            /// <summary>
            /// Signature as an unsigned Int32 value
            /// </summary>
            public const uint SignatureValue = 0x1B51504D;

            /// <summary>
            /// Signature as a byte array
            /// </summary>
            public static readonly byte[] SignatureBytes = new byte[] { 0x4D, 0x50, 0x51, 0x1B };

            #endregion

            #region Properties

            /// <summary>
            /// The user data signature
            /// </summary>
            /// <see cref="SignatureValue"/>
            public uint Signature { get; private set; }

            /// <summary>
            /// Maximum size of the user data
            /// </summary>
            public int UserDataSize { get; private set; }

            /// <summary>
            /// Offset of the MPQ header, relative to the beginning of this header
            /// </summary>
            public int HeaderOffset { get; private set; }

            /// <summary>
            /// Appears to be size of user data header (Starcraft II maps)
            /// </summary>
            public int UserDataHeadersize { get; private set; }

            // TODO: Does this area contain extra data that should be read in?

            #endregion
        }

        /// <summary>
        /// MoPaQ archive header
        /// </summary>
        internal class MoPaQArchiveHeader
        {
            #region V1 Properties

            /// <summary>
            /// The MPQ archive signature
            /// </summary>
            public uint Signature { get; private set; }

            /// <summary>
            /// Size of the archive header
            /// </summary>
            public int HeaderSize { get; private set; }

            /// <summary>
            /// Size of MPQ archive
            /// </summary>
            /// <remarks>
            /// This field is deprecated in the Burning Crusade MoPaQ format, and the size of the archive
            /// is calculated as the size from the beginning of the archive to the end of the hash table,
            /// block table, or extended block table (whichever is largest).
            /// </remarks>
            public int ArchiveSize { get; private set; }

            /// <summary>
            /// 0 = Format 1 (up to The Burning Crusade)
            /// 1 = Format 2 (The Burning Crusade and newer)
            /// 2 = Format 3 (WoW - Cataclysm beta or newer)
            /// 3 = Format 4 (WoW - Cataclysm beta or newer)
            /// </summary>
            public short FormatVersion { get; private set; }

            /// <summary>
            /// Power of two exponent specifying the number of 512-byte disk sectors in each logical sector
            /// in the archive. The size of each logical sector in the archive is 512 * 2 ^ BlockSize.
            /// </summary>
            public short BlockSize { get; private set; }

            /// <summary>
            /// Offset to the beginning of the hash table, relative to the beginning of the archive.
            /// </summary>
            public int HashTablePosition { get; private set; }

            /// <summary>
            /// Offset to the beginning of the block table, relative to the beginning of the archive.
            /// </summary>
            public int BlockTablePosition { get; private set; }

            /// <summary>
            /// Number of entries in the hash table. Must be a power of two, and must be less than 2^16 for
            /// the original MoPaQ format, or less than 2^20 for the Burning Crusade format.
            /// </summary>
            public int HashTableSize { get; private set; }

            /// <summary>
            /// Number of entries in the block table
            /// </summary>
            public int BlockTableSize { get; private set; }

            #endregion

            #region V2 Properties

            /// <summary>
            /// Offset to the beginning of array of 16-bit high parts of file offsets.
            /// </summary>
            public long HiBlockTablePosition { get; private set; }

            /// <summary>
            /// High 16 bits of the hash table offset for large archives.
            /// </summary>
            public short HashTablePositionHi { get; private set; }

            /// <summary>
            /// High 16 bits of the block table offset for large archives.
            /// </summary>
            public short BlockTablePositionHi { get; private set; }

            #endregion

            #region V3 Properties

            /// <summary>
            /// 64-bit version of the archive size
            /// </summary>
            public long ArchiveSizeLong { get; private set; }

            /// <summary>
            /// 64-bit position of the BET table
            /// </summary>
            public long BetTablePosition { get; private set; }

            /// <summary>
            /// 64-bit position of the HET table
            /// </summary>
            public long HetTablePosition { get; private set; }

            #endregion

            #region V4 Properties

            /// <summary>
            /// Compressed size of the hash table
            /// </summary>
            public long HashTableSizeLong { get; private set; }

            /// <summary>
            /// Compressed size of the block table
            /// </summary>
            public long BlockTableSizeLong { get; private set; }

            /// <summary>
            /// Compressed size of the hi-block table
            /// </summary>
            public long HiBlockTableSize { get; private set; }

            /// <summary>
            /// Compressed size of the HET block
            /// </summary>
            public long HetTableSize { get; private set; }

            /// <summary>
            /// Compressed size of the BET block
            /// </summary>
            public long BetTablesize { get; private set; }

            /// <summary>
            /// Size of raw data chunk to calculate MD5.
            /// </summary>
            /// <remarks>MD5 of each data chunk follows the raw file data.</remarks>
            public int RawChunkSize { get; private set; }

            // TODO: Is there a byte[] here of size RawChunkSize?

            /// <summary>
            /// MD5 of the block table before decryption
            /// </summary>
            public byte[] BlockTableMD5 { get; private set; } = new byte[0x10];

            /// <summary>
            /// MD5 of the hash table before decryption
            /// </summary>
            public byte[] HashTableMD5 { get; private set; } = new byte[0x10];

            /// <summary>
            /// MD5 of the hi-block table
            /// </summary>
            public byte[] HiBlockTableMD5 { get; private set; } = new byte[0x10];

            /// <summary>
            /// MD5 of the BET table before decryption
            /// </summary>
            public byte[] BetTableMD5 { get; private set; } = new byte[0x10];

            /// <summary>
            /// MD5 of the HET table before decryption
            /// </summary>
            public byte[] HetTableMD5 { get; private set; } = new byte[0x10];

            /// <summary>
            /// MD5 of the MPQ header from signature to (including) HetTableMD5
            /// </summary>
            public byte[] MpqHeaderMD5 { get; private set; } = new byte[0x10];

            #endregion
        }

        /// <summary>
        /// The HET table is present if the HetTablePos64 member of MPQ header is
        /// set to nonzero. This table can fully replace hash table. Depending on
        /// MPQ size, the pair of HET&BET table can be more efficient than Hash&Block
        /// table. HET table can be encrypted and compressed.
        /// </summary>
        internal class MoPaQHetTable
        {
            #region Constants

            public const int Size = 0x44;

            /// <summary>
            /// Human-readable signature
            /// </summary>
            public static readonly string SignatureString = $"HET{(char)0x1A}";

            /// <summary>
            /// Signature as an unsigned Int32 value
            /// </summary>
            public const uint SignatureValue = 0x1A544548;

            /// <summary>
            /// Signature as a byte array
            /// </summary>
            public static readonly byte[] SignatureBytes = new byte[] { 0x48, 0x45, 0x54, 0x1A };

            #endregion

            // TODO: Extract this out and make in common between HET and BET
            #region Common Table Headers

            /// <summary>
            /// 'HET\x1A'
            /// </summary>
            public uint Signature { get; private set; }

            /// <summary>
            /// Version. Seems to be always 1
            /// </summary>
            public int Version { get; private set; }

            /// <summary>
            /// Size of the contained table
            /// </summary>
            public int DataSize { get; private set; }

            #endregion

            #region Properties

            /// <summary>
            /// Size of the entire hash table, including the header (in bytes)
            /// </summary>
            public int TableSize { get; private set; }

            /// <summary>
            /// Maximum number of files in the MPQ
            /// </summary>
            public int MaxFileCount { get; private set; }

            /// <summary>
            /// Size of the hash table (in bytes)
            /// </summary>
            public int HashTableSize { get; private set; }

            /// <summary>
            /// Effective size of the hash entry (in bits)
            /// </summary>
            public int HashEntrySize { get; private set; }

            /// <summary>
            /// Total size of file index (in bits)
            /// </summary>
            public int TotalIndexSize { get; private set; }

            /// <summary>
            /// Extra bits in the file index
            /// </summary>
            public int IndexSizeExtra { get; private set; }

            /// <summary>
            /// Effective size of the file index (in bits)
            /// </summary>
            public int IndexSize { get; private set; }

            /// <summary>
            /// Size of the block index subtable (in bytes)
            /// </summary>
            public int BlockTableSize { get; private set; }

            /// <summary>
            /// HET hash table. Each entry is 8 bits.
            /// </summary>
            /// <remarks>Size is derived from HashTableSize</remarks>
            public byte[] HashTable { get; private set; }

            // TODO: Implement both of these on parse
            // Array of file indexes. Bit size of each entry is taken from dwTotalIndexSize.
            // Table size is taken from dwHashTableSize.

            #endregion
        }

        /// <summary>
        /// he BET table is present if the BetTablePos64 member of MPQ header is set
        /// to nonzero. BET table is a successor of classic block table, and can fully
        /// replace it. It is also supposed to be more effective.
        /// </summary>
        internal class MoPaQBetTable
        {
            #region Constants

            public const int Size = 0x88;

            /// <summary>
            /// Human-readable signature
            /// </summary>
            public static readonly string SignatureString = $"BET{(char)0x1A}";

            /// <summary>
            /// Signature as an unsigned Int32 value
            /// </summary>
            public const uint SignatureValue = 0x1A544542;

            /// <summary>
            /// Signature as a byte array
            /// </summary>
            public static readonly byte[] SignatureBytes = new byte[] { 0x42, 0x45, 0x54, 0x1A };

            #endregion

            // TODO: Extract this out and make in common between HET and BET
            #region Common Table Headers

            /// <summary>
            /// 'BET\x1A'
            /// </summary>
            public uint Signature { get; private set; }

            /// <summary>
            /// Version. Seems to be always 1
            /// </summary>
            public int Version { get; private set; }

            /// <summary>
            /// Size of the contained table
            /// </summary>
            public int DataSize { get; private set; }

            #endregion

            #region Properties

            /// <summary>
            /// Size of the entire hash table, including the header (in bytes)
            /// </summary>
            public int TableSize { get; private set; }

            /// <summary>
            /// Number of files in the BET table
            /// </summary>
            public int FileCount { get; private set; }

            /// <summary>
            /// Unknown, set to 0x10
            /// </summary>
            public int Unknown { get; private set; }

            /// <summary>
            /// Size of one table entry (in bits)
            /// </summary>
            public int TableEntrySize { get; private set; }

            /// <summary>
            /// Bit index of the file position (within the entry record)
            /// </summary>
            public int FilePositionBitIndex { get; private set; }

            /// <summary>
            /// Bit index of the file size (within the entry record)
            /// </summary>
            public int FileSizeBitIndex { get; private set; }

            /// <summary>
            /// Bit index of the compressed size (within the entry record)
            /// </summary>
            public int CompressedSizeBitIndex { get; private set; }

            /// <summary>
            /// Bit index of the flag index (within the entry record)
            /// </summary>
            public int FlagIndexBitIndex { get; private set; }

            /// <summary>
            /// Bit index of the ??? (within the entry record)
            /// </summary>
            public int UnknownBitIndex { get; private set; }

            /// <summary>
            /// Bit size of file position (in the entry record)
            /// </summary>
            public int FilePositionBitCount { get; private set; }

            /// <summary>
            /// Bit size of file size (in the entry record)
            /// </summary>
            public int FileSizeBitCount { get; private set; }

            /// <summary>
            /// Bit size of compressed file size (in the entry record)
            /// </summary>
            public int CompressedSizeBitCount { get; private set; }

            /// <summary>
            /// Bit size of flags index (in the entry record)
            /// </summary>
            public int FlagIndexBitCount { get; private set; }

            /// <summary>
            /// Bit size of ??? (in the entry record)
            /// </summary>
            public int UnknownBitCount { get; private set; }

            /// <summary>
            /// Total size of the BET hash
            /// </summary>
            public int TotalBetHashSize { get; private set; }

            /// <summary>
            /// Extra bits in the BET hash
            /// </summary>
            public int BetHashSizeExtra { get; private set; }

            /// <summary>
            /// Effective size of BET hash (in bits)
            /// </summary>
            public int BetHashSize { get; private set; }

            /// <summary>
            /// Size of BET hashes array, in bytes
            /// </summary>
            public int BetHashArraySize { get; private set; }

            /// <summary>
            /// Number of flags in the following array
            /// </summary>
            public int FlagCount { get; private set; }

            /// <summary>
            /// Followed by array of file flags. Each entry is 32-bit size and its meaning is the same like
            /// </summary>
            /// <remarks>Size from <see cref="FlagCount"/></remarks>
            public int[] FlagsArray { get; private set; }

            // File table. Size of each entry is taken from dwTableEntrySize.
            // Size of the table is (dwTableEntrySize * dwMaxFileCount), round up to 8.

            // Array of BET hashes. Table size is taken from dwMaxFileCount from HET table

            #endregion
        }

        /// <summary>
        /// Hash table is used for searching files by name. The file name is converted to
        /// two 32-bit hash values, which are then used for searching in the table. The size
        /// of the hash table must always be a power of two. Each entry in the hash table
        /// also contains file locale and offset into block table. Size of one entry of hash
        /// table is 16 bytes.
        /// </summary>
        internal class MoPaQHashEntry
        {
            #region Constants

            public const int Size = 0x10;

            #endregion

            #region Properties

            /// <summary>
            /// The hash of the full file name (part A)
            /// </summary>
            public uint NameHashPartA { get; private set; }

            /// <summary>
            /// The hash of the full file name (part B)
            /// </summary>
            public uint NameHashPartB { get; private set; }

            /// <summary>
            /// The language of the file. This is a Windows LANGID data type, and uses the same values.
            /// 0 indicates the default language (American English), or that the file is language-neutral.
            /// </summary>
            public MoPaQLocale Locale { get; private set; }

            /// <summary>
            /// The platform the file is used for. 0 indicates the default platform.
            /// No other values have been observed.
            /// </summary>
            public short Platform { get; private set; }

            /// <summary>
            /// If the hash table entry is valid, this is the index into the block table of the file.
            /// Otherwise, one of the following two values:
            ///  - FFFFFFFFh: Hash table entry is empty, and has always been empty.
            ///               Terminates searches for a given file.
            ///  - FFFFFFFEh: Hash table entry is empty, but was valid at some point (a deleted file).
            ///               Does not terminate searches for a given file.
            /// </summary>
            public uint BlockIndex { get; private set; }

            #endregion
        }

        internal enum MoPaQLocale : short
        {
            Neutral = 0,
            AmericanEnglish = 0,
            ChineseTaiwan = 0x404,
            Czech = 0x405,
            German = 0x407,
            English = 0x409,
            Spanish = 0x40A,
            French = 0x40C,
            Italian = 0x410,
            Japanese = 0x411,
            Korean = 0x412,
            Polish = 0x415,
            Portuguese = 0x416,
            Russian = 0x419,
            EnglishUK = 0x809,
        }

        /// <summary>
        /// lock table contains informations about file sizes and way of their storage within
        /// the archive. It also contains the position of file content in the archive. Size
        /// of block table entry is (like hash table entry). The block table is also encrypted.
        /// </summary>
        internal class MoPaQBlockEntry
        {
            #region Constants

            public const int Size = 0x10;

            #endregion

            #region Properties

            /// <summary>
            /// Offset of the beginning of the file data, relative to the beginning of the archive.
            /// </summary>
            public int FilePosition { get; private set; }

            /// <summary>
            /// Compressed file size
            /// </summary>
            public int CompressedSize { get; private set; }

            /// <summary>
            /// Size of uncompressed file
            /// </summary>
            public int UncompressedSize { get; private set; }

            /// <summary>
            /// Flags for the file.
            /// </summary>
            public MoPaQFileFlags Flags { get; private set; }

            #endregion
        }

        [Flags]
        internal enum MoPaQFileFlags : uint
        {
            /// <summary>
            /// File is compressed using PKWARE Data compression library
            /// </summary>
            MPQ_FILE_IMPLODE = 0x00000100,

            /// <summary>
            /// File is compressed using combination of compression methods
            /// </summary>
            MPQ_FILE_COMPRESS = 0x00000200,

            /// <summary>
            /// The file is encrypted
            /// </summary>
            MPQ_FILE_ENCRYPTED = 0x00010000,

            /// <summary>
            /// The decryption key for the file is altered according to the
            /// position of the file in the archive
            /// </summary>
            MPQ_FILE_FIX_KEY = 0x00020000,

            /// <summary>
            /// The file contains incremental patch for an existing file in base MPQ
            /// </summary>
            MPQ_FILE_PATCH_FILE = 0x00100000,

            /// <summary>
            /// Instead of being divided to 0x1000-bytes blocks, the file is stored
            /// as single unit
            /// </summary>
            MPQ_FILE_SINGLE_UNIT = 0x01000000,

            /// <summary>
            /// File is a deletion marker, indicating that the file no longer exists.
            /// This is used to allow patch archives to delete files present in
            /// lower-priority archives in the search chain. The file usually has
            /// length of 0 or 1 byte and its name is a hash
            /// </summary>
            MPQ_FILE_DELETE_MARKER = 0x02000000,

            /// <summary>
            /// File has checksums for each sector (explained in the File Data section).
            /// Ignored if file is not compressed or imploded.
            /// </summary>
            MPQ_FILE_SECTOR_CRC = 0x04000000,

            /// <summary>
            /// Set if file exists, reset when the file was deleted
            /// </summary>
            MPQ_FILE_EXISTS = 0x80000000,
        }

        #endregion
    }
}
