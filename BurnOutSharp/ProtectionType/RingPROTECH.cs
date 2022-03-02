using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // Renamed to ProRing at some point
    public class RingPROTECH : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // (char)0x00 + Allocator + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00
                    new ContentMatchSet(new byte?[]
                    {
                        0x00, 0x41, 0x6C, 0x6C, 0x6F, 0x63, 0x61, 0x74,
                        0x6F, 0x72, 0x00, 0x00, 0x00, 0x00
                    }, "Ring PROTECH / ProRing [Check disc for physical ring]"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        // TODO: Confirm if these checks are only for ProRing or if they are also for older Ring PROTECH

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("protect.pro", useEndsWith: true), "Ring PROTECH / ProRing [Check disc for physical ring]"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("protect.pro", useEndsWith: true), "Ring PROTECH / ProRing [Check disc for physical ring]"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
