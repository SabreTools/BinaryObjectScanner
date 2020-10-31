using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibMSPackN;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;

namespace BurnOutSharp.FileType
{
    internal class MSI
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }))
                return true;

            return false;
        }

        // TODO: Add stream opening support
        public static List<string> Scan(Scanner parentScanner, string file, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the MSI file itself fails
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

                using (Database msidb = new Database(file, DatabaseOpenMode.ReadOnly))
                {
                    msidb.ExportAll(tempPath);
                }

                // Collect and format all found protections
                var fileProtections = ProtectionFind.Scan(tempPath, includePosition);
                protections = fileProtections.Select(kvp => kvp.Key.Substring(tempPath.Length) + ": " + kvp.Value.TrimEnd()).ToList();

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }
            }
            catch { }

            return protections;
        }
    }
}
