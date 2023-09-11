using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO;
using static SabreTools.Models.VPK.Constants;

namespace BinaryObjectScanner.Wrappers
{
    public class VPK : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "Valve Package File (VPK)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.VPK.Header.Signature"/>
        public uint Signature => _file.Header.Signature;

        /// <inheritdoc cref="Models.VPK.Header.Version"/>
        public uint Version => _file.Header.Version;

        /// <inheritdoc cref="Models.VPK.Header.DirectoryLength"/>
        public uint DirectoryLength => _file.Header.DirectoryLength;

        #endregion

        #region Extended Header

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.Dummy0"/>
        public uint? Dummy0 => _file.ExtendedHeader?.Dummy0;

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.ArchiveHashLength"/>
        public uint? ArchiveHashLength => _file.ExtendedHeader?.ArchiveHashLength;

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.ExtraLength"/>
        public uint? ExtraLength => _file.ExtendedHeader?.ExtraLength;

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.Dummy1"/>
        public uint? Dummy1 => _file.ExtendedHeader?.Dummy1;

        #endregion

        #region Archive Hashes

        /// <inheritdoc cref="Models.VPK.ArchiveHashes"/>
        public SabreTools.Models.VPK.ArchiveHash[] ArchiveHashes => _file.ArchiveHashes;

        #endregion

        #region Directory Items

