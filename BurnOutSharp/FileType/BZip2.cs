using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;

namespace BurnOutSharp.FileType
{
    internal class BZip2
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x42, 0x52, 0x68 }))
                return true;

            return false;
        }

        public static List<string> Scan(Stream stream, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the 7-zip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (BZip2Stream bz2File = new BZip2Stream(stream, CompressionMode.Decompress, true))
                {
                    // If an individual entry fails
                    try
                    {
                        string tempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                        using (FileStream fs = File.OpenWrite(tempFile))
                        {
                            bz2File.CopyTo(fs);
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
