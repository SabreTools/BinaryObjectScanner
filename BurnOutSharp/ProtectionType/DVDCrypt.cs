using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCrypt : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("DvdCrypt.pdb", useEndsWith: true), "DVD Crypt"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("DvdCrypt.pdb", useEndsWith: true), "DVD Crypt"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
