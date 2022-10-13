using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDProtector : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("_cdp16.dat", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("_cdp16.dll", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("_cdp32.dat", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("_cdp32.dll", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("_cdp16.dat", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("_cdp16.dll", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("_cdp32.dat", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("_cdp32.dll", useEndsWith: true), "CD-Protector (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
