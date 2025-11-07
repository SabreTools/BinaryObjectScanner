using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// nProtect (AKA INCA Internet) (https://nprotect.com/) is a Korean software company that produces several DRM products.
    ///
    /// nProtect GameGuard (https://nprotect.com/kr/b2b/prod_gg.html) is anti-cheat software used in a fair amount of online games.
    /// Partial list of games that use GameGuard: https://en.wikipedia.org/wiki/NProtect_GameGuard.
    /// Known versions of GameGuard:
    /// "2024.2.27.1" - Found in GameGuard.des in "Soulworker" (Steam Depot 1377581, Manifest 5092481117079359342).
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
    public class NProtect : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        // TODO: Add LE checks for "npkcrypt.vxd" in Redump entry 90526.
        // TODO: Add text check for the string mentioned in https://github.com/mnadareski/BinaryObjectScanner/issues/154.

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Investigate if there are any viable checks for the game EXE itself.

            string? name = exe.FileDescription;

            // Found in "GameGuard.des" in Redump entry 90526 and 99598, and "Soulworker" (Steam Depot 1377581, Manifest 5092481117079359342).
            if (name.OptionalContains("nProtect GameGuard Launcher"))
                return $"nProtect GameGuard ({exe.GetInternalVersion()})";

            // Found in "npkcrypt.dll" in Redump entry 90526.
            if (name.OptionalContains("nProtect KeyCrypt Driver Support Dll"))
                return $"nProtect KeyCrypt ({exe.GetInternalVersion()})";

            // Found in "npkcrypt.sys" and "npkcusb.sys" in Redump entry 90526.
            if (name.OptionalContains("nProtect KeyCrypt Driver"))
                return $"nProtect KeyCrypt ({exe.GetInternalVersion()})";

            // Found in "npkpdb.dll" in Redump entry 90526.
            if (name.OptionalContains("nProtect KeyCrypt Program Database DLL"))
                return $"nProtect KeyCrypt ({exe.GetInternalVersion()})";

            name = exe.ProductName;

            // Found in "GameGuard.des" in Redump entry 90526 and 99598.
            if (name.OptionalContains("nProtect GameGuard Launcher"))
                return $"nProtect GameGuard ({exe.GetInternalVersion()})";

            // Found in "npkcrypt.dll" in Redump entry 90526.
            if (name.OptionalContains("nProtect KeyCrypt Driver Support Dll"))
                return $"nProtect KeyCrypt ({exe.GetInternalVersion()})";

            // Found in "npkcrypt.sys" and "npkcusb.sys" in Redump entry 90526.
            if (name.OptionalContains("nProtect KeyCrypt Driver"))
                return $"nProtect KeyCrypt ({exe.GetInternalVersion()})";

            // Found in "npkpdb.dll" in Redump entry 90526.
            if (name.OptionalContains("nProtect KeyCrypt Program Database DLL"))
                return $"nProtect KeyCrypt ({exe.GetInternalVersion()})";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in "MSSetup.exe" in Redump entry 90526 and "mhfSetup_f40_1000.exe" and Redump entry 99598.
                new(new FilePathMatch("GameGuard.des"), "nProtect GameGuard"),

                // Found in "MSSetup.exe" in Redump entry 90526.
                new(new FilePathMatch("npkcrypt.dll"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkcrypt.sys"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkcrypt.vxd"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkcusb.sys"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkpdb.dll"), "nProtect KeyCrypt"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in "MSSetup.exe" in Redump entry 90526.
                new(new FilePathMatch("GameGuard.des"), "nProtect GameGuard"),
                new(new FilePathMatch("npkcrypt.dll"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkcrypt.sys"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkcrypt.vxd"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkcusb.sys"), "nProtect KeyCrypt"),
                new(new FilePathMatch("npkpdb.dll"), "nProtect KeyCrypt"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
