using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
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
                if (includeDebug) Console.WriteLine(ex);
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
                if (includeDebug) Console.WriteLine(ex);
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
            switch (nex.Model.Stub?.Header?.NewExeHeaderAddr)
            {
                case 0x84b0:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x11, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = true };

                case 0x3e10:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3e50:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3c20:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3c30:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x22, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3660:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x40, ArchiveEnd = 0x3c, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x36f0:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x48, ArchiveEnd = 0x44, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3770:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3780:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x37b0:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x37d0:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3c80:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3bd0:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3c10:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

                default:
                    return null;
            }
        }

        /// <summary>
        /// Checks a PE header to see if it matches a known signature
        /// </summary>
        /// <param name="pex">Portable executable to check</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
        private static FormatProperty? GetPEFormat(PortableExecutable pex)
        {
            if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3cf4
                && pex.GetFirstSection(".data")?.VirtualSize == 0x1528)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3cf4
                && pex.GetFirstSection(".data")?.VirtualSize == 0x1568)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d54)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d44)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d04)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            // Found in Binary.WiseCustomCalla
            else if (pex.OverlayAddress == 0x6200)
                return new FormatProperty { Dll = true, ArchiveStart = 0x62, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x3000)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x3800)
                return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x3a00)
                return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

            return null;
        }
    }
}
