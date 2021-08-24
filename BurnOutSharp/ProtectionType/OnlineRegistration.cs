using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class OnlineRegistration : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + r + (char)0x00 + n + (char)0x00 + a + (char)0x00 + l + (char)0x00 + N + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 +  + (char)0x00 +  + (char)0x00 + E + (char)0x00 + R + (char)0x00 + e + (char)0x00 + g + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                    0x72, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6C, 0x00,
                    0x4E, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                    0x00, 0x00, 0x45, 0x00, 0x52, 0x00, 0x65, 0x00,
                    0x67, 0x00
                }, Utilities.GetFileVersion, "Executable-Based Online Registration"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }
    }
}
