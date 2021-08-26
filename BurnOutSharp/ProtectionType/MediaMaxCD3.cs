using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class MediaMaxCD3 : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // Cd3Ctl
                new ContentMatchSet(new byte?[] { 0x43, 0x64, 0x33, 0x43, 0x74, 0x6C }, "MediaMax CD-3"),

                // DllInstallSbcp
                new ContentMatchSet(new byte?[]
                {
                    0x44, 0x6C, 0x6C, 0x49, 0x6E, 0x73, 0x74, 0x61,
                    0x6C, 0x6C, 0x53, 0x62, 0x63, 0x70
                }, "MediaMax CD-3"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = GetContentMatchSets();
            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("LaunchCd.exe", useEndsWith: true), "MediaMax CD-3"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("LaunchCd.exe", useEndsWith: true), "MediaMax CD-3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