        /// <inheritdoc cref="Models.VPK.DirectoryItems"/>
        public SabreTools.Models.VPK.DirectoryItem[] DirectoryItems => _file.DirectoryItems;

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Array of archive filenames attached to the given VPK
        /// </summary>
        public string[] ArchiveFilenames
        {
            get
            {
                // Use the cached value if we have it
                if (_archiveFilenames != null)
                    return _archiveFilenames;

                // If we don't have a source filename
                if (!(_streamData is FileStream fs) || string.IsNullOrWhiteSpace(fs.Name))
                    return null;

                // If the filename is not the right format
                string extension = Path.GetExtension(fs.Name).TrimStart('.');
                string fileName = Path.Combine(Path.GetDirectoryName(fs.Name), Path.GetFileNameWithoutExtension(fs.Name));
                if (fileName.Length < 3)
                    return null;
                else if (fileName.Substring(fileName.Length - 3) != "dir")
                    return null;

                // Get the archive count
                int archiveCount = DirectoryItems
                    .Select(di => di.DirectoryEntry)
                    .Select(de => de.ArchiveIndex)
                    .Where(ai => ai != HL_VPK_NO_ARCHIVE)
                    .Max();

                // Build the list of archive filenames to populate
                _archiveFilenames = new string[archiveCount];

                // Loop through and create the archive filenames
                for (int i = 0; i < archiveCount; i++)
                {
                    // We need 5 digits to print a short, but we already have 3 for dir.
                    string archiveFileName = $"{fileName.Substring(0, fileName.Length - 3)}{i.ToString().PadLeft(3, '0')}.{extension}";
                    _archiveFilenames[i] = archiveFileName;
                }

                // Return the array
                return _archiveFilenames;
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the VPK
        /// </summary>
        private SabreTools.Models.VPK.File _file;

        /// <summary>
        /// Array of archive filenames attached to the given VPK
        /// </summary>
        private string[] _archiveFilenames = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private VPK() { }

        /// <summary>
        /// Create a VPK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VPK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VPK wrapper on success, null on failure</returns>
        public static VPK Create(byte[] data, int offset)
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
        /// Create a VPK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the VPK</param>
        /// <returns>A VPK wrapper on success, null on failure</returns>
        public static VPK Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.VPK().Deserialize(data);
            if (file == null)
                return null;

            var wrapper = new VPK
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
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("VPK Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintExtendedHeader(builder);
            PrintArchiveHashes(builder);
            PrintDirectoryItems(builder);

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
            builder.AppendLine($"  Signature: {Signature} (0x{Signature:X})");
            builder.AppendLine($"  Version: {Version} (0x{Version:X})");
            builder.AppendLine($"  Directory length: {DirectoryLength} (0x{DirectoryLength:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print extended header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintExtendedHeader(StringBuilder builder)
        {
            builder.AppendLine("  Extended Header Information:");
            builder.AppendLine("  -------------------------");
            if (_file.ExtendedHeader == null)
            {
                builder.AppendLine("  No extended header");
            }
            else
            {
                builder.AppendLine($"  Dummy 0: {Dummy0} (0x{Dummy0:X})");
                builder.AppendLine($"  Archive hash length: {ArchiveHashLength} (0x{ArchiveHashLength:X})");
                builder.AppendLine($"  Extra length: {ExtraLength} (0x{ExtraLength:X})");
                builder.AppendLine($"  Dummy 1: {Dummy1} (0x{Dummy1:X})");
                builder.AppendLine();
            }
        }

        /// <summary>
        /// Print archive hashes information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintArchiveHashes(StringBuilder builder)
        {
            builder.AppendLine("  Archive Hashes Information:");
            builder.AppendLine("  -------------------------");
            if (ArchiveHashes == null || ArchiveHashes.Length == 0)
            {
                builder.AppendLine("  No archive hashes");
            }
            else
            {
                for (int i = 0; i < ArchiveHashes.Length; i++)
                {
                    var archiveHash = ArchiveHashes[i];
                    builder.AppendLine($"  Archive Hash {i}");
                    builder.AppendLine($"    Archive index: {archiveHash.ArchiveIndex} (0x{archiveHash.ArchiveIndex:X})");
                    builder.AppendLine($"    Archive offset: {archiveHash.ArchiveOffset} (0x{archiveHash.ArchiveOffset:X})");
                    builder.AppendLine($"    Length: {archiveHash.Length} (0x{archiveHash.Length:X})");
                    builder.AppendLine($"    Hash: {BitConverter.ToString(archiveHash.Hash).Replace("-", string.Empty)}");
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
                    builder.AppendLine($"    Extension: {directoryItem.Extension}");
                    builder.AppendLine($"    Path: {directoryItem.Path}");
                    builder.AppendLine($"    Name: {directoryItem.Name}");
                    PrintDirectoryEntry(directoryItem.DirectoryEntry, builder);
                    // TODO: Print out preload data?
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory entry information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryEntry(SabreTools.Models.VPK.DirectoryEntry directoryEntry, StringBuilder builder)
        {
            if (directoryEntry == null)
            {
                builder.AppendLine("   Directory entry: [NULL]");
            }
            else
            {
                builder.AppendLine($"   Directory entry CRC: {directoryEntry.CRC} (0x{directoryEntry.CRC:X})");
                builder.AppendLine($"   Directory entry preload bytes: {directoryEntry.PreloadBytes} (0x{directoryEntry.PreloadBytes:X})");
                builder.AppendLine($"   Directory entry archive index: {directoryEntry.ArchiveIndex} (0x{directoryEntry.ArchiveIndex:X})");
                builder.AppendLine($"   Directory entry entry offset: {directoryEntry.EntryOffset} (0x{directoryEntry.EntryOffset:X})");
                builder.AppendLine($"   Directory entry entry length: {directoryEntry.EntryLength} (0x{directoryEntry.EntryLength:X})");
                builder.AppendLine($"   Directory entry dummy 0: {directoryEntry.Dummy0} (0x{directoryEntry.Dummy0:X})");
            }
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_file, _jsonSerializerOptions);

#endif

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the VPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryItems.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = DirectoryItems[index];
            if (directoryItem?.DirectoryEntry == null)
                return false;

            // If we have an item with no archive
            byte[] data;
            if (directoryItem.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
            {
                if (directoryItem.PreloadData == null)
                    return false;

                data = directoryItem.PreloadData;
            }
            else
            {
                // If we have invalid archives
                if (ArchiveFilenames == null || ArchiveFilenames.Length == 0)
                    return false;

                // If we have an invalid index
                if (directoryItem.DirectoryEntry.ArchiveIndex < 0 || directoryItem.DirectoryEntry.ArchiveIndex >= ArchiveFilenames.Length)
                    return false;

                // Get the archive filename
                string archiveFileName = ArchiveFilenames[directoryItem.DirectoryEntry.ArchiveIndex];
                if (string.IsNullOrWhiteSpace(archiveFileName))
                    return false;

                // If the archive doesn't exist
                if (!File.Exists(archiveFileName))
                    return false;

                // Try to open the archive
                Stream archiveStream = null;
                try
                {
                    // Open the archive
                    archiveStream = File.OpenRead(archiveFileName);

                    // Seek to the data
                    archiveStream.Seek(directoryItem.DirectoryEntry.EntryOffset, SeekOrigin.Begin);

                    // Read the directory item bytes
                    data = archiveStream.ReadBytes((int)directoryItem.DirectoryEntry.EntryLength);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    archiveStream?.Close();
                }

                // If we have preload data, prepend it
                if (directoryItem.PreloadData != null)
                    data = directoryItem.PreloadData.Concat(data).ToArray();
            }

            // Create the filename
            string filename = $"{directoryItem.Name}.{directoryItem.Extension}";
            if (!string.IsNullOrWhiteSpace(directoryItem.Path))
                filename = Path.Combine(directoryItem.Path, filename);

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