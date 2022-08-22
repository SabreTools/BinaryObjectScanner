using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
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
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Used to detect "LicGen.exe", found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
            string name = pex.FileDescription;
            if (name?.StartsWith("LicGen Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"MediaMax CD-3";

            name = pex.ProductName;
            if (name?.StartsWith("LicGen Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"MediaMax CD-3";

            var resource = pex.FindResource(dataContains: "Cd3Ctl");
            if (resource != null)
                return $"MediaMax CD-3";

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // CD3 Launch Error
                    new ContentMatchSet(new byte?[]
                    {
                        0x43, 0x44, 0x33, 0x20, 0x4C, 0x61, 0x75, 0x6E,
                        0x63, 0x68, 0x20, 0x45, 0x72, 0x72, 0x6F, 0x72
                    }, "MediaMax CD-3"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rdata section, if it exists
            if (pex.ResourceDataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // DllInstallSbcp
                    new ContentMatchSet(new byte?[]
                    {
                        0x44, 0x6C, 0x6C, 0x49, 0x6E, 0x73, 0x74, 0x61,
                        0x6C, 0x6C, 0x53, 0x62, 0x63, 0x70
                    }, "MediaMax CD-3"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.ResourceDataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "All That I Am" by Santana (Barcode 8 2876-59773-2 6)
                new PathMatchSet(new List<PathMatch>
                {
                    // TODO: Verify if these are OR or AND
					// TODO: Verify that this is directly related to MediaMax CD-3.
                    new PathMatch("PlayDisc.exe", useEndsWith: true),
                    new PathMatch("PlayDisc.xml", useEndsWith: true),
                }, "MediaMax CD-3"),

                // Found on "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8)
                // "SCCD3X01.dll" should already be detected by the content checks, but not "SCCD3X02.dll".
                new PathMatchSet(new List<PathMatch>
                {
                    // TODO: Verify if these are OR or AND
                    new PathMatch("SCCD3X01.dll", useEndsWith: true),
                    new PathMatch("SCCD3X02.dll", useEndsWith: true),
                }, "MediaMax CD-3"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found on "Contraband" by Velvet Revolver (Barcode 8 28766 05242 8)
                new PathMatchSet(new PathMatch("SCCD3X01.dll", useEndsWith: true), "MediaMax CD-3"),
                new PathMatchSet(new PathMatch("SCCD3X02.dll", useEndsWith: true), "MediaMax CD-3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
