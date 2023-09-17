using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class TAGES : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // Known TAGES Driver Setup filenames:
            // - DrvSetup.exe
            // - DrvSetup_x64.exe
            // - TagesSetup.exe
            // - TagesSetup_x64.exe

            // Known TAGES Activation Client filenames:
            // - TagesClient.exe
            // - TagesClient.dat (Does not always exist)

            string name = pex.FileDescription;
            if (name?.StartsWith("TagesSetup", StringComparison.OrdinalIgnoreCase) == true)
                return $"TAGES Driver Setup {GetVersion(pex)}";
            else if (name?.StartsWith("Tagès activation client", StringComparison.OrdinalIgnoreCase) == true)
                return $"TAGES Activation Client {GetVersion(pex)}";

            name = pex.ProductName;
            if (name?.StartsWith("Application TagesSetup", StringComparison.OrdinalIgnoreCase) == true)
                return $"TAGES Driver Setup {GetVersion(pex)}";
            else if (name?.StartsWith("T@GES", StringComparison.OrdinalIgnoreCase) == true)
                return $"TAGES Activation Client {GetVersion(pex)}";

            // TODO: Add entry point check
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Tages.2.sg

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // (char)0xE8 + u + (char)0x00 + (char)0x00 + (char)0x00 + (char)0xE8 + ?? + ?? + (char)0xFF + (char)0xFF + "h"
                    new ContentMatchSet(new byte?[] { 0xE8, 0x75, 0x00, 0x00, 0x00, 0xE8, null, null, 0xFF, 0xFF, 0x68 }, GetVersion, "TAGES"),
                };

                string match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far, only known to exist in early versions of "Moto Racer 3" (Redump entries 31578 and 34669).
                new PathMatchSet(new List<PathMatch>
                {
                    // d37f70489207014d7d0fbaa43b081a93e8030498
                    new PathMatch(Path.Combine("Sys", "Devx.sys").Replace("\\", "/"), useEndsWith: true),

                    // a0acbc2f8e321e4f30c913c095e28af444058249
                    new PathMatch(Path.Combine("Sys", "VtPr.sys").Replace("\\", "/"), useEndsWith: true),

                    // SHA-1 is variable, file size is 81,920 bytes
                    new PathMatch("Wave.aif", useEndsWith: true),

                    // f82339d797be6da92f5d9dadeae9025385159057
                    new PathMatch("Wave.alf", useEndsWith: true),

                    // 0351d0f3d4166362a1a9d838c9390a3d92945a44
                    new PathMatch("Wave.apt", useEndsWith: true),

                    // SHA-1 is variable, file size is 61,440 bytes
                    new PathMatch("Wave.axt", useEndsWith: true),
                }, "TAGES"),

                // Currently only found in "Robocop" (Redump entry 35932).
                // Found in a directory named "System", with an executable named "Setup.exe".
                new PathMatchSet(new List<PathMatch>
                {
                    // f82339d797be6da92f5d9dadeae9025385159057
                    new PathMatch(Path.Combine("9x", "Tamlx.alf").Replace("\\", "/"), useEndsWith: true),

                    // 933c004d3043863f019f5ffaf63402a30e65026c
                    new PathMatch(Path.Combine("9x", "Tamlx.apt").Replace("\\", "/"), useEndsWith: true),

                    // d45745fa6b0d23fe0ee12e330ab85d5bf4e0e776
                    new PathMatch(Path.Combine("NT", "enodpl.sys").Replace("\\", "/"), useEndsWith: true),

                    // f111eba05ca6e9061c557547420847d7fdee657d
                    new PathMatch(Path.Combine("NT", "litdpl.sys").Replace("\\", "/"), useEndsWith: true),
                }, "TAGES"),

                // Currently only known to exist in "XIII" and "Beyond Good & Evil" (Redump entries 8774-8776, 45940-45941, 18690-18693, and presumably 21320, 21321, 21323, and 36124).
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("enodpl.sys", useEndsWith: true),
                    new PathMatch("ENODPL.VXD", useEndsWith: true),
                    new PathMatch("tandpl.sys", useEndsWith: true),
                    new PathMatch("TANDPL.VXD", useEndsWith: true),
                }, "TAGES"),

                // The directory of these files has been seen to be named two different things, with two different accompanying executables in the root of the directory.
                // In the example where the directory is named "Drivers", the executable is named "Silent.exe" (Redump entry 51763).
                // In the example where the directory is named "ELBdrivers", the executable is name "ELBDrivers.exe" (Redump entry 91090).
                // The name and file size of the included executable vary, but there should always be one here.
                new PathMatchSet(new List<PathMatch>
                {
                    // 40826e95f3ad8031b6debe15aca052c701288e04
                    new PathMatch(Path.Combine("9x", "hwpsgt.vxd").Replace("\\", "/"), useEndsWith: true),

                    // f82339d797be6da92f5d9dadeae9025385159057
                    new PathMatch(Path.Combine("9x", "lemsgt.vxd").Replace("\\", "/"), useEndsWith: true),

                    // 43f407ecdc0d87a3713126b757ccaad07ade285f
                    new PathMatch(Path.Combine("NT", "hwpsgt.sys").Replace("\\", "/"), useEndsWith: true),

                    // 548dd6359abbcc8c84ce346d078664eeedc716f7
                    new PathMatch(Path.Combine("NT", "lemsgt.sys").Replace("\\", "/"), useEndsWith: true),
                }, "TAGES"),

                // The following files are supposed to only be found inside the driver setup executables, and are present in at least version 5.2.0.1 (Redump entry 15976).
                new PathMatchSet(new PathMatch("ithsgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lilsgt.sys", useEndsWith: true), "TAGES Driver"),

                // The following files are supposed to only be found inside the driver setup executables in versions 5.5.0.1+.
                new PathMatchSet(new PathMatch("atksgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lirsgt.sys", useEndsWith: true), "TAGES Driver"),

                // The following files appear to be container formats for TAGES, but little is currently known about them (Redump entries 85313 and 85315).
                new PathMatchSet(new PathMatch("GameModule.elb", useEndsWith: true), "TAGES/SolidShield Game Executable Container"),
                new PathMatchSet(new PathMatch("InstallModule.elb", useEndsWith: true), "TAGES/SolidShield Installer Container"),

                // Not much is known about this file, but it seems to be related to what PiD reports as "protection level: Tages BASIC" (Redump entry 85313).
                // Seems to always be found with other KWN files.
                new PathMatchSet(new PathMatch("GAME.KWN", useEndsWith: true), "TAGES (BASIC?)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far, only known to exist in early versions of "Moto Racer 3" (Redump entries 31578 and 34669).
                new PathMatchSet(new PathMatch("Devx.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("VtPr.sys", useEndsWith: true), "TAGES Driver"),

                // These are removed because they can potentially overmatch
                // new PathMatchSet(new PathMatch("Wave.aif", useEndsWith: true), "TAGES Driver"),
                // new PathMatchSet(new PathMatch("Wave.alf", useEndsWith: true), "TAGES Driver"),
                // new PathMatchSet(new PathMatch("Wave.apt", useEndsWith: true), "TAGES Driver"),
                // new PathMatchSet(new PathMatch("Wave.axt", useEndsWith: true), "TAGES Driver"),

                // Currently only found in "Robocop" (Redump entry 35932).
                // Found in a directory named "System", with an executable named "Setup.exe".
                new PathMatchSet(new PathMatch("Tamlx.apt", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("Tamlx.alf", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("enodpl.sys", useEndsWith: true), "TAGES NT Driver"),
                new PathMatchSet(new PathMatch("litdpl.sys", useEndsWith: true), "TAGES NT Driver"),

                // Currently only known to exist in "XIII" and "Beyond Good & Evil" (Redump entries 8774-8776, 45940-45941, 18690-18693, and presumably 21320, 21321, 21323, and 36124).
                new PathMatchSet(new PathMatch("enodpl.sys", useEndsWith: true), "TAGES NT Driver"),
                new PathMatchSet(new PathMatch("ENODPL.VXD", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("tandpl.sys", useEndsWith: true), "TAGES NT Driver"),
                new PathMatchSet(new PathMatch("TANDPL.VXD", useEndsWith: true), "TAGES 9x Driver"),

                // The directory of these files has been seen to be named two different things, with two different accompanying executables in the root of the directory.
                // In the example where the directory is named "Drivers", the executable is named "Silent.exe" (Redump entry 51763).
                // In the example where the directory is named "ELBdrivers", the executable is name "ELBDrivers.exe" (Redump entry 91090).
                // The name and file size of the included executable vary, but there should always be one here.
                new PathMatchSet(new PathMatch("hwpsgt.vxd", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("lemsgt.vxd", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("hwpsgt.sys", useEndsWith: true), "TAGES NT Driver"),
                new PathMatchSet(new PathMatch("lemsgt.sys", useEndsWith: true), "TAGES NT Driver"),

                // The following files are supposed to only be found inside the driver setup executables, and are present in at least version 5.2.0.1 (Redump entry 15976).
                new PathMatchSet(new PathMatch("ithsgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lilsgt.sys", useEndsWith: true), "TAGES Driver"),

                // The following files are supposed to only be found inside the driver setup executables in versions 5.5.0.1+.
                new PathMatchSet(new PathMatch("atksgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lirsgt.sys", useEndsWith: true), "TAGES Driver"),

                // The following files appear to be container formats for TAGES, but little is currently known about them (Redump entries 85313 and 85315).
                new PathMatchSet(new PathMatch("GameModule.elb", useEndsWith: true), "TAGES/SolidShield Game Executable Container"),
                new PathMatchSet(new PathMatch("InstallModule.elb", useEndsWith: true), "TAGES/SolidShield Installer Container"),

                // Not much is known about this file, but it seems to be related to what PiD reports as "protection level: Tages BASIC" (Redump entry 85313).
                // Seems to always be found with other KWN files.
                new PathMatchSet(new PathMatch("GAME.KWN", useEndsWith: true), "TAGES (BASIC?)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            string version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // TODO: Determine difference between API and BASIC
            byte typeByte = fileContent[positions[0] + 6];
            byte versionByte = fileContent[positions[0] + 7];

            switch (versionByte)
            {
                case 0x1E:
                    return "5.2";
                case 0x1B:
                    return "5.3-5.4";
                case 0x14:
                    return "5.5.0";
                case 0x04:
                    return "5.5.2";
                default:
                    return string.Empty;
            }
        }
    }
}
