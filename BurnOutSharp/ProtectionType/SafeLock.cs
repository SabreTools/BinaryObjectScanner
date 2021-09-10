using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SafeLock : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // SafeLock
                new ContentMatchSet(new byte?[] { 0x53, 0x61, 0x66, 0x65, 0x4C, 0x6F, 0x63, 0x6B }, "SafeLock"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SafeLock.dat", useEndsWith: true), "SafeLock"),
                new PathMatchSet(new PathMatch("SafeLock.001", useEndsWith: true), "SafeLock"),
                new PathMatchSet(new PathMatch("SafeLock.128", useEndsWith: true), "SafeLock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
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
