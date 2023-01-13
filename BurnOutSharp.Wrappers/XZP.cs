using System;
using System.IO;
using System.Linq;

namespace BurnOutSharp.Wrappers
{
    public class XZP : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.XZP.Header.Signature"/>
        public string Signature => _file.Header.Signature;

        /// <inheritdoc cref="Models.XZP.Header.Version"/>
        public uint Version => _file.Header.Version;

        /// <inheritdoc cref="Models.XZP.Header.PreloadDirectoryEntryCount"/>
        public uint PreloadDirectoryEntryCount => _file.Header.PreloadDirectoryEntryCount;

        /// <inheritdoc cref="Models.XZP.Header.DirectoryEntryCount"/>
        public uint DirectoryEntryCount => _file.Header.DirectoryEntryCount;

        /// <inheritdoc cref="Models.XZP.Header.PreloadBytes"/>
        public uint PreloadBytes => _file.Header.PreloadBytes;

        /// <inheritdoc cref="Models.XZP.Header.HeaderLength"/>
        public uint HeaderLength => _file.Header.HeaderLength;

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemCount"/>
        public uint DirectoryItemCount => _file.Header.DirectoryItemCount;

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemOffset"/>
        public uint DirectoryItemOffset => _file.Header.DirectoryItemOffset;

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemLength"/>
        public uint DirectoryItemLength => _file.Header.DirectoryItemLength;

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.XZP.DirectoryEntries"/>
        public Models.XZP.DirectoryEntry[] DirectoryEntries => _file.DirectoryEntries;

        #endregion

        #region Preload Directory Entries

        /// <inheritdoc cref="Models.XZP.PreloadDirectoryEntries"/>
        public Models.XZP.DirectoryEntry[] PreloadDirectoryEntries => _file.PreloadDirectoryEntries;

        #endregion

        #region Preload Directory Entries

        /// <inheritdoc cref="Models.XZP.PreloadDirectoryMappings"/>
        public Models.XZP.DirectoryMapping[] PreloadDirectoryMappings => _file.PreloadDirectoryMappings;

        #endregion

        #region Directory Items

        /// <inheritdoc cref="Models.XZP.DirectoryItems"/>
        public Models.XZP.DirectoryItem[] DirectoryItems => _file.DirectoryItems;

        #endregion

        #region Footer

        /// <inheritdoc cref="Models.XZP.Footer.FileLength"/>
        public uint F_FileLength => _file.Footer.FileLength;

        /// <inheritdoc cref="Models.XZP.Footer.Signature"/>
        public string F_Signature => _file.Footer.Signature;

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extensions are needed
        
        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the XZP
        /// </summary>
        private Models.XZP.File _file;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private XZP() { }

        /// <summary>
        /// Create a XZP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XZP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
        public static XZP Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a XZP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the XZP</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
        public static XZP Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Builders.XZP.ParseFile(data);
            if (file == null)
                return null;

