using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDLock : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // 2 + (char)0xF2 + (char)0x02 + (char)0x82 + (char)0xC3 + (char)0xBC + (char)0x0B + $ + (char)0x99 + (char)0xAD + 'C + (char)0xE4 + (char)0x9D + st + (char)0x99 + (char)0xFA + 2$ + (char)0x9D + )4 + (char)0xFF + t
                new Matcher(new byte?[]
                {
                    0x32, 0xF2, 0x02, 0x82, 0xC3, 0xBC, 0x0B, 0x24,
                    0x99, 0xAD, 0x27, 0x43, 0xE4, 0x9D, 0x73, 0x74,
                    0x99, 0xFA, 0x32, 0x24, 0x9D, 0x29, 0x34, 0xFF,
                    0x74
                }, "CD-Lock"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetExtension(f).Trim('.').Equals("AFP", StringComparison.OrdinalIgnoreCase)))
                return "CD-Lock";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetExtension(path).Trim('.').Equals("AFP", StringComparison.OrdinalIgnoreCase))
                return "CD-Lock";
            
            return null;
        }
    }
}
