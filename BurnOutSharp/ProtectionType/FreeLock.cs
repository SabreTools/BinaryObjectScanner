using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class FreeLock : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("FREELOCK.IMG", useEndsWith: true), "FreeLock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("FREELOCK.IMG", useEndsWith: true), "FreeLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
