using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;

namespace BurnOutSharp.FileType
{
    internal class SevenZip
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c }))
                return true;

            return false;
        }

        public static Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream)
        {
            // If the 7-zip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (SevenZipArchive sevenZipFile = SevenZipArchive.Open(stream))
                {
                    foreach (var entry in sevenZipFile.Entries)
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
            }
            catch { }

            return null;
        }
    }
}
