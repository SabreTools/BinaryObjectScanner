using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // Got renamed to Ubisoft Connect / Ubisoft Game Launcher
    public class Uplay : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Connect Installer"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Connect Service"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Connect WebCore"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Crash Reporter"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Game Launcher"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Uplay Installer"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Uplay launcher"))
                return "Uplay / Ubisoft Connect";

            // There's also a variant that looks like "Uplay <version> installer"
            name = pex.ProductName;
            if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Connect"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Uplay"))
                return "Uplay / Ubisoft Connect";

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("UbisoftConnect.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher64.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncherInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("Uplay.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayCrashReporter.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayInstaller.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayService.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayWebCore.exe"), "Uplay / Ubisoft Connect"),
            };

            return MatchUtil.GetAllMatches(files ?? System.Array.Empty<string>(), matchers, any: true);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("UbisoftConnect.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher64.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncherInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("Uplay.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayCrashReporter.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayInstaller.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayService.exe"), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new FilePathMatch("UplayWebCore.exe"), "Uplay / Ubisoft Connect"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
