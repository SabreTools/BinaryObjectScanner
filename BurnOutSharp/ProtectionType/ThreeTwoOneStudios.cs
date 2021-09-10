using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class ThreeTwoOneStudios : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
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
                    // 3 + (char)0x00 + 2 + (char)0x00 + 1 + (char)0x00 + S + (char)0x00 + t + (char)0x00 + u + (char)0x00 + d + (char)0x00 + i + (char)0x00 + o + (char)0x00 + s + (char)0x00 +   + (char)0x00 + A + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + v + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x33, 0x00, 0x32, 0x00, 0x31, 0x00, 0x53, 0x00,
                            0x74, 0x00, 0x75, 0x00, 0x64, 0x00, 0x69, 0x00,
                            0x6F, 0x00, 0x73, 0x00, 0x20, 0x00, 0x41, 0x00,
                            0x63, 0x00, 0x74, 0x00, 0x69, 0x00, 0x76, 0x00,
                            0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00,
                            0x6E, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    "321Studios Online Activation"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
