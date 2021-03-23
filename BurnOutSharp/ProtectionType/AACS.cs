using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class AACS : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // BD-ROM
                new PathMatchSet(Path.Combine("AACS", "MKB_RO.inf"), GetVersion, "AACS"),

                // HD-DVD
                new PathMatchSet(Path.Combine("AACS", "MKBROM.AACS"), GetVersion, "AACS"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // BD-ROM
                new PathMatchSet(new PathMatch("MKB_RO.inf", useEndsWith: true), GetVersion, "AACS"),

                // HD-DVD
                new PathMatchSet(new PathMatch("MKBROM.AACS", useEndsWith: true), GetVersion, "AACS"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (!File.Exists(firstMatchedString))
                return "(Unknown Version)";

            try
            {
                using (var fs = File.OpenRead(firstMatchedString))
                {
                    fs.Seek(0xB, SeekOrigin.Begin);
                    return fs.ReadByte().ToString();
                }
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}
