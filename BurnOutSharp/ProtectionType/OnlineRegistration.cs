using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class OnlineRegistration : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // TODO: Implement resource finding instead of using the built in methods
            // Assembly information lives in the .rsrc section
            // I need to find out how to navigate the resources in general
            // as well as figure out the specific resources for both
            // file info and MUI (XML) info. Once I figure this out,
            // that also opens the doors to easier assembly XML checks.

            var fvinfo = Utilities.GetFileVersionInfo(file);

            string name = fvinfo?.InternalName?.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("EReg", StringComparison.OrdinalIgnoreCase))
                return $"Executable-Based Online Registration {Utilities.GetFileVersion(file, fileContent, null)}";

            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rsrc section, if it exists
            var rsrcSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rsrc"));
            if (rsrcSection != null)
            {
                int sectionAddr = (int)rsrcSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rsrcSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + r + (char)0x00 + n + (char)0x00 + a + (char)0x00 + l + (char)0x00 + N + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 +  + (char)0x00 +  + (char)0x00 + E + (char)0x00 + R + (char)0x00 + e + (char)0x00 + g + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                            0x72, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6C, 0x00,
                            0x4E, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                            0x00, 0x00, 0x45, 0x00, 0x52, 0x00, 0x65, 0x00,
                            0x67, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "Executable-Based Online Registration"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
