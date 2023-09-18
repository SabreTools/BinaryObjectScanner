using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// MediaCloQ was a copy protection created by SunnComm to protect music CDs. It's a multisession CD, and all the audio tracks are erroneously marked as data tracks.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/MediaCloQ/MediaCloQ.md/>
    /// </summary>
    public class MediaCloQ : IPathCheck, IPortableExecutableCheck
    {
        // TODO: "Karaoke Spotlight Series - Pop Hits - Vol. 132" - Sound Choice (SC8732)" is currently undetected, due to there seeming to be no reference to MediaCloQ in the disc's contents.

        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in scvfy.exe on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
            var name = pex.FileDescription;
            if (name?.StartsWith("scvfy MFC Application", StringComparison.OrdinalIgnoreCase) == true)
                return $"MediaCloQ";

            // Found in scvfy.exe on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
            name = pex.ProductName;
            if (name?.StartsWith("scvfy Application", StringComparison.OrdinalIgnoreCase) == true)
                return $"MediaCloQ";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "sunncomm.ico" was a previously used file check, but since it's just an icon of the SunnComm logo, it seems too likely to result in false positives.

                // Found on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
                new PathMatchSet(new PathMatch("scvfy.exe", useEndsWith: true), "MediaCloQ"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "sunncomm.ico" was a previously used file check, but since it's just an icon of the SunnComm logo, it seems too likely to result in false positives.

                // Found on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
                new PathMatchSet(new PathMatch("scvfy.exe", useEndsWith: true), "MediaCloQ"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
