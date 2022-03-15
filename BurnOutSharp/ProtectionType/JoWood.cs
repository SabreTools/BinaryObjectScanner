using System;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // Interesting note: the former protection "Xtreme-Protector" was found to be a
    // subset of the JoWood X-Prot checks, more specifically the XPROT section check
    // that now outputs a version of v1.4+.
    public class JoWood : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .ext     section, if it exists
            var extSection = pex.ContainsSection(".ext    ", exact: true);
            if (extSection)
            {
                // Get the .dcrtext section, if it exists
                var dcrtextSectionRaw = pex.ReadRawSection(pex.SourceArray, ".dcrtext");
                if (dcrtextSectionRaw != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // kernel32.dll + (char)0x00 + (char)0x00 + (char)0x00 + VirtualProtect
                        new ContentMatchSet(new byte?[]
                        {
                            0x6B, 0x65, 0x72, 0x6E, 0x65, 0x6C, 0x33, 0x32,
                            0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x00, 0x00, 0x56,
                            0x69, 0x72, 0x74, 0x75, 0x61, 0x6C, 0x50, 0x72,
                            0x6F, 0x74, 0x65, 0x63, 0x74
                        }, GetVersion, "JoWood X-Prot"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, dcrtextSectionRaw, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }

                return "JoWood X-Prot v1.0-v1.3";
            }

            // Get the HC09     section, if it exists
            bool hc09Section = pex.ContainsSection("HC09    ", exact: true);
            if (hc09Section)
                return "JoWood X-Prot v2"; // TODO: Can we get more granular with the version?

            // Get the XPROT    section, if it exists
            var xprotSection = pex.ContainsSection("XPROT   ", exact: true);
            if (xprotSection)
                return "JoWood X-Prot v1.4+"; // TODO: Can we get more granular with the version?

            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            int position = positions[0];
            char[] version = new ArraySegment<byte>(fileContent, position + 67, 8).Select(b => (char)b).ToArray();
            return new string(version);
        }
    }
}
