using System;
using System.Text.RegularExpressions;
using BinaryObjectScanner.Interfaces;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;
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
    public class AlphaROM : IExecutableCheck<PortableExecutable>, IDiskImageCheck<ISO9660>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Add support for detecting Alpha-ROM found in older games made with the RealLive engine. 
            // TODO: Add version detection for Alpha-ROM.

            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("\\SETTEC")))
                    return "Alpha-ROM";

                if (strs.Exists(s => s.Contains("SETTEC0000")))
                    return "Alpha-ROM";
            }

            // Get the .rdata section strings, if they exist
            strs = exe.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("This Game is Japan Only")))
                    return "Alpha-ROM";

                // Found in "Filechk.exe" in Redump entry 115358.
                if (strs.Exists(s => s.Contains("AlphaCheck.exe")))
                    return "Alpha-ROM";

                // Found in "Uninstall.exe" in Redump entry 115358.
                if (strs.Exists(s => s.Contains("AlphaCheck.dat")))
                    return "Alpha-ROM";
            }

            // Get the overlay data, if it exists
            if (exe.OverlayStrings != null)
            {
                // Found in Redump entry 84122.
                if (exe.OverlayStrings.Exists(s => s.Contains("SETTEC0000")))
                    return "Alpha-ROM";
            }

            return null;
        }
         /// <inheritdoc/>
        public string? CheckDiskImage(string file, ISO9660 diskImage, bool includeDebug)
        {
            // Checks can be made even easier once UDF support exists, as most (although not all, some early discs like
            // redump ID 124111 have no UDF partition) discs have "Settec" slathered over every field UDF lets them.

            if (diskImage.VolumeDescriptorSet[0] is not PrimaryVolumeDescriptor pvd)
                return null;
            
            // Alpharom disc check #1: disc has varying (but observed to at least always be larger than 14) length 
            // string made up of numbers and capital letters.
            // TODO: triple-check that length is never below 14
            int offset = 0;
            var applicationIdentifierString = pvd.ApplicationIdentifier.ReadNullTerminatedAnsiString(ref offset)?.Trim();
            if (applicationIdentifierString == null || applicationIdentifierString.Length < 14)
                return null;

            if (!Regex.IsMatch(applicationIdentifierString, "^[A-Z0-9]*$"))
                return null;
            
            offset = 0;
            
            // Alpharom disc check #2: disc has publisher identifier filled with varying amount of data (26-50 bytes
            // have been observed) followed by spaces. There's a decent chance this is just a Japanese text string, but
            // UTF, Shift-JIS, and EUC-JP all fail to display anything but garbage.
            
            var publisherIdentifier = pvd.PublisherIdentifier;
            int firstSpace = Array.FindIndex(publisherIdentifier, b => b == 0x20);
            if (firstSpace <= 10 || firstSpace >= 120)
                return null;
            
            var publisherData = new byte[firstSpace];
            var publisherSpaces = new byte[publisherData.Length - firstSpace];
            Array.Copy(publisherIdentifier, 0, publisherData, 0, firstSpace);
            Array.Copy(publisherIdentifier, firstSpace, publisherSpaces, 0, publisherData.Length - firstSpace);
            
            if (!Array.TrueForAll(publisherSpaces, b => b == 0x20))
                return null;
            
            if (!FileType.ISO9660.IsPureData(publisherData))
                return null;
            
            return "AlphaROM";
        }
    }
}
