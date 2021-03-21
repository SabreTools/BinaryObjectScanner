using System.IO;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction and better version detection
    public class PECompact : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // Another possible version string for version 1 is "PECO" (50 45 43 4F)

            // "pec1"
            byte[] check = new byte[] { 0x70, 0x65, 0x63, 0x31 };
            if (fileContent.FirstPosition(check, out int position, end: 2048))
                return "PE Compact 1" + (includePosition ? $" (Index {position})" : string.Empty);

            // "PEC2"
            check = new byte[] { 0x50, 0x45, 0x43, 0x32 };
            if (fileContent.FirstPosition(check, out position, end: 2048))
            {
                string version = GetVersion(fileContent, position);
                if (version == null)
                    return "PE Compact 2" + (includePosition ? $" (Index {position})" : string.Empty);

                return $"PE Compact 2 v{version}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // "PECompact2"
            check = new byte[] { 0x50, 0x45, 0x43, 0x6F, 0x6D, 0x70, 0x61, 0x63, 0x74, 0x32 };
            if (fileContent.FirstPosition(check, out position))
                return "PE Compact 2" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            int version1 = fileContent[position + 4];
            int version2 = fileContent[position + 5];
            int version = (version2 << 8) | version1;
            return $"{version}";
        }
    }
}
