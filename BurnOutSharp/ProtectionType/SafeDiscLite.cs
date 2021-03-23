using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SafeDiscLite : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
