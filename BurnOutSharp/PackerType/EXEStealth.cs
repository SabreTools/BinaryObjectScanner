using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Figure out how to more granularly determine versions like PiD,
    // at least for the 2.41 -> 2.75 range
    // TODO: Detect 3.15 and up (maybe looking for `Metamorphism`)
    public class EXEStealth : IContentCheck, IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                 {
                     // ??[[__[[_ + (char)0x00 + {{ + (char)0x0 + (char)0x00 + {{ + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x0 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + ?;??;??
                     new ContentMatchSet(new byte?[]
                     {
                         0x3F, 0x3F, 0x5B, 0x5B, 0x5F, 0x5F, 0x5B, 0x5B,
                         0x5F, 0x00, 0x7B, 0x7B, 0x00, 0x00, 0x7B, 0x7B,
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                         0x00, 0x20, 0x3F, 0x3B, 0x3F, 0x3F, 0x3B, 0x3F,
                         0x3F
                     }, "EXE Stealth"),
                 };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPEContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the ExeS/EXES section, if it exists
            bool exesSection = pex.ContainsSection("ExeS", exact: true) || pex.ContainsSection("EXES", exact: true);
            if (exesSection)
                return "EXE Stealth 2.41-2.75";

            // Get the mtw section, if it exists
            bool mtwSection = pex.ContainsSection("mtw", exact: true);
            if (mtwSection)
                return "EXE Stealth 1.1";

            // Get the rsrr section, if it exists
            bool rsrrSection = pex.ContainsSection("rsrr", exact: true);
            if (rsrrSection)
                return "EXE Stealth 2.76";

            return null;
        }
    }
}
