using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;
using SabreTools.IO;
using SabreTools.Matching;
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
    public partial class Macrovision : IPathCheck, INewExecutableCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            var resultsList = new List<string>();

            // Run C-Dilla NE checks
            var cDilla = CDillaCheckNewExecutable(file, nex, includeDebug);
            if (!string.IsNullOrEmpty(cDilla))
                resultsList.Add(cDilla!);

            // Run SafeCast NE checks
            var safeCast = SafeCastCheckNewExecutable(file, nex, includeDebug);
            if (!string.IsNullOrEmpty(safeCast))
                resultsList.Add(safeCast!);

            if (resultsList != null && resultsList.Count > 0)
                return string.Join(", ", [.. resultsList]);

            return null;
        }

        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Check for specific indications for individual Macrovision protections.
            var resultsList = new List<string>();

            // Check for generic indications of Macrovision protections first.
            var name = pex.FileDescription;

            // Present in "secdrv.sys" files found in SafeDisc 2.80.010+.
            if (name?.Equals("Macrovision SECURITY Driver", StringComparison.OrdinalIgnoreCase) == true)
                resultsList.Add($"Macrovision Security Driver {GetSecDrvExecutableVersion(pex)}");

            // Found in hidden resource of "32bit\Tax02\cdac14ba.dll" in IA item "TurboTax Deluxe Tax Year 2002 for Wndows (2.00R)(Intuit)(2002)(352282)".
            // Known versions:
            // 4.16.050 Windows NT 2002/04/24
            if (name?.Equals("Macrovision RTS Service", StringComparison.OrdinalIgnoreCase) == true)
                resultsList.Add($"Macrovision RTS Service {pex.FileVersion}");

            // The stxt371 and stxt774 sections are found in various newer Macrovision products, including various versions of CDS-300, SafeCast, and SafeDisc.
            // They may indicate SafeWrap, but this hasn't been confirmed yet.
            bool stxt371Section = pex.ContainsSection("stxt371", exact: true);
            bool stxt774Section = pex.ContainsSection("stxt774", exact: true);
            if (stxt371Section || stxt774Section)
            {
                // Check the header padding for protected sections.
                var sectionMatch = CheckSectionForProtection(file, includeDebug, pex.HeaderPaddingStrings, pex.HeaderPaddingData, true);
                if (!string.IsNullOrEmpty(sectionMatch))
                {
                    resultsList.Add(sectionMatch!);
                }
                else
                {
                    // Get the .data section, if it exists, for protected sections.
                    sectionMatch = CheckSectionForProtection(file, includeDebug, pex.GetFirstSectionStrings(".data"), pex.GetFirstSectionData(".data"), true);
                    if (!string.IsNullOrEmpty(sectionMatch))
                        resultsList.Add(sectionMatch!);
                }

                int entryPointIndex = pex.FindEntryPointSectionIndex();
                var entryPointSectionName = pex.SectionNames?[entryPointIndex];

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
                var sectionMatch = CheckSectionForProtection(file, includeDebug, pex.HeaderPaddingStrings, pex.HeaderPaddingData, false);
                if (!string.IsNullOrEmpty(sectionMatch))
                {
                    resultsList.Add(sectionMatch!);
                }
                else
                {
                    // Check the .data section, if it exists, for protected sections.
                    sectionMatch = CheckSectionForProtection(file, includeDebug, pex.GetFirstSectionStrings(".data"), pex.GetFirstSectionData(".data"), false);
                    if (!string.IsNullOrEmpty(sectionMatch))
                        resultsList.Add(sectionMatch!);
                }
            }

            // Run Cactus Data Shield PE checks
            var match = CactusDataShieldCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run C-Dilla PE checks
            match = CDillaCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run RipGuard PE checks
            match = RipGuardCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run SafeCast PE checks
            match = SafeCastCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run SafeDisc PE checks
            match = SafeDiscCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Run FLEXnet PE checks
            match = FLEXnetCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrEmpty(match))
                resultsList.Add(match!);

            // Clean the result list
            resultsList = CleanResultList(resultsList);
            if (resultsList != null && resultsList.Count > 0)
                return string.Join(", ", [.. resultsList]);

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
#if NET20 || NET35
            var results = new Queue<string>();
#else
            var results = new ConcurrentQueue<string>();
#endif

            // Run Macrovision directory checks
            var macrovision = MacrovisionCheckDirectoryPath(path, files);
#if NET20 || NET35
            if (macrovision != null && macrovision.Count > 0)
#else
            if (macrovision != null && !macrovision.IsEmpty)
#endif
                results.AddRange(macrovision);

            // Run Cactus Data Shield directory checks
            var cactusDataShield = CactusDataShieldCheckDirectoryPath(path, files);
#if NET20 || NET35
            if (cactusDataShield != null && cactusDataShield.Count > 0)
#else
            if (cactusDataShield != null && !cactusDataShield.IsEmpty)
