using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Serialization.Wrappers;
using WiseUnpacker;
using WiseUnpacker.EWISE;

namespace BinaryObjectScanner.Packer
{
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class WiseInstaller : IExtractableExecutable<NewExecutable>, IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // If we match a known header
            if (MatchesNEVersion(nex) != null)
                return "Wise Installation Wizard Module";

            // TODO: Investigate STUB.EXE in nonresident-name table

            // TODO: Don't read entire file
            var data = nex.ReadArbitraryRange();
            if (data == null)
                return null;

            var neMatchSets = new List<ContentMatchSet>
            {
                // WiseInst
                new(new byte?[] { 0x57, 0x69, 0x73, 0x65, 0x49, 0x6E, 0x73, 0x74 }, "Wise Installation Wizard Module"),

                // WiseMain
                new(new byte?[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E }, "Wise Installation Wizard Module"),
            };

            return MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
        }

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // If we match a known header
            if (GetPEFormat(pex) != null)
                return "Wise Installation Wizard Module";

            // TODO: Investigate STUB32.EXE in export directory table

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("WiseMain")))
                    return "Wise Installation Wizard Module";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("WiseMain")))
                    return "Wise Installation Wizard Module";
            }

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, NewExecutable nex, string outDir, bool includeDebug)
        {
            try
            {
                Directory.CreateDirectory(outDir);
                return Extractor.ExtractTo(file, outDir);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            try
            {
                Directory.CreateDirectory(outDir);
                return Extractor.ExtractTo(file, outDir);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks an NE header to see if it matches a known signature
        /// </summary>
        /// <param name="nex">New executable to check</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
        private static FormatProperty? MatchesNEVersion(NewExecutable nex)
        {
            // TODO: Offset is _not_ the EXE header address, rather where the data starts. Fix this.
            return (nex.Model.Stub?.Header?.NewExeHeaderAddr) switch
            {
                0x84b0 => new FormatProperty { ArchiveEnd = -1 },
                0x3e10 => new FormatProperty { ArchiveEnd = -1 },
                0x3e50 => new FormatProperty { ArchiveEnd = -1 },
                0x3c20 => new FormatProperty { ArchiveEnd = -1 },
                0x3c30 => new FormatProperty { ArchiveEnd = -1 },
                0x3660 => new FormatProperty { ArchiveEnd = 0x3c },
                0x36f0 => new FormatProperty { ArchiveEnd = 0x44 },
                0x3770 => new FormatProperty { ArchiveEnd = 0x4c },
                0x3780 => new FormatProperty { ArchiveEnd = 0x4c },
                0x37b0 => new FormatProperty { ArchiveEnd = 0x4c },
                0x37d0 => new FormatProperty { ArchiveEnd = 0x4c },
                0x3c80 => new FormatProperty { ArchiveEnd = 0x4c },
                0x3bd0 => new FormatProperty { ArchiveEnd = 0x4c },
                0x3c10 => new FormatProperty { ArchiveEnd = 0x4c },
                _ => null,
            };
        }

        /// <summary>
        /// Checks a PE header to see if it matches a known signature
        /// </summary>
        /// <param name="pex">Portable executable to check</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
        private static FormatProperty? GetPEFormat(PortableExecutable pex)
        {
            // Get the current format
            var current = new FormatProperty
            {
                ExecutableType = ExecutableType.PE,
                CodeSectionLength = (int?)pex.GetFirstSection(".text")?.VirtualSize ?? -1,
                DataSectionLength = (int?)pex.GetFirstSection(".data")?.VirtualSize ?? -1,
            };

            // Search known formats
            foreach (var format in FormatProperty.KnownFormats)
            {
                if (current.Equals(format))
                    return format;
            }

            // Found in Binary.WiseCustomCalla
            if (pex.OverlayAddress == 0x6200)
                return new FormatProperty { ArchiveEnd = 0x4c };

            return null;
        }
    }
}
