using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Kalypso Launcher is a launcher used by Kalypso Media games. It is responsible for game activation via product keys.
    /// Several Kalypso developed games are available on Steam, but the launcher isn't able to be (officially) disabled: https://www.reddit.com/r/RailwayEmpire/comments/nktojh/skip_kalypso_launcher_from_steam/
    /// Assumed to be present on all Kalypso Media games on PC since at least 2011 (as it is present in Redump entry 95617), though this needs to be confirmed.
    /// The internal name of the Kalypso Launcher may be "Styx", as it is present as the File Description and Product Name in various versions of "KalypsoLauncher.dll".
    /// Kalypso FAQ, which includes information about Kalypso Launcher: https://www.kalypsomedia.com/us/frequently-asked-questions
    /// It was introduced in or before January 2011, based on this forum post introducing it: https://web.archive.org/web/20120524150700/http://forum.kalypsomedia.com/showthread.php?tid=7909
    /// 
    /// Known versions:
    /// 1.2.0.12: Found in Redump entry 95617.
    /// 2.0.4.2: Newest version as of 3/10/2024, downloaded from updating the installed game from Redump entry 95617.
    /// </summary>
    public class KalypsoLauncher : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Investigate if there are any viable checks for the game EXE itself.
            // "Styx" is found as the File Description and Product Name in "KalypsoLauncher.dll", but checks aren't included due to the risk of false positives.

            var name = pex.InternalName;

            // Found in "KalypsoLauncher.dll" in Redump entry 95617.
            if (name.OptionalContains("KalypsoLauncher.dll"))
                return $"Kalypso Launcher {pex.GetInternalVersion()}";

            name = pex.OriginalFilename;

            // Found in "KalypsoLauncher.dll" in Redump entry 95617.
            if (name.OptionalContains("KalypsoLauncher.dll"))
                return $"Kalypso Launcher {pex.GetInternalVersion()}";

            // Get the .text section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                // Found in "TFT.exe" in Redump entry 95617.
                if (strs.Exists(s => s.Contains("@KalypsoLauncherXml")))
                    return "Kalypso Launcher";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 95617.
                new(new FilePathMatch("KalypsoLauncher.dll"), "Kalypso Launcher"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 95617.
                new(new FilePathMatch("KalypsoLauncher.dll"), "Kalypso Launcher"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
