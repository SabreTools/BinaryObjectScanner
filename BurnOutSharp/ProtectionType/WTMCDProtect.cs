using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class WTMCDProtect : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.Contains("Copy Protection Viewer") == true)
                return "WTM Protection Viewer";

            name = pex.LegalTrademarks;
            if (name?.Contains("WTM Copy Protection") == true)
                return "WTM Protection Viewer";

            name = pex.ProductName;
            if (name?.Contains("WTM Copy Protection Viewer") == true)
                return "WTM Protection Viewer";

            // Get the code/CODE section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings("code") ?? pex.GetFirstSectionStrings("CODE");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("wtmdum.imp")))
                    return "WTM CD Protect";
            }

            // Get the .text section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("WTM DIGITAL Photo Protect")))
                    return "WTM Protection Viewer";
                else if (strs.Any(s => s.Contains("WTM Copy Protection Viewer")))
                    return "WTM Protection Viewer";
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch($"{Path.DirectorySeparatorChar}wtmfiles.dat", useEndsWith: true),
                    new PathMatch($"{Path.DirectorySeparatorChar}Viewer.exe", useEndsWith: true),
                }, "WTM Protection Viewer"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Add ImageX.imp as a wildcard, if possible
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}Image.imp", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}Image1.imp", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}imp.dat", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}wtmfiles.dat", useEndsWith: true), "WTM Protection Viewer"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
