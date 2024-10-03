using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Rainbow Technologies Sentinel (https://www.rainbow.com.my) is a family of DRM products.
    /// Rainbow Sentinel SuperPro: https://www.rainbow.com.my/superpro.php
    /// TODO: Investigate other versions/products.
    /// TODO: See if this is at all related to https://cpl.thalesgroup.com/software-monetization/all-products/sentinel-hl.
    /// TODO: Investigate the possible integration between FlexLM and Rainbow Sentinel in IA item "prog-17_202403".
    /// TODO: Investigate the "NetSentinel Protection System" found in "NSRVOM.EXE" and "NSRVGX.EXE" in IA item "czchip199707cd".
    /// TODO: Investigate "sntnlusb.sys" (https://www.rainbow.com.my/document/endusertroubleshooting.pdf).
    /// 
    /// Versions: 
    /// Rainbow Sentinel PD-5.1: IA items "pcwkcd-1296, "CHIPTRMart97", and "bugcd199801".
    /// Rainbow Sentinel PD-5.1e (Beta): IA item "CHIPTRMart97".
    /// Rainbow Sentinel PD-5.37: File "CICA 32 For Windows CD-ROM (Walnut Creek) (October 1999) (Disc 4).iso" in IA item "CICA_32_For_Windows_CD-ROM_Walnut_Creek_October_1999".
    /// Rainbow Sentinel PD-5.39: IA item "chip-cds-2001-08".
    /// Rainbow Sentinel PD-15: IA items "ASMEsMechanicalEngineeringToolkit1997December", "aplicaciones-windows", and "ASMEsMechanicalEngineeringToolkit1997December".
    /// Rainbow Sentinel PD-17: IA item "czchip199707cd".
    /// Rainbow Sentinel PD-30: BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
    /// Rainbow Sentinel PD-31: BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
    /// 
    /// Rainbow Sentinel SuperPro 5.0: IA items "chip-cds-2001-08".
    /// Rainbow Sentinel SuperPro 5.1: IA items "ASMEsMechanicalEngineeringToolkit1997December" and "aplicaciones-windows".
    /// 
    /// Rainbow SentinelPro 5.1: IA item "pcwkcd-1296".
    /// 
    /// Rainbow NetSentinel: IA item "czchip199707cd".
    /// </summary>
    public class RainbowSentinel : IPathCheck, INewExecutableCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // TODO: Don't read entire file
            var data = nex.ReadArbitraryRange();
            if (data == null)
                return null;

            // TODO: Figure out what NE section this lives in
            var neMatchSets = new List<ContentMatchSet>
            {
                // SentinelPro Windows Driver DLL
                // Found in "SSWIN.dll" in IA item "pcwkcd-1296".
                new(new byte?[]
                {
                    0x53, 0x65, 0x6E, 0x74, 0x69, 0x6E, 0x65, 0x6C,
                    0x50, 0x72, 0x6F, 0x20, 0x57, 0x69, 0x6E, 0x64,
                    0x6F, 0x77, 0x73, 0x20, 0x44, 0x72, 0x69, 0x76,
                    0x65, 0x72, 0x20, 0x44, 0x4C, 0x4C
                }, "Rainbow SentinelPro"),

                // Sentinel Device Driver Version �PD-5.17
                // Found in "SENTINEL.SYS" in IA item "czchip199707cd".
                new(new byte?[]
                {
                    0x53, 0x65, 0x6E, 0x74, 0x69, 0x6E, 0x65, 0x6C,
                    0x20, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x20,
                    0x44, 0x72, 0x69, 0x76, 0x65, 0x72, 0x20, 0x56,
                    0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x20, 0x00,
                    0x50, 0x44, 0x2D, 0x35, 0x2E, 0x31, 0x37
                }, "Rainbow Sentinel PD-5.17"),
                
                // NetSentinel OS/2 security server
                // Found in "NSRVOM.EXE" in IA item "czchip199707cd".
                new(new byte?[]
                {
                    0x4E, 0x65, 0x74, 0x53, 0x65, 0x6E, 0x74, 0x69,
                    0x6E, 0x65, 0x6C, 0x20, 0x4F, 0x53, 0x2F, 0x32,
                    0x20, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72
                }, "Rainbow NetSentinel Server for OS/2"),
                
                // NetSentinel  Monitor
                // Found in "OS2MON.EXE" in IA item "czchip199707cd".
                new(new byte?[]
                {
                    0x4E, 0x65, 0x74, 0x53, 0x65, 0x6E, 0x74, 0x69,
                    0x6E, 0x65, 0x6C, 0x20, 0x20, 0x4D, 0x6F, 0x6E,
                    0x69, 0x74, 0x6F, 0x72
                }, "Rainbow NetSentinel Monitor"),

                // Sentinel Device Driver
                // Generic case to catch unknown versions.
                // TODO: Add version parsing for this check.
                new (new byte?[]
                {
                    0x53, 0x65, 0x6E, 0x74, 0x69, 0x6E, 0x65, 0x6C,
                    0x20, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x20,
                    0x44, 0x72, 0x69, 0x76, 0x65, 0x72, 0x20, 0x56,
                    0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x20, 0x00,
                    0x50, 0x44, 0x2D, 0x35, 0x2E, 0x31, 0x37
                }, "Rainbow Sentinel (Unknown Version - Please report this to us on GitHub)"),
            };

            var match = MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
            if (!string.IsNullOrEmpty(match))
                return match;

            // Check the nonresident-name table
            // Found in "SSWIN.dll" in IA item "pcwkcd-1296".
            bool nonresidentNameTableEntries = nex.Model.NonResidentNameTable?
                .Select(nrnte => nrnte?.NameString == null ? string.Empty : Encoding.ASCII.GetString(nrnte.NameString))
                .Any(s => s.Contains("SentinelPro Windows Driver DLL")) ?? false;
            if (nonresidentNameTableEntries)
                return "Rainbow SentinelPro";

            // Found in "INSTALL.EXE" in IA item "czchip199707cd".
            nonresidentNameTableEntries = nex.Model.NonResidentNameTable?
                .Select(nrnte => nrnte?.NameString == null ? string.Empty : Encoding.ASCII.GetString(nrnte.NameString))
                .Any(s => s.Contains("Rainbow Technologies Installation Program")) ?? false;
            if (nonresidentNameTableEntries)
                return "Rainbow Sentinel";

            // Found in "WNCEDITD.EXE" and "WNCEDITO.EXE" in IA item "czchip199707cd".
            nonresidentNameTableEntries = nex.Model.NonResidentNameTable?
                .Select(nrnte => nrnte?.NameString == null ? string.Empty : Encoding.ASCII.GetString(nrnte.NameString))
                .Any(s => s.Contains("NetSentinel-C Editor for Windows")) ?? false;
            if (nonresidentNameTableEntries)
                return "NetSentinel-C Editor for Windows";

            // TODO: Investigate "SentinelScribe Windows Driver DLL" found in "NKWIN.DLL" in IA item "czchip199707cd".

            return null;
        }

        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

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

            // Found in "SP32W.DLL" in IA item "pcwkcd-1296".
            if (name?.Equals("Rainbow Technologies SentinelPro WIN32 DLL", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow SentinelPro {pex.ProductVersion}";

            // Found in "NSRVGX.EXE" in IA item "czchip199707cd".
            if (name?.Equals("NetSentinel Server for WIN 32", StringComparison.OrdinalIgnoreCase) == true)
                return "Rainbow NetSentinel Server for Win32";

            // Found in "\disc4\cad\sdcc_200.zip\DISK1\_USER1.HDR\Language_Independent_Intel_32_Files\SNTNLUSB.SYS" in "CICA 32 For Windows CD-ROM (Walnut Creek) (October 1999) (Disc 4).iso" in IA item "CICA_32_For_Windows_CD-ROM_Walnut_Creek_October_1999".
            // TODO: Check if the version included with this is useful.
            if (name?.Equals("Rainbow Technologies Sentinel Device Driver", StringComparison.OrdinalIgnoreCase) == true)
                return "Rainbow Sentinel Driver";

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

            // Found in "SP32W.DLL" in IA item "pcwkcd-1296".
            if (name?.Equals("Rainbow Technologies SentinelPro WIN32 DLL", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow SentinelPro {pex.ProductVersion}";

            // Found in "F481_SetupSysDriver.exe.B391C18A_6953_11D4_82CB_00D0B72E1DB9"/"SetupSysDriver.exe" in IA item "chip-cds-2001-08".
            if (name?.Equals("Sentinel System Driver", StringComparison.OrdinalIgnoreCase) == true)
                return $"Rainbow Sentinel {pex.ProductVersion}";

            // Found in "\disc4\cad\sdcc_200.zip\DISK1\_USER1.HDR\Language_Independent_Intel_32_Files\SNTNLUSB.SYS" in "CICA 32 For Windows CD-ROM (Walnut Creek) (October 1999) (Disc 4).iso" in IA item "CICA_32_For_Windows_CD-ROM_Walnut_Creek_October_1999".
            // TODO: Check if the version included with this is useful.
            if (name?.Equals("Rainbow Technologies USB Security Device Driver", StringComparison.OrdinalIgnoreCase) == true)
                return "Rainbow Sentinel Driver";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "ADESKSYS.DLL"/"WINADMIN.EXE"/"WINQUERY.EXE" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\netsetup\SUPPORT\IPX".
                if (strs.Any(s => s.Contains("Rainbow SentinelSuperPro")))
                    return "Rainbow Sentinel SuperPro";

                // Found in "SETUPAXP.EXE", "SETUPMPS.EXE", and "SETUPPPC.EXE" in IA item "czchip199707cd".
                if (strs.Any(s => s.Contains("Sentinel Driver Setup Program")))
                    return "Rainbow Sentinel";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                // Found in "SP32W.DLL" in IA item "pcwkcd-1296".
                if (strs.Any(s => s.Contains("SentinelPro WIN32 DLL")))
                    return "Rainbow SentinelPro";

                // Found in "NKWIN32.DLL" in IA item "czchip199707cd".
                if (strs.Any(s => s.Contains("NetSentinel-C Windows NT Driver DLL")))
                    return "Rainbow NetSentinel-C Windows NT Driver";

                // Found in "NSLMS32.DLL" in IA item "czchip199707cd".
                if (strs.Any(s => s.Contains("NetSentinel 32-Bit Windows DLL")))
                    return "Rainbow NetSentinel Win32 Driver";

                // Found in "W32EDITD.EXE" and "W32EDITO.EXE" in IA item "czchip199707cd".
                if (strs.Any(s => s.Contains("NetSentinel-C Editor for Windows")))
                    return "NetSentinel-C Editor for Win32";

                // Generic case to catch undetected versions.
                if (strs.Any(s => s.Contains("SentinelPro")))
                    return "Rainbow SentinelPro (Unknown Version - Please report to us on GitHub)";
            }

            // Get the .rsrc section strings, if they exist
            strs = pex.GetFirstSectionStrings(".rsrc");
            if (strs != null)
            {
                // Found in "WINMON.exe" in IA item "czchip199707cd".
                if (strs.Any(s => s.Contains("NetSentinel Monitor")))
                    return "Rainbow NetSentinel Monitor";
            }

            // Get the .text section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                // Found in "ACLT.HWL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\aclt\DRV\W95LOCK".
                // Found in "ACAD.HWL" in BA entry "Autodesk AutoCAD r14 (1997)" and IA item "auto-cad-r14-cdrom".
                if (strs.Any(s => s.Contains("\\\\.\\SENTINEL.VXD")))
                    return "Rainbow Sentinel";

                // Found in "ADESKSYS.DLL" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]", folder "\netsetup\SUPPORT\IPX".
                // TODO: Investigate "Elan License Manager" mentioned here.
                if (strs.Any(s => s.Contains("Rainbow SentinelSuperPro")))
                    return "Rainbow Sentinel SuperPro";

                // Found in "F1321_dorapro.exe" in IA item "chip-cds-2001-08".
                if (strs.Any(s => s.Contains("modSentinelSuperPro")))
                    return "Rainbow Sentinel SuperPro";

                // Found in "F1321_dorapro.exe" in IA item "chip-cds-2001-08".
                if (strs.Any(s => s.Contains("clsSentinelSuperPro")))
                    return "Rainbow Sentinel SuperPro";

                // Found in "SENTSTRT.EXE" in IA item "czchip199707cd".
                if (strs.Any(s => s.Contains("Sentinel Driver Startup Program")))
                    return "Rainbow Sentinel";

                // Found in "SETUPX86.EXE" in IA item "czchip199707cd".
                if (strs.Any(s => s.Contains("Sentinel Windows NT Driver Setup")))
                    return "Rainbow Sentinel";
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
                // The Parallel Port driver for Rainbow Sentinel on Win9x (https://www.rainbow.com.my/document/endusertroubleshooting.pdf).
                // Unfortunately, the file name overlaps with a file used by Clam Sentinel (https://clamsentinel.sourceforge.net/).
                // new(new FilePathMatch("SENTINEL.VXD"), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
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

                 // Found in IA item "pcwkcd-1296".
                 new(new FilePathMatch("SP32W.DLL"), "Rainbow Sentinel"),
                 new(new FilePathMatch("SSWIN.DLL"), "Rainbow Sentinel"),

                 // Found in IA item "czchip199707cd".
                 new(new FilePathMatch("SENTINEL.DPP"), "Rainbow Sentinel OS/2 Installation Script"),
                 new(new FilePathMatch("SENTDOS.SYS"), "Rainbow Sentinel DOS Driver"),
                 new(new FilePathMatch("SENTINEL.386"), "Rainbow Sentinel Windows 3.1 Driver"),
                 new(new FilePathMatch("SNTALPHA.DLL"), "Rainbow Sentinel Windows NT Alpha Platform Driver"),
                 new(new FilePathMatch("SNTI386.DLL"), "Rainbow Sentinel Windows NT Intel Platform Driver"),
                 new(new FilePathMatch("SNTMIPS.DLL"), "Rainbow Sentinel Windows NT MIPS Platform Driver"),
                 new(new FilePathMatch("SNTPPC.DLL"), "Rainbow Sentinel Windows NT PowerPC Platform Driver"),
                 new(new FilePathMatch("NSRVDI.EXE"), "Rainbow NetSentinel Server for DOS"),
                 new(new FilePathMatch("NSRVDN.EXE"), "Rainbow NetSentinel Server for DOS"),
                 new(new FilePathMatch("NSRVNI.NLM"), "Rainbow NetSentinel Server for Novell NetWare"),
                 new(new FilePathMatch("NSRVOM.EXE"), "Rainbow NetSentinel Server for OS/2"),
                 new(new FilePathMatch("NSRVGX.EXE"), "Rainbow NetSentinel Server for Win32"),

                 // Found in "\disc4\cad\sdcc_200.zip\DISK1\_USER1.HDR\Language_Independent_Intel_32_Files" in "CICA 32 For Windows CD-ROM (Walnut Creek) (October 1999) (Disc 4).iso" in IA item "CICA_32_For_Windows_CD-ROM_Walnut_Creek_October_1999".
                 // TODO: Add text file checks for these IFX files.
                 new(new FilePathMatch("SNTNLUSB.IFX"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTNLUSB.INF"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTNLUSB.SYS"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTUSB95.IFX"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTUSB95.INF"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTUSB95.SYS"), "Rainbow Sentinel USB Driver"),

                 // Found in IA item "czchip199707cd".
                 new(new List<PathMatch>
                 {
                     new FilePathMatch("DOSMON.EXE"),
                     new FilePathMatch("FIND.EXE"),
                     new FilePathMatch("NCEDIT.EXE"),
                     new FilePathMatch("NETEVAL.EXE"),
                 }, "Rainbow NetSentinel Monitor for DOS"),

                 // Found in IA item "czchip199707cd".
                 new(new List<PathMatch>
                 {
                     new FilePathMatch("OS2MON.EXE"),
                     new FilePathMatch("RHPANELP.DLL"),
                 }, "Rainbow NetSentinel Monitor for OS/2"),

                 // Found in IA item "czchip199707cd".
                 new(new List<PathMatch>
                 {
                     new FilePathMatch("MAPFILE.TXT"),
                     new FilePathMatch("NKWIN32.DLL"),
                     new FilePathMatch("NSLMS32.DLL"),
                     new FilePathMatch("W32EDITD.EXE"),
                     new FilePathMatch("W32EDITO.EXE"),
                     new FilePathMatch("WINMON.DOC"),
                     new FilePathMatch("WINMON.EXE"),
                     new FilePathMatch("WINMON.HLP"),
                     new FilePathMatch("WMON_DOC.EXE"),
                 }, "Rainbow NetSentinel Monitor for Win32"),

                 // Found in IA item "chip-cds-2001-08".
                 // File names for Rainbow Sentinel files sometimes found in ".cab" files.
                 new(new FilePathMatch("F194_rnbovdd.dll.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F195_sentinel.sys.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F225_sentinel.hlp.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F227_snti386.dll.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F288_sentinel.vxd.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F317_sentstrt.exe.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F344_sentw9x.hlp.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F481_SetupSysDriver.exe.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F766_SentinelDriverInstall_Start.htm.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // The Parallel Port driver for Rainbow Sentinel (https://www.rainbow.com.my/document/endusertroubleshooting.pdf).
                // Unforutnately, the file name overlaps with a file used by Clam Sentinel (https://clamsentinel.sourceforge.net/).
                // TODO: Add LE check for "SENTINEL.VXD" once LE checks are implemented. 
                // new(new FilePathMatch("SENTINEL.VXD"), "Rainbow Sentinel"),

                // Found in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]" and IA item "auto-cad-r14-cdrom".
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

                 // Found in IA item "pcwkcd-1296".
                 new(new FilePathMatch("SP32W.DLL"), "Rainbow Sentinel"),
                 new(new FilePathMatch("SSWIN.DLL"), "Rainbow Sentinel"),

                 // Found in IA item "czchip199707cd".
                 new(new FilePathMatch("SENTINEL.DPP"), "Rainbow Sentinel OS/2 Installation Script"),
                 new(new FilePathMatch("SENTDOS.SYS"), "Rainbow Sentinel DOS Driver"),
                 new(new FilePathMatch("SENTINEL.386"), "Rainbow Sentinel Windows 3.1 Driver"),
                 new(new FilePathMatch("SNTALPHA.DLL"), "Rainbow Sentinel Windows NT Alpha Platform Driver"),
                 new(new FilePathMatch("SNTI386.DLL"), "Rainbow Sentinel Windows NT Intel Platform Driver"),
                 new(new FilePathMatch("SNTMIPS.DLL"), "Rainbow Sentinel Windows NT MIPS Platform Driver"),
                 new(new FilePathMatch("SNTPPC.DLL"), "Rainbow Sentinel Windows NT PowerPC Platform Driver"),

                 // Found in "\disc4\cad\sdcc_200.zip\DISK1\_USER1.HDR\Language_Independent_Intel_32_Files" in "CICA 32 For Windows CD-ROM (Walnut Creek) (October 1999) (Disc 4).iso" in IA item "CICA_32_For_Windows_CD-ROM_Walnut_Creek_October_1999".
                 // TODO: Add text file checks for these IFX files.
                 new(new FilePathMatch("SNTNLUSB.IFX"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTNLUSB.INF"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTNLUSB.SYS"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTUSB95.IFX"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTUSB95.INF"), "Rainbow Sentinel USB Driver"),
                 new(new FilePathMatch("SNTUSB95.SYS"), "Rainbow Sentinel USB Driver"),

                 // Found in IA item "chip-cds-2001-08".
                 // File names for Rainbow Sentinel files sometimes found in ".cab" files.
                 new(new FilePathMatch("F194_rnbovdd.dll.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F195_sentinel.sys.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F225_sentinel.hlp.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F227_snti386.dll.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F288_sentinel.vxd.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F317_sentstrt.exe.B391C188_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F344_sentw9x.hlp.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F481_SetupSysDriver.exe.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
                 new(new FilePathMatch("F766_SentinelDriverInstall_Start.htm.B391C18A_6953_11D4_82CB_00D0B72E1DB9"), "Rainbow Sentinel"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
