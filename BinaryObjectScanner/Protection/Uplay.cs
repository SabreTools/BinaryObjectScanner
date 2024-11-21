using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // Got renamed to Ubisoft Connect / Ubisoft Game Launcher
    public class Uplay : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name?.Contains("Ubisoft Connect Installer") == true)
                return "Uplay / Ubisoft Connect";
            else if (name?.Contains("Ubisoft Connect Service") == true)
                return "Uplay / Ubisoft Connect";
            else if (name?.Contains("Ubisoft Connect WebCore") == true)
                return "Uplay / Ubisoft Connect";
            else if (name?.Contains("Ubisoft Crash Reporter") == true)
                return "Uplay / Ubisoft Connect";
            else if (name?.Contains("Ubisoft Game Launcher") == true)
                return "Uplay / Ubisoft Connect";
            else if (name?.Contains("Ubisoft Uplay Installer") == true)
                return "Uplay / Ubisoft Connect";
            else if (name?.Contains("Uplay launcher") == true)
                return "Uplay / Ubisoft Connect";

            // There's also a variant that looks like "Uplay <version> installer"
            name = pex.ProductName;
            if (name?.Contains("Ubisoft Connect") == true)
                return "Uplay / Ubisoft Connect";
            else if (name?.Contains("Uplay") == true)
                return "Uplay / Ubisoft Connect";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("UbisoftConnect.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UbisoftGameLauncher.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UbisoftGameLauncher64.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UbisoftGameLauncherInstaller.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("Uplay.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayCrashReporter.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayInstaller.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayService.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayWebCore.exe"), "Uplay / Ubisoft Connect"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("UbisoftConnect.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UbisoftGameLauncher.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UbisoftGameLauncher64.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UbisoftGameLauncherInstaller.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("Uplay.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayCrashReporter.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayInstaller.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayService.exe"), "Uplay / Ubisoft Connect"),
                new(new FilePathMatch("UplayWebCore.exe"), "Uplay / Ubisoft Connect"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
