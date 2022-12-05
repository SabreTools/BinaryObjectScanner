using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// PlayJ (https://web.archive.org/web/20000815053956/http://www.playj.com/) by EverAd was a form of DRM protected audio that was intended to be distributed freely, but that showed advertisements to the listener.
    /// This format didn't live for very long, having started in the February of 2000 (https://web.archive.org/web/20000301013405/http://www.playj.com/digital_music.htm) and ending with the parent company shutting down in 2001 (https://www.thestreet.com/technology/everad-closes-down-after-having-raised-345m-in-past-two-years-1505964).
    /// The file format itself appears to be an encrypted MP3 (https://www.wired.com/2000/03/ads-take-aim-at-online-music/).
    /// The IANA assignment for the PlayJ format: https://www.iana.org/assignments/media-types/audio/vnd.everad.plj
    /// The official player has been archived and can still be used to listen to the music (https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe).
    /// The music files themselves do not contain advertisements, which is instead handled on the side of the music players and plugins capable of playing the format.
    /// According to the ReadMe.txt found in https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe, there's 2 distinct versions of the format. The differences between versions are currently unknown.
    /// There are a few sources of PlayJ music archived online, these being a few examples:
    /// https://web.archive.org/web/*/http://dlp.playj.com/*/
    /// https://web.archive.org/web/*/http://brightnet.music.tucows.com/*/
    /// Also noted in the ReadMe.txt present in https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe as hosting PlayJ music:
    /// http://www.listen.com
    /// http://www.launch.com
    /// Further information and resources:
    /// https://www.entrepreneur.com/business-news/sales-marketing-playing-around/33284
    /// https://www.adweek.com/brand-marketing/j-records-and-everad-ink-pact-47882/
    /// https://adage.com/article/news/everad-s-playj-partners-clive-davis-j-records/11043
    /// https://www.nytimes.com/2000/07/07/technology/search-for-digital-music-revenue-takes-new-direction.html
    /// </summary>
    public class PlayJ : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Found in "PlayJ.exe" (https://web.archive.org/web/20010417025347/http://dlp.playj.com:80/playj/PlayJIns266.exe) and "CACTUSPJ.exe" ("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])).
            string name = pex.FileDescription;
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
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
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
        public string CheckFilePath(string path)
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
