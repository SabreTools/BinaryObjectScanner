using System;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, which should be possible with LibMSPackN, but it refuses to extract due to SFX files lacking the typical CAB identifiers.
    public class MicrosoftCABSFX : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // TODO: Add byte-based checks for these as well for when we're working on stream alone
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
                    0x5F, 0x63, 0x6C, 0x65, 0x61, 0x6E, 0x75, 0x70
                }, GetVersion, "Microsoft CAB SFX"),

                /* This detects a different but similar type of SFX that uses Microsoft CAB files.
                   Further research is needed to see if it's just a different version or entirely separate. */
                // MSCFu
                new ContentMatchSet(new byte?[] { 0x4D, 0x53, 0x43, 0x46, 0x75 }, GetVersion, "Microsoft CAB SFX"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
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
