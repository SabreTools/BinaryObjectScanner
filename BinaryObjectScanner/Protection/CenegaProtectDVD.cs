using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Cenega ProtectDVD is a protection seemingly created by the publisher Cenega for use with their games.
    /// Games using this protection aren't able to be run from an ISO file, and presumably use DPM as a protection feature.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/Cenega_ProtectDVD/Cenega_ProtectDVD.md"/>
    /// </summary>
    public class CenegaProtectDVD : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the export directory table
            if (exe.Model.ExportTable?.ExportDirectoryTable != null)
            {
                // Found in "cenega.dll" in IA item "speed-pack".
                bool match = exe.Model.ExportTable.ExportDirectoryTable.Name.OptionalEquals("ProtectDVD.dll", StringComparison.OrdinalIgnoreCase);
                if (match)
                    return "Cenega ProtectDVD";
            }

            // Get the .cenega section, if it exists. Seems to be found in the protected game executable ("game.exe" in Redump entry 31422 and "Classic Car Racing.exe" in IA item "speed-pack").
            if (exe.ContainsSection(".cenega", exact: true))
                return "Cenega ProtectDVD";

            // Get the .cenega0 through .cenega2 sections, if they exists. Found in "cenega.dll" in Redump entry 31422 and IA item "speed-pack".
            if (exe.ContainsSection(".cenega0", exact: true))
                return "Cenega ProtectDVD";
            if (exe.ContainsSection(".cenega1", exact: true))
                return "Cenega ProtectDVD";
            if (exe.ContainsSection(".cenega2", exact: true))
                return "Cenega ProtectDVD";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Seems likely to be present in most, if not all discs protected with Cenega ProtectDVD, but unable to confirm due to only having a small sample size.
                // Found in Redump entry 31422 and IA item "speed-pack".
                new(new FilePathMatch("cenega.dll"), "Cenega ProtectDVD"),
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
                new(new FilePathMatch("cenega.dll"), "Cenega ProtectDVD"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
