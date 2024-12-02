using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Document and implement support for this better, including version detection.
    public class WTMCDProtect : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name.OptionalContains("Copy Protection Viewer"))
                return "WTM Protection Viewer";

            name = pex.LegalTrademarks;
            if (name.OptionalContains("WTM Copy Protection"))
                return "WTM Protection Viewer";

            name = pex.ProductName;
            if (name.OptionalContains("WTM Copy Protection Viewer"))
                return "WTM Protection Viewer";

            // Get the code/CODE section strings, if they exist
            var strs = pex.GetFirstSectionStrings("code") ?? pex.GetFirstSectionStrings("CODE");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("wtmdum.imp")))
                    return "WTM CD Protect";
            }

            // Get the .text section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("WTM DIGITAL Photo Protect")))
                    return "WTM Protection Viewer";
                else if (strs.Exists(s => s.Contains("WTM Copy Protection Viewer")))
                    return "WTM Protection Viewer";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(
                [
                    new FilePathMatch("wtmfiles.dat"),
                    new FilePathMatch("Viewer.exe"),
                ], "WTM Protection Viewer"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // TODO: Add ImageX.imp as a wildcard, if possible
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("Image.imp"), "WTM CD Protect"),
                new(new FilePathMatch("Image1.imp"), "WTM CD Protect"),
                // Disabled due to false positives.
                // new(new FilePathMatch("imp.dat"), "WTM CD Protect"),
                new(new FilePathMatch("wtmfiles.dat"), "WTM Protection Viewer"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
