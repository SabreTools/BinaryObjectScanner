using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// TZCopyProtection allows users to alter image files so they can be used to burn copy protected CD-Rs.
    /// It does this through multiple "steps", all of which can be applied independently of one another.
    /// Step 1 shifts the data of the first track into "track 0".
    /// Step 2, seemingly, moves the data of a selected file to a second data track, with a configurable amount of empty audio tracks inbetween data tracks.
    /// Step 3 pads the filesize of a specified file to a specified length, creating a dummy file.
    /// Details on how these steps work: https://web.archive.org/web/20011107003900/http://jove.prohosting.com/~tzcopy/dev.html
    /// Worth noting is that all of these features aim exclusively to prevent the protected CD-R from being copied, there are no program/driver/etc. files added to the modified image whatsoever.
    /// Due to this, it's unlikely that these earlier versions are able to be detected whatsoever.
    /// 
    /// After V1.1 Beta 9, the original author released the source code, retired from the project, and allowed others to continue the project (https://web.archive.org/web/20030619042134/http://www.tzcopyprotection.tk/).
    /// Several others began to contribute, with features such a new GUI, new protection steps, new image formats supported, etc. being added.
    /// Step 4 uses a "ghost" file that is added to the CUE and intended to be present during the start of burning a CD-R, but then modified during the burn so that there are intentional errors at the end of the disc.
    /// Step 5 adds "weak, damaged and hidden sectors" (https://web.archive.org/web/20030205103207/http://snow.prohosting.com/~clone99/tzweb/faqs.html) to the image.
    /// In my experience, V1.5.4 was far less stable than V1.11.1.4, with the latter having fewer newer features in comparison.
    /// There's also a program made by this team called "TZ EXE Protector".
    /// https://web.archive.org/web/20060225153855/http://www.denet.plus.com/tz/Tzep2beta5.zip
    /// https://web.archive.org/web/20030205062029/http://snow.prohosting.com/~clone99/tzweb/files.html
    /// 
    /// Official websites:
    /// https://web.archive.org/web/20001109225600/http://members.nbci.com/tzcopy/
    /// https://web.archive.org/web/20030619042134/http://www.tzcopyprotection.tk/
    /// https://web.archive.org/web/20010223232517/http://jove.prohosting.com/~tzcopy/
    /// https://web.archive.org/web/20021010015204/http://tzcopyprotection.cjb.net:80/
    /// https://web.archive.org/web/20021004072048/http://snow.prohosting.com/~clone99/tzweb/
    /// https://web.archive.org/web/20081014161844/http://wave.prohosting.com/~tzcp/
    /// https://web.archive.org/web/20020816012631/http://www.pcgemu.i12.com:80/downloads/software/tzdownloads.html
    /// https://web.archive.org/web/20020203072859/http://www.robert-knight.net/
    /// https://web.archive.org/web/20021215153849/http://snow.prohosting.com/~clone99/cgi-bin/ikonboard/ikonboard.cgi
    /// https://web.archive.org/web/20021215180832/http://wave.prohosting.com/~tzcp/cgi-bin/ikonboard/ikonboard.cgi
    /// 
    /// Versions:
    /// V1.1 Beta II: https://cmw.mobiletarget.net/?f=tzcopyb2.zip
    /// V1.1 Beta 3: https://web.archive.org/web/20000820144023/http://members.xoom.com:80/tzcopy/upgb3.zip
    /// V1.1 Beta 7: https://web.archive.org/web/20010615230150/http://members.nbci.com:80/tzcopy/upgb7.zip
    /// V1.1 Beta 9: https://cmw.mobiletarget.net/?f=upgb9.zip
    /// V1.1 Beta 9 (Source): https://cmw.mobiletarget.net/?f=tzcp_source.zip
    /// V1.11.1.4: https://web.archive.org/web/20020816012631/http://www.pcgemu.i12.com/downloads/software/tz111compat.exe
    /// V1.5.1 (Internal Beta 2): https://web.archive.org/web/20020823204425/http://www.pcgemu.i12.com/downloads/software/tzib_151_beta2.exe
    /// V1.5.1: https://web.archive.org/web/20030111133225/http://www.pcgemu.i12.com:80/downloads/software/tzib_151.exe
    /// V1.5.3: https://web.archive.org/web/20021005113316/http://www.pcgemu.i12.com/downloads/tz153full.exe
    /// V1.5.4 (EXE only): https://web.archive.org/web/20021225155805/http://www.pcgemu.i12.com/downloads/TZCP_154.exe
    /// V1.5.4 (Full installation): https://web.archive.org/web/20021008221744/http://www.pcgemu.i12.com:80/downloads/tz154full.exe
    /// V1.5.5 (Source code - Lost): https://web.archive.org/web/20021215132351/http://snow.prohosting.com/~clone99/tzweb/index.html)
    /// </summary>
    public class TZCopyProtection : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Unconfirmed, not sure where this is supposed to be found.
                new PathMatchSet(new PathMatch("_742893.016", useEndsWith: true), "TZCopyProtection (Unconfirmed - Please report to us on Github)"),

                // "Ghost" file used by newer versions of TZCopyProtection.
                new PathMatchSet(new PathMatch("ZakMcCrack.Ghost", useEndsWith: true), "TZCopyProtection V1.11+"),

                // Newer versions of TZCopyProtection also create files with a TZC extension, though their purpose is currently unknown.
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
                // Unconfirmed, not sure where this is supposed to be found.
                new PathMatchSet(new PathMatch("_742893.016", useEndsWith: true), "TZCopyProtection (Unconfirmed - Please report to us on Github)"),

                // "Ghost" file used by newer versions of TZCopyProtection.
                new PathMatchSet(new PathMatch("ZakMcCrack.Ghost", useEndsWith: true), "TZCopyProtection V1.11+"),

                // Newer versions of TZCopyProtection also create files with a TZC extension, though their purpose is currently unknown.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
