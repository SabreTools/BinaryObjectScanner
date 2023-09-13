using System.IO;
using System.Text;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace BinaryObjectScanner.Wrappers
{
    public class BFPK : WrapperBase<SabreTools.Models.BFPK.Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "BFPK Archive";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.BFPK.Header.Magic"/>
#if NET48
        public string Magic => _model.Header.Magic;
#else
        public string? Magic => _model.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.BFPK.Header.Version"/>
#if NET48
        public int Version => _model.Header.Version;
#else
        public int? Version => _model.Header?.Version;
#endif

        /// <inheritdoc cref="Models.BFPK.Header.Files"/>
#if NET48
        public int Files => _model.Header.Files;
#else
        public int? Files => _model.Header?.Files;
#endif

        #endregion

        #region Files

        /// <inheritdoc cref="Models.BFPK.Archive.Files"/>
#if NET48
        public SabreTools.Models.BFPK.FileEntry[] FileTable => _model.Files;
#else
        public SabreTools.Models.BFPK.FileEntry?[]? FileTable => _model.Files;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public BFPK(SabreTools.Models.BFPK.Archive model, byte[] data, int offset)
#else
        public BFPK(SabreTools.Models.BFPK.Archive? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public BFPK(SabreTools.Models.BFPK.Archive model, Stream data)
#else
        public BFPK(SabreTools.Models.BFPK.Archive? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a BFPK archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
#if NET48
        public static BFPK Create(byte[] data, int offset)
#else
        public static BFPK? Create(byte[]? data, int offset)
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
        /// Create a BFPK archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
#if NET48
        public static BFPK Create(Stream data)
#else
        public static BFPK? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.BFPK().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new BFPK(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Extract all files from the BFPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (FileTable == null || FileTable.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < FileTable.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the BFPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (Files == 0 || FileTable == null || FileTable.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= FileTable.Length)
                return false;

            // Get the file information
            var file = FileTable[index];
            if (file == null)
                return false;

            // Get the read index and length
            int offset = file.Offset + 4;
            int compressedSize = file.CompressedSize;

            // Some files can lack the length prefix
            if (compressedSize > GetEndOfFile())
            {
                offset -= 4;
                compressedSize = file.UncompressedSize;
            }

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.Name ?? $"file{index}");
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
#if NET48
                    byte[] data = ReadFromDataSource(offset, compressedSize);
#else
                    byte[]? data = ReadFromDataSource(offset, compressedSize);
#endif
                    if (data == null)
                        return false;

                    // If we have uncompressed data
                    if (compressedSize == file.UncompressedSize)
                    {
                        fs.Write(data, 0, compressedSize);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        ZlibStream zs = new ZlibStream(ms, CompressionMode.Decompress);
                        zs.CopyTo(fs);
                    }
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
            Printing.BFPK.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}