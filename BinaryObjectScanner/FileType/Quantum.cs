using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Quantum Archive
    /// </summary>
    public class Quantum : IExtractable
    {
        /// <inheritdoc/>
        public string? Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string? Extract(Stream? stream, string file, bool includeDebug)
        {
            try
            {
                // Create the wrapper
                var quantum = SabreTools.Serialization.Wrappers.Quantum.Create(stream);
                if (quantum == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Extract all files
                ExtractAll(quantum, tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Extract all files from the Quantum archive to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.Quantum item, string outputDirectory)
        {
            // If we have no files
            if (item.Model.FileList == null || item.Model.FileList.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.FileList.Length; i++)
            {
                allExtracted &= ExtractFile(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the Quantum archive to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(SabreTools.Serialization.Wrappers.Quantum item, int index, string outputDirectory)
        {
            // If we have no files
            if (item.Model.Header == null || item.Model.Header.FileCount == 0 || item.Model.FileList == null || item.Model.FileList.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= item.Model.FileList.Length)
                return false;

            // Get the file information
            var fileDescriptor = item.Model.FileList[index];

            // Read the entire compressed data
            int compressedDataOffset = (int)item.Model.CompressedDataOffset;
            int compressedDataLength = item.GetEndOfFile() - compressedDataOffset;
            var compressedData = item.ReadFromDataSource(compressedDataOffset, compressedDataLength);

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
            // if (string.IsNullOrEmpty(outputDirectory))
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
    }
}
