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
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(Path.Combine("ZDAT", "webmast.dxx").Replace("\\", "/"), "Tivola Ring Protection [Check disc for physical ring]"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(Path.Combine("ZDAT", "webmast.dxx").Replace("\\", "/"), "Tivola Ring Protection [Check disc for physical ring]"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
