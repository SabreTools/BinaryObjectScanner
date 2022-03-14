using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDLock : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
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
                    // 2 + (char)0xF2 + (char)0x02 + (char)0x82 + (char)0xC3 + (char)0xBC + (char)0x0B + $ + (char)0x99 + (char)0xAD + 'C + (char)0xE4 + (char)0x9D + st + (char)0x99 + (char)0xFA + 2$ + (char)0x9D + )4 + (char)0xFF + t
                    new ContentMatchSet(new byte?[]
                    {
                        0x32, 0xF2, 0x02, 0x82, 0xC3, 0xBC, 0x0B, 0x24,
                        0x99, 0xAD, 0x27, 0x43, 0xE4, 0x9D, 0x73, 0x74,
                        0x99, 0xFA, 0x32, 0x24, 0x9D, 0x29, 0x34, 0xFF,
                        0x74
                    }, "CD-Lock"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
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
                new PathMatchSet(new PathMatch(".AFP", useEndsWith: true), "CD-Lock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch(".AFP", useEndsWith: true), "CD-Lock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
