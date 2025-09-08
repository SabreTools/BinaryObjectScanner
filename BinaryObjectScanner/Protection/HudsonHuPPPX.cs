using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Hudson huPPPX device control library
    /// </summary>
    /// <remarks>
    /// Basically unknown copy protection scheme found in Bomberman Vol 2 (Japan).
    /// Within the installshield program (setup.inx), there's a call to a disc check (using HVCDISSR.DLL)
    /// that fails even for mounted bin/cue with intentional errors.
    /// </remarks>
    public class HudsonHuPPPX : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            var exportTable = exe.ExportTable;
            if (exportTable != null)
            {
                // Found in Bomberman Vol 2 (Japan)
                if (exportTable.ExportDirectoryTable?.Name == "HVCDISSR.DLL")
                    return "Hudson huPPPX";

                // Found in Bomberman Vol 2 (Japan)
                if (Array.Exists(exportTable.ExportNameTable?.Strings ?? [], s => s.StartsWith("HVRCD_IS_")))
                    return "Hudson huPPPX";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("HVCDISSR.DLL"), "Hudson huPPPX"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("HVCDISSR.DLL"), "Hudson huPPPX"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
