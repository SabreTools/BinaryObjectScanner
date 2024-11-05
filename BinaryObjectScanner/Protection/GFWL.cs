using System;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class GFWL : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name?.StartsWith("Games for Windows - LIVE Zero Day Piracy Protection", StringComparison.OrdinalIgnoreCase) == true)
                return $"Games for Windows LIVE - Zero Day Piracy Protection Module {pex.GetInternalVersion()}";
            else if (name?.StartsWith("Games for Windows", StringComparison.OrdinalIgnoreCase) == true)
                return $"Games for Windows LIVE {pex.GetInternalVersion()}";

            // Get the import directory table
            if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            {
                bool match = pex.Model.ImportTable.ImportDirectoryTable.Any(idte => idte?.Name == "xlive.dll");
                if (match)
                    return "Games for Windows LIVE";
            }

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Might be specifically GFWL/Gfwlivesetup.exe
                new(new FilePathMatch("Gfwlivesetup.exe"), "Games for Windows LIVE"),
                new(new FilePathMatch("xliveinstall.dll"), "Games for Windows LIVE"),
                new(new FilePathMatch("XLiveRedist.msi"), "Games for Windows LIVE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Might be specifically GFWL/Gfwlivesetup.exe
                new(new FilePathMatch("Gfwlivesetup.exe"), "Games for Windows LIVE"),
                new(new FilePathMatch("xliveinstall.dll"), "Games for Windows LIVE"),
                new(new FilePathMatch("XLiveRedist.msi"), "Games for Windows LIVE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
