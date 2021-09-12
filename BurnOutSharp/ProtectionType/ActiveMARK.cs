using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to get version numbers
    public class ActiveMARK : IContentCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // " " + (char)0xC2 + (char)0x16 + (char)0x00 + (char)0xA8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0xB8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x86 + (char)0xC8 + (char)0x16 + (char)0x00 + (char)0x9A + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x10 + (char)0xC2 + (char)0x16 + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x20, 0xC2, 0x16, 0x00, 0xA8, 0xC1, 0x16, 0x00,
                    0xB8, 0xC1, 0x16, 0x00, 0x86, 0xC8, 0x16, 0x00,
                    0x9A, 0xC1, 0x16, 0x00, 0x10, 0xC2, 0x16, 0x00
                }, "ActiveMARK 5"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the last .bss section, if it exists
            var bssSection = pex.GetLastSection(".bss", exact: true);
            if (bssSection != null)
            {
                int sectionAddr = (int)bssSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)bssSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // TMSAMVOF
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x54, 0x4D, 0x53, 0x41, 0x4D, 0x56, 0x4F, 0x46 }, start: sectionAddr, end: sectionEnd),
                    "ActiveMARK"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }
    }
}
