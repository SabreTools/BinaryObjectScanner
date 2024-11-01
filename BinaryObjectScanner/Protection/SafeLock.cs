using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

namespace BinaryObjectScanner.Protection
{
    // https://www.cdrinfo.pl/cdr/porady/safelock/safelock.php3
    // Notes for possible future content checks:
    //  - Protected files (that haven't been renamed) will contain a couple of strings at the end:
    //      + SafeLock.dat
    //      + Proszê w³o¿yæ p³ytê CD do CDROM-u
    //      + Proszê w³o¿yæ oryginaln¹ p³ytê CD do CDROM-u
    //  - Also at the end is an ASCII number possibly indicating version:
    //      + 0.99.15 uses "3144288522"
    //      + 0.99.22 uses "1614884465"
    //      + 1.0.4   uses "3574069264"
    //  - All of the above is at the very end of the file
    //  - All auxiliary files (like .001, .128, and .dat) seem to be encrypted or padding data
    public class SafeLock : IPathCheck
    {
        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            // Technically all need to exist but some might be renamed
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("SafeLock.DAT"), "SafeLock"),
                new(new FilePathMatch("SafeLock.001"), "SafeLock"),
                new(new FilePathMatch("SafeLock.002"), "SafeLock"),
                new(new FilePathMatch("SafeLock.128"), "SafeLock"),
                new(new FilePathMatch("SafeLock.256"), "SafeLock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("SafeLock.DAT"), "SafeLock"),
                new(new FilePathMatch("SafeLock.001"), "SafeLock"),
                new(new FilePathMatch("SafeLock.002"), "SafeLock"),
                new(new FilePathMatch("SafeLock.128"), "SafeLock"),
                new(new FilePathMatch("SafeLock.256"), "SafeLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
