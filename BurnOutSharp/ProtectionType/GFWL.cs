using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class GFWL : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // xlive.dll
                new Matcher(new byte?[] { 0x78, 0x6C, 0x69, 0x76, 0x65, 0x2E, 0x64, 0x6C, 0x6C }, "Games for Windows - Live"),
            };

            return Utilities.GetContentMatches(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("XLiveRedist.msi", StringComparison.OrdinalIgnoreCase)))
                return "Games for Windows - Live";
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("XLiveRedist.msi", StringComparison.OrdinalIgnoreCase))
                return "Games for Windows - Live";

            return null;
        }
    }
}
