using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Compressors.Xz;

namespace BurnOutSharp.FileType
{
    internal class XZ
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00 }))
                return true;

            return false;
        }

        public static List<string> Scan(Scanner parentScanner, Stream stream, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the 7-zip file itself fails
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

                using (XZStream xzFile = new XZStream(stream))
                {
                    // If an individual entry fails
                    try
                    {
                        string tempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                        using (FileStream fs = File.OpenWrite(tempFile))
                        {
                            xzFile.CopyTo(fs);
                        }

                        // Collect and format all found protections
                        var fileProtections = ProtectionFind.Scan(tempFile, includePosition);
                        string protection = string.Join("\r\n", fileProtections.Select(kvp => kvp.Key.Substring(tempPath.Length) + ": " + kvp.Value.TrimEnd()));

                        // If tempfile cleanup fails
                        try
                        {
                            File.Delete(tempFile);
                        }
                        catch { }

                        if (!string.IsNullOrEmpty(protection))
                            protections.Add($"\r\n{protection}");
                    }
                    catch { }

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
