using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // .cenega
                new ContentMatchSet(new byte?[] { 0x2E, 0x63, 0x65, 0x6E, 0x65, 0x67, 0x61 }, "Cenega ProtectDVD"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = GetContentMatchSets();
            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }
    }
}
