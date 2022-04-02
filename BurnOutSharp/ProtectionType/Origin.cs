using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Origin : IPEContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Origin", StringComparison.OrdinalIgnoreCase))
                return "Origin";

            name = pex.ProductName;
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Origin", StringComparison.OrdinalIgnoreCase))
                return "Origin";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("OriginSetup.exe", useEndsWith: true), "Origin"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("OriginSetup.exe", useEndsWith: true), "Origin"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
