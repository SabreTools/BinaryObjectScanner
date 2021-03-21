using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class XtremeProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // XPROT   
                new Matcher(new byte?[] { 0x58, 0x50, 0x52, 0x4F, 0x54, 0x20, 0x20, 0x20 }, "Xtreme-Protector"),
            };

            return Utilities.GetContentMatches(file, fileContent, matchers, includePosition);
        }
    }
}
