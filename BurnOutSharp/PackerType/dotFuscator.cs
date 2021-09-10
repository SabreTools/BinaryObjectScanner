using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class dotFuscator : IContentCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // DotfuscatorAttribute
                new ContentMatchSet(new byte?[]
                {
                    0x44, 0x6F, 0x74, 0x66, 0x75, 0x73, 0x63, 0x61,
                    0x74, 0x6F, 0x72, 0x41, 0x74, 0x74, 0x72, 0x69,
                    0x62, 0x75, 0x74, 0x65
                }, "dotFuscator"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }
    }
}
