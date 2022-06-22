using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class Steam : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (!string.IsNullOrEmpty(name) && name.Contains("Steam Autorun Setup"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Steam Client API"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Steam Client Engine"))
                return $"Steam Client Engine {Utilities.GetInternalVersion(pex)}";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Steam Client Service"))
                return "Steam";

            name = pex.ProductName;
            if (!string.IsNullOrEmpty(name) && name.Contains("Steam Autorun Setup"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Steam Client API"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Steam Client Service"))
                return "Steam";

            /// TODO: Add entry point checks
            /// https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Steam.2.sg

            return null;
        }

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