#endif
                results.AddRange(cactusDataShield);

            // Run C-Dilla directory checks
            var cDilla = CDillaCheckDirectoryPath(path, files);
#if NET20 || NET35
            if (cDilla != null && cDilla.Count > 0)
#else
            if (cDilla != null && !cDilla.IsEmpty)
#endif
                results.AddRange(cDilla);

            // Run RipGuard directory checks
            var ripGuard = RipGuardCheckDirectoryPath(path, files);
#if NET20 || NET35
            if (ripGuard != null && ripGuard.Count > 0)
#else
            if (ripGuard != null && !ripGuard.IsEmpty)
#endif
                results.AddRange(ripGuard);

            // Run SafeCast directory checks
            var safeCast = SafeCastCheckDirectoryPath(path, files);
#if NET20 || NET35
            if (safeCast != null && safeCast.Count > 0)
#else
            if (safeCast != null && !safeCast.IsEmpty)
#endif
                results.AddRange(safeCast);

            // Run SafeDisc directory checks
            var safeDisc = SafeDiscCheckDirectoryPath(path, files);
#if NET20 || NET35
            if (safeDisc != null && safeDisc.Count > 0)
#else
            if (safeDisc != null && !safeDisc.IsEmpty)
#endif
                results.AddRange(safeDisc);

            if (results != null && results.Count > 0)
                return results;

#if NET20 || NET35
            return new Queue<string>();
#else
            return new ConcurrentQueue<string>();
#endif
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

            if (resultsList != null && resultsList.Count > 0)
                return string.Join(", ", [.. resultsList]);

            return null;
        }

        /// <inheritdoc cref="IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
