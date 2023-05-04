using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// RipGuard was a copy protection made by Macrovision/Rovi to protect movie DVDs. It's known to use bad sectors to impede dumping, one of the relatively rare DVD DRMs to do this.
    /// 
    /// Discs known to have it: 
    /// https://forum.redfox.bz/threads/resolved-installs-rootkit-black-lagoon-vol-2-3-region-1.29660/
    /// https://forum.redfox.bz/threads/resolved-one-on-one-with-tony-horton-vol2-disc3.33901/
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="BinaryObjectScanner.Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
        internal string RipGuardCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
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
