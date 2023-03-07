using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CactusDataShield : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Limit these checks to Mac binaries
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // CDSPlayer
                    new ContentMatchSet(new byte?[] { 0x43, 0x44, 0x53, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72 }, "Cactus Data Shield 200"),

                    // yucca.cds
                    new ContentMatchSet(new byte?[] { 0x79, 0x75, 0x63, 0x63, 0x61, 0x2E, 0x63, 0x64, 0x73 }, "Cactus Data Shield 200"),
                };

                if (contentMatchSets != null && contentMatchSets.Any())
                    return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }
    }
}
