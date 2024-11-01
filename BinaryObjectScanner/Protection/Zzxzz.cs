using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

namespace BinaryObjectScanner.Protection
{
    // All current known info: https://web.archive.org/web/20190601101221mp_/https://fileforums.com/showthread.php?t=91941
    public class Zzxzz : IPathCheck
    {
        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
#if NET20 || NET35
                new(Path.Combine(Path.Combine(path, "Zzxzz"), "Zzz.aze"), "Zzxzz"),
#else
                new(Path.Combine(path, "Zzxzz", "Zzz.aze"), "Zzxzz"),
#endif
                new($"Zzxzz/", "Zzxzz"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("Zzz.aze"), "Zzxzz"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
