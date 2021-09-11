using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    public class NSIS : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string description = Utilities.GetManifestDescription(pex);
            if (!string.IsNullOrWhiteSpace(description) && description.StartsWith("Nullsoft Install System"))
                return $"NSIS {description.Substring("Nullsoft Install System".Length).Trim()}";

            // Get the .data section, if it exists
            var dataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".data"));
            if (dataSection != null)
            {
                int sectionAddr = (int)dataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)dataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // NullsoftInst
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x4E, 0x75, 0x6C, 0x6C, 0x73, 0x6F, 0x66, 0x74,
                            0x49, 0x6E, 0x73, 0x74
                        }, start: sectionAddr, end: sectionEnd),
                    "NSIS"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}