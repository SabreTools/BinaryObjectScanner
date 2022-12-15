using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Gefest Protection System is a completely unknown protection. There is only one known sample (Redump entry 93700), and no useful information online that's been found as of yet.
    /// It's unknown if the errors present in the only known sample are a manufacturing error, or if they're related to the protection.
    /// Despite this sample supposedly being the "[License Version]", no license key check appears to take place.
    /// The sample is protected by a seemingly unrelated packer. Until that packer can be investigated further, here's the infomation that can be found online about it:
    /// https://itsafety.net/report/20210912-9edb2d29cbdf1ac7e24fb32d99c1347a-splintercell-exe_general-threat
    /// https://xakep.ru/2003/10/06/20015/
    /// https://j3qx.wordpress.com/2008/12/20/%D0%BF%D0%B8%D1%80%D0%B0%D1%82%D1%81%D0%BA%D0%B8%D0%B5-%D0%B7%D0%B0%D0%BC%D0%B0%D1%88%D0%BA%D0%B8-%D0%B8%D0%BB%D0%B8-%D0%B7%D0%B0%D1%89%D0%B8%D1%82%D0%B0-%D0%BE%D1%82-7wolf/
    /// http://www.imho.ws/showthread.php?t=34225
    /// </summary>
    public class Gefest : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the HeaderPaddingData, if it exists
            if (pex.HeaderPaddingData != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Found in "FDMASTER.EXE" in Redump entry 93700.
                    // Gefest Protection System
                    new ContentMatchSet(new byte?[]
                    {
                        0x47, 0x65, 0x66, 0x65, 0x73, 0x74, 0x20, 0x50,
                        0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x69, 0x6F, 
                        0x6E, 0x20, 0x53, 0x79, 0x73, 0x74, 0x65, 0x6D
                    }, GetVersion, "Gefest Protection System"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.HeaderPaddingData, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Possibly related file "31AD0095.fil" that appears to contain intentional errors found in Redump entry 93700.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Possibly related file "31AD0095.fil" that appears to contain intentional errors found in Redump entry 93700.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // TODO: Verify that this works properly with other samples. Look at possibly ending the version string at the first 0x20 character.
            int position = positions[0];
            char[] version = new ArraySegment<byte>(fileContent, position + 25, 30).Select(b => (char)b).ToArray();
            return new string(version);
        }
    }
}
