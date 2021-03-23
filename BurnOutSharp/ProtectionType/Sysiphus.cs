using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // V SUHPISYSDVD
            new ContentMatchSet(new byte?[]
            {
                0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53,
                0x59, 0x53, 0x44, 0x56, 0x44
            }, GetVersion, "Sysiphus DVD"),

            // V SUHPISYSDVD
            new ContentMatchSet(new byte?[]
            {
                0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53,
                0x59, 0x53
            }, GetVersion, "Sysiphus"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] - 3;
            char subVersion = (char)fileContent[index];
            index++;
            index++;
            char version = (char)fileContent[index];

            if (char.IsNumber(version) && char.IsNumber(subVersion))
                return $"{version}.{subVersion}";

            return string.Empty;
        }
    }
}
