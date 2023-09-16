using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class Quantum : WrapperBase<SabreTools.Models.Quantum.Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Quantum Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public Quantum(SabreTools.Models.Quantum.Archive model, byte[] data, int offset)
#else
        public Quantum(SabreTools.Models.Quantum.Archive? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public Quantum(SabreTools.Models.Quantum.Archive model, Stream data)
#else
        public Quantum(SabreTools.Models.Quantum.Archive? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a Quantum archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
#if NET48
        public static Quantum Create(byte[] data, int offset)
#else
        public static Quantum? Create(byte[]? data, int offset)
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
        /// Create a Quantum archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
#if NET48
        public static Quantum Create(Stream data)
#else
        public static Quantum? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.Quantum().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new Quantum(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Extract all files from the Quantum archive to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (this.Model.FileList == null || this.Model.FileList.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < this.Model.FileList.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the Quantum archive to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (this.Model.Header == null || this.Model.Header.FileCount == 0 || this.Model.FileList == null || this.Model.FileList.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= this.Model.FileList.Length)
                return false;

            // Get the file information
            var fileDescriptor = this.Model.FileList[index];

            // Read the entire compressed data
            int compressedDataOffset = (int)this.Model.CompressedDataOffset;
            int compressedDataLength = GetEndOfFile() - compressedDataOffset;
#if NET48
            byte[] compressedData = ReadFromDataSource(compressedDataOffset, compressedDataLength);
#else
            byte[]? compressedData = ReadFromDataSource(compressedDataOffset, compressedDataLength);
#endif

            // TODO: Figure out decompression
            // - Single-file archives seem to work
            // - Single-file archives with files that span a window boundary seem to work
            // - The first files in each archive seem to work
            return false;

            // // Setup the decompression state
            // State state = new State();
            // Decompressor.InitState(state, TableSize, CompressionFlags);

            // // Decompress the entire array
            // int decompressedDataLength = (int)FileList.Sum(fd => fd.ExpandedFileSize);
            // byte[] decompressedData = new byte[decompressedDataLength];
            // Decompressor.Decompress(state, compressedData.Length, compressedData, decompressedData.Length, decompressedData);

            // // Read the data
            // int offset = (int)FileList.Take(index).Sum(fd => fd.ExpandedFileSize);
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // Array.Copy(decompressedData, offset, data, 0, data.Length);

            // // Loop through all files before the current
            // for (int i = 0; i < index; i++)
            // {
            //     // Decompress the next block of data
            //     byte[] tempData = new byte[FileList[i].ExpandedFileSize];
            //     int lastRead = Decompressor.Decompress(state, compressedData.Length, compressedData, tempData.Length, tempData);
            //     compressedData = new ReadOnlySpan<byte>(compressedData, (lastRead), compressedData.Length - (lastRead)).ToArray();
            // }

            // // Read the data
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // _ = Decompressor.Decompress(state, compressedData.Length, compressedData, data.Length, data);

            // // Create the filename
            // string filename = fileDescriptor.FileName;

            // // If we have an invalid output directory
            // if (string.IsNullOrWhiteSpace(outputDirectory))
            //     return false;

            // // Create the full output path
            // filename = Path.Combine(outputDirectory, filename);

            // // Ensure the output directory is created
            // Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // // Try to write the data
            // try
            // {
            //     // Open the output file for writing
            //     using (Stream fs = File.OpenWrite(filename))
            //     {
            //         fs.Write(data, 0, data.Length);
            //     }
            // }
            // catch
            // {
            //     return false;
            // }

            return true;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.Quantum.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}