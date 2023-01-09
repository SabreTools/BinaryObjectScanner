using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.CFB;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.CFB.Constants;

namespace BurnOutSharp.Builders
{
    public class CFB
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Compound File Binary
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Compound File Binary on success, null on error</returns>
        public static Binary ParseBinary(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseBinary(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Compound File Binary
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Compound File Binary on success, null on error</returns>
        public static Binary ParseBinary(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new binary to fill
            var binary = new Binary();

            #region Header

            // Try to parse the file header
            var fileHeader = ParseFileHeader(data);
            if (fileHeader == null)
                return null;

            // Set the file header
            binary.Header = fileHeader;

            #endregion

            #region DIFAT Sector Numbers

            // Create a DIFAT sector table
            var difatSectors = new List<SectorNumber>();

            // Add the sectors from the header
            difatSectors.AddRange(fileHeader.DIFAT);

            // Loop through and add the DIFAT sectors
            SectorNumber currentSector = (SectorNumber)fileHeader.FirstDIFATSectorLocation;
            for (int i = 0; i < fileHeader.NumberOfDIFATSectors; i++)
            {
                // If we have a readable sector
                if (currentSector <= SectorNumber.MAXREGSECT)
                {
                    // Get the new next sector information
                    long sectorOffset = (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < 0 || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var sectorNumbers = ParseSectorNumbers(data, fileHeader.SectorShift);
                    if (sectorNumbers == null)
                        return null;

                    // Add the sector shifts
                    difatSectors.AddRange(sectorNumbers);
                }

                // Get the next sector from the DIFAT
                currentSector = difatSectors[i];
            }

            // Assign the DIFAT sectors table
            binary.DIFATSectorNumbers = difatSectors.ToArray();

            #endregion

            #region FAT Sector Numbers

            // Create a FAT sector table
            var fatSectors = new List<SectorNumber>();

            // Loop through and add the FAT sectors
            currentSector = binary.DIFATSectorNumbers[0];
            for (int i = 0; i < fileHeader.NumberOfFATSectors; i++)
            {
                // If we have a readable sector
                if (currentSector <= SectorNumber.MAXREGSECT)
                {
                    // Get the new next sector information
                    long sectorOffset = (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < 0 || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var sectorNumbers = ParseSectorNumbers(data, fileHeader.SectorShift);
                    if (sectorNumbers == null)
                        return null;

                    // Add the sector shifts
                    fatSectors.AddRange(sectorNumbers);
                }

                // Get the next sector from the DIFAT
                currentSector = binary.DIFATSectorNumbers[i];
            }

            // Assign the FAT sectors table
            binary.FATSectorNumbers = fatSectors.ToArray();

            #endregion

            #region Mini FAT Sector Numbers

            // Create a mini FAT sector table
            var miniFatSectors = new List<SectorNumber>();

            // Loop through and add the mini FAT sectors
            currentSector = (SectorNumber)fileHeader.FirstMiniFATSectorLocation;
            for (int i = 0; i < fileHeader.NumberOfMiniFATSectors; i++)
            {
                // If we have a readable sector
                if (currentSector <= SectorNumber.MAXREGSECT)
                {
                    // Get the new next sector information
                    long sectorOffset = (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < 0 || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var sectorNumbers = ParseSectorNumbers(data, fileHeader.SectorShift);
                    if (sectorNumbers == null)
                        return null;

                    // Add the sector shifts
                    miniFatSectors.AddRange(sectorNumbers);
                }

                // Get the next sector from the DIFAT
                currentSector = binary.DIFATSectorNumbers[i];
            }

            // Assign the mini FAT sectors table
            binary.MiniFATSectorNumbers = miniFatSectors.ToArray();

            #endregion

            #region Directory Entries

            // Get the offset of the first directory sector
            long firstDirectoryOffset = (long)(fileHeader.FirstDirectorySectorLocation * Math.Pow(2, fileHeader.SectorShift));
            if (firstDirectoryOffset < 0 || firstDirectoryOffset >= data.Length)
                return null;

            // Seek to the first directory sector
            data.Seek(firstDirectoryOffset, SeekOrigin.Begin);

            // Create a directory sector table
            var directorySectors = new List<DirectoryEntry>();

            // Get the number of directory sectors
            uint directorySectorCount = 0;
            switch (fileHeader.MajorVersion)
            {
                case 3:
                    directorySectorCount = int.MaxValue;
                    break;
                case 4:
                    directorySectorCount = fileHeader.NumberOfDirectorySectors;
                    break;
            }

            // Loop through and add the directory sectors
            currentSector = (SectorNumber)fileHeader.FirstDirectorySectorLocation;
            for (int i = 0; i < directorySectorCount; i++)
            {
                // If we have an end of chain
                if (currentSector == SectorNumber.ENDOFCHAIN)
                    break;

                // If we have a readable sector
                if (currentSector <= SectorNumber.MAXREGSECT)
                {
                    // Get the new next sector information
                    long sectorOffset = (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < 0 || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var directoryEntries = ParseDirectoryEntries(data, fileHeader.SectorShift);
                    if (directoryEntries == null)
                        return null;

                    // Add the sector shifts
                    directorySectors.AddRange(directoryEntries);
                }

                // Get the next sector from the DIFAT
                currentSector = binary.DIFATSectorNumbers[i];
            }

            // Assign the Directory sectors table
            binary.DirectoryEntries = directorySectors.ToArray();

            #endregion

            return binary;
        }

        /// <summary>
        /// Parse a Stream into a file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled file header on success, null on error</returns>
        private static FileHeader ParseFileHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            FileHeader header = new FileHeader();

            header.Signature = data.ReadUInt64();
            if (header.Signature != SignatureUInt64)
                return null;

            header.CLSID = data.ReadGuid();
            header.MinorVersion = data.ReadUInt16();
            header.MajorVersion = data.ReadUInt16();
            header.ByteOrder = data.ReadUInt16();
            if (header.ByteOrder != 0xFFFE)
                return null;

            header.SectorShift = data.ReadUInt16();
            if (header.MajorVersion == 3 && header.SectorShift != 0x0009)
                return null;
            else if (header.MajorVersion == 4 && header.SectorShift != 0x000C)
                return null;

            header.MiniSectorShift = data.ReadUInt16();
            header.Reserved = data.ReadBytes(6);
            header.NumberOfDirectorySectors = data.ReadUInt32();
            if (header.MajorVersion == 3 && header.NumberOfDirectorySectors != 0)
                return null;

            header.NumberOfFATSectors = data.ReadUInt32();
            header.FirstDirectorySectorLocation = data.ReadUInt32();
            header.TransactionSignatureNumber = data.ReadUInt32();
            header.MiniStreamCutoffSize = data.ReadUInt32();
            if (header.MiniStreamCutoffSize != 0x00001000)
                return null;

            header.FirstMiniFATSectorLocation = data.ReadUInt32();
            header.NumberOfMiniFATSectors = data.ReadUInt32();
            header.FirstDIFATSectorLocation = data.ReadUInt32();
            header.NumberOfDIFATSectors = data.ReadUInt32();
            header.DIFAT = new SectorNumber[109];
            for (int i = 0; i < header.DIFAT.Length; i++)
            {
                header.DIFAT[i] = (SectorNumber)data.ReadUInt32();
            }

            // Skip rest of sector for version 4
            if (header.MajorVersion == 4)
                _ = data.ReadBytes(3584);

            return header;
        }

        /// <summary>
        /// Parse a Stream into a sector full of sector numbers
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorShift">Sector shift from the header</param>
        /// <returns>Filled sector full of sector numbers on success, null on error</returns>
        private static SectorNumber[] ParseSectorNumbers(Stream data, ushort sectorShift)
        {
            // TODO: Use marshalling here instead of building
            int sectorCount = (int)(Math.Pow(2, sectorShift) / sizeof(uint));
            SectorNumber[] sectorNumbers = new SectorNumber[sectorCount];

            for (int i = 0; i < sectorNumbers.Length; i++)
            {
                sectorNumbers[i] = (SectorNumber)data.ReadUInt32();
            }

            return sectorNumbers;
        }

        /// <summary>
        /// Parse a Stream into a sector full of directory entries
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorShift">Sector shift from the header</param>
        /// <returns>Filled sector full of directory entries on success, null on error</returns>
        private static DirectoryEntry[] ParseDirectoryEntries(Stream data, ushort sectorShift)
        {
            // TODO: Use marshalling here instead of building
            const int directoryEntrySize = 64 + 2 + 1 + 1 + 4 + 4 + 4 + 16 + 4 + 8 + 8 + 4 + 8;
            int sectorCount = (int)(Math.Pow(2, sectorShift) / directoryEntrySize);
            DirectoryEntry[] directoryEntries = new DirectoryEntry[sectorCount];

            for (int i = 0; i < directoryEntries.Length; i++)
            {
                var directoryEntry = ParseDirectoryEntry(data);
                if (directoryEntry == null)
                    return null;

                directoryEntries[i] = directoryEntry;
            }

            return directoryEntries;
        }

        /// <summary>
        /// Parse a Stream into a directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled directory entry on success, null on error</returns>
        private static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryEntry directoryEntry = new DirectoryEntry();

            byte[] name = data.ReadBytes(64);
            directoryEntry.Name = Encoding.Unicode.GetString(name).TrimEnd('\0');
            directoryEntry.NameLength = data.ReadUInt16();
            directoryEntry.ObjectType = (ObjectType)data.ReadByteValue();
            directoryEntry.ColorFlag = (ColorFlag)data.ReadByteValue();
            directoryEntry.LeftSiblingID = (StreamID)data.ReadUInt32();
            directoryEntry.RightSiblingID = (StreamID)data.ReadUInt32();
            directoryEntry.ChildID = (StreamID)data.ReadUInt32();
            directoryEntry.CLSID = data.ReadGuid();
            directoryEntry.StateBits = data.ReadUInt32();
            directoryEntry.CreationTime = data.ReadUInt64();
            directoryEntry.ModifiedTime = data.ReadUInt64();
            directoryEntry.StartingSectorLocation = data.ReadUInt32();
            directoryEntry.StreamSize = data.ReadUInt64();

            return directoryEntry;
        }

        #endregion
    }
}
