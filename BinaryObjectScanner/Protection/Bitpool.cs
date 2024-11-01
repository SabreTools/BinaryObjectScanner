using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Bitpool is a copy protection found most commonly in German releases.
    /// It always has errors present on the disc (either between 1-4, or between 1,000-10,000+ depending on generation), and makes use of padded dummy files to prevent copying.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/Bitpool/Bitpool.md"/>
    /// </summary>
    public class Bitpool : IPathCheck
    {
        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("bitpool.rsc"), "Bitpool"),
                new(new FilePathMatch("CD.IDX"), "Bitpool"),

                // Completely empty file present on multiple discs with Bitpool (Redump entries 52626 and 50229).
                new(new FilePathMatch("LEADOUT.OFS"), "Bitpool"),

                // A set of 4 identically sized (within the same game, not between games), corrupted/padded files present in several games (Redump entries 31782 and 35476).
                // Both examples with only having the first letter uppercase and as the whole file name being uppercase have been seen.
                new(
                [
                    new FilePathMatch("Crc_a"),
                    new FilePathMatch("Crc_b"),
                    new FilePathMatch("Crc_c"),
                    new FilePathMatch("Crc_d"),
                ], "Bitpool"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("bitpool.rsc"), "Bitpool"),
                new(new FilePathMatch("CD.IDX"), "Bitpool"),

                // Completely empty file present on multiple discs with Bitpool (Redump entries 52626 and 50229).
                new(new FilePathMatch("LEADOUT.OFS"), "Bitpool"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
