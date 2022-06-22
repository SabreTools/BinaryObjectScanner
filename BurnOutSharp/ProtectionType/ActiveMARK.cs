using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to get version numbers
    public class ActiveMARK : IContentCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // " " + (char)0xC2 + (char)0x16 + (char)0x00 + (char)0xA8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0xB8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x86 + (char)0xC8 + (char)0x16 + (char)0x00 + (char)0x9A + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x10 + (char)0xC2 + (char)0x16 + (char)0x00
                    new ContentMatchSet(new byte?[]
                    {
                        0x20, 0xC2, 0x16, 0x00, 0xA8, 0xC1, 0x16, 0x00,
                        0xB8, 0xC1, 0x16, 0x00, 0x86, 0xC8, 0x16, 0x00,
                        0x9A, 0xC1, 0x16, 0x00, 0x10, 0xC2, 0x16, 0x00
                    }, "ActiveMARK 5"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the entry point data, if it exists
            if (pex.EntryPointRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // (char)0x89 + (char)0x25 + * + * + * + * + * + * + * + * + (char)0xEB
                    new ContentMatchSet(new byte?[]
                    {
                        0x89, 0x25, null, null, null, null, null, null,
                        null, null, 0xEB,
                    }, "ActiveMARK"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the overlay data, if it exists
            if (pex.OverlayRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // (char)0x00 + TMSAMVOH
                    new ContentMatchSet(new byte?[] { 0x00, 0x54, 0x4D, 0x53, 0x41, 0x4D, 0x56, 0x4F, 0x48, }, "ActiveMARK"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.OverlayRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the last .bss section, if it exists
            var bssSectionRaw = pex.ReadRawSection(".bss", first: false);
            if (bssSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // TMSAMVOF
                    new ContentMatchSet(new byte?[] { 0x54, 0x4D, 0x53, 0x41, 0x4D, 0x56, 0x4F, 0x46 }, "ActiveMARK"),
                };

                string match = MatchUtil.GetFirstMatch(file, bssSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
