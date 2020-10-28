using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static List<string> Scan(Stream stream, bool includePosition = false)
        {
            List<string> protections = new List<string>();

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

                            // Collect and format all found protections
                            var fileProtections = ProtectionFind.Scan(tempFile, includePosition);
                            string protection = string.Join("\r\n", fileProtections.Select(kvp => kvp.Key + ": " + kvp.Value.TrimEnd()));

                            // If tempfile cleanup fails
                            try
                            {
                                File.Delete(tempFile);
                            }
                            catch { }

                            if (!string.IsNullOrEmpty(protection))
                                protections.Add($"\r\n{entry.Key} - {protection}");
                        }
                        catch { }
                    }

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
