using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class BDPlus : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<string>
                {
                    Path.Combine("BDSVM", "00000.svm"),
                    Path.Combine("BDSVM", "BACKUP", "00000.svm"),
                }, GetVersion, "BD+"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("00000.svm", useEndsWith: true), GetVersion, "BD+"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        /// <remarks>
        /// Version detection logic from libbdplus was used to implement this
        /// </remarks>
        public static string GetVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (!File.Exists(firstMatchedString))
                return "(Unknown Version)";

            try
            {
                using (var fs = File.OpenRead(firstMatchedString))
                {
                    fs.Seek(0x0D, SeekOrigin.Begin);
                    byte[] date = new byte[4];
                    fs.Read(date, 0, 4); //yymd
                    short year = BitConverter.ToInt16(date.Reverse().ToArray(), 2);

                    // Do some rudimentary date checking
                    if (year < 2006 || year > 2100 || date[2] < 1 || date[2] > 12 || date[3] < 1 || date[3] > 31)
                        return "(Invalid Version)";

                    return $"{year:0000}/{date[2]:00}/{date[3]:00}";
                }
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}

