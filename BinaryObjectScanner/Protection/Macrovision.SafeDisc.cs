using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

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
    /// It doesn't typically require the disc after installation (unless game files are left on the disc in a minimal installation).
    /// However, according to the white paper, Mac products can still choose to authenticate on each run. 
    /// According to the same white paper, it only uses some protection methods of normal SafeDisc without specifying which.
    /// CDs still contain errors, though typically less (in the ballpark of ~100), and the white paper admits that DVDs with Lite are less secure due to not having this signature.
    /// It's currently unknown if Lite actually uses an active disc check, or if it just relies on the errors to prevent copying of the disc.
    /// SafeDisc Lite games are able to be installed and played in situations where bad sectors are not emulated (such as 86box v.4.2.1), meaning it likely doesn't actually check the signature.
    /// 
    /// Other protections in the Macrovision "Safe-" family of protections that need further investigation:
    /// SafeScan (https://cdn.loc.gov/copyright/1201/2003/reply/029.pdf).
    /// SafeDisc HD (https://web.archive.org/web/20000129100449/http://www.macrovision.com/scp_hd.html).
    /// SafeAuthenticate (https://web.archive.org/web/20041020010136/http://www.ttrtech.com/pdf/SafeAudioFAQ.pdf)
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IExecutableCheck{T}.CheckExecutable(string, T, bool)"/>
        internal static string? SafeDiscCheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Found in Redump entry 57986.
            if (pex.Model.ImportTable?.HintNameTable != null)
            {
                if (Array.Exists(pex.Model.ImportTable.HintNameTable, ihne => ihne?.Name == "LTDLL_Authenticate"))
                    return "SafeDisc Lite";
            }

            // Found in Redump entry 57986.
            if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            {
                if (Array.Exists(pex.Model.ImportTable.ImportDirectoryTable, idte => idte?.Name == "ltdll.dll"))
                    return "SafeDisc Lite";
            }

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in Redump entries 14928, 25579, 32751.
                if (strs.Exists(s => s.Contains("LTDLL_Initialise")))
                    return "SafeDisc Lite";
                if (strs.Exists(s => s.Contains("LTDLL_Authenticate")))
                    return "SafeDisc Lite";
                if (strs.Exists(s => s.Contains("LTDLL_Unwrap")))
                    return "SafeDisc Lite";

                // Present in "Setup.exe" from the earlier "safedisc.exe" driver update provided by Macrovision.
                if (strs.Exists(s => s.Contains("Failed to get the DRVMGT.DLL Setup API address")))
                    return "Macrovision SecDrv Update Installer";
            }

            var name = pex.FileDescription;
            // Present in "Diag.exe" files from SafeDisc 4.50.000+.
            if (name.OptionalEquals("SafeDisc SRV Tool APP", StringComparison.OrdinalIgnoreCase))
                return $"SafeDisc SRV Tool APP {GetSafeDiscDiagExecutableVersion(pex)}";

            // Present in "Setup.exe" from the later "safedisc.exe" driver update provided by Macrovision.
            if (name.OptionalEquals("Macrovision SecDrv Update", StringComparison.OrdinalIgnoreCase))
                return "Macrovision SecDrv Update Installer";

            // Present on all "CLOKSPL.DLL" versions before SafeDisc 1.06.000. Found on Redump entries 61731 and 66004. 
            name = pex.ProductName;
            if (name.OptionalEquals("SafeDisc CDROM Protection System", StringComparison.OrdinalIgnoreCase))
                return "SafeDisc 1.00.025-1.01.044";

            // Present in "Diag.exe" files from SafeDisc 4.50.000+.
            else if (name.OptionalEquals("SafeDisc SRV Tool APP", StringComparison.OrdinalIgnoreCase))
                return $"SafeDisc SRV Tool APP {GetSafeDiscDiagExecutableVersion(pex)}";

            // Present in "Setup.exe" from the later "safedisc.exe" driver update provided by Macrovision.
            if (name.OptionalEquals("Macrovision SecDrv Update", StringComparison.OrdinalIgnoreCase))
                return "Macrovision SecDrv Update Installer";

            // Present in "AuthServ.exe" files from SafeDisc 4+.
            // This filename is confirmed by the file properties in SafeDisc 4+ (such as Redump entry 35382).
            // It is only found extracted into the Windows Temp directory when a protected application is run, and is renamed to begin with a "~" and have the ".tmp" extension.
            else if (name.OptionalEquals("SafeDisc AuthServ APP", StringComparison.OrdinalIgnoreCase))
                return $"SafeDisc AuthServ APP {GetSafeDiscAuthServVersion(pex)}";

            // Present on all "CLOKSPL.EXE" versions before SafeDisc 1.06.000. Found on Redump entries 61731 and 66004. 
            // Only found so far on SafeDisc 1.00.025-1.01.044, but the report is currently left generic due to the generic nature of the check.
            name = pex.FileDescription;
            if (name.OptionalEquals("SafeDisc", StringComparison.OrdinalIgnoreCase))
                return "SafeDisc";

            try
            {
                // Found in Redump entries 20729 and 65569.
                // Get the debug data
                if (pex.FindCodeViewDebugTableByPath("SafeDisc").Count > 0)
                    return "SafeDisc";
                if (pex.FindCodeViewDebugTableByPath("Safedisk").Count > 0)
                    return "SafeDisc";
            }
            catch
            {
                // Absorb inconsistent debug data errors
            }

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

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, List{string})"/>
        internal static List<string> SafeDiscCheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(
                [
                    new FilePathMatch("CLCD16.DLL"),
                    new FilePathMatch("CLCD32.DLL"),
                    new FilePathMatch("CLOKSPL.EXE"),
                    new PathMatch(".icd", useEndsWith: true),
                ], "SafeDisc 1/Lite"),

                // Check for the original filename used for the SafeDisc splash-screens, new file names are used in later versions.
                new(
                [
                    new FilePathMatch("00000001.TMP"),
                    new FilePathMatch("SPLSH16.BMP"),
                    new FilePathMatch("SPLSH256.BMP"),
                ], "SafeDisc 1.00.025-1.01.044"),

                new(
                [
                    new FilePathMatch("00000001.TMP"),
                    // The .016 and .256 files are banners stored in the BMP image format. The 016 and 256 refers to the color depth of the BMP.
                    // There are common file names used, such as 00000407.XXX and 00000409.XXX. Further investigation is needed to determine the consistency of these names.
                    new PathMatch(".016", useEndsWith: true),
                    new PathMatch(".256", useEndsWith: true),
                ], "SafeDisc 1.06.000-3.20.024"),

                new(
                [
                    new FilePathMatch("00000001.TMP"),
                    // The .016 files stop being used as of 4.00.000, while the .256 remain in fairly consistent use.
                    new PathMatch(".256", useEndsWith: true),
                ], "SafeDisc 1.06.000+"),

                // The file "mcp.dll" is known to only be used in a specific version range for SafeDisc, but is currently only used in a grouped file name check with other SafeDisc files to prevent false positives.
                // Found in Redump entries 28810, 30555, 55078, and 62935.
                new(
                [
                    new FilePathMatch("00000001.TMP"),
                    new FilePathMatch("drvmgt.dll"),
                    new FilePathMatch("mcp.dll"),
                    new FilePathMatch("secdrv.sys"),
                ], "SafeDisc 1.45.011-1.50.020"),

                // Search for the splash screen files known to sometimes contain a generic SafeDisc splash-screen.
                new(new FilePathMatch("00000000.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000000.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("0000040c.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("0000040c.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000407.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000407.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000409.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000409.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000809.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000809.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00001009.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00001009.256"), GetSafeDiscSplshVersion, "SafeDisc"),

                // The file "00000001.TMP" is found in most, if not all, SafeDisc protected programs and is detected within the general Macrovision checks due to being used in other Macrovision DRM.

                // Found in many versions of SafeDisc, beginning in 2.05.030 and being used all the way until the final version 4.90.010. It is not always present, even in versions it has been used in. Found in Redump entries 56319 and 72195.
                new(new FilePathMatch("00000002.TMP"), "SafeDisc 2+"),

                new(new FilePathMatch("DPLAYERX.DLL"), GetSafeDiscDPlayerXVersion, "SafeDisc"),
                new(new FilePathMatch("drvmgt.dll"), GetSafeDiscDrvmgtVersion, "SafeDisc"),

                // The SD0XXX.dll files appear to solely contain custom strings that allowed the publisher to customize the SafeDisc error messages. They are currently only known to be used by EA.
                // Each file appears to contain strings for a specific language each.
                // There is one non EA game that makes reference to "SD0809.dll", but doesn't include it (IA item "rayman.-raving.-rabbids.-hebrew.-dvd").
                // TODO: Add a generic check to detect this type of string that appears to be present in all of these DLLs:
                // "d:\DiceCanada\BoosterPack2\Installers\BF2XpackInstaller\Safedisk\SafeDiscDLLs\DLLs\Release\SD0816.pdb" (Redump entry 65569).

                // Found in Redump entries 20729 and 65569.
                new(new FilePathMatch("SD040e.dll"), "SafeDisc"),

                // Found in Redump entry 65569.
                new(new FilePathMatch("SD0c0a.dll"), "SafeDisc"),
                new(new FilePathMatch("SD040b.dll"), "SafeDisc"),
                new(new FilePathMatch("SD040c.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0419.dll"), "SafeDisc"),
                new(new FilePathMatch("SD041d.dll"), "SafeDisc"),
                new(new FilePathMatch("SD041e.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0404.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0405.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0406.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0407.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0409.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0410.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0411.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0412.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0413.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0414.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0415.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0416.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0804.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0809.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0816.dll"), "SafeDisc"),

                // Used to distribute SafeDisc driver updates over the internet. Two distinct versions known to exist, with Microsoft also having distributed the later update as well.
                // Version 1: https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe
                // Version 2: https://web.archive.org/web/20051104123646/http://www.macrovision.com/products/safedisc/safedisc.exe
                // Microsoft download page: https://web.archive.org/web/20080204081329/http://www.microsoft.com/downloads/details.aspx?FamilyID=eae20f0f-c41c-44fe-84ce-1df707d7a2e9&DisplayLang=en
                new(new FilePathMatch("safedisc.exe"), "SafeDisc Driver Installer"),

                // Found in Redump entries 28810 and 30555.
                // Name check overmatches with a seemingly completely unrelated application, ironically included on at least one SafeDisc game (Redump entry 34828).
                // new(new FilePathMatch("mcp.dll"), "SafeDisc (Version 1.45.011-1.50.020)"),

                // Found in Redump entry 58455 and 65569.
                // Unknown if it's a game specific file, but it contains the stxt371 and stxt774 sections.
                // new(new FilePathMatch("CoreDLL.dll"), "SafeDisc"),

                // Found in seemingly every SafeDisc Lite disc. (CD: Redump entries 25579 and 57986. DVD: Redump entry 63941). 
                new(new FilePathMatch("00000001.LT1"), "SafeDisc Lite"),
                new(new FilePathMatch("LTDLL.DLL"), "SafeDisc Lite"),

                // Found in Redump entries 23983, 42762, 72713, 73070, and 89603.
                new(new FilePathMatch(".SafeDiscDVD.bundle"), "SafeDiscDVD for Macintosh"),
                new(new FilePathMatch("SafeDiscDVD"), "SafeDiscDVD for Macintosh"),

                // Found in Redump entries 42762 and 73070.
                // These files, along with "00000001.TMP" as found in the same version of SafeDiscDVD, appear to be likely encrypted game executables and are multiple GB in size.
                new(new FilePathMatch("00000001I.TMP"), "SafeDiscDVD for Macintosh"),
                new(new FilePathMatch("00000001P.TMP"), "SafeDiscDVD for Macintosh"),

                // Found in Redump entry 89649.
                new(new FilePathMatch("SafeDiscLT.bundle"), "SafeDiscLT for Macintosh"),
                new(new FilePathMatch("SafeDiscLT"), "SafeDiscLT for Macintosh"),

                // TODO: Add SafeDisc detection for Redump entry 63769 once Mac executables are supported for scanning.
                // It appears to contain the same "BoG_" string and version detection logic.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal static string? SafeDiscCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CLCD16.DLL"), GetSafeDiscCLCD16Version, "SafeDisc"),
                new(new FilePathMatch("CLCD32.DLL"), GetSafeDiscCLCD32Version, "SafeDisc"),
                new(new FilePathMatch("CLOKSPL.EXE"), GetSafeDiscCLOKSPLVersion, "SafeDisc"),

                // The file "00000001.TMP" is found in most, if not all, SafeDisc protected programs and is detected within the general Macrovision checks due to being used in other Macrovision DRM.

                // Found in many versions of SafeDisc, beginning in 2.05.030 and being used all the way until the final version 4.90.010. It is not always present, even in versions it has been used in. Found in Redump entries 56319 and 72195.
                new(new FilePathMatch("00000002.TMP"), "SafeDisc 2+"),

                // Search for the splash screen files known to sometimes contain a generic SafeDisc splash-screen.
                new(new FilePathMatch("00000000.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000000.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("0000040c.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("0000040c.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000407.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000407.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000409.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000409.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000809.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00000809.256"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00001009.016"), GetSafeDiscSplshVersion, "SafeDisc"),
                new(new FilePathMatch("00001009.256"), GetSafeDiscSplshVersion, "SafeDisc"),


                new(new FilePathMatch("DPLAYERX.DLL"), GetSafeDiscDPlayerXVersion, "SafeDisc"),
                new(new FilePathMatch("drvmgt.dll"), GetSafeDiscDrvmgtVersion, "SafeDisc"),

                // The SD0XXX.dll files appear to solely contain custom strings that allowed the publisher to customize the SafeDisc error messages. They are currently only known to be used by EA.
                // Each file appears to contain strings for a specific language each.
                // There is one non EA game that makes reference to "SD0809.dll", but doesn't include it (IA item "rayman.-raving.-rabbids.-hebrew.-dvd").
                // TODO: Add a generic check to detect this type of string that appears to be present in all of these DLLs:
                // "d:\DiceCanada\BoosterPack2\Installers\BF2XpackInstaller\Safedisk\SafeDiscDLLs\DLLs\Release\SD0816.pdb" (Redump entry 65569).

                // Found in Redump entries 20729 and 65569.
                new(new FilePathMatch("SD040e.dll"), "SafeDisc"),

                // Found in Redump entry 65569.
                new(new FilePathMatch("SD0c0a.dll"), "SafeDisc"),
                new(new FilePathMatch("SD040b.dll"), "SafeDisc"),
                new(new FilePathMatch("SD040c.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0419.dll"), "SafeDisc"),
                new(new FilePathMatch("SD041d.dll"), "SafeDisc"),
                new(new FilePathMatch("SD041e.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0404.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0405.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0406.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0407.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0409.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0410.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0411.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0412.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0413.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0414.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0415.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0416.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0804.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0809.dll"), "SafeDisc"),
                new(new FilePathMatch("SD0816.dll"), "SafeDisc"),

                // Found in Redump entries 28810 and 30555.
                // Name check overmatches with a seemingly completely unrelated application, ironically included on at least one SafeDisc game (Redump entry 34828).
                // new(new FilePathMatch("mcp.dll"), "SafeDisc (Version 1.45.011-1.50.020)"),

                // Found in Redump entry 58455 and 65569.
                // Unknown if it's a game specific file, but it contains the stxt371 and stxt774 sections.
                // new(new FilePathMatch("CoreDLL.dll"), "SafeDisc"),

                // Found in Redump entry 58990.
                new(new FilePathMatch("SafediskSplash.bmp"), "SafeDisc"),
                
                // Used to distribute SafeDisc driver updates over the internet. Two distinct versions known to exist, with Microsoft also having distributed the later update as well.
                // Version 1: https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe
                // Version 2: https://web.archive.org/web/20051104123646/http://www.macrovision.com/products/safedisc/safedisc.exe
                // Microsoft download page: https://web.archive.org/web/20080204081329/http://www.microsoft.com/downloads/details.aspx?FamilyID=eae20f0f-c41c-44fe-84ce-1df707d7a2e9&DisplayLang=en
                new(new FilePathMatch("safedisc.exe"), "SafeDisc Driver Installer"),

                // Found in seemingly every SafeDisc Lite disc. (CD: Redump entries 25579 and 57986. DVD: Redump entry 63941). 
                new(new FilePathMatch("00000001.LT1"), "SafeDisc Lite"),
                new(new FilePathMatch("LTDLL.DLL"), "SafeDisc Lite"),

                // Found in Redump entries 23983, 42762, 72713, 73070, and 89603.
                new(new FilePathMatch(".SafeDiscDVD.bundle"), "SafeDiscDVD for Macintosh"),
                new(new FilePathMatch("SafeDiscDVD"), "SafeDiscDVD for Macintosh"),

                // Found in Redump entries 42762 and 73070.
                // These files, along with "00000001.TMP" as found in the same version of SafeDiscDVD, appear to be likely encrypted game executables and are multiple GB in size.
                new(new FilePathMatch("00000001I.TMP"), "SafeDiscDVD for Macintosh"),
                new(new FilePathMatch("00000001P.TMP"), "SafeDiscDVD for Macintosh"),

                // Found in Redump entry 89649.
                new(new FilePathMatch("SafeDiscLT.bundle"), "SafeDiscLT for Macintosh"),
                new(new FilePathMatch("SafeDiscLT"), "SafeDiscLT for Macintosh"),

                // TODO: Add SafeDisc detection for Redump entry 63769 once Mac executables are supported for scanning.
                // It appears to contain the same "BoG_" string and version detection logic.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private static string GetSafeDiscAuthServVersion(PortableExecutable pex)
        {
            // Different versions of this executable correspond to different SafeDisc versions.
            var version = pex.FileVersion;
            if (!string.IsNullOrEmpty(version))
            {
                return version switch
                {
                    // Found in Redump entries 35382, 36024, 74520, and 79729.
                    // The product version is "4.00.00.0092 2004/09/02".
                    "4.00.00.0092" => "4.00.00.0092 / SafeDisc 4.00.000",

                    // Found in Redump entries 8842-8844, 15614, 38143, 67927, 70504, 74390-74391, and 83017.
                    // The product version is "4.00.01.0004 2004/09/30".
                    "4.00.01.0004" => "4.00.01.0004 / SafeDisc 4.00.001",

                    // Found in Redump entries 33326, 42034, 71646, 78980, 85345-85347, 86196, and 105716.
                    // The product version is "4.00.02.0000 2004/12/15".
                    "4.00.02.0000" => "4.00.02.0000 / SafeDisc 4.00.002",

                    // Found in Redump entries 40595-40597, 51597, 68551-68552, 83408, and 83410.
                    // The product version is "4.00.03.0000 2005/05/11".
                    "4.00.03.0000" => "4.00.03.0000 / SafeDisc 4.00.003",

                    // Found in Redump entries 58073-58074, 58455-58459, 58990-58992, 65569, 74206, 74564 + 74579-74581, 76813, 77440,
                    //   80776-80777, 85384, and 101261.
                    // The product version is "4.50.00.1619 2005/06/08".
                    "4.50.00.1619" => "4.50.00.1619 / SafeDisc 4.50.000",

                    // Found in Redump entries 20092, 31824, 45407-45409, 45469, 45684-45686, 46764-46769, 50682, 57721, 73786, 85859, and 104503.
                    // The product version is "4.60.00.1702 2005/08/03".
                    "4.60.00.1702" => "4.60.00.1702 / SafeDisc 4.60.000",

                    // Found in Redump entries 34783, 56320-56323, and 66403.
                    // The product version is "4.70.00.1941 2006/04/26".
                    "4.70.00.1941" => "4.70.00.1941 / SafeDisc 4.70.000",

                    // Found in Redump entries 64144-64146 + 78543, and 98589-98590.
                    // The product version is "4.80.00.2074 2006/09/06".
                    "4.80.00.2074" => "4.80.00.2074 / SafeDisc 4.80.000",

                    // Found in Redump entries 13014, 52523, 74366, 76346, 83290, 115764, and 116381.
                    // The product version is "4.81.00.2284 2007/04/04".
                    "4.81.00.2284" => "4.81.00.2284 / SafeDisc 4.81.000",

                    // Found in Redump entries 65417
                    // The product version is "4.85.00.2422 2007/08/20".
                    "4.85.00.2422" => "4.85.00.2422 / SafeDisc 4.85.000",

                    // Found in Redump entries 20434, 31766, and 79113.
                    // The product version is "4.85.00.2489 2007/10/26".
                    "4.85.00.2489" => "4.85.00.2489 / SafeDisc 4.85.000",

                    // Found in Redump entries 56319, and 66333.
                    // The product version is "4.90.00.2613 2008/02/27".
                    "4.90.00.2613" => "4.90.00.2613 / SafeDisc 4.90.000",

                    // Found in Redump entry 38142.
                    // The product version is "4.90.10.2747 2008/07/10".
                    "4.90.10.2747" => "4.90.10.2747 / SafeDisc 4.90.000",

                    // Found in Redump entries 11347, 29069, 58573-58575, 78976, and 120303.
                    // The product version is "4.90.10.2781 2008/08/13".
                    "4.90.10.2781" => "4.90.10.2781 / SafeDisc 4.90.010",

                    // Found in Redump entry 120213.
                    // The product version is "4.91.00.2832 2008/10/03".
                    "4.91.00.2832" => "4.91.00.2832 / SafeDisc 4.91.000",

                    _ => $"Unknown Version {version} (Report this to us on GitHub)",
                };
            }

            return "Unknown Version (Report this to us on GitHub)";
        }

        internal static string GetSafeDiscCLCD16Version(string firstMatchedString, IEnumerable<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The hash of the file CLCD16.dll is able to provide a broad version range that appears to be consistent, but it seems it was rarely updated so these checks are quite broad.
            var sha1 = HashTool.GetFileHash(firstMatchedString, HashType.SHA1);
            return sha1?.ToUpperInvariant() switch
            {
                // Found in Redump entries 61731 and 66005.
                "C13493AB753891B8BEE9E4E014896B026C01AC92" => "1.00.025-1.01.044",

                // Found in Redump entries 1882 and 30049. 
                // It is currently unknown why the previous hash covers both the version before this one, and most afterwards, with this one being a consistent outlier between these versions.
                "2418D791C7B9D4F05BCB01FAF98F770CDF798464" => "1.00.026",

                // Found in Redump entries 31149 and 28810.
                // For SafeDisc 1 programs, it can be found bundled together with the rest of the drivers and protected application files.
                // For SafeDisc 2+ programs, it can be found in the Windows Temp directory when running protected programs on Windows 9x, but not on XP or newer.
                // Examples of it in SafeDisc 2+ can be found in Redump entries 2022 and 38541.
                // It can also be found in SafeDisc 4.85.000, as seen in Redump entry 20434.
                "848EDF9F45A8437438B7289BB4D2D1BCF752FD4A" => "1.06.000+/Lite",

                // The following versions can only be found in the Windows Temp directory when running protected programs on Windows 9x, but not on XP or newer.
                // It is unknown why there is such a large gap between updates, or why this file was updated at all, as the majority of programs at this point didn't tend to support 9x.

                // Found in Redump entries 115764 and 116381.
                "12491C7C3A6778A02511F2632F5CFBC535D4E47A" => "4.81.000",

                _ => "Unknown Version (Report this to us on GitHub)",
            };
        }

        internal static string GetSafeDiscCLCD32Version(string firstMatchedString, IEnumerable<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The hash of the file CLCD32.dll so far appears to be a solid indicator of version for versions it was used with. It appears to have been updated with every release, unlike its counterpart, CLCD16.dll.
            var sha1 = HashTool.GetFileHash(firstMatchedString, HashType.SHA1);
            return sha1?.ToUpperInvariant() switch
            {
                // Found in Redump entry 66005.
                "BAD49BA0DEA041E85EF1CABAA9F0ECD822CE1376" => "1.00.025",

                // Found in Redump entry 34828.
                "6137C7E789A329865649FCB8387B963FC8C763C6" => "1.00.026 (pre-10/1998)",

                // Found in Redump entries 1882 and 30049.
                "AFEFBBF1033EA65C366A1156E21566DB419CFD7B" => "1.00.026 (post-10/1998)",

                // Found in Redump entries 31575 and 41923.
                "6E54AC24C344E4A132D1B7A6A61B2EC824DE5092" => "1.00.030",

                // Found in Redump entries 1883 and 42114.
                "23DAA95DAF75732C27CEB133A00F7E10D1D482D3" => "1.00.032",

                // Found in Redump entries 36223 and 40771.
                "C8F609DDFC3E1CF69FADD60B7AED7A63B4B1DA62" => "1.00.035",

                // Found in Redump entries 42155 and 47574.
                "39CC3C053702D9F6EFF0DF6535E54F6C78CEA639" => "1.01.034",

                // Found in Redump entry 51459.
                "BC476F625A4A7A89AE50E2A4CD0F248D6CEB5A84" => "1.01.038",

                // Found in Redump entries 34562 and 63304.
                "1AB79AA78F706A1A24C02CE2B9398EC78249700B" => "1.01.043",

                // Found in Redump entries 61731 and 81619.
                "E2326F66EA9C2E5153EC619EEE424D83E2FD4CA4" => "1.01.044",

                // Found in Redump entries 29073 and 31149.
                "AEDE9939C4B62AC6DCCE3A771919B23A661247B3" => "1.06.000",

                // Found in Redump entries 9718 and 46756.
                "B5503E2222B3DA387BB5D7150A4A32A47824988F" => "1.07.000",

                // Found in Redump entries 12885 and 66210.
                // SafeDisc Lite found in Redump entries 32751.
                "7D33EA7B241245182FFB7A392873079B6183050B" => "1.09.000/Lite",

                // Found in Redump entries 37523 and 66586.
                "61A4A5A758A5CFFB226CE2AE96E55A40AB073AC6" => "1.11.000",

                // Found in Redump entries 21154 and 37982.
                "14D3267C1D5C925F6DA44F1B19CB14F6DFCA73E3" => "1.20.000",

                // Found in Redump entry 37920.
                "CB4570F3F37E0FA70D7B9F3211FDC2105864C664" => "1.20.001",

                // Found in Redump entries 31526 and 55080.
                "1B5FD2D3DFBD89574D602DA9AE317C55F24902F0" => "1.30.010",

                // Found in Redump entries 9617 and 49552.
                "CC73C219BFC2D729515D25CA1B93D53672153175" => "1.35.000",

                // Found in Redump entries 2595 and 30121.
                "5825FF56B50114CD5D82BD4667D7097B29973197" => "1.40.004",

                // Found in Redump entries 44350 and 63323.
                "38DE3C6CF8FA89E5E99C359AA8ABFC65ADE396A5" => "1.41.000",

                // Found in Redump entries 37832 and 42091.
                "894D38AD949576928F67FF1595DC9C877A34A91C" => "1.41.001",

                // Found in Redump entries 30555 and 55078.
                "0235E03CA78232417C93FBB5F56B1BE819926B0C" => "1.45.011",

                // Found in Redump entries 28810 and 62935.
                "331B777A0BA2A358982575EA3FAA2B59ECAEA404" => "1.50.020",

                // Found in Redump entries 57986 and 63941.
                "85A92DC1D9CCBA6349D70F489043E649A8C21F2B" => "Lite",

                // The following versions of the file are only found in the Windows Temp directory when running a SafeDisc 2+ program on Windows 9x.
                // They aren't found when running the same program on Windows 2k or newer.
                // These also aren't currently automatically extracted, and would have to be manually recovered and scanned.

                // Found in Redump entries 2022, 72195, and 73502.
                "3F46BA4BB6D0D725F8BC5BFD374025853D0F8D10" => "2.05.030",

                // Found in Redump entries 38541 and 59462.
                "3AF6AD2EBA63FC96BF1A2E39725C41A022B14550" => "2.10.030",

                // Found in Redump entries 45040, 55823, and 79476.
                "AAB277C3877F654A0EAEC1E0DB1D511CD0D7BA00" => "2.30.030",

                // Found in Redump entries 15312 and 48853.
                "A73A129E50FB872F3AE0BA974A2EC46281556F8C" => "2.30.031",

                // Found in Redump entries 9819 and 53658/53659.
                "1499FC17B7565FC4B47F029412928FCA076D1838" => "2.30.033",

                // Found in Redump entries 9846, 65642, and 68206.
                // Found in SafeDisc Lite in Redump entry 99126.
                "FF4DF7AE5252EF38A69F165A6A180F51DCCA0438" => "2.40.010/Lite",

                // Found in Redump entries 23786 and 110603.
                "0D52948CDC6562EEBB5D078C9C0C7E9D1EDB00CE" => "2.40.011",

                // Found in Redump entries 30022 and 75104.
                "30F5C179AF876292C45463FAE287E109C57B265E" => "2.51.020",

                // Found in Redump entries 38589 and 66852.
                "728D2D788A757341A37E64DE85204EE1096FD509" => "2.51.021",

                // Found in Redump entries 2064, 47047, and 57673.
                "5F4EDEA0B29AA3B6B374EC2C91C5EB3C1D791456" => "2.60.052",

                // Found in Redump entries 13048-13050, 35385, and 46339.
                "6328E7C065E5FB5CD1FB3FE7C47D8B1EA6BF040E" => "2.70.030",

                // Found in Redump entries 9261/9262 and 64198.
                "DB195BF5C6E732CFDA7DC391C0DF1A52D5898140" => "2.72.000",

                // Found in Redump entries 14928, 32783, 75897.
                "538351FF5955A3D8438E8C278E9D6D6274CF13AB" => "2.80.010/Lite",

                // Found in Redump entries 39273/39274 and 59351.
                "51C816A76C831B6EA2B66EEBACFB7032FF813ECC" => "2.80.011",

                // Found in Redump entries 11638/11639, 52606, 62505, 85338/85339, 95322/95324, 119414, and 119415.
                "6492B6164D40633C7AAAC882EF1BA55E6931DBDC" => "2.90.040",

                // Found in Redump entries 116357 and 121411.
                "CC1818B15AD1D0510602D556AB0AFFB8011ECF4F" => "2.90.045",

                // Found in Redump entries 13230 and 68204.
                "E481642064018AD02CE1FA524E539C89B80B8116" => "3.10.020",

                // Found in Redump entries 36511 and 74338.
                "6950E54EFAE8A00D2F54BEAAE34FBE13C9555BB8" => "3.15.010",

                // Found in Redump entries 15383 and 35512.
                "86EBAD43D87C2192FAF457BE922E21963FE8A16C" => "3.15.011",

                // Found in Redump entries 30404, 31621/31623, 56748, 58625, and 64355-64358. TODO: Test 84586.
                "553BA02CCAE2298C6E14F695EA172EB2B47E6798" => "3.20.020",

                // Found in Redump entries 20728, 53667/53668/76775, 58625, 64255, 75782, 84985, 91552, 102135, and 102806.
                "CCC4797FDC387FB5E08F87C1830F43F9B7A28726" => "3.20.022",

                // Found in Redump entries 20729, 28257, 54268-5427, 63810-63813, and 86177.
                "E931EEC20B4A7032BDAD5DC1D76E740A08A6321B" => "3.20.024",

                // Found in Redump entries 35382, 36024, 74520, and 79729.
                "AF437372045AF7D5F74A876581FE2E76D2CEC80A" => "4.00.000",

                // Found in Redump entries 8842-8844, 38143, 67927, 74390-74391, 83017, 15614.
                "CF1BF960995040AB7DA103F95E7C0A2B69DA094C" => "4.00.001",

                // Found in Redump entries 33326, 42034, 71646, 78980, 85345-85347, 86196, and 105716.
                "BD373AE0A919349A5C3270C74AD990E11A836C60" => "4.00.002",

                // Found in Redump entries 40595-40597, 51597, 68551-68552, 83408, and 83410.
                "47A729C462186615DA2B8C6038535B884E7D10BC" => "4.00.003",

                // After this point, games that support 9x are inconsistent, but hashes for all known versions have manged to be acquired.

                // Found in Redump entries 74564 + 74579-74581, 76813, and 101261.
                "FD6A99FEF6AA551A71F4BD683E0334E92CFA546F" => "4.50.000",

                // Found in Redump entries 31824, 45684-45686, 50682, and 104503.
                "86923EE2618814ABDA285C2EB50EA26635479C7A" => "4.60.000",

                // Found in Redump entries 56320-56323.
                "5E26D831981841B4D36EF0B4A195CD073C513544" => "4.70.000",

                // Found in Redump entries 64144-64146 + 78543.
                "1AF42A52234EF989E099C0EB05906A939C7B98EA" => "4.80.000",

                // Found in Redump entries 115764 and 116381.
                "844D1876BD92DEBBA9B529DC5EE9B22CC3F317C2" => "4.81.000",

                // Found in Redump entry 20434.
                "48CAA84CEACFDCB6CEE8C85701A5A85EDC83F0A9" => "4.85.000",

                // Found in Redump entry 56319.
                "98508487638694450B0361B53C1159745A767D72" => "4.90.000",

                // Found in Redump entry 120303.
                "E16551A94B43358401368787E21840AE23137BE7" => "4.90.010",

                // Found in Redump entry 120213.
                "C2F6A1A558946053171037C2A640F3ECEE017FA0" => "4.91.000",

                _ => "Unknown Version (Report this to us on GitHub)",
            };
        }

        internal static string GetSafeDiscCLOKSPLVersion(string firstMatchedString, IEnumerable<string>? files)
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
            var sha1 = HashTool.GetFileHash(firstMatchedString, HashType.SHA1);
            return sha1?.ToUpperInvariant() switch
            {
                // Found in Redump entry 66005.
                "DD131A7B988065764E2A0F20B66C89049B20A7DE" => "1.00.025",

                // Found in Redump entry 34828.
                "41C8699A6E0F046EB7A21984441B555237DA4758" => "1.00.026 (pre-10/1998)",

                // Found in Redump entries 1882 and 30049.
                "D1C19C26DEC7C33825FFC59AD02B0EBA782643FA" => "1.00.026 (post-10/1998)",

                // Found in Redump entries 31575 and 41923.
                "B7C6C61688B354AB5D4E20CDEB36C992F203289B" => "1.00.030",

                // Found in Redump entries 1883 and 42114.
                "7445CD9FB49C322D18E92CC457DD880967C2B010" => "1.00.032",

                // Found in Redump entries 36223 and 40771.
                "50D4466F55BEDB3FE0E262235A6BAC751CA26599" => "1.00.035",

                // Found in Redump entries 42155 and 47574.
                "8C2F792326856C6D326707F76823FC7430AC86D5" => "1.01.034",

                // Found in Redump entry 51459.
                "107BF8077255FD4CA0875FB7C306F0B427E66800" => "1.01.038",

                // Found in Redump entries 34562 and 63304.
                "E8F4BA30376FCDAE00E7B88312300172674ABFA9" => "1.01.043",

                // Found in Redump entries 61731 and 81619.
                "CAB911C5CFC0A13C822DBFE0F0E1570C09F211FB" => "1.01.044",

                // Found in Redump entries 29073 and 31149.
                "43C1318B38742E05E7C858A02D64EEA13D9DFB9B" => "1.06.000",

                // Found in Redump entries 9718 and 46756.
                "451BD4C60AB826C16840815996A5DF03672666A8" => "1.07.000",

                // Found in Redump entries 12885 and 66210.
                "6C02A20A521112777D4843B8ACD9278F34314A35" => "1.09.000",

                // Found in Redump entries 37523 and 66586.
                "0548F1B12F60395C9394DDB7BED5E3E65E09D72E" => "1.11.000",

                // Found in Redump entries 21154 and 37982.
                "64A406FE640F2AC86A0E23F619F6EBE63BFFB8A1" => "1.20.000",

                // Found in Redump entry 37920.
                "8E874C9AF4CE5A9F1CBE96FCC761AA1C201C6938" => "1.20.001",

                // Found in Redump entries 31526 and 55080.
                "766EC536A10E68513138D1183705F5F19B9B8091" => "1.30.010",

                // Found in Redump entries 9617 and 49552.
                "1F1460FD66DD518159CCCDC99C12252EA0B2EEC4" => "1.35.000",

                // Found in Redump entries 2595 and 30121.
                "B1CF007BA36BA1B207DE334635F7BCEC146F8E35" => "1.40.004",

                // Found in Redump entries 44350 and 63323.
                "90F92A6DB15387F4C7619C442493791EBFC1041B" => "1.41.000",

                // Found in Redump entries 37832 and 42091.
                "836D42BF7B7AD719AB67682CF8D6B2D9C07AD218" => "1.41.001",

                // Found in Redump entries 30555 and 55078.
                "24DE959BC4484CD95DAA26947670C63A161E64AE" => "1.45.011",

                // Found in Redump entries 28810 and 62935.
                "9758F0637184816D02049A53CD2653F0BFFE92C9" => "1.50.020",

                _ => "Unknown Version (Report this to us on GitHub)",
            };
        }

        internal static string GetSafeDiscDPlayerXVersion(string firstMatchedString, IEnumerable<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            var fi = new FileInfo(firstMatchedString);
            return fi.Length switch
            {
                // File size of "dplayerx.dll" and others is a commonly used indicator of SafeDisc version, though it has been found to not be completely consistent.
                // Checks for versions 1.2X have been commented out, due to these versions already being detected via more accurate checks.
                // Examples of "dplayerx.dll" that are detected using these more accurate checks can be found in Redump entries 28810, 30121, and 37982. 

                // Found in Redump entry 34828.
                81_408 => "1.00.026 (pre-10/1998)",

                // Found in Redump entries 21154, 41923, 42114, and 66005.
                78_848 => "1.00.025-1.00.032",

                // Found in Redump entries 36223 and 40771.
                77_824 => "1.00.035",

                // Found in Redump entries 42155 and 47574. 
                115_712 => "1.01.034",

                // Found in Redump entry 42155.
                116_736 => "1.01.038",

                // Found in Redump entries 34562 and 63304.
                124_416 => "1.01.043",

                // Found in Redump entries 61731 and 81619.
                125_952 => "1.01.044",

                // Found in Redump entries 29073 and 31149.
                155_648 => "1.06.000",

                // Found in Redump entries 9718, 12885, and 37523.
                // SafeDisc Lite found in Redump entries 32751.
                156_160 => "1.07.000-1.11.000/Lite",

                // File size checks for versions 1.2X+ are superseded by executable string checks, which are more accurate.
                // For reference, the previously used file sizes are kept below as comments:

                /*
                    157,184 bytes corresponds to SafeDisc 1.20.000-1.20.001 (Redump entries 21154 and 37920).
                    163,382 bytes corresponds to SafeDisc 1.30.010 (Redump entries 31526 and 55080).
                    165,888 bytes corresponds to SafeDisc 1.35.000 (Redump entries 9617 and 49552).
                    172,544 bytes corresponds to SafeDisc 1.40.004 (Redump entries 2595 and 30121).
                    173,568 bytes corresponds to SafeDisc 1.41.000-1.41.001 (Redump entries 37832, and 44350). 
                    136,704 bytes corresponds to SafeDisc 1.45.011 (Redump entries 30555 and 55078).
                    138,752 bytes corresponds to SafeDisc 1.50.020 (Redump entries 28810 and 62935).
                */

                _ => "1",

                // Hashes have not been found to be a reliable indicator for these files, and likely differ on a game-to-game basis.
                // Some hashes were previously collected and are collected below:

                // Found in Redump entry 41923.
                // F7A57F83BDC29040E20FD37CD0C6D7E6B2984180" => "1.00.030",

                // Found in Redump entries 3569 and 3570.
                // "A8ED1613D47D1B5064300FF070484528EBB20A3B" => "1.11.000",

                // It is not known which games these files are from.
                // "ED680E9A13F593E7A80A69EE1035D956AB62212B" => "1.3x",
                // "66D8589343E00FA3E11BBF462E38C6F502515BEA" => "1.30.010",
                // "5751AE2EE805D31227CFE7680F3C8BE4AB8945A3" => "1.40",
            };
        }

        internal static string GetSafeDiscDrvmgtVersion(string firstMatchedString, IEnumerable<string>? files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The file "drvmgt.dll" has been found to be incredibly consistent between versions,
            //   with the vast majority of files based on hash corresponding 1:1 with the SafeDisc version used according to the EXE.
            // There are occasionally inconsistencies, even within the well detected version range.
            // This seems to me to mostly happen with later (3.20+) games, and may be an example of the SafeDisc distribution becoming more disorganized with time.
            // Particularly interesting inconsistencies will be noted below:

            // Redump entry 73786 has an EXE with a scrubbed version, a DIAG.exe associated with SD 4.60.000, a copy of drvmgt.dll belonging to 3.10.020,
            //   and a copy of secdrv.sys belonging to version 3.10.020-3.15.011.
            // This may be an accidental distribution of older drivers, as this game was released 3 years after the use of 3.10.020.

            // Redump entry 40337 has an EXE with a scrubbed version, an AuthServ.exe associated with SD 4.60.000, and copies of drvmgt.dll and secdrv.sys belonging to version 2.90.040.
            // This also seems like an accidental distribution of older drivers, as this game was released about 3 years after the use of 2.90.040.

            var sha1 = HashTool.GetFileHash(firstMatchedString, HashType.SHA1);
            return sha1?.ToUpperInvariant() switch
            {
                // Found in Redump entry 102979.
                "B858CB282617FB0956D960215C8E84D1CCF909C6" => "(Empty File)",

                // Found in Redump entry 63488.
                "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709" => "(Empty File)",

                // Found in Redump entries 29073 and 31149.
                "33434590D7DE4EEE2C35FCC98B0BF141F422B26D" => "1.06.000",

                // Found in Redump entries 9718 and 46756.
                "D5E4C99CDCA8091EC8010FCB96C5534A8BE35B43" => "1.07.000",

                // Found in Redump entries 12885 and 66210.
                // SafeDisc Lite found in Redump entries 32751.
                "412067F80F6B644EDFB25932EA34A7E92AD4FC21" => "1.09.000/Lite",

                // Found in Redump entries 37523 and 66586.
                "87C0DA1B52681FA8052A915E85699738993BEA72" => "1.11.000",

                // Found in Redump entries 21154 and 37982. 
                "3569FE747311265FDC83CBDF13361B4E06484725" => "1.20.000",

                // Found in Redump entry 37920.
                "89795A34A2CAD4602713524365F710948F7367D0" => "1.20.001",

                // Found in Redump entries 31526 and 55080.
                "D31725FF99BE44BC1BFFF171F4C4705F786B8E91" => "1.30.010",

                // Found in Redump entries 9617 and 49552.
                "2A86168FE8EFDFC31ABCC7C5D55A1A693F2483CD" => "1.35.000",

                // Found in Redump entries 2595 and 30121.
                "8E41DB1C60BBAC631B06AD4F94ADB4214A0E65DC" => "1.40.004",

                // Found in Redump entries 44350 and 63323.
                "833EA803FB5B0FA22C9CF4DD840F0B7DE31D24FE" => "1.41.000",

                // Found in Redump entries 37832 and 42091.
                "1A9C8F6A5BD68F23CA0C8BCB890991AB214F14E0" => "1.41.001",

                // Found in Redump entries 30555 and 55078.
                "0BF4574813EA92FEE373481CA11DF220B6C4F61A" => "1.45.011",

                // Found in Redump entries 28810 and 62935.
                "812121D39D6DCC4947E177240655903DEC4DA15A" => "1.50.020",

                // Found in Redump entries 72195 and 73502.
                "04ED7AC39FE7A6FAB497A498CBCFF7DA19BF0556" => "2.05.030",

                // Found in Redump entries 38541 and 59462 and 81096.
                "0AB8330A33E188A29E8CE1EA9625AA5935D7E8CE" => "2.10.030",

                // Found in Redump entries 55823 and 79476.
                "5198DA51184CA9F3A8096C6136F645B454A85F6C" => "2.30.030",

                // Found in Redump entries 15312 and 48863.
                "414CAC2BE3D9BE73796D51A15076A5A323ABBF2C" => "2.30.031",

                // Found in Redump entries 9819 and 53658. 
                "126DCA2317DA291CBDE13A91B3FE47BA4719446A" => "2.30.033",

                // Found in Redump entries 9846 and 65642.
                "1437C8C149917C76F741C6DBEE6B6B0CC0664F13" => "2.40.010",

                // Found in Redump entries 23786 and 37478.
                "10FAD492991C251C9C9394A2B746C1BF48A18173" => "2.40.011",

                // Found in Redump entries 30022 and 75014.
                "94267BB97C418A6AA22C1354E38136F889EB0B6A" => "2.51.020",

                // Found in Redump entries 31666 and 66852.
                "27D5E7F7EEE1F22EBDAA903A9E58A7FDB50EF82C" => "2.51.021",

                // Found in Redump entries 2064 and 47047.
                "F346F4D0CAB4775041AD692A6A49C47D34D46571" => "2.60.052",

                // Found in Redump entries 13048 and 35385.
                "88C7AA6E91C9BA5F2023318048E3C3571088776F" => "2.70.030",

                // Found in Redump entries 48101 and 64198.
                "544EE77889092129E9818B5086E19197E5771C7F" => "2.72.000",

                // Found in Redump entries 32783 and 72743.
                "EA6E24B1F306391CD78A1E7C2F2C0C31828EF004" => "2.80.010",

                // Found in Redump entries 39273 and 59351.
                "1BF885FDEF8A1824C34C10E2729AD133F70E1516" => "2.80.011",

                // Found in Redump entries 11638, 52606, and 62505.
                "B824ED257946EEE93F438B25C855E9DDE7A3671A" => "2.90.040",

                // Found in Redump entries 13230 and 68204.
                // SafeDisc 4+ is known to sometimes use old versions of drivers, such as in Redump entry 101261.
                "CDA56FD150C9E9A19D7AF484621352122431F029" => "3.10.020/4+",

                // Found in Redump entries 36511 and 74338.
                "E5504C4C31561D38C1F626C851A8D06212EA13E0" => "3.15.010",

                // Found in Redump entries 15383 and 35512. 
                "AABA7B3EF08E80BC55282DA3C3B7AA598379D385" => "3.15.011",

                // Found in Redump entries 58625, 75782, and 84586.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 3.20.020-3.20.024, including Redump entries 20729, 30404, and 56748.
                // TODO: Further investigate versions 3.20.020-3.20.024, and verify that 3.20.024 doesn't use drvmgt.dll at all.
                "ECB341AB36C5B3B912F568D347368A6A2DEF8D5F" => "3.20.020-3.20.022",

                // Found in Redump entries 53666, 76775, and 102301.
                "69C776F67EBD53CB5FD760B498B4A491BF22F293" => "3.20.022",

                // Found in Redump entry 102806.
                "2BD7CD06CED6F6FB1A31AAE2D6C403C166366C6F" => "3.20.022",

                // Found in Redump entries 15614, 79729, 83408, and 86196.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 4.00.001-4.00.003, including Redump entries 33326, 51597, and 67927.
                "E21FF43C2E663264D6CB11FBBC31EB1DCEE42B1A" => "4.00.000-4.00.003",

                // Found in Redump entry 49677.
                "7C5AB9BDF965B70E60B99086519327168F43F362" => "4.00.002",

                // Found in Redump entries 46765 and 78980.
                "A5247EC0EC50B8F470C93BF23E3F2514C402D5AD" => "4.00.002+",

                // Found in Redump entries 74564 and 80776.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 4.50.000, including Redump entries 58990 and 65569.
                "C658E0B4992903D5E8DD9B235C25CB07EE5BFEEB" => "4.50.000",

                // Found in Redump entry 20092.
                "02F373C1D246DBEFBFD5F39B6D0E40E2964B0027" => "4.60.000",

                // Found in Redump entry 56320.
                "84480ABCE4676EEB9C43DFF7C5C49F0D574FAC25" => "4.70.000",

                // There are no known uses of drvmgt.dll (or secdrv.sys) after 4.70.000.

                // Found distributed in https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe and https://web.archive.org/web/20010707163339/http://www.macrovision.com:80/demos/safedisc.exe, but unknown what version it is associated with.
                "8426690FA43076EE466FD1B2D1F2F1267F9CC3EC" => "Unknown Version (Report this to us on GitHub)",

                // Found in Redump entry 121132.
                // The game itself is protected with SecuROM, and contains a secdrv.sys associated with SafeDisc 2.90.040, despite that version not having been used with DVDs.
                "18AD11E1B8D6A644989E12C12258B548996C1C96" => "Unknown Version (DVD) (Report this to us on GitHub)",

                _ => "Unknown Version (Report this to us on GitHub)",
            };
        }

        internal static string? GetSafeDiscSplshVersion(string firstMatchedString, IEnumerable<string>? files)
        {
            // Special thanks to TheMechasaur for combing through known SafeDisc games and cataloging the splash-screens used in them, making these detections possible. 

            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            var sha1 = HashTool.GetFileHash(firstMatchedString, HashType.SHA1);
            switch (sha1?.ToUpperInvariant())
            {
                // Found in Redump entry 63488.
                case "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709":
                    return "(Empty File)";

                // First known generic SafeDisc splash-screen.
                // 4-bit (16 color) version, found in Redump entries 43321, 45040, 45202, 66586, 68206, 75501, 79272, and 110603.
                case "D8A8CF761DD7C04F635385E4C4589E5F26C6171E":
                    return "1.11.000-2.40.011";
                // 8-bit (256 color) version, found in Redump entries 43321, 45040, 45202, 66586, 68206, 75501, 79272, and 110603.
                case "0C9E45BF3EBE1382A3593994328C22BCB9A55456":
                    return "1.11.000-2.40.011";

                // Second known generic SafeDisc splash-screen.
                // 4-bit (16 color), found in Redump entries 46339 and 75897.
                case "9B80F524D45041ED8CE1613AD5BDE94BFDBB2814":
                    return "2.70.030-2.80.010";
                // 8-bit (256 color) version, found in Redump entries 46339 and 75897.
                case "827AE9A32906CBE9098C9101184E0BE74CEA2744":
                    return "2.70.030-2.80.010";

                // Third known generic SafeDisc splash-screen.
                // 4-bit (16 color), found in Redump entries 74338, 75782, 84985, and 91552.
                case "814ED63FD619655650E271D1B8B46BBE39C3655A":
                    return "3.15.010-3.20.022";
                // 8-bit (256 color) version, found in Redump entries 31824, 74338, 75782, 84985, 91552, and 104053.
                case "40C7ACEDB6C41AB067285090373E886EFB4F4AC4":
                    return "3.15.010-4.60.000";

                default:
                    return null;
            }

            // There appear to be a few distinct generations of file names used for SafeDisc splash-screens.
            // The first are the files named "SPLSH16.BMP"/"SPLSH256.BMP", which were typically used in SafeDisc versions 1.00.025-1.01.044.
            // The next are the files named "000004XX", "000008XX", "00000cXX", and "00001XXX". When one of these is present, they seemingly always come in pairs of 2 with the extensions ".016" and ".256". They're typically present in SafeDisc versions 1.06.000-2.51.021.
            // Next come the files simply named "0000000X", which still come in pairs with the extensions ".016" and ".256", starting in SafeDisc version 2.60.052 up until version 4.85.000. After this point, there doesn't seem to be any consistent SafeDisc splash-screen used at all.
            // Starting SafeDisc version 4.00.000, the files with the ".016" extension seem to mostly disappear, with the ".256" files still being present.
            // Exceptions: 
            // The files "00000409.016" and "00000409.256" are present in Redump entry 39273, despite it being SafeDisc 2.80.011. This may be because this disc contains some form of SafeDisc Lite as well, which tends to more closely resemble SafeDisc 1.
            // Redump entry 51597 contains "00000000.016" and "00000000.256", breaking the trend of SafeDisc 4 not having any files with the ".016" extension. This may be due to this being a rerelease, so the splash-screen may have already been present in the original game files and carried over.

            // TODO: Investigate "/409/splash.bmp" and "/0x0409.ini" files in Redump entry 45469.

            // Known SafeDisc splash-screen file names (case-insensitive):
            // "00000000.016": Found in SafeDisc version 2.60.052-4.00.003 (Redump entries 2064, 9621, 11639, 13230, 32783, 35385, 35512, 39273, 52606, 51597, 63813, 74338, 76775, and 84586).
            // "00000000.256": Found in SafeDisc version 2.60.052-4.85.000 (Redump entries 2064, 9621, 11639, 13230, 32783, 34783, 35382, 35385, 35512, 39273, 46765, 52606, 51597, 63813, 68551, 71646, 74338, 74366, 76775, 76813, 79113, 83017, 84586, and 98589).
            // "00000001.016": Found in SafeDisc version 2.72.000-3.20.024 (Redump entries 9621, 76775, and 86177).
            // "00000001.256": Found in SafeDisc version 2.72.000-4.50.000 (Redump entries 9621, 71646, 76775, 76813, and 86177).
            // "00000002.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000002.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000003.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000003.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000004.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000004.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000005.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000005.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000006.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000006.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000007.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000007.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000008.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000008.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000009.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000009.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000010.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000010.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000011.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000011.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000a.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000a.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000b.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000b.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000c.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000c.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000d.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000d.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000e.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000e.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000f.016": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "0000000f.256": Found in SafeDisc version 2.72.000 (Redump entry 9621).
            // "00000404.016": Found in SafeDisc versions 1.40.004-1.50.020 (IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000404.256": Found in SafeDisc versions 1.40.004-1.50.020 (IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000406.016": Found in SafeDisc versions 1.41.000-2.51.021 (Redump entries 61047 and 66852).
            // "00000406.256": Found in SafeDisc versions 1.41.000-2.51.021 (Redump entries 61047 and 66852).
            // "00000407.016": Found in SafeDisc versions 1.07.000-2.51.021 (Redump entries 43321, 44350, 46756, 48863, 49552, 66586, 66852, 72195, and 79476, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000407.256": Found in SafeDisc versions 1.07.000-2.51.021 (Redump entries 43321, 44350, 46756, 48863, 49552, 66586, 66852, 72195, and 79476, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000408.016": Found in SafeDisc version 2.51.021 (Redump entry 38589).
            // "00000408.256": Found in SafeDisc version 2.51.021 (Redump entry 38589).
            // "00000409.016": Found in SafeDisc versions 1.06.000-2.80.011 (Redump entries 2022, 2595, 9718, 9819, 9846, 12885, 23786, 29073, 30022, 30555, 31526, 31666, 37832, 37920, 37982, 39273, 48863, 49552, 59462, 62935, and 63323).
            // "00000409.256": Found in SafeDisc versions 1.06.000-2.80.011 (Redump entries 2022, 2595, 9718, 9819, 9846, 12885, 23786, 29073, 30022, 30555, 31526, 31666, 37982, 37920, 37832, 39273, 48863, 49552, 59462, 62935, and 63323).
            // "0000040A.016": Found in SafeDisc versions 1.06.000-2.51.021 (Redump entries 29073, 43321, 49552, and 66852).
            // "0000040A.256": Found in SafeDisc versions 1.06.000-2.51.021 (Redump entries 29073, 43321, 49552 and 66852).
            // "0000040c.016": Found in SafeDisc versions 1.30.010-2.51.021 (Redump entries 43321, 48863, 49552, 66852, 72195, and 79476, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "0000040c.256": Found in SafeDisc versions 1.30.010-2.51.021 (Redump entries 43321, 48863, 49552, 66852, 72195, and 79476, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "0000040d.016": Found in SafeDisc version 2.51.021 (Redump entry 38589).
            // "0000040d.256": Found in SafeDisc version 2.51.021 (Redump entry 38589).
            // "0000040f.016": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "0000040f.256": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000410.016": Found in SafeDisc versions 1.35.000-2.51.021 (Redump entries 9617, 48863, 49552, 66852, and 79476, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000410.256": Found in SafeDisc versions 1.35.000-2.51.021 (Redump entries 9617, 48863, 49552, 66852, and 79476, IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000411.016": Found in SafeDisc versions 1.40.004-2.51.031 (Redump entries 38589, 53659 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000411.256": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entries 38589, 53659 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000412.016": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entry 38589 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000412.256": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entry 38589 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000413.016": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entries 66852 and 72195 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000413.256": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entries 66852 and 72195 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000415.016": Found in SafeDisc versions 1.40.004-2.10.030 (Redump entry 38541 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000415.256": Found in SafeDisc versions 1.40.004-2.10.030 (Redump entry 38541 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000416.016": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entry 38589 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000416.256": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entry 38589 and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000419.016": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000419.256": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "0000041d.016": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entries 55823, 66852, and 72195, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "0000041d.256": Found in SafeDisc versions 1.40.004-2.51.021 (Redump entries 55823, 66852, and 72195, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "0000041e.016": Found in SafeDisc version 1.40.004 (IA item the-sims-thai-english-electronic-arts-2000).
            // "0000041e.256": Found in SafeDisc version 1.40.004 (IA item the-sims-thai-english-electronic-arts-2000).
            // "00000429.016": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000429.256": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000804.016": Found in SafeDisc versions 1.40.004-1.50.020 (IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000804.256": Found in SafeDisc versions 1.40.004-1.50.020 (IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000809.016": Found in SafeDisc versions 1.06.000-2.51.021 (Redump entries 9617, 31149, 37478, 37523, 37832, 43321, 48863, 53659, 59462, 66852, 72195, and 79476, and IA items the-sims-thai-english-electronic-arts-2000, the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000, and "primal-3d-interactive-series-professional-edition-2002-english" items "Interactive Hand CD", "Interactive Hip CD", and "Interactive Spine CD").
            // "00000809.256": Found in SafeDisc versions 1.06.000-2.51.021 (Redump entries 9617, 31149, 37478, 37523, 37832, 43321, 48863, 53659, 59462, 66852, 72195, and 79476, and IA items the-sims-thai-english-electronic-arts-2000, the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000, and "primal-3d-interactive-series-professional-edition-2002-english" items "Interactive Hand CD", "Interactive Hip CD", and "Interactive Spine CD").
            // "00000814.016": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000814.256": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000816.016": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000816.256": Found in SafeDisc version 1.41.000 (Redump entry 61047).
            // "00000c0a.016": Found in SafeDisc versions 1.11.000-2.30.031 (Redump entry 3569, 48863, 55078, 55080, and 79476, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00000c0a.256": Found in SafeDisc versions 1.11.000-2.30.031 (Redump entry 3569, 48863, 55078, 55080, and 79476, and IA items the-sims-thai-english-electronic-arts-2000 and the-sims-livin-large-expansion-pack-thai-english-electronic-arts-2000).
            // "00001009.016": Found in SafeDisc version 2.30.030 (Redump entry 45040).
            // "00001009.256": Found in SafeDisc version 2.30.030 (Redump entry 45040).
            // "SPLSH16.BMP": Found in SafeDisc versions 1.00.025-1.01.044 (Redump entries 66005 and 81619).
            // "SPLSH256.BMP": Found in SafeDisc versions 1.00.025-1.01.044 (Redump entries 66005 and 81619).
        }

        private static string GetSafeDiscDiagExecutableVersion(PortableExecutable pex)
        {
            // Different versions of this executable correspond to different SafeDisc versions.
            var version = pex.FileVersion;
            if (!string.IsNullOrEmpty(version))
            {
                return version switch
                {
                    // Found to be in Redump entry 65569.
                    // The product version is "4.50.00.1619 2005/06/08".
                    "4.50.00.1619" => "4.50.0.1619 / SafeDisc 4.50.000",

                    // Found to be in Redump entries 20092 and 73786.
                    // The product version is "4.60.00.1702 2005/08/03".
                    "4.60.00.1702" => "4.60.0.1702 / SafeDisc 4.60.000",

                    // Found to be in Redump entry 34783.
                    // The product version is "4.70.00.1941 2006/04/26".
                    "4.70.00.1941" => "4.70.0.1941 / SafeDisc 4.70.000",

                    _ => $"Unknown Version {version} (Report this to us on GitHub)",
                };
            }

            return "Unknown Version (Report this to us on GitHub)";
        }
    }
}
