﻿#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // Note that this set of checks also contains "Stardock Product Activation"
    // This is intentional, as that protection is highly related to Impulse Reactor
    public class ImpulseReactor : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name?.Contains("ImpulseReactor Dynamic Link Library") == true)
                return $"Impulse Reactor Core Module {pex.GetInternalVersion()}";

            name = pex.ProductName;
            if (name?.Contains("ImpulseReactor Dynamic Link Library") == true)
                return $"Impulse Reactor Core Module {pex.GetInternalVersion()}";

            name = pex.OriginalFilename;
            if (name?.Contains("ReactorActivate.exe") == true)
                return $"Stardock Product Activation {pex.GetInternalVersion()}";

            // TODO: Check for CVP* instead?
            bool containsCheck = pex.Model.ExportTable?.ExportNameTable?.Strings?.Any(s => s?.StartsWith("CVPInitializeClient") ?? false) ?? false;
            bool containsCheck2 = false;

            // Get the .rdata section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                containsCheck2 = strs.Any(s => s.EndsWith("ATTLIST"))
                    && strs.Any(s => s.Equals("ELEMENT"))
                    && strs.Any(s => s.StartsWith("NOTATION"));
            }

            if (containsCheck && containsCheck2)
                return $"Impulse Reactor Core Module {pex.GetInternalVersion()}";
            else if (containsCheck && !containsCheck2)
                return $"Impulse Reactor";

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
                new(new FilePathMatch("ImpulseReactor.dll"), GetInternalVersion, "Impulse Reactor Core Module"),
                new(new FilePathMatch("ReactorActivate.exe"), GetInternalVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("ImpulseReactor.dll"), GetInternalVersion, "Impulse Reactor Core Module"),
                new(new FilePathMatch("ReactorActivate.exe"), GetInternalVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private string? GetInternalVersion(string firstMatchedString, IEnumerable<string>? files)
        {
            try
            {
                using Stream fileStream = File.Open(firstMatchedString, FileMode.Open, FileAccess.Read, FileShare.Read);
                var pex = PortableExecutable.Create(fileStream);
                return pex?.GetInternalVersion() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
