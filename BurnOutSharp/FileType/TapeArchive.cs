using System;
using System.Collections.Generic;
using System.IO;
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

        public static List<string> Scan(Stream stream, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the tar file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

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

                            string tempfile = Path.Combine(tempPath, entry.Key);
                            entry.WriteToFile(tempfile);
                            string protection = ProtectionFind.ScanContent(tempfile, includePosition);

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
