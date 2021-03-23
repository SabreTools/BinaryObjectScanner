using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CopyKiller : IContentCheck, IPathCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // Tom Commander
            new ContentMatchSet(new byte?[]
            {
                0x54, 0x6F, 0x6D, 0x20, 0x43, 0x6F, 0x6D, 0x6D,
                0x61, 0x6E, 0x64, 0x65, 0x72
            }, "CopyKiller"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
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
