using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class Steam : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (!string.IsNullOrEmpty(name) && name!.Contains("Steam Autorun Setup"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Steam Client API"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Steam Client Engine"))
                return $"Steam Client Engine {pex.GetInternalVersion()}";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Steam Client Service"))
                return "Steam";

            name = pex.ProductName;
            if (!string.IsNullOrEmpty(name) && name!.Contains("Steam Autorun Setup"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Steam Client API"))
                return "Steam";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Steam Client Service"))
                return "Steam";

            /// TODO: Add entry point checks
            /// https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Steam.2.sg

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // These checks are grouped together due to the names being generic on their own (Redump entry 91450).
                new(new List<PathMatch>
                {
                    // TODO: Identify based on "Steam(TM)" being present in "Description" but not in "File Description".
                    // Overmatches on some files, such as IA item "ASMEsMechanicalEngineeringToolkit1997December".
                    // new FilePathMatch("steam.exe"),

                    new FilePathMatch("steam.ini"),
                    
                    // TODO: Identify file using MSI property parsing.
                    new FilePathMatch("steam.msi"),
                }, "Steam"),

                new(new FilePathMatch("steam_api.dll"), "Steam"),
                new(new FilePathMatch("steam_api64.dll"), "Steam"),
                new(new FilePathMatch("steam_install_agreement.rtf"), "Steam"),
                new(new FilePathMatch("SteamInstall.bom"), "Steam"),
                new(new FilePathMatch("SteamInstall.exe"), "Steam"),
                new(new FilePathMatch("SteamInstall.info"), "Steam"),
                new(new FilePathMatch("SteamInstall.ini"), "Steam"),
                new(new FilePathMatch("SteamInstall.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall.pax.gz"), "Steam"),
                new(new FilePathMatch("SteamInstall.pkg"), "Steam"),
                new(new FilePathMatch("SteamInstall.sizes"), "Steam"),
                new(new FilePathMatch("SteamInstall_Czech.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_English.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_French.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_German.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Italian.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Polish.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Russian.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Spanish.msi"), "Steam"),
                new(new FilePathMatch("SteamRetailInstaller"), "Steam"),
                new(new FilePathMatch("SteamRetailInstaller.dmg"), "Steam"),
                new(new FilePathMatch("SteamService.exe"), "Steam"),
                new(new FilePathMatch("SteamSetup.exe"), "Steam"),
                new(new FilePathMatch("steamxboxutil64.exe"), "Steam"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("steam_api.dll"), "Steam"),
                new(new FilePathMatch("steam_api64.dll"), "Steam"),
                new(new FilePathMatch("steam_install_agreement.rtf"), "Steam"),
                new(new FilePathMatch("SteamInstall.bom"), "Steam"),
                new(new FilePathMatch("SteamInstall.exe"), "Steam"),
                new(new FilePathMatch("SteamInstall.info"), "Steam"),
                new(new FilePathMatch("SteamInstall.ini"), "Steam"),
                new(new FilePathMatch("SteamInstall.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall.pax.gz"), "Steam"),
                new(new FilePathMatch("SteamInstall.pkg"), "Steam"),
                new(new FilePathMatch("SteamInstall.sizes"), "Steam"),
                new(new FilePathMatch("SteamInstall_Czech.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_English.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_French.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_German.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Italian.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Polish.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Russian.msi"), "Steam"),
                new(new FilePathMatch("SteamInstall_Spanish.msi"), "Steam"),
                new(new FilePathMatch("SteamRetailInstaller"), "Steam"),
                new(new FilePathMatch("SteamRetailInstaller.dmg"), "Steam"),
                new(new FilePathMatch("SteamService.exe"), "Steam"),
                new(new FilePathMatch("SteamSetup.exe"), "Steam"),
                new(new FilePathMatch("steamxboxutil64.exe"), "Steam"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
