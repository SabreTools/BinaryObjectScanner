using System;
using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// RipGuard was a copy protection made by Macrovision/Rovi to protect movie DVDs. It's known to use bad sectors to impede dumping, one of the relatively rare DVD DRMs to do this.
    /// 
    /// Discs known to have it: 
    /// https://forum.redfox.bz/threads/resolved-installs-rootkit-black-lagoon-vol-2-3-region-1.29660/
    /// https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/
    /// https://moral.net.au/writing/2015/10/10/backing_up_dvds/
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IExecutableCheck{T}.CheckExecutable(string, T, bool)"/>
        internal static string? RipGuardCheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            // Found in "RGASDEV.SYS" in the Black Lagoon Season 1 DVD Steelbook box set (Geneon ID 12970).
            if (name.OptionalEquals("rgasdev", StringComparison.OrdinalIgnoreCase))
                return "RipGuard";

            name = exe.ProductName;

            // Found in "RGASDEV.SYS" in the Black Lagoon Season 1 DVD Steelbook box set (Geneon ID 12970).
            if (name.OptionalEquals("rgasdev", StringComparison.OrdinalIgnoreCase))
                return "RipGuard";

            // So far, every seemingly-randomly named EXE on RipGuard discs have a consistent hash.
            if (exe.Length == 49_152)
            {
                var sha1 = HashTool.GetFileHash(file, HashType.SHA1);
                if (string.Equals(sha1, "6A7B8545800E0AB252773A8CD0A2185CA2497938", StringComparison.OrdinalIgnoreCase))
                    return "RipGuard";
            }

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, List{string})"/>
        internal static List<string> RipGuardCheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the Black Lagoon Season 1 DVD steelbook box set (Geneon ID 12970).
                new(new FilePathMatch("G23YHWO1.EXE"), "RipGuard"),
                new(new FilePathMatch("RGASDEV.SYS"), "RipGuard"),

                // Mentioned online in https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/.
                new(new FilePathMatch("9KMJ9G4I.EXE"), "RipGuard (Unconfirmed - Please report to us on GitHub)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal static string? RipGuardCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the Black Lagoon Season 1 DVD steelbook box set (Geneon ID 12970).
                new(new FilePathMatch("G23YHWO1.EXE"), "RipGuard"),
                new(new FilePathMatch("RGASDEV.SYS"), "RipGuard"),

                // Mentioned online in https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/.
                new(new FilePathMatch("9KMJ9G4I.EXE"), "RipGuard (Unconfirmed - Please report to us on GitHub)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
