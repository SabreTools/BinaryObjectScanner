using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.VPK;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.VPK.Constants;

namespace BurnOutSharp.Builders
{
    public static class VPK
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Valve Package
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Valve Package on success, null on error</returns>
        public static Models.VPK.File ParseFile(byte[] data, int offset)
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
        /// Parse a Stream into a Valve Package
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package on success, null on error</returns>
        public static Models.VPK.File ParseFile(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Valve Package to fill
            var file = new Models.VPK.File();

            #region Header

            // Try to parse the header
            // The original version had no signature.
            var header = ParseHeader(data);

            // Set the package header
            file.Header = header;

            #endregion

            #region Extended Header

            if (header?.Version == 2)
            {
                // Try to parse the extended header
                var extendedHeader = ParseExtendedHeader(data);
                if (extendedHeader == null)
                    return null;

                // Set the package extended header
                file.ExtendedHeader = extendedHeader;
            }

            #endregion

            #region Directory Items

            // Create the directory items tree
            var directoryItems = ParseDirectoryItemTree(data);

            // Set the directory items
            file.DirectoryItems = directoryItems;

            #endregion

            #region Archive Hashes

            if (header?.Version == 2 && file.ExtendedHeader != null && file.ExtendedHeader.ArchiveHashLength > 0)
            {
                // Create the archive hashes list
                var archiveHashes = new List<ArchiveHash>();

                // Cache the current offset
                initialOffset = data.Position;

                // Try to parse the directory items
                while (data.Position < initialOffset + file.ExtendedHeader.ArchiveHashLength)
                {
                    var archiveHash = ParseArchiveHash(data);
                    archiveHashes.Add(archiveHash);
                }

                file.ArchiveHashes = archiveHashes.ToArray();
            }

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package header on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Header header = new Header();

            header.Signature = data.ReadUInt32();
            if (header.Signature != SignatureUInt32)
                return null;

            header.Version = data.ReadUInt32();
            if (header.Version > 2)
                return null;

            header.DirectoryLength = data.ReadUInt32();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package extended header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package extended header on success, null on error</returns>
        private static ExtendedHeader ParseExtendedHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ExtendedHeader extendedHeader = new ExtendedHeader();

            extendedHeader.Dummy0 = data.ReadUInt32();
            extendedHeader.ArchiveHashLength = data.ReadUInt32();
            extendedHeader.ExtraLength = data.ReadUInt32();
            extendedHeader.Dummy1 = data.ReadUInt32();

            return extendedHeader;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package archive hash
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package archive hash on success, null on error</returns>
        private static ArchiveHash ParseArchiveHash(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ArchiveHash archiveHash = new ArchiveHash();

            archiveHash.ArchiveIndex = data.ReadUInt32();
            archiveHash.ArchiveOffset = data.ReadUInt32();
            archiveHash.Length = data.ReadUInt32();
            archiveHash.Hash = data.ReadBytes(0x10);

            return archiveHash;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package directory item tree
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package directory item tree on success, null on error</returns>
        private static DirectoryItem[] ParseDirectoryItemTree(Stream data)
        {
            // Create the directory items list
            var directoryItems = new List<DirectoryItem>();

            while (true)
            {
                // Get the extension
                string extensionString = data.ReadString(Encoding.ASCII);
                if (string.IsNullOrEmpty(extensionString))
                    break;

                // Sanitize the extension
                for (int i = 0; i < 0x20; i++)
                {
                    extensionString = extensionString.Replace($"{(char)i}", string.Empty);
                }

                while (true)
                {
                    // Get the path
                    string pathString = data.ReadString(Encoding.ASCII);
                    if (string.IsNullOrEmpty(pathString))
                        break;

                    // Sanitize the path
                    for (int i = 0; i < 0x20; i++)
                    {
                        pathString = pathString.Replace($"{(char)i}", string.Empty);
                    }

                    while (true)
                    {
                        // Get the name
                        string nameString = data.ReadString(Encoding.ASCII);
                        if (string.IsNullOrEmpty(nameString))
                            break;

                        // Sanitize the name
                        for (int i = 0; i < 0x20; i++)
                        {
                            nameString = nameString.Replace($"{(char)i}", string.Empty);
                        }

                        // Get the directory item
                        var directoryItem = ParseDirectoryItem(data, extensionString, pathString, nameString);

                        // Add the directory item
                        directoryItems.Add(directoryItem);
                    }
                }
            }

            return directoryItems.ToArray();
        }

        /// <summary>
        /// Parse a Stream into a Valve Package directory item
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package directory item on success, null on error</returns>
        private static DirectoryItem ParseDirectoryItem(Stream data, string extension, string path, string name)
        {
            DirectoryItem directoryItem = new DirectoryItem();

            directoryItem.Extension = extension;
            directoryItem.Path = path;
            directoryItem.Name = name;

            // Get the directory entry
            var directoryEntry = ParseDirectoryEntry(data);

            // Set the directory entry
            directoryItem.DirectoryEntry = directoryEntry;

            // Get the preload data pointer
            long preloadDataPointer = -1; int preloadDataLength = -1;
            if (directoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE && directoryEntry.EntryLength > 0)
            {
                preloadDataPointer = directoryEntry.EntryOffset;
                preloadDataLength = (int)directoryEntry.EntryLength;
            }
            else if (directoryEntry.PreloadBytes > 0)
            {
                preloadDataPointer = data.Position;
                preloadDataLength = directoryEntry.PreloadBytes;
            }

            // If we had a valid preload data pointer
            byte[] preloadData = null;
            if (preloadDataPointer >= 0 && preloadDataLength > 0)
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Seek to the preload data offset
                data.Seek(preloadDataPointer, SeekOrigin.Begin);

                // Read the preload data
                preloadData = data.ReadBytes(preloadDataLength);

                // Seek back to the original offset
                data.Seek(initialOffset, SeekOrigin.Begin);
            }

            // Set the preload data
            directoryItem.PreloadData = preloadData;

            return directoryItem;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package directory entry on success, null on error</returns>
        private static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryEntry directoryEntry = new DirectoryEntry();

            directoryEntry.CRC = data.ReadUInt32();
            directoryEntry.PreloadBytes = data.ReadUInt16();
            directoryEntry.ArchiveIndex = data.ReadUInt16();
            directoryEntry.EntryOffset = data.ReadUInt32();
            directoryEntry.EntryLength = data.ReadUInt32();
            directoryEntry.Dummy0 = data.ReadUInt16();

            return directoryEntry;
        }

        #endregion
    }
}
