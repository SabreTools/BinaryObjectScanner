#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    // Nothing is currently known about this DRM. One program may possibly have included it, as it has been listed as including these files in the installion directory (https://www.advanceduninstaller.com/Visit-Gallery-2-90896afd3151ed9660dddc23b892863f-application.htm).
    // Unfortunately, this program and developer are so obscure, I'm not able to find any relevant further information on them whatsoever.
    // The only source of valuable information currently known is a forum post about a user attempting to crack this DRM (https://forum.p30world.com/showthread.php?t=413264).
    public class CDX : IPathCheck
    {
        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CHKCDX16.DLL"), "CD-X (Unconfirmed - Please report to us on Github)"),
                new(new FilePathMatch("CHKCDX32.DLL"), "CD-X (Unconfirmed - Please report to us on Github)"),
                new(new FilePathMatch("CHKCDXNT.DLL"), "CD-X (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CHKCDX16.DLL"), "CD-X (Unconfirmed - Please report to us on Github)"),
                new(new FilePathMatch("CHKCDX32.DLL"), "CD-X (Unconfirmed - Please report to us on Github)"),
                new(new FilePathMatch("CHKCDXNT.DLL"), "CD-X (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
