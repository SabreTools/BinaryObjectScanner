using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
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
    /// https://vndb.org/r?f=fwSiglusEngine- (VNs made with an engine known to use Alpha-ROM).
    /// https://vndb.org/r?f=fwRealLive- (VNs made with an engine known to use Alpha-ROM).
    /// References and further information:
    /// http://hhg.sakura.ne.jp/cd-dvd/dust/alpha/1_twin_sector.htm
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
    public class AlphaROM : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Add support for detecting Alpha-ROM found in older games made with the RealLive engine. 
            // TODO: Add version detection for Alpha-ROM.

            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // \SETTEC
                    new ContentMatchSet(new byte?[] { 0x5C, 0x53, 0x45, 0x54, 0x54, 0x45, 0x43 }, "Alpha-ROM"),

                    // SETTEC0000
                    new ContentMatchSet(new byte?[] { 0x53, 0x45, 0x54, 0x54, 0x45, 0x43, 0x30, 0x30, 0x30, 0x30 }, "Alpha-ROM"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rdata section, if it exists
            if (pex.ResourceDataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // T.h.i.s. .G.a.m.e. .i.s. .J.a.p.a.n. .O.n.l.y.....S0n0²0ü0à0o0åe,gýV…Qg0n0.0×0ì0¤0ïSý€g0Y0.0....ÕR\OU0[0‹0k0o0 ..0 .åe,gžŠHrW.i.n.d.o.w.s. ..0 .L0Å_.‰g0Y0.0....²0ü0à0’0B}†NW0~0Y0.0....åe,gžŠHrW.i.n.d.o.w.s
                    // Found in games using the Siglus engine.
                    new ContentMatchSet(new byte?[] 
                    { 
                        0x54, 0x00, 0x68, 0x00, 0x69, 0x00, 0x73, 0x00, 0x20, 0x00, 0x47, 0x00,
                        0x61, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x20, 0x00, 0x69, 0x00, 0x73, 0x00,
                        0x20, 0x00, 0x4A, 0x00, 0x61, 0x00, 0x70, 0x00, 0x61, 0x00, 0x6E, 0x00,
                        0x20, 0x00, 0x4F, 0x00, 0x6E, 0x00, 0x6C, 0x00, 0x79, 0x00, 0x0A, 0x00,
                        0x0A, 0x00, 0x53, 0x30, 0x6E, 0x30, 0xB2, 0x30, 0xFC, 0x30, 0xE0, 0x30,
                        0x6F, 0x30, 0xE5, 0x65, 0x2C, 0x67, 0xFD, 0x56, 0x85, 0x51, 0x67, 0x30,
                        0x6E, 0x30, 0x7F, 0x30, 0xD7, 0x30, 0xEC, 0x30, 0xA4, 0x30, 0xEF, 0x53,
                        0xFD, 0x80, 0x67, 0x30, 0x59, 0x30, 0x02, 0x30, 0x0A, 0x00, 0x0A, 0x00,
                        0xD5, 0x52, 0x5C, 0x4F, 0x55, 0x30, 0x5B, 0x30, 0x8B, 0x30, 0x6B, 0x30,
                        0x6F, 0x30, 0x20, 0x00, 0x0E, 0x30, 0x20, 0x00, 0xE5, 0x65, 0x2C, 0x67,
                        0x9E, 0x8A, 0x48, 0x72, 0x57, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x64, 0x00,
                        0x6F, 0x00, 0x77, 0x00, 0x73 
                    }, "Alpha-ROM"),

                    // This Game is Japan Only..‚±‚ÌƒQ.[ƒ€‚Í“ú–{.‘“à‚Å‚Ì‚ÝƒvƒŒƒC‰Â”\‚Å‚·.B..“®.ì‚³‚¹‚é‚É‚Í .w “ú–{Œê”ÅWindows
                    // Found in games using the RealLive engine.
                    new ContentMatchSet(new byte?[] 
                    {
                        0x54, 0x68, 0x69, 0x73, 0x20, 0x47, 0x61, 0x6D, 0x65, 0x20, 0x69, 0x73,
                        0x20, 0x4A, 0x61, 0x70, 0x61, 0x6E, 0x20, 0x4F, 0x6E, 0x6C, 0x79, 0x0A,
                        0x0A, 0x82, 0xB1, 0x82, 0xCC, 0x83, 0x51, 0x81, 0x5B, 0x83, 0x80, 0x82,
                        0xCD, 0x93, 0xFA, 0x96, 0x7B, 0x8D, 0x91, 0x93, 0xE0, 0x82, 0xC5, 0x82,
                        0xCC, 0x82, 0xDD, 0x83, 0x76, 0x83, 0x8C, 0x83, 0x43, 0x89, 0xC2, 0x94,
                        0x5C, 0x82, 0xC5, 0x82, 0xB7, 0x81, 0x42, 0x0A, 0x0A, 0x93, 0xAE, 0x8D,
                        0xEC, 0x82, 0xB3, 0x82, 0xB9, 0x82, 0xE9, 0x82, 0xC9, 0x82, 0xCD, 0x20,
                        0x81, 0x77, 0x20, 0x93, 0xFA, 0x96, 0x7B, 0x8C, 0xEA, 0x94, 0xC5, 0x57,
                        0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73
                    }, "Alpha-ROM"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.ResourceDataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
