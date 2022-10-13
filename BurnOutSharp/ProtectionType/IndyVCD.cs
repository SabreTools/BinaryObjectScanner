using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class IndyVCD : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("INDYVCD.AX", useEndsWith: true), "IndyVCD (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("INDYMP3.idt", useEndsWith: true), "IndyVCD (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("INDYVCD.AX", useEndsWith: true), "IndyVCD (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch("INDYMP3.idt", useEndsWith: true), "IndyVCD (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
