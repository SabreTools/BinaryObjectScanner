using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class GFWL : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;
            
            if (name.OptionalStartsWith("Games for Windows - LIVE Zero Day Piracy Protection", StringComparison.OrdinalIgnoreCase))
                return $"Games for Windows LIVE - Zero Day Piracy Protection Module {exe.GetInternalVersion()}";
            else if (name.OptionalStartsWith("Games for Windows", StringComparison.OrdinalIgnoreCase))
                return $"Games for Windows LIVE {exe.GetInternalVersion()}";

            // Get the import directory table
            if (exe.ImportTable?.ImportDirectoryTable != null)
            {
                if (Array.Exists(exe.ImportTable.ImportDirectoryTable, idte => idte?.Name == "xlive.dll"))
                    return "Games for Windows LIVE";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
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
                new(new PathMatch($"{Path.DirectorySeparatorChar}XLiveRedist"), "Games for Windows LIVE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
