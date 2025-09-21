using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Got renamed to Ubisoft Connect / Ubisoft Game Launcher
    /// </summary>
    public class Uplay : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            if (name.OptionalContains("Ubisoft Connect Installer"))
                return "Uplay / Ubisoft Connect";
            else if (name.OptionalContains("Ubisoft Connect Service"))
                return "Uplay / Ubisoft Connect";
            else if (name.OptionalContains("Ubisoft Connect WebCore"))
                return "Uplay / Ubisoft Connect";
            else if (name.OptionalContains("Ubisoft Crash Reporter"))
                return "Uplay / Ubisoft Connect";
            else if (name.OptionalContains("Ubisoft Game Launcher"))
                return "Uplay / Ubisoft Connect";
            else if (name.OptionalContains("Ubisoft Uplay Installer"))
                return "Uplay / Ubisoft Connect";
            else if (name.OptionalContains("Uplay launcher"))
                return "Uplay / Ubisoft Connect";
          
            name = exe.ProductName;

            // There's also a variant that looks like "Uplay <version> installer"
            if (name.OptionalContains("Ubisoft Connect"))
                return "Uplay / Ubisoft Connect";
            else if (name.OptionalContains("Uplay"))
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
