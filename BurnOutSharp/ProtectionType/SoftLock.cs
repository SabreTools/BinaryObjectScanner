using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SoftLock : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SOFTLOCKI.dat", useEndsWith: true), "SoftLock"),
                new PathMatchSet(new PathMatch("SOFTLOCKC.dat", useEndsWith: true), "SoftLock"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SOFTLOCKI.dat", useEndsWith: true), "SoftLock"),
                new PathMatchSet(new PathMatch("SOFTLOCKC.dat", useEndsWith: true), "SoftLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
