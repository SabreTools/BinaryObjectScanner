using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Matching;
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
        /// <inheritdoc cref="Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
        internal string? RipGuardCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in "RGASDEV.SYS" in the Black Lagoon Season 1 DVD Steelbook box set (Geneon ID 12970).
            var name = pex.FileDescription;
            if (name?.Equals("rgasdev", StringComparison.OrdinalIgnoreCase) == true)
                return "RipGuard";

            // Found in "RGASDEV.SYS" in the Black Lagoon Season 1 DVD Steelbook box set (Geneon ID 12970).
            name = pex.ProductName;
            if (name?.Equals("rgasdev", StringComparison.OrdinalIgnoreCase) == true)
                return "RipGuard";

            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                try
                {
                    var fi = new FileInfo(file);

                    // So far, every seemingly-randomly named EXE on RipGuard discs have a consistent hash.
                    if (fi.Length == 49_152)
                    {
                        var sha1 = HashTool.GetFileHash(file, HashType.SHA1);
                        if (sha1 == "6A7B8545800E0AB252773A8CD0A2185CA2497938")
                            return "RipGuard";
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
#if NET20 || NET35
        internal Queue<string> RipGuardCheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        internal ConcurrentQueue<string> RipGuardCheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the Black Lagoon Season 1 DVD steelbook box set (Geneon ID 12970).
                new(new PathMatch("G23YHWO1.EXE", useEndsWith: true), "RipGuard"),
                new(new PathMatch("RGASDEV.SYS", useEndsWith: true), "RipGuard"),

                // Mentioned online in https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/.
                new(new PathMatch("9KMJ9G4I.EXE", useEndsWith: true), "RipGuard (Unconfirmed - Please report to us on GitHub)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal string? RipGuardCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the Black Lagoon Season 1 DVD steelbook box set (Geneon ID 12970).
                new(new PathMatch("G23YHWO1.EXE", useEndsWith: true), "RipGuard"),
                new(new PathMatch("RGASDEV.SYS", useEndsWith: true), "RipGuard"),

                // Mentioned online in https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/.
                new(new PathMatch("9KMJ9G4I.EXE", useEndsWith: true), "RipGuard (Unconfirmed - Please report to us on GitHub)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
