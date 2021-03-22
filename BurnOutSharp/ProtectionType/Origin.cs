using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Origin : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // O + (char)0x00 + r + (char)0x00 + i + (char)0x00 + g + (char)0x00 + i + (char)0x00 + n + (char)0x00 + S + (char)0x00 + e + (char)0x00 + t + (char)0x00 + u + (char)0x00 + p + (char)0x00 + . + (char)0x00 + e + (char)0x00 + x + (char)0x00 + e + (char)0x00
                new Matcher(new byte?[] { 0x4F, 0x00, 0x72, 0x00, 0x69, 0x00, 0x67, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00, 0x70, 0x00, 0x2E, 0x00, 0x65, 0x00, 0x78, 0x00, 0x65, 0x00 }, "Origin"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("OriginSetup.exe", StringComparison.OrdinalIgnoreCase)))
                return "Origin";
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("OriginSetup.exe", StringComparison.OrdinalIgnoreCase))
                return "Origin";

            return null;
        }
    }
}
