using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;
using LibMSPackSharp;

namespace BurnOutSharp.FileType
{
    // Specification available at http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf
    public class MicrosoftCAB : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte?[] { 0x4d, 0x53, 0x43, 0x46 }))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the cab file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create the decompressor
                var decompressor = Library.CreateCABDecompressor(null);

                // Open the cab file
                var cabFile = decompressor.Open(file);
                if (cabFile == null)
                {
                    if (scanner.IncludeDebug) Console.WriteLine($"Error occurred opening of '{file}': {decompressor.Error}");
                    return null;
                }

                // If we have a previous CAB and it exists, don't try scanning
                string directory = Path.GetDirectoryName(file);
                if (!string.IsNullOrWhiteSpace(cabFile.PreviousName))
                {
                    if (File.Exists(Path.Combine(directory, cabFile.PreviousName)))
                        return null;
                }

                // If there are additional next CABs, add those
                var cabFile1 = cabFile;
                while (!string.IsNullOrWhiteSpace(cabFile1?.NextName))
                {
                    var cabFile2 = decompressor.Open(Path.Combine(directory, cabFile1.PreviousName));
                    Error error = decompressor.Append(cabFile1, cabFile2);
                    if (error != Error.MSPACK_ERR_OK)
                    {
                        if (scanner.IncludeDebug) Console.WriteLine($"Error occurred appending '{cabFile2.Filename}' to '{cabFile1.Filename}': {error}");

                        decompressor.FixMSZip = true;
                        decompressor.Salvage = true;
                        break;
                    }

                    cabFile1 = cabFile2;
                }

                // Loop through the found internal files
                var sub = cabFile.Files;
                while (sub != null)
                {
                    // If an individual entry fails
                    try
                    {
                        // The trim here is for some very odd and stubborn files
                        string tempFile = Path.Combine(tempPath, sub.Filename.TrimEnd('\0', ' ', '.'));
                        Error error = decompressor.Extract(sub, tempFile);
                        if (error != Error.MSPACK_ERR_OK)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine($"Error occurred during extraction of '{sub.Filename}': {error}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (scanner.IncludeDebug) Console.WriteLine(ex);
                    }

                    sub = sub.Next;
                }

                // Destroy the decompressor
                Library.DestroyCABDecompressor(decompressor);

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
                Utilities.StripFromKeys(protections, tempPath);

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
