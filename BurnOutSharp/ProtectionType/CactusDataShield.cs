using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CactusDataShield : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // DATA.CDS
                new ContentMatchSet(new byte?[] { 0x44, 0x41, 0x54, 0x41, 0x2E, 0x43, 0x44, 0x53 }, "Cactus Data Shield 200"),

                // \*.CDS
                new ContentMatchSet(new byte?[] { 0x5C, 0x2A, 0x2E, 0x43, 0x44, 0x53 }, "Cactus Data Shield 200"),

                // CDSPlayer
                new ContentMatchSet(new byte?[] { 0x43, 0x44, 0x53, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72 }, "Cactus Data Shield 200"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
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
