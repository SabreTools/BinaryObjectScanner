using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // .cenega
            new ContentMatchSet(new byte?[] { 0x2E, 0x63, 0x65, 0x6E, 0x65, 0x67, 0x61 }, "Cenega ProtectDVD"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }
    }
}
