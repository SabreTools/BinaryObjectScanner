using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BinaryObjectScanner.Utilities;
using BurnOutSharp.Wrappers;

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
