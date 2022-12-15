using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Models.MoPaQ;

namespace BurnOutSharp.Builder
{
    // TODO: Make Stream Data rely on Byte Data
    public class MoPaQ
    {
        #region Constants

        #region User Data

        public const int UserDataSize = 0x10;

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string UserDataSignatureString = $"MPQ{(char)0x1B}";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint UserDataSignatureValue = 0x1B51504D;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] UserDataSignatureBytes = new byte[] { 0x4D, 0x50, 0x51, 0x1B };

        #endregion

        #region Archive Header

        #region Signatures

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string ArchiveHeaderSignatureString = $"MPQ{(char)0x1A}";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint ArchiveHeaderSignatureValue = 0x1A51504D;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] ArchiveHeaderSignatureBytes = new byte[] { 0x4D, 0x50, 0x51, 0x1A };

        #endregion

        #region Header Sizes

        public const int ArchiveHeaderHeaderVersion1Size = 0x20;

        public const int ArchiveHeaderHeaderVersion2Size = 0x2C;

        public const int ArchiveHeaderHeaderVersion3Size = 0x44;

        public const int ArchiveHeaderHeaderVersion4Size = 0xD0;

        #endregion

        #endregion

        #region HET Table

        public const int HetTableSize = 0x44;

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string HetTableSignatureString = $"HET{(char)0x1A}";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint HetTableSignatureValue = 0x1A544548;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] HetTableSignatureBytes = new byte[] { 0x48, 0x45, 0x54, 0x1A };

        #endregion

        #region BET Table

        public const int BetTableSize = 0x88;

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string BetTableSignatureString = $"BET{(char)0x1A}";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint BetTableSignatureValue = 0x1A544542;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] BetTableSignatureBytes = new byte[] { 0x42, 0x45, 0x54, 0x1A };

        #endregion

        #region Hash Entry

        public const int HashEntrySize = 0x10;

        #endregion

        #region Block Entry

        public const int BlockEntrySize = 0x10;

        #endregion

        #region Patch Header

        #region Signatures

        #region Patch Header

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string PatchSignatureString = $"PTCH";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint PatchSignatureValue = 0x48435450;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] PatchSignatureBytes = new byte[] { 0x50, 0x54, 0x43, 0x48 };

        #endregion

        #region MD5 Block

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string Md5SignatureString = $"MD5_";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint Md5SignatureValue = 0x5F35444D;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] Md5SignatureBytes = new byte[] { 0x4D, 0x44, 0x35, 0x5F };

        #endregion

        #region XFRM Block

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string XFRMSignatureString = $"XFRM";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint XFRMSignatureValue = 0x4D524658;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] XFRMSignatureBytes = new byte[] { 0x58, 0x46, 0x52, 0x4D };

        #endregion

        #region BSDIFF Patch Type

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string BSDIFF40SignatureString = $"BSDIFF40";

        /// <summary>
        /// Signature as an unsigned Int64 value
        /// </summary>
        public const ulong BSDIFF40SignatureValue = 0x3034464649445342;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] BSDIFF40SignatureBytes = new byte[] { 0x42, 0x53, 0x44, 0x49, 0x46, 0x46, 0x34, 0x30 };

        #endregion

        #endregion

        #endregion

        #endregion

        #region Byte Data

        /// <summary>
        /// Parse a byte array into a MoPaQ archive
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled archive on success, null on error</returns>
        public static Archive ParseArchive(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = offset;

            // Create a new archive to fill
            var archive = new Archive();

            #region User Data

            // Check for User Data
            uint possibleSignature = BitConverter.ToUInt32(data, offset);
            if (possibleSignature == UserDataSignatureValue)
            {
                // Save the current position for offset correction
                int basePtr = offset;

                // Deserialize the user data, returning null if invalid
                var userData = ParseUserData(data, ref offset);
                if (userData == null)
                    return null;

                // Set the user data
                archive.UserData = userData;

                // Set the starting position according to the header offset
                offset = basePtr + (int)archive.UserData.HeaderOffset;
            }

            #endregion

            #region Archive Header

            // Check for the Header
            possibleSignature = BitConverter.ToUInt32(data, offset);
            if (possibleSignature == ArchiveHeaderSignatureValue)
            {
                // Try to parse the archive header
                var archiveHeader = ParseArchiveHeader(data, ref offset);
                if (archiveHeader == null)
                    return null;

                // Set the archive header
                archive.ArchiveHeader = archiveHeader;
            }

            #endregion

            #region Hash Table

            // Version 1
            if (archive.ArchiveHeader.FormatVersion == 0)
            {
                // If we have a hash table
                long hashTableOffset = archive.ArchiveHeader.HashTablePosition;
                if (hashTableOffset != 0)
                {
                    // Find the ending offset based on size
                    long hashTableEnd = hashTableOffset + archive.ArchiveHeader.HashTableSize;

                    // Read in the hash table
                    var hashTable = new List<HashEntry>();

                    while (hashTableOffset < hashTableEnd)
                    {
                        var hashEntry = ParseHashEntry(data, ref hashTableOffset);
                        if (hashEntry == null)
                            return null;

                        hashTable.Add(hashEntry);
                    }

                    archive.HashTable = hashTable.ToArray();
                }
            }

            // Version 2 and 3
            else if (archive.ArchiveHeader.FormatVersion == 1 || archive.ArchiveHeader.FormatVersion == 2)
            {
                // If we have a hash table
                long hashTableOffset = ((uint)archive.ArchiveHeader.HashTablePositionHi << 23) | archive.ArchiveHeader.HashTablePosition;
                if (hashTableOffset != 0)
                {
                    // Find the ending offset based on size
                    long hashTableEnd = hashTableOffset + archive.ArchiveHeader.HashTableSize;

                    // Read in the hash table
                    var hashTable = new List<HashEntry>();

                    while (hashTableOffset < hashTableEnd)
                    {
                        var hashEntry = ParseHashEntry(data, ref hashTableOffset);
                        if (hashEntry == null)
                            return null;

                        hashTable.Add(hashEntry);
                    }

                    archive.HashTable = hashTable.ToArray();
                }
            }

            // Version 4
            else if (archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a hash table
                long hashTableOffset = ((uint)archive.ArchiveHeader.HashTablePositionHi << 23) | archive.ArchiveHeader.HashTablePosition;
                if (hashTableOffset != 0)
                {
                    // Find the ending offset based on size
                    long hashTableEnd = hashTableOffset + (long)archive.ArchiveHeader.HashTableSizeLong;

                    // Read in the hash table
                    var hashTable = new List<HashEntry>();

                    while (hashTableOffset < hashTableEnd)
                    {
                        var hashEntry = ParseHashEntry(data, ref hashTableOffset);
                        if (hashEntry == null)
                            return null;

                        hashTable.Add(hashEntry);
                    }

                    archive.HashTable = hashTable.ToArray();
                }
            }

            #endregion

            #region Block Table

            // Version 1
            if (archive.ArchiveHeader.FormatVersion == 0)
            {
                // If we have a block table
                long blockTableOffset = archive.ArchiveHeader.BlockTablePosition;
                if (blockTableOffset != 0)
                {
                    // Find the ending offset based on size
                    long blockTableEnd = blockTableOffset + archive.ArchiveHeader.BlockTableSize;

                    // Read in the block table
                    var blockTable = new List<BlockEntry>();

                    while (blockTableOffset < blockTableEnd)
                    {
                        var blockEntry = ParseBlockEntry(data, ref blockTableOffset);
                        if (blockEntry == null)
                            return null;

                        blockTable.Add(blockEntry);
                    }

                    archive.BlockTable = blockTable.ToArray();
                }
            }

            // Version 2 and 3
            else if (archive.ArchiveHeader.FormatVersion == 1 || archive.ArchiveHeader.FormatVersion == 2)
            {
                // If we have a block table
                long blockTableOffset = ((uint)archive.ArchiveHeader.BlockTablePositionHi << 23) | archive.ArchiveHeader.BlockTablePosition;
                if (blockTableOffset != 0)
                {
                    // Find the ending offset based on size
                    long blockTableEnd = blockTableOffset + archive.ArchiveHeader.BlockTableSize;

                    // Read in the block table
                    var blockTable = new List<BlockEntry>();

                    while (blockTableOffset < blockTableEnd)
                    {
                        var blockEntry = ParseBlockEntry(data, ref blockTableOffset);
                        if (blockEntry == null)
                            return null;

                        blockTable.Add(blockEntry);
                    }

                    archive.BlockTable = blockTable.ToArray();
                }
            }

            // Version 4
            else if (archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a block table
                long blockTableOffset = ((uint)archive.ArchiveHeader.BlockTablePositionHi << 23) | archive.ArchiveHeader.BlockTablePosition;
                if (blockTableOffset != 0)
                {
                    // Find the ending offset based on size
                    long blockTableEnd = blockTableOffset + (long)archive.ArchiveHeader.BlockTableSizeLong;

                    // Read in the block table
                    var blockTable = new List<BlockEntry>();

                    while (blockTableOffset < blockTableEnd)
                    {
                        var blockEntry = ParseBlockEntry(data, ref blockTableOffset);
                        if (blockEntry == null)
                            return null;

                        blockTable.Add(blockEntry);
                    }

                    archive.BlockTable = blockTable.ToArray();
                }
            }

            #endregion

            #region Hi-Block Table

            // Version 2, 3, and 4
            if (archive.ArchiveHeader.FormatVersion == 1
                || archive.ArchiveHeader.FormatVersion == 2
                || archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a hi-block table
                int hiBlockTableOffset = (int)archive.ArchiveHeader.HiBlockTablePosition;
                if (hiBlockTableOffset != 0)
                {
                    // Read in the hi-block table
                    var hiBlockTable = new List<short>();

                    for (int i = 0; i < archive.BlockTable.Length; i++)
                    {
                        short hiBlockEntry = data.ReadInt16(ref hiBlockTableOffset);
                        hiBlockTable.Add(hiBlockEntry);
                    }

                    archive.HiBlockTable = hiBlockTable.ToArray();
                }
            }

            #endregion

            #region BET Table

            // Version 3 and 4
            if (archive.ArchiveHeader.FormatVersion == 2 || archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a BET table
                int betTableOffset = (int)archive.ArchiveHeader.BetTablePosition;
                if (betTableOffset != 0)
                {
                    // Read in the BET table
                    var betTable = ParseBetTable(data, ref betTableOffset);
                    if (betTable == null)
                        return null;

                    archive.BetTable = betTable;
                }
            }

            #endregion

            #region HET Table

            // Version 3 and 4
            if (archive.ArchiveHeader.FormatVersion == 2 || archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a HET table
                int hetTableOffset = (int)archive.ArchiveHeader.HetTablePosition;
                if (hetTableOffset != 0)
                {
                    // Read in the HET table
                    var hetTable = ParseHetTable(data, ref hetTableOffset);
                    if (hetTable == null)
                        return null;

                    archive.HetTable = hetTable;
                }
            }

            #endregion

            return archive;
        }

        /// <summary>
        /// Parse a byte array into a archive header
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled archive header on success, null on error</returns>
        private static ArchiveHeader ParseArchiveHeader(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            ArchiveHeader archiveHeader = new ArchiveHeader();

            // V1 - Common
            archiveHeader.Signature = data.ReadUInt32(ref offset);
            if (archiveHeader.Signature != ArchiveHeaderSignatureValue)
                return null;

            archiveHeader.HeaderSize = data.ReadUInt32(ref offset);
            archiveHeader.ArchiveSize = data.ReadUInt32(ref offset);
            archiveHeader.FormatVersion = data.ReadUInt16(ref offset);
            archiveHeader.BlockSize = data.ReadUInt16(ref offset);
            archiveHeader.HashTablePosition = data.ReadUInt32(ref offset);
            archiveHeader.BlockTablePosition = data.ReadUInt32(ref offset);
            archiveHeader.HashTableSize = data.ReadUInt32(ref offset);
            archiveHeader.BlockTableSize = data.ReadUInt32(ref offset);

            // V2
            if (archiveHeader.FormatVersion >= 2 && archiveHeader.HeaderSize >= ArchiveHeaderHeaderVersion2Size)
            {
                archiveHeader.HiBlockTablePosition = data.ReadUInt64(ref offset);
                archiveHeader.HashTablePositionHi = data.ReadUInt16(ref offset);
                archiveHeader.BlockTablePositionHi = data.ReadUInt16(ref offset);
            }

            // V3
            if (archiveHeader.FormatVersion >= 3 && archiveHeader.HeaderSize >= ArchiveHeaderHeaderVersion3Size)
            {
                archiveHeader.ArchiveSizeLong = data.ReadUInt64(ref offset);
                archiveHeader.BetTablePosition = data.ReadUInt64(ref offset);
                archiveHeader.HetTablePosition = data.ReadUInt64(ref offset);
            }

            // V4
            if (archiveHeader.FormatVersion >= 4 && archiveHeader.HeaderSize >= ArchiveHeaderHeaderVersion4Size)
            {
                archiveHeader.HashTableSizeLong = data.ReadUInt64(ref offset);
                archiveHeader.BlockTableSizeLong = data.ReadUInt64(ref offset);
                archiveHeader.HiBlockTableSize = data.ReadUInt64(ref offset);
                archiveHeader.HetTableSize = data.ReadUInt64(ref offset);
                archiveHeader.BetTablesize = data.ReadUInt64(ref offset);
                archiveHeader.RawChunkSize = data.ReadUInt32(ref offset);

                archiveHeader.BlockTableMD5 = data.ReadBytes(ref offset, 0x10);
                archiveHeader.HashTableMD5 = data.ReadBytes(ref offset, 0x10);
                archiveHeader.HiBlockTableMD5 = data.ReadBytes(ref offset, 0x10);
                archiveHeader.BetTableMD5 = data.ReadBytes(ref offset, 0x10);
                archiveHeader.HetTableMD5 = data.ReadBytes(ref offset, 0x10);
                archiveHeader.HetTableMD5 = data.ReadBytes(ref offset, 0x10);
            }

            return archiveHeader;
        }

        /// <summary>
        /// Parse a byte array into a user data object
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled user data on success, null on error</returns>
        private static UserData ParseUserData(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            UserData userData = new UserData();

            userData.Signature = data.ReadUInt32(ref offset);
            if (userData.Signature != UserDataSignatureValue)
                return null;

            userData.UserDataSize = data.ReadUInt32(ref offset);
            userData.HeaderOffset = data.ReadUInt32(ref offset);
            userData.UserDataHeaderSize = data.ReadUInt32(ref offset);

            return userData;
        }

        /// <summary>
        /// Parse a byte array into a HET table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled HET table on success, null on error</returns>
        private static HetTable ParseHetTable(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            HetTable hetTable = new HetTable();

            // Common Headers
            hetTable.Signature = data.ReadUInt32(ref offset);
            if (hetTable.Signature != HetTableSignatureValue)
                return null;

            hetTable.Version = data.ReadUInt32(ref offset);
            hetTable.DataSize = data.ReadUInt32(ref offset);

            // HET-Specific
            hetTable.TableSize = data.ReadUInt32(ref offset);
            hetTable.MaxFileCount = data.ReadUInt32(ref offset);
            hetTable.HashTableSize = data.ReadUInt32(ref offset);
            hetTable.TotalIndexSize = data.ReadUInt32(ref offset);
            hetTable.IndexSizeExtra = data.ReadUInt32(ref offset);
            hetTable.IndexSize = data.ReadUInt32(ref offset);
            hetTable.BlockTableSize = data.ReadUInt32(ref offset);
            hetTable.HashTable = data.ReadBytes(ref offset, (int)hetTable.HashTableSize);

            // TODO: Populate the file indexes array
            hetTable.FileIndexes = new byte[(int)hetTable.HashTableSize][];

            return hetTable;
        }

        /// <summary>
        /// Parse a byte array into a BET table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled BET table on success, null on error</returns>
        private static BetTable ParseBetTable(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            BetTable betTable = new BetTable();

            // Common Headers
            betTable.Signature = data.ReadUInt32(ref offset);
            if (betTable.Signature != BetTableSignatureValue)
                return null;

            betTable.Version = data.ReadUInt32(ref offset);
            betTable.DataSize = data.ReadUInt32(ref offset);

            // BET-Specific
            betTable.TableSize = data.ReadUInt32(ref offset);
            betTable.FileCount = data.ReadUInt32(ref offset);
            betTable.Unknown = data.ReadUInt32(ref offset);
            betTable.TableEntrySize = data.ReadUInt32(ref offset);

            betTable.FilePositionBitIndex = data.ReadUInt32(ref offset);
            betTable.FileSizeBitIndex = data.ReadUInt32(ref offset);
            betTable.CompressedSizeBitIndex = data.ReadUInt32(ref offset);
            betTable.FlagIndexBitIndex = data.ReadUInt32(ref offset);
            betTable.UnknownBitIndex = data.ReadUInt32(ref offset);

            betTable.FilePositionBitCount = data.ReadUInt32(ref offset);
            betTable.FileSizeBitCount = data.ReadUInt32(ref offset);
            betTable.CompressedSizeBitCount = data.ReadUInt32(ref offset);
            betTable.FlagIndexBitCount = data.ReadUInt32(ref offset);
            betTable.UnknownBitCount = data.ReadUInt32(ref offset);

            betTable.TotalBetHashSize = data.ReadUInt32(ref offset);
            betTable.BetHashSizeExtra = data.ReadUInt32(ref offset);
            betTable.BetHashSize = data.ReadUInt32(ref offset);
            betTable.BetHashArraySize = data.ReadUInt32(ref offset);
            betTable.FlagCount = data.ReadUInt32(ref offset);

            betTable.FlagsArray = new uint[betTable.FlagCount];
            Buffer.BlockCopy(data, offset, betTable.FlagsArray, 0, (int)betTable.FlagCount * 4);
            offset += (int)betTable.FlagCount * 4;

            // TODO: Populate the file table
            // TODO: Populate the hash table

            return betTable;
        }

        /// <summary>
        /// Parse a byte array into a hash entry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled hash entry on success, null on error</returns>
        private static HashEntry ParseHashEntry(byte[] data, ref long offset)
        {
            // TODO: Use marshalling here instead of building
            HashEntry hashEntry = new HashEntry();
            int intOffset = (int)offset;

            hashEntry.NameHashPartA = data.ReadUInt32(ref intOffset);
            hashEntry.NameHashPartB = data.ReadUInt32(ref intOffset);
            hashEntry.Locale = (Locale)data.ReadUInt16(ref intOffset);
            hashEntry.Platform = data.ReadUInt16(ref intOffset);
            hashEntry.BlockIndex = data.ReadUInt32(ref intOffset);

            offset = intOffset;
            return hashEntry;
        }

        /// <summary>
        /// Parse a byte array into a block entry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled block entry on success, null on error</returns>
        private static BlockEntry ParseBlockEntry(byte[] data, ref long offset)
        {
            // TODO: Use marshalling here instead of building
            BlockEntry blockEntry = new BlockEntry();
            int intOffset = (int)offset;

            blockEntry.FilePosition = data.ReadUInt32(ref intOffset);
            blockEntry.CompressedSize = data.ReadUInt32(ref intOffset);
            blockEntry.UncompressedSize = data.ReadUInt32(ref intOffset);
            blockEntry.Flags = (FileFlags)data.ReadUInt32(ref intOffset);

            offset = intOffset;
            return blockEntry;
        }

        /// <summary>
        /// Parse a byte array into a patch info
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled patch info on success, null on error</returns>
        private static PatchInfo ParsePatchInfo(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            PatchInfo patchInfo = new PatchInfo();

            patchInfo.Length = data.ReadUInt32(ref offset);
            patchInfo.Flags = data.ReadUInt32(ref offset);
            patchInfo.DataSize = data.ReadUInt32(ref offset);
            patchInfo.MD5 = data.ReadBytes(ref offset, 0x10);

            // TODO: Fill the sector offset table

            return patchInfo;
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a MoPaQ archive
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled archive on success, null on error</returns>
        public static Archive ParseArchive(Stream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new archive to fill
            var archive = new Archive();

            #region User Data

            // Check for User Data
            uint possibleSignature = data.ReadUInt32();
            data.Seek(-4, SeekOrigin.Current);
            if (possibleSignature == UserDataSignatureValue)
            {
                // Save the current position for offset correction
                long basePtr = data.Position;

                // Deserialize the user data, returning null if invalid
                var userData = ParseUserData(data);
                if (userData == null)
                    return null;

                // Set the user data
                archive.UserData = userData;

                // Set the starting position according to the header offset
                data.Seek(basePtr + (int)archive.UserData.HeaderOffset, SeekOrigin.Begin);
            }

            #endregion

            #region Archive Header

            // Check for the Header
            possibleSignature = data.ReadUInt32();
            data.Seek(-4, SeekOrigin.Current);
            if (possibleSignature == ArchiveHeaderSignatureValue)
            {
                // Try to parse the archive header
                var archiveHeader = ParseArchiveHeader(data);
                if (archiveHeader == null)
                    return null;

                // Set the archive header
                archive.ArchiveHeader = archiveHeader;
            }

            #endregion

            #region Hash Table

            // Version 1
            if (archive.ArchiveHeader.FormatVersion == 0)
            {
                // If we have a hash table
                long hashTableOffset = archive.ArchiveHeader.HashTablePosition;
                if (hashTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(hashTableOffset, SeekOrigin.Begin);

                    // Find the ending offset based on size
                    long hashTableEnd = hashTableOffset + archive.ArchiveHeader.HashTableSize;

                    // Read in the hash table
                    var hashTable = new List<HashEntry>();

                    while (data.Position < hashTableEnd)
                    {
                        var hashEntry = ParseHashEntry(data);
                        if (hashEntry == null)
                            return null;

                        hashTable.Add(hashEntry);
                    }

                    archive.HashTable = hashTable.ToArray();
                }
            }

            // Version 2 and 3
            else if (archive.ArchiveHeader.FormatVersion == 1 || archive.ArchiveHeader.FormatVersion == 2)
            {
                // If we have a hash table
                long hashTableOffset = ((uint)archive.ArchiveHeader.HashTablePositionHi << 23) | archive.ArchiveHeader.HashTablePosition;
                if (hashTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(hashTableOffset, SeekOrigin.Begin);

                    // Find the ending offset based on size
                    long hashTableEnd = hashTableOffset + archive.ArchiveHeader.HashTableSize;

                    // Read in the hash table
                    var hashTable = new List<HashEntry>();

                    while (data.Position < hashTableEnd)
                    {
                        var hashEntry = ParseHashEntry(data);
                        if (hashEntry == null)
                            return null;

                        hashTable.Add(hashEntry);
                    }

                    archive.HashTable = hashTable.ToArray();
                }
            }

            // Version 4
            else if (archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a hash table
                long hashTableOffset = ((uint)archive.ArchiveHeader.HashTablePositionHi << 23) | archive.ArchiveHeader.HashTablePosition;
                if (hashTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(hashTableOffset, SeekOrigin.Begin);

                    // Find the ending offset based on size
                    long hashTableEnd = hashTableOffset + (long)archive.ArchiveHeader.HashTableSizeLong;

                    // Read in the hash table
                    var hashTable = new List<HashEntry>();

                    while (data.Position < hashTableEnd)
                    {
                        var hashEntry = ParseHashEntry(data);
                        if (hashEntry == null)
                            return null;

                        hashTable.Add(hashEntry);
                    }

                    archive.HashTable = hashTable.ToArray();
                }
            }

            #endregion

            #region Block Table

            // Version 1
            if (archive.ArchiveHeader.FormatVersion == 0)
            {
                // If we have a block table
                long blockTableOffset = archive.ArchiveHeader.BlockTablePosition;
                if (blockTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(blockTableOffset, SeekOrigin.Begin);

                    // Find the ending offset based on size
                    long blockTableEnd = blockTableOffset + archive.ArchiveHeader.BlockTableSize;

                    // Read in the block table
                    var blockTable = new List<BlockEntry>();

                    while (data.Position < blockTableEnd)
                    {
                        var blockEntry = ParseBlockEntry(data);
                        if (blockEntry == null)
                            return null;

                        blockTable.Add(blockEntry);
                    }

                    archive.BlockTable = blockTable.ToArray();
                }
            }

            // Version 2 and 3
            else if (archive.ArchiveHeader.FormatVersion == 1 || archive.ArchiveHeader.FormatVersion == 2)
            {
                // If we have a block table
                long blockTableOffset = ((uint)archive.ArchiveHeader.BlockTablePositionHi << 23) | archive.ArchiveHeader.BlockTablePosition;
                if (blockTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(blockTableOffset, SeekOrigin.Begin);

                    // Find the ending offset based on size
                    long blockTableEnd = blockTableOffset + archive.ArchiveHeader.BlockTableSize;

                    // Read in the block table
                    var blockTable = new List<BlockEntry>();

                    while (data.Position < blockTableEnd)
                    {
                        var blockEntry = ParseBlockEntry(data);
                        if (blockEntry == null)
                            return null;

                        blockTable.Add(blockEntry);
                    }

                    archive.BlockTable = blockTable.ToArray();
                }
            }

            // Version 4
            else if (archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a block table
                long blockTableOffset = ((uint)archive.ArchiveHeader.BlockTablePositionHi << 23) | archive.ArchiveHeader.BlockTablePosition;
                if (blockTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(blockTableOffset, SeekOrigin.Begin);

                    // Find the ending offset based on size
                    long blockTableEnd = blockTableOffset + (long)archive.ArchiveHeader.BlockTableSizeLong;

                    // Read in the block table
                    var blockTable = new List<BlockEntry>();

                    while (data.Position < blockTableEnd)
                    {
                        var blockEntry = ParseBlockEntry(data);
                        if (blockEntry == null)
                            return null;

                        blockTable.Add(blockEntry);
                    }

                    archive.BlockTable = blockTable.ToArray();
                }
            }

            #endregion

            #region Hi-Block Table

            // Version 2, 3, and 4
            if (archive.ArchiveHeader.FormatVersion == 1
                || archive.ArchiveHeader.FormatVersion == 2
                || archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a hi-block table
                long hiBlockTableOffset = (long)archive.ArchiveHeader.HiBlockTablePosition;
                if (hiBlockTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(hiBlockTableOffset, SeekOrigin.Begin);

                    // Read in the hi-block table
                    var hiBlockTable = new List<short>();

                    for (int i = 0; i < archive.BlockTable.Length; i++)
                    {
                        short hiBlockEntry = data.ReadInt16();
                        hiBlockTable.Add(hiBlockEntry);
                    }

                    archive.HiBlockTable = hiBlockTable.ToArray();
                }
            }

            #endregion

            #region BET Table

            // Version 3 and 4
            if (archive.ArchiveHeader.FormatVersion == 2 || archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a BET table
                long betTableOffset = (long)archive.ArchiveHeader.BetTablePosition;
                if (betTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(betTableOffset, SeekOrigin.Begin);

                    // Read in the BET table
                    var betTable = ParseBetTable(data);
                    if (betTable != null)
                        return null;

                    archive.BetTable = betTable;
                }
            }

            #endregion

            #region HET Table

            // Version 3 and 4
            if (archive.ArchiveHeader.FormatVersion == 2 || archive.ArchiveHeader.FormatVersion == 3)
            {
                // If we have a HET table
                long hetTableOffset = (long)archive.ArchiveHeader.HetTablePosition;
                if (hetTableOffset != 0)
                {
                    // Seek to the offset
                    data.Seek(hetTableOffset, SeekOrigin.Begin);

                    // Read in the HET table
                    var hetTable = ParseHetTable(data);
                    if (hetTable != null)
                        return null;

                    archive.HetTable = hetTable;
                }
            }

            #endregion

            return archive;
        }

        /// <summary>
        /// Parse a Stream into a archive header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled archive header on success, null on error</returns>
        private static ArchiveHeader ParseArchiveHeader(Stream data)
        {
            ArchiveHeader archiveHeader = new ArchiveHeader();

            // V1 - Common
            archiveHeader.Signature = data.ReadUInt32();
            if (archiveHeader.Signature != ArchiveHeaderSignatureValue)
                return null;

            archiveHeader.HeaderSize = data.ReadUInt32();
            archiveHeader.ArchiveSize = data.ReadUInt32();
            archiveHeader.FormatVersion = data.ReadUInt16();
            archiveHeader.BlockSize = data.ReadUInt16();
            archiveHeader.HashTablePosition = data.ReadUInt32();
            archiveHeader.BlockTablePosition = data.ReadUInt32();
            archiveHeader.HashTableSize = data.ReadUInt32();
            archiveHeader.BlockTableSize = data.ReadUInt32();

            // V2
            if (archiveHeader.FormatVersion >= 2 && archiveHeader.HeaderSize >= ArchiveHeaderHeaderVersion2Size)
            {
                archiveHeader.HiBlockTablePosition = data.ReadUInt64();
                archiveHeader.HashTablePositionHi = data.ReadUInt16();
                archiveHeader.BlockTablePositionHi = data.ReadUInt16();
            }

            // V3
            if (archiveHeader.FormatVersion >= 3 && archiveHeader.HeaderSize >= ArchiveHeaderHeaderVersion3Size)
            {
                archiveHeader.ArchiveSizeLong = data.ReadUInt64();
                archiveHeader.BetTablePosition = data.ReadUInt64();
                archiveHeader.HetTablePosition = data.ReadUInt64();
            }

            // V4
            if (archiveHeader.FormatVersion >= 4 && archiveHeader.HeaderSize >= ArchiveHeaderHeaderVersion4Size)
            {
                archiveHeader.HashTableSizeLong = data.ReadUInt64();
                archiveHeader.BlockTableSizeLong = data.ReadUInt64();
                archiveHeader.HiBlockTableSize = data.ReadUInt64();
                archiveHeader.HetTableSize = data.ReadUInt64();
                archiveHeader.BetTablesize = data.ReadUInt64();
                archiveHeader.RawChunkSize = data.ReadUInt32();

                archiveHeader.BlockTableMD5 = data.ReadBytes(0x10);
                archiveHeader.HashTableMD5 = data.ReadBytes(0x10);
                archiveHeader.HiBlockTableMD5 = data.ReadBytes(0x10);
                archiveHeader.BetTableMD5 = data.ReadBytes(0x10);
                archiveHeader.HetTableMD5 = data.ReadBytes(0x10);
                archiveHeader.HetTableMD5 = data.ReadBytes(0x10);
            }

            return archiveHeader;
        }

        /// <summary>
        /// Parse a Stream into a user data object
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled user data on success, null on error</returns>
        private static UserData ParseUserData(Stream data)
        {
            UserData userData = new UserData();

            userData.Signature = data.ReadUInt32();
            if (userData.Signature != UserDataSignatureValue)
                return null;

            userData.UserDataSize = data.ReadUInt32();
            userData.HeaderOffset = data.ReadUInt32();
            userData.UserDataHeaderSize = data.ReadUInt32();

            return userData;
        }

        /// <summary>
        /// Parse a Stream into a HET table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HET table on success, null on error</returns>
        private static HetTable ParseHetTable(Stream data)
        {
            HetTable hetTable = new HetTable();

            // Common Headers
            hetTable.Signature = data.ReadUInt32();
            if (hetTable.Signature != HetTableSignatureValue)
                return null;

            hetTable.Version = data.ReadUInt32();
            hetTable.DataSize = data.ReadUInt32();

            // HET-Specific
            hetTable.TableSize = data.ReadUInt32();
            hetTable.MaxFileCount = data.ReadUInt32();
            hetTable.HashTableSize = data.ReadUInt32();
            hetTable.TotalIndexSize = data.ReadUInt32();
            hetTable.IndexSizeExtra = data.ReadUInt32();
            hetTable.IndexSize = data.ReadUInt32();
            hetTable.BlockTableSize = data.ReadUInt32();
            hetTable.HashTable = data.ReadBytes((int)hetTable.HashTableSize);

            // TODO: Populate the file indexes array
            hetTable.FileIndexes = new byte[(int)hetTable.HashTableSize][];

            return hetTable;
        }

        /// <summary>
        /// Parse a Stream into a BET table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BET table on success, null on error</returns>
        private static BetTable ParseBetTable(Stream data)
        {
            BetTable betTable = new BetTable();

            // Common Headers
            betTable.Signature = data.ReadUInt32();
            if (betTable.Signature != BetTableSignatureValue)
                return null;

            betTable.Version = data.ReadUInt32();
            betTable.DataSize = data.ReadUInt32();

            // BET-Specific
            betTable.TableSize = data.ReadUInt32();
            betTable.FileCount = data.ReadUInt32();
            betTable.Unknown = data.ReadUInt32();
            betTable.TableEntrySize = data.ReadUInt32();

            betTable.FilePositionBitIndex = data.ReadUInt32();
            betTable.FileSizeBitIndex = data.ReadUInt32();
            betTable.CompressedSizeBitIndex = data.ReadUInt32();
            betTable.FlagIndexBitIndex = data.ReadUInt32();
            betTable.UnknownBitIndex = data.ReadUInt32();

            betTable.FilePositionBitCount = data.ReadUInt32();
            betTable.FileSizeBitCount = data.ReadUInt32();
            betTable.CompressedSizeBitCount = data.ReadUInt32();
            betTable.FlagIndexBitCount = data.ReadUInt32();
            betTable.UnknownBitCount = data.ReadUInt32();

            betTable.TotalBetHashSize = data.ReadUInt32();
            betTable.BetHashSizeExtra = data.ReadUInt32();
            betTable.BetHashSize = data.ReadUInt32();
            betTable.BetHashArraySize = data.ReadUInt32();
            betTable.FlagCount = data.ReadUInt32();

            betTable.FlagsArray = new uint[betTable.FlagCount];
            byte[] flagsArray = data.ReadBytes((int)betTable.FlagCount * 4);
            Buffer.BlockCopy(flagsArray, 0, betTable.FlagsArray, 0, (int)betTable.FlagCount * 4);

            // TODO: Populate the file table
            // TODO: Populate the hash table

            return betTable;
        }

        /// <summary>
        /// Parse a Stream into a hash entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled hash entry on success, null on error</returns>
        private static HashEntry ParseHashEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            HashEntry hashEntry = new HashEntry();

            hashEntry.NameHashPartA = data.ReadUInt32();
            hashEntry.NameHashPartB = data.ReadUInt32();
            hashEntry.Locale = (Locale)data.ReadUInt16();
            hashEntry.Platform = data.ReadUInt16();
            hashEntry.BlockIndex = data.ReadUInt32();

            return hashEntry;
        }

        /// <summary>
        /// Parse a Stream into a block entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled block entry on success, null on error</returns>
        private static BlockEntry ParseBlockEntry(Stream data)
        {
            BlockEntry blockEntry = new BlockEntry();

            blockEntry.FilePosition = data.ReadUInt32();
            blockEntry.CompressedSize = data.ReadUInt32();
            blockEntry.UncompressedSize = data.ReadUInt32();
            blockEntry.Flags = (FileFlags)data.ReadUInt32();

            return blockEntry;
        }

        /// <summary>
        /// Parse a Stream into a patch info
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled patch info on success, null on error</returns>
        private static PatchInfo ParsePatchInfo(Stream data)
        {
            // TODO: Use marshalling here instead of building
            PatchInfo patchInfo = new PatchInfo();

            patchInfo.Length = data.ReadUInt32();
            patchInfo.Flags = data.ReadUInt32();
            patchInfo.DataSize = data.ReadUInt32();
            patchInfo.MD5 = data.ReadBytes(0x10);

            // TODO: Fill the sector offset table

            return patchInfo;
        }

        #endregion
    }
}
