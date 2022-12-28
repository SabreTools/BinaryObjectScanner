﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Bitpool is a copy protection found most commonly in German releases.
    /// It always has errors present on the disc (either between 1-4, or between 1,000-10,000+ depending on generation), and makes use of padded dummy files to prevent copying.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/Bitpool.md"/>
    /// </summary>
    public class Bitpool : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("bitpool.rsc", useEndsWith: true), "Bitpool"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}CD.IDX", useEndsWith: true), "Bitpool"),

                // Completely empty file present on multiple discs with Bitpool (Redump entries 52626 and 50229).
                new PathMatchSet(new PathMatch("LEADOUT.OFS", useEndsWith: true), "Bitpool"),

                // A set of 4 identically sized (within the same game, not between games), corrupted/padded files present in several games (Redump entries 31782 and 35476).
                // Both examples with only having the first letter uppercase and as the whole file name being uppercase have been seen.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch($"{Path.DirectorySeparatorChar}Crc_a", useEndsWith: true),
                    new PathMatch($"{Path.DirectorySeparatorChar}Crc_b", useEndsWith: true),
                    new PathMatch($"{Path.DirectorySeparatorChar}Crc_c", useEndsWith: true),
                    new PathMatch($"{Path.DirectorySeparatorChar}Crc_d", useEndsWith: true),
                }, "Bitpool"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("bitpool.rsc", useEndsWith: true), "Bitpool"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}CD.IDX", useEndsWith: true), "Bitpool"),

                // Completely empty file present on multiple discs with Bitpool (Redump entries 52626 and 50229).
                new PathMatchSet(new PathMatch("LEADOUT.OFS", useEndsWith: true), "Bitpool"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
