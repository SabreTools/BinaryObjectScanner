using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class WTMCDProtect : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the CODE section, if it exists
            var codeSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith("CODE"));
            if (codeSection != null)
            {
                int sectionAddr = (int)codeSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)codeSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // wtmdum.imp
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x77, 0x74, 0x6D, 0x64, 0x75, 0x6D, 0x2E, 0x69,
                            0x6D, 0x70
                        }, start: sectionAddr, end: sectionEnd),
                    "WTM CD Protect"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .text section, if it exists
            var textSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".text"));
            if (textSection != null)
            {
                int sectionAddr = (int)textSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)textSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // WTM DIGITAL Photo Protect
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x57, 0x54, 0x4D, 0x20, 0x44, 0x49, 0x47, 0x49,
                            0x54, 0x41, 0x4C, 0x20, 0x50, 0x68, 0x6F, 0x74,
                            0x6F, 0x20, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63,
                            0x74
                        }, start: sectionAddr, end: sectionEnd),
                    "WTM Protection Viewer"),

                    // WTM Copy Protection Viewer
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x57, 0x54, 0x4D, 0x20, 0x43, 0x6F, 0x70, 0x79,
                            0x20, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74,
                            0x69, 0x6F, 0x6E, 0x20, 0x56, 0x69, 0x65, 0x77,
                            0x65, 0x72
                        }, start: sectionAddr, end: sectionEnd),
                    "WTM Protection Viewer"),
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
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("wtmfiles.dat", useEndsWith: true),
                    new PathMatch("Viewer.exe", useEndsWith: true),
                }, "WTM Protection Viewer"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Add ImageX.imp as a wildcard, if possilbe
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Image.imp", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("Image1.imp", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("imp.dat", useEndsWith: true), "WTM CD Protect"),
                new PathMatchSet(new PathMatch("wtmfiles.dat", useEndsWith: true), "WTM Protection Viewer"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
