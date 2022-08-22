using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
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
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Should be present on all LabelGate CD2 discs (Redump entry 95010 and product ID SVWC-7185).
            string name = pex.FileDescription;
            if (name?.StartsWith("MAGIQLIP2 Installer", StringComparison.OrdinalIgnoreCase) == true)
                return $"LabelGate CD2 Media Player";

            name = pex.ProductName;
            if (name?.StartsWith("MQSTART", StringComparison.OrdinalIgnoreCase) == true)
                return $"LabelGate CD2 Media Player";

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // LGCD2_LAUNCH
                    // Found in "START.EXE" (Redump entry 95010 and product ID SVWC-7185).
                    new ContentMatchSet(new byte?[] { 0x4C, 0x47, 0x43, 0x44, 0x32, 0x5F, 0x4C, 0x41, 0x55, 0x4E, 0x43, 0x48 }, "LabelGate CD2"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
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
                // All found to be present on at multiple albums with LabelGate CD2 (Redump entry 95010 and product ID SVWC-7185), the original version of LabelGate still needs to be investigated.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch(Path.Combine("BIN", "WIN32", "MQ2SETUP.EXE").Replace("\\", "/"), useEndsWith: true),
                    new PathMatch(Path.Combine("BIN", "WIN32", "MQSTART.EXE").Replace("\\", "/"), useEndsWith: true),
                }, "LabelGate CD2 Media Player"),

                // All of these are also found present on all known LabelGate CD2 releases, though an additional file "RESERVED.DAT" is found in the same directory in at least one release (Product ID SVWC-7185)
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch(Path.Combine("MQDISC", "LICENSE.TXT").Replace("\\", "/"), useEndsWith: true),
                    new PathMatch(Path.Combine("MQDISC", "MQDISC.INI").Replace("\\", "/"), useEndsWith: true),
                    new PathMatch(Path.Combine("MQDISC", "START.INI").Replace("\\", "/"), useEndsWith: true),
                }, "LabelGate CD2"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // This is the installer for the media player used by LabelGate CD2 (Redump entry 95010 and product ID SVWC-7185).
                new PathMatchSet(new PathMatch("MQ2SETUP.EXE", useEndsWith: true), "LabelGate CD2 Media Player"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
