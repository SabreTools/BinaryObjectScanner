﻿using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// MediaMax CD-3 is a copy protection for audio CDs created by SunnComm, which once installed, restricted users by only allowing a limited number of copies to be made, and only using Windows Media Player.
    /// It appears to accomplish this using the official Windows Media Data Session Toolkit.
    /// List of discs known to contain MediaMax CD-3: https://en.wikipedia.org/wiki/List_of_compact_discs_sold_with_MediaMax_CD-3
    /// TODO: Add support for detecting the Mac version, which is present on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
    /// </summary>
    public class MediaMaxCD3 : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Used to detect "LicGen.exe", found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
            var name = pex.FileDescription;
            if (name?.StartsWith("LicGen Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"MediaMax CD-3";

            name = pex.ProductName;
            if (name?.StartsWith("LicGen Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"MediaMax CD-3";

            var cd3CtrlResources = pex.FindGenericResource("Cd3Ctl");
            if (cd3CtrlResources.Any())
                return $"MediaMax CD-3";

            var limitedProductionResources = pex.FindDialogBoxByItemTitle("This limited production advanced CD is not playable on your computer. It is solely intended for playback on standard CD players.");
            if (limitedProductionResources.Any())
                return $"MediaMax CD-3";

            // TODO: Investigate the following dialog item title resource
            // "This limited production advanced CD is not playable on your computer. It is solely intended for playback on standard CD players."

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("CD3 Launch Error")))
                    return "MediaMax CD-3";
            }

            // Get the export name table
            if (pex.Model.ExportTable?.ExportNameTable?.Strings != null)
            {
                if (pex.Model.ExportTable.ExportNameTable.Strings.Any(s => s == "DllInstallSbcp"))
                    return "MediaMax CD-3";
            }

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
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
