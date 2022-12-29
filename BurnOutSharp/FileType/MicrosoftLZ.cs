using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Compression;
using BurnOutSharp.Interfaces;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Microsoft LZ-compressed Files (LZ32)
    /// </summary>
    /// <remarks>This is treated like an archive type due to the packing style</remarks>
    public class MicrosoftLZ : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the LZ file itself fails
            try
            {
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

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
