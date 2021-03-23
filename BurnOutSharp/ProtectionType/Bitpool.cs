using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Bitpool : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("bitpool.rsc", useEndsWith: true), "Bitpool"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("bitpool.rsc", useEndsWith: true), "Bitpool"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
