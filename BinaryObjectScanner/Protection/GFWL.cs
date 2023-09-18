using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class GFWL : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
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
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Might be specifically GFWL/Gfwlivesetup.exe
                new PathMatchSet(new PathMatch("Gfwlivesetup.exe", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("xliveinstall.dll", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows LIVE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Might be specifically GFWL/Gfwlivesetup.exe
                new PathMatchSet(new PathMatch("Gfwlivesetup.exe", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("xliveinstall.dll", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows LIVE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
