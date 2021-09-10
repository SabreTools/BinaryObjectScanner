using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDCops : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // CD-Cops,  ver. 
                new ContentMatchSet(new byte?[]
                {
                    0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                    0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "CD-Cops"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .grand section, if it exists
            var grandSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".grand"));
            if (grandSection != null)
            {
                int sectionAddr = (int)grandSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)grandSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // CD-Cops,  ver. 
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                            0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                        }, start: sectionAddr, end: sectionEnd),
                    GetVersion, "CD-Cops"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
                
                // return "CD-Cops (Unknown Version)";
            }

            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Original had "CDCOPS.DLL" required and all the rest in a combined OR
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            char[] version = new ArraySegment<byte>(fileContent, positions[0] + 15, 4).Select(b => (char)b).ToArray();
            if (version[0] == 0x00)
                return string.Empty;

            return new string(version);
        }
    }
}
