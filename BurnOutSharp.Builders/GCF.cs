using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.GCF;
using BinaryObjectScanner.Utilities;

namespace BurnOutSharp.Builders
{
    public static class GCF
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Half-Life Game Cache
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Half-Life Game Cache on success, null on error</returns>
        public static Models.GCF.File ParseFile(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseFile(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache on success, null on error</returns>
        public static Models.GCF.File ParseFile(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Half-Life Game Cache to fill
            var file = new Models.GCF.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the game cache header
            file.Header = header;

            #endregion

            #region Block Entry Header

            // Try to parse the block entry header
            var blockEntryHeader = ParseBlockEntryHeader(data);
            if (blockEntryHeader == null)
                return null;

            // Set the game cache block entry header
            file.BlockEntryHeader = blockEntryHeader;

            #endregion

            #region Block Entries

            // Create the block entry array
            file.BlockEntries = new BlockEntry[blockEntryHeader.BlockCount];

            // Try to parse the block entries
            for (int i = 0; i < blockEntryHeader.BlockCount; i++)
            {
                var blockEntry = ParseBlockEntry(data);
                file.BlockEntries[i] = blockEntry;
            }

            #endregion

            #region Fragmentation Map Header

            // Try to parse the fragmentation map header
            var fragmentationMapHeader = ParseFragmentationMapHeader(data);
            if (fragmentationMapHeader == null)
                return null;

            // Set the game cache fragmentation map header
            file.FragmentationMapHeader = fragmentationMapHeader;

            #endregion

            #region Fragmentation Maps

            // Create the fragmentation map array
            file.FragmentationMaps = new FragmentationMap[fragmentationMapHeader.BlockCount];

            // Try to parse the fragmentation maps
            for (int i = 0; i < fragmentationMapHeader.BlockCount; i++)
            {
                var fragmentationMap = ParseFragmentationMap(data);
                file.FragmentationMaps[i] = fragmentationMap;
            }

            #endregion

            #region Block Entry Map Header

            if (header.MinorVersion < 6)
            {
                // Try to parse the block entry map header
                var blockEntryMapHeader = ParseBlockEntryMapHeader(data);
                if (blockEntryMapHeader == null)
                    return null;

                // Set the game cache block entry map header
                file.BlockEntryMapHeader = blockEntryMapHeader;
            }

            #endregion

            #region Block Entry Maps

            if (header.MinorVersion < 6)
            {
                // Create the block entry map array
                file.BlockEntryMaps = new BlockEntryMap[file.BlockEntryMapHeader.BlockCount];

                // Try to parse the block entry maps
                for (int i = 0; i < file.BlockEntryMapHeader.BlockCount; i++)
                {
                    var blockEntryMap = ParseBlockEntryMap(data);
                    file.BlockEntryMaps[i] = blockEntryMap;
                }
            }

            #endregion

            // Cache the current offset
            initialOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = ParseDirectoryHeader(data);
            if (directoryHeader == null)
                return null;

            // Set the game cache directory header
            file.DirectoryHeader = directoryHeader;

            #endregion

            #region Directory Entries

            // Create the directory entry array
            file.DirectoryEntries = new DirectoryEntry[directoryHeader.ItemCount];

            // Try to parse the directory entries
            for (int i = 0; i < directoryHeader.ItemCount; i++)
            {
                var directoryEntry = ParseDirectoryEntry(data);
                file.DirectoryEntries[i] = directoryEntry;
            }

            #endregion

            #region Directory Names

            if (directoryHeader.NameSize > 0)
            {
                // Get the current offset for adjustment
                long directoryNamesStart = data.Position;

                // Get the ending offset
                long directoryNamesEnd = data.Position + directoryHeader.NameSize;

                // Create the string dictionary
                file.DirectoryNames = new Dictionary<long, string>();

                // Loop and read the null-terminated strings
                while (data.Position < directoryNamesEnd)
                {
                    long nameOffset = data.Position - directoryNamesStart;
                    string directoryName = data.ReadString(Encoding.ASCII);
                    if (data.Position > directoryNamesEnd)
                    {
                        data.Seek(-directoryName.Length, SeekOrigin.Current);
                        byte[] endingData = data.ReadBytes((int)(directoryNamesEnd - data.Position));
                        if (endingData != null)
                            directoryName = Encoding.ASCII.GetString(endingData);
                        else
                            directoryName = null;
                    }

                    file.DirectoryNames[nameOffset] = directoryName;
                }

                // Loop and assign to entries
                foreach (var directoryEntry in file.DirectoryEntries)
                {
                    directoryEntry.Name = file.DirectoryNames[directoryEntry.NameOffset];
                }
            }

            #endregion

            #region Directory Info 1 Entries

            // Create the directory info 1 entry array
            file.DirectoryInfo1Entries = new DirectoryInfo1Entry[directoryHeader.Info1Count];

            // Try to parse the directory info 1 entries
            for (int i = 0; i < directoryHeader.Info1Count; i++)
            {
                var directoryInfo1Entry = ParseDirectoryInfo1Entry(data);
                file.DirectoryInfo1Entries[i] = directoryInfo1Entry;
            }

            #endregion

            #region Directory Info 2 Entries

            // Create the directory info 2 entry array
            file.DirectoryInfo2Entries = new DirectoryInfo2Entry[directoryHeader.ItemCount];

            // Try to parse the directory info 2 entries
            for (int i = 0; i < directoryHeader.ItemCount; i++)
            {
                var directoryInfo2Entry = ParseDirectoryInfo2Entry(data);
                file.DirectoryInfo2Entries[i] = directoryInfo2Entry;
            }

            #endregion

            #region Directory Copy Entries

            // Create the directory copy entry array
            file.DirectoryCopyEntries = new DirectoryCopyEntry[directoryHeader.CopyCount];

            // Try to parse the directory copy entries
            for (int i = 0; i < directoryHeader.CopyCount; i++)
            {
                var directoryCopyEntry = ParseDirectoryCopyEntry(data);
                file.DirectoryCopyEntries[i] = directoryCopyEntry;
            }

            #endregion

            #region Directory Local Entries

            // Create the directory local entry array
            file.DirectoryLocalEntries = new DirectoryLocalEntry[directoryHeader.LocalCount];

            // Try to parse the directory local entries
            for (int i = 0; i < directoryHeader.LocalCount; i++)
            {
                var directoryLocalEntry = ParseDirectoryLocalEntry(data);
                file.DirectoryLocalEntries[i] = directoryLocalEntry;
            }

            #endregion

            // Seek to end of directory section, just in case
            data.Seek(initialOffset + directoryHeader.DirectorySize, SeekOrigin.Begin);

            #region Directory Map Header

            if (header.MinorVersion >= 5)
            {
                // Try to parse the directory map header
                var directoryMapHeader = ParseDirectoryMapHeader(data);
                if (directoryMapHeader == null)
                    return null;

                // Set the game cache directory map header
                file.DirectoryMapHeader = directoryMapHeader;
            }

            #endregion

            #region Directory Map Entries

            // Create the directory map entry array
            file.DirectoryMapEntries = new DirectoryMapEntry[directoryHeader.ItemCount];

            // Try to parse the directory map entries
            for (int i = 0; i < directoryHeader.ItemCount; i++)
            {
                var directoryMapEntry = ParseDirectoryMapEntry(data);
                file.DirectoryMapEntries[i] = directoryMapEntry;
            }

            #endregion

            #region Checksum Header

            // Try to parse the checksum header
            var checksumHeader = ParseChecksumHeader(data);
            if (checksumHeader == null)
                return null;

            // Set the game cache checksum header
            file.ChecksumHeader = checksumHeader;

            #endregion

            // Cache the current offset
            initialOffset = data.Position;

            #region Checksum Map Header

            // Try to parse the checksum map header
            var checksumMapHeader = ParseChecksumMapHeader(data);
            if (checksumMapHeader == null)
                return null;

            // Set the game cache checksum map header
            file.ChecksumMapHeader = checksumMapHeader;

            #endregion

            #region Checksum Map Entries

            // Create the checksum map entry array
            file.ChecksumMapEntries = new ChecksumMapEntry[checksumMapHeader.ItemCount];

            // Try to parse the checksum map entries
            for (int i = 0; i < checksumMapHeader.ItemCount; i++)
            {
                var checksumMapEntry = ParseChecksumMapEntry(data);
                file.ChecksumMapEntries[i] = checksumMapEntry;
            }

            #endregion

            #region Checksum Entries

            // Create the checksum entry array
            file.ChecksumEntries = new ChecksumEntry[checksumMapHeader.ChecksumCount];

            // Try to parse the checksum entries
            for (int i = 0; i < checksumMapHeader.ChecksumCount; i++)
            {
                var checksumEntry = ParseChecksumEntry(data);
                file.ChecksumEntries[i] = checksumEntry;
            }

            #endregion

            // Seek to end of checksum section, just in case
            data.Seek(initialOffset + checksumHeader.ChecksumSize, SeekOrigin.Begin);

            #region Data Block Header

            // Try to parse the data block header
            var dataBlockHeader = ParseDataBlockHeader(data, header.MinorVersion);
            if (dataBlockHeader == null)
                return null;

            // Set the game cache data block header
            file.DataBlockHeader = dataBlockHeader;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Header header = new Header();

            header.Dummy0 = data.ReadUInt32();
            if (header.Dummy0 != 0x00000001)
                return null;

            header.MajorVersion = data.ReadUInt32();
            if (header.MajorVersion != 0x00000001)
                return null;

            header.MinorVersion = data.ReadUInt32();
            if (header.MinorVersion != 3 && header.MinorVersion != 5 && header.MinorVersion != 6)
                return null;

            header.CacheID = data.ReadUInt32();
            header.LastVersionPlayed = data.ReadUInt32();
            header.Dummy1 = data.ReadUInt32();
            header.Dummy2 = data.ReadUInt32();
            header.FileSize = data.ReadUInt32();
            header.BlockSize = data.ReadUInt32();
            header.BlockCount = data.ReadUInt32();
            header.Dummy3 = data.ReadUInt32();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry header on success, null on error</returns>
        private static BlockEntryHeader ParseBlockEntryHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            BlockEntryHeader blockEntryHeader = new BlockEntryHeader();

            blockEntryHeader.BlockCount = data.ReadUInt32();
            blockEntryHeader.BlocksUsed = data.ReadUInt32();
            blockEntryHeader.Dummy0 = data.ReadUInt32();
            blockEntryHeader.Dummy1 = data.ReadUInt32();
            blockEntryHeader.Dummy2 = data.ReadUInt32();
            blockEntryHeader.Dummy3 = data.ReadUInt32();
            blockEntryHeader.Dummy4 = data.ReadUInt32();
            blockEntryHeader.Checksum = data.ReadUInt32();

            return blockEntryHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry on success, null on error</returns>
        private static BlockEntry ParseBlockEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            BlockEntry blockEntry = new BlockEntry();

            blockEntry.EntryFlags = data.ReadUInt32();
            blockEntry.FileDataOffset = data.ReadUInt32();
            blockEntry.FileDataSize = data.ReadUInt32();
            blockEntry.FirstDataBlockIndex = data.ReadUInt32();
            blockEntry.NextBlockEntryIndex = data.ReadUInt32();
            blockEntry.PreviousBlockEntryIndex = data.ReadUInt32();
            blockEntry.DirectoryIndex = data.ReadUInt32();

            return blockEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache fragmentation map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache fragmentation map header on success, null on error</returns>
        private static FragmentationMapHeader ParseFragmentationMapHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            FragmentationMapHeader fragmentationMapHeader = new FragmentationMapHeader();

            fragmentationMapHeader.BlockCount = data.ReadUInt32();
            fragmentationMapHeader.FirstUnusedEntry = data.ReadUInt32();
            fragmentationMapHeader.Terminator = data.ReadUInt32();
            fragmentationMapHeader.Checksum = data.ReadUInt32();

            return fragmentationMapHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache fragmentation map
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache fragmentation map on success, null on error</returns>
        private static FragmentationMap ParseFragmentationMap(Stream data)
        {
            // TODO: Use marshalling here instead of building
            FragmentationMap fragmentationMap = new FragmentationMap();

            fragmentationMap.NextDataBlockIndex = data.ReadUInt32();

            return fragmentationMap;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry map header on success, null on error</returns>
        private static BlockEntryMapHeader ParseBlockEntryMapHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            BlockEntryMapHeader blockEntryMapHeader = new BlockEntryMapHeader();

            blockEntryMapHeader.BlockCount = data.ReadUInt32();
            blockEntryMapHeader.FirstBlockEntryIndex = data.ReadUInt32();
            blockEntryMapHeader.LastBlockEntryIndex = data.ReadUInt32();
            blockEntryMapHeader.Dummy0 = data.ReadUInt32();
            blockEntryMapHeader.Checksum = data.ReadUInt32();

            return blockEntryMapHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry map
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry map on success, null on error</returns>
        private static BlockEntryMap ParseBlockEntryMap(Stream data)
        {
            // TODO: Use marshalling here instead of building
            BlockEntryMap blockEntryMap = new BlockEntryMap();

            blockEntryMap.PreviousBlockEntryIndex = data.ReadUInt32();
            blockEntryMap.NextBlockEntryIndex = data.ReadUInt32();

            return blockEntryMap;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory header on success, null on error</returns>
        private static DirectoryHeader ParseDirectoryHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryHeader directoryHeader = new DirectoryHeader();

            directoryHeader.Dummy0 = data.ReadUInt32();
            directoryHeader.CacheID = data.ReadUInt32();
            directoryHeader.LastVersionPlayed = data.ReadUInt32();
            directoryHeader.ItemCount = data.ReadUInt32();
            directoryHeader.FileCount = data.ReadUInt32();
            directoryHeader.Dummy1 = data.ReadUInt32();
            directoryHeader.DirectorySize = data.ReadUInt32();
            directoryHeader.NameSize = data.ReadUInt32();
            directoryHeader.Info1Count = data.ReadUInt32();
            directoryHeader.CopyCount = data.ReadUInt32();
            directoryHeader.LocalCount = data.ReadUInt32();
            directoryHeader.Dummy2 = data.ReadUInt32();
            directoryHeader.Dummy3 = data.ReadUInt32();
            directoryHeader.Checksum = data.ReadUInt32();

            return directoryHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory entry on success, null on error</returns>
        private static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryEntry directoryEntry = new DirectoryEntry();

            directoryEntry.NameOffset = data.ReadUInt32();
            directoryEntry.ItemSize = data.ReadUInt32();
            directoryEntry.ChecksumIndex = data.ReadUInt32();
            directoryEntry.DirectoryFlags = (HL_GCF_FLAG)data.ReadUInt32();
            directoryEntry.ParentIndex = data.ReadUInt32();
            directoryEntry.NextIndex = data.ReadUInt32();
            directoryEntry.FirstIndex = data.ReadUInt32();

            return directoryEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory info 1 entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory info 1 entry on success, null on error</returns>
        private static DirectoryInfo1Entry ParseDirectoryInfo1Entry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryInfo1Entry directoryInfo1Entry = new DirectoryInfo1Entry();

            directoryInfo1Entry.Dummy0 = data.ReadUInt32();

            return directoryInfo1Entry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory info 2 entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory info 2 entry on success, null on error</returns>
        private static DirectoryInfo2Entry ParseDirectoryInfo2Entry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryInfo2Entry directoryInfo2Entry = new DirectoryInfo2Entry();

            directoryInfo2Entry.Dummy0 = data.ReadUInt32();

            return directoryInfo2Entry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory copy entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory copy entry on success, null on error</returns>
        private static DirectoryCopyEntry ParseDirectoryCopyEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryCopyEntry directoryCopyEntry = new DirectoryCopyEntry();

            directoryCopyEntry.DirectoryIndex = data.ReadUInt32();

            return directoryCopyEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory local entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory local entry on success, null on error</returns>
        private static DirectoryLocalEntry ParseDirectoryLocalEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryLocalEntry directoryLocalEntry = new DirectoryLocalEntry();

            directoryLocalEntry.DirectoryIndex = data.ReadUInt32();

            return directoryLocalEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory map header on success, null on error</returns>
        private static DirectoryMapHeader ParseDirectoryMapHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryMapHeader directoryMapHeader = new DirectoryMapHeader();

            directoryMapHeader.Dummy0 = data.ReadUInt32();
            if (directoryMapHeader.Dummy0 != 0x00000001)
                return null;

            directoryMapHeader.Dummy1 = data.ReadUInt32();
            if (directoryMapHeader.Dummy1 != 0x00000000)
                return null;

            return directoryMapHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory map entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory map entry on success, null on error</returns>
        private static DirectoryMapEntry ParseDirectoryMapEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryMapEntry directoryMapEntry = new DirectoryMapEntry();

            directoryMapEntry.FirstBlockIndex = data.ReadUInt32();

            return directoryMapEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum header on success, null on error</returns>
        private static ChecksumHeader ParseChecksumHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ChecksumHeader checksumHeader = new ChecksumHeader();

            checksumHeader.Dummy0 = data.ReadUInt32();
            if (checksumHeader.Dummy0 != 0x00000001)
                return null;

            checksumHeader.ChecksumSize = data.ReadUInt32();

            return checksumHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum map header on success, null on error</returns>
        private static ChecksumMapHeader ParseChecksumMapHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ChecksumMapHeader checksumMapHeader = new ChecksumMapHeader();

            checksumMapHeader.Dummy0 = data.ReadUInt32();
            if (checksumMapHeader.Dummy0 != 0x14893721)
                return null;

            checksumMapHeader.Dummy1 = data.ReadUInt32();
            if (checksumMapHeader.Dummy1 != 0x00000001)
                return null;

            checksumMapHeader.ItemCount = data.ReadUInt32();
            checksumMapHeader.ChecksumCount = data.ReadUInt32();

            return checksumMapHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum map entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum map entry on success, null on error</returns>
        private static ChecksumMapEntry ParseChecksumMapEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ChecksumMapEntry checksumMapEntry = new ChecksumMapEntry();

            checksumMapEntry.ChecksumCount = data.ReadUInt32();
            checksumMapEntry.FirstChecksumIndex = data.ReadUInt32();

            return checksumMapEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum entry on success, null on error</returns>
        private static ChecksumEntry ParseChecksumEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ChecksumEntry checksumEntry = new ChecksumEntry();

            checksumEntry.Checksum = data.ReadUInt32();

            return checksumEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache data block header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="minorVersion">Minor version field from the header</param>
        /// <returns>Filled Half-Life Game Cache data block header on success, null on error</returns>
        private static DataBlockHeader ParseDataBlockHeader(Stream data, uint minorVersion)
        {
            // TODO: Use marshalling here instead of building
            DataBlockHeader dataBlockHeader = new DataBlockHeader();

            // In version 3 the DataBlockHeader is missing the LastVersionPlayed field.
            if (minorVersion >= 5)
                dataBlockHeader.LastVersionPlayed = data.ReadUInt32();

            dataBlockHeader.BlockCount = data.ReadUInt32();
            dataBlockHeader.BlockSize = data.ReadUInt32();
            dataBlockHeader.FirstBlockOffset = data.ReadUInt32();
            dataBlockHeader.BlocksUsed = data.ReadUInt32();
            dataBlockHeader.Checksum = data.ReadUInt32();

            return dataBlockHeader;
        }

        #endregion
    }
}
