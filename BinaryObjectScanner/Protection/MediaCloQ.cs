using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// MediaCloQ was a copy protection created by SunnComm to protect music CDs. It's a multisession CD, and all the audio tracks are erroneously marked as data tracks.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/MediaCloQ/MediaCloQ.md/>
    /// </summary>
    public class MediaCloQ : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        // TODO: "Karaoke Spotlight Series - Pop Hits - Vol. 132" - Sound Choice (SC8732)" is currently undetected, due to there seeming to be no reference to MediaCloQ in the disc's contents.

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            // Found in scvfy.exe on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
            if (name.OptionalStartsWith("scvfy MFC Application", StringComparison.OrdinalIgnoreCase))
                return "MediaCloQ";

            name = exe.ProductName;

            // Found in scvfy.exe on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
            if (name.OptionalStartsWith("scvfy Application", StringComparison.OrdinalIgnoreCase))
                return "MediaCloQ";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "sunncomm.ico" was a previously used file check, but since it's just an icon of the SunnComm logo, it seems too likely to result in false positives.

                // Found on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
                new(new FilePathMatch("scvfy.exe"), "MediaCloQ"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "sunncomm.ico" was a previously used file check, but since it's just an icon of the SunnComm logo, it seems too likely to result in false positives.

                // Found on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
                new(new FilePathMatch("scvfy.exe"), "MediaCloQ"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
