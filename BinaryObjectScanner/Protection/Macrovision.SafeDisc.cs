using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;
using static BinaryObjectScanner.Utilities.Hashing;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// SafeDisc is an incredibly commonly used copy protection created by Macrovision and C-Dilla in 1998 (https://www.thefreelibrary.com/MACROVISION+ACQUIRES+C-DILLA+CD-ROM+TECHNOLOGIES+DEVELOPER-a055217923).
    /// It uses several different copy protection mechanisms, such as reading a disc signature dependent on the presence of bad sectors and the attempted prevention of burning copies to CD-R.
    /// SafeDisc has been most commonly found on PC games and applications, though there a number of Mac discs that contain the protection as well.
    /// At least one system other than PC/Mac is known to use SafeDisc as well, this being the "ZAPiT Games Game Wave Family Entertainment System" which seems to use a form of SafeDisc 4 (Redump entry 46269).
    /// SafeDisc resources:
    /// https://web.archive.org/web/20010707163339/http://www.macrovision.com:80/demos/safedisc.exe (SafeDisc Demo)
    /// https://web.archive.org/web/20000307003925/http://www.macrovision.com/scp_faq.html (SafeDisc FAQ)
    /// https://web.archive.org/web/20030620175810/http://www.macrovision.com:80/solutions/software/cdrom/SafeDisc_V2_FAQ_April_2002.pdf (SafeDisc 2 FAQ)
    /// https://web.archive.org/web/20040610212031/http://www.macrovision.com:80/pdfs/SafeDisc_V3_FAQ_Oct2003.pdf (SafeDisc 3 FAQ)
    /// https://web.archive.org/web/20040610205241/http://www.macrovision.com:80/pdfs/SafeDisc_v315_FAQ_Dec2003.pdf (SafeDisc 3.15 FAQ)
    /// https://web.archive.org/web/20051015170118/http://www.macrovision.com/pdfs/safedisc_v4_FAQ_sep2004.pdf (SafeDisc 4 FAQ)
    /// https://web.archive.org/web/20070124144331/http://www.macrovision.com/pdfs/SafeDisc_Brochure_Oct04.pdf (SafeDisc brochure)
    /// https://web.archive.org/web/20041008173722/http://www.macrovision.com/pdfs/safedisc_datasheet.pdf (SafeDisc datasheet)
    /// https://web.archive.org/web/20030421023647/http://www.macrovision.com:80/solutions/software/cdrom/SafeDisc_WhitePaper_4-17-02-web.pdf (SafeDisc WhitePaper)
    /// https://web.archive.org/web/20011005034102/http://www.macrovision.com/solutions/software/cdrom/pccdrom/safedischd.php3 (SafeDisc HD product page)
    /// https://web.archive.org/web/20031009091909/http://www.macrovision.com/products/safedisc/index.shtml
    /// https://web.archive.org/web/20041023011150/http://www.macrovision.com/products/safedisc/index.shtml (Marketed as "SafeDisc Advanced")
    /// https://web.archive.org/web/20080604020524/http://www.trymedia.com/safedisc-advanced.html
    /// https://web.archive.org/web/20041008173722/http://www.macrovision.com/pdfs/safedisc_datasheet.pdf
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_safedisc.shtml
    /// https://computerizedaccount.tripod.com/computerizedaccountingtraining/id27.html
    /// 
    /// SafeDisc Lite/LT is an alternate version of SafeDisc available that was based on SafeDisc 1 (https://web.archive.org/web/20030421023647/http://www.macrovision.com:80/solutions/software/cdrom/SafeDisc_WhitePaper_4-17-02-web.pdf).
    /// Although seemingly only officially referred to as "SafeDisc LT", a multitude of sources, including one that seemingly worked directly with Macrovision, call it "SafeDisc Lite" (http://www.eclipsedata.com/insidepages.asp?pageID=149).
    /// Other protections in the Macrovision "Safe-" family of protections that need further investigation:
    /// SafeScan (https://cdn.loc.gov/copyright/1201/2003/reply/029.pdf).
    /// SafeDisc HD (https://web.archive.org/web/20000129100449/http://www.macrovision.com/scp_hd.html).
    /// SafeAuthenticate (https://web.archive.org/web/20041020010136/http://www.ttrtech.com/pdf/SafeAudioFAQ.pdf)
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
#if NET48
        internal string SafeDiscCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        internal string? SafeDiscCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in Redump entry 57986.
            bool hintNameTableMatch = pex.Model.ImportTable?.HintNameTable?.Any(ihne => ihne?.Name == "LTDLL_Authenticate") ?? false;
            if (hintNameTableMatch)
                return "SafeDisc Lite";

            // Found in Redump entry 57986.
            bool importTableMatch = pex.Model.ImportTable?.ImportDirectoryTable?.Any(idte => idte?.Name == "ltdll.dll") ?? false;
            if (importTableMatch)
                return "SafeDisc Lite";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in Redump entries 14928, 25579, 32751.
                if (strs.Any(s => s.Contains("LTDLL_Initialise")))
                    return "SafeDisc Lite";
                if (strs.Any(s => s.Contains("LTDLL_Authenticate")))
                    return "SafeDisc Lite";
                if (strs.Any(s => s.Contains("LTDLL_Unwrap")))
                    return "SafeDisc Lite";
            }

            var name = pex.FileDescription;
            // Present in "Diag.exe" files from SafeDisc 4.50.000+.
            if (name?.Equals("SafeDisc SRV Tool APP", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeDisc SRV Tool APP {GetSafeDiscDiagExecutableVersion(pex)}";

            // Present on all "CLOKSPL.DLL" versions before SafeDisc 1.06.000. Found on Redump entries 61731 and 66004. 
            name = pex.ProductName;
            if (name?.Equals("SafeDisc CDROM Protection System", StringComparison.OrdinalIgnoreCase) == true)
                return "SafeDisc 1.00.025-1.01.044";
            // Present in "Diag.exe" files from SafeDisc 4.50.000+.
            else if (name?.Equals("SafeDisc SRV Tool APP", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeDisc SRV Tool APP {GetSafeDiscDiagExecutableVersion(pex)}";

            // Present on all "CLOKSPL.EXE" versions before SafeDisc 1.06.000. Found on Redump entries 61731 and 66004. 
            // Only found so far on SafeDisc 1.00.025-1.01.044, but the report is currently left generic due to the generic nature of the check.
            name = pex.FileDescription;
            if (name?.Equals("SafeDisc", StringComparison.OrdinalIgnoreCase) == true)
                return "SafeDisc";

            // Found in Redump entries 20729 and 65569.
            // Get the debug data
            if (pex.FindCodeViewDebugTableByPath("SafeDisc").Any() || pex.FindCodeViewDebugTableByPath("Safedisk").Any())
                return "SafeDisc";

            // TODO: Investigate various section names:
            // "STLPORT_" - Found in Redump entry 11638.
            // "PACODE" - Found in Redump entry 9621.
            // "CSEG" + "DSEG" + "TQIA_DAT" + "GRPOLY_D"  - Found in Redump entry 72195.
            // "LBMPEG_D" - Found in Redump entries 11638 and 72195.
            // "UVA_DATA" + "IDCT_DAT" Found in Redump entries 9621 and 72195.

            // TODO: Add entry point check
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Safedisc.2.sg

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
#if NET48
        internal ConcurrentQueue<string> SafeDiscCheckDirectoryPath(string path, IEnumerable<string> files)
#else
        internal ConcurrentQueue<string> SafeDiscCheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("CLCD16.DLL", useEndsWith: true),
                    new PathMatch("CLCD32.DLL", useEndsWith: true),
                    new PathMatch("CLOKSPL.EXE", useEndsWith: true),
                    new PathMatch(".icd", useEndsWith: true),
                }, "SafeDisc 1/Lite"),

                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("00000001.TMP", useEndsWith: true),
                    // The .016 and .256 files are banners stored in the BMP image format. The 016 and 256 refers to the color depth of the BMP.
                    // There are common file names used, such as 00000407.XXX and 00000409.XXX. Further investigation is needed to determine the consistency of these names.
                    new PathMatch(".016", useEndsWith: true),
                    new PathMatch(".256", useEndsWith: true),
                }, "SafeDisc 1.06.000-3.20.024"),

                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("00000001.TMP", useEndsWith: true),
                    // The .016 files stop being used as of 4.00.000, while the .256 remain in fairly consistent use.
                    new PathMatch(".256", useEndsWith: true),
                }, "SafeDisc 1.06.000+"),

                // The file "mcp.dll" is known to only be used in a specific version range for SafeDisc, but is currently only used in a grouped file name check with other SafeDisc files to prevent false positives.
                // Found in Redump entries 28810, 30555, 55078, and 62935.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("00000001.TMP", useEndsWith: true),
                    new PathMatch("drvmgt.dll", useEndsWith: true),
                    new PathMatch("mcp.dll", useEndsWith: true),
                    new PathMatch("secdrv.sys", useEndsWith: true),

                }, "SafeDisc 1.45.011-1.50.020"),

                // TODO: Research "splash16.bmp" and "splash256.bmp".

                // Found to be present in every version of SafeDisc, possibly every single release.
                //new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), GetSafeDisc00000001TMPVersion, "SafeDisc"),

                // Found in many versions of SafeDisc, beginning in 2.05.030 and being used all the way until the final version 4.90.010. It is not always present, even in versions it has been used in. Found in Redump entries 56319 and 72195.
                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2+"),

                new PathMatchSet(new FilePathMatch("DPLAYERX.DLL"), GetSafeDiscDPlayerXVersion, "SafeDisc"),
                new PathMatchSet(new FilePathMatch("drvmgt.dll"), GetSafeDiscDrvmgtVersion, "SafeDisc"),

                // The SD0XXX.dll files appear to solely contain custom strings that allowed the publisher to customize the SafeDisc error messages. They are currently only known to be used by EA.
                // Each file appears to contain strings for a specific language each.
                // There is one non EA game that makes reference to "SD0809.dll", but doesn't include it (IA item "rayman.-raving.-rabbids.-hebrew.-dvd").
                // TODO: Add a generic check to detect this type of string that appears to be present in all of these DLLs:
                // "d:\DiceCanada\BoosterPack2\Installers\BF2XpackInstaller\Safedisk\SafeDiscDLLs\DLLs\Release\SD0816.pdb" (Redump entry 65569).

                // Found in Redump entries 20729 and 65569.
                new PathMatchSet(new PathMatch("SD040e.dll", useEndsWith: true), "SafeDisc"),

                // Found in Redump entry 65569.
                new PathMatchSet(new PathMatch("SD0c0a.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD040b.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD040c.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0419.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD041d.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD041e.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0404.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0405.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0406.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0407.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0409.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0410.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0411.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0412.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0413.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0414.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0415.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0416.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0804.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0809.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0816.dll", useEndsWith: true), "SafeDisc"),

                // Used to distribute SafeDisc driver updates over the internet. Two distinct versions known to exist, with Microsoft also having distributed the later update as well.
                // Version 1: https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe
                // Version 2: https://web.archive.org/web/20051104123646/http://www.macrovision.com/products/safedisc/safedisc.exe
                // Microsoft download page: https://web.archive.org/web/20080204081329/http://www.microsoft.com/downloads/details.aspx?FamilyID=eae20f0f-c41c-44fe-84ce-1df707d7a2e9&DisplayLang=en
                new PathMatchSet(new PathMatch("safedisc.exe", useEndsWith: true), "SafeDisc Driver Installer"),

                // Found in Redump entries 28810 and 30555.
                // Name check overmatches with a seemingly completely unrelated application, ironically included on at least one SafeDisc game (Redump entry 34828).
                // new PathMatchSet(new PathMatch("mcp.dll", useEndsWith: true), "SafeDisc (Version 1.45.011-1.50.020)"),

                // Found in Redump entry 58455 and 65569.
                // Unknown if it's a game specific file, but it contains the stxt371 and stxt774 sections.
                // new PathMatchSet(new PathMatch("CoreDLL.dll", useEndsWith: true), "SafeDisc"),

                // Found in seemingly every SafeDisc Lite disc. (CD: Redump entries 25579 and 57986. DVD: Redump entry 63941). 
                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),
                new PathMatchSet(new PathMatch("LTDLL.DLL", useEndsWith: true), "SafeDisc Lite"),

                // Found on Redump entry 42762.
                new PathMatchSet(".SafeDiscDVD.bundle", "SafeDisc for Macintosh"),
            };

            return MatchUtil.GetAllMatches(files ?? Array.Empty<string>(), matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
#if NET48
        internal string SafeDiscCheckFilePath(string path)
#else
        internal string? SafeDiscCheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CLCD16.DLL", useEndsWith: true), GetSafeDiscCLCD16Version, "SafeDisc"),
                new PathMatchSet(new PathMatch("CLCD32.DLL", useEndsWith: true), GetSafeDiscCLCD32Version, "SafeDisc"),
                new PathMatchSet(new PathMatch("CLOKSPL.EXE", useEndsWith: true), GetSafeDiscCLOKSPLVersion, "SafeDisc"),

                //new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), GetSafeDisc00000001TMPVersion, "SafeDisc"),
                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2+"),

                // TODO: Research "splash16.bmp" and "splash256.bmp".

                new PathMatchSet(new FilePathMatch("DPLAYERX.DLL"), GetSafeDiscDPlayerXVersion, "SafeDisc"),
                new PathMatchSet(new FilePathMatch("drvmgt.dll"), GetSafeDiscDrvmgtVersion, "SafeDisc"),

                // The SD0XXX.dll files appear to solely contain custom strings that allowed the publisher to customize the SafeDisc error messages. They are currently only known to be used by EA.
                // Each file appears to contain strings for a specific language each.
                // There is one non EA game that makes reference to "SD0809.dll", but doesn't include it (IA item "rayman.-raving.-rabbids.-hebrew.-dvd").
                // TODO: Add a generic check to detect this type of string that appears to be present in all of these DLLs:
                // "d:\DiceCanada\BoosterPack2\Installers\BF2XpackInstaller\Safedisk\SafeDiscDLLs\DLLs\Release\SD0816.pdb" (Redump entry 65569).

                // Found in Redump entries 20729 and 65569.
                new PathMatchSet(new PathMatch("SD040e.dll", useEndsWith: true), "SafeDisc"),

                // Found in Redump entry 65569.
                new PathMatchSet(new PathMatch("SD0c0a.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD040b.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD040c.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0419.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD041d.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD041e.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0404.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0405.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0406.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0407.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0409.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0410.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0411.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0412.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0413.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0414.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0415.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0416.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0804.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0809.dll", useEndsWith: true), "SafeDisc"),
                new PathMatchSet(new PathMatch("SD0816.dll", useEndsWith: true), "SafeDisc"),

                // Found in Redump entries 28810 and 30555.
                // Name check overmatches with a seemingly completely unrelated application, ironically included on at least one SafeDisc game (Redump entry 34828).
                // new PathMatchSet(new PathMatch("mcp.dll", useEndsWith: true), "SafeDisc (Version 1.45.011-1.50.020)"),

                // Found in Redump entry 58455 and 65569.
                // Unknown if it's a game specific file, but it contains the stxt371 and stxt774 sections.
                // new PathMatchSet(new PathMatch("CoreDLL.dll", useEndsWith: true), "SafeDisc"),

                // Found in Redump entry 58990.
                new PathMatchSet(new PathMatch("SafediskSplash.bmp", useEndsWith: true), "SafeDisc"),
                
                // Used to distribute SafeDisc driver updates over the internet. Two distinct versions known to exist, with Microsoft also having distributed the later update as well.
                // Version 1: https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe
                // Version 2: https://web.archive.org/web/20051104123646/http://www.macrovision.com/products/safedisc/safedisc.exe
                // Microsoft download page: https://web.archive.org/web/20080204081329/http://www.microsoft.com/downloads/details.aspx?FamilyID=eae20f0f-c41c-44fe-84ce-1df707d7a2e9&DisplayLang=en
                new PathMatchSet(new PathMatch("safedisc.exe", useEndsWith: true), "SafeDisc Driver Installer"),

                // Found in seemingly every SafeDisc Lite disc. (CD: Redump entries 25579 and 57986. DVD: Redump entry 63941). 
                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),
                new PathMatchSet(new PathMatch("LTDLL.DLL", useEndsWith: true), "SafeDisc Lite"),

                // Found in Redump entry 42762.
                new PathMatchSet(".SafeDiscDVD.bundle", "SafeDisc for Macintosh"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        internal static string GetSafeDiscCLCD16Version(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The hash of the file CLCD16.dll is able to provide a broad version range that appears to be consistent, but it seems it was rarely updated so these checks are quite broad.
            var sha1 = GetFileSHA1(firstMatchedString);
            switch (sha1)
            {
                // Found in Redump entries 61731 and 66005.
                case "C13493AB753891B8BEE9E4E014896B026C01AC92":
                    return "1.00.025-1.01.044";
                // Found in Redump entries 1882 and 30049. 
                // It is currently unknown why the previous hash covers both the version before this one, and several afterwards, with this one being a consistent outlier between these versions.
                case "2418D791C7B9D4F05BCB01FAF98F770CDF798464":
                    return "1.00.026";
                // Found in Redump entries 31149 and 28810.
                case "848EDF9F45A8437438B7289BB4D2D1BCF752FD4A":
                    return "1.06.000-1.50.020/Lite";
                default:
                    return "Unknown Version (Report this to us on GitHub)";
            }
        }

        internal static string GetSafeDiscCLCD32Version(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The hash of the file CLCD32.dll so far appears to be a solid indicator of version for versions it was used with. It appears to have been updated with every release, unlike its counterpart, CLCD16.dll.
            var sha1 = GetFileSHA1(firstMatchedString);
            switch (sha1)
            {
                // Found in Redump entry 66005.
                case "BAD49BA0DEA041E85EF1CABAA9F0ECD822CE1376":
                    return "1.00.025";
                // Found in Redump entry 34828.
                case "6137C7E789A329865649FCB8387B963FC8C763C6":
                    return "1.00.026 (pre-10/1998)";
                // Found in Redump entries 1882 and 30049.
                case "AFEFBBF1033EA65C366A1156E21566DB419CFD7B":
                    return "1.00.026 (post-10/1998)";
                // Found in Redump entries 31575 and 41923.
                case "6E54AC24C344E4A132D1B7A6A61B2EC824DE5092":
                    return "1.00.030";
                // Found in Redump entries 1883 and 42114.
                case "23DAA95DAF75732C27CEB133A00F7E10D1D482D3":
                    return "1.00.032";
                // Found in Redump entries 36223 and 40771.
                case "C8F609DDFC3E1CF69FADD60B7AED7A63B4B1DA62":
                    return "1.00.035";
                // Found in Redump entries 42155 and 47574.
                case "39CC3C053702D9F6EFF0DF6535E54F6C78CEA639":
                    return "1.01.034";
                // Found in Redump entry 51459.
                case "BC476F625A4A7A89AE50E2A4CD0F248D6CEB5A84":
                    return "1.01.038";
                // Found in Redump entries 34562 and 63304.
                case "1AB79AA78F706A1A24C02CE2B9398EC78249700B":
                    return "1.01.043";
                // Found in Redump entries 61731 and 81619.
                case "E2326F66EA9C2E5153EC619EEE424D83E2FD4CA4":
                    return "1.01.044";
                // Found in Redump entries 29073 and 31149.
                case "AEDE9939C4B62AC6DCCE3A771919B23A661247B3":
                    return "1.06.000";
                // Found in Redump entries 9718 and 46756.
                case "B5503E2222B3DA387BB5D7150A4A32A47824988F":
                    return "1.07.000";
                // Found in Redump entries 12885 and 66210.
                case "7D33EA7B241245182FFB7A392873079B6183050B":
                    return "1.09.000";
                // Found in Redump entries 37523 and 66586.
                case "61A4A5A758A5CFFB226CE2AE96E55A40AB073AC6":
                    return "1.11.000";
                // Found in Redump entries 21154 and 37982.
                case "14D3267C1D5C925F6DA44F1B19CB14F6DFCA73E3":
                    return "1.20.000";
                // Found in Redump entry 37920.
                case "CB4570F3F37E0FA70D7B9F3211FDC2105864C664":
                    return "1.20.001";
                // Found in Redump entries 31526 and 55080.
                case "1B5FD2D3DFBD89574D602DA9AE317C55F24902F0":
                    return "1.30.010";
                // Found in Redump entries 9617 and 49552.
                case "CC73C219BFC2D729515D25CA1B93D53672153175":
                    return "1.35.000";
                // Found in Redump entries 2595 and 30121.
                case "5825FF56B50114CD5D82BD4667D7097B29973197":
                    return "1.40.004";
                // Found in Redump entries 44350 and 63323.
                case "38DE3C6CF8FA89E5E99C359AA8ABFC65ADE396A5":
                    return "1.41.000";
                // Found in Redump entries 37832 and 42091.
                case "894D38AD949576928F67FF1595DC9C877A34A91C":
                    return "1.41.001";
                // Found in Redump entries 30555 and 55078.
                case "0235E03CA78232417C93FBB5F56B1BE819926B0C":
                    return "1.45.011";
                // Found in Redump entries 28810 and 62935.
                case "331B777A0BA2A358982575EA3FAA2B59ECAEA404":
                    return "1.50.020";
                // Found in Redump entries 57986 and 63941.
                case "85A92DC1D9CCBA6349D70F489043E649A8C21F2B":
                    return "Lite";
                // Found in Redump entry 14928.
                case "538351FF5955A3D8438E8C278E9D6D6274CF13AB":
                    return "Lite";
                default:
                    return "Unknown Version (Report this to us on GitHub)";
            }
        }

        internal static string GetSafeDiscCLOKSPLVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // Versions of "CLOKSPL.EXE" before SafeDisc 1.06.000 (can be identified by the presence of "SafeDisc CDROM Protection System" as the Product Name) have a human readable Product Version.
            // This included version doesn't seem to be as accurate as hash checks are, but will be documented here and considered for future use if needed.
            // "1.0.25 1998/07/30" -> SafeDisc 1.00.025 (Redump entry 66005).
            // "1.0.26 1998/08/06" -> SafeDisc 1.00.026-1.00.030 (Redump entries 1882, 31575, and 34828).
            // "1.0.32 1998/11/04" -> SafeDisc 1.00.032-1.00.035 (Redump entries 1883 and 36223). 
            // "1.1.34 1998/11/14" -> SafeDisc 1.01.034 (Redump entries 42155 and 47574).
            // "1.1.38 1999/01/21" -> SafeDisc 1.01.038 (Redump entry 51459).
            // "1.1.43 1999/02/25  -> SafeDisc 1.01.043 (Redump entries 34562 and 63304).
            // "1.1.44 1999/03/08" -> SafeDisc 1.01.044 (Redump entries 61731 and 81619).


            // The hash of every "CLOKSPL.EXE" correlates directly to a specific SafeDisc version.
            var sha1 = GetFileSHA1(firstMatchedString);
            switch (sha1)
            {
                // Found in Redump entry 66005.
                case "DD131A7B988065764E2A0F20B66C89049B20A7DE":
                    return "1.00.025";
                // Found in Redump entry 34828.
                case "41C8699A6E0F046EB7A21984441B555237DA4758":
                    return "1.00.026 (pre-10/1998)";
                // Found in Redump entries 1882 and 30049.
                case "D1C19C26DEC7C33825FFC59AD02B0EBA782643FA":
                    return "1.00.026 (post-10/1998)";
                // Found in Redump entries 31575 and 41923.
                case "B7C6C61688B354AB5D4E20CDEB36C992F203289B":
                    return "1.00.030";
                // Found in Redump entries 1883 and 42114.
                case "7445CD9FB49C322D18E92CC457DD880967C2B010":
                    return "1.00.032";
                // Found in Redump entries 36223 and 40771.
                case "50D4466F55BEDB3FE0E262235A6BAC751CA26599":
                    return "1.00.035";
                // Found in Redump entries 42155 and 47574.
                case "8C2F792326856C6D326707F76823FC7430AC86D5":
                    return "1.01.034";
                // Found in Redump entry 51459.
                case "107BF8077255FD4CA0875FB7C306F0B427E66800":
                    return "1.01.038";
                // Found in Redump entries 34562 and 63304.
                case "E8F4BA30376FCDAE00E7B88312300172674ABFA9":
                    return "1.01.043";
                // Found in Redump entries 61731 and 81619.
                case "CAB911C5CFC0A13C822DBFE0F0E1570C09F211FB":
                    return "1.01.044";
                // Found in Redump entries 29073 and 31149.
                case "43C1318B38742E05E7C858A02D64EEA13D9DFB9B":
                    return "1.06.000";
                // Found in Redump entries 9718 and 46756.
                case "451BD4C60AB826C16840815996A5DF03672666A8":
                    return "1.07.000";
                // Found in Redump entries 12885 and 66210.
                case "6C02A20A521112777D4843B8ACD9278F34314A35":
                    return "1.09.000";
                // Found in Redump entries 37523 and 66586.
                case "0548F1B12F60395C9394DDB7BED5E3E65E09D72E":
                    return "1.11.000";
                // Found in Redump entries 21154 and 37982.
                case "64A406FE640F2AC86A0E23F619F6EBE63BFFB8A1":
                    return "1.20.000";
                // Found in Redump entry 37920.
                case "8E874C9AF4CE5A9F1CBE96FCC761AA1C201C6938":
                    return "1.20.001";
                // Found in Redump entries 31526 and 55080.
                case "766EC536A10E68513138D1183705F5F19B9B8091":
                    return "1.30.010";
                // Found in Redump entries 9617 and 49552.
                case "1F1460FD66DD518159CCCDC99C12252EA0B2EEC4":
                    return "1.35.000";
                // Found in Redump entries 2595 and 30121.
                case "B1CF007BA36BA1B207DE334635F7BCEC146F8E35":
                    return "1.40.004";
                // Found in Redump entries 44350 and 63323.
                case "90F92A6DB15387F4C7619C442493791EBFC1041B":
                    return "1.41.000";
                // Found in Redump entries 37832 and 42091.
                case "836D42BF7B7AD719AB67682CF8D6B2D9C07AD218":
                    return "1.41.001";
                // Found in Redump entries 30555 and 55078.
                case "24DE959BC4484CD95DAA26947670C63A161E64AE":
                    return "1.45.011";
                // Found in Redump entries 28810 and 62935.
                case "9758F0637184816D02049A53CD2653F0BFFE92C9":
                    return "1.50.020";
                default:
                    return "Unknown Version (Report this to us on GitHub)";
            }
        }

        internal static string GetSafeDiscDPlayerXVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                // File size of "dplayerx.dll" and others is a commonly used indicator of SafeDisc version, though it has been found to not be completely consistent.
                // Checks for versions 1.2X have been commented out, due to these versions already being detected via more accurate checks.
                // Examples of "dplayerx.dll" that are detected using these more accurate checks can be found in Redump entries 28810, 30121, and 37982. 

                // Found in Redump entry 34828.
                case 81_408:
                    return "1.00.026 (pre-10/1998)";

                // Found in Redump entries 21154, 41923, 42114, and 66005.
                case 78_848:
                    return "1.00.025-1.00.032";

                // Found in Redump entries 36223 and 40771.
                case 77_824:
                    return "1.00.035";

                // Found in Redump entries 42155 and 47574. 
                case 115_712:
                    return "1.01.034";

                // Found in Redump entry 42155.
                case 116_736:
                    return "1.01.038";

                // Found in Redump entries 34562 and 63304.
                case 124_416:
                    return "1.01.043";

                // Found in Redump entries 61731 and 81619.
                case 125_952:
                    return "1.01.044";

                // Found in Redump entries 29073 and 31149.
                case 155_648:
                    return "1.06.000";

                // Found in Redump entries 9718, 12885, and 37523.
                case 156_160:
                    return "1.07.000-1.11.000";

                // File size checks for versions 1.2X+ are superceded by executable string checks, which are more accurate. For reference, the previously used file sizes are kept as comments.
                // 157,184 bytes corresponds to SafeDisc 1.20.000-1.20.001 (Redump entries 21154 and 37920).
                // 163,382 bytes corresponds to SafeDisc 1.30.010 (Redump entries 31526 and 55080).
                // 165,888 bytes corresponds to SafeDisc 1.35.000 (Redump entries 9617 and 49552).
                // 172,544 bytes corresponds to SafeDisc 1.40.004 (Redump entries 2595 and 30121).
                // 173,568 bytes corresponds to SafeDisc 1.41.000-1.41.001 (Redump entries 37832, and 44350). 
                // 136,704 bytes corresponds to SafeDisc 1.45.011 (Redump entries 30555 and 55078).
                // 138,752 bytes corresponds to SafeDisc 1.50.020 (Redump entries 28810 and 62935).

                default:
                    return "1";
            }
        }

        internal static string GetSafeDiscDrvmgtVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The file "drvmgt.dll" has been found to be incredibly consistent between versions, with the vast majority of files based on hash corresponding 1:1 with the SafeDisc version used according to the EXE.
            // There are occasionaly inconsistencies, even within the well detected version range. This seems to me to mostly happen with later (3.20+) games, and seems to me to be an example of the SafeDisc distribution becoming more disorganized with time.
            // Particularly interesting inconsistencies will be noted below:
            // Redump entry 73786 has an EXE with a scrubbed version, a DIAG.exe with a version of 4.60.000, and a copy of drvmgt.dll belonging to version 3.10.020. This seems like an accidental(?) distribution of older drivers, as this game was released 3 years after the use of 3.10.020.
            var sha1 = GetFileSHA1(firstMatchedString);
            switch (sha1)
            {
                // Found in Redump entry 102979.
                case "B858CB282617FB0956D960215C8E84D1CCF909C6":
                    return "(Empty File)";
                // Found in Redump entries 29073 and 31149.
                case "33434590D7DE4EEE2C35FCC98B0BF141F422B26D":
                    return "1.06.000";
                // Found in Redump entries 9718 and 46756.
                case "D5E4C99CDCA8091EC8010FCB96C5534A8BE35B43":
                    return "1.07.000";
                // Found in Redump entries 12885 and 66210.
                case "412067F80F6B644EDFB25932EA34A7E92AD4FC21":
                    return "1.09.000";
                // Found in Redump entries 37523 and 66586.
                case "87C0DA1B52681FA8052A915E85699738993BEA72":
                    return "1.11.000";
                // Found in Redump entries 21154 and 37982. 
                case "3569FE747311265FDC83CBDF13361B4E06484725":
                    return "1.20.000";
                // Found in Redump entry 37920.
                case "89795A34A2CAD4602713524365F710948F7367D0":
                    return "1.20.001";
                // Found in Redump entries 31526 and 55080.
                case "D31725FF99BE44BC1BFFF171F4C4705F786B8E91":
                    return "1.30.010";
                // Found in Redump entries 9617 and 49552.
                case "2A86168FE8EFDFC31ABCC7C5D55A1A693F2483CD":
                    return "1.35.000";
                // Found in Redump entries 2595 and 30121.
                case "8E41DB1C60BBAC631B06AD4F94ADB4214A0E65DC":
                    return "1.40.004";
                // Found in Redump entries 44350 and 63323.
                case "833EA803FB5B0FA22C9CF4DD840F0B7DE31D24FE":
                    return "1.41.000";
                // Found in Redump entries 37832 and 42091.
                case "1A9C8F6A5BD68F23CA0C8BCB890991AB214F14E0":
                    return "1.41.001";
                // Found in Redump entries 30555 and 55078.
                case "0BF4574813EA92FEE373481CA11DF220B6C4F61A":
                    return "1.45.011";
                // Found in Redump entries 28810 and 62935.
                case "812121D39D6DCC4947E177240655903DEC4DA15A":
                    return "1.50.020";
                // Found in Redump entries 72195 and 73502.
                case "04ED7AC39FE7A6FAB497A498CBCFF7DA19BF0556":
                    return "2.05.030";
                // Found in Redump entries 38541 and 59462 and 81096.
                case "0AB8330A33E188A29E8CE1EA9625AA5935D7E8CE":
                    return "2.10.030";
                // Found in Redump entries 55823 and 79476.
                case "5198DA51184CA9F3A8096C6136F645B454A85F6C":
                    return "2.30.030";
                // Found in Redump entries 15312 and 48863.
                case "414CAC2BE3D9BE73796D51A15076A5A323ABBF2C":
                    return "2.30.031";
                // Found in Redump entries 9819 and 53658. 
                case "126DCA2317DA291CBDE13A91B3FE47BA4719446A":
                    return "2.30.033";
                // Found in Redump entries 9846 and 65642.
                case "1437C8C149917C76F741C6DBEE6B6B0CC0664F13":
                    return "2.40.010";
                // Found in Redump entries 23786 and 37478.
                case "10FAD492991C251C9C9394A2B746C1BF48A18173":
                    return "2.40.011";
                // Found in Redump entries 30022 and 75014.
                case "94267BB97C418A6AA22C1354E38136F889EB0B6A":
                    return "2.51.020";
                // Found in Redump entries 31666 and 66852.
                case "27D5E7F7EEE1F22EBDAA903A9E58A7FDB50EF82C":
                    return "2.51.021";
                // Found in Redump entries 2064 and 47047.
                case "F346F4D0CAB4775041AD692A6A49C47D34D46571":
                    return "2.60.052";
                // Found in Redump entries 13048 and 35385.
                case "88C7AA6E91C9BA5F2023318048E3C3571088776F":
                    return "2.70.030";
                // Found in Redump entries 48101 and 64198.
                case "544EE77889092129E9818B5086E19197E5771C7F":
                    return "2.72.000";
                // Found in Redump entries 32783 and 72743.
                case "EA6E24B1F306391CD78A1E7C2F2C0C31828EF004":
                    return "2.80.010";
                // Found in Redump entries 39273 and 59351.
                case "1BF885FDEF8A1824C34C10E2729AD133F70E1516":
                    return "2.80.011";
                // Found in Redump entries 11638, 52606, and 62505.
                case "B824ED257946EEE93F438B25C855E9DDE7A3671A":
                    return "2.90.010-2.90.040";
                // Found in Redump entries 13230 and 68204.
                case "CDA56FD150C9E9A19D7AF484621352122431F029":
                    return "3.10.020";
                // Found in Redump entries 36511 and 74338.
                case "E5504C4C31561D38C1F626C851A8D06212EA13E0":
                    return "3.15.010";
                // Found in Redump entries 15383 and 35512. 
                case "AABA7B3EF08E80BC55282DA3C3B7AA598379D385":
                    return "3.15.011";
                // Found in Redump entries 58625, 75782, and 84586.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 3.20.020-3.20.024, including Redump entries 20729, 30404, and 56748.
                // TODO: Further investigate versions 3.20.020-3.20.024, and verify that 3.20.024 doesn't use drvmgt.dll at all.
                case "ECB341AB36C5B3B912F568D347368A6A2DEF8D5F":
                    return "3.20.020-3.20.022";
                // Found in Redump entries 53666, 76775, and 102301.
                case "69C776F67EBD53CB5FD760B498B4A491BF22F293":
                    return "3.20.022";
                // Found in Redump entry 102806.
                case "2BD7CD06CED6F6FB1A31AAE2D6C403C166366C6F":
                    return "3.20.022";
                // Found in Redump entries 15614, 79729, 83408, and 86196.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 4.00.001-4.00.003, including Redump entries 33326, 51597, and 67927.
                case "E21FF43C2E663264D6CB11FBBC31EB1DCEE42B1A":
                    return "4.00.000-4.00.003";
                // Found in Redump entry 49677.
                case "7C5AB9BDF965B70E60B99086519327168F43F362":
                    return "4.00.002";
                // Found in Redump entries 46765 and 78980.
                case "A5247EC0EC50B8F470C93BF23E3F2514C402D5AD":
                    return "4.00.002+";
                // Found in Redump entries 74564 and 80776.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 4.50.000, including Redump entries 58990 and 65569.
                case "C658E0B4992903D5E8DD9B235C25CB07EE5BFEEB":
                    return "4.50.000";
                // Found in Redump entry 20092.
                case "02F373C1D246DBEFBFD5F39B6D0E40E2964B0027":
                    return "4.60.000";
                // Found in Redump entry 56320.
                case "84480ABCE4676EEB9C43DFF7C5C49F0D574FAC25":
                    return "4.70.000";
                // Found distributed in https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe and https://web.archive.org/web/20010707163339/http://www.macrovision.com:80/demos/safedisc.exe, but unknown what version it is associated with.
                case "8426690FA43076EE466FD1B2D1F2F1267F9CC3EC":
                    return "Unknown Version (Report this to us on GitHub)";
                default:
                    return "Unknown Version (Report this to us on GitHub)";

                    // File size of drvmgt.dll and others is a commonly used indicator of SafeDisc version, though it has been found to not be completely consistent, and is completely replaced by hash checks.
                    // 34,816 bytes corresponds to SafeDisc 1.0x
                    // 32,256 bytes corresponds to SafeDisc 1.1x-1.3x
                    // 31,744 bytes corresponds to SafeDisc 1.4x
                    // 34,304 bytes corresponds to SafeDisc 1.5x-2.40
                    // 35,840 bytes corresponds to SafeDisc 2.51-2.60
                    // 40,960 bytes corresponds to SafeDisc 2.70
                    // 23,552 bytes corresponds to SafeDisc 2.80
                    // 41,472 bytes corresponds to SafeDisc 2.90-3.10
                    // 24,064 bytes corresponds to SafeDisc 3.15-3.20;
            }
        }

