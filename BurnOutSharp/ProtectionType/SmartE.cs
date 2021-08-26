using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SmartE : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // BITARTS
                new ContentMatchSet(new byte?[] { 0x42, 0x49, 0x54, 0x41, 0x52, 0x54, 0x53 }, "SmartE"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00001.TMP", useEndsWith: true), "SmartE"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00002.TMP", useEndsWith: true), "SmartE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00001.TMP", useEndsWith: true), "SmartE"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00002.TMP", useEndsWith: true), "SmartE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
