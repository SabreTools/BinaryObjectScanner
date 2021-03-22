using System.Collections.Generic;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class UPX : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // UPX!
                new Matcher(new byte?[] { 0x55, 0x50, 0x58, 0x21 }, GetVersion, "Inno Setup"),

                // NOS 
                new Matcher(new byte?[] { 0x4E, 0x4F, 0x53, 0x20 }, GetVersion, "UPX (NOS Variant)"),

                new Matcher(
                    new List<byte?[]>
                    {
                        // UPX0
                        new byte?[] { 0x55, 0x50, 0x58, 0x30 },

                        // UPX1
                        new byte?[] { 0x55, 0x50, 0x58, 0x31 },
                    },
                    "UPX (Unknown Version)"
                ),

                new Matcher(
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

            return Utilities.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        public static string GetVersion(string file, byte[] fileContent, int index)
        {
            try
            {
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