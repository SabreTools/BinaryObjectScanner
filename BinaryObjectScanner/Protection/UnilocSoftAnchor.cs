using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Uniloc SoftAnchor is an activator-based protection
    /// </summary>
    /// <remarks>
    /// Apparently present on the original release of Alpha Protocol
    /// (https://web.archive.org/web/20120613033042/http://blogs.sega.com/2010/05/01/alpha-protocol-pc-drm-details/),
    /// but was seemingly removed accord to PCGW.
    /// </remarks>
    /// <see href="http://redump.org/discs/quicksearch/uniloc/protection/only"/>
    public class UnilocSoftAnchor : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Add version number finding
            // TODO: Come to an agreement as to what the version should be

            // Found in Redump entry 114428
            var name = exe.CompanyName;
            if (name.OptionalStartsWith("Uniloc USA Inc."))
                return "Uniloc SoftAnchor";

            // Found in Redump entry 114428
            name = exe.LegalCopyright;
            if (name.OptionalContains("Uniloc"))
                return "Uniloc SoftAnchor";

            // Found via https://www.pcgamingwiki.com/wiki/Football_Manager_2010
            name = exe.OriginalFilename;
            if (name.OptionalEquals("saAudit.dll"))
                return "Uniloc SoftAnchor";
            if (name.OptionalEquals("saui.dll"))
                return "Uniloc SoftAnchor";

            // Found via https://www.pcgamingwiki.com/wiki/Football_Manager_2010
            name = exe.ProductName;
            if (name.OptionalStartsWith("saAudit"))
                return "Uniloc SoftAnchor";

            // Found in Redump entry 114428
            if (Array.IndexOf(exe.SectionNames, "SAAC0") > -1)
                return "Uniloc SoftAnchor";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found via https://www.pcgamingwiki.com/wiki/Football_Manager_2010
                new(new FilePathMatch("saAudit.dll"), "Uniloc SoftAnchor"),
                new(new FilePathMatch("saAudit2005MT.dll"), "Uniloc SoftAnchor"),
                new(new FilePathMatch("saui.dll"), "Uniloc SoftAnchor"),

                // Found in Redump entry 114428
                new(new FilePathMatch("saAuditMD.dll"), "Uniloc SoftAnchor"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found via https://www.pcgamingwiki.com/wiki/Football_Manager_2010
                new(new FilePathMatch("saAudit.dll"), "Uniloc SoftAnchor"),
                new(new FilePathMatch("saAudit2005MT.dll"), "Uniloc SoftAnchor"),
                new(new FilePathMatch("saui.dll"), "Uniloc SoftAnchor"),

                // Found in Redump entry 114428
                new(new FilePathMatch("saAuditMD.dll"), "Uniloc SoftAnchor"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}