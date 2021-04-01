using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDKey : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + r + (char)0x00 + n + (char)0x00 + a + (char)0x00 + l + (char)0x00 + N + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 +  + (char)0x00 +  + (char)0x00 + C + (char)0x00 + D + (char)0x00 + K + (char)0x00 + e + (char)0x00 + y + (char)0x00
            new ContentMatchSet(new byte?[]
            {
                0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                0x72, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6C, 0x00,
                0x4E, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                0x00, 0x00, 0x43, 0x00, 0x44, 0x00, 0x4B, 0x00,
                0x65, 0x00, 0x79, 0x00
            }, Utilities.GetFileVersion, "CD-Key / Serial"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }
    }
}
