using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
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
    public partial class Macrovision
    {
        /// <inheritdoc cref="BinaryObjectScanner.Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
        internal string CactusDataShieldCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .data/DATA section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("\\*.CDS")))
                    return "Cactus Data Shield 200";

                if (strs.Any(s => s.Contains("DATA.CDS")))
                    return "Cactus Data Shield 200";
            }

            // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
            // Modified version of the PlayJ Music Player specificaly for CDS, as indicated by the About page present when running the executable.
            var resources = pex.FindGenericResource("CactusPJ");
            if (resources.Any())
                return "PlayJ Music Player (Cactus Data Shield 200)";

            // Found in various files in "Les Paul & Friends" (Barcode 4 98806 834170).
            string name = pex.ProductName;
            if (name?.Equals("CDS300", StringComparison.OrdinalIgnoreCase) == true)
                return $"Cactus Data Shield 300";

            return null;
        }

        /// <inheritdoc cref="BinaryObjectScanner.Interfaces.IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
        internal ConcurrentQueue<string> CactusDataShieldCheckDirectoryPath(string path, IEnumerable<string> files)
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
                new PathMatchSet(new PathMatch("CDSPlayer.app", useEndsWith: true), GetCactusDataShieldVersion, "Cactus Data Shield"),
                new PathMatchSet(new PathMatch("wmmp.exe", useEndsWith: true), GetCactusDataShieldVersion, "Cactus Data Shield"),

                // The file "00000001.TMP" (with a filesize of 2,048 bytes) can be found in CDS-300, as well as SafeDisc.
                // Due to this file being used in both protections, this file is detected within the general Macrovision checks.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="BinaryObjectScanner.Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal string CactusDataShieldCheckFilePath(string path)
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

                // The file "00000001.TMP" (with a filesize of 2,048 bytes) can be found in CDS-300, as well as SafeDisc.
                // Due to this file being used in both protections, this file is detected within the general Macrovision checks.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetCactusDataShieldVersion(string firstMatchedString, IEnumerable<string> files)
        {
            // Find the version.txt file first
            string versionPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("version.txt", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(versionPath))
            {
                string version = GetCactusDataShieldInternalVersion(versionPath);
                if (!string.IsNullOrWhiteSpace(version))
                    return version;
            }

            return "200";
        }

        private static string GetCactusDataShieldInternalVersion(string path)
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
