using System;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction and better version detection
    public class PECompact : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // Another possible version string for version 1 is "PECO" (50 45 43 4F)
            var matchers = new List<ContentMatchSet>
            {
                // pec1
                new ContentMatchSet(new ContentMatch(new byte?[] { 0x70, 0x65, 0x63, 0x31 }, end: 2048), "PE Compact 1"),

                // PEC2
                new ContentMatchSet(new ContentMatch(new byte?[] { 0x50, 0x45, 0x43, 0x32 }, end: 2048), GetVersion, "PE Compact 2"),

                // PECompact2
                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x43, 0x6F, 0x6D, 0x70, 0x61, 0x63,
                    0x74, 0x32
                }, "PE Compact 2"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
        }

        // TODO: Improve version detection, Protection ID is able to detect ranges of versions. For example, 1.66-1.84 or 2.20-3.02.
        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            short version = BitConverter.ToInt16(fileContent, positions[0] + 4);
            if (version == 0)
                return "PE Compact 2";
            return $"Internal Version {version}";
        }
    }
}
