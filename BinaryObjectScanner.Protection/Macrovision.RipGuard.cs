using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Matching;
using SabreTools.Serialization.Wrappers;
using static BinaryObjectScanner.Utilities.Hashing;

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
        /// <inheritdoc cref="BinaryObjectScanner.Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
        internal string RipGuardCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in "RGASDEV.SYS" in the Black Lagoon Season 1 DVD Steelbook box set (Geneon ID 12970).
            string name = pex.FileDescription;
            if (name?.Equals("rgasdev", StringComparison.OrdinalIgnoreCase) == true)
                return "RipGuard";

            // Found in "RGASDEV.SYS" in the Black Lagoon Season 1 DVD Steelbook box set (Geneon ID 12970).
            name = pex.ProductName;
            if (name?.Equals("rgasdev", StringComparison.OrdinalIgnoreCase) == true)
                return "RipGuard";

            if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
            {
                try
                {
                    FileInfo fi = new FileInfo(file);

                    // So far, every seemingly-randomly named EXE on RipGuard discs have a consistent hash.
                    if (fi.Length == 49_152)
                    {
                        string sha1 = GetFileSHA1(file);
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

        /// <inheritdoc cref="BinaryObjectScanner.Interfaces.IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
        internal ConcurrentQueue<string> RipGuardCheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the Black Lagoon Season 1 DVD steelbook box set (Geneon ID 12970).
                new PathMatchSet(new PathMatch("G23YHWO1.EXE", useEndsWith: true), "RipGuard"),
                new PathMatchSet(new PathMatch("RGASDEV.SYS", useEndsWith: true), "RipGuard"),

                // Mentioned online in https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/.
                new PathMatchSet(new PathMatch("9KMJ9G4I.EXE", useEndsWith: true), "RipGuard (Unconfirmed - Please report to us on GitHub)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="BinaryObjectScanner.Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal string RipGuardCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the Black Lagoon Season 1 DVD steelbook box set (Geneon ID 12970).
                new PathMatchSet(new PathMatch("G23YHWO1.EXE", useEndsWith: true), "RipGuard"),
                new PathMatchSet(new PathMatch("RGASDEV.SYS", useEndsWith: true), "RipGuard"),

                // Mentioned online in https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/.
                new PathMatchSet(new PathMatch("9KMJ9G4I.EXE", useEndsWith: true), "RipGuard (Unconfirmed - Please report to us on GitHub)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
