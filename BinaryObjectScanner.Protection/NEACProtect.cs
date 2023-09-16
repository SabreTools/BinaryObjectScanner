using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// NEAC Protect is an anti-cheat currently only known to be used in the game "NARAKA: BLADEPOINT".
    /// It requires HVCI to be disabled before play.
    /// As the game's publisher is NetEase Games, the name of the protection may be short for "NetEase AntiCheat".
    /// This is further supported by the fact that the "NeacSafe.sys" driver is signed by NetEase.
    /// Another program by NetEase, MuMu Player, seems to have also included a "NetEase AC" at one point(https://www.pcgamingwiki.com/wiki/MuMu_Player).
    /// There is also a separate DRM service provided by NetEase called Yidun (http://dun.163.com + http://dun.163.com/locale/en), though this seems to be unrelated.
    /// 
    /// Additional resources:
    /// https://www.pcgamingwiki.com/wiki/Naraka:_Bladepoint
    /// https://github.com/SteamDatabase/FileDetectionRuleSets/pull/235
    /// https://www.protondb.com/app/1203220
    /// </summary>
    public class NEACProtect : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Most of the relevant executables are highly obfuscated, making executable detection mostly impractical.
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .neac0 and .neac1 sections, if they exist.
            // Found in "NeacSafe64.sys" and "NeacSafe.sys".
            bool neac0Section = pex.ContainsSection(".neac0", exact: true);
            bool neac1Section = pex.ContainsSection(".neac1", exact: true);

            // Check if either .neac section is present.
            if (neac0Section || neac1Section)
                return "NEAC Protect";

            string name = pex.ProductName;

            // Found in "NeacSafe64.sys" and "NeacSafe.sys".
            // TODO: Fix Product Name not being properly grabbed from the file.
            if (!string.IsNullOrEmpty(name) && name.Contains("neacsafe"))
                return "NEAC Protect";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found installed in the main game folder.
                new PathMatchSet(new PathMatch("NeacClient.exe", useEndsWith: true), "NEAC Protect"),
                new PathMatchSet(new PathMatch("NeacInterface.dll", useEndsWith: true), "NEAC Protect"),
                new PathMatchSet(new PathMatch("NeacSafe64.sys", useEndsWith: true), "NEAC Protect"),

                // Found installed in "System32\drivers".
                new PathMatchSet(new PathMatch("NeacSafe.sys", useEndsWith: true), "NEAC Protect"),

                // Known associated log files: "NeacSafe.log", "Neac.log", "NeacDll.log", "NeacLoader.log", and "NeacBak.log".
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found installed in the main game folder.
                new PathMatchSet(new PathMatch("NeacClient.exe", useEndsWith: true), "NEAC Protect"),
                new PathMatchSet(new PathMatch("NeacInterface.dll", useEndsWith: true), "NEAC Protect"),
                new PathMatchSet(new PathMatch("NeacSafe64.sys", useEndsWith: true), "NEAC Protect"),

                // Found installed in "System32\drivers".
                new PathMatchSet(new PathMatch("NeacSafe.sys", useEndsWith: true), "NEAC Protect"),

                // Known associated log files: "NeacSafe.log", "Neac.log", "NeacDll.log", "NeacLoader.log", and "NeacBak.log".
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
