using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, which should be possible with LibMSPackN, but it refuses to extract due to SFX files lacking the typical CAB identifiers.
    public class MicrosoftCABSFX : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var fvinfo = Utilities.GetFileVersionInfo(file);

            string name = fvinfo?.InternalName.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Wextract", StringComparison.OrdinalIgnoreCase))
            {
                string version = GetVersion(file, fileContent, null);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"Microsoft CAB SFX v{Utilities.GetFileVersion(file)}";

                return "Microsoft CAB SFX";
            }

            name = fvinfo?.OriginalFilename.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("WEXTRACT.EXE", StringComparison.OrdinalIgnoreCase))
            {
                string version = GetVersion(file, fileContent, null);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"Microsoft CAB SFX v{Utilities.GetFileVersion(file)}";

                return "Microsoft CAB SFX";
            }

            var matchers = new List<ContentMatchSet>
            {
                // wextract_cleanup
                new ContentMatchSet(new byte?[]
                {
                    0x77, 0x65, 0x78, 0x74, 0x72, 0x61, 0x63, 0x74, 
                    0x5F, 0x63, 0x6C, 0x65, 0x61, 0x6E, 0x75, 0x70,
                }, GetVersion, "Microsoft CAB SFX"),

                // W + (char)0x00 + e + (char)0x00 + x + (char)0x00 + t + (char)0x00 + r + (char)0x00 + a + (char)0x00 + c + (char)0x00 + t + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x57, 0x00, 0x65, 0x00, 0x78, 0x00, 0x74, 0x00, 
                    0x72, 0x00, 0x61, 0x00, 0x63, 0x00, 0x74, 0x00,
                }, GetVersion, "Microsoft CAB SFX"),

                // W + (char)0x00 + E + (char)0x00 + X + (char)0x00 + T + (char)0x00 + R + (char)0x00 + A + (char)0x00 + C + (char)0x00 + T + (char)0x00 + . + (char)0x00 + E + (char)0x00 + X + (char)0x00 + E + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x57, 0x00, 0x45, 0x00, 0x58, 0x00, 0x54, 0x00,
                    0x52, 0x00, 0x41, 0x00, 0x43, 0x00, 0x54, 0x00,
                    0x2E, 0x00, 0x45, 0x00, 0x58, 0x00, 0x45, 0x00,
                }, GetVersion, "Microsoft CAB SFX"),

                /* This detects a different but similar type of SFX that uses Microsoft CAB files.
                   Further research is needed to see if it's just a different version or entirely separate. */
                // MSCFu
                new ContentMatchSet(new byte?[] { 0x4D, 0x53, 0x43, 0x46, 0x75 }, GetVersion, "Microsoft CAB SFX"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        // This method of version detection is suboptimal because the version is sometimes the version of the included software, not the SFX itself.
        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            string version = Utilities.GetFileVersion(file);
            if (!string.IsNullOrWhiteSpace(version))
                return $"v{version}";

            return string.Empty;
        }
    }
}
