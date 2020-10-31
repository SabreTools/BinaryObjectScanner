using System;
using System.Collections.Generic;
using System.IO;
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

        public static Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream)
        {
            // If the xz file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

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
                    }
                    catch { }
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
