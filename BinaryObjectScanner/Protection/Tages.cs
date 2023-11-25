using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.ProtectionType
{
    public class TAGES : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
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

            var name = pex.FileDescription;
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
                    new(new byte?[] { 0xE8, 0x75, 0x00, 0x00, 0x00, 0xE8, null, null, 0xFF, 0xFF, 0x68 }, GetVersion, "TAGES"),
                };

                var match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
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
                // So far, only known to exist in early versions of "Moto Racer 3" (Redump entries 31578 and 34669).
                new(new List<PathMatch>
                {
                    // d37f70489207014d7d0fbaa43b081a93e8030498
                    new(Path.Combine("Sys", "Devx.sys").Replace("\\", "/"), useEndsWith: true),

                    // a0acbc2f8e321e4f30c913c095e28af444058249
                    new(Path.Combine("Sys", "VtPr.sys").Replace("\\", "/"), useEndsWith: true),

                    // SHA-1 is variable, file size is 81,920 bytes
                    new FilePathMatch("Wave.aif"),

                    // f82339d797be6da92f5d9dadeae9025385159057
                    new FilePathMatch("Wave.alf"),

                    // 0351d0f3d4166362a1a9d838c9390a3d92945a44
                    new FilePathMatch("Wave.apt"),

                    // SHA-1 is variable, file size is 61,440 bytes
                    new FilePathMatch("Wave.axt"),
                }, "TAGES"),

                // Currently only found in "Robocop" (Redump entry 35932).
                // Found in a directory named "System", with an executable named "Setup.exe".
                new(new List<PathMatch>
                {
                    // f82339d797be6da92f5d9dadeae9025385159057
                    new(Path.Combine("9x", "Tamlx.alf").Replace("\\", "/"), useEndsWith: true),

                    // 933c004d3043863f019f5ffaf63402a30e65026c
                    new(Path.Combine("9x", "Tamlx.apt").Replace("\\", "/"), useEndsWith: true),

                    // d45745fa6b0d23fe0ee12e330ab85d5bf4e0e776
                    new(Path.Combine("NT", "enodpl.sys").Replace("\\", "/"), useEndsWith: true),

                    // f111eba05ca6e9061c557547420847d7fdee657d
                    new(Path.Combine("NT", "litdpl.sys").Replace("\\", "/"), useEndsWith: true),
                }, "TAGES"),

                // Currently only known to exist in "XIII" and "Beyond Good & Evil" (Redump entries 8774-8776, 45940-45941, 18690-18693, and presumably 21320, 21321, 21323, and 36124).
                new(new List<PathMatch>
                {
                    new FilePathMatch("enodpl.sys"),
                    new FilePathMatch("ENODPL.VXD"),
                    new FilePathMatch("tandpl.sys"),
                    new FilePathMatch("TANDPL.VXD"),
                }, "TAGES"),

                // The directory of these files has been seen to be named two different things, with two different accompanying executables in the root of the directory.
                // In the example where the directory is named "Drivers", the executable is named "Silent.exe" (Redump entry 51763).
                // In the example where the directory is named "ELBdrivers", the executable is name "ELBDrivers.exe" (Redump entry 91090).
                // The name and file size of the included executable vary, but there should always be one here.
                new(new List<PathMatch>
                {
                    // 40826e95f3ad8031b6debe15aca052c701288e04
                    new(Path.Combine("9x", "hwpsgt.vxd").Replace("\\", "/"), useEndsWith: true),

                    // f82339d797be6da92f5d9dadeae9025385159057
                    new(Path.Combine("9x", "lemsgt.vxd").Replace("\\", "/"), useEndsWith: true),

                    // 43f407ecdc0d87a3713126b757ccaad07ade285f
                    new(Path.Combine("NT", "hwpsgt.sys").Replace("\\", "/"), useEndsWith: true),

                    // 548dd6359abbcc8c84ce346d078664eeedc716f7
                    new(Path.Combine("NT", "lemsgt.sys").Replace("\\", "/"), useEndsWith: true),
                }, "TAGES"),

                // The following files are supposed to only be found inside the driver setup executables, and are present in at least version 5.2.0.1 (Redump entry 15976).
                new(new FilePathMatch("ithsgt.sys"), "TAGES Driver"),
                new(new FilePathMatch("lilsgt.sys"), "TAGES Driver"),

                // The following files are supposed to only be found inside the driver setup executables in versions 5.5.0.1+.
                new(new FilePathMatch("atksgt.sys"), "TAGES Driver"),
                new(new FilePathMatch("lirsgt.sys"), "TAGES Driver"),

                // The following files appear to be container formats for TAGES, but little is currently known about them (Redump entries 85313 and 85315).
                new(new FilePathMatch("GameModule.elb"), "TAGES/SolidShield Game Executable Container"),
                new(new FilePathMatch("InstallModule.elb"), "TAGES/SolidShield Installer Container"),

                // Not much is known about this file, but it seems to be related to what PiD reports as "protection level: Tages BASIC" (Redump entry 85313).
                // Seems to always be found with other KWN files.
                new(new FilePathMatch("GAME.KWN"), "TAGES (BASIC?)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far, only known to exist in early versions of "Moto Racer 3" (Redump entries 31578 and 34669).
                new(new FilePathMatch("Devx.sys"), "TAGES Driver"),
                new(new FilePathMatch("VtPr.sys"), "TAGES Driver"),

                // These are removed because they can potentially overmatch
                // new(new FilePathMatch("Wave.aif"), "TAGES Driver"),
                // new(new FilePathMatch("Wave.alf"), "TAGES Driver"),
                // new(new FilePathMatch("Wave.apt"), "TAGES Driver"),
                // new(new FilePathMatch("Wave.axt"), "TAGES Driver"),

                // Currently only found in "Robocop" (Redump entry 35932).
                // Found in a directory named "System", with an executable named "Setup.exe".
                new(new FilePathMatch("Tamlx.apt"), "TAGES 9x Driver"),
                new(new FilePathMatch("Tamlx.alf"), "TAGES 9x Driver"),
                new(new FilePathMatch("enodpl.sys"), "TAGES NT Driver"),
                new(new FilePathMatch("litdpl.sys"), "TAGES NT Driver"),

                // Currently only known to exist in "XIII" and "Beyond Good & Evil" (Redump entries 8774-8776, 45940-45941, 18690-18693, and presumably 21320, 21321, 21323, and 36124).
                new(new FilePathMatch("enodpl.sys"), "TAGES NT Driver"),
                new(new FilePathMatch("ENODPL.VXD"), "TAGES 9x Driver"),
                new(new FilePathMatch("tandpl.sys"), "TAGES NT Driver"),
                new(new FilePathMatch("TANDPL.VXD"), "TAGES 9x Driver"),

                // The directory of these files has been seen to be named two different things, with two different accompanying executables in the root of the directory.
                // In the example where the directory is named "Drivers", the executable is named "Silent.exe" (Redump entry 51763).
                // In the example where the directory is named "ELBdrivers", the executable is name "ELBDrivers.exe" (Redump entry 91090).
                // The name and file size of the included executable vary, but there should always be one here.
                new(new FilePathMatch("hwpsgt.vxd"), "TAGES 9x Driver"),
                new(new FilePathMatch("lemsgt.vxd"), "TAGES 9x Driver"),
                new(new FilePathMatch("hwpsgt.sys"), "TAGES NT Driver"),
                new(new FilePathMatch("lemsgt.sys"), "TAGES NT Driver"),

                // The following files are supposed to only be found inside the driver setup executables, and are present in at least version 5.2.0.1 (Redump entry 15976).
                new(new FilePathMatch("ithsgt.sys"), "TAGES Driver"),
                new(new FilePathMatch("lilsgt.sys"), "TAGES Driver"),

                // The following files are supposed to only be found inside the driver setup executables in versions 5.5.0.1+.
                new(new FilePathMatch("atksgt.sys"), "TAGES Driver"),
                new(new FilePathMatch("lirsgt.sys"), "TAGES Driver"),

                // The following files appear to be container formats for TAGES, but little is currently known about them (Redump entries 85313 and 85315).
                new(new FilePathMatch("GameModule.elb"), "TAGES/SolidShield Game Executable Container"),
                new(new FilePathMatch("InstallModule.elb"), "TAGES/SolidShield Installer Container"),

                // Not much is known about this file, but it seems to be related to what PiD reports as "protection level: Tages BASIC" (Redump entry 85313).
                // Seems to always be found with other KWN files.
                new(new FilePathMatch("GAME.KWN"), "TAGES (BASIC?)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            var version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version!;

            return "(Unknown Version)";
        }

        public static string? GetVersion(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            // TODO: Determine difference between API and BASIC
            byte typeByte = fileContent[positions[0] + 6];
            byte versionByte = fileContent[positions[0] + 7];

            return versionByte switch
            {
                0x1E => "5.2",
                0x1B => "5.3-5.4",
                0x14 => "5.5.0",
                0x04 => "5.5.2",
                _ => string.Empty,
            };
        }
    }
}
