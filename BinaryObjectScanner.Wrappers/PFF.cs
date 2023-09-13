using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class PFF : WrapperBase<SabreTools.Models.PFF.Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "NovaLogic Game Archive Format (PFF)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.PFF.Header.HeaderSize"/>
#if NET48
        public uint HeaderSize => _model.Header.HeaderSize;
#else
        public uint? HeaderSize => _model.Header?.HeaderSize;
#endif

        /// <inheritdoc cref="Models.PFF.Header.Signature"/>
#if NET48
        public string Signature => _model.Header.Signature;
#else
        public string? Signature => _model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.PFF.Header.NumberOfFiles"/>
#if NET48
        public uint NumberOfFiles => _model.Header.NumberOfFiles;
#else
        public uint? NumberOfFiles => _model.Header?.NumberOfFiles;
#endif

        /// <inheritdoc cref="Models.PFF.Header.FileSegmentSize"/>
#if NET48
        public uint FileSegmentSize => _model.Header.FileSegmentSize;
#else
        public uint? FileSegmentSize => _model.Header?.FileSegmentSize;
#endif

        /// <inheritdoc cref="Models.PFF.Header.FileListOffset"/>
#if NET48
        public uint FileListOffset => _model.Header.FileListOffset;
#else
        public uint? FileListOffset => _model.Header?.FileListOffset;
#endif

        #endregion

        #region Segments

        /// <inheritdoc cref="Models.PFF.Archive.Segments"/>
#if NET48
        public SabreTools.Models.PFF.Segment[] Segments => _model.Segments;
#else
        public SabreTools.Models.PFF.Segment?[]? Segments => _model.Segments;
#endif

        #endregion

        #region Footer

        /// <inheritdoc cref="Models.PFF.Footer.SystemIP"/>
#if NET48
        public uint SystemIP => _model.Footer.SystemIP;
#else
        public uint? SystemIP => _model.Footer?.SystemIP;
#endif

        /// <inheritdoc cref="Models.PFF.Footer.Reserved"/>
#if NET48
        public uint Reserved => _model.Footer.Reserved;
#else
        public uint? Reserved => _model.Footer?.Reserved;
#endif

        /// <inheritdoc cref="Models.PFF.Footer.KingTag"/>
#if NET48
        public string KingTag => _model.Footer.KingTag;
#else
        public string? KingTag => _model.Footer?.KingTag;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public PFF(SabreTools.Models.PFF.Archive model, byte[] data, int offset)
#else
        public PFF(SabreTools.Models.PFF.Archive? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public PFF(SabreTools.Models.PFF.Archive model, Stream data)
#else
        public PFF(SabreTools.Models.PFF.Archive? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }/// <summary>
         /// Create a PFF archive from a byte array and offset
         /// </summary>
         /// <param name="data">Byte array representing the archive</param>
         /// <param name="offset">Offset within the array to parse</param>
         /// <returns>A PFF archive wrapper on success, null on failure</returns>
#if NET48
        public static PFF Create(byte[] data, int offset)
#else
        public static PFF? Create(byte[]? data, int offset)
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
        /// Create a PFF archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A PFF archive wrapper on success, null on failure</returns>
#if NET48
        public static PFF Create(Stream data)
#else
        public static PFF? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.PFF().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new PFF(archive, data);
            }
            catch
            {
                return null;
            }
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
            if (file == null)
                return false;

            // Get the read index and length
            int offset = (int)file.FileLocation;
            int size = (int)file.FileSize;

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.FileName ?? $"file{index}");
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
#if NET48
                    byte[] data = ReadFromDataSource(offset, size);
#else
                    byte[]? data = ReadFromDataSource(offset, size);
#endif
                    if (data == null)
                        return false;

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
                    if (segment == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}