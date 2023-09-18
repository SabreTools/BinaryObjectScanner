using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    // Renamed to ProRing at some point
    // TODO: Investigate Redump entry 82475, which PiD detects as having "Optgraph Copy-X / Ring-Protech".
    public class RingPROTECH : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
#else
        public string? CheckContents(string file, byte[] fileContent, bool includeDebug)
#endif
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
                    }, "Ring PROTECH / ProRing [Check disc for physical ring] (Unconfirmed - Please report to us on Github)"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        // TODO: Confirm if these checks are only for ProRing or if they are also for older Ring PROTECH

        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 94161
                new PathMatchSet(new PathMatch("protect.pro", useEndsWith: true), "Ring PROTECH / ProRing [Check disc for physical ring]"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 94161
                new PathMatchSet(new PathMatch("protect.pro", useEndsWith: true), "Ring PROTECH / ProRing [Check disc for physical ring]"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
