using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// CopyKiller was a program made by Webstylerzone that allowed you to copyprotect your own discs.
    /// It appears to have used 3 different main forms of copy protection:
    /// 
    /// First, its core copy protection is applied by adding a folder from the program's installation directory to the disc as you burn it.
    /// The files in this folder appear to only be text files identifying the software used, and seemingly random file contents.
    /// How this protects the disc is currently not confirmed, and the data itself isn't corrupted or copied incorrectly on purpose.
    /// A personal guess is that it intended to use the same effect as SafeDisc's "weak sectors" to rely on the drive writing the disc incorrectly and making an "uncopyable" disc.
    /// This is backed up by an official description of how CopyKillers works, saying how it "uses a firmware error to make the cd copy protected." (https://web.archive.org/web/20061109151642/http://www.webtoolmaster.com/copykiller.htm)
    /// 
    /// Second, an optional autorun feature can be used by adding the appropriate contents of the "Autorun" folder from the program's installation directory to the disc as you burn it.
    /// This relies on Window running the autorun automatically, causing a window to warning to popup that tells the user that this is a pirated copy, with seemingly nothing else happening.
    /// I believe that it simply checks for the presence of the other protection files due to the complete lack of any ability to customize the DRM.
    /// 
    /// Last, there is a locked option to learn how to use it to protect audio CDs, but unfortunately this is only available with a registered version.
    /// This means that the mechanics of how this was done are currently unknown, but may have simply been to write the same folder's data in, whether as raw audio data or a separate data track.
    /// 
    /// At some point at least as early as 2006 (https://web.archive.org/web/20061109151642/http://www.webtoolmaster.com/copykiller.htm), WTM (WebToolMaster) and Webstylerzone had some sort of partnership.
    /// For example, WTM began hosting a link to CopyKiller beginning in 2006, and Webstylerzoning advertising WTM's products (https://web.archive.org/web/20070811202419/http://www.webstylerzone.com/en/download_brenner_copykiller_safedisc_safediscscanner_whatspeed_copyprotection_copy_protection_protect_cd_cds_audiocd_datacd_against_copying.htm).
    /// As of October of 2011, WTM announced that CopyKiller was officially no longer being developed (https://web.archive.org/web/20111014233821/http://webtoolmaster.com/copykiller.htm). 
    /// 
    /// CopyKiller website: https://web.archive.org/web/20030312200712/http://www.webstylerzone.com/CopyKiller/index.htm
    /// Version 3.62 Installer: https://web.archive.org/web/20031130192048/http://www.webstylerzone.com/Downloads/Brennertools/CopyKiller-Setup.exe
    /// Version 3.64 Installer: https://web.archive.org/web/20060524220845/http://download.webstylerzone.com:80/exe/CopyKiller-Setup.exe
    /// Version 3.99 Installer: https://web.archive.org/web/20060524220845/http://download.webstylerzone.com:80/exe/CopyKiller-Setup.exe
    /// Version 3.99a Installer: https://web.archive.org/web/20070721070138/http://www.webstylerzone.com/Downloads/exe/CopyKiller-Setup.exe
    /// Version 3.99a Portable: https://web.archive.org/web/20070721070214/http://www.webstylerzone.com/Downloads/zip/CopyKiller.zip
    /// 
    /// TODO: Add support for the developer's EXE obfuscator, "EXEShield Deluxe". Most, if not all, EXEShield protected files are currently detected as "EXE Stealth" by BOS.
    /// Samples include CopyKiller (Versions 3.64 & 3.99a) and SafeDiscScanner (Version 0.16) (https://archive.org/details/safediscscanner-0.16-webstylerzone-from-unofficial-source).
    /// </summary>
    public class CopyKiller : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Figure out how to differentiate between V3.99 and V3.99a.
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Figure out why this check doesn't work.
            // Found in "autorun.exe" in CopyKiller V3.64, V3.99, and V3.99a.
            var name = pex.ProductName;
            if (name?.StartsWith("CopyKiller", StringComparison.OrdinalIgnoreCase) == true)
                return "CopyKiller V3.64+";

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            // Previous versions of BOS noted to look at ".PFF" files as possible indicators of CopyKiller, but those files seem unrelated.
            // TODO: Figure out why this doesn't work.
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CopyKillerV3"), "CopyKiller V3.62-3.64"),
                new(new FilePathMatch("CopyKillerV4"), "CopyKiller V3.99-3.99a"),

                new(new List<PathMatch>
                {
                    new FilePathMatch("ACK3900.ckt"),
                    new FilePathMatch("ACK3999.ckt"),
                    new FilePathMatch("CK100.wzc"),
                    new FilePathMatch("CK2500.ck"),
                    new FilePathMatch("CK3600.tcwz"),
                    new FilePathMatch("Engine.wzc"),
                    new FilePathMatch("P261XP.tcck"),
                    new FilePathMatch("WZ200.rwzc"),
                    new FilePathMatch("XCK3900.ck2"),
                }, "CopyKiller V3.99+"),

                new(new List<PathMatch>
                {
                    new FilePathMatch("ACK3900.ckt"),
                    new FilePathMatch("CK100.wzc"),
                    new FilePathMatch("CK2500.ck"),
                    new FilePathMatch("CK3600.tcwz"),
                    new FilePathMatch("Engine.wzc"),
                    new FilePathMatch("P261XP.tcck"),
                    new FilePathMatch("WZ200.rwzc"),
                    new FilePathMatch("XCK3900.ck2"),
                }, "CopyKiller V3.64+"),

                new(new List<PathMatch>
                {
                    new FilePathMatch("CK100.wzc"),
                    new FilePathMatch("Engine.wzc"),
                }, "CopyKiller V3.62+"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // Previous versions of BOS noted to look at ".PFF" files as possible indicators of CopyKiller, but those files seem unrelated.
            // TODO: Figure out why this doesn't work.
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CopyKillerV3"), "CopyKiller V3.62-3.64"),
                new(new FilePathMatch("CopyKillerV4"), "CopyKiller V3.99-3.99a"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
