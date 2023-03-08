using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class PAK : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "Half-Life Package File (PAK)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.PAK.Header.Signature"/>
        public string Signature => _file.Header.Signature;

        /// <inheritdoc cref="Models.PAK.Header.DirectoryOffset"/>
        public uint DirectoryOffset => _file.Header.DirectoryOffset;

        /// <inheritdoc cref="Models.PAK.Header.DirectoryLength"/>
        public uint DirectoryLength => _file.Header.DirectoryLength;

        #endregion

        #region Directory Items

        /// <inheritdoc cref="Models.PAK.DirectoryItems"/>
        public Models.PAK.DirectoryItem[] DirectoryItems => _file.DirectoryItems;

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extensions are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the PAK
        /// </summary>
        private Models.PAK.File _file;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private PAK() { }

        /// <summary>
        /// Create a PAK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the PAK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
        public static PAK Create(byte[] data, int offset)
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
        /// Create a PAK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the PAK</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
        public static PAK Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Builders.PAK.ParseFile(data);
            if (file == null)
                return null;

            var wrapper = new PAK
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

            builder.AppendLine("PAK Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
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
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Directory offset: {DirectoryOffset} (0x{DirectoryOffset:X})");
            builder.AppendLine($"  Directory length: {DirectoryLength} (0x{DirectoryLength:X})");
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
                    builder.AppendLine($"    Item name: {directoryItem.ItemName}");
                    builder.AppendLine($"    Item offset: {directoryItem.ItemOffset} (0x{directoryItem.ItemOffset:X})");
                    builder.AppendLine($"    Item length: {directoryItem.ItemLength} (0x{directoryItem.ItemLength:X})");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_file, _jsonSerializerOptions);

#endif

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the PAK to an output directory
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
        /// Extract a file from the PAK to an output directory by index
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
            if (directoryItem == null)
                return false;

            // Read the item data
            byte[] data = ReadFromDataSource((int)directoryItem.ItemOffset, (int)directoryItem.ItemLength);

            // Create the filename
            string filename = directoryItem.ItemName;

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