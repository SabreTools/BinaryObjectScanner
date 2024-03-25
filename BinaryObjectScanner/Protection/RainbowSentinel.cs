using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Rainbow Technologies Sentinel (https://www.rainbow.com.my) is a family of DRM products.
    /// Rainbow Sentinel SuperPro: https://www.rainbow.com.my/superpro.php
    /// TODO: Investigate other versions/products.
    /// TODO: See if this is at all related to https://cpl.thalesgroup.com/software-monetization/all-products/sentinel-hl.
    /// TODO: Investigate the possible integration between FlexLM and Rainbow Sentinel in IA item "prog-17_202403".
    /// 
    /// Versions: 
    /// Rainbow Sentinel PD-15: IA items "ASMEsMechanicalEngineeringToolkit1997December" and "aplicaciones-windows".
    /// Rainbow Sentinel PD-30: BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
    /// Rainbow Sentinel PD-31: BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
    /// 
    /// Rainbow Sentinel SuperPro 5.1: IA items "ASMEsMechanicalEngineeringToolkit1997December" and "aplicaciones-windows".
    /// </summary>
    public class RainbowSentinel : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "ADESKSYS.DLL"/"WINADMIN.EXE"/"WINQUERY.EXE" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\netsetup\SUPPORT\IPX".
                if (strs.Any(s => s.Contains("Rainbow SentinelSuperPro")))
                    return "Rainbow Sentinel SuperPro";
            }

            // Get the .text section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                // Found in "ACLT.HWL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\W95LOCK".
                // Found in "ACAD.HWL" in BA entry "Autodesk AutoCAD r14 (1997)" and IA item "auto-cad-r14-cdrom".
                if (strs.Any(s => s.Contains("SENTINEL.VXD")))
                    return "Rainbow Sentinel SuperPro";

                // Found in "ADESKSYS.DLL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\netsetup\SUPPORT\IPX".
                // TODO: Investigate "Elan License Manager" mentioned here.
                if (strs.Any(s => s.Contains("Rainbow SentinelSuperPro")))
                    return "Rainbow Sentinel SuperPro";
            }

            // TODO: Figure out why resources for "RNBOVTMP.DLL", "SENTTEMP.DLL", "SNTI386.DLL", and "SX32W.DL_"/"SX32W.DLL" aren't getting read properly, causing checks for these files to not work.

            var name = pex.FileDescription;

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

            // Found in "wd126.zip/WDSHARE.EXE/SX32W.DL_" in IA item "ASMEsMechanicalEngineeringToolkit1997December" and "WDSHARE.ZIP/WDSHARE.EXE/SX32W.DL_" in IA item "aplicaciones-windows".
            if (name?.Equals("Rainbow Technologies SentinelSuperPro WIN32 DLL", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel SuperPro {pex.ProductVersion}";

            name = pex.ProductName;

            // Found in multiple files in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", including "RNBOVTMP.DLL", "SENTTEMP.DLL", and "SNTI386.DLL".
            if (name?.Equals("Rainbow Technologies Sentinel", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            // Found in "SETUPX86.EXE"/"SENTW95.EXE" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
            if (name?.Equals("Sentinel Driver Setup", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            // Found in "wd126.zip/WDSHARE.EXE/SX32W.DL_" in IA item "ASMEsMechanicalEngineeringToolkit1997December" and "WDSHARE.ZIP/WDSHARE.EXE/SX32W.DL_" in IA item "aplicaciones-windows".
            if (name?.Equals("Rainbow Technologies SentinelSuperPro WIN32 DLL", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel SuperPro {pex.ProductVersion}";

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
                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("SENTINEL.VXD"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTSTRT.EXE"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTW95.DLL"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTW95.EXE"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTW95.HLP"), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and in IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("SNTI386.DLL"), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and in IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("RNBOVTMP.DLL"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTINEL.HLP"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTTEMP.SYS"), "Rainbow Sentinel"),

                // Found in BA entries "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and "Autodesk AutoCAD r14 (1997)", and IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("RAINB95.Z"), "Rainbow Sentinel"),
                new(new FilePathMatch("RAINBNT.Z"), "Rainbow Sentinel"),

                // Found in "wd126.zip/WDSHARE.EXE" in IA item "ASMEsMechanicalEngineeringToolkit1997December" and "WDSHARE.ZIP/WDSHARE.EXE/SX32W.DL_" in IA item "aplicaciones-windows".
                 new(new FilePathMatch("RainbowSentinel.386"), "Rainbow Sentinel"),
                 new(new FilePathMatch("SX32W.DL_"), "Rainbow Sentinel"),
                 new(new FilePathMatch("SX32W.DLL"), "Rainbow Sentinel"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("SENTINEL.VXD"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTSTRT.EXE"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTW95.DLL"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTW95.EXE"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTW95.HLP"), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and in IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("SNTI386.DLL"), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and in IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("RNBOVTMP.DLL"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTINEL.HLP"), "Rainbow Sentinel"),
                new(new FilePathMatch("SENTTEMP.SYS"), "Rainbow Sentinel"),

                // Found in BA entries "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and "Autodesk AutoCAD r14 (1997)", and IA item "auto-cad-r14-cdrom".
                new(new FilePathMatch("RAINB95.Z"), "Rainbow Sentinel"),
                new(new FilePathMatch("RAINBNT.Z"), "Rainbow Sentinel"),

                // Found in "wd126.zip/WDSHARE.EXE" in IA item "ASMEsMechanicalEngineeringToolkit1997December" and "WDSHARE.ZIP/WDSHARE.EXE/SX32W.DL_" in IA item "aplicaciones-windows".
                 new(new FilePathMatch("RainbowSentinel.386"), "Rainbow Sentinel"),
                 new(new FilePathMatch("SX32W.DL_"), "Rainbow Sentinel"),
                 new(new FilePathMatch("SX32W.DLL"), "Rainbow Sentinel"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
