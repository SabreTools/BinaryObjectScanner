using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Package File
    /// </summary>
    public class PAK : IExtractable
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
                var pak = SabreTools.Serialization.Wrappers.PAK.Create(stream);
                if (pak == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Loop through and extract all files
                ExtractAll(pak, tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Extract all files from the PAK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.PAK item, string outputDirectory)
        {
            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.DirectoryItems.Length; i++)
            {
                allExtracted &= ExtractFile(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the PAK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(SabreTools.Serialization.Wrappers.PAK item, int index, string outputDirectory)
        {
            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= item.Model.DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = item.Model.DirectoryItems[index];
            if (directoryItem == null)
                return false;

            // Read the item data
            var data = item.ReadFromDataSource((int)directoryItem.ItemOffset, (int)directoryItem.ItemLength);
            if (data == null)
                return false;

            // Create the filename
            var filename = directoryItem.ItemName;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename ?? $"file{index}");

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
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

            return true;
        }
    }
}
