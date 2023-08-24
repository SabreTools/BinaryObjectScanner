﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // Got renamed to Ubisoft Connect / Ubisoft Game Launcher
    public class Uplay : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
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
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("UbisoftConnect.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncher64.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch("UbisoftGameLauncherInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}Uplay.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayCrashReporter.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayService.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayWebCore.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
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
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}Uplay.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayCrashReporter.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayInstaller.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayService.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}UplayWebCore.exe", useEndsWith: true), "Uplay / Ubisoft Connect"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
