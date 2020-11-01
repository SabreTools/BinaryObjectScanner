using System;
using System.Collections.Generic;
using System.IO;
#if NET_FRAMEWORK
using Microsoft.Deployment.WindowsInstaller;
#endif

namespace BurnOutSharp.FileType
{
    internal class MSI
    {
        public static bool ShouldScan(byte[] magic)
        {
#if NET_FRAMEWORK
            if (magic.StartsWith(new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }))
                return true;
#endif

            return false;
        }

        // TODO: Add stream opening support
        public static Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
#if NET_FRAMEWORK
            // If the MSI file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (Database msidb = new Database(file, DatabaseOpenMode.ReadOnly))
                {
                    msidb.ExportAll(tempPath);
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
