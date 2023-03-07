using System.IO;
using System.Text;
using BurnOutSharp.Models.XZP;
using BinaryObjectScanner.Utilities;
using static BurnOutSharp.Models.XZP.Constants;

namespace BurnOutSharp.Builders
{
    public static class XZP
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a XBox Package File
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled XBox Package File on success, null on error</returns>
        public static Models.XZP.File ParseFile(byte[] data, int offset)
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
        /// Parse a Stream into a XBox Package File
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File on success, null on error</returns>
        public static Models.XZP.File ParseFile(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new XBox Package File to fill
            var file = new Models.XZP.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Directory Entries

            // Create the directory entry array
            file.DirectoryEntries = new DirectoryEntry[header.DirectoryEntryCount];

            // Try to parse the directory entries
            for (int i = 0; i < header.DirectoryEntryCount; i++)
            {
                var directoryEntry = ParseDirectoryEntry(data);
                file.DirectoryEntries[i] = directoryEntry;
            }

            #endregion

            #region Preload Directory Entries

            if (header.PreloadBytes > 0)
            {
                // Create the preload directory entry array
                file.PreloadDirectoryEntries = new DirectoryEntry[header.PreloadDirectoryEntryCount];

                // Try to parse the preload directory entries
                for (int i = 0; i < header.PreloadDirectoryEntryCount; i++)
                {
                    var directoryEntry = ParseDirectoryEntry(data);
                    file.PreloadDirectoryEntries[i] = directoryEntry;
                }
            }

            #endregion

            #region Preload Directory Mappings

            if (header.PreloadBytes > 0)
            {
                // Create the preload directory mapping array
                file.PreloadDirectoryMappings = new DirectoryMapping[header.PreloadDirectoryEntryCount];

                // Try to parse the preload directory mappings
                for (int i = 0; i < header.PreloadDirectoryEntryCount; i++)
                {
                    var directoryMapping = ParseDirectoryMapping(data);
                    file.PreloadDirectoryMappings[i] = directoryMapping;
                }
            }

            #endregion

            #region Directory Items

            if (header.DirectoryItemCount > 0)
            {
                // Get the directory item offset
                uint directoryItemOffset = header.DirectoryItemOffset;
                if (directoryItemOffset < 0 || directoryItemOffset >= data.Length)
                    return null;
                
                // Seek to the directory items
                data.Seek(directoryItemOffset, SeekOrigin.Begin);

                // Create the directory item array
                file.DirectoryItems = new DirectoryItem[header.DirectoryItemCount];

                // Try to parse the directory items
                for (int i = 0; i < header.DirectoryItemCount; i++)
                {
                    var directoryItem = ParseDirectoryItem(data);
                    file.DirectoryItems[i] = directoryItem;
                }
            }

            #endregion

            #region Footer

            // Seek to the footer
            data.Seek(-8, SeekOrigin.End);

            // Try to parse the footer
            var footer = ParseFooter(data);
            if (footer == null)
                return null;

            // Set the package footer
            file.Footer = footer;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File header on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Header header = new Header();

            byte[] signature = data.ReadBytes(4);
            header.Signature = Encoding.ASCII.GetString(signature);
            if (header.Signature != HeaderSignatureString)
                return null;

            header.Version = data.ReadUInt32();
            if (header.Version != 6)
                return null;

            header.PreloadDirectoryEntryCount = data.ReadUInt32();
            header.DirectoryEntryCount = data.ReadUInt32();
            header.PreloadBytes = data.ReadUInt32();
            header.HeaderLength = data.ReadUInt32();
            header.DirectoryItemCount = data.ReadUInt32();
            header.DirectoryItemOffset = data.ReadUInt32();
            header.DirectoryItemLength = data.ReadUInt32();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File directory entry on success, null on error</returns>
        private static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryEntry directoryEntry = new DirectoryEntry();

            directoryEntry.FileNameCRC = data.ReadUInt32();
            directoryEntry.EntryLength = data.ReadUInt32();
            directoryEntry.EntryOffset = data.ReadUInt32();

            return directoryEntry;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File directory mapping
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File directory mapping on success, null on error</returns>
        private static DirectoryMapping ParseDirectoryMapping(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryMapping directoryMapping = new DirectoryMapping();

            directoryMapping.PreloadDirectoryEntryIndex = data.ReadUInt16();

            return directoryMapping;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File directory item
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File directory item on success, null on error</returns>
        private static DirectoryItem ParseDirectoryItem(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryItem directoryItem = new DirectoryItem();

            directoryItem.FileNameCRC = data.ReadUInt32();
            directoryItem.NameOffset = data.ReadUInt32();
            directoryItem.TimeCreated = data.ReadUInt32();

            // Cache the current offset
            long currentPosition = data.Position;

            // Seek to the name offset
            data.Seek(directoryItem.NameOffset, SeekOrigin.Begin);

            // Read the name
            directoryItem.Name = data.ReadString(Encoding.ASCII);

            // Seek back to the right position
            data.Seek(currentPosition, SeekOrigin.Begin);

            return directoryItem;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File footer on success, null on error</returns>
        private static Footer ParseFooter(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Footer footer = new Footer();

            footer.FileLength = data.ReadUInt32();
            byte[] signature = data.ReadBytes(4);
            footer.Signature = Encoding.ASCII.GetString(signature);
            if (footer.Signature != FooterSignatureString)
                return null;

            return footer;
        }

        #endregion
    }
}
