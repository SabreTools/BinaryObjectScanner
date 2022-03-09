using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to use path check framework here
    public class TAGES : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            /* 
                Expected files to contain "TagesSetup"/"Application TagesSetup":
                DrvSetup.exe
                DrvSetup_x64.exe
                TagesSetup.exe
                TagesSetup_x64.exe
             
                Expected files to contain "Tages activation client"/"T@GES":
                TagesClient.exe
                (There is generally a TagesClient.dat accompanying the TagesClient.exe.)
            */
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

            // TODO: Obtain a sample to find where this string is in a typical executable
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
        
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<string>
                {
                    /*
                    So far, only known to exist in early versions of "Moto Racer 3".

                    Expected information about these checks:
                    ./sys/Devx.sys (d37f70489207014d7d0fbaa43b081a93e8030498)
                    ./sys/VtPr.sys (a0acbc2f8e321e4f30c913c095e28af444058249)
                    ./Wave.aif (SHA-1 is variable, file size is 81,920 bytes)
                    ./Wave.alf (f82339d797be6da92f5d9dadeae9025385159057)
                    ./Wave.apt (0351d0f3d4166362a1a9d838c9390a3d92945a44)
                    ./Wave.axt (SHA-1 is variable, file size is 61,440 bytes)
                    */
                    Path.Combine("Sys", "Devx.sys").Replace("\\", "/"),
                    Path.Combine("Sys", "VtPr.sys").Replace("\\", "/"),
                    Path.Combine("Wave.aif").Replace("\\", "/"),
                    Path.Combine("Wave.alf").Replace("\\", "/"),
                    Path.Combine("Wave.apt").Replace("\\", "/"),
                    Path.Combine("Wave.axt").Replace("\\", "/"),
                }, "TAGES"),

                new PathMatchSet(new List<string>
                {
                    /*
                    Currently only known to exist in "XIII" and (presumably) "Beyond Good & Evil".
                    */
                    Path.Combine("enodpl.sys").Replace("\\", "/"),
                    Path.Combine("ENODPL.VXD").Replace("\\", "/"),
                    Path.Combine("tandpl.sys").Replace("\\", "/"),
                    Path.Combine("TANDPL.VXD").Replace("\\", "/"),
                }, "TAGES"),

                new PathMatchSet(new List<string>
                {
                    /*
                    The directory of these files has been seen to be named two different things, with two different accompanying executables in the root of the directory.
                    In the example where the directory is named "Drivers", the executable is named "Silent.exe".
                    In the example where the directory is named "ELBdrivers", the executable is name "ELBDrivers.exe".

                    Expected information about these checks:
                    ./9x/hwpsgt.vxd (40826e95f3ad8031b6debe15aca052c701288e04)
                    ./9x/lemsgt.vxd (f82339d797be6da92f5d9dadeae9025385159057)
                    ./NT/hwpsgt.sys (43f407ecdc0d87a3713126b757ccaad07ade285f)
                    ./NT/lemsgt.sys (548dd6359abbcc8c84ce346d078664eeedc716f7)
                    (The name and file size of the included executable vary, but there should always be one here.)
                    */
                    Path.Combine("9x", "hwpsgt.vxd").Replace("\\", "/"),
                    Path.Combine("9x", "lemsgt.vxd").Replace("\\", "/"),
                    Path.Combine("NT", "hwpsgt.sys").Replace("\\", "/"),
                    Path.Combine("NT", "lemsgt.sys").Replace("\\", "/"),
                }, "TAGES"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("Devx.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("VtPr.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("hwpsgt.vxd", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("lemsgt.vxd", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("hwpsgt.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("lemsgt.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }

            // The following files are supposed to only be found inside the driver setup executables.
            else if (Path.GetFileName(path).Equals("ithsgt.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("lilsgt.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("atksgt.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }
            else if (Path.GetFileName(path).Equals("lirsgt.sys", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Driver";
            }

            // The following files appear to be container formats for TAGES, but little is currently known about them.
            else if (Path.GetFileName(path).Equals("GameModule.elb", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES/SolidShield Game Executable Container";
            }
            else if (Path.GetFileName(path).Equals("InstallModule.elb", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES/SolidShield Installer Container";
            }

            // Not much is known about this file, but it seems to be related to what PiD reports as "protection level: Tages BASIC". Seems to always be found with other KWN files.
            else if (Path.GetFileName(path).Equals("GAME.KWN", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES (BASIC?)";
            }

            return null;
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
