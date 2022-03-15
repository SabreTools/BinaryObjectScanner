using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class TAGES : IPEContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // protected-tages-runtime.exe
                    new ContentMatchSet(new byte?[]
                    {
                        0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x65,
                        0x64, 0x2D, 0x74, 0x61, 0x67, 0x65, 0x73, 0x2D,
                        0x72, 0x75, 0x6E, 0x74, 0x69, 0x6D, 0x65, 0x2E,
                        0x65, 0x78, 0x65
                    }, Utilities.GetFileVersion, "TAGES [DEBUG]"),

                    // This check seems to currently be broken, as files that appear to have this string aren't being detected.
                    // (char)0xE8 + u + (char)0x00 + (char)0x00 + (char)0x00 + (char)0xE8
                    new ContentMatchSet(new byte?[] { 0xE8, 0x75, 0x00, 0x00, 0x00, 0xE8 }, GetVersion, "TAGES [DEBUG]"),
                };
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
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

            string name = Utilities.GetFileDescription(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("TagesSetup", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Driver Setup {GetVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Tagès activation client", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Activation Client {GetVersion(pex)}";

            name = Utilities.GetProductName(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Application TagesSetup", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Driver Setup {GetVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("T@GES", StringComparison.OrdinalIgnoreCase))
                return $"TAGES Activation Client {GetVersion(pex)}";

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

                // Currently only known to exist in "XIII" and (presumably) "Beyond Good & Evil".
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

                // Currently only known to exist in "XIII" and (presumably) "Beyond Good & Evil".
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
            // Check the file version first
            string version = Utilities.GetFileVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;

            // Then check the manifest version
            version = Utilities.GetManifestVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // (char)0xFF + (char)0xFF + "h"
            if (new ArraySegment<byte>(fileContent, --positions[0] + 8, 3).SequenceEqual(new byte[] { 0xFF, 0xFF, 0x68 })) // TODO: Verify this subtract
                return GetVersion(fileContent, positions[0]);
                
            return null;
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            switch (fileContent[position + 7])
            {
                case 0x1B:
                    return "5.3-5.4";
                case 0x14:
                    return "5.5.0";
                case 0x4:
                    return "5.5.2";
                default:
                    return string.Empty;
            }
        }
    }
}
