#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Channelware was an online activation DRM.
    /// 
    /// Official websites:
    /// 
    /// https://web.archive.org/web/19980212121046/http://www.channelware.com/index.html
    /// https://web.archive.org/web/20021002225705/http://cwsw.com/Home/default.asp
    /// https://web.archive.org/web/20040101180929/http://www.netactive.com/Home/
    /// 
    /// TODO:
    /// Add version detection. Redump entry 116358 is version 1.x and Redump entry 12354 is 2.x, but the file versions are inconsistent. 
    /// Investigate "NetActive Reach", which is is either a newer version of this DRM, or a new DRM created by the same company. (https://web.archive.org/web/20040101162921/http://www.netactive.com/Products/)
    /// </summary>
    public class Channelware : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in "AbeWincw.dll" in Redump entry 116358 and in "TOYSGMcw.dll" in the "TOYSTORY" installation folder from Redump entry 12354.
            var name = pex.ProductName;
            if (name?.Equals("ChannelWare Utilities") == true)
                return "Channelware";

            // Found in "cwbrowse.exe" in the "Channelware" folder installed from Redump entry 12354.
            if (name?.Equals("Channelware Browser Launcher") == true)
                return "Channelware";

            // Found in "cwuninst.exe" in the "Channelware" folder installed from Redump entry 12354.
            if (name?.Equals("Channelware Launcher Uninstall Application") == true)
                return "Channelware";

            // Found in "cwbrowse.exe" in the "Channelware\CWBrowse" folder installed from Redump entry 116358.
            if (name?.Equals("Channelware Authorization Server Browser Launcher") == true)
                return "Channelware";

            name = pex.FileDescription;
            // Found in "cwuninst.exe" in the "Channelware" folder installed from Redump entry 12354.
            if (name?.Equals("Channelware Launcher Uninstall") == true)
                return "Channelware";

            name = pex.LegalTrademarks;
            // Found in "CWAuto.dll" and "Upgrader.exe" in the "TOYSTORY" installation folder from Redump entry 12354.
            if (name?.Equals("Channelware") == true)
                return "Channelware";

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entries 12354 and 116358.
                new(new FilePathMatch("cwlaunch.hlp"), "Channelware"),

                // Found in the "Channelware\CWBrowse" folder installed from Redump entry 116358, and in the "Channelware" folder installed from Redump entry 12354.
                new(new FilePathMatch("cwbrowse.exe"), "Channelware"),

                // Found in the "Channelware" folder installed from Redump entry 12354.
                new(new FilePathMatch("cwuninst.exe"), "Channelware"),
                new(new FilePathMatch("chanwr.ini"), "Channelware"),
                new(new FilePathMatch("CWAuto.dll"), "Channelware"),

                // TODO: Add check for "CWare" and "cware" folders.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entries 12354 and 116358.
                new(new FilePathMatch("cwlaunch.hlp"), "Channelware"),

                // Found in the "Channelware\CWBrowse" folder installed from Redump entry 116358, and in the "Channelware" folder installed from Redump entry 12354.
                new(new FilePathMatch("cwbrowse.exe"), "Channelware"),

                // Found in the "Channelware" folder installed from Redump entry 12354.
                new(new FilePathMatch("cwuninst.exe"), "Channelware"),
                new(new FilePathMatch("chanwr.ini"), "Channelware"),
                new(new FilePathMatch("CWAuto.dll"), "Channelware"),

                // TODO: Add check for "CWare" and "cware" folders.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
