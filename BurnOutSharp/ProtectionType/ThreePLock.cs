using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class ThreePLock : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                new Matcher(new List<byte?[]>
                {
                    // .ldr
                    new byte?[] { 0x2E, 0x6C, 0x64, 0x72 },

                    // .ldt
                    new byte?[] { 0x2E, 0x6C, 0x64, 0x74 },
                }, "3PLock"),

                // This produced false positives in some DirectX 9.0c installer files
                // "Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW"
                // new Matcher(new byte?[]
                // {
                //     0x59, 0xC3, 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x30,
                //     0x53, 0x56, 0x57
                // }, "3PLock"),
            };

            return Utilities.GetContentMatches(file, fileContent, matchers, includePosition);
        }
    }
}
