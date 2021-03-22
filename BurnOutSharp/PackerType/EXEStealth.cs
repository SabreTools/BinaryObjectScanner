using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class EXEStealth : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // ??[[__[[_ + (char)0x00 + {{ + (char)0x0 + (char)0x00 + {{ + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x0 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + ?;??;??
                new Matcher(new byte?[]
                {
                    0x3F, 0x3F, 0x5B, 0x5B, 0x5F, 0x5F, 0x5B, 0x5B,
                    0x5F, 0x00, 0x7B, 0x7B, 0x00, 0x00, 0x7B, 0x7B,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x20, 0x3F, 0x3B, 0x3F, 0x3F, 0x3B, 0x3F,
                    0x3F
                }, "EXE Stealth"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }
    }
}
