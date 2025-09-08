using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// yuPlay Launcher
    /// </summary>
    public class YuPlay : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("yuPlay.exe"), "yuPlay Launcher"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("yuPlay.exe"), "yuPlay Launcher"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
