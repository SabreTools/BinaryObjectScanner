using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class AlphaDVD : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("PlayDVD.exe", useEndsWith: true), "Alpha-DVD"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("PlayDVD.exe", useEndsWith: true), "Alpha-DVD"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
