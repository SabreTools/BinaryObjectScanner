using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Roxxe was a Czech DRM. It appears to have been a simple disc check that also relied on unusual disc manufacturing and dummy files to attempt to prevent copying.
    /// 
    /// DRML: https://github.com/TheRogueArchivist/DRML/blob/main/entries/Roxxe/Roxxe.md
    /// </summary>
    public class Roxxe : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
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
                if (strs.Any(s => s.Contains("TRCHANGER.INI")))
                    return "Roxxe";
            }

            // Get the .rsrc section strings, if they exist
            // TODO: Check for these strings specifically within the application-defined resource that they're found in, not just the generic resource section.
            strs = pex.GetFirstSectionStrings(".rsrc");
            if (strs != null)
            {
                // Found in "Owar.exe" in IA items "game4u-22-cd" and "original-war".
                // These checks are less reliable, as they are still found in a version of the game that appears to have patched out Roxxe (the version present in IA item "original-war").
                if (strs.Any(s => s.Contains("PRRT01")))
                    return "Roxxe (Possibly remnants)";
                
                if (strs.Any(s => s.Contains("CommonPRRT")))
                    return "Roxxe (Possibly remnants)";

                if (strs.Any(s => s.Contains("roxe")))
                    return "Roxxe (Possibly remnants)";
            }

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
