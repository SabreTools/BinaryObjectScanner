using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO;
using SabreTools.Models.MoPaQ;
using static SabreTools.Models.MoPaQ.Constants;

namespace BinaryObjectScanner.Builders
{
    public class MoPaQ
    {
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

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseArchive(dataStream);
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
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
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
            if (possibleSignature == 0x1B51504D)
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
            if (possibleSignature == 0x1A51504D)
            {
                // Try to parse the archive header
                var archiveHeader = ParseArchiveHeader(data);
                if (archiveHeader == null)
                    return null;

                // Set the archive header
                archive.ArchiveHeader = archiveHeader;
            }
            else
            {
                return null;
            }

            #endregion

            #region Hash Table

            // TODO: The hash table has to be be decrypted before reading

            // Version 1
            if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format1)
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
            else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format2 || archive.ArchiveHeader.FormatVersion == FormatVersion.Format3)
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
            else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format4)
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
            if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format1)
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
            else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format2 || archive.ArchiveHeader.FormatVersion == FormatVersion.Format3)
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
            else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format4)
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
            if (archive.ArchiveHeader.FormatVersion >= FormatVersion.Format2)
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
            if (archive.ArchiveHeader.FormatVersion >= FormatVersion.Format3)
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
            if (archive.ArchiveHeader.FormatVersion >= FormatVersion.Format3)
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
            byte[] signature = data.ReadBytes(4);
            archiveHeader.Signature = Encoding.ASCII.GetString(signature);
            if (archiveHeader.Signature != ArchiveHeaderSignatureString)
                return null;

            archiveHeader.HeaderSize = data.ReadUInt32();
            archiveHeader.ArchiveSize = data.ReadUInt32();
            archiveHeader.FormatVersion = (FormatVersion)data.ReadUInt16();
            archiveHeader.BlockSize = data.ReadUInt16();
            archiveHeader.HashTablePosition = data.ReadUInt32();
            archiveHeader.BlockTablePosition = data.ReadUInt32();
            archiveHeader.HashTableSize = data.ReadUInt32();
            archiveHeader.BlockTableSize = data.ReadUInt32();

            // V2
            if (archiveHeader.FormatVersion >= FormatVersion.Format2)
            {
                archiveHeader.HiBlockTablePosition = data.ReadUInt64();
                archiveHeader.HashTablePositionHi = data.ReadUInt16();
                archiveHeader.BlockTablePositionHi = data.ReadUInt16();
            }

            // V3
            if (archiveHeader.FormatVersion >= FormatVersion.Format3)
            {
                archiveHeader.ArchiveSizeLong = data.ReadUInt64();
                archiveHeader.BetTablePosition = data.ReadUInt64();
                archiveHeader.HetTablePosition = data.ReadUInt64();
            }

            // V4
            if (archiveHeader.FormatVersion >= FormatVersion.Format4)
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

            byte[] signature = data.ReadBytes(4);
            userData.Signature = Encoding.ASCII.GetString(signature);
            if (userData.Signature != UserDataSignatureString)
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
            byte[] signature = data.ReadBytes(4);
            hetTable.Signature = Encoding.ASCII.GetString(signature);
            if (hetTable.Signature != HetTableSignatureString)
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
            byte[] signature = data.ReadBytes(4);
            betTable.Signature = Encoding.ASCII.GetString(signature);
            if (betTable.Signature != BetTableSignatureString)
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

        #region Helpers

        /// <summary>
        /// Buffer for encryption and decryption
        /// </summary>
        private uint[] _stormBuffer = new uint[STORM_BUFFER_SIZE];

        /// <summary>
        /// Prepare the encryption table
        /// </summary>
        private void PrepareCryptTable()
        {
            uint seed = 0x00100001;
            for (uint index1 = 0; index1 < 0x100; index1++)
            {
                for (uint index2 = index1, i = 0; i < 5; i++, index2 += 0x100)
                {
                    seed = (seed * 125 + 3) % 0x2AAAAB;
                    uint temp1 = (seed & 0xFFFF) << 0x10;

                    seed = (seed * 125 + 3) % 0x2AAAAB;
                    uint temp2 = (seed & 0xFFFF);

                    _stormBuffer[index2] = (temp1 | temp2);
                }
            }
        }

        /// <summary>
        /// Decrypt a single block of data
        /// </summary>
        private unsafe byte[] DecryptBlock(byte[] block, uint length, uint key)
        {
            uint seed = 0xEEEEEEEE;

            uint[] castBlock = new uint[length / 4];
            Buffer.BlockCopy(block, 0, castBlock, 0, (int)length);
            int castBlockPtr = 0;

            // Round to uints
            length >>= 2;

            while (length-- > 0)
            {
                seed += _stormBuffer[MPQ_HASH_KEY2_MIX + (key & 0xFF)];
                uint ch = castBlock[castBlockPtr] ^ (key + seed);

                key = ((~key << 0x15) + 0x11111111) | (key >> 0x0B);
                seed = ch + seed + (seed << 5) + 3;
                castBlock[castBlockPtr++] = ch;
            }

            Buffer.BlockCopy(castBlock, 0, block, 0, (int)length);
            return block;
        }

        #endregion
    }
}
