using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Cenega ProtectDVD is a protection seemingly created by the publisher Cenega for use with their games.
    /// Games using this protection aren't able to be run from an ISO file, and presumably use DPM as a protection feature.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/Cenega_ProtectDVD/Cenega_ProtectDVD.md"/>
    /// </summary>
    public class CengaProtectDVD : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the export directory table
            if (pex.Model.ExportTable?.ExportDirectoryTable != null)
            {
                // Found in "cenega.dll" in IA item "speed-pack".
                bool match = pex.Model.ExportTable.ExportDirectoryTable.Name?.Equals("ProtectDVD.dll", StringComparison.OrdinalIgnoreCase) == true;
                if (match)
                    return "Cenega ProtectDVD";
            }

            // Get the .cenega section, if it exists. Seems to be found in the protected game executable ("game.exe" in Redump entry 31422 and "Classic Car Racing.exe" in IA item "speed-pack").
            bool cenegaSection = pex.ContainsSection(".cenega", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            // Get the .cenega0 through .cenega2 sections, if they exists. Found in "cenega.dll" in Redump entry 31422 and IA item "speed-pack".
            cenegaSection = pex.ContainsSection(".cenega0", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            cenegaSection = pex.ContainsSection(".cenega1", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            cenegaSection = pex.ContainsSection(".cenega2", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

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
                // Seems likely to be present in most, if not all discs protected with Cenega ProtectDVD, but unable to confirm due to only having a small sample size.
                // Found in Redump entry 31422 and IA item "speed-pack".
                new PathMatchSet(new PathMatch("cenega.dll", useEndsWith: true), "Cenega ProtectDVD"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Seems likely to be present in most, if not all discs protected with Cenega ProtectDVD, but unable to confirm due to only having a small sample size.
                // Found in Redump entry 31422 and IA item "speed-pack".
                new PathMatchSet(new PathMatch("cenega.dll", useEndsWith: true), "Cenega ProtectDVD"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
