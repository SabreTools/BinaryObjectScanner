using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibMSPackN;

namespace BurnOutSharp.FileType
{
    internal class MicrosoftCAB
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x4d, 0x53, 0x43, 0x46 }))
                return true;

            return false;
        }

        // TODO: Add stream opening support
        public static List<string> Scan(string file, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the cab file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (MSCabinet cabfile = new MSCabinet(file))
                {
                    foreach (var sub in cabfile.GetFiles())
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(tempPath, sub.Filename);
                            sub.ExtractTo(tempFile);

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
                                protections.Add($"\r\n{sub.Filename} - {protection}");
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
