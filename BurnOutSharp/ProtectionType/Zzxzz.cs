using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Zzxzz : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(Path.Combine(path, "Zzxzz", "Zzz.aze").Replace("\\", "/"), "Zzxzz"),
                new PathMatchSet($"Zzxzz/", "Zzxzz"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Zzz.aze", useEndsWith: true), "Zzxzz"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
