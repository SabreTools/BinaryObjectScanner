using System;
using System.Collections.Generic;
using System.IO;
using Wise = WiseUnpacker.WiseUnpacker;

namespace BurnOutSharp.PackerType
{
    public class WiseInstaller : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // WiseMain
            byte[] check = new byte[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E };
            if (fileContent.FirstPosition(check, out int position))
                return "Wise Installation Wizard Module" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
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
