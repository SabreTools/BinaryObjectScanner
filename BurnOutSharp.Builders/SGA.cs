using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.SGA;
using BurnOutSharp.Utilities;

namespace BurnOutSharp.Builders
{
    public static class SGA
    {
        #region Constants

        /// <summary>
        /// Length of a SGA checksum in bytes
        /// </summary>
        public const int HL_SGA_CHECKSUM_LENGTH = 0x00008000;

        #endregion

        #region Byte Data

        /// <summary>
        /// Parse a byte array into an SGA
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled SGA on success, null on error</returns>
        public static Models.SGA.File ParseFile(byte[] data, int offset)
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
        /// Parse a Stream into an SGA
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA on success, null on error</returns>
        public static Models.SGA.File ParseFile(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new SGA to fill
            var file = new Models.SGA.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the SGA header
            file.Header = header;

            #endregion

            #region Directory

            // Try to parse the directory
            var directory = ParseDirectory(data, header.MajorVersion);
            if (directory == null)
                return null;

            // Set the SGA directory
            file.Directory = directory;
            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into an SGA header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA header on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            byte[] signatureBytes = data.ReadBytes(8);
            string signature = Encoding.ASCII.GetString(signatureBytes);
            if (signature != "_ARCHIVE")
                return null;

            ushort majorVersion = data.ReadUInt16();
            ushort minorVersion = data.ReadUInt16();
            if (minorVersion != 0)
                return null;

            switch (majorVersion)
            {
                // Versions 4 and 5 share the same header
                case 4:
                case 5:
                    Header4 header4 = new Header4();

                    header4.Signature = signature;
                    header4.MajorVersion = majorVersion;
                    header4.MinorVersion = minorVersion;
                    header4.FileMD5 = data.ReadBytes(0x10);
                    byte[] header4Name = data.ReadBytes(count: 128);
                    header4.Name = Encoding.Unicode.GetString(header4Name).TrimEnd('\0');
                    header4.HeaderMD5 = data.ReadBytes(0x10);
                    header4.HeaderLength = data.ReadUInt32();
                    header4.FileDataOffset = data.ReadUInt32();
                    header4.Dummy0 = data.ReadUInt32();

                    return header4;

                // Versions 6 and 7 share the same header
                case 6:
                case 7:
                    Header6 header6 = new Header6();

                    header6.Signature = signature;
                    header6.MajorVersion = majorVersion;
                    header6.MinorVersion = minorVersion;
                    byte[] header6Name = data.ReadBytes(count: 128);
                    header6.Name = Encoding.Unicode.GetString(header6Name).TrimEnd('\0');
                    header6.HeaderLength = data.ReadUInt32();
                    header6.FileDataOffset = data.ReadUInt32();
                    header6.Dummy0 = data.ReadUInt32();

                    return header6;

                // No other major versions are recognized
                default:
                    return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an SGA directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA directory on success, null on error</returns>
        private static Models.SGA.Directory ParseDirectory(Stream data, ushort majorVersion)
        {
            #region Directory

            // Create the appropriate type of directory
            Models.SGA.Directory directory;
            switch (majorVersion)
            {
                case 4: directory = new Directory4(); break;
                case 5: directory = new Directory5(); break;
                case 6: directory = new Directory6(); break;
                case 7: directory = new Directory7(); break;
                default: return null;
            }

            #endregion

            // Cache the current offset
            long currentOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = ParseDirectoryHeader(data, majorVersion);
            if (directoryHeader == null)
                return null;

            // Set the directory header
            switch (majorVersion)
            {
                case 4: (directory as Directory4).DirectoryHeader = directoryHeader as DirectoryHeader4; break;
                case 5: (directory as Directory5).DirectoryHeader = directoryHeader as DirectoryHeader5; break;
                case 6: (directory as Directory6).DirectoryHeader = directoryHeader as DirectoryHeader5; break;
                case 7: (directory as Directory7).DirectoryHeader = directoryHeader as DirectoryHeader7; break;
                default: return null;
            }

            #endregion

            #region Sections

            // Get the sections offset
            long sectionOffset;
            switch (majorVersion)
            {
                case 4: sectionOffset = (directoryHeader as DirectoryHeader4).SectionOffset; break;
                case 5:
                case 6: sectionOffset = (directoryHeader as DirectoryHeader5).SectionOffset; break;
                case 7: sectionOffset = (directoryHeader as DirectoryHeader7).SectionOffset; break;
                default: return null;
            }

            // Adjust the sections offset based on the directory
            sectionOffset += currentOffset;

            // Validate the offset
            if (sectionOffset < 0 || sectionOffset >= data.Length)
                return null;

            // Seek to the sections
            data.Seek(sectionOffset, SeekOrigin.Begin);

            // Get the section count
            uint sectionCount;
            switch (majorVersion)
            {
                case 4: sectionCount = (directoryHeader as DirectoryHeader4).SectionCount; break;
                case 5:
                case 6: sectionCount = (directoryHeader as DirectoryHeader5).SectionCount; break;
                case 7: sectionCount = (directoryHeader as DirectoryHeader7).SectionCount; break;
                default: return null;
            }

            // Create the sections array
            object[] sections;
            switch (majorVersion)
            {
                case 4: sections = new Section4[sectionCount]; break;
                case 5:
                case 6:
                case 7: sections = new Section5[sectionCount]; break;
                default: return null;
            }

            // Try to parse the sections
            for (int i = 0; i < sections.Length; i++)
            {
                switch (majorVersion)
                {
                    case 4: sections[i] = ParseSection4(data); break;
                    case 5:
                    case 6:
                    case 7: sections[i] = ParseSection5(data); break;
                    default: return null;
                }
            }

            // Assign the sections
            switch (majorVersion)
            {
                case 4: (directory as Directory4).Sections = sections as Section4[]; break;
                case 5: (directory as Directory5).Sections = sections as Section5[]; break;
                case 6: (directory as Directory6).Sections = sections as Section5[]; break;
                case 7: (directory as Directory7).Sections = sections as Section5[]; break;
                default: return null;
            }

            #endregion

            #region Folders

            // Get the folders offset
            long folderOffset;
            switch (majorVersion)
            {
                case 4: folderOffset = (directoryHeader as DirectoryHeader4).FolderOffset; break;
                case 5: folderOffset = (directoryHeader as DirectoryHeader5).FolderOffset; break;
                case 6: folderOffset = (directoryHeader as DirectoryHeader5).FolderOffset; break;
                case 7: folderOffset = (directoryHeader as DirectoryHeader7).FolderOffset; break;
                default: return null;
            }

            // Adjust the folders offset based on the directory
            folderOffset += currentOffset;

            // Validate the offset
            if (folderOffset < 0 || folderOffset >= data.Length)
                return null;

            // Seek to the folders
            data.Seek(folderOffset, SeekOrigin.Begin);

            // Get the folder count
            uint folderCount;
            switch (majorVersion)
            {
                case 4: folderCount = (directoryHeader as DirectoryHeader4).FolderCount; break;
                case 5: folderCount = (directoryHeader as DirectoryHeader5).FolderCount; break;
                case 6: folderCount = (directoryHeader as DirectoryHeader5).FolderCount; break;
                case 7: folderCount = (directoryHeader as DirectoryHeader7).FolderCount; break;
                default: return null;
            }

            // Create the folders array
            object[] folders;
            switch (majorVersion)
            {
                case 4: folders = new Folder4[folderCount]; break;
                case 5: folders = new Folder5[folderCount]; break;
                case 6: folders = new Folder5[folderCount]; break;
                case 7: folders = new Folder5[folderCount]; break;
                default: return null;
            }

            // Try to parse the folders
            for (int i = 0; i < folders.Length; i++)
            {
                switch (majorVersion)
                {
                    case 4: folders[i] = ParseFolder4(data); break;
                    case 5: folders[i] = ParseFolder5(data); break;
                    case 6: folders[i] = ParseFolder5(data); break;
                    case 7: folders[i] = ParseFolder5(data); break;
                    default: return null;
                }
            }

            // Assign the folders
            switch (majorVersion)
            {
                case 4: (directory as Directory4).Folders = folders as Folder4[]; break;
                case 5: (directory as Directory5).Folders = folders as Folder5[]; break;
                case 6: (directory as Directory6).Folders = folders as Folder5[]; break;
                case 7: (directory as Directory7).Folders = folders as Folder5[]; break;
                default: return null;
            }

            #endregion

            #region Files

            // Get the files offset
            long fileOffset;
            switch (majorVersion)
            {
                case 4: fileOffset = (directoryHeader as DirectoryHeader4).FileOffset; break;
                case 5: fileOffset = (directoryHeader as DirectoryHeader5).FileOffset; break;
                case 6: fileOffset = (directoryHeader as DirectoryHeader5).FileOffset; break;
                case 7: fileOffset = (directoryHeader as DirectoryHeader7).FileOffset; break;
                default: return null;
            }

            // Adjust the files offset based on the directory
            fileOffset += currentOffset;

            // Validate the offset
            if (fileOffset < 0 || fileOffset >= data.Length)
                return null;

            // Seek to the files
            data.Seek(fileOffset, SeekOrigin.Begin);

            // Get the file count
            uint fileCount;
            switch (majorVersion)
            {
                case 4: fileCount = (directoryHeader as DirectoryHeader4).FileCount; break;
                case 5: fileCount = (directoryHeader as DirectoryHeader5).FileCount; break;
                case 6: fileCount = (directoryHeader as DirectoryHeader5).FileCount; break;
                case 7: fileCount = (directoryHeader as DirectoryHeader7).FileCount; break;
                default: return null;
            }

            // Create the files array
            object[] files;
            switch (majorVersion)
            {
                case 4: files = new File4[fileCount]; break;
                case 5: files = new File4[fileCount]; break;
                case 6: files = new File6[fileCount]; break;
                case 7: files = new File7[fileCount]; break;
                default: return null;
            }

            // Try to parse the files
            for (int i = 0; i < files.Length; i++)
            {
                switch (majorVersion)
                {
                    case 4: files[i] = ParseFile4(data); break;
                    case 5: files[i] = ParseFile4(data); break;
                    case 6: files[i] = ParseFile6(data); break;
                    case 7: files[i] = ParseFile7(data); break;
                    default: return null;
                }
            }

            // Assign the files
            switch (majorVersion)
            {
                case 4: (directory as Directory4).Files = files as File4[]; break;
                case 5: (directory as Directory5).Files = files as File4[]; break;
                case 6: (directory as Directory6).Files = files as File6[]; break;
                case 7: (directory as Directory7).Files = files as File7[]; break;
                default: return null;
            }

            #endregion

            #region String Table

            // Get the string table offset
            long stringTableOffset;
            switch (majorVersion)
            {
                case 4: stringTableOffset = (directoryHeader as DirectoryHeader4).StringTableOffset; break;
                case 5: stringTableOffset = (directoryHeader as DirectoryHeader5).StringTableOffset; break;
                case 6: stringTableOffset = (directoryHeader as DirectoryHeader5).StringTableOffset; break;
                case 7: stringTableOffset = (directoryHeader as DirectoryHeader7).StringTableOffset; break;
                default: return null;
            }

            // Adjust the string table offset based on the directory
            stringTableOffset += currentOffset;

            // Validate the offset
            if (stringTableOffset < 0 || stringTableOffset >= data.Length)
                return null;

            // Seek to the string table
            data.Seek(stringTableOffset, SeekOrigin.Begin);

            // Get the string table count
            uint stringCount;
            switch (majorVersion)
            {
                case 4: stringCount = (directoryHeader as DirectoryHeader4).StringTableCount; break;
                case 5: stringCount = (directoryHeader as DirectoryHeader5).StringTableCount; break;
                case 6: stringCount = (directoryHeader as DirectoryHeader5).StringTableCount; break;
                case 7: stringCount = (directoryHeader as DirectoryHeader7).StringTableCount; break;
                default: return null;
            }

            // TODO: Are these strings actually indexed by number and not position?
            // TODO: If indexed by position, I think it needs to be adjusted by start of table

            // Create the strings dictionary
            Dictionary<long, string> strings = new Dictionary<long, string>((int)stringCount);

            // Get the current position to adjust the offsets
            long stringTableStart = data.Position;

            // Try to parse the strings
            for (int i = 0; i < stringCount; i++)
            {
                long currentPosition = data.Position - stringTableStart;
                strings[currentPosition] = data.ReadString(Encoding.ASCII);
            }

            // Assign the files
            switch (majorVersion)
            {
                case 4: (directory as Directory4).StringTable = strings; break;
                case 5: (directory as Directory5).StringTable = strings; break;
                case 6: (directory as Directory6).StringTable = strings; break;
                case 7: (directory as Directory7).StringTable = strings; break;
                default: return null;
            }

            // Loop through all folders to assign names
            for (int i = 0; i < folderCount; i++)
            {
                switch (majorVersion)
                {
                    case 4: (directory as Directory4).Folders[i].Name = strings[(directory as Directory4).Folders[i].NameOffset]; break;
                    case 5: (directory as Directory4).Folders[i].Name = strings[(directory as Directory4).Folders[i].NameOffset]; break;
                    case 6: (directory as Directory4).Folders[i].Name = strings[(directory as Directory4).Folders[i].NameOffset]; break;
                    case 7: (directory as Directory4).Folders[i].Name = strings[(directory as Directory4).Folders[i].NameOffset]; break;
                    default: return null;
                }
            }

            // Loop through all files to assign names
            for (int i = 0; i < fileCount; i++)
            {
                switch (majorVersion)
                {
                    case 4: (directory as Directory4).Files[i].Name = strings[(directory as Directory4).Files[i].NameOffset]; break;
                    case 5: (directory as Directory4).Files[i].Name = strings[(directory as Directory4).Files[i].NameOffset]; break;
                    case 6: (directory as Directory4).Files[i].Name = strings[(directory as Directory4).Files[i].NameOffset]; break;
                    case 7: (directory as Directory4).Files[i].Name = strings[(directory as Directory4).Files[i].NameOffset]; break;
                    default: return null;
                }
            }

            #endregion

            return directory;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA directory header on success, null on error</returns>
        private static object ParseDirectoryHeader(Stream data, ushort majorVersion)
        {
            switch (majorVersion)
            {
                case 4: return ParseDirectory4Header(data);
                case 5: return ParseDirectory5Header(data);
                case 6: return ParseDirectory5Header(data);
                case 7: return ParseDirectory7Header(data);
                default: return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an SGA directory header version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory header version 4 on success, null on error</returns>
        private static DirectoryHeader4 ParseDirectory4Header(Stream data)
        {
            DirectoryHeader4 directoryHeader4 = new DirectoryHeader4();

            directoryHeader4.SectionOffset = data.ReadUInt32();
            directoryHeader4.SectionCount = data.ReadUInt16();
            directoryHeader4.FolderOffset = data.ReadUInt32();
            directoryHeader4.FolderCount = data.ReadUInt16();
            directoryHeader4.FileOffset = data.ReadUInt32();
            directoryHeader4.FileCount = data.ReadUInt16();
            directoryHeader4.StringTableOffset = data.ReadUInt32();
            directoryHeader4.StringTableCount = data.ReadUInt16();

            return directoryHeader4;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory header version 5
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory header version 5 on success, null on error</returns>
        private static DirectoryHeader5 ParseDirectory5Header(Stream data)
        {
            DirectoryHeader5 directoryHeader5 = new DirectoryHeader5();

            directoryHeader5.SectionOffset = data.ReadUInt32();
            directoryHeader5.SectionCount = data.ReadUInt32();
            directoryHeader5.FolderOffset = data.ReadUInt32();
            directoryHeader5.FolderCount = data.ReadUInt32();
            directoryHeader5.FileOffset = data.ReadUInt32();
            directoryHeader5.FileCount = data.ReadUInt32();
            directoryHeader5.StringTableOffset = data.ReadUInt32();
            directoryHeader5.StringTableCount = data.ReadUInt32();

            return directoryHeader5;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory header version 7
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory header version 7 on success, null on error</returns>
        private static DirectoryHeader7 ParseDirectory7Header(Stream data)
        {
            DirectoryHeader7 directoryHeader7 = new DirectoryHeader7();

            directoryHeader7.SectionOffset = data.ReadUInt32();
            directoryHeader7.SectionCount = data.ReadUInt32();
            directoryHeader7.FolderOffset = data.ReadUInt32();
            directoryHeader7.FolderCount = data.ReadUInt32();
            directoryHeader7.FileOffset = data.ReadUInt32();
            directoryHeader7.FileCount = data.ReadUInt32();
            directoryHeader7.StringTableOffset = data.ReadUInt32();
            directoryHeader7.StringTableCount = data.ReadUInt32();
            directoryHeader7.HashTableOffset = data.ReadUInt32();
            directoryHeader7.BlockSize = data.ReadUInt32();

            return directoryHeader7;
        }

        /// <summary>
        /// Parse a Stream into an SGA section version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA section version 4 on success, null on error</returns>
        private static Section4 ParseSection4(Stream data)
        {
            Section4 section4 = new Section4();

            byte[] section4Alias = data.ReadBytes(count: 64);
            section4.Alias = Encoding.ASCII.GetString(section4Alias).TrimEnd('\0');
            byte[] section4Name = data.ReadBytes(64);
            section4.Name = Encoding.ASCII.GetString(section4Name).TrimEnd('\0');
            section4.FolderStartIndex = data.ReadUInt16();
            section4.FolderEndIndex = data.ReadUInt16();
            section4.FileStartIndex = data.ReadUInt16();
            section4.FileEndIndex = data.ReadUInt16();
            section4.FolderRootIndex = data.ReadUInt16();

            return section4;
        }

        /// <summary>
        /// Parse a Stream into an SGA section version 5
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA section version 5 on success, null on error</returns>
        private static Section5 ParseSection5(Stream data)
        {
            Section5 section5 = new Section5();

            byte[] section5Alias = data.ReadBytes(count: 64);
            section5.Alias = Encoding.ASCII.GetString(section5Alias).TrimEnd('\0');
            byte[] section5Name = data.ReadBytes(64);
            section5.Name = Encoding.ASCII.GetString(section5Name).TrimEnd('\0');
            section5.FolderStartIndex = data.ReadUInt32();
            section5.FolderEndIndex = data.ReadUInt32();
            section5.FileStartIndex = data.ReadUInt32();
            section5.FileEndIndex = data.ReadUInt32();
            section5.FolderRootIndex = data.ReadUInt32();

            return section5;
        }

        /// <summary>
        /// Parse a Stream into an SGA folder version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA folder version 4 on success, null on error</returns>
        private static Folder4 ParseFolder4(Stream data)
        {
            Folder4 folder4 = new Folder4();

            folder4.NameOffset = data.ReadUInt32();
            folder4.Name = null; // Read from string table
            folder4.FolderStartIndex = data.ReadUInt16();
            folder4.FolderEndIndex = data.ReadUInt16();
            folder4.FileStartIndex = data.ReadUInt16();
            folder4.FileEndIndex = data.ReadUInt16();

            return folder4;
        }

        /// <summary>
        /// Parse a Stream into an SGA folder version 5
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA folder version 5 on success, null on error</returns>
        private static Folder5 ParseFolder5(Stream data)
        {
            Folder5 folder5 = new Folder5();

            folder5.NameOffset = data.ReadUInt32();
            folder5.Name = null; // Read from string table
            folder5.FolderStartIndex = data.ReadUInt32();
            folder5.FolderEndIndex = data.ReadUInt32();
            folder5.FileStartIndex = data.ReadUInt32();
            folder5.FileEndIndex = data.ReadUInt32();

            return folder5;
        }

        /// <summary>
        /// Parse a Stream into an SGA file version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA file version 4 on success, null on error</returns>
        private static File4 ParseFile4(Stream data)
        {
            File4 file4 = new File4();

            file4.NameOffset = data.ReadUInt32();
            file4.Name = null; // Read from string table
            file4.Offset = data.ReadUInt32();
            file4.SizeOnDisk = data.ReadUInt32();
            file4.Size = data.ReadUInt32();
            file4.TimeModified = data.ReadUInt32();
            file4.Dummy0 = data.ReadByteValue();
            file4.Type = data.ReadByteValue();

            return file4;
        }

        /// <summary>
        /// Parse a Stream into an SGA file version 6
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA file version 6 on success, null on error</returns>
        private static File6 ParseFile6(Stream data)
        {
            File6 file6 = new File6();

            file6.NameOffset = data.ReadUInt32();
            file6.Name = null; // Read from string table
            file6.Offset = data.ReadUInt32();
            file6.SizeOnDisk = data.ReadUInt32();
            file6.Size = data.ReadUInt32();
            file6.TimeModified = data.ReadUInt32();
            file6.Dummy0 = data.ReadByteValue();
            file6.Type = data.ReadByteValue();
            file6.CRC32 = data.ReadUInt32();

            return file6;
        }

        /// <summary>
        /// Parse a Stream into an SGA file version 7
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA file version 7 on success, null on error</returns>
        private static File7 ParseFile7(Stream data)
        {
            File7 file7 = new File7();

            file7.NameOffset = data.ReadUInt32();
            file7.Name = null; // Read from string table
            file7.Offset = data.ReadUInt32();
            file7.SizeOnDisk = data.ReadUInt32();
            file7.Size = data.ReadUInt32();
            file7.TimeModified = data.ReadUInt32();
            file7.Dummy0 = data.ReadByteValue();
            file7.Type = data.ReadByteValue();
            file7.CRC32 = data.ReadUInt32();
            file7.HashOffset = data.ReadUInt32();

            return file7;
        }

        #endregion
    }
}
