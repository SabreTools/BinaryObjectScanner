using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// PlayJ (https://web.archive.org/web/20000815053956/http://www.playj.com/) by EverAd was a form of DRM protected audio that was intended to be distributed freely, but that showed advertisements to the listener.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/PlayJ/PlayJ.md"/>
    /// </summary>
    public class PlayJ : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Found in "PlayJ.exe" (https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe) and "CACTUSPJ.exe" ("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])).
            var name = exe.FileDescription;
            if (name.OptionalStartsWith("PlayJ Music Player", StringComparison.OrdinalIgnoreCase))
                return $"PlayJ Music Player";

            // Found in "PJSTREAM.DLL" ("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])).
            name = exe.FileDescription;
            if (name.OptionalStartsWith("EVAUX32 Module", StringComparison.OrdinalIgnoreCase))
                return $"PlayJ Music Player Component";

            // Found in "PlayJ.exe" (https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe) and "CACTUSPJ.exe" ("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])).
            name = exe.ProductName;
            if (name.OptionalStartsWith("PlayJ", StringComparison.OrdinalIgnoreCase))
                return $"PlayJ";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe.
                // The files "Data8.bml" and "Data16.bml" are also present in the installation directory, but it is currently unknown if they are specific to this DRM or not.
                new(new FilePathMatch("PlayJ.exe"), "PlayJ Music Player"),
                new(new FilePathMatch("PLJFilter.ax"), "PlayJ Windows Media Player Plug-in"),

                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
                new(new FilePathMatch("PJSTREAM.DLL"), "PlayJ Music Player Component"),
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
                new(new FilePathMatch("PlayJ.exe"), "PlayJ Music Player"),
                new(new FilePathMatch("PLJFilter.ax"), "PlayJ Windows Media Player Plug-in"),

                // Found in "Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427]).
                new(new FilePathMatch("PJSTREAM.DLL"), "PlayJ Music Player Component"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
