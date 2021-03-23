using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDX : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CHKCDX16.DLL", useEndsWith: true), "CD-X"),
                new PathMatchSet(new PathMatch("CHKCDX32.DLL", useEndsWith: true), "CD-X"),
                new PathMatchSet(new PathMatch("CHKCDXNT.DLL", useEndsWith: true), "CD-X"),
            };

            var matches = MatchUtil.GetAllMatches(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CHKCDX16.DLL", useEndsWith: true), "CD-X"),
                new PathMatchSet(new PathMatch("CHKCDX32.DLL", useEndsWith: true), "CD-X"),
                new PathMatchSet(new PathMatch("CHKCDXNT.DLL", useEndsWith: true), "CD-X"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
