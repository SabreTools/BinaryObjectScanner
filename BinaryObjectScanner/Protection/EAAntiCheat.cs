using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// EA Anti Cheat is a kernel-level anti-cheat developed and used by EA. (https://www.ea.com/security/news/eaac-deep-dive).
    /// List of games that contain EA Anti Cheat on Steam: https://steamdb.info/tech/AntiCheat/EA_AntiCheat/
    /// 
    /// An EasyAntiCheat installer is present in the file "EAAntiCheat.Installer.Tool.exe" found in "Plants vs. Zombies: Battle for Neighborville" (Steam Depot 1262241, Manifest 8124759833120741594).
    /// This could indicate that EasyAntiCheat is directly integrated into EA Anti Cheat.
    /// 
    /// The internal name appears to be "skyfall", as this is the Internal Name set to several EA Anti Cheat files, and the string "C:\dev\gitlab-runner\builds\r5uPUG7E\0\anticheat\skyfall\Build\Retail\EAAntiCheat.Installer.pdb" is present in "EAAntiCheat.Installer.Tool.exe".
    /// </summary>
    public class EAAntiCheat : IPathCheck, IPortableExecutableCheck
    {
        // TODO: Add support for detecting older versions, especially versions made before Easy Anti-Cheat was purchased by Epic Games.
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            // Found in "EAAntiCheat.GameServiceLauncher.exe" and "EAAntiCheat.Installer.exe" in "Plants vs. Zombies: Battle for Neighborville" (Steam Depot 1262241, Manifest 8124759833120741594).
            if (!string.IsNullOrEmpty(name) && name!.Contains("EA Anticheat"))
                return "EA Anti Cheat";

            name = pex.ProductName;
            // Found in "EAAntiCheat.GameServiceLauncher.exe" and "EAAntiCheat.Installer.exe" in "Plants vs. Zombies: Battle for Neighborville" (Steam Depot 1262241, Manifest 8124759833120741594).
            if (!string.IsNullOrEmpty(name) && name!.Contains("EA Anticheat"))
                return "EA Anti Cheat";

            name = pex.InternalName;
            // Found in "EAAntiCheat.GameServiceLauncher.exe" and "EAAntiCheat.Installer.exe" in "Plants vs. Zombies: Battle for Neighborville" (Steam Depot 1262241, Manifest 8124759833120741594).
            if (!string.IsNullOrEmpty(name) && name!.Equals("skyfall"))
                return "EA Anti Cheat";

            // TODO: Add check for "EA SPEAR AntiCheat Engineering" in ASN.1 certificate data. Found in files "EAAntiCheat.GameServiceLauncher.dll", "EAAntiCheat.GameServiceLauncher.exe", "EAAntiCheat.Installer.exe", and "preloader_l.dll".
            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in "Plants vs. Zombies: Battle for Neighborville" (Steam Depot 1262241, Manifest 8124759833120741594).
                new(new FilePathMatch("EAAntiCheat.cfg"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.GameServiceLauncher.dll"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.GameServiceLauncher.exe"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.splash.png"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.Installer.exe"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.Installer.Tool.exe"), "EA Anti Cheat"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in "Plants vs. Zombies: Battle for Neighborville" (Steam Depot 1262241, Manifest 8124759833120741594).
                new(new FilePathMatch("EAAntiCheat.cfg"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.GameServiceLauncher.dll"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.GameServiceLauncher.exe"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.splash.png"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.Installer.exe"), "EA Anti Cheat"),
                new(new FilePathMatch("EAAntiCheat.Installer.Tool.exe"), "EA Anti Cheat"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