#if NET48
        private string GetVersionFromSHA1Hash(string sha1Hash)
#else
        private string? GetVersionFromSHA1Hash(string sha1Hash)
#endif
        {
            switch (sha1Hash.ToLowerInvariant())
            {
                // dplayerx.dll
                case "f7a57f83bdc29040e20fd37cd0c6d7e6b2984180":
                    return "1.00.030";
                case "a8ed1613d47d1b5064300ff070484528ebb20a3b":
                    return "1.11.000";
                case "ed680e9a13f593e7a80a69ee1035d956ab62212b":
                    return "1.3x";
                case "66d8589343e00fa3e11bbf462e38c6f502515bea":
                    return "1.30.010";
                case "5751ae2ee805d31227cfe7680f3c8be4ab8945a3":
                    return "1.40";

                // secdrv.sys
                case "b64ad3ec82f2eb9fb854512cb59c25a771322181":
                    return "1.11.000";
                case "ebf69b0a96adfc903b7e486708474dc864cc0c7c":
                    return "1.40.004";
                case "f68a1370660f8b94f896bbba8dc6e47644d19092":
                    return "2.30";
                case "60bc8c3222081bf76466c521474d63714afd43cd":
                    return "2.40";
                case "08ceca66432278d8c4e0f448436b77583c3c61c8":
                    return "2.50";
                case "10080eb46bf76ac9cf9ea74372cfa4313727f0ca":
                    return "2.51";
                case "832d359a6de191c788b0e61e33f3d01f8d793d3c":
                    return "2.70";
                case "afcfaac945a5b47712719a5e6a7eb69e36a5a6e0":
                case "cb24fbe8aa23a49e95f3c83fb15123ffb01f43f4":
                    return "2.80";
                case "0383b69f98d0a9c0383c8130d52d6b431c79ac48":
                    return "2.90";
                case "d7c9213cc78ff57f2f655b050c4d5ac065661aa9":
                    return "3.20";
                case "fc6fedacc21a7244975b8f410ff8673285374cc2":
                    return "4.00.002"; // Also 4.60.000, might be a fluke
                case "2d9f54f35f5bacb8959ef3affdc3e4209a4629cb":
                    return "1-4";

                default:
                    return null;
            }
        }

        private string GetSafeDiscDiagExecutableVersion(PortableExecutable pex)
        {
            // Different versions of this executable correspond to different SafeDisc versions.
            var version = pex.FileVersion;
            if (!string.IsNullOrEmpty(version))
            {
                switch (version)
                {
                    // Found to be in Redump entry 65569.
                    // The product version is "4.50.00.1619 2005/06/08".
                    case "4.50.00.1619":
                        return "4.50.0.1619 / SafeDisc 4.50.000";
                    // Found to be in Redump entry 20092.
                    // The product version is "4.60.00.1702 2005/08/03".
                    case "4.60.00.1702":
                        return "4.60.0.1702 / SafeDisc 4.60.000";
                    // Found to be in Redump entry 34783.
                    // The product version is "4.70.00.1941 2006/04/26".
                    case "4.70.00.1941":
                        return "4.70.0.1941 / SafeDisc 4.70.000";
                    default:
                        return $"Unknown Version {version} (Report this to us on GitHub)";
                }
            }

            return "Unknown Version (Report this to us on GitHub)";
        }
    }
}
