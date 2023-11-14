using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// nProtect (AKA INCA Internet) (https://nprotect.com/) is a Korean software company that produces several DRM products.
    /// 
    /// nProtect GameGuard (https://nprotect.com/kr/b2b/prod_gg.html) is anti-cheat software used in a fair amount of online games.
    /// Partial list of games that use GameGuard: https://en.wikipedia.org/wiki/NProtect_GameGuard.
    /// 
    /// nProtect KeyCrypt is an anti-keylogging product that seemingly has other DRM functions as well, such as shutting down processes it deems unnecessary (https://en.wikipedia.org/wiki/INCA_Internet#nProtect_Netizen,_nProtect_Personal,_nProtect_Keycrypt)
    /// TODO: Verify the exact functions of KeyCrypt.
    /// 
    /// Official sites for KeyCrypt (it is unknown what the difference between them are, as both are still online and active at the same time):
    /// https://nprotect.com/kr/b2b/prod_kcv.html
    /// https://nprotect.com/kr/b2b/prod_kcv65.html
    /// 
    /// Official documents about KeyCrypt:
    /// https://nprotect.com/nprotect_pdf/nProtect_KeyCryptV.pdf
    /// https://nprotect.com/nprotect_pdf/nProtect_KeyCrypt.pdf
    /// </summary>
    public class nProtect : IPathCheck, IPortableExecutableCheck
    {
        // TODO: Add LE checks for "npkcrypt.vxd" in Redump entry 90526.
        // TODO: Add text check for the string mentioned in https://github.com/mnadareski/BinaryObjectScanner/issues/154.

        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Investigate if there are any viable checks for the game EXE itself.

            var name = pex.FileDescription;

            // Found in "GameGuard.des" in Redump entry 90526 and 99598.
            if (name?.Contains("nProtect GameGuard Launcher") == true)
                return $"nProtect GameGuard ({pex.GetInternalVersion()})";

            // Found in "npkcrypt.dll" in Redump entry 90526.
            if (name?.Contains("nProtect KeyCrypt Driver Support Dll") == true)
                return $"nProtect KeyCrypt ({pex.GetInternalVersion()})";

            // Found in "npkcrypt.sys" and "npkcusb.sys" in Redump entry 90526.
            if (name?.Contains("nProtect KeyCrypt Driver") == true)
                return $"nProtect KeyCrypt ({pex.GetInternalVersion()})";

            // Found in "npkpdb.dll" in Redump entry 90526.
            if (name?.Contains("nProtect KeyCrypt Program Database DLL") == true)
                return $"nProtect KeyCrypt ({pex.GetInternalVersion()})";

            name = pex.ProductName;

            // Found in "GameGuard.des" in Redump entry 90526 and 99598.
            if (name?.Contains("nProtect GameGuard Launcher") == true)
                return $"nProtect GameGuard ({pex.GetInternalVersion()})";

            // Found in "npkcrypt.dll" in Redump entry 90526.
            if (name?.Contains("nProtect KeyCrypt Driver Support Dll") == true)
                return $"nProtect KeyCrypt ({pex.GetInternalVersion()})";

            // Found in "npkcrypt.sys" and "npkcusb.sys" in Redump entry 90526.
            if (name?.Contains("nProtect KeyCrypt Driver") == true)
                return $"nProtect KeyCrypt ({pex.GetInternalVersion()})";

            // Found in "npkpdb.dll" in Redump entry 90526.
            if (name?.Contains("nProtect KeyCrypt Program Database DLL") == true)
                return $"nProtect KeyCrypt ({pex.GetInternalVersion()})";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in "MSSetup.exe" in Redump entry 90526 and "mhfSetup_f40_1000.exe" and Redump entry 99598.
                new PathMatchSet(new PathMatch("GameGuard.des", useEndsWith: true), "nProtect GameGuard"),

                // Found in "MSSetup.exe" in Redump entry 90526.
                new PathMatchSet(new PathMatch("npkcrypt.dll", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkcrypt.sys", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkcrypt.vxd", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkcusb.sys", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkpdb.dll", useEndsWith: true), "nProtect KeyCrypt"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in "MSSetup.exe" in Redump entry 90526.
                new PathMatchSet(new PathMatch("GameGuard.des", useEndsWith: true), "nProtect GameGuard"),
                new PathMatchSet(new PathMatch("npkcrypt.dll", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkcrypt.sys", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkcrypt.vxd", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkcusb.sys", useEndsWith: true), "nProtect KeyCrypt"),
                new PathMatchSet(new PathMatch("npkpdb.dll", useEndsWith: true), "nProtect KeyCrypt"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
