using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Alpha-ROM is a form of copy protection created by SETTEC. It is known to make use of twin sectors as well as region locking.
    /// Later forms of Alpha-ROM appear to be digital only, and it's currently unsure what forms of protection the digital only version includes, except that it does make use of region locking.
    /// It seems that Alpha-ROM was used in Visual Novels using certain game engines, most notably RealLive and Siglus (https://forums.fuwanovel.net/topic/20927-cannot-crack-siglus-engine-with-alpharom/). 
    /// Not every Siglus engine game uses Alpha-ROM (Source: https://sample9.dmm.co.jp/digital/pcgame/vsat_0263/vsat_0263t.zip {Official trial mirror}).
    /// Not every RealLive engine game uses Alpha-ROM (Source: IA item "Kanon_Standard_Edition_Japan").
    /// Alpha-ROM also seems to have made use of something called "Alpha-DPS" for non-executable data files (http://www.gonsuke.co.jp/protect.html).
    /// Example of Alpha-ROM (official trial download mirrors):
    /// (Siglus Engine)
    /// http://suezou.dyndns.org/dl2018/key/summer_pokets/Summer_Pockets_trial.zip
    /// http://mirror.studio-ramble.com/upload/300/201103/RewriteTE_Ver200.zip
    /// http://suezou.dyndns.org/dl2012/tone-works/hatsukoi1-1/hatsukoi_tr_web.zip
    /// (RealLive Engine)
    /// http://suezou.dyndns.org/dl2020/hadashi/princess_heart_link/phl_trial.exe
    /// https://archive.org/details/little-busters-regular-edition-iso-only-2007
    /// Games that may have Alpha-ROM:
    /// http://cpdb.kemuri-net.com/ (Protection database that includes many different protections, including Alpha-ROM).
    /// https://w.atwiki.jp/tirasinoura/pages/3.html (List of games with Alpha-ROM, and explains some version differences).
    /// https://vndb.org/r?f=fwSiglusEngine- (VNs made with an engine known to use Alpha-ROM).
    /// https://vndb.org/r?f=fwRealLive- (VNs made with an engine known to use Alpha-ROM).
    /// References and further information:
    /// http://hhg.sakura.ne.jp/cd-dvd/dust/alpha/alpha_index.htm
    /// https://www.weblio.jp/content/Alpha-ROM
    /// https://ameblo.jp/michael-j-fox/entry-10046574609.html
    /// http://s2000.yokinihakarae.com/sub03-10-2(DVD).html
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_alpha.shtml
    /// Special thanks to Bestest for researching this protection and helping make further improvements possible!
    /// </summary>

    // TODO: Alternative string possibilities:
    //      - \AlphaDiscLog.txt
    //      - \SETTEC
    //      - AlphaROM
    //      - SETTEC0000SETTEC1111
    //      - SOFTWARE\SETTEC
    // TODO: Are there version numbers?
    public class AlphaROM : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Add support for detecting Alpha-ROM found in older games made with the RealLive engine. 
            // TODO: Add version detection for Alpha-ROM.

            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("\\SETTEC")))
                    return "Alpha-ROM";

                if (strs.Any(s => s.Contains("SETTEC0000")))
                    return "Alpha-ROM";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("This Game is Japan Only")))
                    return "Alpha-ROM";
                // Found in "Filechk.exe" in Redump entry 115358.
                if (strs.Any(s => s.Contains("AlphaCheck.exe")))
                    return "Alpha-ROM";
                // Found in "Uninstall.exe" in Redump entry 115358.
                if (strs.Any(s => s.Contains("AlphaCheck.dat")))
                    return "Alpha-ROM";
            }

            // Get the overlay data, if it exists
            if (pex.OverlayStrings != null)
            {
                // Found in Redump entry 84122.
                if (pex.OverlayStrings.Any(s => s.Contains("SETTEC0000")))
                    return "Alpha-ROM";
            }

            return null;
        }
    }
}
