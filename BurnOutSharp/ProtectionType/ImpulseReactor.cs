using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    // Note that this set of checks also contains "Stardock Product Activation"
    // This is intentional, as that protection is highly related to Impulse Reactor
    public class ImpulseReactor : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.Contains("ImpulseReactor Dynamic Link Library") == true)
                return $"Impulse Reactor Core Module {Tools.Utilities.GetInternalVersion(pex)}";

            name = pex.ProductName;
            if (name?.Contains("ImpulseReactor Dynamic Link Library") == true)
                return $"Impulse Reactor Core Module {Tools.Utilities.GetInternalVersion(pex)}";

            name = pex.OriginalFilename;
            if (name?.Contains("ReactorActivate.exe") == true)
                return $"Stardock Product Activation {Tools.Utilities.GetInternalVersion(pex)}";

            // TODO: Check for CVP* instead?
            bool containsCheck = pex.ExportNameTable?.Any(s => s?.StartsWith("CVPInitializeClient") ?? false) ?? false;
            bool containsCheck2 = false;

            // Get the .rdata section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                containsCheck2 = strs.Any(s => s.EndsWith("ATTLIST"))
                    && strs.Any(s => s.Equals("ELEMENT"))
                    && strs.Any(s => s.StartsWith("NOTATION"));
            }

            if (containsCheck && containsCheck2)
                return $"Impulse Reactor Core Module {Tools.Utilities.GetInternalVersion(pex)}";
            else if (containsCheck && !containsCheck2)
                return $"Impulse Reactor";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Tools.Utilities.GetInternalVersion, "Impulse Reactor Core Module"),
                new PathMatchSet(new PathMatch("ReactorActivate.exe", useEndsWith: true), Tools.Utilities.GetInternalVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Tools.Utilities.GetInternalVersion, "Impulse Reactor Core Module"),
                new PathMatchSet(new PathMatch("ReactorActivate.exe", useEndsWith: true), Tools.Utilities.GetInternalVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
