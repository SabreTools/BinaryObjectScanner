using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SafeLock : IContentCheck, IPathCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // SafeLock
            new ContentMatchSet(new byte?[] { 0x53, 0x61, 0x66, 0x65, 0x4C, 0x6F, 0x63, 0x6B }, "SafeLock"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SafeLock.dat", useEndsWith: true), "SafeLock"),
                new PathMatchSet(new PathMatch("SafeLock.001", useEndsWith: true), "SafeLock"),
                new PathMatchSet(new PathMatch("SafeLock.128", useEndsWith: true), "SafeLock"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SafeLock.dat", useEndsWith: true), "SafeLock"),
                new PathMatchSet(new PathMatch("SafeLock.001", useEndsWith: true), "SafeLock"),
                new PathMatchSet(new PathMatch("SafeLock.128", useEndsWith: true), "SafeLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
