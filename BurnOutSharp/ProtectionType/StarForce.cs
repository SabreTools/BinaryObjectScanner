using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class StarForce : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.LegalCopyright;
            if (name?.Contains("Protection Technology") == true) // Protection Technology (StarForce)?
                return $"StarForce {Utilities.GetInternalVersion(pex)}";

            name = pex.InternalName;
            if (name?.Equals("CORE.EXE", StringComparison.Ordinal) == true)
                return $"StarForce {Utilities.GetInternalVersion(pex)}";
            else if (name?.Equals("protect.exe", StringComparison.Ordinal) == true)
                return $"StarForce {Utilities.GetInternalVersion(pex)}";

            // TODO: Find what fvinfo field actually maps to this
            name = pex.FileDescription;
            if (name?.Contains("Protected Module") == true)
                return $"StarForce 5";

            // TODO: Check to see if there are any missing checks
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/StarForce.2.sg

            // Get the .brick section, if it exists
            bool brickSection = pex.ContainsSection(".brick", exact: true);
            if (brickSection)
                return "StarForce 3-5";

            // Get the .sforce* section, if it exists
            bool sforceSection = pex.ContainsSection(".sforce", exact: false);
            if (sforceSection)
                return "StarForce 3-5";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // These have too high of a chance of over-matching by themselves
            // var matchers = new List<PathMatchSet>
            // {
            //     // TODO: Re-consolidate these once path matching is improved
            //     new PathMatchSet(new PathMatch("/protect.dll", useEndsWith: true), "StarForce"),
            //     new PathMatchSet(new PathMatch("/protect.exe", useEndsWith: true), "StarForce"),
            // };

            // return MatchUtil.GetAllMatches(files, matchers, any: false);
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // These have too high of a chance of over-matching by themselves
            // var matchers = new List<PathMatchSet>
            // {
            //     // TODO: Re-consolidate these once path matching is improved
            //     new PathMatchSet(new PathMatch("/protect.dll", useEndsWith: true), "StarForce"),
            //     new PathMatchSet(new PathMatch("/protect.exe", useEndsWith: true), "StarForce"),
            // };

            // return MatchUtil.GetFirstMatch(path, matchers, any: true);
            return null;
        }
    }
}