            var wrapper = new XZP
            {
                _file = file,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void PrettyPrint()
        {
            Console.WriteLine("XZP Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintDirectoryEntries();
            PrintPreloadDirectoryEntries();
            PrintPreloadDirectoryMappings();
            PrintDirectoryItems();
            PrintFooter();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Version: {Version} (0x{Version:X})");
            Console.WriteLine($"  Preload directory entry count: {PreloadDirectoryEntryCount} (0x{PreloadDirectoryEntryCount:X})");
            Console.WriteLine($"  Directory entry count: {DirectoryEntryCount} (0x{DirectoryEntryCount:X})");
            Console.WriteLine($"  Preload bytes: {PreloadBytes} (0x{PreloadBytes:X})");
            Console.WriteLine($"  Header length: {HeaderLength} (0x{HeaderLength:X})");
            Console.WriteLine($"  Directory item count: {DirectoryItemCount} (0x{DirectoryItemCount:X})");
            Console.WriteLine($"  Directory item offset: {DirectoryItemOffset} (0x{DirectoryItemOffset:X})");
            Console.WriteLine($"  Directory item length: {DirectoryItemLength} (0x{DirectoryItemLength:X})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory entries information
        /// </summary>
        private void PrintDirectoryEntries()
        {
            Console.WriteLine("  Directory Entries Information:");
            Console.WriteLine("  -------------------------");
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
            {
                Console.WriteLine("  No directory entries");
            }
            else
            {
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    var directoryEntry = DirectoryEntries[i];
                    Console.WriteLine($"  Directory Entry {i}");
                    Console.WriteLine($"    File name CRC: {directoryEntry.FileNameCRC} (0x{directoryEntry.FileNameCRC:X})");
                    Console.WriteLine($"    Entry length: {directoryEntry.EntryLength} (0x{directoryEntry.EntryLength:X})");
                    Console.WriteLine($"    Entry offset: {directoryEntry.EntryOffset} (0x{directoryEntry.EntryOffset:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print preload directory entries information
        /// </summary>
        private void PrintPreloadDirectoryEntries()
        {
            Console.WriteLine("  Preload Directory Entries Information:");
            Console.WriteLine("  -------------------------");
            if (PreloadDirectoryEntries == null || PreloadDirectoryEntries.Length == 0)
            {
                Console.WriteLine("  No preload directory entries");
            }
            else
            {
                for (int i = 0; i < PreloadDirectoryEntries.Length; i++)
                {
                    var preloadDirectoryEntry = PreloadDirectoryEntries[i];
                    Console.WriteLine($"  Directory Entry {i}");
                    Console.WriteLine($"    File name CRC: {preloadDirectoryEntry.FileNameCRC} (0x{preloadDirectoryEntry.FileNameCRC:X})");
                    Console.WriteLine($"    Entry length: {preloadDirectoryEntry.EntryLength} (0x{preloadDirectoryEntry.EntryLength:X})");
                    Console.WriteLine($"    Entry offset: {preloadDirectoryEntry.EntryOffset} (0x{preloadDirectoryEntry.EntryOffset:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print preload directory mappings information
        /// </summary>
        private void PrintPreloadDirectoryMappings()
        {
            Console.WriteLine("  Preload Directory Mappings Information:");
            Console.WriteLine("  -------------------------");
            if (PreloadDirectoryMappings == null || PreloadDirectoryMappings.Length == 0)
            {
                Console.WriteLine("  No preload directory mappings");
            }
            else
            {
                for (int i = 0; i < PreloadDirectoryMappings.Length; i++)
                {
                    var preloadDirectoryMapping = PreloadDirectoryMappings[i];
                    Console.WriteLine($"  Directory Mapping {i}");
                    Console.WriteLine($"    Preload directory entry index: {preloadDirectoryMapping.PreloadDirectoryEntryIndex} (0x{preloadDirectoryMapping.PreloadDirectoryEntryIndex:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory items information
        /// </summary>
        private void PrintDirectoryItems()
        {
            Console.WriteLine("  Directory Items Information:");
            Console.WriteLine("  -------------------------");
            if (DirectoryItems == null || DirectoryItems.Length == 0)
            {
                Console.WriteLine("  No directory items");
            }
            else
            {
                for (int i = 0; i < DirectoryItems.Length; i++)
                {
                    var directoryItem = DirectoryItems[i];
                    Console.WriteLine($"  Directory Item {i}");
                    Console.WriteLine($"    File name CRC: {directoryItem.FileNameCRC} (0x{directoryItem.FileNameCRC:X})");
                    Console.WriteLine($"    Name offset: {directoryItem.NameOffset} (0x{directoryItem.NameOffset:X})");
                    Console.WriteLine($"    Name: {directoryItem.Name ?? "[NULL]"}");
                    Console.WriteLine($"    Time created: {directoryItem.TimeCreated} (0x{directoryItem.TimeCreated:X})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print footer information
        /// </summary>
        private void PrintFooter()
        {
            Console.WriteLine("  Footer Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  File length: {F_FileLength} (0x{F_FileLength:X})");
            Console.WriteLine($"  Signature: {F_Signature}");
            Console.WriteLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_file, _jsonSerializerOptions);

#endif

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the XZP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no directory entries
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the XZP to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no directory entries
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // If the directory entry index is invalid
            if (index < 0 || index >= DirectoryEntries.Length)
                return false;

            // Get the directory entry
            var directoryEntry = DirectoryEntries[index];
            if (directoryEntry == null)
                return false;

            // Get the associated directory item
            var directoryItem = DirectoryItems.Where(di => di.FileNameCRC == directoryEntry.FileNameCRC).FirstOrDefault();
            if (directoryItem == null)
                return false;

            // Load the item data
            byte[] data = ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);

            // Create the filename
            string filename = directoryItem.Name;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}