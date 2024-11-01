using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// DigiGuard (AKA DigiGuard ESD) was an online activation based DRM by Greenleaf Technologies Corporation. It was seemingly used for several OEM game collections, such as for Dell and Sony.
    /// Discs with DigiGuard appear to contain the full games in an encrypted format, with the option of purchasing the game online or via telephone so they can be installed from the disc.
    /// 
    /// Links:
    /// First archived GLFC website: https://web.archive.org/web/19991008072239/http://www.glfc.com:80/
    /// Later GLFC website: https://web.archive.org/web/20010331132439/http://www.glfc.com/index2.html
    /// Press release regarding the use of DigiGuard with Sony OEM discs: https://web.archive.org/web/20001010225304/http://www.glfc.com:80/press_releases/8_23_00.asp
    /// Press release regarding the use of DigiGuard with Dell OEM discs: https://web.archive.org/web/20010430042245fw_/http://www.glfc.com/pr_oct16_00.html
    /// Press release regarding the use of DigiGuard with the "Accolade Family Spectacular" DVD: https://web.archive.org/web/20010430042245fw_/http://www.glfc.com/pr_oct16_00.html
    /// Press release regarding GLFC and Ritek: https://www.bloomberg.com/press-releases/2000-10-24/ritek-and-greenleaf-technologies-align-to-drive-encrypted-dvd
    /// Press release regarding GLFC and Dell: https://www.bloomberg.com/press-releases/2000-10-04/greenleaf-technologies-signs-agreement-with-dell-computers-to
    /// First list of archived GLFC press releases: https://web.archive.org/web/*/http://glfc.com/press_releases/*
    /// Second list of archived GLFC press releases (Search for "pr_"): https://web.archive.org/web/*/http://glfc.com/* 
    /// News article regarding GLFC and Accolade Games: https://www.gamespot.com/articles/accolade-games-on-dvd/1100-2460436/
    /// eBay listing for the "BIGWIG SOFTWARE LOCKER", which very likely contains DigiGuard: https://www.ebay.com/itm/325573968970
    /// </summary>
    public class DigiGuard : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in "Start.exe" in IA item "Nova_DellBigWIGDVD_USA"/Redump entry 108588.
            var name = pex.FileDescription;
            if (name?.Equals("DigiGuard3 Client") == true)
                return $"DigiGuard3";

            // Found in "Start.exe" in IA item "Nova_DellBigWIGDVD_USA"/Redump entry 108588.
            name = pex.LegalTrademarks;
            if (name?.Equals("DigiGuard") == true)
                return $"DigiGuard";

            // Found in "PJS3.exe" in IA item "Nova_DellBigWIGDVD_USA"/Redump entry 108588.
            name = pex.ProductName;
            if (name?.Equals("Greenleaf Wrapper3") == true)
                return $"DigiGuard";

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "Nova_DellBigWIGDVD_USA"/Redump entry 108588.
                new(new FilePathMatch("DecryptWrap.exe"), "DigiGuard"),

                // There are at least two additional specifically named DecryptWrap files, "DecryptWrapTW2000.exe" and "DecryptWrapTW2KCode.exe" in IA item "Nova_DellBigWIGDVD_USA"/Redump entry 108588.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "Nova_DellBigWIGDVD_USA"/Redump entry 108588.
                new(new FilePathMatch("DecryptWrap.exe"), "DigiGuard"),

                // There are at least two additional specifically named DecryptWrap files, "DecryptWrapTW2000.exe" and "DecryptWrapTW2KCode.exe" in IA item "Nova_DellBigWIGDVD_USA"/Redump entry 108588.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
