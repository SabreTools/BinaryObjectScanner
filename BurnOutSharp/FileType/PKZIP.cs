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

        public static List<string> Scan(Stream stream)
        {
            List<string> protections = new List<string>();

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
