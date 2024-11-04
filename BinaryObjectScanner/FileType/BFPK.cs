using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;
#endif

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// BFPK custom archive format
    /// </summary>
    public class BFPK : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            try
            {
                // Create the wrapper
                var bfpk = SabreTools.Serialization.Wrappers.BFPK.Create(stream);
                if (bfpk == null)
                    return false;

                // Extract all files
                Directory.CreateDirectory(outDir);
                ExtractAll(bfpk, outDir);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract all files from the BFPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.BFPK item, string outputDirectory)
        {
            // If we have no files
            if (item.Model.Files == null || item.Model.Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Files.Length; i++)
            {
                allExtracted &= ExtractFile(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the BFPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(SabreTools.Serialization.Wrappers.BFPK item, int index, string outputDirectory)
        {
            // If we have no files
            if (item.Model.Files == null || item.Model.Files.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= item.Model.Files.Length)
                return false;

            // Get the file information
            var file = item.Model.Files[index];
            if (file == null)
                return false;

            // Get the read index and length
            int offset = file.Offset + 4;
            int compressedSize = file.CompressedSize;

            // Some files can lack the length prefix
            if (compressedSize > item.GetEndOfFile())
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
                using FileStream fs = File.OpenWrite(filePath);

                // Read the data block
                var data = item.ReadFromDataSource(offset, compressedSize);
                if (data == null)
                    return false;

                // If we have uncompressed data
                if (compressedSize == file.UncompressedSize)
                {
                    fs.Write(data, 0, compressedSize);
                }
#if NET462_OR_GREATER || NETCOREAPP
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        ZlibStream zs = new ZlibStream(ms, CompressionMode.Decompress);
                        zs.CopyTo(fs);
                    }
#endif

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
