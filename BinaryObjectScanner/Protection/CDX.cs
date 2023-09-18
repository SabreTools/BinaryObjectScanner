using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    public class CDX : IPathCheck
    {
        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CHKCDX16.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("CHKCDX32.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("CHKCDXNT.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files ?? System.Array.Empty<string>(), matchers, any: true);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CHKCDX16.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("CHKCDX32.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("CHKCDXNT.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