#if NET20 || NET35
        internal Queue<string> MacrovisionCheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        internal ConcurrentQueue<string> MacrovisionCheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new(new PathMatch("00000001.TMP", useEndsWith: true), Get00000001TMPVersion, string.Empty),
                new(new FilePathMatch("secdrv.sys"), GetSecdrvFileSizeVersion, "Macrovision Security Driver"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="IPathCheck.CheckFilePath(string)"/>
        internal string? MacrovisionCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new PathMatch("00000001.TMP", useEndsWith: true), Get00000001TMPVersion, string.Empty),
                new(new FilePathMatch("secdrv.sys"), GetSecdrvFileSizeVersion, "Macrovision Security Driver"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        internal static string? Get00000001TMPVersion(string firstMatchedString, IEnumerable<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // This file is present in most, if not all, SafeDisc protected discs. It seems to have very consistent file sizes, only being found to use three different file sizes in it's entire run.
            // A rough estimate of the product and version can be gotten by checking the file size.
            // One filesize is known to overlap with both SafeDisc and CDS-300, and so is detected separately here.
            var fi = new FileInfo(firstMatchedString);
            return fi.Length switch
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
        internal static string? GetSecdrvFileSizeVersion(string firstMatchedString, IEnumerable<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            var fi = new FileInfo(firstMatchedString);
            return fi.Length switch
            {
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
                12_400 => "3.18.000 / SafeDisc 2.90.010-2.90.040",

                // Found in Redump entries 13230, 15383, and 36511.
                // SafeDisc 4+ is known to sometimes use old versions of drivers, such as in Redump entry 101261.
                12_528 => "3.19.000 / SafeDisc 3.10.020-3.15.011/4+";
                
                // Found in Redump entries 58625 and 84586.
                11_973 => "3.22.000 / SafeDisc 3.20.020-3.20.022",

                // Found in Redump entries 15614, 42034, 45686, 56320, 60021, 79729, and 80776.
                163_644 => "4.00.060 / SafeDisc 4.00.000-4.70.000",

                // Found distributed online, but so far not in a game release. TODO: Discover original source.
                // Can be found at https://github.com/ericwj/PsSecDrv/blob/master/tools/SECDRV/SECDRV.sys, and the file is confirmed to be distributed officialy by Microsoft: https://www.virustotal.com/gui/file/34bbb0459c96b3de94ccb0d73461562935c583d7bf93828da4e20a6bc9b7301d/.
                23_040 => "4.03.086 / Product Unknown",

                // Found in https://web.archive.org/web/20010417215205/http://www.macrovision.com:80/demos/Trialware.exe.
                10_784 => "/ SafeCast ESD 2.02.040",

                // This file is not currently known to be used in versions past 4.70.000.
                _ => "/ Product Unknown (Report this to us on GitHub)",
            };
        }

        // TODO: Combine with filesize version checks if possible.
        private string GetSecDrvExecutableVersion(PortableExecutable pex)
        {
            // Different versions of this driver correspond to different SafeDisc versions.
            // TODO: Check if earlier versions of this driver contain the version string in a less obvious place. 
            var version = pex.FileVersion;
            if (!string.IsNullOrEmpty(version))
            {
                return version switch
                {
                    // Found in Redump entries 32783 and 39273.
                    // The product version is "3.17.000 Windows NT 2002/07/01".
                    "3.17.000" => "3.17.000 / SafeDisc 2.80.010-2.80.011",

                    // Found in Redump entries 11638 and 52606.
                    // The product version is "3.18.000 Windows NT 2002/11/14".
                    "3.18.000" => "3.18.000 / SafeDisc 2.90.010-2.90.040",

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

        private string? CheckSectionForProtection(string file, bool includeDebug, List<string>? sectionStrings, byte[]? sectionRaw, bool newVersion)
        {
            // Get the section strings, if they exist
            if (sectionStrings != null)
            {
                // If we don't have the "BoG_" string, the section isn't protected.
                if (!sectionStrings.Any(s => s.Contains("BoG_")))
                    return null;

                // If we have the "BoG_" string but not the full "BoG_ *90.0&!!  Yy>" string, the section has had the portion of the section that included the version number removed or obfuscated (Redump entry 40337).
                if (!sectionStrings.Any(s => s.Contains("BoG_ *90.0&!!  Yy>")))
                    return newVersion ? "Macrovision Protected Application [Version Expunged]" : null;
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
            int version = fileContent.ReadInt32(ref index);
            int subVersion = fileContent.ReadInt32(ref index);
            int subsubVersion = fileContent.ReadInt32(ref index);

            if (version != 0)
            {
                string versionString = $"{version}.{subVersion:00}.{subsubVersion:000}";
                return $"{MacrovisionVersionToProductName(versionString)} {versionString}";
            }

            // Begin reading 14 bytes after "BoG_ *90.0&!!  Yy>" for newer versions
            index = positions[0] + 18 + 14;
            version = fileContent.ReadInt32(ref index);
            subVersion = fileContent.ReadInt32(ref index);
            subsubVersion = fileContent.ReadInt32(ref index);

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
                    or "2.11.020"
                    or "2.11.060"
                    or "2.16.050"
                    or "2.60.030"
                    or "2.67.010" => "SafeCast",

                // SafeCast (Unconfirmed)
                // Found in Adobe Photoshop according to http://www.reversing.be/article.php?story=2006102413541932
                "2.41.000"
                    or "2.42.000"
                    or "2.50.030"
                    or "2.51.000" => "SafeCast (Unconfirmed - Please report to us on GitHub)",

                // SafeCast ESD (Confirmed)
                // Found in https://web.archive.org/web/20010417215205/http://www.macrovision.com:80/demos/Trialware.exe.
                "2.02.040" => "SafeCast ESD",

                // SafeDisc (Confirmed)
                // Found in Redump entry 66005.
                "1.00.025"
                    or "1.00.026"
                    or "1.00.030"
                    or "1.00.032"
                    or "1.00.035"
                    or "1.01.034"
                    or "1.01.038"
                    or "1.01.043"
                    or "1.01.044"
                    or "1.06.000"
                    or "1.07.000"
                    or "1.09.000"
                    or "1.11.000"
                    or "1.20.000"
                    or "1.20.001"
                    or "1.30.010"
                    or "1.35.000"
                    or "1.40.004"
                    or "1.41.000"
                    or "1.41.001"
                    or "1.45.011"
                    or "1.50.020"
                    or "2.05.030"
                    or "2.10.030"
                    or "2.30.030"
                    or "2.30.031"
                    or "2.30.033"
                    or "2.40.010"
                    or "2.40.011"
                    or "2.51.020"
                    or "2.51.021"
                    or "2.60.052"
                    or "2.70.030"
                    or "2.72.000"
                    or "2.80.010"
                    or "2.80.011"
                    or "2.90.040"
                    or "3.10.020"
                    or "3.15.010"
                    or "3.15.011"
                    or "3.20.020"
                    or "3.20.022"
                    or "3.20.024"
                    or "4.00.000"
                    or "4.00.001"
                    or "4.00.002"
                    or "4.00.003"
                    or "4.50.000"
                    or "4.60.000"
                    or "4.70.000"
                    or "4.80.000"
                    or "4.81.000"
                    or "4.85.000"
                    or "4.90.000"
                    or "4.90.010" => "SafeDisc",

                // SafeDisc (Unconfirmed)
                // Currently only found in a pirate compilation disc: IA item "cdrom-classic-fond-58".
                "1.01.045" => "SafeDisc (Unconfirmed - Please report to us on GitHub)",

                // SafeDisc Lite (Confirmed)
                // Found in Redump entry 14928.
                "2.60.020" => "SafeDisc Lite",

                _ => "Macrovision Protected Application (Generic detection - Report to us on GitHub)",
            };
        }

        private List<string>? CleanResultList(List<string>? resultsList)
        {
            // If we have an invalid result list
            if (resultsList == null || resultsList.Count == 0)
                return resultsList;

            // Get distinct and order
            return [.. resultsList.Distinct().OrderBy(s => s)];
        }
    }
}
