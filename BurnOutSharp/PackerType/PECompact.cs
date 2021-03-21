using System.IO;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction and better version detection
    public class PECompact : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "PEC2"
            byte[] check = new byte[] { 0x50, 0x45, 0x43, 0x32 };
            if (fileContent.FirstPosition(check, out int position, end: 2048))
            {
                string version = GetVersion(file, position);
                if (version == null)
                    return "PE Compact 2" + (includePosition ? $" (Index {position})" : string.Empty);
                return $"PE Compact 2 Version {version}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // Another possible version string for version 1 is "PECO" (50 45 43 4F)

            // "pec1"
            check = new byte[] { 0x70, 0x65, 0x63, 0x31 };
            if (fileContent.FirstPosition(check, out position, end: 2048))
            {
                return "PE Compact 1" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // "PECompact2"
            check = new byte[] { 0x50, 0x45, 0x43, 0x6F, 0x6D, 0x70, 0x61, 0x63, 0x74, 0x32 };
            if (fileContent.FirstPosition(check, out position))
                return "PE Compact 2" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        private static string GetVersion(string path, int position)
        {
            if (!File.Exists(path))
                return null;
            try
            {
                using (var fs = File.OpenRead(path))
                {
                    fs.Seek(position + 4, SeekOrigin.Begin);
                    int version1 = fs.ReadByte();
                    int version2 = fs.ReadByte();
                    int version = (version2 << 8) | version1;
                    return $"{version}";
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
