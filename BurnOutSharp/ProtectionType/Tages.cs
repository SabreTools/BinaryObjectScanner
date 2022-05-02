using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class TAGES : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
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
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("TagesSetup", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Driver Setup {GetVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Tagès activation client", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Activation Client {GetVersion(pex)}";

            name = pex.ProductName;
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Application TagesSetup", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Driver Setup {GetVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("T@GES", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Activation Client {GetVersion(pex)}";

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // (char)0xE8 + u + (char)0x00 + (char)0x00 + (char)0x00 + (char)0xE8 + ?? + ?? + (char)0xFF + (char)0xFF + "h"
                    new ContentMatchSet(new byte?[] { 0xE8, 0x75, 0x00, 0x00, 0x00, 0xE8, null, null, 0xFF, 0xFF, 0x68 }, GetVersion, "TAGES"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
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
                // So far, only known to exist in early versions of "Moto Racer 3".
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

                // Currently only known to exist in "XIII" and "Beyond Good & Evil".
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("enodpl.sys", useEndsWith: true),
                    new PathMatch("ENODPL.VXD", useEndsWith: true),
                    new PathMatch("tandpl.sys", useEndsWith: true),
                    new PathMatch("TANDPL.VXD", useEndsWith: true),
                }, "TAGES"),

                // The directory of these files has been seen to be named two different things, with two different accompanying executables in the root of the directory.
                // In the example where the directory is named "Drivers", the executable is named "Silent.exe".
                // In the example where the directory is named "ELBdrivers", the executable is name "ELBDrivers.exe".
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

                // The following files are supposed to only be found inside the driver setup executables.
                new PathMatchSet(new PathMatch("ithsgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lilsgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("atksgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lirsgt.sys", useEndsWith: true), "TAGES Driver"),

                // The following files appear to be container formats for TAGES, but little is currently known about them.
                new PathMatchSet(new PathMatch("GameModule.elb", useEndsWith: true), "TAGES/SolidShield Game Executable Container"),
                new PathMatchSet(new PathMatch("InstallModule.elb", useEndsWith: true), "TAGES/SolidShield Installer Container"),

                // Not much is known about this file, but it seems to be related to what PiD reports as "protection level: Tages BASIC".
                // Seems to always be found with other KWN files.
                new PathMatchSet(new PathMatch("GAME.KWN", useEndsWith: true), "TAGES (BASIC?)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far, only known to exist in early versions of "Moto Racer 3".
                new PathMatchSet(new PathMatch("Devx.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("VtPr.sys", useEndsWith: true), "TAGES Driver"),

                // These are removed because they can potentially overmatch
                // new PathMatchSet(new PathMatch("Wave.aif", useEndsWith: true), "TAGES Driver"),
                // new PathMatchSet(new PathMatch("Wave.alf", useEndsWith: true), "TAGES Driver"),
                // new PathMatchSet(new PathMatch("Wave.apt", useEndsWith: true), "TAGES Driver"),
                // new PathMatchSet(new PathMatch("Wave.axt", useEndsWith: true), "TAGES Driver"),

                // Currently only known to exist in "XIII" and "Beyond Good & Evil".
                new PathMatchSet(new PathMatch("enodpl.sys", useEndsWith: true), "TAGES NT Driver"),
                new PathMatchSet(new PathMatch("ENODPL.VXD", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("tandpl.sys", useEndsWith: true), "TAGES NT Driver"),
                new PathMatchSet(new PathMatch("TANDPL.VXD", useEndsWith: true), "TAGES 9x Driver"),

                // The directory of these files has been seen to be named two different things, with two different accompanying executables in the root of the directory.
                // In the example where the directory is named "Drivers", the executable is named "Silent.exe".
                // In the example where the directory is named "ELBdrivers", the executable is name "ELBDrivers.exe".
                // The name and file size of the included executable vary, but there should always be one here.
                new PathMatchSet(new PathMatch("hwpsgt.vxd", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("lemsgt.vxd", useEndsWith: true), "TAGES 9x Driver"),
                new PathMatchSet(new PathMatch("hwpsgt.sys", useEndsWith: true), "TAGES NT Driver"),
                new PathMatchSet(new PathMatch("lemsgt.sys", useEndsWith: true), "TAGES NT Driver"),

                // The following files are supposed to only be found inside the driver setup executables.
                new PathMatchSet(new PathMatch("ithsgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lilsgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("atksgt.sys", useEndsWith: true), "TAGES Driver"),
                new PathMatchSet(new PathMatch("lirsgt.sys", useEndsWith: true), "TAGES Driver"),

                // The following files appear to be container formats for TAGES, but little is currently known about them.
                new PathMatchSet(new PathMatch("GameModule.elb", useEndsWith: true), "TAGES/SolidShield Game Executable Container"),
                new PathMatchSet(new PathMatch("InstallModule.elb", useEndsWith: true), "TAGES/SolidShield Installer Container"),

                // Not much is known about this file, but it seems to be related to what PiD reports as "protection level: Tages BASIC".
                // Seems to always be found with other KWN files.
                new PathMatchSet(new PathMatch("GAME.KWN", useEndsWith: true), "TAGES (BASIC?)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            string version = Utilities.GetInternalVersion(pex);
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
