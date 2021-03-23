using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class AACS : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // HD-DVD
                new PathMatchSet(Path.Combine("AACS", "MKB_RO.inf"), GetVersion, "AACS"),

                // HD-DVD
                new PathMatchSet(Path.Combine("AACS", "MKBROM.AACS"), GetVersion, "AACS"),
            };

            var matches = MatchUtil.GetFirstMatch(files, matchers, any: true);
            return string.Join(", ", matches);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // HD-DVD
                new PathMatchSet("MKB_RO.inf", GetVersion, "AACS"),

                // HD-DVD
                new PathMatchSet("MKBROM.AACS", GetVersion, "AACS"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetVersion(string path, IEnumerable<string> files)
        {
            if (!File.Exists(path))
                return "(Unknown Version)";

            try
            {
                using (var fs = File.OpenRead(path))
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
