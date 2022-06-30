using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Bitpool is a copy protection found most commonly in German releases.
    /// It makes use of bad sectors, and contains what appears to be an encrypted game executable always called "CD.IDX" (similar to SafeDisc ICD files), which seems to be present in every disc protected with it.
    /// I'm not entirely sure of the extent that Bitpool "creatively uses" the ISO9660 filesystem, but there are many, many issues in the filesystems present on these discs.
    /// The most obvious example is the "CD.IDX" file universally found in Bitpool games, which is often reported as being too large of a file to fit on a CD, as well as various other files that do the same but tend to vary from disc to disc.
    /// Investigating these files in a hex editor gives confusing results, at times matching the setup executable, or having other seemingly valid data before reaching the erroneous part of the file.
    /// Most Bitpool protected discs have errors (according to Redump/DIC standards) ranging in the 10,000's, though it seems that pre-1999 discs typically have 1-4 errors (Redump entries 48276 + 51376).
    /// A "bitpool.rsc" file is present in some, but not all Bitpool protected games. The purpose of it is so far unclear.
    /// Bitpool protected discs made 1999+ without additional intentional audio tracks always seem to have 3 tracks. The first being the "real" contents of the disc, the second being a brief audio track, and the third being a seemingly empty data track.
    /// Discs with additional intentional audio tracks don't seem to have any dedicated copy protection tracks, instead having only the main data track, and the additional sound tracks as expected.
    /// Discs made pre-1999 seem to not have any additional tracks specifically related to Bitpool present.
    /// It seems likely that some internal indicator may be present for executables with Bitpool, but it is currently unknown.
    /// Current files that are or may be related to Bitpool but aren't currently checked for include:
    /// "_" - A corrupted/padded file present in multiple games, in some cases may be a copy of CD.IDX? (Redump entries 35480 + 67046 + 68160).
    /// "__" - A corrupted/padded file present in at least one game (Redump entry 50229).
    /// "01.rsc" + "jmptable.rsc" - Corrupted/Padded files present in at least one game (Redump entry 52626).
    /// "AMAZON.GDB" + "DB.GDB" + "DV.GDB" + "MENTOR.GDB" + "PIMMONS.GDB" + "SAJIKI.GDB" + "TUTORIALS.GDB" - Corrupted/Padded files found in a specific game containing Bitpool (Redump entry 44976).
    /// "BDA.cab" + "BDB.RSC" - Corrupted/Padded files present in one specific game, with the file names seemingly representing the specific game they're found on (Redump entry 67046).
    /// "ds.dat" - A corrupted/padded file present in one specific game, with the file name seemingly representing the specific game it's found on (Redump entry 35476).
    /// Many other game specific, corrupted/padded files not worth noting down individually.
    /// </summary>
    public class Bitpool : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("bitpool.rsc", useEndsWith: true), "Bitpool"),
                new PathMatchSet(new PathMatch("CD.IDX", useEndsWith: true), "Bitpool"),

                // Completely empty file present on multiple discs with Bitpool (Redump entries 52626 and 50229)
                new PathMatchSet(new PathMatch("LEADOUT.OFS", useEndsWith: true), "Bitpool"),

                // A set of 4 identically sized (within the same game, not between games), corrupted/padded files present in several games (Redump entries 31782 + 35476).
                // Both examples with only having the first letter uppercase and as the whole file name being uppercase have been seen.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("Crc_a", useEndsWith: true),
                    new PathMatch("Crc_b", useEndsWith: true),
                    new PathMatch("Crc_c", useEndsWith: true),
                    new PathMatch("Crc_d", useEndsWith: true),
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
                new PathMatchSet(new PathMatch("CD.IDX", useEndsWith: true), "Bitpool"),

                // Completely empty file present on multiple discs with Bitpool (Redump entries 52626 and 50229)
                new PathMatchSet(new PathMatch("LEADOUT.OFS", useEndsWith: true), "Bitpool"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
