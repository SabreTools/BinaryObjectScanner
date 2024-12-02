using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// CopyKiller was a program made by WebStylerZone that allowed users to copy-protect their burned discs.
    /// It worked by having users copy files with byte patterns that would create weak sectors to their discs to burn, and relied on drives with buggy firmwares to create bad burns of the discs.
    /// This would result in discs having intentional bad sectors, making them harder to copy. There was also an optional autorun available that would check for the original CopyKiller files on the disc.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/CopyKiller/CopyKiller.md"/>
    /// TODO: Add support for the developer's EXE obfuscator, "EXEShield Deluxe". Most, if not all, EXEShield protected files are currently detected as "EXE Stealth" by BOS.
    /// Samples include CopyKiller (Versions 3.64 & 3.99a) and SafeDiscScanner (Version 0.16) (https://archive.org/details/safediscscanner-0.16-webstylerzone-from-unofficial-source).
    /// </summary>
    public class CopyKiller : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Figure out why this check doesn't work.
            // Found in "autorun.exe" in CopyKiller V3.64, V3.99, and V3.99a.
            var name = pex.ProductName;
            if (name.OptionalStartsWith("CopyKiller", StringComparison.OrdinalIgnoreCase))
                return "CopyKiller V3.64+";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            // Previous versions of BOS noted to look at ".PFF" files as possible indicators of CopyKiller, but those files seem unrelated.
            // TODO: Figure out why this doesn't work.
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CopyKillerV3"), "CopyKiller V3.62-3.64"),
                new(new FilePathMatch("CopyKillerV4"), "CopyKiller V3.99-3.99a"),

                new(
                [
                    new FilePathMatch("ACK3900.ckt"),
                    new FilePathMatch("ACK3999.ckt"),
                    new FilePathMatch("CK100.wzc"),
                    new FilePathMatch("CK2500.ck"),
                    new FilePathMatch("CK3600.tcwz"),
                    new FilePathMatch("Engine.wzc"),
                    new FilePathMatch("P261XP.tcck"),
                    new FilePathMatch("WZ200.rwzc"),
                    new FilePathMatch("XCK3900.ck2"),
                ], "CopyKiller V3.99+"),

                new(
                [
                    new FilePathMatch("ACK3900.ckt"),
                    new FilePathMatch("CK100.wzc"),
                    new FilePathMatch("CK2500.ck"),
                    new FilePathMatch("CK3600.tcwz"),
                    new FilePathMatch("Engine.wzc"),
                    new FilePathMatch("P261XP.tcck"),
                    new FilePathMatch("WZ200.rwzc"),
                    new FilePathMatch("XCK3900.ck2"),
                ], "CopyKiller V3.64+"),

                new(
                [
                    new FilePathMatch("CK100.wzc"),
                    new FilePathMatch("Engine.wzc"),
                ], "CopyKiller V3.62+"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // Previous versions of BOS noted to look at ".PFF" files as possible indicators of CopyKiller, but those files seem unrelated.
            // TODO: Figure out why this doesn't work.
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CopyKillerV3"), "CopyKiller V3.62-3.64"),
                new(new FilePathMatch("CopyKillerV4"), "CopyKiller V3.99-3.99a"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
