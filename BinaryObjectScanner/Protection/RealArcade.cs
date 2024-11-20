using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// RealArcade was a game platform that allowed users to play timed demos of games, and then prompted them to purchase the game in order to play the game without a limit.
    /// Although the servers are long dead, there is a community project actively being developed to allow users to properly download and play these games.
    /// Links:
    /// https://github.com/lightbulbatelier/RealArcade-DGA
    /// https://archive.org/details/realrcade-games-preservation-project
    /// </summary>
    public class RealArcade : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data");
            if (strs != null)
            {
                // Found in "rebound.exe" in the installation directory for "Rebound" in IA item "Nova_RealArcadeCD_USA".
                if (strs.Exists(s => s.Contains("RngInterstitialDLL")))
                    return "RealArcade";
            }

            // Found in "RngInterstitial.dll" in the RealArcade installation directory in IA item "Nova_RealArcadeCD_USA".
            var name = pex.FileDescription;
            if (name?.Contains("RngInterstitial") == true)
                return "RealArcade";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // ".rgs" and ".mez" files are also associated with RealArcade.
                new(new FilePathMatch("RngInterstitial.dll"), "RealArcade"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // ".rgs" and ".mez" files are also associated with RealArcade.
                new(new FilePathMatch("RngInterstitial.dll"), "RealArcade"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
