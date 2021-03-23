using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class MediaMaxCD3 : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // Cd3Ctl
                new ContentMatchSet(new byte?[] { 0x43, 0x64, 0x33, 0x43, 0x74, 0x6C }, "MediaMax CD-3"),

                // DllInstallSbcp
                new ContentMatchSet(new byte?[]
                {
                    0x44, 0x6C, 0x6C, 0x49, 0x6E, 0x73, 0x74, 0x61,
                    0x6C, 0x6C, 0x53, 0x62, 0x63, 0x70
                }, "MediaMax CD-3"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("LaunchCd.exe", StringComparison.OrdinalIgnoreCase)))
                return "MediaMax CD-3";
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("LaunchCd.exe", StringComparison.OrdinalIgnoreCase))
                return "MediaMax CD-3";

            return null;
        }
    }
}
