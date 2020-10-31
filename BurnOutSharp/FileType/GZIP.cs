using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;

namespace BurnOutSharp.FileType
{
    internal class GZIP
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x1f, 0x8b }))
                return true;

            return false;
        }

        public static Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream)
        {
            // If the gzip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (GZipArchive zipFile = GZipArchive.Open(stream))
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
                            entry.WriteToFile(tempFile);
                        }
                        catch { }
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }

                return protections;
            }
            catch { }

            return null;
        }
    }
}
