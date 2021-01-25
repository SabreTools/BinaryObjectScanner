using System;
using System.Collections.Generic;
using System.IO;
using Wise = WiseUnpacker.WiseUnpacker;

namespace BurnOutSharp.ProtectionType
{
    public class WiseInstaller
    {
        public static Dictionary<string, List<string>> CheckContents(Scanner scanner, string file, byte[] fileContent)
        {
            // WiseMain
            byte[] check = new byte[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E };
            if (fileContent.Contains(check, out int position))
            {
                Dictionary<string, List<string>> protections = new Dictionary<string, List<string>>
                {
                    [file ?? "NO FILENAME"] = new List<string> { "Wise Installation Wizard Module" + (scanner.IncludePosition ? $" (Index {position})" : string.Empty) },
                };

                if (file == null || !File.Exists(file))
                    return protections;

                if (scanner.ScanArchives)
                {
                    var subProtections = Scan(scanner, file);
                    Utilities.PrependToKeys(subProtections, file);
                    Utilities.AppendToDictionary(protections, subProtections);
                }

                return protections;
            }

            return null;
        }

        public static Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            // If the installer file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                Wise unpacker = new Wise();
                unpacker.ExtractTo(file, tempPath);

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
