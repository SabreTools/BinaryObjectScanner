using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

namespace BinaryObjectScanner.Protection
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
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // The disk image that every version of Freelock is distributed through.
                new(new FilePathMatch("FREELOCK.IMG"), "Freelock Disk Image"),

                // Found in every "FREELOCK.IMG".
                new(new FilePathMatch("FREELOCK.EXE"), "Freelock"),
                new(new FilePathMatch("FREELOCK.TXT"), "Freelock"),

                // Found in "FREELOCK.IMG" from Freelock 1.0-1.2.
                new(new FilePathMatch("FREELOCK"), "Freelock 1.0-1.2"),

                // Found in "FREELOCK.IMG" from Freelock 1.2+.
                new(new FilePathMatch("GENLOCK.EXE"), "Freelock 1.2+"),
                new(new FilePathMatch("freelock.ico"), "Freelock 1.2+"),
                new(new FilePathMatch("freelock.pif"), "Freelock 1.2+"),

                // Created by "GENLOCK.EXE" in Freelock 1.2+.
                new(new FilePathMatch("FREELOCK.DAT"), "Freelock 1.2+"),

                // Found in "FREELOCK.IMG" From Freelock 1.3.
                new(new FilePathMatch("FREELOCK.13"), "Freelock 1.3"),

                // Found in "FREELOCK.IMG" From Freelock 1.3.
                new(
                [
                    new FilePathMatch("FREELOCK.13"),
                    new FilePathMatch("FL.DAT"),
                ], "Freelock 1.3"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // The disk image that every version of Freelock is distributed through.
                new(new FilePathMatch("FREELOCK.IMG"), "Freelock Disk Image"),

                // Found in every "FREELOCK.IMG".
                new(new FilePathMatch("FREELOCK.EXE"), "Freelock"),
                new(new FilePathMatch("FREELOCK.TXT"), "Freelock"),

                // Found in "FREELOCK.IMG" from Freelock 1.0-1.2.
                new(new FilePathMatch("FREELOCK"), "Freelock 1.0-1.2"),

                // Found in "FREELOCK.IMG" from Freelock 1.2+.
                new(new FilePathMatch("GENLOCK.EXE"), "Freelock 1.2+"),
                new(new FilePathMatch("freelock.ico"), "Freelock 1.2+"),
                new(new FilePathMatch("freelock.pif"), "Freelock 1.2+"),

                // Created by "GENLOCK.EXE" in Freelock 1.2+.
                new(new FilePathMatch("FREELOCK.DAT"), "Freelock 1.2+"),

                // Found in "FREELOCK.IMG" From Freelock 1.3.
                new(new FilePathMatch("FREELOCK.13"), "Freelock 1.3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
