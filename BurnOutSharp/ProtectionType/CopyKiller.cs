using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CopyKiller : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // Tom Commander
                new ContentMatchSet(new byte?[]
                {
                    0x54, 0x6F, 0x6D, 0x20, 0x43, 0x6F, 0x6D, 0x6D,
                    0x61, 0x6E, 0x64, 0x65, 0x72
                }, "CopyKiller"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: The following checks are overly broad and should be refined
            var matchers = new List<PathMatchSet>
            {
                //new PathMatchSet(new PathMatch("Autorun.dat", useEndsWith: true), "CopyKiller"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: The following checks are overly broad and should be refined
            var matchers = new List<PathMatchSet>
            {
                //new PathMatchSet(new PathMatch("Autorun.dat", useEndsWith: true), "CopyKiller"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
