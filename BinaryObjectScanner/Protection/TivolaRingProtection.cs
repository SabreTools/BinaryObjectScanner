using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    /// <remarks>
    /// This protection needs far more research
    /// </remarks>
    public class TivolaRingProtection : IPathCheck
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
                new PathMatchSet(Path.Combine("ZDAT", "webmast.dxx").Replace("\\", "/"), "Tivola Ring Protection [Check disc for physical ring]"),
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
                new PathMatchSet(Path.Combine("ZDAT", "webmast.dxx").Replace("\\", "/"), "Tivola Ring Protection [Check disc for physical ring]"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
