using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class StarForce : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        // TODO: Bring up to par with PiD.
        // Known issues: 
        // "Game.exe" not detected, "SF Crypto" not found in protect.* files (Redump entry 96137).
        // "HR.exe" Themida not detected, doesn't detect "[Builder]" (Is that the default StarForce?) (Redump entry 94805).
        // "ChromeEngine3.dll" and "SGP4.dll" not detected, doesn't detect "[FL Disc]" (Redump entry 93098).
        // "Replay.exe" not detected, doesn't detect "[FL Disc]" (Redump entry 81756).
        // Doesn't detect "[Pro]" (Redump entry 91336).
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Find what fvinfo field actually maps to this
            var name = pex.FileDescription;

            // There are some File Description checks that are currently too generic to use.
            // "Host Library" - Found in "protect.dll" in Redump entry 81756.
            // "User Interface Application" - Found in "protect.exe" in Redump entry 81756.
            // "Helper Application" - Found in "protect.x64" and "protect.x86" in Redump entry 81756.

            // Found in "sfdrvrem.exe" in Redump entry 102677.
            if (name?.Contains("FrontLine Drivers Removal Tool") == true)
                return $"StarForce FrontLine Driver Removal Tool";

            // Found in "protect.exe" in Redump entry 94805.
            if (name?.Contains("FrontLine Protection GUI Application") == true)
                return $"StarForce {pex.GetInternalVersion()}";

            // Found in "protect.dll" in Redump entry 94805.
            if (name?.Contains("FrontLine Protection Library") == true)
                return $"StarForce {pex.GetInternalVersion()}";

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name?.Contains("FrontLine Helper") == true)
                return $"StarForce {pex.GetInternalVersion()}";

            // TODO: Find a sample of this check.
            if (name?.Contains("Protected Module") == true)
                return $"StarForce 5";

            name = pex.LegalCopyright;
            if (name?.StartsWith("(c) Protection Technology") == true) // (c) Protection Technology (StarForce)?
                return $"StarForce {pex.GetInternalVersion()}";
            else if (name?.Contains("Protection Technology") == true) // Protection Technology (StarForce)?
                return $"StarForce {pex.GetInternalVersion()}";

            // TODO: Decide if internal name checks are safe to use.
            name = pex.InternalName;

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name?.Equals("CORE.ADMIN", StringComparison.Ordinal) == true)
                return $"StarForce {pex.GetInternalVersion()}";

            
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
            if (pex.Model.ExportTable?.ExportNameTable?.Strings != null)
            {
                // TODO: Should we just check for "PSA_*" instead of a single entry?
                if (Array.Exists(pex.Model.ExportTable.ExportNameTable.Strings, s => s == "PSA_GetDiscLabel"))
                    return $"StarForce {pex.GetInternalVersion()}";
            }

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
        public List<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // This file combination is found in Redump entry 21136.
                new(new List<PathMatch>
                {
                    new FilePathMatch("protect.x86"),
                    new FilePathMatch("protect.x64"),
                    new FilePathMatch("protect.dll"),
                    new FilePathMatch("protect.exe"),
                    new FilePathMatch("protect.msg"),
                }, "StarForce"),

                // This file combination is found in multiple games, such as Redump entries 81756, 91336, and 93657.
                new(new List<PathMatch>
                {
                    new FilePathMatch("protect.x86"),
                    new FilePathMatch("protect.x64"),
                    new FilePathMatch("protect.dll"),
                    new FilePathMatch("protect.exe"),
                }, "StarForce"),

                // This file combination is found in Redump entry 96137.
                new(new List<PathMatch>
                {
                    new FilePathMatch("protect.x86"),
                    new FilePathMatch("protect.dll"),
                    new FilePathMatch("protect.exe"),
                }, "StarForce"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // TODO: Determine if there are any file name checks that aren't too generic to use on their own.
            return null;
        }
    }
}
