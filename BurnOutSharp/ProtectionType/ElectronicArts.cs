using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class ElectronicArts : IContentCheck
    {
        // TODO: Verify this doesn't over-match
        // TODO: Do more research into the Cucko protection:
        //      - Reference to `EASTL` and `EAStdC` are standard for EA products and does not indicate Cucko by itself
        //      - There's little information outside of PiD detection that actually knows about Cucko
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // EASTL
                //new ContentMatchSet(new byte?[] { 0x45, 0x41, 0x53, 0x54, 0x4C }, "Cucko (EA Custom)"),

                // ereg.ea-europe.com
                new ContentMatchSet(new byte?[]
                {
                    0x65, 0x72, 0x65, 0x67, 0x2E, 0x65, 0x61, 0x2D,
                    0x65, 0x75, 0x72, 0x6F, 0x70, 0x65, 0x2E, 0x63,
                    0x6F, 0x6D
                }, Utilities.GetFileVersion, "EA CdKey Registration Module"),
            };
        }

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
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("Registration code installer program"))
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(file)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.Equals("EA DRM Helper", StringComparison.OrdinalIgnoreCase))
                return $"EA DRM Protection {Utilities.GetFileVersion(file)}";

            name = fvinfo?.InternalName?.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("CDCode", StringComparison.Ordinal))
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(file)}";

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
                    // EReg Config Form
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x45, 0x52, 0x65, 0x67, 0x20, 0x43, 0x6F, 0x6E,
                            0x66, 0x69, 0x67, 0x20, 0x46, 0x6F, 0x72, 0x6D
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "EA CdKey Registration Module"),
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
                    // A + (char)0x00 + b + (char)0x00 + o + (char)0x00 + u + (char)0x00 + t + (char)0x00 +   + (char)0x00 + C + (char)0x00 + D + (char)0x00 + K + (char)0x00 + e + (char)0x00 + y + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x41, 0x00, 0x62, 0x00, 0x6F, 0x00, 0x75, 0x00,
                            0x74, 0x00, 0x20, 0x00, 0x43, 0x00, 0x44, 0x00,
                            0x4B, 0x00, 0x65, 0x00, 0x79, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "EA CdKey Registration Module"),

                     // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + r + (char)0x00 + n + (char)0x00 + a + (char)0x00 + l + (char)0x00 + N + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 +  + (char)0x00 +  + (char)0x00 + C + (char)0x00 + D + (char)0x00 + C + (char)0x00 + o + (char)0x00 + d + (char)0x00 + e + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                            0x72, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6C, 0x00,
                            0x4E, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                            0x00, 0x00, 0x43, 0x00, 0x44, 0x00, 0x43, 0x00,
                            0x6F, 0x00, 0x64, 0x00, 0x65, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "EA CdKey Registration Module"),

                    // R + (char)0x00 + e + (char)0x00 + g + (char)0x00 + i + (char)0x00 + s + (char)0x00 + t + (char)0x00 + r + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + C/c + (char)0x00 + o + (char)0x00 + d + (char)0x00 + e + (char)0x00 +   + (char)0x00 + i + (char)0x00 + n + (char)0x00 + s + (char)0x00 + t + (char)0x00 + a + (char)0x00 + l + (char)0x00 + l + (char)0x00 + e + (char)0x00 + r + (char)0x00 +   + (char)0x00 + p + (char)0x00 + r + (char)0x00 + o + (char)0x00 + g + (char)0x00 + r + (char)0x00 + a + (char)0x00 + m + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x52, 0x00, 0x65, 0x00, 0x67, 0x00, 0x69, 0x00,
                            0x73, 0x00, 0x74, 0x00, 0x72, 0x00, 0x61, 0x00,
                            0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00,
                            0x20, 0x00, null, 0x00, 0x6F, 0x00, 0x64, 0x00,
                            0x65, 0x00, 0x20, 0x00, 0x69, 0x00, 0x6E, 0x00,
                            0x73, 0x00, 0x74, 0x00, 0x61, 0x00, 0x6C, 0x00,
                            0x6C, 0x00, 0x65, 0x00, 0x72, 0x00, 0x20, 0x00,
                            0x70, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x67, 0x00,
                            0x72, 0x00, 0x61, 0x00, 0x6D, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    Utilities.GetFileVersion, "EA CdKey Registration Module"),

                    // E + (char)0x00 + A + (char)0x00 +   + (char)0x00 + D + (char)0x00 + R + (char)0x00 + M + (char)0x00 +   + (char)0x00 + H + (char)0x00 + e + (char)0x00 + l + (char)0x00 + p + (char)0x00 + e + (char)0x00 + r + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x45, 0x00, 0x41, 0x00, 0x20, 0x00, 0x44, 0x00,
                            0x52, 0x00, 0x4D, 0x00, 0x20, 0x00, 0x48, 0x00,
                            0x65, 0x00, 0x6C, 0x00, 0x70, 0x00, 0x65, 0x00,
                            0x72, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    "EA DRM Protection"),
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
                    // GenericEA + (char)0x00 + (char)0x00 + (char)0x00 + Activation
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x47, 0x65, 0x6E, 0x65, 0x72, 0x69, 0x63, 0x45,
                            0x41, 0x00, 0x00, 0x00, 0x41, 0x63, 0x74, 0x69,
                            0x76, 0x61, 0x74, 0x69, 0x6F, 0x6E
                        }, start: sectionAddr, end: sectionEnd),
                    "EA DRM Protection"),
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
                    // GenericEA + (char)0x00 + (char)0x00 + (char)0x00 + Activation
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x47, 0x65, 0x6E, 0x65, 0x72, 0x69, 0x63, 0x45,
                            0x41, 0x00, 0x00, 0x00, 0x41, 0x63, 0x74, 0x69,
                            0x76, 0x61, 0x74, 0x69, 0x6F, 0x6E
                        }, start: sectionAddr, end: sectionEnd),
                    "EA DRM Protection"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
