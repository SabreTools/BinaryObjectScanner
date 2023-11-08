using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class SmartE : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the last section strings, if they exist
            var strs = pex.GetSectionStrings(sections.Length - 1);
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("BITARTS")))
                    return "SmartE";
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new FilePathMatch("00001.TMP"),
                    new FilePathMatch("00002.TMP")
                 }, "SmartE"),
            };

            return MatchUtil.GetAllMatches(files ?? System.Array.Empty<string>(), matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new FilePathMatch("00001.TMP"), "SmartE"),
                new PathMatchSet(new FilePathMatch("00002.TMP"), "SmartE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
