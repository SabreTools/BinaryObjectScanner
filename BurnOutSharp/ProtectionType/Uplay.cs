using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Uplay : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("UbisoftGameLauncherInstaller.exe", useEndsWith: true), "Uplay"),
                new PathMatchSet(new PathMatch("UplayInstaller.exe", useEndsWith: true), "Uplay"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("UbisoftGameLauncherInstaller.exe", useEndsWith: true), "Uplay"),
                new PathMatchSet(new PathMatch("UplayInstaller.exe", useEndsWith: true), "Uplay"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
