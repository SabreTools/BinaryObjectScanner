﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// WinLock (by B16MCC) is a program that allows the user to create a CD-R with intentional errors. 
    /// It does this by modifying an existing cuesheet to include 3 additional tracks that wiould be created from the file "WinLock.PSX".
    /// It then allows the user to automatically modify that file while the CD-R is being burnt, causing there to be errors at the end of the disc.
    /// Note that no additional files are written to the disc itself, the additional tracks created are invalid.
    /// Based on the included "WinLock.txt", this is intended specifically for PSX discs.
    /// The website appears to have relied heavily on Java, none of which appears to be archived online (https://web.archive.org/web/20010221030118/http://www.diamondcdroms.co.uk:80/).
    /// Download: https://cmw.mobiletarget.net/?f=winlock.zip.
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_news_0001.shtml
    /// Doesn't appear to be related to https://www.crystaloffice.com/winlock/.
    /// </summary>
    public class WinLock : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // This is the temporary dummy file created in the C: drive.
                new PathMatchSet(new PathMatch("WinLock.PSX", useEndsWith: true), "WinLock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // This is the temporary dummy file created in the C: drive.
                new PathMatchSet(new PathMatch("WinLock.PSX", useEndsWith: true), "WinLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
