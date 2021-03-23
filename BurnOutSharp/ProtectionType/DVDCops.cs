using System;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCops : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // DVD-Cops,  ver. 
                new ContentMatchSet(new byte?[]
                {
                    0x44, 0x56, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73,
                    0x2C, 0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "DVD-Cops"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            char[] version = new ArraySegment<byte>(fileContent, positions[0] + 15, 4).Select(b => (char)b).ToArray();
            if (version[0] == 0x00)
                return string.Empty;

            return new string(version);
        }
    }
}
