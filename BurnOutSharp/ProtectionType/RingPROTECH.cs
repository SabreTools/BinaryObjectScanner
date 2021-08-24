using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class RingPROTECH : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // (char)0x00 + Allocator + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x41, 0x6C, 0x6C, 0x6F, 0x63, 0x61, 0x74,
                    0x6F, 0x72, 0x00, 0x00, 0x00, 0x00
                }, "Ring PROTECH [Check disc for physical ring]"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }
    }
}
