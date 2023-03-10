using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;

namespace BinaryObjectScanner.Protection
{
    public class CDX : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
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
        public string CheckFilePath(string path)
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
