using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using static System.Net.WebRequestMethods;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// This is a placeholder for all Macrovision-based protections. See partial classes for more details
    /// </summary>
    public partial class Macrovision : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // TODO: Add all common Macrovision PE checks here

            // Example: SafeDisc invocation
            //string safeDisc = SafeDiscCheckPortableExecutable(file, pex, includeDebug);
            //if (!string.IsNullOrWhiteSpace(safeDisc))
            //    return safeDisc;

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Add all common Macrovision directory path checks here

            // Example: SafeDisc invocation
            //var safeDisc = SafeDiscCheckDirectoryPath(path, files);
            //if (safeDisc != null && !safeDisc.IsEmpty)
            //    return safeDisc;

            return MatchUtil.GetAllMatches(files, null, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Add all common Macrovision file path checks here

            // Example: SafeDisc invocation
            //string safeDisc = SafeDiscCheckFilePath(path);
            //if (!string.IsNullOrWhiteSpace(safeDisc))
            //    return safeDisc;

            return MatchUtil.GetFirstMatch(path, null, any: true);
        }
    }
}
