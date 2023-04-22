using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Utilities;
using BinaryObjectScanner.Wrappers;
using System;

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
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Check we have a valid executable
            if (nex == null)
                return null;

            List<string> resultsList = new List<string>();

            // Run C-Dilla NE checks
            string cDilla = CDillaCheckNewExecutable(file, nex, includeDebug);
            if (!string.IsNullOrWhiteSpace(cDilla))
                resultsList.Add(cDilla);

            // Run SafeCast NE checks
            string safeCast = SafeCastCheckNewExecutable(file, nex, includeDebug);
            if (!string.IsNullOrWhiteSpace(safeCast))
                resultsList.Add(safeCast);

            if (resultsList != null && resultsList.Count > 0)
                return string.Join(", ", resultsList);

            return null;
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Check for specific indications for individual Macrovision protections.
            List<string> resultsList = new List<string>();

            // Check for generic indications of Macrovision protections first.
            string name = pex.FileDescription;

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
                string sectionMatch = CheckSectionForProtection(file, includeDebug, pex.HeaderPaddingStrings, pex.HeaderPaddingData, true);
                if (!string.IsNullOrWhiteSpace(sectionMatch))
                {
                    resultsList.Add(sectionMatch);
                }
                else
                {
                    // Get the .data section, if it exists, for protected sections.
                    sectionMatch = CheckSectionForProtection(file, includeDebug, pex.GetFirstSectionStrings(".data"), pex.GetFirstSectionData(".data"), true);
                    if (!string.IsNullOrWhiteSpace(sectionMatch))
                        resultsList.Add(sectionMatch);
                }

                int entryPointIndex = pex.FindEntryPointSectionIndex();
                string entryPointSectionName = pex.SectionNames[entryPointIndex];

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
                string sectionMatch = CheckSectionForProtection(file, includeDebug, pex.HeaderPaddingStrings, pex.HeaderPaddingData, false);
                if (!string.IsNullOrWhiteSpace(sectionMatch))
                {
                    resultsList.Add(sectionMatch);
                }
                else
                {
                    // Check the .data section, if it exists, for protected sections.
                    sectionMatch = CheckSectionForProtection(file, includeDebug, pex.GetFirstSectionStrings(".data"), pex.GetFirstSectionData(".data"), false);
                    if (!string.IsNullOrWhiteSpace(sectionMatch))
                        resultsList.Add(sectionMatch);
                }
            }

            // Run Cactus Data Shield PE checks
            string match = CactusDataShieldCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                resultsList.Add(match);

            // Run C-Dilla PE checks
            match = CDillaCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                resultsList.Add(match);

            // Run SafeCast PE checks
            match = SafeCastCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                resultsList.Add(match);

            // Run SafeDisc PE checks
            match = SafeDiscCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                resultsList.Add(match);

            // Run FLEXnet PE checks
            match = FLEXnetCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                resultsList.Add(match);

            // Clean the result list
            resultsList = CleanResultList(resultsList);
            if (resultsList != null && resultsList.Count > 0)
                return string.Join(", ", resultsList);

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            ConcurrentQueue<string> results = new ConcurrentQueue<string>();

            // Run Macrovision directory checks
            var macrovision = MacrovisionCheckDirectoryPath(path, files);
            if (macrovision != null && !macrovision.IsEmpty)
                results.AddRange(macrovision);

            // Run Cactus Data Shield directory checks
            var cactusDataShield = CactusDataShieldCheckDirectoryPath(path, files);
            if (cactusDataShield != null && !cactusDataShield.IsEmpty)
                results.AddRange(cactusDataShield);

            // Run C-Dilla directory checks
            var cDilla = CDillaCheckDirectoryPath(path, files);
            if (cDilla != null && !cDilla.IsEmpty)
                results.AddRange(cDilla);

            // Run SafeCast directory checks
            var safeCast = SafeCastCheckDirectoryPath(path, files);
            if (safeCast != null && !safeCast.IsEmpty)
                results.AddRange(safeCast);

            // Run SafeDisc directory checks
            var safeDisc = SafeDiscCheckDirectoryPath(path, files);
            if (safeDisc != null && !safeDisc.IsEmpty)
                results.AddRange(safeDisc);

            if (results != null && results.Count > 0)
                return results;

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            List<string> resultsList = new List<string>();

            // Run Macrovision file checks
            string macrovision = MacrovisionCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(macrovision))
                resultsList.Add(macrovision);

            // Run Cactus Data Shield file checks
            string cactusDataShield = CactusDataShieldCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(cactusDataShield))
                resultsList.Add(cactusDataShield);

            // Run C-Dilla file checks
            string cDilla = CDillaCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(cDilla))
                resultsList.Add(cDilla);

            // Run SafeCast file checks
            string safeCast = SafeCastCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(safeCast))
                resultsList.Add(safeCast);

            // Run SafeDisc file checks
            string safeDisc = SafeDiscCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(safeDisc))
                resultsList.Add(safeDisc);

            if (resultsList != null && resultsList.Count > 0)
                return string.Join(", ", resultsList);

            return null;
        }

        /// <inheritdoc cref="IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
        internal ConcurrentQueue<string> MacrovisionCheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), Get00000001TMPVersion, string.Empty),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}secdrv.sys", useEndsWith: true), GetSecdrvFileSizeVersion, "Macrovision Security Driver"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="IPathCheck.CheckFilePath(string)"/>
        internal string MacrovisionCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), Get00000001TMPVersion, string.Empty),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}secdrv.sys", useEndsWith: true), GetSecdrvFileSizeVersion, "Macrovision Security Driver"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        internal static string Get00000001TMPVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // This file is present in most, if not all, SafeDisc protected discs. It seems to have very consistent file sizes, only being found to use three different file sizes in it's entire run.
            // A rough estimate of the product and version can be gotten by checking the file size.
            // One filesize is known to overlap with both SafeDisc and CDS-300, and so is detected separately here.
            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                // Found in Redump entries 37832 and 66005. 
                case 20:
                    return "SafeDisc 1.00.025-1.41.001";
                // Found in Redump entries 30555 and 58573.
                case 2_048:
                    return "Macrovision Protection File [Likely indicates either SafeDisc 1.45.011+ (CD) or CDS-300]";
                // Found in Redump entries 11347 and 64255.
                case 20_482_048:
                    return "SafeDisc 3+ (DVD)";
                default:
                    return "(Unknown Version - Report this to us on GitHub)";
            }
        }

        // TODO: Verify these checks and remove any that may not be needed, file version checks should remove the need for any checks for 2.80+.
        internal static string GetSecdrvFileSizeVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                // Found in Redump entry 102979.
                case 1:
                    return "(Empty File)";
                // Found in Redump entries 9718, 12885, 21154, 31149, 37523, 37920.
                case 14_304:
                    return "/ SafeDisc 1.06.000-1.20.001";
                // Found in Redump entries 9617 and 31526.
                case 14_368:
                    return "/ SafeDisc 1.30.010-1.35.000";
                // Found in Redump entries 2595, 37832, and 44350.
                case 10_848:
                    return "/ SafeDisc 1.40.004-1.41.001";
                // Found in Redump entries 30555 and 55078.
                case 11_968:
                    return "/ SafeDisc 1.45.011";
                // Found in Redump entries 28810 and 62935.
                case 11_616:
                    return "/ SafeDisc 1.50.020";
                // Found in Redump entries 72195 and 73502.
                case 18_768:
                    return "/ SafeDisc 2.05.030";
                // Found in Redump entries 38541 and 59462.
                case 20_128:
                    return "/ SafeDisc 2.10.030";
                // Found in Redump entries 9819, 15312, 55823.
                case 27_440:
                    return "/ SafeDisc 2.30.030-2.30.033";
                // Found in Redump entries 9846 and 23786.
                case 28_624:
                    return "/ SafeDisc 2.40.010-2.40.011";
                // Found in Redump entries 30022 and 31666.
                case 28_400:
                    return "/ SafeDisc 2.51.020-2.51.021";
                // Found in Redump entries 2064 and 47047.
                case 29_392:
                    return "/ SafeDisc 2.60.052";
                // Found in Redump entries 13048 and 48101.
                case 11_376:
                    return "/ SafeDisc 2.70.030-2.72.000";
                // Found in Redump entries 32783 and 39273.
                case 12_464:
                    return "3.17.000 / SafeDisc 2.80.010-2.80.011";
                // Found in Redump entries 11638 and 52606.
                case 12_400:
                    return "3.18.000 / SafeDisc 2.90.010-2.90.040";
                // Found in Redump entries 13230, 15383, and 36511.
                case 12_528:
                    return "3.19.000 / SafeDisc 3.10.020-3.15.011";
                // Found in Redump entries 58625 and 84586.
                case 11_973:
                    return "3.22.000 / SafeDisc 3.20.020-3.20.022";
                // Found in Redump entries 15614, 42034, 45686, 56320, 60021, 79729, and 80776.
                case 163_644:
                    return "4.00.060 / SafeDisc 4.00.000-4.70.000";
                // Found distributed online, but so far not in a game release. TODO: Discover original source.
                // Can be found at https://github.com/ericwj/PsSecDrv/blob/master/tools/SECDRV/SECDRV.sys, and the file is confirmed to be distributed officialy by Microsoft: https://www.virustotal.com/gui/file/34bbb0459c96b3de94ccb0d73461562935c583d7bf93828da4e20a6bc9b7301d/.
                case 23_040:
                    return "4.03.086 / Product Unknown";
                // Found in https://web.archive.org/web/20010417215205/http://www.macrovision.com:80/demos/Trialware.exe.
                case 10_784:
                    return "/ SafeCast ESD 2.02.040";
                // This file is not currently known to be used in versions past 4.70.000.
                default:
                    return "/ Product Unknown (Report this to us on GitHub)";
            }
        }

        // TODO: Combine with filesize version checks if possible.
        private string GetSecDrvExecutableVersion(PortableExecutable pex)
        {
            // Different versions of this driver correspond to different SafeDisc versions.
            // TODO: Check if earlier versions of this driver contain the version string in a less obvious place. 
            string version = pex.FileVersion;
            if (!string.IsNullOrEmpty(version))
            {
                switch (version)
                {
                    // Found to be in Redump entry 32783.
                    // The product version is "3.17.000 Windows NT 2002/07/01".
                    case "3.17.000":
                        return "3.17.000 / SafeDisc 2.80.010-2.80.011";
                    // Found to be in Redump entry 52606.
                    // The product version is "3.18.000 Windows NT 2002/11/14".
                    case "3.18.000":
                        return "3.18.000 / SafeDisc 2.90.010-2.90.040";
                    // Found to be in Redump entry 13230.
                    // The product version is "3.19.000 Windows NT/2K/XP 2003/03/19".
                    case "3.19.000":
                        return "3.19.000 / SafeDisc 3.10.020-3.15.011";
                    // Found to be in Redump entry 58625.
                    // The product version is "SECURITY Driver 3.22.000 2004/01/16".
                    case "3.22.000":
                        return "3.22.000 / SafeDisc 3.20.020-3.20.022";
                    // Found to be in Redump entry 15614.
                    // The product version is "SECURITY Driver 4.00.060 2004/08/31".
                    case "4.00.060":
                        return "4.00.060 / SafeDisc 4.00.000-4.70.000";
                    // Found distributed online, but so far not in a game release. TODO: Discover original source.
                    // Can be found at https://github.com/ericwj/PsSecDrv/blob/master/tools/SECDRV/SECDRV.sys, and the file is confirmed to be distributed officialy by Microsoft: https://www.virustotal.com/gui/file/34bbb0459c96b3de94ccb0d73461562935c583d7bf93828da4e20a6bc9b7301d/.
                    // The product version is "SECURITY Driver 4.03.086 2006/09/13".
                    case "4.03.086":
                        return "4.03.086 / Unknown SafeDisc version";
                    default:
                        return $"Unknown Version {version} (Report this to us on GitHub)";
                }
            }

            return "Unknown Version (Report this to us on GitHub)";
        }

        private string CheckSectionForProtection(string file, bool includeDebug, List<string> sectionStrings, byte[] sectionRaw, bool newVersion)
        {
            // Get the section strings, if they exist
            if (sectionStrings == null)
                return null;

            // If we don't have the "BoG_" string, the section isn't protected.
            if (!sectionStrings.Any(s => s.Contains("BoG_")))
                return null;

            // If we have the "BoG_" string but not the full "BoG_ *90.0&!!  Yy>" string, the section has had the portion of the section that included the version number removed or obfuscated (Redump entry 40337).
            if (!sectionStrings.Any(s => s.Contains("BoG_ *90.0&!!  Yy>")))
                return newVersion ? "Macrovision Protected Application [Version Expunged]" : null;

            // TODO: Add more checks to help differentiate between SafeDisc and SafeCast.
            var matchers = new List<ContentMatchSet>
            {
                // BoG_ *90.0&!!  Yy>
                new ContentMatchSet(new byte?[]
                {
                    0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                    0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                    0x79, 0x3E
                }, GetMacrovisionVersion, string.Empty),
            };

            return MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
        }

        internal static string GetMacrovisionVersion(string file, byte[] fileContent, List<int> positions)
        {
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
            switch (version)
            {
                // CDS-300 (Confirmed)
                case "2.90.044": // Found in "American Made World Played" by Les Paul & Friends (Japan) (https://www.discogs.com/release/18934432-Les-Paul-Friends-American-Made-World-Played) and "X&Y" by Coldplay (Japan) (https://www.discogs.com/release/822378-Coldplay-XY).
                    return "CDS-300";

                // SafeCast (Confirmed)
                // Version 1.04.000/1.4.0.0 can be found in "cdac01aa.dll" and "cdac01ba.dll" from IA item "ejay_nestle_trial", but needs further research.
                case "2.11.010": // Found in Redump entry 83145.
                case "2.11.060": // Found in Redump entry 102979.
                case "2.16.050": // Found in IA items "cdrom-turbotax-2002", "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282", and "TurboTax_Premier_Tax_Year_2002_for_Windows_v02.00Z-R_Intuit_352283_2002".
                case "2.60.030": // Found in Redump entry 74384 (Semi-confirmed) and "Data Becker Web To Date v3.1" according to https://web.archive.org/web/20210331144912/https://protectionid.net/ (Unconfirmed).
                case "2.67.010": // Found in "[Win] Photoshop CS2.7z" in IA item "Adobe-CS2".
                    return "SafeCast";

                // SafeCast (Unconfirmed)
                case "2.41.000": // Found in Adobe Photoshop according to http://www.reversing.be/article.php?story=2006102413541932
                case "2.42.000": // Found in "Dreamweaver MX 2004 v7.0.1" according to https://web.archive.org/web/20210331144912/https://protectionid.net/.
                case "2.50.030": // Found in "ArcSoft Media Card Companion v1.0" according to https://web.archive.org/web/20210331144912/https://protectionid.net/.
                case "2.51.000": // Found in "Autodesk Inventor Professional v9.0" according to https://web.archive.org/web/20210331144912/https://protectionid.net/.
                    return "SafeCast (Unconfirmed - Please report to us on GitHub)";

                // SafeCast ESD (Confirmed)
                case "2.02.040": // Found in https://web.archive.org/web/20010417215205/http://www.macrovision.com:80/demos/Trialware.exe.
                    return "SafeCast ESD";

                // SafeDisc (Confirmed)
                case "1.00.025": // Found in Redump entry 66005.
                case "1.00.026": // Found in Redump entries 1882 and 30049.
                case "1.00.030": // Found in Redump entries 31575 and 41923.
                case "1.00.032": // Found in Redump entries 1883 and 42114.
                case "1.00.035": // Found in Redump entries 36223 and 40771.
                case "1.01.034": // Found in Redump entries 42155 and 47574.
                case "1.01.038": // Found in Redump entry 51459.
                case "1.01.043": // Found in Redump entries 34562 and 63304.
                case "1.01.044": // Found in Redump entries 61731 and 81619.
                case "1.06.000": // Found in Redump entries 29073 and 31149.
                case "1.07.000": // Found in Redump entries 9718 and 46756.
                case "1.09.000": // Found in Redump entries 12885 and 66210.
                case "1.11.000": // Found in Redump entries 37523 and 66586.
                case "1.20.000": // Found in Redump entries 21154 and 37982.
                case "1.20.001": // Found in Redump entry 37920.
                case "1.30.010": // Found in Redump entries 31526 and 55080.
                case "1.35.000": // Found in Redump entries 9617 and 49552.
                case "1.40.004": // Found in Redump entries 2595 and 30121.
                case "1.41.000": // Found in Redump entries 44350 and 63323.
                case "1.41.001": // Found in Redump entries 37832 and 42091.
                case "1.45.011": // Found in Redump entries 30555 and 55078.
                case "1.50.020": // Found in Redump entries 28810 and 62935.
                case "2.05.030": // Found in Redump entries 72195 and 73502.
                case "2.10.030": // Found in Redump entries 38541, 59462, and 81096.
                case "2.30.030": // Found in Redump entries 55823 and 79476.
                case "2.30.031": // Found in Redump entries 15312 and 48863.
                case "2.30.033": // Found in Redump entries 9819 and 53658.
                case "2.40.010": // Found in Redump entries 9846 and 65642.
                case "2.40.011": // Found in Redump entries 23786 and 37478.
                case "2.51.020": // Found in Redump entries 30022 and 75014.
                case "2.51.021": // Found in Redump entries 31666 and 66852.
                case "2.60.052": // Found in Redump entries 2064 and 47047.
                case "2.70.030": // Found in Redump entries 13048 and 35385.
                case "2.72.000": // Found in Redump entries 48101 and 64198.
                case "2.80.010": // Found in Redump entries 32783 and 72743.
                case "2.80.011": // Found in Redump entries 39273 and 59351.
                case "2.90.040": // Found in Redump entries 52606 and 62505.
                case "3.10.020": // Found in Redump entries 13230 and 68204.
                case "3.15.010": // Found in Redump entries 36511 and 74338.
                case "3.15.011": // Found in Redump entries 15383 and 35512.
                case "3.20.020": // Found in Redump entries 30404 and 56748.
                case "3.20.022": // Found in Redump entries 58625 and 91552.
                case "3.20.024": // Found in Redump entries 20729 and 63813 (CD) and Redump entries 20728 and 64255 (DVD).
                case "4.00.000": // Found in Redump entries 35382 and 79729 (CD) and Redump entry 74520 (DVD).
                case "4.00.001": // Found in  Redump entries 8842 and 83017 (CD) and Redump entries 15614 and 38143 (DVD).
                case "4.00.002": // Found in Redump entries 42034 and 71646 (CD) and Redump entries 78980 and 86196 (DVD).
                case "4.00.003": // Found in Redump entries 60021 and 68551 (CD) and Redump entries 51597 and 83408 (DVD).
                case "4.50.000": // Found in Redump entries 58990 and 80776 (CD) and Redump entries 65569 and 76813 (DVD).
                case "4.60.000": // Found in Redump entries 45686 and 46765 (CD) and Redump entries 45469 and 50682 (DVD).
                case "4.70.000": // Found in Redump entry 56320 (CD) and Redump entries 34783 and 66403 (DVD).
                case "4.80.000": // Found in Redump entries 64145 and 78543 (CD only).
                case "4.81.000": // Found in Redump entries 52523 and 76346 (DVD only).
                case "4.85.000": // Found in Redump entries 20434 and 31766 (DVD only).
                case "4.90.000": // Found in Redump entries 56319 and 66333 (DVD only).
                case "4.90.010": // Found in Redump entries 58573 and 78976 (CD) and Redump entries 11347 and 29069 (DVD).
                    return "SafeDisc";

                // SafeDisc (Unconfirmed)
                case "1.01.045": // Currently only found in a pirate compilation disc: IA item "cdrom-classic-fond-58".
                    return "SafeDisc (Unconfirmed - Please report to us on GitHub)";

                // SafeDisc Lite (Confirmed)
                case "2.60.020": // Found in Redump entry 14928.
                    return "SafeDisc Lite";

                default:
                    return "Macrovision Protected Application (Generic detection - Report to us on GitHub)";
            }
        }

        private List<string> CleanResultList(List<string> resultsList)
        {
            // If we have an invalid result list
            if (resultsList == null || resultsList.Count == 0)
                return resultsList;

            // Get distinct and order
            return resultsList.Distinct().OrderBy(s => s).ToList();
        }
    }
}
