using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{

    public class CactusDataShield : IContentCheck, IPathCheck
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

                // The file "00000001.TMP" (with a filesize of 2,048 bytes) can be found in CDS-300, as well as SafeDisc.
                // Due to this file being used in both protections, this file is detected within the general Macrovision checks.
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

                // The file "00000001.TMP" (with a filesize of 2,048 bytes) can be found in CDS-300, as well as SafeDisc.
                // Due to this file being used in both protections, this file is detected within the general Macrovision checks.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
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
