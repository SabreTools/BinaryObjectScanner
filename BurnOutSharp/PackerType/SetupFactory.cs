using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    public class SetupFactory : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // TODO: Implement resource finding instead of using the built in methods
            // Assembly information lives in the .rsrc section
            // I need to find out how to navigate the resources in general
            // as well as figure out the specific resources for both
            // file info and MUI (XML) info. Once I figure this out,
            // that also opens the doors to easier assembly XML checks.

            var fvinfo = Utilities.GetFileVersionInfo(file);

            string name = fvinfo?.LegalTrademarks?.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Setup Factory", StringComparison.OrdinalIgnoreCase))
                return $"Setup Factory {GetVersion(file, fileContent, null)}";

            name = fvinfo?.ProductName?.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Setup Factory", StringComparison.OrdinalIgnoreCase))
                return $"Setup Factory {GetVersion(file, fileContent, null)}";

            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rsrc section, if it exists
            var rsrcSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rsrc"));
            if (rsrcSection != null)
            {
                int sectionAddr = (int)rsrcSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rsrcSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // S + (char)0x00 + e + (char)0x00 + t + (char)0x00 + u + (char)0x00 + p + (char)0x00 +   + (char)0x00 + F + (char)0x00 + a + (char)0x00 + c + (char)0x00 + t + (char)0x00 + o + (char)0x00 + r + (char)0x00 + y + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00,
                            0x70, 0x00, 0x20, 0x00, 0x46, 0x00, 0x61, 0x00,
                            0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00,
                            0x79, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    GetVersion, "Setup Factory"),

                    // Longer version of the check that can be used if false positves become an issue:
                    // S + (char)0x00 + e + (char)0x00 + t + (char)0x00 + u + (char)0x00 + p + (char)0x00 +   + (char)0x00 + F + (char)0x00 + a + (char)0x00 + c + (char)0x00 + t + (char)0x00 + o + (char)0x00 + r + (char)0x00 + y + (char)0x00 +   + (char)0x00 + i + (char)0x00 + s + (char)0x00 +   + (char)0x00 + a + (char)0x00 +   + (char)0x00 + t + (char)0x00 + r + (char)0x00 + a + (char)0x00 + d + (char)0x00 + e + (char)0x00 + m + (char)0x00 + a + (char)0x00 + r + (char)0x00 + k + (char)0x00 +   + (char)0x00 + o + (char)0x00 + f + (char)0x00 +   + (char)0x00 + I + (char)0x00 + n + (char)0x00 + d + (char)0x00 + i + (char)0x00 + g + (char)0x00 + o + (char)0x00 +   + (char)0x00 + R + (char)0x00 + o + (char)0x00 + s + (char)0x00 + e + (char)0x00 +   + (char)0x00 + C + (char)0x00 + o + (char)0x00 + r + (char)0x00 + p + (char)0x00 + o + (char)0x00 + r + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00
                    // new ContentMatchSet(
                    //     new ContentMatch(new byte?[]
                    //     {
                    //         0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00,
                    //         0x70, 0x00, 0x20, 0x00, 0x46, 0x00, 0x61, 0x00,
                    //         0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00,
                    //         0x79, 0x00, 0x20, 0x00, 0x69, 0x00, 0x73, 0x00,
                    //         0x20, 0x00, 0x61, 0x00, 0x20, 0x00, 0x74, 0x00,
                    //         0x72, 0x00, 0x61, 0x00, 0x64, 0x00, 0x65, 0x00,
                    //         0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x6B, 0x00,
                    //         0x20, 0x00, 0x6F, 0x00, 0x66, 0x00, 0x20, 0x00,
                    //         0x49, 0x00, 0x6E, 0x00, 0x64, 0x00, 0x69, 0x00,
                    //         0x67, 0x00, 0x6F, 0x00, 0x20, 0x00, 0x52, 0x00,
                    //         0x6F, 0x00, 0x73, 0x00, 0x65, 0x00, 0x20, 0x00,
                    //         0x43, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x70, 0x00,
                    //         0x6F, 0x00, 0x72, 0x00, 0x61, 0x00, 0x74, 0x00,
                    //         0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00
                    //     }, start: sectionAddr, end: sectionEnd),
                    // GetVersion, "Setup Factory"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        // TODO: Add extraction, which is possible but the only tools available that can
        // do this seem to be Universal Extractor 2 and InstallExplorer (https://totalcmd.net/plugring/InstallExplorer.html)
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }
    
        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // Check the manifest version first
            string version = Utilities.GetManifestVersion(fileContent);
            if (!string.IsNullOrEmpty(version))
                return version;
            
            // Then check the file version
            version = Utilities.GetFileVersion(fileContent);
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}
