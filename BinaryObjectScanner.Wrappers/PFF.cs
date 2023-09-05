using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class PFF : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "NovaLogic Game Archive Format (PFF)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.PFF.Header.HeaderSize"/>
        public uint HeaderSize => _archive.Header.HeaderSize;

        /// <inheritdoc cref="Models.PFF.Header.Signature"/>
        public string Signature => _archive.Header.Signature;

        /// <inheritdoc cref="Models.PFF.Header.NumberOfFiles"/>
        public uint NumberOfFiles => _archive.Header.NumberOfFiles;

        /// <inheritdoc cref="Models.PFF.Header.FileSegmentSize"/>
        public uint FileSegmentSize => _archive.Header.FileSegmentSize;

        /// <inheritdoc cref="Models.PFF.Header.FileListOffset"/>
        public uint FileListOffset => _archive.Header.FileListOffset;

        #endregion

        #region Segments

        /// <inheritdoc cref="Models.PFF.Archive.Segments"/>
        public SabreTools.Models.PFF.Segment[] Segments => _archive.Segments;

        #endregion

        #region Footer

        /// <inheritdoc cref="Models.PFF.Footer.SystemIP"/>
        public uint SystemIP => _archive.Footer.SystemIP;

        /// <inheritdoc cref="Models.PFF.Footer.Reserved"/>
        public uint Reserved => _archive.Footer.Reserved;

        /// <inheritdoc cref="Models.PFF.Footer.KingTag"/>
        public string KingTag => _archive.Footer.KingTag;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the archive
        /// </summary>
        private SabreTools.Models.PFF.Archive _archive;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private PFF() { }

        /// <summary>
        /// Create a PFF archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PFF archive wrapper on success, null on failure</returns>
        public static PFF Create(byte[] data, int offset)
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
        /// Create a PFF archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A PFF archive wrapper on success, null on failure</returns>
        public static PFF Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = Builders.PFF.ParseArchive(data);
            if (archive == null)
                return null;

            var wrapper = new PFF
            {
                _archive = archive,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Data

        /// <summary>
        /// Extract all segments from the PFF to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all segments extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no segments
            if (Segments == null || Segments.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Segments.Length; i++)
            {
                allExtracted &= ExtractSegment(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a segment from the PFF to an output directory by index
        /// </summary>
        /// <param name="index">Segment index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the segment extracted, false otherwise</returns>
        public bool ExtractSegment(int index, string outputDirectory)
        {
            // If we have no segments
            if (NumberOfFiles == 0 || Segments == null || Segments.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= Segments.Length)
                return false;

            // Get the segment information
            var file = Segments[index];

            // Get the read index and length
            int offset = (int)file.FileLocation;
            int size = (int)file.FileSize;

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.FileName);
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
                    byte[] data = ReadFromDataSource(offset, size);

                    // Write the data -- TODO: Compressed data?
                    fs.Write(data, 0, size);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("PFF Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintSegments(builder);
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
            builder.AppendLine($"  Header size: {HeaderSize} (0x{HeaderSize:X})");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Number of files: {NumberOfFiles} (0x{NumberOfFiles:X})");
            builder.AppendLine($"  File segment size: {FileSegmentSize} (0x{FileSegmentSize:X})");
            builder.AppendLine($"  File list offset: {FileListOffset} (0x{FileListOffset:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print segmentsinformation
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintSegments(StringBuilder builder)
        {
            builder.AppendLine("  Segments Information:");
            builder.AppendLine("  -------------------------");
            if (NumberOfFiles == 0 || Segments == null || Segments.Length == 0)
            {
                builder.AppendLine("  No segments");
            }
            else
            {
                for (int i = 0; i < Segments.Length; i++)
                {
                    var segment = Segments[i];
                    builder.AppendLine($"  Segment {i}");
                    builder.AppendLine($"    Deleted: {segment.Deleted} (0x{segment.Deleted:X})");
                    builder.AppendLine($"    File location: {segment.FileLocation} (0x{segment.FileLocation:X})");
                    builder.AppendLine($"    File size: {segment.FileSize} (0x{segment.FileSize:X})");
                    builder.AppendLine($"    Packed date: {segment.PackedDate} (0x{segment.PackedDate:X})");
                    builder.AppendLine($"    File name: {segment.FileName ?? "[NULL]"}");
                    builder.AppendLine($"    Modified date: {segment.ModifiedDate} (0x{segment.ModifiedDate:X})");
                    builder.AppendLine($"    Compression level: {segment.CompressionLevel} (0x{segment.CompressionLevel:X})");
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
            builder.AppendLine($"  System IP: {SystemIP} (0x{SystemIP:X})");
            builder.AppendLine($"  Reserved: {Reserved} (0x{Reserved:X})");
            builder.AppendLine($"  King tag: {KingTag ?? "[NULL]"}");
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_archive, _jsonSerializerOptions);

#endif

        #endregion
    }
}