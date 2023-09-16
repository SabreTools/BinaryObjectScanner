using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace BinaryObjectScanner.Wrappers
{
    public class SGA : WrapperBase<SabreTools.Models.SGA.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "SGA";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public SGA(SabreTools.Models.SGA.File model, byte[] data, int offset)
#else
        public SGA(SabreTools.Models.SGA.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public SGA(SabreTools.Models.SGA.File model, Stream data)
#else
        public SGA(SabreTools.Models.SGA.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an SGA from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the SGA</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
#if NET48
        public static SGA Create(byte[] data, int offset)
#else
        public static SGA? Create(byte[]? data, int offset)
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
        /// Create a SGA from a Stream
        /// </summary>
        /// <param name="data">Stream representing the SGA</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
#if NET48
        public static SGA Create(Stream data)
#else
        public static SGA? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.SGA().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new SGA(file, data);
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
            Printing.SGA.Print(builder, this.Model);
            return builder;
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the SGA to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // Get the number of files
            int filesLength;
            switch (this.Model.Header?.MajorVersion)
            {
                case 4: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory4)?.Files?.Length ?? 0; break;
                case 5: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory5)?.Files?.Length ?? 0; break;
                case 6: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory6)?.Files?.Length ?? 0; break;
                case 7: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory7)?.Files?.Length ?? 0; break;
                default: return false;
            }

            // If we have no files
            if (filesLength == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < filesLength; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the SGA to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // Get the number of files
            int filesLength;
            switch (this.Model.Header?.MajorVersion)
            {
                case 4: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory4)?.Files?.Length ?? 0; break;
                case 5: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory5)?.Files?.Length ?? 0; break;
                case 6: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory6)?.Files?.Length ?? 0; break;
                case 7: filesLength = (Model.Directory as SabreTools.Models.SGA.Directory7)?.Files?.Length ?? 0; break;
                default: return false;
            }

            // If we have no files
            if (filesLength == 0)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= filesLength)
                return false;

            // Get the files
#if NET48
            object file;
#else
            object? file;
#endif
            switch (this.Model.Header?.MajorVersion)
            {
                case 4: file = (Model.Directory as SabreTools.Models.SGA.Directory4)?.Files?[index]; break;
                case 5: file = (Model.Directory as SabreTools.Models.SGA.Directory5)?.Files?[index]; break;
                case 6: file = (Model.Directory as SabreTools.Models.SGA.Directory6)?.Files?[index]; break;
                case 7: file = (Model.Directory as SabreTools.Models.SGA.Directory7)?.Files?[index]; break;
                default: return false;
            }

            if (file == null)
                return false;

            // Create the filename
#if NET48
            string filename;
#else
            string? filename;
#endif
            switch (this.Model.Header?.MajorVersion)
            {
                case 4:
                case 5: filename = (file as SabreTools.Models.SGA.File4)?.Name; break;
                case 6: filename = (file as SabreTools.Models.SGA.File6)?.Name; break;
                case 7: filename = (file as SabreTools.Models.SGA.File7)?.Name; break;
                default: return false;
            }

            // Loop through and get all parent directories
#if NET48
            var parentNames = new List<string> { filename };
#else
            var parentNames = new List<string?> { filename };
#endif

            // Get the parent directory
#if NET48
            object folder;
#else
            object? folder;
#endif
            switch (this.Model.Header?.MajorVersion)
            {
#if NET48
                case 4: folder = (Model.Directory as SabreTools.Models.SGA.Directory4)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 5: folder = (Model.Directory as SabreTools.Models.SGA.Directory5)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 6: folder = (Model.Directory as SabreTools.Models.SGA.Directory6)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 7: folder = (Model.Directory as SabreTools.Models.SGA.Directory7)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
#else
                case 4: folder = (Model.Directory as SabreTools.Models.SGA.Directory4)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 5: folder = (Model.Directory as SabreTools.Models.SGA.Directory5)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 6: folder = (Model.Directory as SabreTools.Models.SGA.Directory6)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 7: folder = (Model.Directory as SabreTools.Models.SGA.Directory7)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
#endif
                default: return false;
            }

            // If we have a parent folder
            if (folder != null)
            {
                switch (this.Model.Header?.MajorVersion)
                {
                    case 4: parentNames.Add((folder as SabreTools.Models.SGA.Folder4)?.Name); break;
                    case 5:
                    case 6:
                    case 7: parentNames.Add((folder as SabreTools.Models.SGA.Folder5)?.Name); break;
                    default: return false;
                }
            }

            // TODO: Should the section name/alias be used in the path as well?

            // Reverse and assemble the filename
            parentNames.Reverse();
            filename = Path.Combine(parentNames.Cast<string>().ToArray());

            // Get the file offset
            long fileOffset;
            switch (this.Model.Header?.MajorVersion)
            {
                case 4:
                case 5: fileOffset = (file as SabreTools.Models.SGA.File4)?.Offset ?? 0; break;
                case 6: fileOffset = (file as SabreTools.Models.SGA.File6)?.Offset ?? 0; break;
                case 7: fileOffset = (file as SabreTools.Models.SGA.File7)?.Offset ?? 0; break;
                default: return false;
            }

            // Adjust the file offset
            switch (this.Model.Header?.MajorVersion)
            {
                case 4: fileOffset += (Model.Header as SabreTools.Models.SGA.Header4)?.FileDataOffset ?? 0; break;
                case 5: fileOffset += (Model.Header as SabreTools.Models.SGA.Header4)?.FileDataOffset ?? 0; break;
                case 6: fileOffset += (Model.Header as SabreTools.Models.SGA.Header6)?.FileDataOffset ?? 0; break;
                case 7: fileOffset += (Model.Header as SabreTools.Models.SGA.Header6)?.FileDataOffset ?? 0; break;
                default: return false;
            };

            // Get the file sizes
            long fileSize, outputFileSize;
            switch (this.Model.Header?.MajorVersion)
            {
                case 4:
                case 5:
                    fileSize = (file as SabreTools.Models.SGA.File4)?.SizeOnDisk ?? 0;
                    outputFileSize = (file as SabreTools.Models.SGA.File4)?.Size ?? 0;
                    break;
                case 6:
                    fileSize = (file as SabreTools.Models.SGA.File6)?.SizeOnDisk ?? 0;
                    outputFileSize = (file as SabreTools.Models.SGA.File6)?.Size ?? 0;
                    break;
                case 7:
                    fileSize = (file as SabreTools.Models.SGA.File7)?.SizeOnDisk ?? 0;
                    outputFileSize = (file as SabreTools.Models.SGA.File7)?.Size ?? 0;
                    break;
                default: return false;
            }

            // Read the compressed data directly
#if NET48
            byte[] compressedData = ReadFromDataSource((int)fileOffset, (int)fileSize);
#else
            byte[]? compressedData = ReadFromDataSource((int)fileOffset, (int)fileSize);
#endif
            if (compressedData == null)
                return false;

            // If the compressed and uncompressed sizes match
            byte[] data;
            if (fileSize == outputFileSize)
            {
                data = compressedData;
            }
            else
            {
                // Decompress the data
                data = new byte[outputFileSize];
                Inflater inflater = new Inflater();
                inflater.SetInput(compressedData);
                inflater.Inflate(data);
            }

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

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

            return false;
        }

        #endregion
    }
}