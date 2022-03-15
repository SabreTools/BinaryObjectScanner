using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Alternative string possibilities:
    //      - \AlphaDiscLog.txt
    //      - \SETTEC
    //      - AlphaROM
    //      - SETTEC0000SETTEC1111
    //      - SOFTWARE\SETTEC
    // TODO: Are there version numbers?
    public class AlphaROM : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // \SETTEC
                    new ContentMatchSet(new byte?[] { 0x5C, 0x53, 0x45, 0x54, 0x54, 0x45, 0x43 }, "Alpha-ROM"),

                    // SETTEC0000
                    new ContentMatchSet(new byte?[] { 0x53, 0x45, 0x54, 0x54, 0x45, 0x43, 0x30, 0x30, 0x30, 0x30 }, "Alpha-ROM"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
