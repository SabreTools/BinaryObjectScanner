using System;
using System.Text;
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
    public class AlphaROM : IDiskImageCheck<ISO9660>, IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckDiskImage(string file, ISO9660 diskImage, bool includeDebug)
        {
            // Checks can be made even easier once UDF support exists, as most (although not all, some early discs like
            // redump ID 124111 have no UDF partition) discs have "Settec" slathered over every field UDF lets them.

            if (diskImage.VolumeDescriptorSet.Length == 0)
                return null;
            if (diskImage.VolumeDescriptorSet[0] is not PrimaryVolumeDescriptor pvd)
                return null;

            // Disc has varying (but observed to at least always be larger than 14) length
            // application identifier string made up of numbers and capital letters.
            // TODO: triple-check that length is never below 14
            int offset = 0;
            var applicationIdentifierString = pvd.ApplicationIdentifier.ReadNullTerminatedAnsiString(ref offset)?.Trim();
            if (applicationIdentifierString == null || applicationIdentifierString.Length < 14)
                return null;

            if (!Regex.IsMatch(applicationIdentifierString, "^[A-Z0-9]*$"))
                return null;

            // While some alpharom discs have data in the publisher identifier that can be checked, not all of them do,
            // so that can't reliably be used. There are two formats currently observed regarding the application
            // identifier strings.
            // #1 examples: DCOBG11C1B094961XN, DCXA9083CA554846GP, RCXA1107UD2510461A
            // #2 examples: 2003120514103077LAHD, 20040326195254AVKC, 20051019163346WXUDCD

            var applicationIdentifierStringBytes = Encoding.ASCII.GetBytes(applicationIdentifierString);
            
            // Type #1: 18 characters long, mix of letters and numbers. Since the string has already been confirmed
            // to only consist of capital letters and numbers, a basic byte value check can be performed to ensure
            // at least 5 bytes are numbers and 5 bytes are letters. Unfortunately, there doesn't seem to be quite
            // enough of a pattern to have a better check than this, but it works well enough.
            if (applicationIdentifierString.Length == 18
                && Array.FindAll(applicationIdentifierStringBytes, b => b < 60).Length >= 5
                && Array.FindAll(applicationIdentifierStringBytes, b => b > 60).Length >= 5)
            {
                return "AlphaROM";
            }
            
            // Type #2: Usually 20 characters long, but Redump ID 124334 is 18 characters long. Validate that it
            // starts with YYYYMMDD, followed by 6-8 more numbers, followed by letters.
            if (applicationIdentifierString.Length >= 18 && applicationIdentifierString.Length <= 20)
            {
                if (Int32.TryParse(applicationIdentifierString.Substring(0, 4), out int year) == false
                    || Int32.TryParse(applicationIdentifierString.Substring(4, 2), out int month) == false
                    || Int32.TryParse(applicationIdentifierString.Substring(6, 2), out int day) == false
                    || Int32.TryParse(applicationIdentifierString.Substring(8, 6), out int extraTime) == false)
                {
                    return null;
                }
                
                if (year >= 2009 || year < 2000 || month > 12 || day > 31)
                    return null;
                
                int index = Array.FindIndex(applicationIdentifierStringBytes, b => b > 60);
                
                var startingNumbers = Encoding.ASCII.GetBytes(applicationIdentifierString.Substring(0, index));
                var finalCharacters = Encoding.ASCII.GetBytes(applicationIdentifierString.Substring(index));
                if (Array.TrueForAll(startingNumbers, b => b < 60) && Array.TrueForAll(finalCharacters, b => b > 60))
                    return "AlphaROM";
            }

            return null;
        }

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
    }
}
