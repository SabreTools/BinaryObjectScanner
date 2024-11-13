using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Roxxe was a Czech DRM. It appears to have been a simple disc check that also relied on unusual disc manufacturing and dummy files to attempt to prevent copying.
    /// 
    /// DRML: https://github.com/TheRogueArchivist/DRML/blob/main/entries/Roxxe/Roxxe.md
    /// </summary>
    public class Roxxe : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the code/CODE section strings, if they exist
            var strs = pex.GetFirstSectionStrings("code") ?? pex.GetFirstSectionStrings("CODE");
            if (strs != null)
            {
                // Found in "Owar.exe" in IA item "game4u-22-cd".
                if (strs.Exists(s => s.Contains("TRCHANGER.INI")))
                    return "Roxxe";
            }

            // Get the .rsrc section strings, if they exist
            // TODO: Check for these strings specifically within the application-defined resource that they're found in, not just the generic resource section.
            strs = pex.GetFirstSectionStrings(".rsrc");
            if (strs != null)
            {
                // Found in "Owar.exe" in IA items "game4u-22-cd" and "original-war".
                // These checks are less reliable, as they are still found in a version of the game that appears to have patched out Roxxe (the version present in IA item "original-war").
                if (strs.Exists(s => s.Contains("PRRT01")))
                    return "Roxxe (Possibly remnants)";
                
                if (strs.Exists(s => s.Contains("CommonPRRT")))
                    return "Roxxe (Possibly remnants)";

                // Currently overmatches, will likely be a viable check when better Delphi executable parsing is available.
                // if (strs.Exists(s => s.Contains("roxe")))
                //     return "Roxxe (Possibly remnants)";
            }

            // If any dialog boxes match
            // Found in "Data6.OWP" in IA item "game4u-22-cd".
            if (pex.FindDialogBoxByItemTitle("SharpTiny Version 1.0").Any())
                return "Roxxe";
            // Found in "Data8.OWP" in IA item "game4u-22-cd".
            else if (pex.FindDialogBoxByItemTitle("T32xWin Version 1.0").Any())
                return "Roxxe";

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Files such as "TRCHANGER.INI" may be present, but haven't been found yet.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Files such as "TRCHANGER.INI" may be present, but haven't been found yet.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
