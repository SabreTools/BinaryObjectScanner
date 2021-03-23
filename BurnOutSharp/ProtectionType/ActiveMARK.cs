using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class ActiveMARK : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // TMSAMVOF
                new ContentMatchSet(new byte?[] { 0x54, 0x4D, 0x53, 0x41, 0x4D, 0x56, 0x4F, 0x46 }, "ActiveMARK"),

                // " " + (char)0xC2 + (char)0x16 + (char)0x00 + (char)0xA8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0xB8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x86 + (char)0xC8 + (char)0x16 + (char)0x00 + (char)0x9A + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x10 + (char)0xC2 + (char)0x16 + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x20, 0xC2, 0x16, 0x00, 0xA8, 0xC1, 0x16, 0x00,
                    0xB8, 0xC1, 0x16, 0x00, 0x86, 0xC8, 0x16, 0x00,
                    0x9A, 0xC1, 0x16, 0x00, 0x10, 0xC2, 0x16, 0x00
                }, "ActiveMARK 5"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }
    }
}
