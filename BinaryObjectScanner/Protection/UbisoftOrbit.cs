using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Ubitsoft Orbit (Online DRM)
    /// </summary>
    /// TODO: Investigate the DLLs to find more markers
    public class UbisoftOrbit : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("GameOrbit.dll"), "Ubisoft Orbit"),
                new(new FilePathMatch("ubiorbitapi_r2_loader.dll"), "Ubisoft Orbit"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("GameOrbit.dll"), "Ubisoft Orbit"),
                new(new FilePathMatch("ubiorbitapi_r2_loader.dll"), "Ubisoft Orbit"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
