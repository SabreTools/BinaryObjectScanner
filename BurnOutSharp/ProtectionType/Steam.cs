using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Investigate what content checks can be done here
    public class Steam : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("steam_api.dll", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("steam_api64.dll", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.bom", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.info", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.ini", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.pax.gz", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.pkg", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.sizes", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Czech.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_English.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_French.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_German.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Italian.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Polish.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Russian.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Spanish.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamRetailInstaller", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamRetailInstaller.dmg", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamService.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamSetup.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("steamxboxutil64.exe", useEndsWith: true), "Steam"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("steam_api.dll", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("steam_api64.dll", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.bom", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.info", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.ini", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.pax.gz", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.pkg", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall.sizes", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Czech.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_English.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_French.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_German.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Italian.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Polish.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Russian.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamInstall_Spanish.msi", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamRetailInstaller", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamRetailInstaller.dmg", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamService.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("SteamSetup.exe", useEndsWith: true), "Steam"),
                new PathMatchSet(new PathMatch("steamxboxutil64.exe", useEndsWith: true), "Steam"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
