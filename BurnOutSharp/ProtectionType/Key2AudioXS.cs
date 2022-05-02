using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Key2AudioXS : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("SDKHM (KEEP)"))
                return "key2AudioXS";
            else if (!string.IsNullOrWhiteSpace(name) && name.Contains("SDKHM (KEPT)"))
                return "key2AudioXS";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SDKHM.EXE", useEndsWith: true), "key2AudioXS"),
                new PathMatchSet(new PathMatch("SDKHM.DLL", useEndsWith: true), "key2AudioXS"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SDKHM.EXE", useEndsWith: true), "key2AudioXS"),
                new PathMatchSet(new PathMatch("SDKHM.DLL", useEndsWith: true), "key2AudioXS"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
