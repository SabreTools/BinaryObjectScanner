using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class ByteShield : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Byteshield.dll", useEndsWith: true), "ByteShield"),
                new PathMatchSet(new PathMatch(".bbz", useEndsWith: true), "ByteShield"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
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
