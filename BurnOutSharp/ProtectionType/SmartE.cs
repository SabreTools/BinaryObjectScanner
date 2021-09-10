using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SmartE : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .edata section, if it exists
            var edataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".edata"));
            string match = GetMatchForSection(edataSection, file, fileContent, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .idata section, if it exists
            var idataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".idata"));
            match = GetMatchForSection(idataSection, file, fileContent, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .rdata section, if it exists
            var rdataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rdata"));
            match = GetMatchForSection(rdataSection, file, fileContent, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .tls section, if it exists
            var tlsSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".tls"));
            match = GetMatchForSection(tlsSection, file, fileContent, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch($"{Path.DirectorySeparatorChar}00001.TMP", useEndsWith: true),
                    new PathMatch($"{Path.DirectorySeparatorChar}00002.TMP", useEndsWith: true)
                 }, "SmartE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00001.TMP", useEndsWith: true), "SmartE"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00002.TMP", useEndsWith: true), "SmartE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        /// <summary>
        /// Check a section for the SmartE string(s)
        /// </summary>
        private string GetMatchForSection(SectionHeader section, string file, byte[] fileContent, bool includeDebug)
        {
            if (section == null)
                return null;

            int sectionAddr = (int)section.PointerToRawData;
            int sectionEnd = sectionAddr + (int)section.VirtualSize;
            var matchers = new List<ContentMatchSet>
            {
                // BITARTS
                new ContentMatchSet(
                    new ContentMatch(new byte?[] { 0x42, 0x49, 0x54, 0x41, 0x52, 0x54, 0x53 }, start: sectionAddr, end: sectionEnd),
                "SmartE"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }
    }
}
