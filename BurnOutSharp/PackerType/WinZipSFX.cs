using System;
using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp.PackerType
{
    public class WinZipSFX : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "WinZip Self-Extractor"
            byte[] check = new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53, 0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72, 0x61, 0x63, 0x74, 0x6F, 0x72 };
            if (fileContent.Contains(check, out int position))
            {
                // "version="
                byte[] check2 = new byte[] { 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D };
                if (fileContent.Contains(check2, out int position2))
                    return $"WinZip SFX {GetV3PlusVersion(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);
                else
                    return $"WinZip SFX {GetV2Version(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

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
            return null;
        }

        private static string GetV3PlusVersion(byte[] fileContent, int position)
        {
            string version = "Version 3+";
            return $"{version}";
        }

        private static string GetV2Version(byte[] fileContent, int position)
        {
            string version = "Version 2.x";
            return $"{version}";
        }
    }
}
