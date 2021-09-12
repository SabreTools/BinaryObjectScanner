using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add version checking, if possible
    public class Armadillo : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists
            bool nicodeSection = pex.ContainsSection(".nicode", exact: true);
            if (nicodeSection)
                return "Armadillo";

            // Loop through all "extension" sections
            foreach (var section in sections.Where(s => s != null && Encoding.ASCII.GetString(s.Name).Trim('\0').EndsWith("1")))
            {
                int sectionAddr = (int)section.PointerToRawData;
                int sectionEnd = sectionAddr + (int)section.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // ARMDEBUG
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x41, 0x52, 0x4D, 0x44, 0x45, 0x42, 0x55, 0x47 }, start: sectionAddr, end: sectionEnd),
                        "Armadillo"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
