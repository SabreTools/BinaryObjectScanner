using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDProtector : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("_cdp16.dat", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp16.dll", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp32.dat", useEndsWith: true), "CD-Protector"),
                new PathMatchSet(new PathMatch("_cdp32.dll", useEndsWith: true), "CD-Protector"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
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
