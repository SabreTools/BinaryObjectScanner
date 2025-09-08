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
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Find what fvinfo field actually maps to this
            var name = exe.FileDescription;

            // There are some File Description checks that are currently too generic to use.
            // "Host Library" - Found in "protect.dll" in Redump entry 81756.
            // "User Interface Application" - Found in "protect.exe" in Redump entry 81756.
            // "Helper Application" - Found in "protect.x64" and "protect.x86" in Redump entry 81756.

            // Found in "sfdrvrem.exe" in Redump entry 102677.
            if (name.OptionalContains("FrontLine Drivers Removal Tool"))
                return $"StarForce FrontLine Driver Removal Tool";

            // Found in "protect.exe" in Redump entry 94805.
            if (name.OptionalContains("FrontLine Protection GUI Application"))
                return $"StarForce {exe.GetInternalVersion()}";

            // Found in "protect.dll" in Redump entry 94805.
            if (name.OptionalContains("FrontLine Protection Library"))
                return $"StarForce {exe.GetInternalVersion()}";

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name.OptionalContains("FrontLine Helper"))
                return $"StarForce {exe.GetInternalVersion()}";

            // TODO: Find a sample of this check.
            if (name.OptionalContains("Protected Module"))
                return $"StarForce 5";

            name = exe.LegalCopyright;
            if (name.OptionalStartsWith("(c) Protection Technology")) // (c) Protection Technology (StarForce)?
                return $"StarForce {exe.GetInternalVersion()}";
            else if (name.OptionalContains("Protection Technology")) // Protection Technology (StarForce)?
                return $"StarForce {exe.GetInternalVersion()}";
            
            // FrontLine ProActive (digital activation), samples: 
            // https://dbox.tools/titles/pc/46450FA4/ 
            // https://dbox.tools/titles/pc/4F430FA0/ 
            // https://dbox.tools/titles/pc/53450FA1/
            name = exe.TradeName;
            if (name.OptionalContains("FL ProActive")) 
                return $"FrontLine ProActive";

            // TODO: Decide if internal name checks are safe to use.
            name = exe.InternalName;

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name.OptionalEquals("CORE.ADMIN", StringComparison.Ordinal))
                return $"StarForce {exe.GetInternalVersion()}";


            // These checks currently disabled due being possibly too generic:
            // Found in "protect.dll" in Redump entry 94805.
            // if (name.OptionalEquals("CORE.DLL", StringComparison.Ordinal))
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(exe)}";
            //
            // Found in "protect.exe" in Redump entry 94805.
            // if (name.OptionalEquals("CORE.EXE", StringComparison.Ordinal))
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(exe)}";
            //
            // else if (name.OptionalEquals("protect.exe", StringComparison.Ordinal))
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(exe)}";

            // Check the export name table
            if (exe.Model.ExportTable?.ExportNameTable?.Strings != null)
            {
                // TODO: Should we just check for "PSA_*" instead of a single entry?
                if (Array.Exists(exe.Model.ExportTable.ExportNameTable.Strings, s => s == "PSA_GetDiscLabel"))
                    return $"StarForce {exe.GetInternalVersion()}";
            }

            // TODO: Check to see if there are any missing checks
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/StarForce.2.sg

            // Get the .brick section, if it exists
            if (exe.ContainsSection(".brick", exact: true))
                return "StarForce 3-5";

            // Get the .sforce* section, if it exists
            if (exe.ContainsSection(".sforce", exact: false))
                return "StarForce 3-5";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
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
