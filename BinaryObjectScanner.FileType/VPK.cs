using System;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Valve Package File
    /// </summary>
    public class VPK : IExtractable
    {
        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
        {
            try
            {
                // Create the wrapper
                var vpk = SabreTools.Serialization.Wrappers.VPK.Create(stream);
                if (vpk == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Loop through and extract all files
                ExtractAll(vpk, tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Extract all files from the VPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.VPK item, string outputDirectory)
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
        /// Extract a file from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(SabreTools.Serialization.Wrappers.VPK item, int index, string outputDirectory)
        {
            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= item.Model.DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = item.Model.DirectoryItems[index];
            if (directoryItem?.DirectoryEntry == null)
                return false;

            // If we have an item with no archive
#if NET48
            byte[] data;
#else
            byte[]? data;
#endif
            if (directoryItem.DirectoryEntry.ArchiveIndex == SabreTools.Models.VPK.Constants.HL_VPK_NO_ARCHIVE)
            {
                if (directoryItem.PreloadData == null)
                    return false;

                data = directoryItem.PreloadData;
            }
            else
            {
                // If we have invalid archives
                if (item.ArchiveFilenames == null || item.ArchiveFilenames.Length == 0)
                    return false;

                // If we have an invalid index
                if (directoryItem.DirectoryEntry.ArchiveIndex < 0 || directoryItem.DirectoryEntry.ArchiveIndex >= item.ArchiveFilenames.Length)
                    return false;

                // Get the archive filename
                string archiveFileName = item.ArchiveFilenames[directoryItem.DirectoryEntry.ArchiveIndex];
                if (string.IsNullOrWhiteSpace(archiveFileName))
                    return false;

                // If the archive doesn't exist
                if (!File.Exists(archiveFileName))
                    return false;

                // Try to open the archive
#if NET48
                Stream archiveStream = null;
#else
                Stream? archiveStream = null;
#endif
                try
                {
                    // Open the archive
                    archiveStream = File.OpenRead(archiveFileName);

                    // Seek to the data
                    archiveStream.Seek(directoryItem.DirectoryEntry.EntryOffset, SeekOrigin.Begin);

                    // Read the directory item bytes
                    data = archiveStream.ReadBytes((int)directoryItem.DirectoryEntry.EntryLength);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    archiveStream?.Close();
                }

                // If we have preload data, prepend it
                if (data != null && directoryItem.PreloadData != null)
                    data = directoryItem.PreloadData.Concat(data).ToArray();
            }

            // If there is nothing to write out
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{directoryItem.Name}.{directoryItem.Extension}";
            if (!string.IsNullOrWhiteSpace(directoryItem.Path))
                filename = Path.Combine(directoryItem.Path, filename);

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

            return true;
        }
    }
}
