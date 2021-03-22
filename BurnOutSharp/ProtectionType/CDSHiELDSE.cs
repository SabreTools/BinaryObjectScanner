using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDSHiELDSE : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // ~0017.tmp
                new Matcher(new byte?[] { 0x7E, 0x30, 0x30, 0x31, 0x37, 0x2E, 0x74, 0x6D, 0x70 }, "CDSHiELD SE"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }
    }
}
