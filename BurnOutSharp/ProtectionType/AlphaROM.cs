using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class AlphaROM : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // SETTEC
            new ContentMatchSet(new byte?[] { 0x53, 0x45, 0x54, 0x54, 0x45, 0x43 }, "Alpha-ROM"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }
    }
}
