using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class GFWL : IContentCheck, IPathCheck
    {
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

            string name = fvinfo?.FileDescription?.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Games for Windows - LIVE Zero Day Piracy Protection", StringComparison.OrdinalIgnoreCase))
                return $"Games for Windows LIVE - Zero Day Piracy Protection Module {Utilities.GetFileVersion(fileContent)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Games for Windows", StringComparison.OrdinalIgnoreCase))
                return $"Games for Windows LIVE {Utilities.GetFileVersion(fileContent)}";

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
                    // G + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + s + (char)0x00 +   + (char)0x00 + f + (char)0x00 + o + (char)0x00 + r + (char)0x00 +   + (char)0x00 + W + (char)0x00 + i + (char)0x00 + n + (char)0x00 + d + (char)0x00 + o + (char)0x00 + w + (char)0x00 + s + (char)0x00 +   + (char)0x00 + - + (char)0x00 +   + (char)0x00 + L + (char)0x00 + I + (char)0x00 + V + (char)0x00 + E + (char)0x00 +   + (char)0x00 + Z + (char)0x00 + e + (char)0x00 + r + (char)0x00 + o + (char)0x00 +   + (char)0x00 + D + (char)0x00 + a + (char)0x00 + y + (char)0x00 +   + (char)0x00 + P + (char)0x00 + i + (char)0x00 + r + (char)0x00 + a + (char)0x00 + c + (char)0x00 + y + (char)0x00 +   + (char)0x00 + P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x47, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                            0x73, 0x00, 0x20, 0x00, 0x66, 0x00, 0x6F, 0x00,
                            0x72, 0x00, 0x20, 0x00, 0x57, 0x00, 0x69, 0x00,
                            0x6E, 0x00, 0x64, 0x00, 0x6F, 0x00, 0x77, 0x00,
                            0x73, 0x00, 0x20, 0x00, 0x2D, 0x00, 0x20, 0x00,
                            0x4C, 0x00, 0x49, 0x00, 0x56, 0x00, 0x45, 0x00,
                            0x20, 0x00, 0x5A, 0x00, 0x65, 0x00, 0x72, 0x00,
                            0x6F, 0x00, 0x20, 0x00, 0x44, 0x00, 0x61, 0x00,
                            0x79, 0x00, 0x20, 0x00, 0x50, 0x00, 0x69, 0x00,
                            0x72, 0x00, 0x61, 0x00, 0x63, 0x00, 0x79, 0x00,
                            0x20, 0x00, 0x50, 0x00, 0x72, 0x00, 0x6F, 0x00,
                            0x74, 0x00, 0x65, 0x00, 0x63, 0x00, 0x74, 0x00,
                            0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00,
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "Games for Windows LIVE - Zero Day Piracy Protection Module"),

                    // G + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + s + (char)0x00 +   + (char)0x00 + f + (char)0x00 + o + (char)0x00 + r + (char)0x00 +   + (char)0x00 + W + (char)0x00 + i + (char)0x00 + n + (char)0x00 + d + (char)0x00 + o + (char)0x00 + w + (char)0x00 + s + (char)0x00 +   + (char)0x00 + - + (char)0x00 +   + (char)0x00 + L + (char)0x00 + I + (char)0x00 + V + (char)0x00 + E + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x47, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                            0x73, 0x00, 0x20, 0x00, 0x66, 0x00, 0x6F, 0x00,
                            0x72, 0x00, 0x20, 0x00, 0x57, 0x00, 0x69, 0x00,
                            0x6E, 0x00, 0x64, 0x00, 0x6F, 0x00, 0x77, 0x00,
                            0x73, 0x00, 0x20, 0x00, 0x2D, 0x00, 0x20, 0x00,
                            0x4C, 0x00, 0x49, 0x00, 0x56, 0x00, 0x45, 0x00, 
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "Games for Windows LIVE"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rdata section, if it exists
            var rdataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rdata"));
            if (rdataSection != null)
            {
                int sectionAddr = (int)rdataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rdataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // xlive.dll
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x78, 0x6C, 0x69, 0x76, 0x65, 0x2E, 0x64, 0x6C, 0x6C }, start: sectionAddr, end: sectionEnd),
                    "Games for Windows LIVE"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
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
                // Might be specifically GFWL/Gfwlivesetup.exe
                new PathMatchSet(new PathMatch("Gfwlivesetup.exe", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("xliveinstall.dll", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows LIVE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Might be specifically GFWL/Gfwlivesetup.exe
                new PathMatchSet(new PathMatch("Gfwlivesetup.exe", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("xliveinstall.dll", useEndsWith: true), "Games for Windows LIVE"),
                new PathMatchSet(new PathMatch("XLiveRedist.msi", useEndsWith: true), "Games for Windows LIVE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
