using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.ProtectionType
{
    public class GFWL : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
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
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
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
