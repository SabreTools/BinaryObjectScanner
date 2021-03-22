using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class NSIS : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // Nullsoft Install System
                new Matcher(new byte?[]
                {
                    0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74,
                    0x20, 0x49, 0x6e, 0x73, 0x74, 0x61, 0x6c, 0x6c,
                    0x20, 0x53, 0x79, 0x73, 0x74, 0x65, 0x6d
                }, GetVersion, "NSIS"),

                // NullsoftInst
                new Matcher(new byte?[]
                {
                    0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74,
                    0x49, 0x6e, 0x73, 0x74
                }, "NSIS"),
            };

            return Utilities.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        public static string GetVersion(string file, byte[] fileContent, int index)
        {
            try
            {
                index += 24;
                if (fileContent[index] != 'v')
                    return "(Unknown Version)";

                var versionBytes = new ReadOnlySpan<byte>(fileContent, index, 16).ToArray();
                var onlyVersion = versionBytes.TakeWhile(b => b != '<').ToArray();
                return Encoding.ASCII.GetString(onlyVersion);
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}