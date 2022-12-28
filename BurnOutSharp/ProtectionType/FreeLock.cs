using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Freelock is software intended to allow users to burn a copy protected PSX CD-R.
    /// It accomplishes this by adding additional dummy tracks to an image before burning, and then attempting to use a corrupted file to burn to these tracks, causing the write process to fail at the end.
    /// Freelock versions 1.0-1.2a were intended solely to be copied to a floppy disk and then used, as the main executable itself was intentionally corrupted in these versions.
    /// Freelock version 1.3 intentionally corrupts a different file, allowing you to run the program without a floppy disk.
    /// 
    /// Official websites:
    /// https://web.archive.org/web/20010801181527/http://www.geocities.com/freelock_psx/
    /// https://web.archive.org/web/19991001075001/http://www.geocities.com/SiliconValley/Code/6061/
    /// 
    /// Versions:
    /// Freelock 1.0: https://web.archive.org/web/20040615215309/http://www.geocities.com/SiliconValley/Code/6061/programs/flock10.zip
    /// Freelock 1.2: https://web.archive.org/web/20091027114741/http://geocities.com/siliconvalley/code/6061/programs/flock12.zip
    /// Freelock 1.2a: https://web.archive.org/web/20040613085437/http://www.geocities.com/SiliconValley/Code/6061/programs/Flock12a.zip
    /// Freelock 1.3: https://web.archive.org/web/20040606222542/http://www.geocities.com/SiliconValley/Code/6061/programs/flock13.zip
    /// </summary>
    public class Freelock : IPathCheck
    {
        // TODO: Add an MS-DOS executable check for "FREELOCK.EXE".

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // The disk image that every version of Freelock is distributed through.
                new PathMatchSet(new PathMatch("FREELOCK.IMG", useEndsWith: true), "Freelock Disk Image"),

                // Found in every "FREELOCK.IMG".
                new PathMatchSet(new PathMatch("FREELOCK.EXE", useEndsWith: true), "Freelock"),
                new PathMatchSet(new PathMatch("FREELOCK.TXT", useEndsWith: true), "Freelock"),

                // Found in "FREELOCK.IMG" from Freelock 1.0-1.2.
                new PathMatchSet(new PathMatch("FREELOCK", useEndsWith: true), "Freelock 1.0-1.2"),

                // Found in "FREELOCK.IMG" from Freelock 1.2+.
                new PathMatchSet(new PathMatch("GENLOCK.EXE", useEndsWith: true), "Freelock 1.2-1.3"),
                new PathMatchSet(new PathMatch("freelock.ico", useEndsWith: true), "Freelock 1.2-1.3"),
                new PathMatchSet(new PathMatch("freelock.pif", useEndsWith: true), "Freelock 1.2-1.3"),

                // Found in "FREELOCK.IMG" From Freelock 1.3.
                new PathMatchSet(new PathMatch("FREELOCK.13", useEndsWith: true), "Freelock 1.3"),

                // Found in "FREELOCK.IMG" From Freelock 1.3.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("FREELOCK.13", useEndsWith: true),
                    new PathMatch("FL.DAT", useEndsWith: true),
                }, "Freelock 1.3"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // The disk image that every version of Freelock is distributed through.
                new PathMatchSet(new PathMatch("FREELOCK.IMG", useEndsWith: true), "Freelock Disk Image"),

                // Found in every "FREELOCK.IMG".
                new PathMatchSet(new PathMatch("FREELOCK.EXE", useEndsWith: true), "Freelock"),
                new PathMatchSet(new PathMatch("FREELOCK.TXT", useEndsWith: true), "Freelock"),

                // Found in "FREELOCK.IMG" from Freelock 1.0-1.2.
                new PathMatchSet(new PathMatch("FREELOCK", useEndsWith: true), "Freelock 1.0-1.2"),

                // Found in "FREELOCK.IMG" from Freelock 1.2+.
                new PathMatchSet(new PathMatch("GENLOCK.EXE", useEndsWith: true), "Freelock 1.2-1.3"),
                new PathMatchSet(new PathMatch("freelock.ico", useEndsWith: true), "Freelock 1.2-1.3"),
                new PathMatchSet(new PathMatch("freelock.pif", useEndsWith: true), "Freelock 1.2-1.3"),

                // Found in "FREELOCK.IMG" From Freelock 1.3.
                new PathMatchSet(new PathMatch("FREELOCK.13", useEndsWith: true), "Freelock 1.3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
