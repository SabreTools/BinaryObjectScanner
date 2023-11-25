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
    /// The Windows Media Data Session Toolkit was created as a form of DRM for audio CDs (and supposedly DVDs, but with no standard for multiple sessions existing for pressed DVDs, this seems unlikely), which ultimately acts a framework
    /// It seems to provide a framework for standardized storing of protected music, as well as allowing the disc publisher to specify how many copies they wish users to be able to create using Windows Media Player.
    /// Known to be used along with MediaMax CD-3 and XCP2, possibly others.
    /// Reference: https://news.microsoft.com/2003/01/20/microsoft-releases-new-windows-media-data-session-toolkit-enabling-second-session-creation/
    /// </summary>
    public class WMDS : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
            var name = pex.FileDescription;
            if (name?.StartsWith("Windows Media Data Session Licensing Engine", StringComparison.OrdinalIgnoreCase) == true)
                return "Windows Media Data Session DRM";

            // Found in "autorun.exe" ("Touch" by Amerie).
            var resource = pex.FindDialogBoxByItemTitle("If you attempt to play this content on a computer without a license, you will first have to acquire a license before it will play.");
            if (resource.Any())
                return "Windows Media Data Session DRM";

            // Found in "autorun.exe" ("Touch" by Amerie).
            resource = pex.FindDialogBoxByItemTitle("You cannot generate a licence to play the protected Windows Media files without an original disc.");
            if (resource.Any())
                return "Windows Media Data Session DRM";

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
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6) and "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8), "Touch" by Amerie, likely among others.
                new(new List<PathMatch>
                {
                    // These files always appear to be present together.
                    new FilePathMatch("WMDS.dll"),
                    new FilePathMatch("WMDS.ini"),
                }, "Windows Media Data Session DRM"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6) and "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8), "Touch" by Amerie, likely among others.
                new(new FilePathMatch("WMDS.dll"), "Windows Media Data Session DRM"),
                new(new FilePathMatch("WMDS.ini"), "Windows Media Data Session DRM"),

                // Found on "Touch" by Amerie, along with "autorun.exe".
                new(new FilePathMatch("WMDST.DAT"), "Windows Media Data Session DRM"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
