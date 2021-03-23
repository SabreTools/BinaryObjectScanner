using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class NSIS : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // Nullsoft Install System
            new ContentMatchSet(new byte?[]
            {
                0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74,
                0x20, 0x49, 0x6e, 0x73, 0x74, 0x61, 0x6c, 0x6c,
                0x20, 0x53, 0x79, 0x73, 0x74, 0x65, 0x6d
            }, GetVersion, "NSIS"),

            // NullsoftInst
            new ContentMatchSet(new byte?[]
            {
                0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74,
                0x49, 0x6e, 0x73, 0x74
            }, "NSIS"),
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