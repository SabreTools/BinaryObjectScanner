using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class WTMCDProtect : IContentCheck, IPathCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // WTM76545
            new ContentMatchSet(new byte?[] { 0x57, 0x54, 0x4D, 0x37, 0x36, 0x35, 0x34, 0x35 }, "WTM CD Protect"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch(".IMP", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("imp.dat", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("wtmfiles.dat", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("Viewer.exe", useEndsWith: true), "WTM CD Protect"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch(".IMP", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("\\imp.dat", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("\\wtmfiles.dat", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("\\Viewer.exe", useEndsWith: true), "WTM CD Protect"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
