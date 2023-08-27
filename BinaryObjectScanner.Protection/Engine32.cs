using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Engine32 is the presumed name of a specific disc check DRM. This disc check merely checks for the presence of a specifically named file on the disc. 
    /// The file "engine32.dll" is always present (hence the name), and is where the disc checking logic is present.
    /// Engine32 appears to have been initially used in games localized by Nival and then later by Atomy.
    /// There is mention of the file "engine32.dll" being present in Fritz 15 as well (https://steamcommunity.com/app/427480/discussions/0/358416640404165471), though that's likely an unrelated file with the same name.
    /// </summary>
    public class Engine32 : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Most every tested sample of "engine32.dll" has a product name of "engine32", and the file description typically follows the naming pattern of "[Game Name] DLL-helper".

            // Detects Engine32 within the game executables that contain it.
            if (pex.ImportTable?.ImportDirectoryTable != null)
            {
                bool importDirectoryTableMatch = pex.ImportTable.ImportDirectoryTable.Any(idte => idte.Name?.Equals("ENGINE32.DLL", StringComparison.OrdinalIgnoreCase) == true);
                bool hintNameTableMatch = pex.ImportHintNameTable?.Any(ihne => ihne == "InitEngine") ?? false;

                // The Hint/Name Table Entry "DeinitEngine" is present in every tested sample, aside from TOCA Race Driver 2 (Redump entries 104593-104596).

                if (hintNameTableMatch && importDirectoryTableMatch)
                    return "Engine32";
            }

            // Detects Engine32 within the file "engine32.dll".
            if (pex.ExportNameTable != null)
            {
                bool exportNameTableMatch1 = pex.ExportNameTable.Any(s => s == "engine32.dll");
                bool exportNameTableMatch2 = pex.ExportNameTable.Any(s => s == "DeinitEngine");

                if (exportNameTableMatch1 && exportNameTableMatch2)
                    return "Engine32";
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "engine32.dll" is present in every known instance of this DRM, but isn't being checked for currently due to the generic file name.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "engine32.dll" is present in every known instance of this DRM, but isn't being checked for currently due to the generic file name.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
