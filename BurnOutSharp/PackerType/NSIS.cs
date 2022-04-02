using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class NSIS : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string description = pex.GetManifestDescription();
            if (!string.IsNullOrWhiteSpace(description) && description.StartsWith("Nullsoft Install System"))
                return $"NSIS {description.Substring("Nullsoft Install System".Length).Trim()}";

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // NullsoftInst
                    new ContentMatchSet(new byte?[]
                    {
                        0x4E, 0x75, 0x6C, 0x6C, 0x73, 0x6F, 0x66, 0x74,
                        0x49, 0x6E, 0x73, 0x74
                    }, "NSIS"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}