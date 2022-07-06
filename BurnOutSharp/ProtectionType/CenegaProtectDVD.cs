using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Cenega ProtectDVD is a protection seemingly created by the publisher Cenega for use with their games.
    /// Games using this protection aren't able to be run from an ISO file, and presumably use DMI as a protection feature.
    /// </summary>
    public class CengaProtectDVD : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .cenega section, if it exists. Seems to be found in the protected game executable (Redump entry 31422).
            bool cenegaSection = pex.ContainsSection(".cenega", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            // Get the .cenega0 through .cenega2 sections, if they exists. Found in cenega.dll (Redump entry 31422).
            cenegaSection = pex.ContainsSection(".cenega0", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            cenegaSection = pex.ContainsSection(".cenega1", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            cenegaSection = pex.ContainsSection(".cenega2", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Seems likely to be present in most, if not all discs protected with Cenega ProtectDVD, but unable to confirm due to only having a small sample size.
                // References the existence of a "ProtectDVD.dll", which has not yet been located (Redump entry 31422).
                new PathMatchSet(new PathMatch("cenega.dll", useEndsWith: true), "Cenega ProtectDVD"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Seems likely to be present in most, if not all discs protected with Cenega ProtectDVD, but unable to confirm due to only having a small sample size.
                // References the existence of a "ProtectDVD.dll", which has not yet been located (Redump entry 31422).
                new PathMatchSet(new PathMatch("cenega.dll", useEndsWith: true), "Cenega ProtectDVD"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
