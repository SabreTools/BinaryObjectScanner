using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // Got renamed to Ubisoft Connect / Ubisoft Game Launcher
    public class Uplay : IPEContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetFileDescription(pex);
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
            name = Utilities.GetProductName(pex);
            if (!string.IsNullOrEmpty(name) && name.Contains("Ubisoft Connect"))
                return "Uplay / Ubisoft Connect";
            else if (!string.IsNullOrEmpty(name) && name.Contains("Uplay"))
                return "Uplay / Ubisoft Connect";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("UbisoftConnect.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher64.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncherInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("Uplay.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayCrashReporter.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayService.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayWebCore.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("UbisoftConnect.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher64.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncherInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("Uplay.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayCrashReporter.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayService.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UplayWebCore.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
