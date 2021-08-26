using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Bitpool is a copy protection that seems to resemble SafeDisc in several ways, found mostly in German releases.
    /// Makes use of bad sectors, and contains what appears to be an encrypted game executable always called "CD.IDX" (similar to SafeDisc ICD files), which seems to be present in every disc protected with it.
    /// The "CD.IDX" appears to purposefully contain bad sectors in most, if not all, cases.
    /// A "bitpool.rsc" file is present in some, but not all Bitpool protected games. The purpose of it is so far unclear.
    /// </summary>
    public class Bitpool : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // Sometimes found in CD.IDX
                // BITPOOL.RSC
                new ContentMatchSet(new byte?[]
                {
                    0x42, 0x49, 0x54, 0x50, 0x4F, 0x4F, 0x4C, 0x2E,
                    0x52, 0x53, 0x43
                }, "Bitpool"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("bitpool.rsc", useEndsWith: true), "Bitpool"),
                new PathMatchSet(new PathMatch("CD.IDX", useEndsWith: true), "Bitpool"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("bitpool.rsc", useEndsWith: true), "Bitpool"),
                new PathMatchSet(new PathMatch("CD.IDX", useEndsWith: true), "Bitpool"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
