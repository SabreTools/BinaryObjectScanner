using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class SmartE : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .edata section, if it exists
            string match = GetMatchForSection(file, pex.GetFirstSectionData(".edata"), includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .idata section, if it exists
            match = GetMatchForSection(file, pex.GetFirstSectionData(".idata"), includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .rdata section, if it exists
            match = GetMatchForSection(file, pex.GetFirstSectionData(".rdata"), includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .tls section, if it exists
            var tlsSectionRaw = pex.GetLastSectionData(".tls");
            match = GetMatchForSection(file, tlsSectionRaw, includeDebug);
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
        private string GetMatchForSection(Models.PortableExecutable.SectionHeader section, string file, byte[] sectionContent, bool includeDebug)
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

            return MatchUtil.GetFirstMatch(file, sectionContent, matchers, includeDebug);
        }

        /// <summary>
        /// Check a section for the SmartE string(s)
        /// </summary>
        private string GetMatchForSection(string file, byte[] sectionContent, bool includeDebug)
        {
            if (sectionContent == null)
                return null;

            var matchers = new List<ContentMatchSet>
            {
                // BITARTS
                new ContentMatchSet(new byte?[] { 0x42, 0x49, 0x54, 0x41, 0x52, 0x54, 0x53 }, "SmartE"),
            };

            return MatchUtil.GetFirstMatch(file, sectionContent, matchers, includeDebug);
        }
    }
}
