using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CactusDataShield : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Both of these are found in Mac binaries
            return new List<ContentMatchSet>
            {
                // CDSPlayer
                new ContentMatchSet(new byte?[] { 0x43, 0x44, 0x53, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72 }, "Cactus Data Shield 200"),

                // yucca.cds
                new ContentMatchSet(new byte?[] { 0x79, 0x75, 0x63, 0x63, 0x61, 0x2E, 0x63, 0x64, 0x73 }, "Cactus Data Shield 200"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section, if it exists
            var dataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".data"));
            if (dataSection != null)
            {
                int sectionAddr = (int)dataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)dataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // \*.CDS
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x5C, 0x2A, 0x2E, 0x43, 0x44, 0x53 }, start: sectionAddr, end: sectionEnd),
                        "Cactus Data Shield 200"),
                    
                    // DATA.CDS
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x44, 0x41, 0x54, 0x41, 0x2E, 0x43, 0x44, 0x53 }, start: sectionAddr, end: sectionEnd),
                        "Cactus Data Shield 200"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CACTUSPJ.exe", useEndsWith: true), GetVersion, "Cactus Data Shield"),
                new PathMatchSet(new PathMatch("CDSPlayer.app", useEndsWith: true), GetVersion, "Cactus Data Shield"),
                new PathMatchSet(new PathMatch("PJSTREAM.DLL", useEndsWith: true), GetVersion, "Cactus Data Shield"),
                new PathMatchSet(new PathMatch("wmmp.exe", useEndsWith: true), GetVersion, "Cactus Data Shield"),
                new PathMatchSet(new PathMatch(".cds", useEndsWith: true), GetVersion, "Cactus Data Shield"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CACTUSPJ.exe", useEndsWith: true), "Cactus Data Shield 200"),
                new PathMatchSet(new PathMatch("CDSPlayer.app", useEndsWith: true), "Cactus Data Shield 200"),
                new PathMatchSet(new PathMatch("PJSTREAM.DLL", useEndsWith: true), "Cactus Data Shield 200"),
                new PathMatchSet(new PathMatch("wmmp.exe", useEndsWith: true), "Cactus Data Shield 200"),
                new PathMatchSet(new PathMatch(".cds", useEndsWith: true), "Cactus Data Shield 200"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

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
