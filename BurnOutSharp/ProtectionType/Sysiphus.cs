using System;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // V SUHPISYS
                    new ContentMatchSet(new byte?[]
                    {
                        0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53,
                        0x59, 0x53
                    }, GetVersion, "Sysiphus"),
                };

                string match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
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
