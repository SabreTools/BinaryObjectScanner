﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // Interesting note: the former protection "Xtreme-Protector" was found to be a
    // subset of the JoWood X-Prot checks, more specifically the XPROT section check
    // that now outputs a version of v1.4+.
    public class JoWood : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .ext     section, if it exists
            if (pex.ContainsSection(".ext    ", exact: true))
            {
                bool importTableMatches = (pex.Model.ImportTable?.ImportDirectoryTable?.Any(idte => idte?.Name == "kernel32.dll") ?? false)
                    && (pex.Model.ImportTable?.HintNameTable?.Any(s => s?.Name == "VirtualProtect") ?? false);

                // Get the .dcrtext section, if it exists
                if (pex.ContainsSection(".dcrtext") && importTableMatches)
                {
                    var dcrtextData = pex.GetFirstSectionData(".dcrtext");
                    if (dcrtextData != null)
                    {
                        var matchers = new List<ContentMatchSet>
                    {
                        // kernel32.dll + (char)0x00 + (char)0x00 + (char)0x00 + VirtualProtect
                        new(new byte?[]
                        {
                            0x6B, 0x65, 0x72, 0x6E, 0x65, 0x6C, 0x33, 0x32,
                            0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x00, 0x00, 0x56,
                            0x69, 0x72, 0x74, 0x75, 0x61, 0x6C, 0x50, 0x72,
                            0x6F, 0x74, 0x65, 0x63, 0x74
                        }, GetVersion, "JoWood X-Prot"),
                    };

                        var match = MatchUtil.GetFirstMatch(file, dcrtextData, matchers, includeDebug);
                        if (!string.IsNullOrEmpty(match))
                            return match;
                    }
                }

                return "JoWood X-Prot v1.0-v1.3";
            }

            // Get the HC09     section, if it exists
            bool hc09Section = pex.ContainsSection("HC09    ", exact: true);
            if (hc09Section)
                return "JoWood X-Prot v2"; // TODO: Can we get more granular with the version?

            // Get the XPROT    section, if it exists
            var xprotSection = pex.ContainsSection("XPROT   ", exact: true);
            if (xprotSection)
                return "JoWood X-Prot v1.4+"; // TODO: Can we get more granular with the version?

            return null;
        }

        public static string? GetVersion(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            int position = positions[0];
#if NET20 || NET35 || NET40
            byte[] versionBytes = new byte[8];
            Array.Copy(fileContent, position + 67, versionBytes, 0, 8);
            char[] version = versionBytes.Select(b => (char)b).ToArray();
#else
            char[] version = new ArraySegment<byte>(fileContent, position + 67, 8).Select(b => (char)b).ToArray();
#endif
            return new string(version);
        }
    }
}
