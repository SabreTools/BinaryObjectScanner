using System.IO;
using System.Linq;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class XZP : WrapperBase<SabreTools.Models.XZP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox Package File (XZP)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.XZP.Header.Signature"/>
#if NET48
        public string Signature => _model.Header.Signature;
#else
        public string? Signature => _model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.XZP.Header.Version"/>
#if NET48
        public uint Version => _model.Header.Version;
#else
        public uint? Version => _model.Header?.Version;
#endif

        /// <inheritdoc cref="Models.XZP.Header.PreloadDirectoryEntryCount"/>
#if NET48
        public uint PreloadDirectoryEntryCount => _model.Header.PreloadDirectoryEntryCount;
#else
        public uint? PreloadDirectoryEntryCount => _model.Header?.PreloadDirectoryEntryCount;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryEntryCount"/>
#if NET48
        public uint DirectoryEntryCount => _model.Header.DirectoryEntryCount;
#else
        public uint? DirectoryEntryCount => _model.Header?.DirectoryEntryCount;
#endif

        /// <inheritdoc cref="Models.XZP.Header.PreloadBytes"/>
#if NET48
        public uint PreloadBytes => _model.Header.PreloadBytes;
#else
        public uint? PreloadBytes => _model.Header?.PreloadBytes;
#endif

        /// <inheritdoc cref="Models.XZP.Header.HeaderLength"/>
#if NET48
        public uint HeaderLength => _model.Header.HeaderLength;
#else
        public uint? HeaderLength => _model.Header?.HeaderLength;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemCount"/>
#if NET48
        public uint DirectoryItemCount => _model.Header.DirectoryItemCount;
#else
        public uint? DirectoryItemCount => _model.Header?.DirectoryItemCount;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemOffset"/>
#if NET48
        public uint DirectoryItemOffset => _model.Header.DirectoryItemOffset;
#else
        public uint? DirectoryItemOffset => _model.Header?.DirectoryItemOffset;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemLength"/>
#if NET48
        public uint DirectoryItemLength => _model.Header.DirectoryItemLength;
#else
        public uint? DirectoryItemLength => _model.Header?.DirectoryItemLength;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.XZP.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryEntry[] DirectoryEntries => _model.DirectoryEntries;
#else
        public SabreTools.Models.XZP.DirectoryEntry?[]? DirectoryEntries => _model.DirectoryEntries;
#endif

        #endregion

        #region Preload Directory Entries

        /// <inheritdoc cref="Models.XZP.PreloadDirectoryEntries"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryEntry[] PreloadDirectoryEntries => _model.PreloadDirectoryEntries;
#else
        public SabreTools.Models.XZP.DirectoryEntry?[]? PreloadDirectoryEntries => _model.PreloadDirectoryEntries;
#endif

        #endregion

        #region Preload Directory Entries

        /// <inheritdoc cref="Models.XZP.PreloadDirectoryMappings"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryMapping[] PreloadDirectoryMappings => _model.PreloadDirectoryMappings;
#else
        public SabreTools.Models.XZP.DirectoryMapping?[]? PreloadDirectoryMappings => _model.PreloadDirectoryMappings;
#endif

        #endregion

        #region Directory Items

        /// <inheritdoc cref="Models.XZP.DirectoryItems"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryItem[] DirectoryItems => _model.DirectoryItems;
#else
        public SabreTools.Models.XZP.DirectoryItem?[]? DirectoryItems => _model.DirectoryItems;
#endif

        #endregion

        #region Footer

        /// <inheritdoc cref="Models.XZP.Footer.FileLength"/>
#if NET48
        public uint F_FileLength => _model.Footer.FileLength;
#else
        public uint? F_FileLength => _model.Footer?.FileLength;
#endif

        /// <inheritdoc cref="Models.XZP.Footer.Signature"/>
#if NET48
        public string F_Signature => _model.Footer.Signature;
#else
        public string? F_Signature => _model.Footer?.Signature;
