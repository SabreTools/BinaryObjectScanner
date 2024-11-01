using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// LabelGate CD is a copy protection used by Sony in some Japanese CD releases. There are at least two distinct versions, characterized by the presence of either "MAGIQLIP" or "MAGIQLIP2".
    /// It made use of Sony OpenMG DRM, and allowed users to make limited copies.
    /// References:
    /// https://web.archive.org/web/20040604223749/http://www.sonymusic.co.jp/cccd/
    /// https://web.archive.org/web/20040407150004/http://www.sonymusic.co.jp/cccd/lgcd2/help/foreign.html
    /// https://vgmdb.net/forums/showthread.php?p=92206
    /// </summary>
    public class LabelGate : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Should be present on all LabelGate CD2 discs (Redump entry 95010 and product ID SVWC-7185).
            var name = pex.FileDescription;
            if (name?.StartsWith("MAGIQLIP2 Installer", StringComparison.OrdinalIgnoreCase) == true)
                return $"LabelGate CD2 Media Player";

            name = pex.ProductName;
            if (name?.StartsWith("MQSTART", StringComparison.OrdinalIgnoreCase) == true)
                return $"LabelGate CD2 Media Player";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "START.EXE" (Redump entry 95010 and product ID SVWC-7185).
                if (strs.Any(s => s.Contains("LGCD2_LAUNCH")))
                    return "LabelGate CD2";
            }

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // All found to be present on at multiple albums with LabelGate CD2 (Redump entry 95010 and product ID SVWC-7185), the original version of LabelGate still needs to be investigated.
                new(new List<PathMatch>
                {
#if NET20 || NET35
                    new(Path.Combine(Path.Combine("BIN", "WIN32"), "MQ2SETUP.EXE")),
                    new(Path.Combine(Path.Combine("BIN", "WIN32"), "MQSTART.EXE")),
#else
                    new(Path.Combine("BIN", "WIN32", "MQ2SETUP.EXE")),
                    new(Path.Combine("BIN", "WIN32", "MQSTART.EXE")),
#endif
                }, "LabelGate CD2 Media Player"),

                // All of these are also found present on all known LabelGate CD2 releases, though an additional file "RESERVED.DAT" is found in the same directory in at least one release (Product ID SVWC-7185)
                new(new List<PathMatch>
                {
                    new(Path.Combine("MQDISC", "LICENSE.TXT")),
                    new(Path.Combine("MQDISC", "MQDISC.INI")),
                    new(Path.Combine("MQDISC", "START.INI")),
                }, "LabelGate CD2"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // This is the installer for the media player used by LabelGate CD2 (Redump entry 95010 and product ID SVWC-7185).
                new(new FilePathMatch("MQ2SETUP.EXE"), "LabelGate CD2 Media Player"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
