using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Matching;

namespace BinaryObjectScanner.Protection
{
    // http://www.vsubhash.in/ubuntu-gnome-diary.html#play_copy_protected_vcds_indygenius_indyvcd
    // https://forum.videohelp.com/threads/241664-How-to-copy-this-vcd
    // https://forum.videohelp.com/threads/315331-VCD-cannot-play-in-PC-but-can-play-in-DVD-player
    // https://answers.microsoft.com/en-us/windows/forum/all/call-to-dii-register-server-failed/16cb344e-979a-4fb6-a8cd-1c9bc4dd14dd
    public class IndyVCD : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("INDYVCD.AX"), "IndyVCD (Unconfirmed - Please report to us on Github)"),
                new(new FilePathMatch("INDYMP3.idt"), "IndyVCD (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("INDYVCD.AX"), "IndyVCD (Unconfirmed - Please report to us on Github)"),
                new(new FilePathMatch("INDYMP3.idt"), "IndyVCD (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
