using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class GFWL : IContentCheck, IPathCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // xlive.dll
            new ContentMatchSet(new byte?[] { 0x78, 0x6C, 0x69, 0x76, 0x65, 0x2E, 0x64, 0x6C, 0x6C }, "Games for Windows - Live"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows - Live"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows - Live"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
