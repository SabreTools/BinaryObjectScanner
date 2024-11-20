using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Alpha-DVD is a DVD-Video copy protection created by SETTEC.
    /// References and further information:
    /// http://www.gonsuke.co.jp/protect.html
    /// http://copy2.info/copy_protect.html
    /// http://s2000.yokinihakarae.com/sub03-10-2(DVD).html
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_alpha.shtml
    /// </summary>
    public class AlphaDVD : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("PlayDVD.exe"), "Alpha-DVD (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("PlayDVD.exe"), "Alpha-DVD (Unconfirmed - Please report to us on Github"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
