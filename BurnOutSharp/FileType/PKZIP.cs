using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace BurnOutSharp.FileType
{
    internal class PKZIP
    {
        public static bool ShouldScan(byte[] magic)
        {
            // PKZIP
            if (magic.StartsWith(new byte[] { 0x50, 0x4b, 0x03, 0x04 }))
                return true;

            // PKZIP (Empty Archive)
            if (magic.StartsWith(new byte[] { 0x50, 0x4b, 0x05, 0x06 }))
                return true;

            // PKZIP (Spanned Archive)
            if (magic.StartsWith(new byte[] { 0x50, 0x4b, 0x07, 0x08 }))
                return true;

            return false;
        }

        public static Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream)
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

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch { }

            return null;
        }
    }
}
