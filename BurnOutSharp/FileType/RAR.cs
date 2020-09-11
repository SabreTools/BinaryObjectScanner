using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;

namespace BurnOutSharp.FileType
{
    internal class RAR
    {
        public static bool ShouldScan(byte[] magic)
        {
            // RAR archive version 1.50 onwards
            if (magic.StartsWith(new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x00 }))
                return true;

            // RAR archive version 5.0 onwards
            if (magic.StartsWith(new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x01, 0x00 }))
                return true;

            return false;
        }

        public static List<string> Scan(Stream stream)
        {
            List<string> protections = new List<string>();

            // If the rar file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (RarArchive zipFile = RarArchive.Open(stream))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        // If an individual entry fails
                        try
                        {
                            // If we have a directory, skip it
                            if (entry.IsDirectory)
                                continue;

                            string tempfile = Path.Combine(tempPath, entry.Key);
                            entry.WriteToFile(tempfile);
                            string protection = ProtectionFind.ScanContent(tempfile);

                            // If tempfile cleanup fails
                            try
                            {
                                File.Delete(tempfile);
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
