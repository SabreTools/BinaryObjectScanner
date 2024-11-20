using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// DiscGuard (https://web.archive.org/web/19990208210940/http://www.ttrtech.com/discgard.htm) was a copy protection created by TTR (https://web.archive.org/web/19981212021829/http://ttrtech.com/) for protecting software.
    /// They also created a similar copy protection for audio CDs called DiscAudit (https://web.archive.org/web/19981206095259/http://www.ttrtech.com/discaudi.htm).
    /// It seems to work by encrypting the main game executable, and by having a "signature" that is supposedly only present on a genuine disc (https://www.cdmediaworld.com/hardware/cdrom/cd_protections_discguard.shtml).
    /// Due to the fact that these games can seemingly be burned to CD-R under the right conditions using CloneCD, it likely isn't using twin sectors or DPM (https://www.gameburnworld.com/protections_discguard.shtml).
    /// It uses subchannels, at a minimum, to create this signature. Redump entry 79374 is confirmed to be affected by subchannels (IA item "ii-seven-kingdoms-ii-the-fryhtan-wars-dic-dump-1999").
    /// If a CUE image is used, the protection states to insert the original disc. If a 00'd SUB file is used, the protection states to remove any virtual drive software.
    /// With a properly dumped SUB, the game seemingly begins to play as intended.
    /// DiscGuard is seemingly able to be detect so-called "active debuggers", based on text found in "Alternate.exe" (Redump entry 31914) and "Alt.exe" (Redump entries 46743, 46961, 79284, and 79374).
    /// There's a reference to a file "Dg.vbn" being present in "Alternate.exe" (Redump entry 31914) and "Alt.exe" (Redump entries 46743, 46961, 79284, and 79374), but no copy has been found in any sample so far.
    /// There are several distinct versions of DiscGuard, with the earliest only being present on one known game (Redump entry 31914).
    /// "Alternate.exe" in the earlier version is "Alt.exe" in the rest.
    /// "TTR1.DLL" and "TTR2.DLL" in the earlier version appear to be "T111.DLL" and "T222.DLL" in the rest, respectively.
    /// "IOSLink.sys" and "IOSLink.vxd" seem to be the same throughout all versions, even down to the reported File Version in "IOSLink.sys".
    /// There are other various DLLs, but their names seem to be different on every disc. Their filenames all seem to start with the letter "T", and it seems to be one of these specifically that the protected executable imports.
    /// These outlying DLLs are:
    /// "T29.dll" (Redump entry 31914).
    /// "T5375.dll" (Redump entry 79284).
    /// "TD352.dll" and "TE091.dll" (Redump entry 46743).
    /// "T71E1.dll" and "T7181.dll" (Redump entry 46961).
    /// "TA0E4.DLL" (Redump entry 79374).
    /// Further discs that are noted to contain DiscGuard:
    /// https://web.archive.org/web/19990503082646/http://www.ttrtech.com/prmakh.htm
    /// https://web.archive.org/web/19990209180542/http://www.ttrtech.com/pr2cont.htm
    /// </summary>
    public class DiscGuard : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        // TODO: Add checks for the game executables, which seem likely to contain some kind of indicators that can be checked for. The game executables all seem to import "Ord(1)" from one of the varying DLLs present.
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in "IOSLinksys.dll" (Redump entries 31914, 46743, 46961, 79284, and 79374).
            var name = pex.FileDescription;
            if (name?.StartsWith("IOSLinkNT", StringComparison.OrdinalIgnoreCase) == true)
                return "DiscGuard";

            // Found in "T29.dll" (Redump entry 31914).
            if (name?.StartsWith("TTR Technologies DiscGuard (tm)", StringComparison.OrdinalIgnoreCase) == true)
                return $"DiscGuard {GetVersion(pex)}";

            // Found in "T29.dll" (Redump entry 31914).
            name = pex.ProductName;
            if (name?.StartsWith("DiscGuard (tm)", StringComparison.OrdinalIgnoreCase) == true)
                return "DiscGuard";

            // Found in "IOSLinksys.dll" (Redump entries 31914, 46743, 46961, 79284, and 79374).
            name = pex.ProductName;
            if (name?.StartsWith("TTR Technologies Ltd. DiscGuard (tm)", StringComparison.OrdinalIgnoreCase) == true)
                return "DiscGuard";

            // Found in "Alternate.exe" (Redump entry 31914) and "Alt.exe" (Redump entries 46743, 46961, 79284, and 79374).
            var resources = pex.FindStringTableByEntry("DiscGuard")
                .Concat(pex.FindStringTableByEntry("The file Dg.vbn was not found."))
                .Concat(pex.FindStringTableByEntry("The file IosLink.VxD was not found."))
                .Concat(pex.FindStringTableByEntry("The file IosLink.sys was not found."));
            if (resources.Any())
                return "DiscGuard";

            // Get the .vbn section, if it exists
            if (pex.ContainsSection(".vbn"))
            {
                var vbnData = pex.GetFirstSectionData(".vbn");
                if (vbnData != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // Found in "T29.dll" (Redump entry 31914).
                        // This check should be as long as the following check, as this data is nearly identical (including length) in the original files, but for some reason the section ends early, causing part of the remaining data to not be part of a section.
                        new(new byte?[]
                        {
                            0x7B, 0x39, 0x8F, 0x07, 0x47, 0xE9, 0x96, 0x8C, 0xCA, 0xB2, 0x5C, 0x50,
                            0xC7, 0x5A, 0x18, 0xBD, 0x75, 0xB5, 0x68, 0x6A, 0x78, 0xB5, 0xCF, 0xF2,
                            0xBE, 0xB3, 0xDB, 0xE9, 0x4E, 0x87, 0x8D, 0x46, 0x63, 0x0A, 0x54, 0xB8,
                            0x4F, 0x85, 0x60, 0x2C, 0x06, 0xEC, 0xBD, 0x75, 0xF5, 0x6A, 0x6E, 0x35,
                            0x4D, 0x5A, 0x8B, 0xF4, 0x12, 0x15, 0x23, 0xC8, 0xE9, 0x80, 0x01, 0x10,
                            0xFE, 0xDB, 0xC6, 0x70, 0x1D, 0xC1, 0x4D, 0xAE, 0x9E, 0xE1, 0x01, 0xAA,
                            0x9E, 0x50, 0x50, 0xC5, 0x66, 0x80, 0xC0, 0xA2, 0x2F, 0xA9, 0x7A, 0x3B,
                            0x48, 0x74, 0x9D, 0x17, 0x33, 0x5D, 0x4C, 0x84, 0xD9, 0x54, 0xC4, 0x08,
                            0xCC, 0x10, 0x2A, 0xF6, 0x91, 0x40, 0x51, 0xD3, 0xF5, 0x9A, 0x07, 0xE7,
                            0xAB, 0xE9, 0x0B, 0xAD, 0xD4, 0x3A, 0xEC, 0xBA, 0x4B, 0x6C, 0xD2, 0x82,
                            0x0D, 0xF5, 0x49, 0x83, 0x8E, 0xAB, 0x85, 0x92, 0x78, 0x1D, 0x69, 0x1E,
                            0x44, 0xC6, 0xF6, 0xB4, 0x5F, 0x5F, 0xC2, 0x48, 0x5A, 0xED, 0x43, 0xD3,
                            0xA4, 0x41, 0x81
                        }, GetVersion, "DiscGuard"),

                        // Found in "T5375.dll" (Redump entry 79284), "TD352.dll" and "TE091.dll" (Redump entry 46743), "T71E1.dll" and "T7181.dll" (Redump entry 46961), and "TA0E4.DLL" (Redump entry 79374).
                        new(new byte?[]
                        {
                            0x7B, 0x39, 0x8F, 0x07, 0x45, 0xE9, 0x96, 0x8C, 0xCA, 0xB2, 0x5C, 0x50,
                            0xC7, 0x5A, 0x18, 0xBD, 0x75, 0xB5, 0x68, 0x6A, 0x78, 0xB5, 0xCF, 0xF2,
                            0xBE, 0xB3, 0xDB, 0xE9, 0x4E, 0x87, 0x8D, 0x46, 0x63, 0x0A, 0x54, 0xB8,
                            0x4F, 0x85, 0x60, 0x2C, 0x06, 0xEC, 0xBD, 0x75, 0xC6, 0xEB, 0x6E, 0x35,
                            0xED, 0xD0, 0x8B, 0xF4, 0x15, 0x12, 0x3D, 0xF3, 0x65, 0xF7, 0x01, 0x10,
                            0xF8, 0xFA, 0xC6, 0x70, 0x1D, 0xC1, 0x4D, 0xAE, 0x9E, 0xE1, 0x01, 0xAA,
                            0x9E, 0x50, 0x50, 0xC5, 0x66, 0x80, 0xC0, 0xA2, 0x2F, 0xA9, 0x7A, 0x3B,
                            0x48, 0x74, 0x9D, 0x17, 0x33, 0x5D, 0x4C, 0x84, 0xD9, 0x54, 0xC4, 0x08,
                            0xCC, 0x10, 0x2A, 0xF6, 0x91, 0x40, 0x51, 0xD3, 0x41, 0x9A, 0x07, 0xE7,
                            0xAB, 0xE9, 0x0B, 0xAD, 0xD4, 0x3A, 0xEC, 0xBA, 0xAF, 0x69, 0xD2, 0x82,
                            0x67, 0xF5, 0x49, 0x83, 0x8E, 0xAB, 0x85, 0x92, 0x04, 0x19, 0x69, 0x1E,
                            0x44, 0xC6, 0xF6, 0xB4, 0x5F, 0x5F, 0xC2, 0x48, 0x5A, 0xED, 0x43, 0xD3,
                            0xA4, 0x41, 0x81, 0xAF, 0xB8, 0xCB, 0x46, 0xE3, 0xDA, 0x05, 0x36, 0xEA,
                            0x05, 0xF5, 0xB9, 0xCE, 0x5F, 0x9A, 0xF5, 0x7D, 0x9E, 0x64, 0x66, 0xF9,
                            0xA5, 0x7C, 0x4D, 0x1D, 0x1D, 0x95, 0x02, 0x52, 0x66, 0x23, 0xEF, 0xFF,
                            0xEC, 0x63, 0x11, 0xEB, 0xF6, 0x66, 0x8F, 0x2B, 0xCF, 0x07, 0x50, 0x18,
                            0xBE, 0x58, 0xCA, 0x08, 0x24, 0xAD, 0x81, 0x1A, 0xAB, 0x0E, 0x2D, 0x16,
                            0x38, 0xAB, 0x22, 0xB5, 0xA8, 0xF0, 0x7D, 0x2E, 0xAF, 0x5E, 0xEA, 0x02,
                            0x72, 0x20, 0x14, 0x19, 0x0E, 0x31, 0xF3, 0xD0, 0x40, 0xAE, 0xA2, 0xD5,
                            0x0A, 0xA7, 0xB7, 0xAE, 0x02, 0xCF, 0xAC, 0x5F, 0xB8, 0x03, 0x15, 0x80,
                            0x9A, 0x58, 0x5C, 0x03, 0x28, 0x31, 0x9E, 0xB8, 0x21, 0x5D, 0x07, 0xB3,
                            0xB9, 0xEC, 0x75, 0xBA, 0xC2, 0xC8, 0x9D, 0x6F, 0x7A, 0xA1, 0x00, 0x8A
                        }, GetVersion, "DiscGuard"),
                    };

                    var match = MatchUtil.GetFirstMatch(file, vbnData, matchers, includeDebug);
                    if (!string.IsNullOrEmpty(match))
                        return match;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found together in seemingly every DiscGuard game (Redump entries 31914, 46743, 46961, 79284, and 79374).
                new(
                [
                    new FilePathMatch("IOSLINK.VXD"),
                    new FilePathMatch("IOSLINK.SYS"),
                ], "DiscGuard"),

                // Found together in one DiscGuard game (Redump entry 31914).
                new(
                [
                    new FilePathMatch("TTR1.DLL"),
                    new FilePathMatch("TTR2.DLL"),
                ], "DiscGuard"),

                // Found together in most DiscGuard games (Redump entries 46743, 46961, 79284, and 79374).
                new(
                [
                    new FilePathMatch("T111.DLL"),
                    new FilePathMatch("T222.DLL"),
                ], "DiscGuard"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found together in seemingly every DiscGuard game (Redump entries 31914, 46743, 46961, 79284, and 79374).
                new(new FilePathMatch("IOSLINK.VXD"), "DiscGuard"),
                new(new FilePathMatch("IOSLINK.SYS"), "DiscGuard"),

                // IOSLINK.DLL doesn't seem to be present in any known samples, but a check for it was in the original BurnOut.
                new(new FilePathMatch("IOSLINK.DLL"), "DiscGuard (Unconfirmed check, report this to us on GitHub))"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        /// <inheritdoc/>
        private static string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            var version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version!;

            return "(Unknown Version)";
        }

        /// <inheritdoc/>
        private string? GetVersion(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            // Check the internal versions
            string version = GetInternalVersion(file);
            if (!string.IsNullOrEmpty(version))
                return version;

            return string.Empty;
        }

        private static string GetInternalVersion(string firstMatchedString)
        {
            try
            {
                using Stream fileStream = File.Open(firstMatchedString, FileMode.Open, FileAccess.Read, FileShare.Read);
                var pex = PortableExecutable.Create(fileStream);
                return pex?.GetInternalVersion() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
