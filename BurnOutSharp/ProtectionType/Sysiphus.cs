using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  BurnOutSharp.ExecutableType.Microsoft; 
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus : IContentCheck
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

            // Get the .data section, if it exists
            var dataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".data"));
            if (dataSection != null)
            {
                int sectionAddr = (int)dataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)dataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // V SUHPISYS
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53,
                            0x59, 0x53
                        }, start: sectionAddr, end: sectionEnd),
                    GetVersion, "Sysiphus"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // The version is reversed
            string version = new string(
                new ArraySegment<byte>(fileContent, positions[0] - 4, 4)
                    .Reverse()
                    .Select(b => (char)b)
                    .ToArray())
                .Trim();

            // Check for the DVD extra string
            string extra = new string(
                new ArraySegment<byte>(fileContent, positions[0] + "V SUHPISYS".Length, 3)
                    .Select(b => (char)b)
                    .ToArray());
            bool isDVD = extra == "DVD";

            if (char.IsNumber(version[0]) && char.IsNumber(version[2]))
                return isDVD ? $"DVD {version}" : version;

            return isDVD ? "DVD" : string.Empty;
        }
    }
}
