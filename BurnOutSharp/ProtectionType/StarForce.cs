using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class StarForce : IPathCheck, IPortableExecutableCheck
    {
        // TODO: Bring up to par with PiD.
        // Known issues: 
        // "Game.exe" not detected, "SF Crypto" not found in protect.* files (Redump entry 96137).
        // "HR.exe" Themida not detected, doesn't detect "[Builder]" (Is that the default StarForce?) (Redump entry 94805).
        // "ChromeEngine3.dll" and "SGP4.dll" not detected, doesn't detect "[FL Disc]" (Redump entry 93098).
        // "Replay.exe" not detected, doesn't detect "[FL Disc]" (Redump entry 81756).
        // Doesn't detect "[Pro]" (Redump entry 91336).
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.LegalCopyright;
            if (name?.StartsWith("(c) Protection Technology") == true) // (c) Protection Technology (StarForce)?
                return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";
            else if (name?.Contains("Protection Technology") == true) // Protection Technology (StarForce)?
                return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";

            // TODO: Decide if internal name checks are safe to use.
            name = pex.InternalName;

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name?.Equals("CORE.ADMIN", StringComparison.Ordinal) == true)
                return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";

            
            // These checks currently disabled due being possibly too generic:
            // Found in "protect.dll" in Redump entry 94805.
            // if (name?.Equals("CORE.DLL", StringComparison.Ordinal) == true)
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";
            //
            // Found in "protect.exe" in Redump entry 94805.
            // if (name?.Equals("CORE.EXE", StringComparison.Ordinal) == true)
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";
            //
            // else if (name?.Equals("protect.exe", StringComparison.Ordinal) == true)
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";

            // Check the export name table
            if (pex.ExportNameTable != null)
            {
                // TODO: Should we just check for "PSA_*" instead of a single entry?
                if (pex.ExportNameTable.Any(s => s == "PSA_GetDiscLabel"))
                    return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";
            }

            // TODO: Find what fvinfo field actually maps to this
            name = pex.FileDescription;

            // There are some File Description checks that are currently too generic to use.
            // "Host Library" - Found in "protect.dll" in Redump entry 81756.
            // "User Interface Application" - Found in "protect.exe" in Redump entry 81756.
            // "Helper Application" - Found in "protect.x64" and "protect.x86" in Redump entry 81756.

            // Found in "protect.exe" in Redump entry 94805.
            if (name?.Contains("FrontLine Protection GUI Application") == true)
                return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";

            // Found in "protect.dll" in Redump entry 94805.
            if (name?.Contains("FrontLine Protection Library") == true)
                return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name?.Contains("FrontLine Helper") == true)
                return $"StarForce {Tools.Utilities.GetInternalVersion(pex)}";

            // TODO: Find a sample of this check.
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
            var matchers = new List<PathMatchSet>
            {
                // This file combination is found in Redump entry 21136.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("protect.x86", useEndsWith: true),
                    new PathMatch("protect.x64", useEndsWith: true),
                    new PathMatch("protect.dll", useEndsWith: true),
                    new PathMatch("protect.exe", useEndsWith: true),
                    new PathMatch("protect.msg", useEndsWith: true),
                }, "StarForce"),

                // This file combination is found in multiple games, such as Redump entries 81756, 91336, and 93657.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("protect.x86", useEndsWith: true),
                    new PathMatch("protect.x64", useEndsWith: true),
                    new PathMatch("protect.dll", useEndsWith: true),
                    new PathMatch("protect.exe", useEndsWith: true),
                }, "StarForce"),

                // This file combination is found in Redump entry 96137.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("protect.x86", useEndsWith: true),
                    new PathMatch("protect.dll", useEndsWith: true),
                    new PathMatch("protect.exe", useEndsWith: true),
                }, "StarForce"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Determine if there are any file name checks that aren't too generic to use on their own.
            return null;
        }
    }
}
