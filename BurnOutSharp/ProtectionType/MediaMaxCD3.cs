using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class MediaMaxCD3 : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section, if it exists
            var rdataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rdata"));
            if (rdataSection != null)
            {
                int sectionAddr = (int)rdataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rdataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // DllInstallSbcp
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x44, 0x6C, 0x6C, 0x49, 0x6E, 0x73, 0x74, 0x61,
                            0x6C, 0x6C, 0x53, 0x62, 0x63, 0x70
                        }, start: sectionAddr, end: sectionEnd),
                    "MediaMax CD-3"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rsrc section, if it exists
            var rsrcSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rsrc"));
            if (rsrcSection != null)
            {
                int sectionAddr = (int)rsrcSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rsrcSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // Cd3Ctl
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x43, 0x64, 0x33, 0x43, 0x74, 0x6C }, start: sectionAddr, end: sectionEnd),
                    "MediaMax CD-3"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .data section, if it exists
            var dataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".data"));
            if (dataSection != null)
            {
                int sectionAddr = (int)dataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)dataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // CD3 Launch Error
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x43, 0x44, 0x33, 0x20, 0x4C, 0x61, 0x75, 0x6E,
                            0x63, 0x68, 0x20, 0x45, 0x72, 0x72, 0x6F, 0x72
                        }, start: sectionAddr, end: sectionEnd),
                    "MediaMax CD-3"),
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
                new PathMatchSet(new PathMatch("LaunchCd.exe", useEndsWith: true), "MediaMax CD-3"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("LaunchCd.exe", useEndsWith: true), "MediaMax CD-3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
