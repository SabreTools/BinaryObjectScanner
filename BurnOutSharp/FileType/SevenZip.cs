using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// 7-zip archive
    /// </summary>
    public class SevenZip : IExtractable
    {
        /// <inheritdoc/>
        public string Extract(string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file)
        {
            // Create a temp output directory
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            using (SevenZipArchive sevenZipFile = SevenZipArchive.Open(stream))
            {
                foreach (var entry in sevenZipFile.Entries)
                {
                    // If we have a directory, skip it
                    if (entry.IsDirectory)
                        continue;

                    string tempFile = Path.Combine(tempPath, entry.Key);
                    entry.WriteToFile(tempFile);
                }
            }

            return tempPath;
        }
    }
}
