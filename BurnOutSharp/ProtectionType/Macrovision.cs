using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Utilities;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// This is a placeholder for all Macrovision-based protections. See partial classes for more details
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

            // Check for generic indications of Macrovision protections first.
            // TODO: Fill out generic indicators

            // Check for specific indications for individual Macrovision protections.

            List<string> resultsList = new List<string>();

            // Check the header padding
            string match = CheckSectionForProtection(file, includeDebug, pex.HeaderPaddingStrings, pex.HeaderPaddingData);
            if (!string.IsNullOrWhiteSpace(match))
            {
                resultsList.Add(match);
            }
            else
            {
                // Get the .data section, if it exists
                match = CheckSectionForProtection(file, includeDebug, pex.GetFirstSectionStrings(".data"), pex.GetFirstSectionData(".data"));
                if (!string.IsNullOrWhiteSpace(match))
                    resultsList.Add(match);
            }

            // Run Cactus Data Shield PE checks
            match = CactusDataShieldCheckPortableExecutable(file, pex, includeDebug);
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

            resultsList = FindProductFromVersion(resultsList);

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
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="IPathCheck.CheckFilePath(string)"/>
        internal string MacrovisionCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), Get00000001TMPVersion, string.Empty),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        internal static string Get00000001TMPVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // This file is present in most, if not all, SafeDisc protected discs. It seems to have very consistent file sizes, only being found to use three different file sizes in it's entire run.
            // A rough estimate of the product and version can be gotten by checking the file size.
            // One filesize is known to overlap with both SafeDisc and CDS-300.
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

        internal static string GetMacrovisionVersion(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 20; // Begin reading after "BoG_ *90.0&!!  Yy>" for old SafeDisc
            int version = fileContent.ReadInt32(ref index);
            int subVersion = fileContent.ReadInt32(ref index);
            int subsubVersion = fileContent.ReadInt32(ref index);

            if (version != 0)
                return $"{version}.{subVersion:00}.{subsubVersion:000}";

            index = positions[0] + 18 + 14; // Begin reading after "BoG_ *90.0&!!  Yy>" for newer SafeDisc
            version = fileContent.ReadInt32(ref index);
            subVersion = fileContent.ReadInt32(ref index);
            subsubVersion = fileContent.ReadInt32(ref index);

            if (version == 0)
                return string.Empty;

            return $"{version}.{subVersion:00}.{subsubVersion:000}";
        }

        private string CheckSectionForProtection(string file, bool includeDebug, List<string> sectionStrings, byte[] sectionRaw)
        {
            // Get the section strings, if they exist
            if (sectionStrings == null)
                return null;

            // If we don't have the "BoG_" string
            if (!sectionStrings.Any(s => s.Contains("BoG_ *90.0&!!  Yy>")))
                return null;

            // TODO: Add more checks to help differentiate between SafeDisc and SafeCast.
            var matchers = new List<ContentMatchSet>
            {
                // BoG_ *90.0&!!  Yy>
                new ContentMatchSet(new byte?[]
                {
                    0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                    0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                    0x79, 0x3E
                }, GetMacrovisionVersion, "Macrovision Protected Application"),
            };

            return MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
        }

        private List<string> FindProductFromVersion(List<string> resultsList)
        {
            // If we have an invalid result list
            if (resultsList == null || resultsList.Count == 0)
                return resultsList;

            List<string> VerifiedCDS300Versions = new List<string>()
            {
                // Found in "Les Paul & Friends" (Barcode 4 98806 834170).
                "2.90.044"
            };

            List<string> UnverifiedSafeCastVersions = new List<string>()
            {
                // Found in "Dreamweaver MX 2004 v7.0.1" according to https://web.archive.org/web/20210331144912/https://protectionid.net/.
                "2.42.000",
                // Found in "ArcSoft Media Card Companion v1.0" according to https://web.archive.org/web/20210331144912/https://protectionid.net/.
                "2.50.030",
                // Found in "Autodesk Inventor Professional v9.0" according to https://web.archive.org/web/20210331144912/https://protectionid.net/.
                "2.51.000",
                // Found in "Adobe Photoshop CS2" according to https://web.archive.org/web/20210331144912/https://protectionid.net/.
                "2.67.010"
            };

            List<string> VerifiedSafeCastVersions = new List<string>()
            {
                // Found in Redump entry 83145.
                "2.11.010",
                // Found in IA items "cdrom-turbotax-2002", "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282", and "TurboTax_Premier_Tax_Year_2002_for_Windows_v02.00Z-R_Intuit_352283_2002".
                "2.16.050",
                // Found in Redump entry 74384 (Semi-confirmed) and "Data Becker Web To Date v3.1" according to https://web.archive.org/web/20210331144912/https://protectionid.net/ (Unconfirmed).
                "2.60.030"
            };

            List<string> UnverifiedSafeDiscVersions = new List<string>()
            {
                // Currently only found in a pirate compilation disc: IA item "cdrom-classic-fond-58".
                "1.01.045"
            };

            List<string> VerifiedSafeDiscVersions = new List<string>()
            {
                // Found in Redump entry 66005.
                "1.00.025",
                // Found in Redump entries 1882 and 30049.
                "1.00.026",
                // Found in Redump entries 31575 and 41923.
                "1.00.030",
                // Found in Redump entries 1883 and 42114.
                "1.00.032",
                // Found in Redump entries 36223 and 40771.
                "1.00.035",
                // Found in Redump entries 42155 and 47574.
                "1.01.034",
                // Found in Redump entry 51459.
                "1.01.038",
                // Found in Redump entries 34562 and 63304. 
                "1.01.043",
                // Found in Redump entries 61731 and 81619.
                "1.01.044",
                // Found in Redump entries 29073 and 31149.
                "1.06.000",
                // Found in Redump entries 9718 and 46756.
                "1.07.000",
                // Found in Redump entries 12885 and 66210.
                "1.09.000",
                // Found in Redump entries 37523 and 66586.
                "1.11.000",
                // Found in Redump entries 21154 and 37982.
                "1.20.000",
                // Found in Redump entry 37920.
                "1.20.001",
                // Found in Redump entries 31526 and 55080.
                "1.30.010",
                // Found in Redump entries 9617 and 49552.
                "1.35.000",
                // Found in Redump entries 2595 and 30121.
                "1.40.004",
                // Found in Redump entries 44350 and 63323.
                "1.41.000",
                // Found in Redump entries 37832 and 42091.
                "1.41.001",
                // Found in Redump entries 30555 and 55078.
                "1.45.011",
                // Found in Redump entries 28810 and 62935.
                "1.50.020",
                // Found in Redump entries 72195 and 73502.
                "2.05.030",
                // Found in Redump entries 38541 and 59462 and 81096.
                "2.10.030",
                // Found in Redump entries 55823 and 79476.
                "2.30.030",
                // Found in Redump entries 15312 and 48863.
                "2.30.031",
                // Found in Redump entries 9819 and 53658.
                "2.30.033",
                // Found in Redump entries 9846 and 65642.
                "2.40.010",
                // Found in Redump entries 23786 and 37478.
                "2.40.011",
                // Found in Redump entries 30022 and 75014.
                "2.51.020",
                // Found in Redump entries 31666 and 66852.
                "2.51.021",
                // Found in Redump entries 2064 and 47047.
                "2.60.052",
                // Found in Redump entries 13048 and 35385.
                "2.70.030",
                // Found in Redump entries 48101 and 64198.
                "2.72.000",
                // Found in Redump entries 32783 and 72743.
                "2.80.010",
                // Found in Redump entries 39273 and 59351.
                "2.80.011",
                // Found in Redump entries 52606 and 62505.
                "2.90.040",
                // Found in Redump entries 13230 and 68204.
                "3.10.020",
                // Found in Redump entries 36511 and 74338.
                "3.15.010",
                // Found in Redump entries 15383 and 35512.
                "3.15.011",
                // Found in Redump entries 30404 and 56748.
                "3.20.020",
                // Found in Redump entries 58625 and 91552.
                "3.20.022",
                // Found in Redump entries 20729 and 63813 (CD) and Redump entries 20728 and 64255 (DVD).
                "3.20.024",
                // Found in Redump entries 35382 and 79729 (CD) and Redump entry 74520 (DVD).
                "4.00.000",
                // Found in Redump entries 8842 and 83017 (CD) and Redump entries 15614 and 38143 (DVD).
                "4.00.001",
                // Found in Redump entries 42034 and 71646 (CD) and Redump entries 78980 and 86196 (DVD). 
                "4.00.002",
                // Found in Redump entries 60021 and 68551 (CD) and Redump entries 51597 and 83408 (DVD).
                "4.00.003",
                // Found in Redump entries 58990 and 80776 (CD) and Redump entries 65569 and 76813 (DVD).
                "4.50.000",
                // Found in Redump entries 45686 and 46765 (CD) and Redump entries 45469 and 50682 (DVD). 
                "4.60.000",
                // Found in Redump entry 56320 (CD) and Redump entries 34783 and 66403 (DVD).
                "4.70.000",
                // Found in Redump entries 64145 and 78543 (CD only).
                "4.80.000",
                // Found in Redump entries 52523 and 76346 (DVD only). 
                "4.81.000",
                // Found in Redump entries 20434 and 31766 (DVD only). 
                "4.85.000", 
                // Found in Redump entries 56319 and 66333 (DVD only).
                "4.90.000",
                // Found in Redump entries 58573 and 78976 (CD) and redump entries 11347 and 29069 (DVD). 
                "4.90.010"
            };

            resultsList = resultsList.Select(s =>
            {
                for (int i = 0; i < VerifiedCDS300Versions.Count(); i++)
                {
                    if (s.StartsWith("Macrovision Protected Application") && (s.Contains(VerifiedCDS300Versions[i])))
                        return s.Replace("Macrovision Protected Application", "CDS-300");
                }
                return s;
            })
            .Where(s => s != null)
            .ToList();

            resultsList = resultsList.Select(s =>
            {
                for (int i = 0; i < UnverifiedSafeCastVersions.Count(); i++)
                {
                    if (s.StartsWith("Macrovision Protected Application") && (s.Contains(UnverifiedSafeCastVersions[i])))
                        return s.Replace("Macrovision Protected Application", "SafeCast (Unverified - Please report to us on GitHub)");
                }
                return s;
            })
            .Where(s => s != null)
            .ToList();

            resultsList = resultsList.Select(s =>
            {
                for (int i = 0; i < VerifiedSafeCastVersions.Count(); i++)
                {
                    if (s.StartsWith("Macrovision Protected Application") && (s.Contains(VerifiedSafeCastVersions[i])))
                        return s.Replace("Macrovision Protected Application", "SafeCast");
                }
                return s;
            })
            .Where(s => s != null)
            .ToList();

            resultsList = resultsList.Select(s =>
            {
                for (int i = 0; i < UnverifiedSafeDiscVersions.Count(); i++)
                {
                    if (s.StartsWith("Macrovision Protected Application") && (s.Contains(UnverifiedSafeDiscVersions[i])))
                        return s.Replace("Macrovision Protected Application", "SafeDisc (Unverified - Please report to us on GitHub)");
                }
                return s;
            })
            .Where(s => s != null)
            .ToList();

            resultsList = resultsList.Select(s =>
            {
                for (int i = 0; i < VerifiedSafeDiscVersions.Count(); i++)
                {
                    if (s.StartsWith("Macrovision Protected Application") && (s.Contains(VerifiedSafeDiscVersions[i])))
                        return s.Replace("Macrovision Protected Application", "SafeDisc");
                }
                    return s;
            })
            .Where(s => s != null)
            .ToList();

            // Get distinct and order
            return resultsList.Distinct().OrderBy(s => s).ToList();
        }

        private List<string> CleanResultList(List<string> resultsList)
        {
            // If we have an invalid result list
            if (resultsList == null || resultsList.Count == 0)
                return resultsList;

            // Cache the version expunged string
            string versionExpunged = GetSafeDisc320to4xVersion(null, null, null);

            // Clean SafeCast results
            if (resultsList.Any(s => s == "SafeCast") && resultsList.Any(s => s.StartsWith("Macrovision Protected Application")))
            {
                resultsList = resultsList.Select(s =>
                {
                    if (s.StartsWith("Macrovision Protected Application"))
                        return s.Replace("Macrovision Protected Application", "SafeCast");
                    else if (s == "SafeCast" || s.EndsWith(versionExpunged))
                        return null;
                    else
                        return s;
                })
                .Where(s => s != null)
                .ToList();
            }

            // Clean SafeDisc results
            if (resultsList.Any(s => s == "SafeDisc") && resultsList.Any(s => s.StartsWith("Macrovision Protected Application")))
            {
                resultsList = resultsList.Select(s =>
                {
                    if (s.StartsWith("Macrovision Protected Application"))
                        return s.Replace("Macrovision Protected Application", "SafeDisc");
                    else if (s == "SafeDisc" || s.EndsWith(versionExpunged))
                        return null;
                    else
                        return s;
                })
                .Where(s => s != null)
                .ToList();
            }

            // Clean incorrect version expunged results
            if (resultsList.Any(s => s.StartsWith("Macrovision Protected Application")) && resultsList.Any(s => s.EndsWith(versionExpunged)))
                resultsList = resultsList.Where(s => !s.EndsWith(versionExpunged)).ToList();

            // Get distinct and order
            return resultsList.Distinct().OrderBy(s => s).ToList();
        }
    }
}
