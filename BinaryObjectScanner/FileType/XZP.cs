using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// XBox Package File
    /// </summary>
    public class XZP : IExtractable
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
                var xzp = SabreTools.Serialization.Wrappers.XZP.Create(stream);
                if (xzp == null)
                    return false;

                // Loop through and extract all files
                Directory.CreateDirectory(outDir);
                ExtractAll(xzp, outDir);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
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
            var directoryItem = Array.Find(item.Model.DirectoryItems, di => di?.FileNameCRC == directoryEntry.FileNameCRC);
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
                using Stream fs = File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
