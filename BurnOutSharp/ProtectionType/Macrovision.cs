using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
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

            string name = pex.FileDescription;

            // Present in "Diag.exe" files from SafeDisc 4.50.000+.
            if (name?.Equals("SafeDisc SRV Tool APP", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeDisc SRV Tool APP {GetSafeDiscDiagExecutableVersion(pex)}";

            name = pex.ProductName;

            // Present in "Diag.exe" files from SafeDisc 4.50.000+.
            if (name?.Equals("SafeDisc SRV Tool APP", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeDisc SRV Tool APP {GetSafeDiscDiagExecutableVersion(pex)}";

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
            // TODO: Add all common Macrovision directory path checks here

            ConcurrentQueue<string> results = new ConcurrentQueue<string>();

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

            return MatchUtil.GetAllMatches(files, null, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Add all common Macrovision file path checks here

            List<string> resultsList = new List<string>();

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

            return MatchUtil.GetFirstMatch(path, null, any: true);
        }

        static string GetMacrovisionVersion(string file, byte[] fileContent, List<int> positions)
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
                }, GetMacrovisionVersion, "SafeCast/SafeDisc"),
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
            if (resultsList.Any(s => s == "SafeCast") && resultsList.Any(s => s.StartsWith("SafeCast/SafeDisc")))
            {
                resultsList = resultsList.Select(s =>
                {
                    if (s.StartsWith("SafeCast/SafeDisc"))
                        return s.Replace("SafeCast/SafeDisc", "SafeCast");
                    else if (s == "SafeCast" || s.EndsWith(versionExpunged))
                        return null;
                    else
                        return s;
                })
                .Where(s => s != null)
                .ToList();
            }

            // Clean incorrect version expunged results
            if (resultsList.Any(s => s.StartsWith("SafeCast/SafeDisc")) && resultsList.Any(s => s.EndsWith(versionExpunged)))
                resultsList = resultsList.Where(s => !s.EndsWith(versionExpunged)).ToList();

            // Get distinct and order
            return resultsList.Distinct().OrderBy(s => s).ToList();
        }
    }
}
