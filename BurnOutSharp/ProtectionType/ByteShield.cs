using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class ByteShield : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Byteshield.dll", useEndsWith: true), "ByteShield"),
                new PathMatchSet(new PathMatch(".bbz", useEndsWith: true), "ByteShield"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Byteshield.dll", useEndsWith: true), "ByteShield"),
                new PathMatchSet(new PathMatch(".bbz", useEndsWith: true), "ByteShield"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
