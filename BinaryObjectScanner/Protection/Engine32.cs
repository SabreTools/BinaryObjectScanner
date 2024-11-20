using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Engine32 is the presumed name of a specific disc check DRM. This disc check merely checks for the presence of a specifically named file on the disc. 
    /// The file "engine32.dll" is always present (hence the name), and is where the disc checking logic is present.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/engine32/engine32.md"/>
    /// </summary>
    public class Engine32 : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Most every tested sample of "engine32.dll" has a product name of "engine32", and the file description typically follows the naming pattern of "[Game Name] DLL-helper".

            // Detects Engine32 within the game executables that contain it.
            if (pex.Model.ImportTable?.ImportDirectoryTable != null && pex.Model.ImportTable?.HintNameTable != null)
            {
                bool importDirectoryTableMatch = Array.Exists(pex.Model.ImportTable.ImportDirectoryTable, idte => idte?.Name != null && idte.Name.Equals("ENGINE32.DLL", StringComparison.OrdinalIgnoreCase));
                bool hintNameTableMatch = Array.Exists(pex.Model.ImportTable?.HintNameTable ?? [], ihne => ihne?.Name == "InitEngine");

                // The Hint/Name Table Entry "DeinitEngine" is present in every tested sample, aside from TOCA Race Driver 2 (Redump entries 104593-104596).

                if (hintNameTableMatch && importDirectoryTableMatch)
                    return "Engine32";
            }

            // Detects Engine32 within the file "engine32.dll".
            if (pex.Model.ExportTable?.ExportNameTable?.Strings != null)
            {
                bool exportNameTableMatch1 = Array.Exists(pex.Model.ExportTable.ExportNameTable.Strings, s => s == "engine32.dll");
                bool exportNameTableMatch2 = Array.Exists(pex.Model.ExportTable.ExportNameTable.Strings, s => s == "DeinitEngine");

                if (exportNameTableMatch1 && exportNameTableMatch2)
                    return "Engine32";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "engine32.dll" is present in every known instance of this DRM, but isn't being checked for currently due to the generic file name.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "engine32.dll" is present in every known instance of this DRM, but isn't being checked for currently due to the generic file name.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
