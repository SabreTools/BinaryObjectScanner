using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// This is a placeholder FLEXnet (sub-Macrovision) specific functionality
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
        internal string? FLEXnetCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.ProductName;

            // Found in "IsSvcInstDanceEJay7.dll" in IA item "computer200709dvd" (Dance eJay 7).
            if (name?.Equals("FLEXnet Activation Toolkit", StringComparison.OrdinalIgnoreCase) == true)
                return "FLEXnet";

            // Found in "INSTALLS.EXE", "LMGR326B.DLL", "LMGRD.EXE", and "TAKEFIVE.EXE" in IA item "prog-17_202403".
            if (name?.Equals("Globetrotter Software Inc lmgr326b Flexlm", StringComparison.OrdinalIgnoreCase) == true)
                return $"FlexLM {pex.ProductVersion}";

            // Generic case to catch unknown versions.
            if (name?.Contains("Flexlm") == true)
                return "FlexLM (Unknown Version - Please report to us on GitHub)";

            name = pex.FileDescription;

            // Found in "INSTALLS.EXE", "LMGR326B.DLL", "LMGRD.EXE", and "TAKEFIVE.EXE" in IA item "prog-17_202403".
            if (name?.Equals("lmgr326b", StringComparison.OrdinalIgnoreCase) == true)
                return $"FlexLM {pex.ProductVersion}";

            name = pex.LegalTrademarks;

            // Found in "INSTALLS.EXE", "LMGR326B.DLL", "LMGRD.EXE", and "TAKEFIVE.EXE" in IA item "prog-17_202403".
            if (name?.Equals("Flexible License Manager,FLEXlm,Globetrotter,FLEXID", StringComparison.OrdinalIgnoreCase) == true)
                return $"FlexLM {pex.ProductVersion}";

            if (name?.Contains("FLEXlm") == true)
                return $"FlexLM {pex.ProductVersion}";

            name = pex.OriginalFilename;

            // Found in "INSTALLS.EXE", "LMGR326B.DLL", "LMGRD.EXE", and "TAKEFIVE.EXE" in IA item "prog-17_202403".
            // It isn't known why these various executables have the same original filename.
            if (name?.Equals("lmgr326b.dll", StringComparison.OrdinalIgnoreCase) == true)
                return $"FlexLM {pex.ProductVersion}";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "FLEXLM.CPL", "INSTALLS.EXE", "LMGR326B.DLL", "LMGRD.EXE", and "TAKEFIVE.EXE" in IA item "prog-17_202403".
                if (strs.Any(s => s.Contains("FLEXlm License Manager")))
                    return "FlexLM";
            }

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
#if NET20 || NET35
        internal Queue<string> FLEXNetCheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        internal ConcurrentQueue<string> FLEXNetDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "prog-17_202403".
                new(new PathMatch("FlexLM-6.1F", useEndsWith: true), "FlexLM 6.1f"),
                new(new PathMatch("FlexLM", useEndsWith: true), "FlexLM"),
                new(new PathMatch("FLexLM_Licensing.wri", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMGR326B.DLL", useEndsWith: true), "FlexLM"),
                new(new PathMatch("FLEXLM.CPL", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMGRD.EXE", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMGRD95.EXE", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMUTIL.EXE", useEndsWith: true), "FlexLM"),
                new(new PathMatch("READFLEX.WRI", useEndsWith: true), "FlexLM"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal string? FLEXNetCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "prog-17_202403".
                new(new PathMatch("FlexLM-6.1F", useEndsWith: true), "FlexLM 6.1f"),
                new(new PathMatch("FlexLM", useEndsWith: true), "FlexLM"),
                new(new PathMatch("FLexLM_Licensing.wri", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMGR326B.DLL", useEndsWith: true), "FlexLM"),
                new(new PathMatch("FLEXLM.CPL", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMGRD.EXE", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMGRD95.EXE", useEndsWith: true), "FlexLM"),
                new(new PathMatch("LMUTIL.EXE", useEndsWith: true), "FlexLM"),
                new(new PathMatch("READFLEX.WRI", useEndsWith: true), "FlexLM"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
