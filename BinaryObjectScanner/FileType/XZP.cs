using System;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// XBox Package File
    /// </summary>
    public class XZP : IExtractable
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
                var xzp = SabreTools.Serialization.Wrappers.XZP.Create(stream);
                if (xzp == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Loop through and extract all files
                ExtractAll(xzp, tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Extract all files from the XZP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.XZP item, string outputDirectory)
        {
            // If we have no directory entries
            if (item.Model.DirectoryEntries == null || item.Model.DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractFile(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the XZP to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(SabreTools.Serialization.Wrappers.XZP item, int index, string outputDirectory)
        {
            // If we have no directory entries
            if (item.Model.DirectoryEntries == null || item.Model.DirectoryEntries.Length == 0)
                return false;

            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // If the directory entry index is invalid
            if (index < 0 || index >= item.Model.DirectoryEntries.Length)
                return false;

            // Get the directory entry
            var directoryEntry = item.Model.DirectoryEntries[index];
            if (directoryEntry == null)
                return false;

            // Get the associated directory item
            var directoryItem = item.Model.DirectoryItems.Where(di => di?.FileNameCRC == directoryEntry.FileNameCRC).FirstOrDefault();
            if (directoryItem == null)
                return false;

            // Load the item data
            var data = item.ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
            if (data == null)
                return false;

            // Create the filename
            var filename = directoryItem.Name;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
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
