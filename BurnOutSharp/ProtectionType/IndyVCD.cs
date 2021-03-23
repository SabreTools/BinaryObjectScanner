using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class IndyVCD : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("INDYVCD.AX", useEndsWith: true), "IndyVCD"),
                new PathMatchSet(new PathMatch("INDYMP3.idt", useEndsWith: true), "IndyVCD"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("INDYVCD.AX", useEndsWith: true), "IndyVCD"),
                new PathMatchSet(new PathMatch("INDYMP3.idt", useEndsWith: true), "IndyVCD"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
