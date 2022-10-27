using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// CactusDataShield was a copy protection originally developed by Midbar Technologies, which was then purchased by Macrovision in 2002 (https://variety.com/2002/digital/news/macrovision-acquires-midbar-cuts-ttr-link-1117875824/).
    /// Macrovision's product page for CDS: https://web.archive.org/web/20050215235405/http://www.macrovision.com/products/cds/index.shtml
    /// CDS-100 appears to function by attempting to prevent dumping/ripping the discs protected with it.
    /// CDS-200+ uses a dedicated audio player to play the music "legitimately".
    /// Patent relating to CDS-100: https://patents.google.com/patent/US6425098B1/
    /// Known CDS versions:
    /// CDS-100 ("The Loveparade Compilation 2001" by various artists (Barcode 74321 86986 2) (Likely Discogs Release Code [r155963]) and "World Of Our Own" (Limited Edition) by Westlife (Barcode 7 43218 98572 0) (Discogs Release Code [r1357706])).
    /// CDS-200 (PlayJ) (("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])) (Confirmed to be CDS-200 from https://www.cdrinfo.com/d7/content/cactus-data-shield-200?page=2).
    /// CDS200.0.4 - 3.0 build 16a (Redump entry 95036)
    /// CDS200.0.4 - 3.0 build 16c ("TMF Hitzone 20" by various artists (Barcode 7 31458 37062 8)).
    /// CDS200.0.4 - 4.1 build 2a ("Ich Habe Einen Traum" by Uwe Busse (Barcode 9 002723 251203)).
    /// CDS200.0.4 - 4.1 build 2e ("Hallucinations" by David Usher (Barcode 7 24359 30322 2)).
    /// CDS200.5.11.90 - 5.10.090 ("Finn 5 Fel!" by Gyllene Tider (Barcode 7 24357 10922 2)).
    /// CDS-300
    /// Further information:
    /// https://www.cdrinfo.com/d7/content/cactus-data-shield-200
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_cactus_data_shield.shtml
    /// </summary>
    public class CactusDataShield : IContentCheck, IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Limit these checks to Mac binaries
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // CDSPlayer
                    new ContentMatchSet(new byte?[] { 0x43, 0x44, 0x53, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72 }, "Cactus Data Shield 200"),

                    // yucca.cds
                    new ContentMatchSet(new byte?[] { 0x79, 0x75, 0x63, 0x63, 0x61, 0x2E, 0x63, 0x64, 0x73 }, "Cactus Data Shield 200"),
                };

                if (contentMatchSets != null && contentMatchSets.Any())
                    return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // \*.CDS
                    new ContentMatchSet(new byte?[] { 0x5C, 0x2A, 0x2E, 0x43, 0x44, 0x53 }, "Cactus Data Shield 200"),
                    
                    // DATA.CDS
                    new ContentMatchSet(new byte?[] { 0x44, 0x41, 0x54, 0x41, 0x2E, 0x43, 0x44, 0x53 }, "Cactus Data Shield 200"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rsrc section, if it exists
            var rsrcSectionRaw = pex.ReadRawSection(".rsrc", first: false);
            if (rsrcSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // CactusPJ
                    // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
                    // Modified version of the PlayJ Music Player specificaly for CDS, as indicated by the About page present when running the executable.
                    new ContentMatchSet(new byte?[] { 0x43, 0x61, 0x63, 0x74, 0x75, 0x73, 0x50, 0x4A }, "PlayJ Music Player (Cactus Data Shield 200)"),
                };

                string match = MatchUtil.GetFirstMatch(file, rsrcSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }
			
            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
                // Modified version of the PlayJ Music Player specificaly for CDS, as indicated by the About page present when running the executable.
                // The file "DATA16.BML" is also present on this disc but the name is too generic to check for.
                new PathMatchSet(new PathMatch("CACTUSPJ.exe", useEndsWith: true), "PlayJ Music Player (Cactus Data Shield 200)"),

                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]). 
                // In "Volumina! - Puur" (7 43218 63282 2), this file is composed of multiple PLJ files combined together.
                // In later versions, this file is a padded dummy file. ("Ich Habe Einen Traum" by Uwe Busse (Barcode 9 002723 251203)).
                new PathMatchSet(new PathMatch("YUCCA.CDS", useEndsWith: true), "Cactus Data Shield 200"),

                // TODO: Find samples of the following: 
                new PathMatchSet(new PathMatch("CDSPlayer.app", useEndsWith: true), GetVersion, "Cactus Data Shield"),
                new PathMatchSet(new PathMatch("wmmp.exe", useEndsWith: true), GetVersion, "Cactus Data Shield"),

                // Present on CDS-300, as well as SafeDisc. This is likely due to both protections being created by Macrovision.
                new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), Get00000001TMPVersion, "Cactus Data Shield 300 (Confirm presence of other CDS-300 files)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
                // Modified version of the PlayJ Music Player specificaly for CDS, as indicated by the About page present when running the executable.
                // The file "DATA16.BML" is also present on this disc but the name is too generic to check for.
                new PathMatchSet(new PathMatch("CACTUSPJ.exe", useEndsWith: true), "PlayJ Music Player (Cactus Data Shield 200)"),

                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]), 
                // In "Volumia! - Puur", this file is composed of multiple PLJ files combined together.
                // In later versions, this file is a padded dummy file. ("Ich Habe Einen Traum" by Uwe Busse (Barcode 9 002723 251203)).
                new PathMatchSet(new PathMatch("YUCCA.CDS", useEndsWith: true), "Cactus Data Shield 200"),

                // TODO: Find samples of the following: 
                new PathMatchSet(new PathMatch("CDSPlayer.app", useEndsWith: true), "Cactus Data Shield 200"),
                new PathMatchSet(new PathMatch("wmmp.exe", useEndsWith: true), "Cactus Data Shield 200"),

                // Present on CDS-300, as well as SafeDisc. This is likely due to both protections being created by Macrovision.
                new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), Get00000001TMPVersion, "Cactus Data Shield 300"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string Get00000001TMPVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // This file is present on both CDS-300 and SafeDisc.
            // Only one specific file size appears to be associated with CDS-300, so any files with a differing file size are discarded. If it is the correct file size, return it as valid.
            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                case 2_048:
                    return "(Confirm presence of other CDS-300 files)";
                default:
                    return null;
            }
        }
        // TODO: Simplify version checking.
        public static string GetVersion(string firstMatchedString, IEnumerable<string> files)
        {
            // Find the version.txt file first
            string versionPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("version.txt", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(versionPath))
            {
                string version = GetInternalVersion(versionPath);
                if (!string.IsNullOrWhiteSpace(version))
                    return version;
            }

            return "200";
        }

        private static string GetInternalVersion(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                using (var sr = new StreamReader(path, Encoding.Default))
                {
                    return $"{sr.ReadLine().Substring(3)} ({sr.ReadLine()})";
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
