using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class GFWL : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // xlive.dll
                new ContentMatchSet(new byte?[] { 0x78, 0x6C, 0x69, 0x76, 0x65, 0x2E, 0x64, 0x6C, 0x6C }, "Games for Windows LIVE"),

                // G + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + s + (char)0x00 +   + (char)0x00 + f + (char)0x00 + o + (char)0x00 + r + (char)0x00 +   + (char)0x00 + W + (char)0x00 + i + (char)0x00 + n + (char)0x00 + d + (char)0x00 + o + (char)0x00 + w + (char)0x00 + s + (char)0x00 +   + (char)0x00 + - + (char)0x00 +   + (char)0x00 + L + (char)0x00 + I + (char)0x00 + V + (char)0x00 + E + (char)0x00 +   + (char)0x00 + Z + (char)0x00 + e + (char)0x00 + r + (char)0x00 + o + (char)0x00 +   + (char)0x00 + D + (char)0x00 + a + (char)0x00 + y + (char)0x00 +   + (char)0x00 + P + (char)0x00 + i + (char)0x00 + r + (char)0x00 + a + (char)0x00 + c + (char)0x00 + y + (char)0x00 +   + (char)0x00 + P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x47, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                    0x73, 0x00, 0x20, 0x00, 0x66, 0x00, 0x6F, 0x00,
                    0x72, 0x00, 0x20, 0x00, 0x57, 0x00, 0x69, 0x00,
                    0x6E, 0x00, 0x64, 0x00, 0x6F, 0x00, 0x77, 0x00,
                    0x73, 0x00, 0x20, 0x00, 0x2D, 0x00, 0x20, 0x00,
                    0x4C, 0x00, 0x49, 0x00, 0x56, 0x00, 0x45, 0x00,
                    0x20, 0x00, 0x5A, 0x00, 0x65, 0x00, 0x72, 0x00,
                    0x6F, 0x00, 0x20, 0x00, 0x44, 0x00, 0x61, 0x00,
                    0x79, 0x00, 0x20, 0x00, 0x50, 0x00, 0x69, 0x00,
                    0x72, 0x00, 0x61, 0x00, 0x63, 0x00, 0x79, 0x00,
                    0x20, 0x00, 0x50, 0x00, 0x72, 0x00, 0x6F, 0x00,
                    0x74, 0x00, 0x65, 0x00, 0x63, 0x00, 0x74, 0x00,
                    0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00,
                }, Utilities.GetFileVersion, "Games for Windows LIVE - Zero Day Piracy Protection Module"),

                // G + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + s + (char)0x00 +   + (char)0x00 + f + (char)0x00 + o + (char)0x00 + r + (char)0x00 +   + (char)0x00 + W + (char)0x00 + i + (char)0x00 + n + (char)0x00 + d + (char)0x00 + o + (char)0x00 + w + (char)0x00 + s + (char)0x00 +   + (char)0x00 + - + (char)0x00 +   + (char)0x00 + L + (char)0x00 + I + (char)0x00 + V + (char)0x00 + E + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x47, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                    0x73, 0x00, 0x20, 0x00, 0x66, 0x00, 0x6F, 0x00,
                    0x72, 0x00, 0x20, 0x00, 0x57, 0x00, 0x69, 0x00,
                    0x6E, 0x00, 0x64, 0x00, 0x6F, 0x00, 0x77, 0x00,
                    0x73, 0x00, 0x20, 0x00, 0x2D, 0x00, 0x20, 0x00,
                    0x4C, 0x00, 0x49, 0x00, 0x56, 0x00, 0x45, 0x00,
                }, Utilities.GetFileVersion, "Games for Windows LIVE"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = GetContentMatchSets();
            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Might be specifically GFWL/Gfwlivesetup.exe
                new PathMatchSet(new PathMatch("Gfwlivesetup.exe", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("xliveinstall.dll", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows LIVE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Might be specifically GFWL/Gfwlivesetup.exe
                new PathMatchSet(new PathMatch("Gfwlivesetup.exe", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("xliveinstall.dll", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows LIVE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
