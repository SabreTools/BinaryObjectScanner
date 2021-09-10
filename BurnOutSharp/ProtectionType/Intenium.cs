using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Intenium : IContentCheck
    {
        /*
         * Possible strings for finding INTENIUM Trial & Buy Protection
         * 
         * Luxor Only:
         * - command_buyNowCb - 63 6F 6D 6D 61 6E 64 5F 62 75 79 4E 6F 77 43 62
         * - command_testTrialCb - 63 6F 6D 6D 61 6E 64 5F 74 65 73 74 54 72 69 61 6C 43 62
         * - PHRASE_TRIAL - 50 48 52 41 53 45 5F 54 52 49 41 4C
         * - V_TRIAL_GAME - 56 5F 54 52 49 41 4C 5F 47 41 4D 45
         * - V_FULL_GAME - 56 5F 46 55 4C 4C 5F 47 41 4D 45
         * 
         * Luxor, World, Cradle, and Kingdom:
         * - Trial + (char)0x00 + P - 54 72 69 61 6C 00 50
         *     + This is possibly followed by a version number. Undetermined if it's the software or protection version.
         * - NO NESTED PRMS SUPPORTED - 4E 4F 20 4E 45 53 54 45 44 20 50 52 4D 53 20 53 55 50 50 4F 52 54 45 44
         */

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // TODO: Find this inside of the .rsrc section using the executable header
            // Get the .rsrc section, if it exists
            var rsrcSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rsrc"));
            if (rsrcSection != null)
            {
                int sectionAddr = (int)rsrcSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rsrcSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // Trial + (char)0x00 + P
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x54, 0x72, 0x69, 0x61, 0x6C, 0x00, 0x50 }, start: sectionAddr, end: sectionEnd),
                    "INTENIUM Trial & Buy Protection"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
