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
            if (this.Model.Segments == null || this.Model.Segments.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < this.Model.Segments.Length; i++)
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
            if (this.Model.Header?.NumberOfFiles == null || this.Model.Header.NumberOfFiles == 0 || this.Model.Segments == null || this.Model.Segments.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= this.Model.Segments.Length)
                return false;

            // Get the segment information
            var file = this.Model.Segments[index];
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
            Printing.PFF.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}