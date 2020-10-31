using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;

namespace BurnOutSharp.FileType
{
    internal class TapeArchive
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30 }))
                return true;

            if (magic.StartsWith(new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00 }))
                return true;

            return false;
        }

        public static List<string> Scan(Scanner parentScanner, Stream stream, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the tar file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create a new scanner for the new temp path
                Scanner subScanner = new Scanner(tempPath, parentScanner.FileProgress)
                {
                    IncludePosition = parentScanner.IncludePosition,
                    ScanAllFiles = parentScanner.ScanAllFiles,
                    ScanArchives = parentScanner.ScanArchives,
                };

                using (TarArchive tarFile = TarArchive.Open(stream))
                {
                    foreach (var entry in tarFile.Entries)
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
                    var fileProtections = ProtectionFind.Scan(tempPath, includePosition);
                    protections = fileProtections.Select(kvp => kvp.Key.Substring(tempPath.Length) + ": " + kvp.Value.TrimEnd()).ToList();

                    // If temp directory cleanup fails
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch { }
                }
            }
            catch { }

            return protections;
        }
    }
}
