using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add version checking, if possible
    public class Armadillo : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;
        // {
        //     // TODO: Remove this if the below section check is proven
        //     return new List<ContentMatchSet>
        //     {
        //         // .nicode + (char)0x00
        //         new ContentMatchSet(new byte?[] { 0x2E, 0x6E, 0x69, 0x63, 0x6F, 0x64, 0x65, 0x00 }, "Armadillo"),
        //     };
        // }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists -- TODO: Confirm this check with a real disc
            var nicodeSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".nicode"));
            if (nicodeSection != null)
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
