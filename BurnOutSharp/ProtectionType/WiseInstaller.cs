using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wise = WiseUnpacker.WiseUnpacker;

namespace BurnOutSharp.ProtectionType
{
    public class WiseInstaller
    {
        public static List<string> CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "WiseMain"
            byte[] check = new byte[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E };
            if (fileContent.Contains(check, out int position))
            {
                List<string> protections = new List<string> { "Wise Installation Wizard Module" + (includePosition ? $" (Index {position})" : string.Empty) };
                if (!File.Exists(file))
                    return protections;

                protections.AddRange(WiseInstaller.Scan(file, includePosition));

                return protections;
            }

            return null;
        }

        public static List<string> Scan(string file, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the installer file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                Wise unpacker = new Wise();
                unpacker.ExtractTo(file, tempPath);

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
