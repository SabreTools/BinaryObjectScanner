using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace BurnOutSharp.FileType
{
    public class PKZIP : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            // PKZIP
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x03, 0x04 }))
                return true;

            // PKZIP (Empty Archive)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x05, 0x06 }))
                return true;

            // PKZIP (Spanned Archive)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x07, 0x08 }))
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

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the zip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (ZipArchive zipFile = ZipArchive.Open(stream))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        // If an individual entry fails
                        try
                        {
                            // If we have a directory, skip it
                            if (entry.IsDirectory)
                                continue;

                            string tempFile = Path.Combine(tempPath, entry.Key);
                            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                            entry.WriteToFile(tempFile);
                        }
                        catch (Exception ex)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine(ex);
                        }
                    }
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
