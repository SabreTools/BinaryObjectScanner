using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
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
    public class Channelware : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Found in "AbeWincw.dll" in Redump entry 116358 and in "TOYSGMcw.dll" in the "TOYSTORY" installation folder from Redump entry 12354.
            var name = pex.ProductName;
            if (name.OptionalEquals("ChannelWare Utilities"))
                return "Channelware";

            // Found in "cwbrowse.exe" in the "Channelware" folder installed from Redump entry 12354.
            if (name.OptionalEquals("Channelware Browser Launcher"))
                return "Channelware";

            // Found in "cwuninst.exe" in the "Channelware" folder installed from Redump entry 12354.
            if (name.OptionalEquals("Channelware Launcher Uninstall Application"))
                return "Channelware";

            // Found in "cwbrowse.exe" in the "Channelware\CWBrowse" folder installed from Redump entry 116358.
            if (name.OptionalEquals("Channelware Authorization Server Browser Launcher"))
                return "Channelware";

            name = pex.FileDescription;
            // Found in "cwuninst.exe" in the "Channelware" folder installed from Redump entry 12354.
            if (name.OptionalEquals("Channelware Launcher Uninstall"))
                return "Channelware";

            name = pex.LegalTrademarks;
            // Found in "CWAuto.dll" and "Upgrader.exe" in the "TOYSTORY" installation folder from Redump entry 12354.
            if (name.OptionalEquals("Channelware"))
                return "Channelware";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
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

                // Found in Redump entry 116358.
                new(Path.Combine("CWare", "install.exe"), "Channelware"),

                // Found in Redump entry 12354.
                new(Path.Combine("cware", "Install.exe"), "Channelware"),
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

                // Found in Redump entry 116358.
                new(Path.Combine("CWare", "install.exe"), "Channelware"),

                // Found in Redump entry 12354.
                new(Path.Combine("cware", "Install.exe"), "Channelware"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
