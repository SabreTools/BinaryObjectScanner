using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Freelock is software intended to allow users to burn a copy protected PSX CD-R. 
    /// It adds multiple garbage tracks to the end of the disc and creates intentional errors on the disc by attempting to burn a corrupted file.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/Freelock/Freelock.md"/>
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
                new PathMatchSet(new PathMatch("GENLOCK.EXE", useEndsWith: true), "Freelock 1.2+"),
                new PathMatchSet(new PathMatch("freelock.ico", useEndsWith: true), "Freelock 1.2+"),
                new PathMatchSet(new PathMatch("freelock.pif", useEndsWith: true), "Freelock 1.2+"),

                // Created by "GENLOCK.EXE" in Freelock 1.2+.
                new PathMatchSet(new PathMatch("FREELOCK.DAT", useEndsWith: true), "Freelock 1.2+"),

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
                new PathMatchSet(new PathMatch("GENLOCK.EXE", useEndsWith: true), "Freelock 1.2+"),
                new PathMatchSet(new PathMatch("freelock.ico", useEndsWith: true), "Freelock 1.2+"),
                new PathMatchSet(new PathMatch("freelock.pif", useEndsWith: true), "Freelock 1.2+"),

                // Created by "GENLOCK.EXE" in Freelock 1.2+.
                new PathMatchSet(new PathMatch("FREELOCK.DAT", useEndsWith: true), "Freelock 1.2+"),

                // Found in "FREELOCK.IMG" From Freelock 1.3.
                new PathMatchSet(new PathMatch("FREELOCK.13", useEndsWith: true), "Freelock 1.3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
