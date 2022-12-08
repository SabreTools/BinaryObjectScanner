using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using BurnOutSharp.Wrappers;

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
                return $"Impulse Reactor Core Module {Utilities.GetInternalVersion(pex)}";

            name = pex.ProductName;
            if (name?.Contains("ImpulseReactor Dynamic Link Library") == true)
                return $"Impulse Reactor Core Module {Utilities.GetInternalVersion(pex)}";

            name = pex.OriginalFilename;
            if (name?.Contains("ReactorActivate.exe") == true)
                return $"Stardock Product Activation {Utilities.GetInternalVersion(pex)}";

            // TODO: Check for CVP* instead?
            bool containsCheck = pex.ExportNameTable?.Any(s => s.StartsWith("CVPInitializeClient")) ?? false;

            // Get the .rdata section, if it exists
            if (pex.ContainsSection(".rdata"))
            {
                // TODO: Find what resource this is in
                // A + (char)0x00 + T + (char)0x00 + T + (char)0x00 + L + (char)0x00 + I + (char)0x00 + S + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + E + (char)0x00 + L + (char)0x00 + E + (char)0x00 + M + (char)0x00 + E + (char)0x00 + N + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + N + (char)0x00 + O + (char)0x00 + T + (char)0x00 + A + (char)0x00 + T + (char)0x00 + I + (char)0x00 + O + (char)0x00 + N + (char)0x00
                // ATTLIST\0ELEMENT\0NOTATION
                byte?[] check2 = new byte?[]
                {
                    0x41, 0x00, 0x54, 0x00, 0x54, 0x00, 0x4C, 0x00,
                    0x49, 0x00, 0x53, 0x00, 0x54, 0x00, 0x00, 0x00,
                    0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x4D, 0x00,
                    0x45, 0x00, 0x4E, 0x00, 0x54, 0x00, 0x00, 0x00,
                    0x4E, 0x00, 0x4F, 0x00, 0x54, 0x00, 0x41, 0x00,
                    0x54, 0x00, 0x49, 0x00, 0x4F, 0x00, 0x4E
                };
                bool containsCheck2 = pex.GetFirstSectionData(".rdata").FirstPosition(check2, out int position2);

                if (containsCheck && containsCheck2)
                    return $"Impulse Reactor Core Module {Utilities.GetInternalVersion(pex)}" + (includeDebug ? $" (Index {position}, {position2})" : string.Empty);
                else if (containsCheck && !containsCheck2)
                    return $"Impulse Reactor";
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Utilities.GetInternalVersion, "Impulse Reactor Core Module"),
                new PathMatchSet(new PathMatch("ReactorActivate.exe", useEndsWith: true), Utilities.GetInternalVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Utilities.GetInternalVersion, "Impulse Reactor Core Module"),
                new PathMatchSet(new PathMatch("ReactorActivate.exe", useEndsWith: true), Utilities.GetInternalVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
