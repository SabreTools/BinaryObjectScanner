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
                new ContentMatchSet(new byte?[] { 0x70, 0x65, 0x63, 0x31 }, "PE Compact 1"),

                // PEC2
                new ContentMatchSet(new byte?[] { 0x50, 0x45, 0x43, 0x32 }, GetVersion, "PE Compact 2"),

                // PECompact2
                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x43, 0x6F, 0x6D, 0x70, 0x61, 0x63,
                    0x74, 0x32
                }, "PE Compact 2"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            return $"v{BitConverter.ToInt16(fileContent, positions[0] + 4)}";
        }
    }
}
