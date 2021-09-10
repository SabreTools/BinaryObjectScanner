using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class EXEStealth : IContentCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // ??[[__[[_ + (char)0x00 + {{ + (char)0x0 + (char)0x00 + {{ + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x0 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + ?;??;??
                new ContentMatchSet(new byte?[]
                {
                    0x3F, 0x3F, 0x5B, 0x5B, 0x5F, 0x5F, 0x5B, 0x5B,
                    0x5F, 0x00, 0x7B, 0x7B, 0x00, 0x00, 0x7B, 0x7B,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x20, 0x3F, 0x3B, 0x3F, 0x3F, 0x3B, 0x3F,
                    0x3F
                }, "EXE Stealth"),
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