#endif

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extensions are needed

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public XZP(SabreTools.Models.XZP.File model, byte[] data, int offset)
#else
        public XZP(SabreTools.Models.XZP.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public XZP(SabreTools.Models.XZP.File model, Stream data)
#else
        public XZP(SabreTools.Models.XZP.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a XZP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XZP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
#if NET48
        public static XZP Create(byte[] data, int offset)
#else
        public static XZP? Create(byte[]? data, int offset)
#endif
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
#if NET48
        public static XZP Create(Stream data)
#else
        public static XZP? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.XZP().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new XZP(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("XZP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintDirectoryEntries(builder);
            PrintPreloadDirectoryEntries(builder);
            PrintPreloadDirectoryMappings(builder);
            PrintDirectoryItems(builder);
            PrintFooter(builder);

            return builder;
        }

        /// <summary>
        /// Print header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintHeader(StringBuilder builder)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Version: {Version} (0x{Version:X})");
            builder.AppendLine($"  Preload directory entry count: {PreloadDirectoryEntryCount} (0x{PreloadDirectoryEntryCount:X})");
            builder.AppendLine($"  Directory entry count: {DirectoryEntryCount} (0x{DirectoryEntryCount:X})");
            builder.AppendLine($"  Preload bytes: {PreloadBytes} (0x{PreloadBytes:X})");
            builder.AppendLine($"  Header length: {HeaderLength} (0x{HeaderLength:X})");
            builder.AppendLine($"  Directory item count: {DirectoryItemCount} (0x{DirectoryItemCount:X})");
            builder.AppendLine($"  Directory item offset: {DirectoryItemOffset} (0x{DirectoryItemOffset:X})");
            builder.AppendLine($"  Directory item length: {DirectoryItemLength} (0x{DirectoryItemLength:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryEntries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Entries Information:");
            builder.AppendLine("  -------------------------");
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
            {
                builder.AppendLine("  No directory entries");
            }
            else
            {
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    var directoryEntry = DirectoryEntries[i];
                    builder.AppendLine($"  Directory Entry {i}");
                    if (directoryEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    File name CRC: {directoryEntry.FileNameCRC} (0x{directoryEntry.FileNameCRC:X})");
                    builder.AppendLine($"    Entry length: {directoryEntry.EntryLength} (0x{directoryEntry.EntryLength:X})");
                    builder.AppendLine($"    Entry offset: {directoryEntry.EntryOffset} (0x{directoryEntry.EntryOffset:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print preload directory entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintPreloadDirectoryEntries(StringBuilder builder)
        {
            builder.AppendLine("  Preload Directory Entries Information:");
            builder.AppendLine("  -------------------------");
            if (PreloadDirectoryEntries == null || PreloadDirectoryEntries.Length == 0)
            {
                builder.AppendLine("  No preload directory entries");
            }
            else
            {
                for (int i = 0; i < PreloadDirectoryEntries.Length; i++)
                {
                    var preloadDirectoryEntry = PreloadDirectoryEntries[i];
                    builder.AppendLine($"  Directory Entry {i}");
                    if (preloadDirectoryEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    File name CRC: {preloadDirectoryEntry.FileNameCRC} (0x{preloadDirectoryEntry.FileNameCRC:X})");
                    builder.AppendLine($"    Entry length: {preloadDirectoryEntry.EntryLength} (0x{preloadDirectoryEntry.EntryLength:X})");
                    builder.AppendLine($"    Entry offset: {preloadDirectoryEntry.EntryOffset} (0x{preloadDirectoryEntry.EntryOffset:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print preload directory mappings information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintPreloadDirectoryMappings(StringBuilder builder)
        {
            builder.AppendLine("  Preload Directory Mappings Information:");
            builder.AppendLine("  -------------------------");
            if (PreloadDirectoryMappings == null || PreloadDirectoryMappings.Length == 0)
            {
                builder.AppendLine("  No preload directory mappings");
            }
            else
            {
                for (int i = 0; i < PreloadDirectoryMappings.Length; i++)
                {
                    var preloadDirectoryMapping = PreloadDirectoryMappings[i];
                    builder.AppendLine($"  Directory Mapping {i}");
                    if (preloadDirectoryMapping == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Preload directory entry index: {preloadDirectoryMapping.PreloadDirectoryEntryIndex} (0x{preloadDirectoryMapping.PreloadDirectoryEntryIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory items information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryItems(StringBuilder builder)
        {
            builder.AppendLine("  Directory Items Information:");
            builder.AppendLine("  -------------------------");
            if (DirectoryItems == null || DirectoryItems.Length == 0)
            {
                builder.AppendLine("  No directory items");
            }
            else
            {
                for (int i = 0; i < DirectoryItems.Length; i++)
                {
                    var directoryItem = DirectoryItems[i];
                    builder.AppendLine($"  Directory Item {i}");
                    if (directoryItem == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    File name CRC: {directoryItem.FileNameCRC} (0x{directoryItem.FileNameCRC:X})");
                    builder.AppendLine($"    Name offset: {directoryItem.NameOffset} (0x{directoryItem.NameOffset:X})");
                    builder.AppendLine($"    Name: {directoryItem.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Time created: {directoryItem.TimeCreated} (0x{directoryItem.TimeCreated:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print footer information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFooter(StringBuilder builder)
        {
            builder.AppendLine("  Footer Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  File length: {F_FileLength} (0x{F_FileLength:X})");
            builder.AppendLine($"  Signature: {F_Signature}");
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

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
            var directoryItem = DirectoryItems.Where(di => di?.FileNameCRC == directoryEntry.FileNameCRC).FirstOrDefault();
            if (directoryItem == null)
                return false;

            // Load the item data
#if NET48
            byte[] data = ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
#else
            byte[]? data = ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
#endif
            if (data == null)
                return false;

            // Create the filename
#if NET48
            string filename = directoryItem.Name;
#else
            string? filename = directoryItem.Name;
#endif

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename ?? $"file{index}");

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

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