#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // Got renamed to Ubisoft Connect / Ubisoft Game Launcher
    public class Uplay : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (!string.IsNullOrEmpty(name) && name!.Contains("Ubisoft Connect Installer"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Ubisoft Connect Service"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Ubisoft Connect WebCore"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Ubisoft Crash Reporter"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Ubisoft Game Launcher"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Ubisoft Uplay Installer"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Uplay launcher"))
                return "Uplay / Ubisoft Connect";

            // There's also a variant that looks like "Uplay <version> installer"
            name = pex.ProductName;
            if (!string.IsNullOrEmpty(name) && name!.Contains("Ubisoft Connect"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Uplay"))
                return "Uplay / Ubisoft Connect";

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
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
