using System.Collections.Generic;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class UPX : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // UPX!
            new ContentMatchSet(new byte?[] { 0x55, 0x50, 0x58, 0x21 }, GetVersion, "UPX"),

            // NOS 
            new ContentMatchSet(new byte?[] { 0x4E, 0x4F, 0x53, 0x20 }, GetVersion, "UPX (NOS Variant)"),

            new ContentMatchSet(
                new List<byte?[]>
                {
                    // UPX0
                    new byte?[] { 0x55, 0x50, 0x58, 0x30 },

                    // UPX1
                    new byte?[] { 0x55, 0x50, 0x58, 0x31 },
                },
                "UPX (Unknown Version)"
            ),

            new ContentMatchSet(
                new List<byte?[]>
                {
                    // NOS0
                    new byte?[] { 0x4E, 0x4F, 0x53, 0x30 },

                    // NOS1
                    new byte?[] { 0x4E, 0x4F, 0x53, 0x31 },
                },
                "UPX (NOS Variant) (Unknown Version)"
            ),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            try
            {
                int index = positions[0];
                index -= 5;
                string versionString = Encoding.ASCII.GetString(fileContent, index, 4);
                if (!char.IsNumber(versionString[0]))
                    return "(Unknown Version)";

                return versionString;
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}