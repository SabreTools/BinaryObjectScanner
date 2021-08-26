using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class WTMCDProtect : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // This string is found in the .imp files associated with this protection.
                // WTM76545
                new ContentMatchSet(new byte?[] { 0x57, 0x54, 0x4D, 0x37, 0x36, 0x35, 0x34, 0x35 }, "WTM CD Protect"),

                // Found in the copy protected setup used by this protection.
                // wtmdum.imp
                new ContentMatchSet(new byte?[] { 0x77, 0x74, 0x6D, 0x64, 0x75, 0x6D, 0x2E, 0x69, 0x6D, 0x70 }, "WTM CD Protect"),

                // WTM DIGITAL Photo Protect
                new ContentMatchSet(new byte?[]
                {
                    0x57, 0x54, 0x4D, 0x20, 0x44, 0x49, 0x47, 0x49,
                    0x54, 0x41, 0x4C, 0x20, 0x50, 0x68, 0x6F, 0x74,
                    0x6F, 0x20, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63,
                    0x74
                }, "WTM Protection Viewer"),

                // WTM Copy Protection Viewer
                new ContentMatchSet(new byte?[]
                {
                    0x48, 0x61, 0x6E, 0x73, 0x70, 0x65, 0x74, 0x65, 0x72
                }, "WTM Protection Viewer"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("wtmfiles.dat", useEndsWith: true),
                    new PathMatch("Viewer.exe", useEndsWith: true),
                }, "WTM Protection Viewer"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Add ImageX.imp as a wildcard, if possilbe
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Image.imp", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("Image1.imp", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("imp.dat", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("wtmfiles.dat", useEndsWith: true), "WTM Protection Viewer"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
