using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Origin : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // O + (char)0x00 + r + (char)0x00 + i + (char)0x00 + g + (char)0x00 + i + (char)0x00 + n + (char)0x00 + S + (char)0x00 + e + (char)0x00 + t + (char)0x00 + u + (char)0x00 + p + (char)0x00 + . + (char)0x00 + e + (char)0x00 + x + (char)0x00 + e + (char)0x00
                new ContentMatchSet(new byte?[] { 0x4F, 0x00, 0x72, 0x00, 0x69, 0x00, 0x67, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00, 0x70, 0x00, 0x2E, 0x00, 0x65, 0x00, 0x78, 0x00, 0x65, 0x00 }, "Origin"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = GetContentMatchSets();
            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("OriginSetup.exe", useEndsWith: true), "Origin"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("OriginSetup.exe", useEndsWith: true), "Origin"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
