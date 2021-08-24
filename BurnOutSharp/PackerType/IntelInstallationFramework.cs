using System;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var fvinfo = Utilities.GetFileVersionInfo(file);

            string name = fvinfo?.FileDescription.Trim();
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";
            }

            name = fvinfo?.ProductName.Trim();
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";
            }

            var matchers = new List<ContentMatchSet>
            {
                // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + l + (char)0x00 + ( + (char)0x00 + R + (char)0x00 + ) + (char)0x00 +   + (char)0x00 + I + (char)0x00 + n + (char)0x00 + s + (char)0x00 + t + (char)0x00 + a + (char)0x00 + l + (char)0x00 + l + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + F + (char)0x00 + r + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + w + (char)0x00 + o + (char)0x00 + r + (char)0x00 + k + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                    0x6C, 0x00, 0x28, 0x00, 0x52, 0x00, 0x29, 0x00,
                    0x20, 0x00, 0x49, 0x00, 0x6E, 0x00, 0x73, 0x00,
                    0x74, 0x00, 0x61, 0x00, 0x6C, 0x00, 0x6C, 0x00,
                    0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00,
                    0x6E, 0x00, 0x20, 0x00, 0x46, 0x00, 0x72, 0x00,
                    0x61, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x77, 0x00,
                    0x6F, 0x00, 0x72, 0x00, 0x6B, 0x00,
                }, Utilities.GetFileVersion, "Intel Installation Framework"),
                
                // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + l + (char)0x00 +   + (char)0x00 + I + (char)0x00 + n + (char)0x00 + s + (char)0x00 + t + (char)0x00 + a + (char)0x00 + l + (char)0x00 + l + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + F + (char)0x00 + r + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + w + (char)0x00 + o + (char)0x00 + r + (char)0x00 + k + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                    0x6C, 0x00, 0x20, 0x00, 0x49, 0x00, 0x6E, 0x00,
                    0x73, 0x00, 0x74, 0x00, 0x61, 0x00, 0x6C, 0x00,
                    0x6C, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00,
                    0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x46, 0x00,
                    0x72, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                    0x77, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6B, 0x00,
                }, Utilities.GetFileVersion, "Intel Installation Framework"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
        }
    }
}
