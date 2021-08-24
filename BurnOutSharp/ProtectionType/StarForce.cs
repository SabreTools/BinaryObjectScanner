using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class StarForce : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                new ContentMatchSet(new List<byte?[]>
                {
                    // ( + (char)0x00 + c + (char)0x00 + ) + (char)0x00 +   + (char)0x00 + P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + T + (char)0x00 + e + (char)0x00 + c + (char)0x00 + h + (char)0x00 + n + (char)0x00 + o + (char)0x00 + l + (char)0x00 + o + (char)0x00 + g + (char)0x00 + y + (char)0x00
                    new byte?[]
                    {
                        0x28, 0x00, 0x63, 0x00, 0x29, 0x00, 0x20, 0x00,
                        0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x74, 0x00,
                        0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00,
                        0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x54, 0x00,
                        0x65, 0x00, 0x63, 0x00, 0x68, 0x00, 0x6E, 0x00,
                        0x6F, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x67, 0x00,
                        0x79, 0x00
                    },

                    // // PSA_GetDiscLabel
                    // new byte?[]
                    // {
                    //     0x50, 0x53, 0x41, 0x5F, 0x47, 0x65, 0x74, 0x44,
                    //     0x69, 0x73, 0x63, 0x4C, 0x61, 0x62, 0x65, 0x6C
                    // },

                    // (c) Protection Technology
                    // new byte?[]
                    // {
                    //     0x28, 0x63, 0x29, 0x20, 0x50, 0x72, 0x6F, 0x74,
                    //     0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54,
                    //     0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67,
                    //     0x79
                    // },

                    // TradeName
                    new byte?[] { 0x54, 0x72, 0x61, 0x64, 0x65, 0x4E, 0x61, 0x6D, 0x65 },
                }, GetVersion, "StarForce"),

                // ( + (char)0x00 + c + (char)0x00 + ) + (char)0x00 +   + (char)0x00 + P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + T + (char)0x00 + e + (char)0x00 + c + (char)0x00 + h + (char)0x00 + n + (char)0x00 + o + (char)0x00 + l + (char)0x00 + o + (char)0x00 + g + (char)0x00 + y + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x28, 0x00, 0x63, 0x00, 0x29, 0x00, 0x20, 0x00,
                    0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x74, 0x00,
                    0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00,
                    0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x54, 0x00,
                    0x65, 0x00, 0x63, 0x00, 0x68, 0x00, 0x6E, 0x00,
                    0x6F, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x67, 0x00,
                    0x79, 0x00
                }, Utilities.GetFileVersion, "StarForce"),

                new ContentMatchSet(new List<byte?[]>
                {
                    // Protection Technology, Ltd.
                    new byte?[]
                    {
                        0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x69,
                        0x6F, 0x6E, 0x20, 0x54, 0x65, 0x63, 0x68, 0x6E,
                        0x6F, 0x6C, 0x6F, 0x67, 0x79, 0x2C, 0x20, 0x4C,
                        0x74, 0x64, 0x2E
                    },

                    // // PSA_GetDiscLabel
                    // new byte?[]
                    // {
                    //     0x50, 0x53, 0x41, 0x5F, 0x47, 0x65, 0x74, 0x44,
                    //     0x69, 0x73, 0x63, 0x4C, 0x61, 0x62, 0x65, 0x6C
                    // },

                    // (c) Protection Technology
                    // new byte?[]
                    // {
                    //     0x28, 0x63, 0x29, 0x20, 0x50, 0x72, 0x6F, 0x74,
                    //     0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54,
                    //     0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67,
                    //     0x79
                    // },

                    // TradeName
                    new byte?[] { 0x54, 0x72, 0x61, 0x64, 0x65, 0x4E, 0x61, 0x6D, 0x65 },
                }, GetVersion, "StarForce"),

                // .sforce
                new ContentMatchSet(new byte?[] { 0x2E, 0x73, 0x66, 0x6F, 0x72, 0x63, 0x65 }, "StarForce 3-5"),

                // .brick
                new ContentMatchSet(new byte?[] { 0x2E, 0x62, 0x72, 0x69, 0x63, 0x6B }, "StarForce 3-5"),

                // Protection Technology, Ltd.
                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x69,
                    0x6F, 0x6E, 0x20, 0x54, 0x65, 0x63, 0x68, 0x6E,
                    0x6F, 0x6C, 0x6F, 0x67, 0x79, 0x2C, 0x20, 0x4C,
                    0x74, 0x64, 0x2E
                }, Utilities.GetFileVersion, "StarForce"),

                // P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + e + (char)0x00 + d + (char)0x00 +   + (char)0x00 + M + (char)0x00 + o + (char)0x00 + d + (char)0x00 + u + (char)0x00 + l + (char)0x00 + e + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x00, 0x72, 0x00, 0x6f, 0x00, 0x74, 0x00,
                    0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x65, 0x00,
                    0x64, 0x00, 0x20, 0x00, 0x4d, 0x00, 0x6f, 0x00,
                    0x64, 0x00, 0x75, 0x00, 0x6c, 0x00, 0x65, 0x00
                }, "StarForce 5"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("protect.dll", useEndsWith: true), "StarForce"),
                new PathMatchSet(new PathMatch("protect.exe", useEndsWith: true), "StarForce"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // TODO: Re-consolidate these once path matching is improved
                new PathMatchSet(new PathMatch("/protect.dll", useEndsWith: true), "StarForce"),
                new PathMatchSet(new PathMatch("/protect.exe", useEndsWith: true), "StarForce"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    
        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            return $"{Utilities.GetFileVersion(file)} ({fileContent.Skip(positions[1] + 22).TakeWhile(c => c != 0x00)})";
        }
    }
}
