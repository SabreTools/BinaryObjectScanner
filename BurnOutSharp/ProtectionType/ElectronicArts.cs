using System;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class ElectronicArts : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.Contains("EReg MFC Application") == true)
                return $"EA CdKey Registration Module {Utilities.GetInternalVersion(pex)}";
            else if (name?.Contains("Registration code installer program") == true)
                return $"EA CdKey Registration Module {Utilities.GetInternalVersion(pex)}";
            else if (name?.Equals("EA DRM Helper", StringComparison.OrdinalIgnoreCase) == true)
                return $"EA DRM Protection {Utilities.GetInternalVersion(pex)}";

            name = pex.InternalName;
            if (name?.Equals("CDCode", StringComparison.Ordinal) == true)
                return $"EA CdKey Registration Module {Utilities.GetInternalVersion(pex)}";

            if (pex.FindDialogByTitle("About CDKey").Any())
                return $"EA CdKey Registration Module {Utilities.GetInternalVersion(pex)}";

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // EReg Config Form
                    new ContentMatchSet(new byte?[]
                    {
                        0x45, 0x52, 0x65, 0x67, 0x20, 0x43, 0x6F, 0x6E,
                        0x66, 0x69, 0x67, 0x20, 0x46, 0x6F, 0x72, 0x6D
                    }, Utilities.GetInternalVersion, "EA CdKey Registration Module"),
                };

                string match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rdata section, if it exists
            if (pex.ContainsSection(".rdata"))
            {
                var matchers = new List<ContentMatchSet>
                {
                    // GenericEA + (char)0x00 + (char)0x00 + (char)0x00 + Activation
                    new ContentMatchSet(new byte?[]
                    {
                        0x47, 0x65, 0x6E, 0x65, 0x72, 0x69, 0x63, 0x45,
                        0x41, 0x00, 0x00, 0x00, 0x41, 0x63, 0x74, 0x69,
                        0x76, 0x61, 0x74, 0x69, 0x6F, 0x6E
                    }, "EA DRM Protection"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.GetFirstSectionData(".rdata"), matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .text section, if it exists
            if (pex.ContainsSection(".text"))
            {
                var matchers = new List<ContentMatchSet>
                {
                    // GenericEA + (char)0x00 + (char)0x00 + (char)0x00 + Activation
                    new ContentMatchSet(new byte?[]
                    {
                        0x47, 0x65, 0x6E, 0x65, 0x72, 0x69, 0x63, 0x45,
                        0x41, 0x00, 0x00, 0x00, 0x41, 0x63, 0x74, 0x69,
                        0x76, 0x61, 0x74, 0x69, 0x6F, 0x6E
                    }, "EA DRM Protection"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.GetFirstSectionData(".text"), matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
