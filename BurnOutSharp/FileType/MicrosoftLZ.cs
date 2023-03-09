using System;
using System.IO;
using BinaryObjectScanner.Compression;
using BinaryObjectScanner.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Microsoft LZ-compressed Files (LZ32)
    /// </summary>
    /// <remarks>This is treated like an archive type due to the packing style</remarks>
    public class MicrosoftLZ : IExtractable
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

            byte[] data = LZ.Decompress(stream);

            // Create the temp filename
            string tempFile = "temp.bin";
            if (!string.IsNullOrEmpty(file))
            {
                string expandedFilePath = LZ.GetExpandedName(file, out _);
                tempFile = Path.GetFileName(expandedFilePath).TrimEnd('\0');
                if (tempFile.EndsWith(".ex"))
                    tempFile += "e";
                else if (tempFile.EndsWith(".dl"))
                    tempFile += "l";
            }

            tempFile = Path.Combine(tempPath, tempFile);

            // Write the file data to a temp file
            using (Stream tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                tempStream.Write(data, 0, data.Length);
            }

            return tempPath;
        }
    }
}
