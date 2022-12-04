using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// The Windows Media Data Session Toolkit was created as a form of DRM for audio CDs (and supposedly DVDs, but with no standard for multiple sessions existing for pressed DVDs, this seems unlikely), which ultimately acts a framework
    /// It seems to provide a framework for standardized storing of protected music, as well as allowing the disc publisher to specify how many copies they wish users to be able to create using Windows Media Player.
    /// Known to be used along with MediaMax CD-3 and XCP2, possibly others.
    /// Reference: https://news.microsoft.com/2003/01/20/microsoft-releases-new-windows-media-data-session-toolkit-enabling-second-session-creation/
    /// </summary>
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
            if (name?.StartsWith("Windows Media Data Session Licensing Engine", StringComparison.OrdinalIgnoreCase) == true)
                return $"Windows Media Data Session DRM";

            // Get the .rdata section, if it exists
            if (pex.ContainsSection(".rdata"))
            {
                var matchers = new List<ContentMatchSet>
                {
                    // You cannot generate a licence to play the protected Windows Media files without an original disc.
                    // Found in "autorun.exe" ("Touch" by Amerie).
                    new ContentMatchSet(new byte?[] {
                        0x59, 0x6F, 0x75, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F, 0x74, 0x20, 0x67,
                        0x65, 0x6E, 0x65, 0x72, 0x61, 0x74, 0x65, 0x20, 0x61, 0x20, 0x6C, 0x69,
                        0x63, 0x65, 0x6E, 0x63, 0x65, 0x20, 0x74, 0x6F, 0x20, 0x70, 0x6C, 0x61,
                        0x79, 0x20, 0x74, 0x68, 0x65, 0x20, 0x70, 0x72, 0x6F, 0x74, 0x65, 0x63,
                        0x74, 0x65, 0x64, 0x20, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x20,
                        0x4D, 0x65, 0x64, 0x69, 0x61, 0x20, 0x66, 0x69, 0x6C, 0x65, 0x73, 0x20,
                        0x77, 0x69, 0x74, 0x68, 0x6F, 0x75, 0x74, 0x20, 0x61, 0x6E, 0x20, 0x6F,
                        0x72, 0x69, 0x67, 0x69, 0x6E, 0x61, 0x6C, 0x20, 0x64, 0x69, 0x73, 0x63,
                        0x2E
                    }, "Windows Media Data Session DRM"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.GetFirstSectionData(".rdata"), matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6) and "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8), "Touch" by Amerie, likely among others.
                new PathMatchSet(new List<PathMatch>
                {
                    // These files always appear to be present together.
                    new PathMatch("WMDS.dll", useEndsWith: true),
                    new PathMatch("WMDS.ini", useEndsWith: true),
                }, "Windows Media Data Session DRM"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6) and "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8), "Touch" by Amerie, likely among others.
                new PathMatchSet(new PathMatch("WMDS.dll", useEndsWith: true), "Windows Media Data Session DRM"),
                new PathMatchSet(new PathMatch("WMDS.ini", useEndsWith: true), "Windows Media Data Session DRM"),

                // Found on "Touch" by Amerie, along with "autorun.exe".
                new PathMatchSet(new PathMatch("WMDST.DAT", useEndsWith: true), "Windows Media Data Session DRM"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
