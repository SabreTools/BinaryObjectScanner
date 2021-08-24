using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class XtremeProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // XPROT   
                new ContentMatchSet(new byte?[] { 0x58, 0x50, 0x52, 0x4F, 0x54, 0x20, 0x20, 0x20 }, "Xtreme-Protector"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }
    }
}
