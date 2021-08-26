using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class ImpulseReactor : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                new ContentMatchSet(new List<byte?[]>
                {
                    // CVPInitializeClient
                    new byte?[]
                    {
                        0x43, 0x56, 0x50, 0x49, 0x6E, 0x69, 0x74, 0x69,
                        0x61, 0x6C, 0x69, 0x7A, 0x65, 0x43, 0x6C, 0x69,
                        0x65, 0x6E, 0x74
                    },

                    // A + (char)0x00 + T + (char)0x00 + T + (char)0x00 + L + (char)0x00 + I + (char)0x00 + S + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + E + (char)0x00 + L + (char)0x00 + E + (char)0x00 + M + (char)0x00 + E + (char)0x00 + N + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + N + (char)0x00 + O + (char)0x00 + T + (char)0x00 + A + (char)0x00 + T + (char)0x00 + I + (char)0x00 + O + (char)0x00 + N + (char)0x00
                    new byte?[]
                    {
                        0x41, 0x00, 0x54, 0x00, 0x54, 0x00, 0x4C, 0x00,
                        0x49, 0x00, 0x53, 0x00, 0x54, 0x00, 0x00, 0x00,
                        0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x4D, 0x00,
                        0x45, 0x00, 0x4E, 0x00, 0x54, 0x00, 0x00, 0x00,
                        0x4E, 0x00, 0x4F, 0x00, 0x54, 0x00, 0x41, 0x00,
                        0x54, 0x00, 0x49, 0x00, 0x4F, 0x00, 0x4E
                    },
                }, Utilities.GetFileVersion, "Impulse Reactor"),

                // CVPInitializeClient
                new ContentMatchSet(new byte?[]
                {
                    0x43, 0x56, 0x50, 0x49, 0x6E, 0x69, 0x74, 0x69,
                    0x61, 0x6C, 0x69, 0x7A, 0x65, 0x43, 0x6C, 0x69,
                    0x65, 0x6E, 0x74
                }, "Impulse Reactor"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Utilities.GetFileVersion, "Impulse Reactor"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Utilities.GetFileVersion, "Impulse Reactor"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
