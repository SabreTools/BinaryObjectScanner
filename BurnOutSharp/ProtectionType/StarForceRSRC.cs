using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class StarForceRSRC : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rsrc section, if it exists
            var rsrcSection = pex.GetLastSection(".rsrc", exact: true);
            if (rsrcSection != null)
            {
                var rsrcSectionData = pex.GetLastSectionData(".rsrc");
                if (rsrcSectionData != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + e + (char)0x00 + d + (char)0x00 +   + (char)0x00 + M + (char)0x00 + o + (char)0x00 + d + (char)0x00 + u + (char)0x00 + l + (char)0x00 + e + (char)0x00
                        new ContentMatchSet(
                            new byte?[]
                            {
                                0x50, 0x00, 0x72, 0x00, 0x6f, 0x00, 0x74, 0x00,
                                0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x65, 0x00,
                                0x64, 0x00, 0x20, 0x00, 0x4d, 0x00, 0x6f, 0x00,
                                0x64, 0x00, 0x75, 0x00, 0x6c, 0x00, 0x65, 0x00
                            },
                            "StarForce 5 [**Protected Module**] (Unconfirmed - Please report to us on Github)"),
                    };

                    string match = string.Join(", ", MatchUtil.GetAllMatches(file, rsrcSectionData, matchers, includeDebug));
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
            }

            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            return $"{Utilities.GetInternalVersion(file)} ({fileContent.Skip(positions[1] + 22).TakeWhile(c => c != 0x00)})";
        }
    }
}
