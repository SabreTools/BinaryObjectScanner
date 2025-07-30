using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Models.CFB;
using static SabreTools.Models.CFB.Constants;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Compound File Binary
    /// </summary>
    public class CFB : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Get a wrapper for the CFB
            var model = Deserialize(stream);
            if (model == null)
                return false;

            var cfb = new SabreTools.Serialization.Wrappers.CFB(model, stream);
            if (cfb?.Model == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            ExtractAll(cfb, outDir);

            return true;
        }

        #region REMOVE WHEN SERIALIZATION UPDATED

        /// <summary>
        /// Extract all files from the CFB to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        private static bool ExtractAll(SabreTools.Serialization.Wrappers.CFB? cfb, string outputDirectory)
        {
            // If we have no files
            if (cfb?.Model?.DirectoryEntries == null || cfb.Model.DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all directory entries to the output
            bool allExtracted = true;
            for (int i = 0; i < cfb.Model.DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractEntry(cfb, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the CFB to an output directory by index
        /// </summary>
        /// <param name="index">Entry index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        private static bool ExtractEntry(SabreTools.Serialization.Wrappers.CFB cfb, int index, string outputDirectory)
        {
            // If we have no entries
            if (cfb?.Model?.DirectoryEntries == null || cfb.Model.DirectoryEntries.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= cfb.Model.DirectoryEntries.Length)
                return false;

            // Get the entry information
            var entry = cfb.Model.DirectoryEntries[index];
            if (entry == null)
                return false;

            // Only try to extract stream objects
            if (entry.ObjectType != ObjectType.StreamObject)
                return true;

            // Get the entry data
            byte[]? data = GetDirectoryEntryData(cfb, entry);
            if (data == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure the output filename is trimmed
            string filename = entry.Name ?? $"entry{index}";
            byte[] nameBytes = Encoding.UTF8.GetBytes(filename);
            if (nameBytes[0] == 0xe4 && nameBytes[1] == 0xa1 && nameBytes[2] == 0x80)
                filename = Encoding.UTF8.GetString(nameBytes, 3, nameBytes.Length - 3);

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, '_');
            }

            // Ensure directory separators are consistent
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using FileStream fs = File.OpenWrite(filename);
                fs.Write(data);
                fs.Flush();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read the entry data for a single directory entry, if possible
        /// </summary>
        /// <param name="entry">Entry to try to retrieve data for</param>
        /// <returns>Byte array representing the entry data on success, null otherwise</returns>
        private static byte[]? GetDirectoryEntryData(SabreTools.Serialization.Wrappers.CFB cfb, DirectoryEntry entry)
        {
            // If the CFB is invalid
            if (cfb.Model?.Header == null)
                return null;

            // Only try to extract stream objects
            if (entry.ObjectType != ObjectType.StreamObject)
                return null;

            // Determine which FAT is being used
            bool miniFat = entry.StreamSize < cfb.Model.Header.MiniStreamCutoffSize;

            // Get the chain data
            var chain = miniFat
                ? GetMiniFATSectorChainData(cfb, (SectorNumber)entry.StartingSectorLocation)
                : GetFATSectorChainData(cfb, (SectorNumber)entry.StartingSectorLocation);
            if (chain == null)
                return null;

            // Return only the proper amount of data
            byte[] data = new byte[entry.StreamSize];
            Array.Copy(chain, 0, data, 0, (int)Math.Min(chain.Length, (long)entry.StreamSize));
            return data;
        }

        /// <remarks>Adapted from LibMSI</remarks>
        private static string? DecodeStreamName(string input)
        {
            if (input == null)
                return null;

            int count = 0;
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            int p = 0; // inputBytes[0]

            byte[] output = new byte[inputBytes.Length + 1];
            int q = 0; // output[0]
            while (p < inputBytes.Length && inputBytes[p] != 0)
            {
                int ch = inputBytes[p];
                if ((ch == 0xe3 && inputBytes[p + 1] >= 0xa0) || (ch == 0xe4 && inputBytes[p + 1] < 0xa0))
                {
                    // UTF-8 encoding of 0x3800..0x47ff. 
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 2] & 0x7f);
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 1] ^ 0xa0);
                    p += 3;
                    count += 2;
                    continue;
                }

                if (ch == 0xe4 && inputBytes[p + 1] == 0xa0)
                {
                    // UTF-8 encoding of 0x4800..0x483f.
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 2] & 0x7f);
                    p += 3;
                    count++;
                    continue;
                }

                output[q++] = inputBytes[p++];
                if (ch >= 0xc1)
                    output[q++] = inputBytes[p++];
                if (ch >= 0xe0)
                    output[q++] = inputBytes[p++];
                if (ch >= 0xf0)
                    output[q++] = inputBytes[p++];

                count++;
            }

            output[q] = 0;
            return Encoding.UTF8.GetString(output);
        }

        /// <remarks>Adapted from LibMSI</remarks>
        private static int Mime2Utf(int x)
        {
            if (x < 10)
                return x + '0';
            if (x < (10 + 26))
                return x - 10 + 'A';
            if (x < (10 + 26 + 26))
                return x - 10 - 26 + 'a';
            if (x == (10 + 26 + 26))
                return '.';
            return '_';
        }

        /// <inheritdoc/>
        private static Binary? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new binary to fill
                var binary = new Binary();

                #region Header

                // Try to parse the file header
                var fileHeader = ParseFileHeader(data);
                if (fileHeader?.Signature != SignatureUInt64)
                    return null;
                if (fileHeader.ByteOrder != 0xFFFE)
                    return null;
                if (fileHeader.MajorVersion == 3 && fileHeader.SectorShift != 0x0009)
                    return null;
                else if (fileHeader.MajorVersion == 4 && fileHeader.SectorShift != 0x000C)
                    return null;
                if (fileHeader.MajorVersion == 3 && fileHeader.NumberOfDirectorySectors != 0)
                    return null;
                if (fileHeader.MiniStreamCutoffSize != 0x00001000)
                    return null;

                // Set the file header
                binary.Header = fileHeader;

                #endregion

                #region DIFAT Sector Numbers

                // Create a DIFAT sector table
                var difatSectors = new List<SectorNumber>();

                // Add the sectors from the header
                if (fileHeader.DIFAT != null)
                    difatSectors.AddRange(fileHeader.DIFAT);

                // Loop through and add the DIFAT sectors
                var currentSector = (SectorNumber?)fileHeader.FirstDIFATSectorLocation;
                for (int i = 0; i < fileHeader.NumberOfDIFATSectors; i++)
                {
                    // If we have an unreadable sector
                    if (currentSector > SectorNumber.MAXREGSECT)
                        break;

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

                    // Add all but the last sector number that was parsed
                    for (int j = 0; j < sectorNumbers.Length - 1; j++)
                    {
                        difatSectors.Add(sectorNumbers[j]);
                    }

                    // Get the next sector from the final sector number
                    currentSector = sectorNumbers[sectorNumbers.Length - 1];
                }

                // Assign the DIFAT sectors table
                binary.DIFATSectorNumbers = [.. difatSectors];

                #endregion

                #region FAT Sector Numbers

                // Create a FAT sector table
                var fatSectors = new List<SectorNumber>();

                // Loop through and add the FAT sectors
                for (int i = 0; i < fileHeader.NumberOfFATSectors; i++)
                {
                    // Get the next sector from the DIFAT
                    currentSector = binary.DIFATSectorNumbers[i];

                    // If we have an unreadable sector
                    if (currentSector > SectorNumber.MAXREGSECT)
                        break;

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

                // Assign the FAT sectors table
                binary.FATSectorNumbers = [.. fatSectors];

                #endregion

                #region Mini FAT Sector Numbers

                // Create a mini FAT sector table
                var miniFatSectors = new List<SectorNumber>();

                // Loop through and add the mini FAT sectors
                currentSector = (SectorNumber)fileHeader.FirstMiniFATSectorLocation;
                for (int i = 0; i < fileHeader.NumberOfMiniFATSectors; i++)
                {
                    // If we have an unreadable sector
                    if (currentSector > SectorNumber.MAXREGSECT)
                        break;

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

                    // Get the next sector from the FAT
                    currentSector = binary.FATSectorNumbers[(int)currentSector];
                }

                // Assign the mini FAT sectors table
                binary.MiniFATSectorNumbers = [.. miniFatSectors];

                #endregion

                #region Directory Entries

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

                    // If we have an unusable sector for a version 3 file
                    if (directorySectorCount == int.MaxValue && currentSector > SectorNumber.MAXREGSECT)
                        break;

                    // Get the new next sector information
                    long sectorOffset = (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < 0 || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var directoryEntries = ParseDirectoryEntries(data, fileHeader.SectorShift, fileHeader.MajorVersion);
                    if (directoryEntries == null)
                        return null;

                    // Add the sector shifts
                    directorySectors.AddRange(directoryEntries);

                    // Get the next sector from the FAT
                    currentSector = binary.FATSectorNumbers[(int)currentSector];
                }

                // Assign the Directory sectors table
                binary.DirectoryEntries = [.. directorySectors];

                #endregion

                return binary;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a DirectoryEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryEntry on success, null on error</returns>
        private static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            var obj = new DirectoryEntry();

            byte[] name = data.ReadBytes(64);
            obj.Name = DecodeStreamName(Encoding.Unicode.GetString(name))?.TrimEnd('\0');
            obj.NameLength = data.ReadUInt16LittleEndian();
            obj.ObjectType = (ObjectType)data.ReadByteValue();
            obj.ColorFlag = (ColorFlag)data.ReadByteValue();
            obj.LeftSiblingID = (StreamID)data.ReadUInt32LittleEndian();
            obj.RightSiblingID = (StreamID)data.ReadUInt32LittleEndian();
            obj.ChildID = (StreamID)data.ReadUInt32LittleEndian();
            obj.CLSID = data.ReadGuid();
            obj.StateBits = data.ReadUInt32LittleEndian();
            obj.CreationTime = data.ReadUInt64LittleEndian();
            obj.ModifiedTime = data.ReadUInt64LittleEndian();
            obj.StartingSectorLocation = data.ReadUInt32LittleEndian();
            obj.StreamSize = data.ReadUInt64LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileHeader on success, null on error</returns>
        private static FileHeader ParseFileHeader(Stream data)
        {
            var obj = new FileHeader();

            obj.Signature = data.ReadUInt64LittleEndian();
            obj.CLSID = data.ReadGuid();
            obj.MinorVersion = data.ReadUInt16LittleEndian();
            obj.MajorVersion = data.ReadUInt16LittleEndian();
            obj.ByteOrder = data.ReadUInt16LittleEndian();
            obj.SectorShift = data.ReadUInt16LittleEndian();
            obj.MiniSectorShift = data.ReadUInt16LittleEndian();
            obj.Reserved = data.ReadBytes(6);
            obj.NumberOfDirectorySectors = data.ReadUInt32LittleEndian();
            obj.NumberOfFATSectors = data.ReadUInt32LittleEndian();
            obj.FirstDirectorySectorLocation = data.ReadUInt32LittleEndian();
            obj.TransactionSignatureNumber = data.ReadUInt32LittleEndian();
            obj.MiniStreamCutoffSize = data.ReadUInt32LittleEndian();
            obj.FirstMiniFATSectorLocation = data.ReadUInt32LittleEndian();
            obj.NumberOfMiniFATSectors = data.ReadUInt32LittleEndian();
            obj.FirstDIFATSectorLocation = data.ReadUInt32LittleEndian();
            obj.NumberOfDIFATSectors = data.ReadUInt32LittleEndian();
            obj.DIFAT = new SectorNumber[109];
            for (int i = 0; i < 109; i++)
            {
                obj.DIFAT[i] = (SectorNumber)data.ReadUInt32LittleEndian();
            }

            // Skip rest of sector for version 4
            if (obj.MajorVersion == 4)
                _ = data.ReadBytes(3584);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a sector full of sector numbers
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorShift">Sector shift from the header</param>
        /// <returns>Filled sector full of sector numbers on success, null on error</returns>
        private static SectorNumber[] ParseSectorNumbers(Stream data, ushort sectorShift)
        {
            int sectorCount = (int)(Math.Pow(2, sectorShift) / sizeof(uint));
            var sectorNumbers = new SectorNumber[sectorCount];

            for (int i = 0; i < sectorNumbers.Length; i++)
            {
                sectorNumbers[i] = (SectorNumber)data.ReadUInt32LittleEndian();
            }

            return sectorNumbers;
        }

        /// <summary>
        /// Parse a Stream into a sector full of directory entries
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorShift">Sector shift from the header</param>
        /// <param name="majorVersion">Major version from the header</param>
        /// <returns>Filled sector full of directory entries on success, null on error</returns>
        private static DirectoryEntry[]? ParseDirectoryEntries(Stream data, ushort sectorShift, ushort majorVersion)
        {
            // <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-CFB/%5bMS-CFB%5d.pdf"/>
            int directoryEntrySize = 128;

            int dirsPerSector = (int)(Math.Pow(2, sectorShift) / directoryEntrySize);
            var directoryEntries = new DirectoryEntry[dirsPerSector];

            for (int i = 0; i < directoryEntries.Length; i++)
            {
                var directoryEntry = ParseDirectoryEntry(data);

                // Handle version 3 entries
                if (majorVersion == 3)
                    directoryEntry.StreamSize &= 0x0000FFFF;

                directoryEntries[i] = directoryEntry;
            }

            return directoryEntries;
        }

        /// <summary>
        /// Get the ordered FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        private static List<SectorNumber>? GetFATSectorChain(SabreTools.Serialization.Wrappers.CFB cfb, SectorNumber? startingSector)
        {
            // If we have an invalid sector
            if (startingSector == null || startingSector < 0 || cfb.Model.FATSectorNumbers == null || (long)startingSector >= cfb.Model.FATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<SectorNumber> { startingSector.Value };

            var lastSector = startingSector;
            while (true)
            {
                if (lastSector == null)
                    break;

                // Get the next sector from the lookup table
                var nextSector = cfb.Model.FATSectorNumbers[(uint)lastSector!.Value];

                // If we have an invalid sector
                if (nextSector >= SectorNumber.MAXREGSECT)
                    break;

                // Add the next sector to the list and replace the last sector
                sectors.Add(nextSector);
                lastSector = nextSector;
            }

            return sectors;
        }

        /// <summary>
        /// Get the data for the FAT sector chain starting at a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        private static byte[]? GetFATSectorChainData(SabreTools.Serialization.Wrappers.CFB cfb, SectorNumber startingSector)
        {
            // Get the sector chain first
            var sectorChain = GetFATSectorChain(cfb, startingSector);
            if (sectorChain == null)
                return null;

            // Sequentially read the sectors
            var data = new List<byte>();
            for (int i = 0; i < sectorChain.Count; i++)
            {
                // Try to get the sector data offset
                int sectorDataOffset = (int)FATSectorToFileOffset(cfb, sectorChain[i]);
                if (sectorDataOffset < 0 || sectorDataOffset >= cfb.GetEndOfFile())
                    return null;

                // Try to read the sector data
                var sectorData = cfb.ReadFromDataSource(sectorDataOffset, (int)cfb.SectorSize);
                if (sectorData == null)
                    return null;

                // Add the sector data to the output
                data.AddRange(sectorData);
            }

            return [.. data];
        }

        /// <summary>
        /// Convert a FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
        private static long FATSectorToFileOffset(SabreTools.Serialization.Wrappers.CFB cfb, SectorNumber? sector)
        {
            // If we have an invalid sector number
            if (sector == null || sector > SectorNumber.MAXREGSECT)
                return -1;

            // Convert based on the sector shift value
            return (long)(sector + 1) * cfb.SectorSize;
        }

        /// <summary>
        /// Get the ordered Mini FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial Mini FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        private static List<SectorNumber>? GetMiniFATSectorChain(SabreTools.Serialization.Wrappers.CFB cfb, SectorNumber? startingSector)
        {
            // If we have an invalid sector
            if (startingSector == null || startingSector < 0 || cfb.Model.MiniFATSectorNumbers == null || (long)startingSector >= cfb.Model.MiniFATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<SectorNumber> { startingSector.Value };

            var lastSector = startingSector;
            while (true)
            {
                if (lastSector == null)
                    break;

                // Get the next sector from the lookup table
                var nextSector = cfb.Model.MiniFATSectorNumbers[(uint)lastSector!.Value];

                // If we have an invalid sector
                if (nextSector >= SectorNumber.MAXREGSECT)
                    break;

                // Add the next sector to the list and replace the last sector
                sectors.Add(nextSector);
                lastSector = nextSector;
            }

            return sectors;
        }

        /// <summary>
        /// Get the data for the Mini FAT sector chain starting at a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial Mini FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        private static byte[]? GetMiniFATSectorChainData(SabreTools.Serialization.Wrappers.CFB cfb, SectorNumber startingSector)
        {
            // Ignore invalid data
            if (cfb.Model?.Header == null || cfb.Model.DirectoryEntries == null || cfb.Model.DirectoryEntries.Length == 0)
                return null;

            // Get the mini stream offset
            uint miniStreamSectorLocation = cfb.Model.DirectoryEntries[0].StartingSectorLocation;

            // Get the mini stream data
            var miniStreamData = GetFATSectorChainData(cfb, (SectorNumber)miniStreamSectorLocation);
            if (miniStreamData == null)
                return null;

            // Get the sector chain
            var sectorChain = GetMiniFATSectorChain(cfb, startingSector);
            if (sectorChain == null)
                return null;

            // Sequentially read the sectors
            var data = new List<byte>();
            for (int i = 0; i < sectorChain.Count; i++)
            {
                // Try to get the mini stream data offset
                int streamDataOffset = (int)MiniFATSectorToMiniStreamOffset(cfb, sectorChain[i]);
                if (streamDataOffset < 0 || streamDataOffset > miniStreamData.Length)
                    return null;

                // Try to read the sector data
                var sectorData = miniStreamData.ReadBytes(ref streamDataOffset, (int)cfb.MiniSectorSize);
                if (sectorData == null)
                    return null;

                // Add the sector data to the output
                data.AddRange(sectorData);
            }

            return [.. data];
        }

        /// <summary>
        /// Convert a Mini FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>Stream offset in bytes, -1 on error</returns>
        /// <remarks>Offset is within the mini stream, not the full file</remarks>
        private static long MiniFATSectorToMiniStreamOffset(SabreTools.Serialization.Wrappers.CFB cfb, SectorNumber? sector)
        {
            // If we have an invalid sector number
            if (sector == null || sector > SectorNumber.MAXREGSECT)
                return -1;

            // Get the mini stream location
            return (long)sector * cfb.MiniSectorSize;
        }

        #endregion
    }
}
