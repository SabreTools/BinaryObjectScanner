using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDCops : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // CD-Cops,  ver. 
                new ContentMatchSet(new byte?[]
                {
                    0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                    0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "CD-Cops"),

                // .grand + (char)0x00
                new ContentMatchSet(new byte?[] { 0x2E, 0x67, 0x72, 0x61, 0x6E, 0x64, 0x00 }, "CD-Cops"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Original had "CDCOPS.DLL" required and all the rest in a combined OR
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            char[] version = new ArraySegment<byte>(fileContent, positions[0] + 15, 4).Select(b => (char)b).ToArray();
            if (version[0] == 0x00)
                return string.Empty;

            return new string(version);
        }
    }
}
