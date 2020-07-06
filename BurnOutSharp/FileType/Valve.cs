using System;
using System.Collections.Generic;
using System.IO;
using HLExtract.Net;

namespace BurnOutSharp.FileType
{
    internal class Valve
    {
        // TODO: Re-enable when scanning is fixed
        public static bool ShouldScan(byte[] magic, string extension)
        {
            return false;

            // GCF
            if (magic.StartsWith(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }))
                return true;

            // GCF
            if (string.Equals(extension, "gcf", StringComparison.OrdinalIgnoreCase))
                return true;

            // VPK
            if (string.Equals(extension, "vpk", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        // TODO: Add stream opening support
        public static List<string> Scan(string file)
        {
            List<string> protections = new List<string>();

            // TODO: Figure out how to extract root AND/OR port native code
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            string[] args = new string[]
            {
                "-p", file,
                "-d", tempPath,
            };

            HLExtractProgram.Process(args);

            if (Directory.Exists(tempPath))
            {
                foreach (string tempFile in Directory.EnumerateFiles(tempPath, "*", SearchOption.AllDirectories))
                {
                    string protection = ProtectionFind.ScanContent(tempFile);

                    // If tempfile cleanup fails
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(protection))
                        protections.Add(tempFile);
                }
            }

            // If temp directory cleanup fails
            try
            {
                Directory.Delete(tempPath, true);
            }
            catch { }

            return protections;
        }
    }
}
