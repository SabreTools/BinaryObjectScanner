using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Add the content checks from SafeDisc here
    // TODO: Investigate if this entire file should be wrapped into SafeDisc
    public class SafeCast : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                new ContentMatchSet(new List<byte?[]>
                {
                    // BoG_ *90.0&!!  Yy>
                    new byte?[]
                    {
                        0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                        0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                        0x79, 0x3E
                    },

                    // product activation library
                    new byte?[]
                    {
                        0x70, 0x72, 0x6F, 0x64, 0x75, 0x63, 0x74, 0x20,
                        0x61, 0x63, 0x74, 0x69, 0x76, 0x61, 0x74, 0x69,
                        0x6F, 0x6E, 0x20, 0x6C, 0x69, 0x62, 0x72, 0x61,
                        0x72, 0x79
                    },
                }, GetVersion, "SafeCast"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("cdac11ba.exe", useEndsWith: true), "SafeCast"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("cdac11ba.exe", useEndsWith: true), "SafeCast"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 20; // Begin reading after "BoG_ *90.0&!!  Yy>" for old SafeDisc
            int version = BitConverter.ToInt32(fileContent, index);
            index += 4;
            int subVersion = BitConverter.ToInt32(fileContent, index);
            index += 4;
            int subsubVersion = BitConverter.ToInt32(fileContent, index);

            if (version != 0)
                return $"{version}.{subVersion:00}.{subsubVersion:000}";

            index = positions[0] + 18 + 14; // Begin reading after "BoG_ *90.0&!!  Yy>" for newer SafeDisc
            version = BitConverter.ToInt32(fileContent, index);
            index += 4;
            subVersion = BitConverter.ToInt32(fileContent, index);
            index += 4;
            subsubVersion = BitConverter.ToInt32(fileContent, index);

            if (version == 0)
                return string.Empty;

            return $"{version}.{subVersion:00}.{subsubVersion:000}";
        }
    }
}
