using System;
using System.Collections.Generic;
using System.IO;
#if NET_FRAMEWORK
using LibMSPackN;
#endif

namespace BurnOutSharp.FileType
{
    internal class MicrosoftCAB
    {
        public static bool ShouldScan(byte[] magic)
        {
#if NET_FRAMEWORK
            if (magic.StartsWith(new byte[] { 0x4d, 0x53, 0x43, 0x46 }))
                return true;
#endif

            return false;
        }

        // TODO: Add stream opening support
        public static Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
#if NET_FRAMEWORK
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
#endif
            return null;
        }
    }
}
