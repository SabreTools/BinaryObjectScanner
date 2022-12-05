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

                         new ContentMatchSet(new List<ContentMatch>
                         {
                             // P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + T + (char)0x00 + e + (char)0x00 + c + (char)0x00 + h + (char)0x00 + n + (char)0x00 + o + (char)0x00 + l + (char)0x00 + o + (char)0x00 + g + (char)0x00 + y + (char)0x00
                             new ContentMatch(new byte?[]
                             {
                                 0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x74, 0x00,
                                 0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00,
                                 0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x54, 0x00,
                                 0x65, 0x00, 0x63, 0x00, 0x68, 0x00, 0x6E, 0x00,
                                 0x6F, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x67, 0x00,
                                 0x79, 0x00
                             }),

                             // // PSA_GetDiscLabel
                              new ContentMatch(new byte?[]
                              {
                                  0x50, 0x53, 0x41, 0x5F, 0x47, 0x65, 0x74, 0x44,
                                  0x69, 0x73, 0x63, 0x4C, 0x61, 0x62, 0x65, 0x6C
                              }),

                             // (c) Protection Technology
                              new ContentMatch(new byte?[]
                              {
                                  0x28, 0x63, 0x29, 0x20, 0x50, 0x72, 0x6F, 0x74,
                                  0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54,
                                  0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67,
                                  0x79
                              }),

                             // TradeName
                             new ContentMatch(new byte?[] { 0x54, 0x72, 0x61, 0x64, 0x65, 0x4E, 0x61, 0x6D, 0x65 }),
                         }, GetVersion, "StarForce [**Large Combined Check**] (Unconfirmed - Please report to us on Github)"),
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
