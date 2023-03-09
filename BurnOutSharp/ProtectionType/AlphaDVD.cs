using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;

namespace BurnOutSharp.ProtectionType
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
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("PlayDVD.exe", useEndsWith: true), "Alpha-DVD (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("PlayDVD.exe", useEndsWith: true), "Alpha-DVD (Unconfirmed - Please report to us on Github"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
