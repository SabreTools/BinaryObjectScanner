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
        private List<ContentMatchSet> GetContentMatchSets()
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
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetFileDescription(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("Registration code installer program"))
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.Equals("EA DRM Helper", StringComparison.OrdinalIgnoreCase))
                return $"EA DRM Protection {Utilities.GetFileVersion(pex)}";

            name = Utilities.GetInternalName(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("CDCode", StringComparison.Ordinal))
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(pex)}";

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

            // TODO: Find this inside of the .rsrc section using the executable header
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

            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }
    }
}
