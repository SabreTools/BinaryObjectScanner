using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    // All current known info: https://web.archive.org/web/20190601101221mp_/https://fileforums.com/showthread.php?t=91941
    public class Zzxzz : IPathCheck
    {
        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(Path.Combine(path, "Zzxzz", "Zzz.aze").Replace("\\", "/"), "Zzxzz"),
                new PathMatchSet($"Zzxzz/", "Zzxzz"),
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
                new PathMatchSet(new PathMatch("Zzz.aze", useEndsWith: true), "Zzxzz"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
