#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
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
                new PathMatchSet(new PathMatch("CHKCDX16.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("CHKCDX32.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("CHKCDXNT.DLL", useEndsWith: true), "CD-X (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
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
