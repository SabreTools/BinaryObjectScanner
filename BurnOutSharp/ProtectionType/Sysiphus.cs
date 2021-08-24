using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
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

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
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
