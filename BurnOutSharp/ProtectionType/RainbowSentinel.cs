using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Rainbow Technologies Sentinel (https://www.rainbow.com.my) is a family of DRM products.
    /// Rainbow Sentinel SuperPro: https://www.rainbow.com.my/superpro.php
    /// TODO: Investigate other versions/products.
    /// TODO: See if this is at all related to https://cpl.thalesgroup.com/software-monetization/all-products/sentinel-hl.
    /// </summary>
    public class RainbowSentinel : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Rainbow SentinelSuperPro
                    // Found in "ADESKSYS.DLL"/"WINADMIN.EXE"/"WINQUERY.EXE" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\netsetup\SUPPORT\IPX".
                    new ContentMatchSet(new byte?[]
                    {
                        0x52, 0x61, 0x69, 0x6E, 0x62, 0x6F, 0x77, 0x20,
                        0x53, 0x65, 0x6E, 0x74, 0x69, 0x6E, 0x65, 0x6C,
                        0x53, 0x75, 0x70, 0x65, 0x72, 0x50, 0x72, 0x6F
                    }, "Rainbow Sentinel SuperPro"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .text section, if it exists
            if (pex.TextSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // SENTINEL.VXD
                    // Found in "ACLT.HWL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\W95LOCK".
                    // TODO: Add version for this check based on PEX Product Version.
                    new ContentMatchSet(new byte?[]
                    {
                        0x53, 0x45, 0x4E, 0x54, 0x49, 0x4E, 0x45, 0x4C, 0x2E, 0x56, 0x58, 0x44
                    }, "Rainbow Sentinel"),

                    // Rainbow SentinelSuperPro
                    // Found in "ADESKSYS.DLL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\netsetup\SUPPORT\IPX".
                    // TODO: Investigate "Elan License Manager" mentioned here.
                    new ContentMatchSet(new byte?[]
                    {
                        0x52, 0x61, 0x69, 0x6E, 0x62, 0x6F, 0x77, 0x20, 
                        0x53, 0x65, 0x6E, 0x74, 0x69, 0x6E, 0x65, 0x6C,
                        0x53, 0x75, 0x70, 0x65, 0x72, 0x50, 0x72, 0x6F
                    }, "Rainbow Sentinel SuperPro"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.TextSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // TODO: Figure out why resources for "RNBOVTMP.DLL", "SENTTEMP.DLL", and "SNTI386.DLL" aren't getting read properly, causing checks for these files to not work.

            string name = pex.FileDescription;

            // Found in "RNBOVTMP.DLL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
            if (name?.Equals("Rainbow Technologies Virtual Device Driver", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            // Found in "SENTTEMP.DLL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
            if (name?.Equals("Rainbow Technologies Sentinel Driver", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            // Found in "SETUPX86.EXE"/"SENTW95.EXE" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
            if (name?.Equals("Sentinel Driver Setup DLL", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            // Found in "SNTI386.DLL"/"SENTW95.DLL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
            if (name?.Equals("Install, Setup - Sentinel Driver", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            name = pex.ProductName;

            // Found in multiple files i, BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", including "RNBOVTMP.DLL", "SENTTEMP.DLL", and "SNTI386.DLL".
            if (name?.Equals("Rainbow Technologies Sentinel", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            // Found in "SETUPX86.EXE"/"SENTW95.EXE" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
            if (name?.Equals("Sentinel Driver Setup", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\W95LOCK".
                new PathMatchSet(new PathMatch("SENTINEL.VXD", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTSTRT.EXE", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTW95.DLL", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTW95.EXE", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTW95.HLP", useEndsWith: true), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\NTLOCK".
                new PathMatchSet(new PathMatch("SNTI386.DLL", useEndsWith: true), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\NTLOCK\I386".
                new PathMatchSet(new PathMatch("RNBOVTMP.DLL", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTINEL.HLP", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTTEMP.SYS", useEndsWith: true), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\data".
                new PathMatchSet(new PathMatch("RAINB95.Z", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("RAINBNT.Z", useEndsWith: true), "Rainbow Sentinel"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\W95LOCK".
                new PathMatchSet(new PathMatch("SENTINEL.VXD", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTSTRT.EXE", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTW95.DLL", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTW95.EXE", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTW95.HLP", useEndsWith: true), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\NTLOCK".
                new PathMatchSet(new PathMatch("SNTI386.DLL", useEndsWith: true), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\NTLOCK\I386".
                new PathMatchSet(new PathMatch("RNBOVTMP.DLL", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTINEL.HLP", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("SENTTEMP.SYS", useEndsWith: true), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\data".
                new PathMatchSet(new PathMatch("RAINB95.Z", useEndsWith: true), "Rainbow Sentinel"),
                new PathMatchSet(new PathMatch("RAINBNT.Z", useEndsWith: true), "Rainbow Sentinel"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
