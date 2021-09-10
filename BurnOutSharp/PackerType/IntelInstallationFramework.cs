using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IContentCheck
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
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(fileContent)}";
            }

            name = fvinfo?.ProductName?.Trim();
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(fileContent)}";
            }

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
                    // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + l + (char)0x00 + ( + (char)0x00 + R + (char)0x00 + ) + (char)0x00 +   + (char)0x00 + I + (char)0x00 + n + (char)0x00 + s + (char)0x00 + t + (char)0x00 + a + (char)0x00 + l + (char)0x00 + l + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + F + (char)0x00 + r + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + w + (char)0x00 + o + (char)0x00 + r + (char)0x00 + k + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                            0x6C, 0x00, 0x28, 0x00, 0x52, 0x00, 0x29, 0x00,
                            0x20, 0x00, 0x49, 0x00, 0x6E, 0x00, 0x73, 0x00,
                            0x74, 0x00, 0x61, 0x00, 0x6C, 0x00, 0x6C, 0x00,
                            0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00,
                            0x6E, 0x00, 0x20, 0x00, 0x46, 0x00, 0x72, 0x00,
                            0x61, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x77, 0x00,
                            0x6F, 0x00, 0x72, 0x00, 0x6B, 0x00,
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "Intel Installation Framework"),

                    // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + l + (char)0x00 + ( + (char)0x00 + R + (char)0x00 + ) + (char)0x00 +   + (char)0x00 + I + (char)0x00 + n + (char)0x00 + s + (char)0x00 + t + (char)0x00 + a + (char)0x00 + l + (char)0x00 + l + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + F + (char)0x00 + r + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 + w + (char)0x00 + o + (char)0x00 + r + (char)0x00 + k + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                            0x6C, 0x00, 0x20, 0x00, 0x49, 0x00, 0x6E, 0x00,
                            0x73, 0x00, 0x74, 0x00, 0x61, 0x00, 0x6C, 0x00,
                            0x6C, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00,
                            0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x46, 0x00,
                            0x72, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                            0x77, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6B, 0x00,
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "Intel Installation Framework"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
