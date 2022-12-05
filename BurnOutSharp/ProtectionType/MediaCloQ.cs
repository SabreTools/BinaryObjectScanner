using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// MediaCloQ was a copy protection created by SunnComm to protect music CDs. It's a multisession CD, and all the audio tracks are erroneously marked as data tracks.
    /// Unlike other audio CD protections, this one doesn't give you an option to listen to the music off of PC or make "legitimate" copies.
    /// It does, however, have a data track containing a message that the CD is protected with MediaCloQ, and directs you to their website to purchase digital copies of the tracks.
    /// It appears that a version 2.0 was planned, and maybe even released, but so far no discs with MediaCloQ 2.0 are known (https://www.cdmediaworld.com/hardware/cdrom/cd_protections_mediacloq_v20.shtml, https://www.cdmediaworld.com/hardware/cdrom/news/0108/mediacloq_20.shtml).
    /// The only known album to contain MediaCloQ is "Charley Pride - A Tribute to Jim Reeves", with the release that has the barcode "7 816190222-2 4" being confirmed to have it.
    /// References and further information:
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_mediacloq_v10.shtml
    /// https://web.archive.org/web/20190513192224/https://club.myce.com/t/just-some-mediacloq-info/29773
    /// https://myce.wiki/news/MediaCloQ-CD-protection-bypassed-2397/
    /// https://www.cdrfaq.org/faq02.html#S2-4-4
    /// https://www.cnet.com/tech/services-and-software/bmg-tests-unrippable-cds/
    /// </summary>
    public class MediaCloQ : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Found in scvfy.exe on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
            string name = pex.FileDescription;
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
                // The file "sunncomm.ico" was previously used a file check, but since it's just an icon of the SunnComm logo, it seems too likely to result in false positives.

                // Found on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
                new PathMatchSet(new PathMatch("scvfy.exe", useEndsWith: true), "MediaCloQ"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // The file "sunncomm.ico" was previously used a file check, but since it's just an icon of the SunnComm logo, it seems too likely to result in false positives.

                // Found on "Charley Pride - A Tribute to Jim Reeves" (barcode "7 816190222-2 4").
                new PathMatchSet(new PathMatch("scvfy.exe", useEndsWith: true), "MediaCloQ"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
