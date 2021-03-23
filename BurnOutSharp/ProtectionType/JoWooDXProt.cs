using System;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class JoWooDXProt : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // @HC09    
            new ContentMatchSet(new byte?[] { 0x40, 0x48, 0x43, 0x30, 0x39, 0x20, 0x20, 0x20, 0x20 }, "JoWooD X-Prot v2"),

            new ContentMatchSet(new List<byte?[]>
            {
                // .ext    
                new byte?[] { 0x2E, 0x65, 0x78, 0x74, 0x20, 0x20, 0x20, 0x20 },

                // kernel32.dll + (char)0x00 + (char)0x00 + (char)0x00 + VirtualProtect
                new byte?[]
                {
                    0x6B, 0x65, 0x72, 0x6E, 0x65, 0x6C, 0x33, 0x32,
                    0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x00, 0x00, 0x56,
                    0x69, 0x72, 0x74, 0x75, 0x61, 0x6C, 0x50, 0x72,
                    0x6F, 0x74, 0x65, 0x63, 0x74
                },
            }, GetVersion, "JoWooD X-Prot"),

            // .ext      
            new ContentMatchSet(new byte?[] { 0x2E, 0x65, 0x78, 0x74, 0x20, 0x20, 0x20, 0x20 }, "JoWooD X-Prot v1"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            int position = positions[1]--; // TODO: Verify this subtract
            char[] version = new ArraySegment<byte>(fileContent, position + 67, 8).Select(b => (char)b).ToArray();
            return $"{version[0]}.{version[2]}.{version[4]}.{version[6]}{version[7]}";
        }
    }
}
