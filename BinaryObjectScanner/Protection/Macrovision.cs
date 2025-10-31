using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Macrovision was a company that specialized in various forms of DRM. They had an extensive product line, their most infamous product (within this context) being SafeDisc.
    /// Due to there being a significant amount of backend tech being shared between various protections, a separate class is needed for generic Macrovision detections.
    /// 
    /// Macrovision Corporation CD-ROM Unauthorized Copying Study: https://web.archive.org/web/20011005161810/http://www.macrovision.com/solutions/software/cdrom/images/Games_CD-ROM_Study.PDF
    /// List of trademarks associated with Marovision: https://tmsearch.uspto.gov/bin/showfield?f=toc&state=4804%3Au8wykd.5.1&p_search=searchss&p_L=50&BackReference=&p_plural=yes&p_s_PARA1=&p_tagrepl%7E%3A=PARA1%24LD&expr=PARA1+AND+PARA2&p_s_PARA2=macrovision&p_tagrepl%7E%3A=PARA2%24ALL&p_op_ALL=AND&a_default=search&a_search=Submit+Query&a_search=Submit+Query
    /// </summary>
    public partial class Macrovision : IExecutableCheck<NewExecutable>, IExecutableCheck<PortableExecutable>, IPathCheck, IISOCheck<ISO9660>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, NewExecutable exe, bool includeDebug)
        {
            var resultsList = new List<string>();

            // Run C-Dilla NE checks
            var cDilla = CDillaCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(cDilla))
                resultsList.Add(cDilla!);

            // Run SafeCast NE checks
            var safeCast = SafeCastCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(safeCast))
                resultsList.Add(safeCast!);

            if (resultsList != null && resultsList.Count > 0)
                return string.Join(";", [.. resultsList]);

            return null;
        }

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Check for specific indications for individual Macrovision protections.
            var resultsList = new List<string>();

            // Check for generic indications of Macrovision protections first.
            string? name = exe.FileDescription;

            // Present in "secdrv.sys" files found in SafeDisc 2.80.010+.
            if (name.OptionalEquals("Macrovision SECURITY Driver", StringComparison.OrdinalIgnoreCase))
                resultsList.Add($"Macrovision Security Driver {GetSecDrvExecutableVersion(exe)}");

            // Found in hidden resource of "32bit\Tax02\cdac14ba.dll" in IA item "TurboTax Deluxe Tax Year 2002 for Windows (2.00R)(Intuit)(2002)(352282)".
            // Known versions:
            // 4.16.050 Windows NT 2002/04/24
            if (name.OptionalEquals("Macrovision RTS Service", StringComparison.OrdinalIgnoreCase))
                resultsList.Add($"Macrovision RTS Service {exe.FileVersion}");

            // The stxt371 and stxt774 sections are found in various newer Macrovision products, including various versions of CDS-300, SafeCast, and SafeDisc.
            // A stxt381 section has also been found in the "~df89e9.tmp" file, which is extracted into the Windows temp directory when running Redump entry 42034 on Windows 9x.
            // This file serves an unknown function, as it's only roughly 12 KB in size and consists of mostly empty (00) data.
            // They may indicate SafeWrap, but this hasn't been confirmed yet.
            // Almost every single sample known has both sections, though one only contains the "stxt371" section. It is unknown if this is intentional, or if the game functions without it.
            // It is present in the "Texas HoldEm!" game in "boontybox_PCGamer_DVD.exe" in IA items PC_Gamer_Disc_7.55_July_2005 and cdrom-pcgamercd7.58.
            // Other games in this set also aren't functional despite having the normal layout of stxt sections, and the primary program doesn't install at all due to activation servers being down.
            if (exe.ContainsSection("stxt371", exact: true) || exe.ContainsSection("stxt774", exact: true))
            {
                // Check the header padding for protected sections.
                var sectionMatch = CheckSectionForProtection(file, includeDebug, exe.HeaderPaddingStrings, exe.HeaderPaddingData, true);
                if (sectionMatch != null)
                    resultsList.Add(sectionMatch);

                // Get the .data section, if it exists, for protected sections.
                sectionMatch = CheckSectionForProtection(file, includeDebug, exe.GetFirstSectionStrings(".data"), exe.GetFirstSectionData(".data"), true);
                if (sectionMatch != null)
                    resultsList.Add(sectionMatch!);

                int entryPointIndex = exe.FindEntryPointSectionIndex();
                var entryPointSectionName = exe.SectionNames?[entryPointIndex];

                switch (entryPointSectionName)
                {
                    // Check if the entry point is in the expected section for normal protected executables.
                    // If it isn't, the executable has likely been cracked to remove the protection, or has been corrupted or tampered with and is no longer functional.
                    case "stxt371":
                        resultsList.Add("Macrovision Protected Application");
                        break;
                    // It isn't known if this section ever contains the entry point, so if that does happen, it's worth investigating.
                    case "stxt774":
                        resultsList.Add("Macrovision Protected Application (Report this to us on GitHub)");
                        break;
                    default:
                        resultsList.Add("Macrovision Protected Application (Entry point not present in the stxt371 section. Executable is either unprotected or nonfunctional)");
                        break;
                }
            }

            // If the file doesn't have the stxt* sections, check if any sections are protected assuming it's an older Macrovision product.
            else
            {
                // Check the header padding for protected sections.
                var sectionMatch = CheckSectionForProtection(file, includeDebug, exe.HeaderPaddingStrings, exe.HeaderPaddingData, false);
                if (sectionMatch != null)
                    resultsList.Add(sectionMatch);

                // Check the .data section, if it exists, for protected sections.
                sectionMatch = CheckSectionForProtection(file, includeDebug, exe.GetFirstSectionStrings(".data"), exe.GetFirstSectionData(".data"), false);
                if (sectionMatch != null)
                    resultsList.Add(sectionMatch);
            }

            // Run Cactus Data Shield PE checks
            var match = CactusDataShieldCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run C-Dilla PE checks
            match = CDillaCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run RipGuard PE checks
            match = RipGuardCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run SafeCast PE checks
            match = SafeCastCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run SafeDisc PE checks
            match = SafeDiscCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run FLEXnet PE checks
            match = FLEXnetCheckExecutable(file, exe, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Clean the result list
            resultsList = CleanResultList(resultsList);
            if (resultsList != null && resultsList.Count > 0)
                return string.Join(";", [.. resultsList]);

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var results = new List<string>();

            // Run Macrovision directory checks
            var macrovision = MacrovisionCheckDirectoryPath(path, files);
            if (macrovision != null)
                results.AddRange(macrovision);

            // Run Cactus Data Shield directory checks
            var cactusDataShield = CactusDataShieldCheckDirectoryPath(path, files);
            if (cactusDataShield != null)
                results.AddRange(cactusDataShield);

            // Run C-Dilla directory checks
            var cDilla = CDillaCheckDirectoryPath(path, files);
            if (cDilla != null)
                results.AddRange(cDilla);

            // Run FLEXnet directory checks
            var flexNet = FLEXNetCheckDirectoryPath(path, files);
            if (flexNet != null)
                results.AddRange(flexNet);

            // Run RipGuard directory checks
            var ripGuard = RipGuardCheckDirectoryPath(path, files);
            if (ripGuard != null)
                results.AddRange(ripGuard);

            // Run SafeCast directory checks
            var safeCast = SafeCastCheckDirectoryPath(path, files);
            if (safeCast != null)
                results.AddRange(safeCast);

            // Run SafeDisc directory checks
            var safeDisc = SafeDiscCheckDirectoryPath(path, files);
            if (safeDisc != null)
                results.AddRange(safeDisc);

            if (results != null && results.Count > 0)
                return results;

            return [];
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var resultsList = new List<string>();

            // Run Macrovision file checks
            var macrovision = MacrovisionCheckFilePath(path);
            if (!string.IsNullOrEmpty(macrovision))
                resultsList.Add(macrovision!);

            // Run Cactus Data Shield file checks
            var cactusDataShield = CactusDataShieldCheckFilePath(path);
            if (!string.IsNullOrEmpty(cactusDataShield))
                resultsList.Add(cactusDataShield!);

            // Run C-Dilla file checks
            var cDilla = CDillaCheckFilePath(path);
            if (!string.IsNullOrEmpty(cDilla))
                resultsList.Add(cDilla!);

            // Run FLEXnet file checks
            var flexNet = FLEXNetCheckFilePath(path);
            if (!string.IsNullOrEmpty(flexNet))
                resultsList.Add(flexNet!);

            // Run RipGuard file checks
            var ripGuard = RipGuardCheckFilePath(path);
            if (!string.IsNullOrEmpty(ripGuard))
                resultsList.Add(ripGuard!);

            // Run SafeCast file checks
            var safeCast = SafeCastCheckFilePath(path);
            if (!string.IsNullOrEmpty(safeCast))
                resultsList.Add(safeCast!);

            // Run SafeDisc file checks
            var safeDisc = SafeDiscCheckFilePath(path);
            if (!string.IsNullOrEmpty(safeDisc))
                resultsList.Add(safeDisc!);

            // Clean the result list
            resultsList = CleanResultList(resultsList);
            if (resultsList != null && resultsList.Count > 0)
                return string.Join(";", [.. resultsList]);

            return null;
        }
        
        public string? CheckISO(string file, ISO9660 iso, bool includeDebug)
        {
            #region Initial Checks
            
            var pvd = (PrimaryVolumeDescriptor)iso.VolumeDescriptorSet[0];

            if (!FileType.ISO9660.NoteworthyApplicationUse(pvd))
                return null;
            
            // Early SafeDisc actually doesn't cross into reserved bytes. Regardless, SafeDisc CD is easy enough to
            // identify for obvious other reasons, so there's not much point in potentially running into false positives.
            
            if (!FileType.ISO9660.NoteworthyReserved653Bytes(pvd))
                return null;
            
            #endregion
            
            var applicationUse = pvd.ApplicationUse;
            var reserved653Bytes = pvd.Reserved653Bytes;
            
            #region Read Application Use
            
            int offset = 0;
            var appUsefirst256Bytes = applicationUse.ReadBytes(ref offset, 256);
            var appUsemiddle128Bytes = applicationUse.ReadBytes(ref offset, 128);
            offset += 64; // Some extra values get added here over time. Check is good enough, easier to skip this.
            var appUseFirstUint =  applicationUse.ReadUInt16LittleEndian(ref offset);
            var appUseFollowingFirstUint = applicationUse.ReadBytes(ref offset, 20);
            var appUseSecondUint =  applicationUse.ReadUInt32LittleEndian(ref offset);
            var finalAppUseData = applicationUse.ReadBytes(ref offset, 38);
            
            #endregion
            
            offset = 0;
            
            #region Read Reserved 653 Bytes
            
            // Somewhat arbitrary, but going further than 11 seems to exclude some discs.
            var reservedFirst10Bytes = reserved653Bytes.ReadBytes(ref offset, 10);
            offset = 132; // TODO: Does it ever go further than this?
            var reservedFinal521Bytes = reserved653Bytes.ReadBytes(ref offset, 521);
            
            #endregion
            
            // TODO: once ST is fixed, finish this up. read to the end of the AU, then read however many bytes from the 
            // TODO: start of the reserved, confirm everything, check if reserved ends with enough 0x00 bytes too.
            
            // The first 256 bytes of application use, and the last 521 bytes of reserved data, should all be 0x00.
            // It's possible reserved might need to be shortened a bit, but a need for that has not been observed yet.
            if (!Array.TrueForAll(appUsefirst256Bytes, b => b == 0x00) || !Array.TrueForAll(reservedFinal521Bytes, b => b == 0x00))
                return null;
            
            // All of these sections should be pure data
            if (!FileType.ISO9660.IsPureData(appUsemiddle128Bytes) 
                || !FileType.ISO9660.IsPureData(appUseFollowingFirstUint)
                || !FileType.ISO9660.IsPureData(finalAppUseData)
                || !FileType.ISO9660.IsPureData(reservedFirst10Bytes))
                return null;

            // appUseFirstUint has only ever been observed as 0xBB, but no need to be this strict yet. Can be checked
            // if it's found that it's needed to, and always viable. appUseSecondUint varies more, but is still always
            // under 0xFF so far.
            if (appUseFirstUint > 0xFF || appUseSecondUint > 0xFF)
                return null;
            
            return "SafeDisc";
        }

        /// <inheritdoc cref="IPathCheck.CheckDirectoryPath(string, List{string})"/>
        internal static List<string> MacrovisionCheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("00000001.TMP"), Get00000001TMPVersion, string.Empty),
                new(new FilePathMatch("secdrv.sys"), GetSecdrvFileSizeVersion, "Macrovision Security Driver"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="IPathCheck.CheckFilePath(string)"/>
        internal static string? MacrovisionCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("00000001.TMP"), Get00000001TMPVersion, string.Empty),
                new(new FilePathMatch("secdrv.sys"), GetSecdrvFileSizeVersion, "Macrovision Security Driver"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        internal static string? Get00000001TMPVersion(string firstMatchedString, List<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // This file is present in most, if not all, SafeDisc protected discs. It seems to have very consistent file sizes, only being found to use three different file sizes in it's entire run.
            // A rough estimate of the product and version can be gotten by checking the file size.
            // One filesize is known to overlap with both SafeDisc and CDS-300, and so is detected separately here.
            return firstMatchedString.FileSize() switch
            {
                // Found in Redump entries 37832 and 66005. 
                20 => "SafeDisc 1.00.025-1.41.001",

                // Found in Redump entries 30555 and 58573.
                2_048 => "Macrovision Protection File [Likely indicates either SafeDisc 1.45.011+ (CD) or CDS-300]",

                // Found in Redump entries 11347 and 64255.
                20_482_048 => "SafeDisc 3+ (DVD)",

                _ => "(Unknown Version - Report this to us on GitHub)",
            };
        }

        // TODO: Verify these checks and remove any that may not be needed, file version checks should remove the need for any checks for 2.80+.
        internal static string? GetSecdrvFileSizeVersion(string firstMatchedString, List<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            return firstMatchedString.FileSize() switch
            {
                // Found in Redump entry 63488.
                0 => "(Empty File)",

                // Found in Redump entry 102979.
                1 => "(Empty File)",

                // Found in Redump entries 9718, 12885, 21154, 31149, 37523, 37920.
                14_304 => "/ SafeDisc 1.06.000-1.20.001",

                // Found in Redump entries 9617 and 31526.
                14_368 => "/ SafeDisc 1.30.010-1.35.000",

                // Found in Redump entries 2595, 37832, and 44350.
                10_848 => "/ SafeDisc 1.40.004-1.41.001",

                // Found in Redump entries 30555 and 55078.
                11_968 => "/ SafeDisc 1.45.011",

                // Found in Redump entries 28810 and 62935.
                11_616 => "/ SafeDisc 1.50.020",

                // Found in Redump entries 72195 and 73502.
                18_768 => "/ SafeDisc 2.05.030",

                // Found in Redump entries 38541 and 59462.
                20_128 => "/ SafeDisc 2.10.030",

                // Found in Redump entries 9819, 15312, 55823.
                27_440 => "/ SafeDisc 2.30.030-2.30.033",

                // Found in Redump entries 9846 and 23786.
                28_624 => "/ SafeDisc 2.40.010-2.40.011",

                // Found in Redump entries 30022 and 31666.
                28_400 => "/ SafeDisc 2.51.020-2.51.021",

                // Found in Redump entries 2064 and 47047.
                29_392 => "/ SafeDisc 2.60.052",

                // Found in Redump entries 13048 and 48101.
                11_376 => "/ SafeDisc 2.70.030-2.72.000",

                // Found in Redump entries 32783 and 39273.
                12_464 => "3.17.000 / SafeDisc 2.80.010-2.80.011",

                // Found in Redump entries 11638 and 52606.
                12_400 => "3.18.000 / SafeDisc 2.90.040",

                // Found in Redump entries 13230, 15383, and 36511.
                // SafeDisc 4+ is known to sometimes use old versions of drivers, such as in Redump entry 101261.
                12_528 => "3.19.000 / SafeDisc 3.10.020-3.15.011/4+",

                // Found in Redump entries 58625 and 84586.
                11_973 => "3.22.000 / SafeDisc 3.20.020-3.20.022",

                // Found in Redump entries 15614, 42034, 45686, 56320, 60021, 79729, and 80776.
                163_644 => "4.00.060 / SafeDisc 4.00.000-4.70.000",

                // Found distributed online, but so far not in a game release. May be a final driver version never released with a game. TODO: Discover original source.
                // Can be found at https://github.com/ericwj/PsSecDrv/blob/master/tools/SECDRV/SECDRV.sys, and the file is confirmed to be distributed officially by Microsoft: https://www.virustotal.com/gui/file/34bbb0459c96b3de94ccb0d73461562935c583d7bf93828da4e20a6bc9b7301d/.
                // Further confirmed to have been distributed in a Windows Server Trial in IA item pcworld-0410 (PCWorld0410.iso/WindowsServerTrial/server.iso/sources/install.wim/3/Windows/System32/drivers).
                23_040 => "4.03.086 / Product Unknown",

                // Found in https://web.archive.org/web/20010417215205/http://www.macrovision.com:80/demos/Trialware.exe.
                10_784 => "/ SafeCast ESD 2.02.040",

                // This file is not currently known to be used in versions past 4.70.000.
                _ => "/ Product Unknown (Report this to us on GitHub)",

                // Hashes have not been found to be a reliable indicator for these files, and likely differ on a game-to-game basis. Some hashes were previously collected and are collected below:
                // Found in Redump entries 3569 and 3570.
                // "B64AD3EC82F2EB9FB854512CB59C25A771322181" => "1.11.000",
                // It is not known which games these files are from.
                // "EBF69B0A96ADFC903B7E486708474DC864CC0C7C" => "1.40.004",
                // "F68A1370660F8B94F896BBBA8DC6E47644D19092" => "2.30",
                // "60BC8C3222081BF76466C521474D63714AFD43CD" => "2.40",
                // "08CECA66432278D8C4E0F448436B77583C3C61C8" => "2.50",
                // "10080EB46BF76AC9CF9EA74372CFA4313727F0CA" => "2.51",
                // "832D359A6DE191C788B0E61E33F3D01F8D793D3C" => "2.70",
                // "AFCFAAC945A5B47712719A5E6A7EB69E36A5A6E0" or "CB24FBE8AA23A49E95F3C83FB15123FFB01F43F4" => "2.80",
                // "0383B69F98D0A9C0383C8130D52D6B431C79AC48" => "2.90",
                // "D7C9213CC78FF57F2F655B050C4D5AC065661AA9" => "3.20",
                // "FC6FEDACC21A7244975B8F410FF8673285374CC2" => "4.00.002",// Also 4.60.000, might be a fluke
                // "2D9F54F35F5BACB8959EF3AFFDC3E4209A4629CB" => "1-4",
            };
        }

        // TODO: Combine with filesize version checks if possible.
        private static string GetSecDrvExecutableVersion(PortableExecutable exe)
        {
            // Different versions of this driver correspond to different SafeDisc versions.
            // TODO: Check if earlier versions of this driver contain the version string in a less obvious place. 
            var version = exe.FileVersion;
            if (!string.IsNullOrEmpty(version))
            {
                return version switch
                {
                    // Found in Redump entries 32783 and 39273.
                    // The product version is "3.17.000 Windows NT 2002/07/01".
                    "3.17.000" => "3.17.000 / SafeDisc 2.80.010-2.80.011",

                    // Found in Redump entries 11638 and 52606.
                    // The product version is "3.18.000 Windows NT 2002/11/14".
                    "3.18.000" => "3.18.000 / SafeDisc 2.90.040",

                    // Found in Redump entries 13230, 15383, and 36511.
                    // SafeDisc 4+ is known to sometimes use old versions of drivers, such as in Redump entry 101261.
                    // The product version is "3.19.000 Windows NT/2K/XP 2003/03/19".
                    "3.19.000" => "3.19.000 / SafeDisc 3.10.020-3.15.011/4+",

                    // Found in Redump entries 58625 and 84586.
                    // The product version is "SECURITY Driver 3.22.000 2004/01/16".
                    "3.22.000" => "3.22.000 / SafeDisc 3.20.020-3.20.022",

                    // Found in Redump entries 15614, 42034, 45686, 56320, 60021, 79729, and 80776.
                    // The product version is "SECURITY Driver 4.00.060 2004/08/31".
                    "4.00.060" => "4.00.060 / SafeDisc 4.00.000-4.70.000",

                    // Found distributed online, but so far not in a game release. TODO: Discover original source.
                    // Can be found at https://github.com/ericwj/PsSecDrv/blob/master/tools/SECDRV/SECDRV.sys, and the file is confirmed to be distributed officialy by Microsoft: https://www.virustotal.com/gui/file/34bbb0459c96b3de94ccb0d73461562935c583d7bf93828da4e20a6bc9b7301d/.
                    // The product version is "SECURITY Driver 4.03.086 2006/09/13".
                    "4.03.086" => "4.03.086 / Unknown Product",

                    _ => $"Unknown Version {version} (Report this to us on GitHub)",
                };
            }

            return "Unknown Version (Report this to us on GitHub)";
        }

        private static string? CheckSectionForProtection(string file, bool includeDebug, List<string>? sectionStrings, byte[]? sectionRaw, bool newVersion)
        {
            // Get the section strings, if they exist
            if (sectionStrings != null)
            {
                // If we don't have the "BoG_" string, the section isn't protected.
                if (!sectionStrings.Exists(s => s.Contains("BoG_")))
                    return null;

                // If we have the "BoG_" string but not the full "BoG_ *90.0&!!  Yy>" string,
                //   the section has had the portion of the section that included the version number removed or obfuscated (Redump entry 40337).
                if (!sectionStrings.Exists(s => s.Contains("BoG_ *90.0&!!  Yy>")))
                    return newVersion ? "Macrovision Protected Application [Version Expunged]" : null;

                // So far, every executable with an expunged version has been able to be identified via other means,
                //   most consistently by scanning files extracted to the Temp folder when the program is run.

                // The following versions have been found as expunged:

                // 2.90.040: Found in Redump entries 11638+11639, 58510+58511, 58510+110103, 71617+71618, and 95322+95324.
                // 3.20.020: Found in Redump entries 31621+31623 and 107085-107086.
                // 3.20.024: Found in Redump entries 101449+101450.
                // 4.00.001: Found in Redump entries 70504 and 74390-74391.
                // 4.00.003: Found in Redump entry 83410.
                // 4.50.000: Found in Redump entries 58990-58992, 74206, 77440, 76813, 85384, and 101261.
                // 4.60.000: Found in Redump entries 40337, 57721, 65209-65212, 73786, and 85859.
            }

            // Get the section data, if it exists
            if (sectionRaw != null)
            {
                // TODO: Add more checks to help differentiate between SafeDisc and SafeCast.
                var matchers = new List<ContentMatchSet>
                {
                    // BoG_ *90.0&!!  Yy>
                    new(new byte?[]
                    {
                        0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                        0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                        0x79, 0x3E
                    }, GetMacrovisionVersion, string.Empty),
                };

                return MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
            }

            return null;
        }

        internal static string? GetMacrovisionVersion(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            // Begin reading 2 bytes after "BoG_ *90.0&!!  Yy>" for older versions
            int index = positions[0] + 18 + 2;
            int version = fileContent.ReadInt32LittleEndian(ref index);
            int subVersion = fileContent.ReadInt32LittleEndian(ref index);
            int subsubVersion = fileContent.ReadInt32LittleEndian(ref index);

            if (version != 0)
            {
                string versionString = $"{version}.{subVersion:00}.{subsubVersion:000}";
                return $"{MacrovisionVersionToProductName(versionString)} {versionString}";
            }

            // Begin reading 14 bytes after "BoG_ *90.0&!!  Yy>" for newer versions
            index = positions[0] + 18 + 14;
            version = fileContent.ReadInt32LittleEndian(ref index);
            subVersion = fileContent.ReadInt32LittleEndian(ref index);
            subsubVersion = fileContent.ReadInt32LittleEndian(ref index);

            if (version != 0)
            {
                string versionString = $"{version}.{subVersion:00}.{subsubVersion:000}";
                return $"{MacrovisionVersionToProductName(versionString)} {versionString}";
            }

            return string.Empty;
        }

        private static string MacrovisionVersionToProductName(string version)
        {
            return version switch
            {
                // CDS-300 (Confirmed)
                // Found in "American Made World Played" by Les Paul & Friends (Japan) (https://www.discogs.com/release/18934432-Les-Paul-Friends-American-Made-World-Played) and "X&Y" by Coldplay (Japan) (https://www.discogs.com/release/822378-Coldplay-XY).
                "2.90.044" => "CDS-300",

                // SafeCast (Confirmed)
                // Version 1.04.000/1.4.0.0 can be found in "cdac01aa.dll" and "cdac01ba.dll" from IA item "ejay_nestle_trial", but needs further research.
                // Found in Redump entry 83145.
                "2.11.010"

                    // Found in IA item microsoft-software-jukebox-for-toshiba-1.0.
                    or "2.11.020"

                    // Source not documented.
                    or "2.11.060"
                    or "2.16.050"

                    // Found in IA item CMCD0204 (Software/Demo/3DSMax/3dsmax6.exe).
                    or "2.20.030"

                    // Found in Redump entry 90157 / IA item microsoft-software-jukebox-usa-hp-oem.
                    or "2.41.000"

                    // Source not documented.
                    or "2.60.030"

                    // Found in IA item game-programming-in-c-start-to-finish-2006 (tools_install/3dsMax8_Demo.zip).
                    or "2.66.000"

                    // Found in Photoshop CS2 in IA item ccd0605.
                    or "2.67.010" => "SafeCast",

                // SafeCast (Unconfirmed)
                // Found in Adobe Photoshop according to http://www.reversing.be/article.php?story=2006102413541932
                "2.42.000"

                    // Source not documented.
                    or "2.50.030"
                    or "2.51.000" => "SafeCast (Unconfirmed - Please report to us on GitHub)",

                // SafeCast ESD (Confirmed)
                // Found in https://web.archive.org/web/20010417215205/http://www.macrovision.com:80/demos/Trialware.exe.
                "2.02.040" => "SafeCast ESD",

                // SafeDisc (Confirmed)
                // Found in Redump entry 66005.
                "1.00.025"

                    // Found in Redump entries 1882, 30049, 34828, 35922, 38212, 84280, and 97611.
                    or "1.00.026"

                    // Found in Redump entries 31575 and 41923.
                    or "1.00.030"

                    // Found in Redump entries 1883 and 42114.
                    or "1.00.032"

                    // Found in Redump entries 36223 and 40770.
                    or "1.00.035"

                    // Found in Redump entries 42155 and 47574.
                    or "1.01.034"

                    // Found in Redump entry 51459.
                    or "1.01.038"

                    // Found in Redump entries 34562 and 63304.
                    or "1.01.043"

                    // Found in Redump entries 61731 and 81619.
                    or "1.01.044"

                    // Found in Redump entries 29073 and 31149.
                    or "1.06.000"

                    // Found in Redump entries 9718 and 46756.
                    or "1.07.000"

                    // Found in Redump entries 12885 and 66210.
                    or "1.09.000"

                    // Found in Redump entries 3569, 3570, 37523, and 66586.
                    or "1.11.000"

                    // Found in Redump entries 21154, 37982, and 108632.
                    or "1.20.000"

                    // Found in Redump entries 17024/37920. 
                    or "1.20.001"

                    // Found in Redump entries 28708, 31526, 43321, 55080, and 75501.
                    or "1.30.010"

                    // Found in Redump entries 9617 and 49552. 
                    or "1.35.000"

                    // Found in Redump entries 2595 and 30121.
                    or "1.40.004"

                    // Found in Redump entries 1887, 44350, 61047, and 63323.
                    or "1.41.000"

                    // Found in Redump entries 37832 and 42091.
                    or "1.41.001"

                    // Found in Redump entries 30555 and 55078.
                    or "1.45.011"

                    // Found in Redump entries 28810 and 62935.
                    or "1.50.020"

                    // Found in Redump entries 2022, 72195, and 73502.
                    or "2.05.030"

                    // Found in Redump entries 38541 and 59462.
                    or "2.10.030"

                    // Found in Redump entries 45040, 55823, and 79476.
                    or "2.30.030"

                    // Found in Redump entries 15312 and 48863.
                    or "2.30.031"

                    // Found in Redump entries 9819 and 53659/53659.
                    or "2.30.033"

                    // Found in Redump entries 9846, 45202, 65642, and 68206.
                    or "2.40.010"

                    // Found in Redump entries 23786, 37478, and 110603.
                    or "2.40.011"

                    // Found in Redump entries 30022 and 75014.
                    or "2.51.020"

                    // Found in Redump entries 31666, 38589, 66852, and 83145.
                    or "2.51.021"

                    // Found in Redump entries 2064, 47047, and 57673.
                    or "2.60.052"

                    // Found in Redump entries 13048-13050, 35385, and 46339.
                    or "2.70.030"

                    // Found in Redump entries 9261/9262 and 64198.
                    or "2.72.000"

                    // Found in Redump entries 32783, 72743, 75897, and 86176.
                    or "2.80.010"

                    // Found in Redump entries 39273/39274 and 59351.
                    or "2.80.011"

                    // Version 2.90.010 was erroneously thought to be a valid version, likely due to the SafeDisc version of Redump entry 11639 being updated from "2.80.010" to "2.90.010", presumably as a typo.
                    // The version string found in SimCity 4 Deluxe is expunged, and so it seems likely that that version was filled in to several other entries of the same game due to the uncertainty.
                    // Due to this, several BOS checks used to report a version range from 2.90.010-2.90.040, which have all since been updated.

                    // Found in Redump entries 11638/11639, 52606, 62505, 85338/85339, 95322/95324, 119414, and 119415.
                    or "2.90.040"

                    // Found in Redump entries 116357 and 121411.
                    // This version is particularly unusual, as it was in a game released in late 2007, when 2.90.040 was used from 2004/2005.
                    // It also doesn't appear to contain the SecDrv or DrvMgt drivers. It may be a Long Term Support release of SafeDisc 2 for customers unwilling or unable to use SafeDisc 3+.
                    or "2.90.045"

                    // Found in Redump entries 13230 and 68204.
                    or "3.10.020"

                    // Found in Redump entries 36511 and 74338.
                    or "3.15.010"

                    // Found in Redump entries 15383 and 35512.
                    or "3.15.011"

                    // Found in Redump entries 30404, 31621/31623, 56748, 58625, and 64355-64358.
                    or "3.20.020"

                    // Found in Redump entries 20728, 53667/53668/76775, 58625, 64255, 75782, 84985, 91552, 102135, and 102806.
                    or "3.20.022"

                    // Found in Redump entries 20729, 28257, 54268-5427, 63810-63813, and 86177.
                    or "3.20.024"

                    // Found in Redump entries 35382, 36024, 74520, and 79729.
                    or "4.00.000"

                    // Found in Redump entries 8842-8844, 15614, 38143, 67927, 70504, 74390-74391, and 83017.
                    or "4.00.001"

                    // Found in Redump entries 33326, 42034, 49677x, 71646, 78980, 85345-85347, 86196, and 105716.
                    or "4.00.002"

                    // Found in Redump entries 40595-40597, 51597, 68551-68552, 83408, and 83410.
                    or "4.00.003"

                    // Found in Redump entries 58073-58074, 58455-58459, 58990-58992, 65569, 74206, 74564 + 74579-74581, 76813, 77440, 80776-80777, 85384, and 101261.
                    or "4.50.000"

                    // Found in Redump entries 20092, 31824, 45407-45409, 45469, 45684-45686, 46764-46769, 50682, 57721, 73786, 85859, and 104503.
                    or "4.60.000"

                    // Found in Redump entries 34783, 56320-56323, and 66403.
                    or "4.70.000"

                    // Found in Redump entries 64144-64146 + 78543, and 98589-98590.
                    or "4.80.000"

                    // Found in Redump entries 13014, 52523, 74366, 76346, 83290, 115764, and 116381.
                    or "4.81.000"

                    // Found in Redump entries 20434, and 79113.
                    or "4.85.000"

                    // Found in Redump entries 38142, 56319, and 66333.
                    or "4.90.000"

                    // Found in Redump entries 11347, 29069, 58573-58575, 78976, and 120303.
                    or "4.90.010"

                    // Found in Redump entry 120213.
                    // This is a particularly odd version, as despite being the last known version of SafeDisc, it was not known to exist until recently.
                    // The copyright for "AuthServ.exe" in this version is set to RealNetworks, instead of Macrovision.
                    // RealNetworks presumably acquired SafeDisc when they purchased Trymedia from Macrovision (https://realnetworks.com/press/releases/2008/realnetworks-acquire-trymedia-macrovision).
                    // Due to this being the only known sample, it may be that they did a trial run of a new version of SafeDisc, before deciding against continuing its development.
                    or "4.91.000"

                    => "SafeDisc",

                // SafeDisc (Unconfirmed)
                // Currently only found in a pirate compilation disc: IA item "cdrom-classic-fond-58".
                "1.01.045" => "SafeDisc (Unconfirmed - Please report to us on GitHub)",

                // "4.82.01 0004" - Unknown version from https://extreme.pcgameshardware.de/threads/windows-10-spiele-mit-safedisc-kopierschutz-starten-nicht-u-a-battlefield-1942.400359/page-5

                // SafeDisc Lite (Confirmed)
                // Found in Redump entry 14928.
                "2.60.020" => "SafeDisc Lite",

                // SafeDisc Lite (Unconfirmed)
                // Found in Redump entries 63769 and 89649.
                // This is unconfirmed because it is only known to exist in Mac versions of Lite, which currently isn't scanned for.
                // If it is present on PC, or at least in an executable format that can be scanned, it is not currently known or documented.
                "2.70.020" => "SafeDisc Lite (Unconfirmed - Please report to us on GitHub)",

                _ => "Macrovision Protected Application (Generic detection - Report to us on GitHub)",
            };
        }

        private static List<string>? CleanResultList(List<string>? resultsList)
        {
            // If we have an invalid result list
            if (resultsList == null || resultsList.Count == 0)
                return resultsList;

            // Remove duplicates
            if (resultsList.Contains("Macrovision Protected Application"))
            {
                if (resultsList.Contains("Macrovision Protected Application [Version Expunged]"))
                    resultsList = resultsList.FindAll(r => r != "Macrovision Protected Application");
                else if (resultsList.Contains("Macrovision Protected Application (Entry point not present in the stxt371 section. Executable is either unprotected or nonfunctional)"))
                    resultsList = resultsList.FindAll(r => r != "Macrovision Protected Application");
                else if (resultsList.Contains("Macrovision Protected Application (Generic detection - Report to us on GitHub)"))
                    resultsList = resultsList.FindAll(r => r != "Macrovision Protected Application");
                else if (resultsList.Contains("Macrovision Protected Application (Report this to us on GitHub)"))
                    resultsList = resultsList.FindAll(r => r != "Macrovision Protected Application");
            }

            // Get distinct and order
            var distinct = new List<string>();
            foreach (string result in resultsList)
            {
                if (!distinct.Contains(result))
                    distinct.Add(result);
            }

            distinct.Sort();
            return distinct;
        }
    }
}
