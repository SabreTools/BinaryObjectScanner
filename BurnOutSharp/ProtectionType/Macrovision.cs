using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using static System.Net.WebRequestMethods;

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
            // Get the DOS stub from the executable, if possible
            var stub = nex?.DOSStubHeader;
            if (stub == null)
                return null;

            string cDilla = CDillaCheckNewExecutable(file, nex, includeDebug);
            if (!string.IsNullOrWhiteSpace(cDilla))
                return cDilla;

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

            // Get the .text section, if it exists
            string match = CheckSectionForProtection(file, includeDebug, pex, ".text");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .txt2 section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, ".txt2");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the CODE section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, "CODE");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .data section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, ".data");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Check for specific indications for individual Macrovision protections.

            List<string> resultsList = new List<string>();

            // Run C-Dilla PE checks
            string cDilla = CDillaCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(cDilla))
                resultsList.Add(cDilla);

            // Run SafeCast PE checks
            string safeCast = SafeCastCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(safeCast))
                resultsList.Add(safeCast);

            // Run SafeDisc PE checks
            string safeDisc = SafeDiscCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(safeDisc))
                resultsList.Add(safeDisc);

            // Run FLEXnet PE checks
            string flexnet = FLEXnetCheckPortableExecutable(file, pex, includeDebug);
            if (!string.IsNullOrWhiteSpace(flexnet))
                resultsList.Add(flexnet);

            if (resultsList != null && resultsList.Count > 0)
                return string.Join(", ", resultsList);

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Add all common Macrovision directory path checks here

            // Run C-Dilla directory checks
            var cDilla = CDillaCheckDirectoryPath(path, files);
            if (cDilla != null && !cDilla.IsEmpty)
                return cDilla;

            // Run SafeCast directory checks
            var safeCast = SafeCastCheckDirectoryPath(path, files);
            if (safeCast != null && !safeCast.IsEmpty)
                return safeCast;

            // Run SafeDisc directory checks
            var safeDisc = SafeDiscCheckDirectoryPath(path, files);
            if (safeDisc != null && !safeDisc.IsEmpty)
                return safeDisc;

            return MatchUtil.GetAllMatches(files, null, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Add all common Macrovision file path checks here

            // Run C-Dilla file checks
            string cDilla = CDillaCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(cDilla))
                return cDilla;

            // Run SafeCast file checks
            string safeCast = SafeCastCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(safeCast))
                return safeCast;

            // Run SafeDisc file checks
            string safeDisc = SafeDiscCheckFilePath(path);
            if (!string.IsNullOrWhiteSpace(safeDisc))
                return safeDisc;

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

        private string CheckSectionForProtection(string file, bool includeDebug, PortableExecutable pex, string sectionName)
        {
            // This subtract is needed because BoG_ starts before the section
            var sectionRaw = pex.ReadRawSection(sectionName, first: true, offset: -64);
            if (sectionRaw != null)
            {
                // TODO: Add more checks to help differentiate between SafeDisc and SafeCast.
                var matchers = new List<ContentMatchSet>
                {
                    // Checks for presence of two different strings to differentiate between SafeDisc and SafeCast.
                    new ContentMatchSet(new List<byte?[]>
                    {
                        // BoG_ *90.0&!!  Yy>
                        new byte?[]
                        {
                            0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                            0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                            0x79, 0x3E
                        },

                        // product activation library
                        new byte?[]
                        {
                            0x70, 0x72, 0x6F, 0x64, 0x75, 0x63, 0x74, 0x20,
                            0x61, 0x63, 0x74, 0x69, 0x76, 0x61, 0x74, 0x69,
                            0x6F, 0x6E, 0x20, 0x6C, 0x69, 0x62, 0x72, 0x61,
                            0x72, 0x79
                        },
                    }, GetMacrovisionVersion, "SafeCast"),

                    // TODO: Investigate likely false positive in Redump entry 74384.
                    // Unfortunately, this string is used throughout a wide variety of SafeDisc and SafeCast versions. If no previous checks are able to able to differentiate between them, then a generic result has to be given.
                    // BoG_ *90.0&!!  Yy>
                    new ContentMatchSet(new byte?[]
                    {
                        0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                        0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                        0x79, 0x3E
                    }, GetMacrovisionVersion, "SafeCast/SafeDisc"),

                    // (char)0x00 + (char)0x00 + BoG_
                    new ContentMatchSet(new byte?[] { 0x00, 0x00, 0x42, 0x6F, 0x47, 0x5F }, GetSafeDisc320to4xVersion, "SafeDisc"),
                };

                return MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
            }

            return null;
        }
    }
}
