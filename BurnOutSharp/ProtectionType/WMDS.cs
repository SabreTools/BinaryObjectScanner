using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // The Windows Media Data Session Toolkit was created as a form of DRM for audio CDs (and supposedly DVDs, but with no standard for multiple sessions existing for pressed DVDs, this seems unlikely), which ultimately acts a framework
    // It seems to provide a framework for standardized storing of protected music, as well as allowing the disc publisher to specify how many copies they wish users to be able to create using Windows Media Player.
    // Known to be used along with MediaMax CD-3, possibly others.
    // Reference: https://news.microsoft.com/2003/01/20/microsoft-releases-new-windows-media-data-session-toolkit-enabling-second-session-creation/
    public class WMDS : IPathCheck, IPortableExecutableCheck
    {
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
            string name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Windows Media Data Session Licensing Engine", StringComparison.OrdinalIgnoreCase))
                return $"Windows Media Data Session DRM";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
                new PathMatchSet(new List<PathMatch>
                {
                    // TODO: Verify if these are OR or AND
                    new PathMatch("LicGen.exe", useEndsWith: true),
                    new PathMatch("WMDS.dll", useEndsWith: true),
                    new PathMatch("WMDS.ini", useEndsWith: true),
                }, "Windows Media Data Session DRM"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6). "LicGen.exe" excluded from this check due to the file name being known to be in use by other software, more specifically program cracks.
                new PathMatchSet(new PathMatch("WMDS.dll", useEndsWith: true), "Windows Media Data Session DRM"),
                new PathMatchSet(new PathMatch("WMDS.ini", useEndsWith: true), "Windows Media Data Session DRM"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
