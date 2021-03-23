using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SVKProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // ?SVKP + (char)0x00 + (char)0x00
                new ContentMatchSet(new byte?[] { 0x3F, 0x53, 0x56, 0x4B, 0x50, 0x00, 0x00 }, "SVK Protector"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }
    }
}
