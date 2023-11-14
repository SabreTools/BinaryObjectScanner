using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// PlayJ (https://web.archive.org/web/20000815053956/http://www.playj.com/) by EverAd was a form of DRM protected audio that was intended to be distributed freely, but that showed advertisements to the listener.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/PlayJ/PlayJ.md"/>
    /// </summary>
    public class PlayJ : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in "PlayJ.exe" (https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe) and "CACTUSPJ.exe" ("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])).
            var name = pex.FileDescription;
            if (name?.StartsWith("PlayJ Music Player", StringComparison.OrdinalIgnoreCase) == true)
                return $"PlayJ Music Player";

            // Found in "PJSTREAM.DLL" ("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])).
            name = pex.FileDescription;
            if (name?.StartsWith("EVAUX32 Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"PlayJ Music Player Component";

            // Found in "PlayJ.exe" (https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe) and "CACTUSPJ.exe" ("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])).
            name = pex.ProductName;
            if (name?.StartsWith("PlayJ", StringComparison.OrdinalIgnoreCase) == true)
                return $"PlayJ";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe.
                // The files "Data8.bml" and "Data16.bml" are also present in the installation directory, but it is currently unknown if they are specific to this DRM or not.
                new PathMatchSet(new PathMatch("PlayJ.exe", useEndsWith: true), "PlayJ Music Player"),
                new PathMatchSet(new PathMatch("PLJFilter.ax", useEndsWith: true), "PlayJ Windows Media Player Plug-in"),

                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
                new PathMatchSet(new PathMatch("PJSTREAM.DLL", useEndsWith: true), "PlayJ Music Player Component"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe.
                // The files "Data8.bml" and "Data16.bml" are also present in the installation directory, but it is currently unknown if they are specific to this DRM or not.
                new PathMatchSet(new PathMatch("PlayJ.exe", useEndsWith: true), "PlayJ Music Player"),
                new PathMatchSet(new PathMatch("PLJFilter.ax", useEndsWith: true), "PlayJ Windows Media Player Plug-in"),

                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
                new PathMatchSet(new PathMatch("PJSTREAM.DLL", useEndsWith: true), "PlayJ Music Player Component"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
