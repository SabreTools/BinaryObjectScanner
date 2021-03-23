using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Steam : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SteamInstall.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.ini", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamRetailInstaller.dmg", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamSetup.exe", useEndsWith: true), "Steam"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SteamInstall.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.ini", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamRetailInstaller.dmg", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamSetup.exe", useEndsWith: true), "Steam"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
