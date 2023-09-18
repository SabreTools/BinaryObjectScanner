using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class WTMCDProtect : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name?.Contains("Copy Protection Viewer") == true)
                return "WTM Protection Viewer";

            name = pex.LegalTrademarks;
            if (name?.Contains("WTM Copy Protection") == true)
                return "WTM Protection Viewer";

            name = pex.ProductName;
            if (name?.Contains("WTM Copy Protection Viewer") == true)
                return "WTM Protection Viewer";

            // Get the code/CODE section strings, if they exist
            var strs = pex.GetFirstSectionStrings("code") ?? pex.GetFirstSectionStrings("CODE");
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
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new FilePathMatch("wtmfiles.dat"),
                    new FilePathMatch("Viewer.exe"),
                }, "WTM Protection Viewer"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            // TODO: Add ImageX.imp as a wildcard, if possible
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new FilePathMatch("Image.imp"), "WTM CD Protect"),
                new PathMatchSet(new FilePathMatch("Image1.imp"), "WTM CD Protect"),
                new PathMatchSet(new FilePathMatch("imp.dat"), "WTM CD Protect"),
                new PathMatchSet(new FilePathMatch("wtmfiles.dat"), "WTM Protection Viewer"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
