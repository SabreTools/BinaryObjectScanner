using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDProtector : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("_cdp16.dat", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp16.dll", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp32.dat", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp32.dll", useEndsWith: true), "CD-Protector"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("_cdp16.dat", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp16.dll", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp32.dat", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp32.dll", useEndsWith: true), "CD-Protector"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
