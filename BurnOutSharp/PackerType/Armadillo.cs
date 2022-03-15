using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add version checking, if possible
    public class Armadillo : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists
            bool nicodeSection = pex.ContainsSection(".nicode", exact: true);
            if (nicodeSection)
                return "Armadillo";

            // Loop through all "extension" sections -- usually .data1 or .text1
            foreach (var section in sections.Where(s => s != null && Encoding.ASCII.GetString(s.Name).Trim('\0').EndsWith("1")))
            {
                string sectionName = Encoding.ASCII.GetString(section.Name).Trim('\0');
                var sectionRaw = pex.ReadRawSection(pex.SourceArray, sectionName);
                var matchers = new List<ContentMatchSet>
                {
                    // ARMDEBUG
                    new ContentMatchSet(new byte?[] { 0x41, 0x52, 0x4D, 0x44, 0x45, 0x42, 0x55, 0x47 }, $"Armadillo"),
                };

                string match = MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
