using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// MediaMax is a copy protection for audio CDs created by SunnComm, which once installed, restricted users by only allowing a limited number of copies to be made, and only using Windows Media Player.
    /// It appears to accomplish this using the official Windows Media Data Session Toolkit.
    /// List of discs known to contain MediaMax CD-3: https://en.wikipedia.org/wiki/List_of_compact_discs_sold_with_MediaMax_CD-3
    /// TODO: Add support for detecting the Mac version, which is present on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
    /// </summary>
    public class MediaMax : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            // Used to detect "LicGen.exe", found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
            if (name.OptionalStartsWith("LicGen Module", StringComparison.OrdinalIgnoreCase))
                return $"MediaMax CD-3";

            name = exe.ProductName;

            if (name.OptionalStartsWith("LicGen Module", StringComparison.OrdinalIgnoreCase))
                return $"MediaMax CD-3";

            if (exe.FindGenericResource("Cd3Ctl").Count > 0)
                return $"MediaMax CD-3";

            if (exe.FindDialogBoxByItemTitle("This limited production advanced CD is not playable on your computer. It is solely intended for playback on standard CD players.").Count > 0)
                return $"MediaMax CD-3";

            // TODO: Investigate the following dialog item title resource
            // "This limited production advanced CD is not playable on your computer. It is solely intended for playback on standard CD players."

            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("CD3 Launch Error")))
                    return "MediaMax CD-3";
            }

            // Get the export name table
            if (exe.ExportTable?.ExportNameTable?.Strings != null)
            {
                if (Array.Exists(exe.ExportTable.ExportNameTable.Strings, s => s == "DllInstallSbcp"))
                    return "MediaMax CD-3";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
                new(new List<PathMatch>
                {
                    // TODO: Verify if these are OR or AND
					// TODO: Verify that this is directly related to MediaMax CD-3.
                    new FilePathMatch("PlayDisc.exe"),
                    new FilePathMatch("PlayDisc.xml"),
                }, "MediaMax CD-3"),

                // Found on "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8)
                // "SCCD3X01.dll" should already be detected by the content checks, but not "SCCD3X02.dll".
                new(new List<PathMatch>
                {
                    // TODO: Verify if these are OR or AND
                    new FilePathMatch("SCCD3X01.dll"),
                    new FilePathMatch("SCCD3X02.dll"),
                }, "MediaMax CD-3"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8)
                new(new FilePathMatch("SCCD3X01.dll"), "MediaMax CD-3"),
                new(new FilePathMatch("SCCD3X02.dll"), "MediaMax CD-3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
