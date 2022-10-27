using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// This is a placeholder SafeCast (sub-Macrovision) specific functionality
    /// </summary>
    public partial class Macrovision
    {
        internal string SafeCastCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.Equals("SafeCast2", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeCast";

            return null;
        }

        /// <inheritdoc/>
        internal ConcurrentQueue<string> SafeCastCheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in multiple versions of SafeCast, including Redump entry 83145 and IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new PathMatchSet(new PathMatch("cdac14ba.dll", useEndsWith: true), "SafeCast"),

                // Found in Redump entry 83145.
                new PathMatchSet(new PathMatch("CDAC21BA.DLL", useEndsWith: true), "SafeCast"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        internal string SafeCastCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("cdac11ba.exe", useEndsWith: true), "SafeCast"),

                // Found in multiple versions of SafeCast, including Redump entry 83145 and IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new PathMatchSet(new PathMatch("cdac14ba.dll", useEndsWith: true), "SafeCast"),

                // Found in Redump entry 83145.
                new PathMatchSet(new PathMatch("CDAC21BA.DLL", useEndsWith: true), "SafeCast"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
