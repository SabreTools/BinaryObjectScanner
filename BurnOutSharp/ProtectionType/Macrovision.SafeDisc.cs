using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// SafeDisc is an incredibly commonly used copy protection created by Macrovision in 1998.
    /// It uses several different copy protection mechanisms, such as reading a disc signature dependent on the presence of bad sectors and the attempted prevention of burning copies to CD-R.
    /// SafeDisc has been most commonly found on PC games and applications, though there a number of Mac discs that contain the protection as well.
    /// At least one system other than PC/Mac is known to use SafeDisc as well, this being the "ZAPiT Games Game Wave Family Entertainment System" which seems to use a form of SafeDisc 4 (Redump entry 46269).
    /// SafeDisc resources:
    /// https://web.archive.org/web/20031009091909/http://www.macrovision.com/products/safedisc/index.shtml
    /// https://web.archive.org/web/20041023011150/http://www.macrovision.com/products/safedisc/index.shtml (Marketed as "SafeDisc Advanced")
    /// https://web.archive.org/web/20041008173722/http://www.macrovision.com/pdfs/safedisc_datasheet.pdf
    /// SafeCast is in the same family of protections, and appears to mainly be for license management, and doesn't appear to affect the mastering of the disc in any way.
    /// Although SafeCast is most commonly used in non-game software, there is one game that comes with both SafeDisc and SafeCast protections (Redump entry 83145).
    /// Macrovision bought the company C-Dilla and created SafeCast based on C-Dilla's existing products (https://web.archive.org/web/20030212040047/http://www.auditmypc.com/freescan/readingroom/cdilla.asp).
    /// That being said, there are references to C-Dilla within SafeDisc protected executables as early as 1.00.025, making the exact relationship between SafeDisc/Macrovision/C-Dilla unclear.
    /// SafeCast resources: 
    /// https://web.archive.org/web/20031204024544mp_/http://www.macrovision.com/products/safecast/index.shtml
    /// https://web.archive.org/web/20010417222834/http://www.macrovision.com/press_rel3_17_99.html
    /// https://www.extremetech.com/computing/53394-turbotax-so-what-do-i-do-now/4
    /// https://web.archive.org/web/20031013085038/http://www.pestpatrol.com/PestInfo/c/c-dilla.asp
    /// Other protections in the Macrovision "Safe-" family of protections that need further investigation:
    /// SafeScan (https://cdn.loc.gov/copyright/1201/2003/reply/029.pdf).
    /// SafeDisc HD (https://computerizedaccount.tripod.com/computerizedaccountingtraining/id27.html).
    /// Additional resources and information:
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_safedisc.shtml
    /// https://web.archive.org/web/20080604020524/http://www.trymedia.com/safedisc-advanced.html
    /// </summary>
    public partial class Macrovision
    {
        internal string SafeDiscCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Present in "secdrv.sys" files found in SafeDisc 2.80.010+.
            string name = pex.FileDescription;
            if (name?.Equals("Macrovision SECURITY Driver", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeDisc Security Driver {GetSecDrvExecutableVersion(pex)}";

            // Present on all "CLOKSPL.DLL" versions before SafeDisc 1.06.000. Found on Redump entries 61731 and 66004. 
            name = pex.ProductName;
            if (name?.Equals("SafeDisc CDROM Protection System", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeDisc 1.00.025-1.01.044";

            // Get the .text section, if it exists
            string match = CheckSectionForProtection(file, includeDebug, pex, ".text");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .txt2 section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, ".txt2");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the CODE section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, "CODE");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .data section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, ".data");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the stxt371 and stxt774 sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            bool stxt371Section = pex.ContainsSection("stxt371", exact: true);
            bool stxt774Section = pex.ContainsSection("stxt774", exact: true);
            if (stxt371Section || stxt774Section)
                return $"SafeDisc {GetSafeDisc320to4xVersion(null, null, null)}";

            // Present on all "CLOKSPL.EXE" versions before SafeDisc 1.06.000. Found on Redump entries 61731 and 66004. 
            // Only found so far on SafeDisc 1.00.025-1.01.044, but the report is currently left generic due to the generic nature of the check.
            name = pex.FileDescription;
            if (name?.Equals("SafeDisc", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeDisc";

            // TODO: Add entry point check
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Safedisc.2.sg

            return null;
        }

        /// <inheritdoc/>
        internal ConcurrentQueue<string> SafeDiscCheckDirectoryPath(string path, IEnumerable<string> files)
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
                new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), GetSafeDisc00000001TMPVersion, "SafeDisc"),

                // Found in many versions of SafeDisc, beginning in 2.05.030 and being used all the way until the final version 4.90.010. It is not always present, even in versions it has been used in. Found in Redump entries 56319 and 72195.
                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2+"),

                new PathMatchSet(new PathMatch("DPLAYERX.DLL", useEndsWith: true), GetSafeDiscDPlayerXVersion, "SafeDisc"),
                new PathMatchSet(new PathMatch("drvmgt.dll", useEndsWith: true), GetSafeDiscDrvmgtVersion, "SafeDisc"),
                new PathMatchSet(new PathMatch("secdrv.sys", useEndsWith: true), GetSafeDiscSecdrvVersion, "SafeDisc Security Driver"),

                // Used to distribute SafeDisc driver updates over the internet. Two distinct versions known to exist, with Microsoft also having distributed the later update as well.
                // Version 1: https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe
                // Version 2: https://web.archive.org/web/20051104123646/http://www.macrovision.com/products/safedisc/safedisc.exe
                // Microsoft download page: https://web.archive.org/web/20080204081329/http://www.microsoft.com/downloads/details.aspx?FamilyID=eae20f0f-c41c-44fe-84ce-1df707d7a2e9&DisplayLang=en
                new PathMatchSet(new PathMatch("safedisc.exe", useEndsWith: true), "SafeDisc Driver Installer"),

                // Found in Redump entries 28810 and 30555.
                // Name check overmatches with a seemingly completely unrelated application, ironically included on at least one SafeDisc game (Redump entry 34828).
                // new PathMatchSet(new PathMatch("mcp.dll", useEndsWith: true), "SafeDisc (Version 1.45.011-1.50.020)"),

                // Found in Redump entry 58455.
                // Unknown if it's a game specific file, but it contains the stxt371 and stxt774 sections.
                // new PathMatchSet(new PathMatch("CoreDLL.dll", useEndsWith: true), "SafeDisc"),

                // DIAG.exe is present in some SafeDisc discs between 4.50.000-4.70.000, but is already detected through other checks and properly reports the expected version string.
                // Incase further detection is needed, it's Product Description is "SafeDisc SRV Tool APP", and the Product version seems to correspond directly to the appropriate SafeDisc version.
                // "4.50.00.1619 2005/06/08" -> SafeDisc 4.50.000 (Redump entry 58455).
                // "4.60.00.1702 2005/08/30" -> SafeDisc 4.60.000 (Redump entry 65209).
                // "4.70.00.1941 2006/04/26" -> SafeDisc 4.70.000 (Redump entry 34783).

                // Found in seemingly every SafeDisc Lite disc. (CD: Redump entries 25579 and 57986. DVD: Redump entry 63941). 
                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),
                new PathMatchSet(new PathMatch("LTDLL.DLL", useEndsWith: true), "SafeDisc Lite"),

                // Found on Redump entry 42762.
                new PathMatchSet(".SafeDiscDVD.bundle", "SafeDisc for Macintosh"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        internal string SafeDiscCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CLCD16.DLL", useEndsWith: true), GetSafeDiscCLCD16Version, "SafeDisc"),
                new PathMatchSet(new PathMatch("CLCD32.DLL", useEndsWith: true), GetSafeDiscCLCD32Version, "SafeDisc"),
                new PathMatchSet(new PathMatch("CLOKSPL.EXE", useEndsWith: true), GetSafeDiscCLOKSPLVersion, "SafeDisc"),

                new PathMatchSet(new PathMatch("00000001.TMP", useEndsWith: true), GetSafeDisc00000001TMPVersion, "SafeDisc"),
                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2+"),

                // TODO: Research "splash16.bmp" and "splash256.bmp".

                new PathMatchSet(new PathMatch("DPLAYERX.DLL", useEndsWith: true), GetSafeDiscDPlayerXVersion, "SafeDisc"),
                new PathMatchSet(new PathMatch("drvmgt.dll", useEndsWith: true), GetSafeDiscDrvmgtVersion, "SafeDisc"),
                new PathMatchSet(new PathMatch("secdrv.sys", useEndsWith: true), GetSafeDiscSecdrvVersion, "SafeDisc Security Driver"),

                // Found in Redump entries 28810 and 30555.
                // Name check overmatches with a seemingly completely unrelated application, ironically included on at least one SafeDisc game (Redump entry 34828).
                // new PathMatchSet(new PathMatch("mcp.dll", useEndsWith: true), "SafeDisc (Version 1.45.011-1.50.020)"),

                // Found in Redump entry 58455.
                // Unknown if it's a game specific file, but it contains the stxt371 and stxt774 sections.
                // new PathMatchSet(new PathMatch("CoreDLL.dll", useEndsWith: true), "SafeDisc"),

                // DIAG.exe is present in some SafeDisc discs between 4.50.000-4.70.000, but is already detected through other checks and properly reports the expected version string.
                // Incase further detection is needed, it's Product Description is "SafeDisc SRV Tool APP", and the Product version seems to correspond directly to the appropriate SafeDisc version.
                // "4.50.00.1619 2005/06/08" -> SafeDisc 4.50.000 (Redump entry 58455).
                // "4.60.00.1702 2005/08/30" -> SafeDisc 4.60.000 (Redump entry 65209).
                // "4.70.00.1941 2006/04/26" -> SafeDisc 4.70.000 (Redump entry 34783).

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

        internal static string GetSafeDisc320to4xVersion(string file, byte[] fileContent, List<int> positions) => "3.20-4.xx [version expunged]";

        internal static string GetSafeDiscVersion(string file, byte[] fileContent, List<int> positions)
        {
            // TODO: Figure out how to properly distinguish SafeDisc and SafeCast since both use
            // the same generic BoG_ string. The current combination check doesn't seem consistent

            // Known SafeDisc versions:
            // 1.00.025 (Redump entry 66005).
            // 1.00.026 (Redump entries 1882 and 30049).
            // 1.00.030 (Redump entries 31575 and 41923).
            // 1.00.032 (Redump entries 1883 and 42114).
            // 1.00.035 (Redump entries 36223 and 40771).
            // 1.01.034 (Redump entries 42155 and 47574).
            // 1.01.038 (Redump entry 51459).
            // 1.01.043 (Redump entries 34562 and 63304).
            // 1.01.044 (Redump entries 61731 and 81619).
            // 1.01.045 (Currently only found in a pirate compilation disc: IA item "cdrom-classic-fond-58").
            // 1.06.000 (Redump entries 29073 and 31149).
            // 1.07.000 (Redump entries 9718 and 46756).
            // 1.09.000 (Redump entries 12885 and 66210).
            // 1.11.000 (Redump entries 37523 and 66586).
            // 1.20.000 (Redump entries 21154 and 37982).
            // 1.20.001 (Redump entry 37920).
            // 1.30.010 (Redump entries 31526 and 55080).
            // 1.35.000 (Redump entries 9617 and 49552).
            // 1.40.004 (Redump entries 2595 and 30121).
            // 1.41.000 (Redump entries 44350 and 63323).
            // 1.41.001 (Redump entries 37832 and 42091).
            // 1.45.011 (Redump entries 30555 and 55078).
            // 1.50.020 (Redump entries 28810 and 62935).
            // 2.05.030 (Redump entries 72195 and 73502).
            // 2.10.030 (Redump entries 38541 and 59462 and 81096).
            // 2.30.030 (Redump entries 55823 and 79476).
            // 2.30.031 (Redump entries 15312 and 48863).
            // 2.30.033 (Redump entries 9819 and 53658).
            // 2.40.010 (Redump entries 9846 and 65642).
            // 2.40.011 (Redump entries 23786 and 37478).
            // 2.51.020 (Redump entries 30022 and 75014).
            // 2.51.021 (Redump entries 31666 and 66852).
            // 2.60.052 (Redump entries 2064 and 47047).
            // 2.70.030 (Redump entries 13048 and 35385).
            // 2.72.000 (Redump entries 48101 and 64198).
            // 2.80.010 (Redump entries 32783 and 72743).
            // 2.80.011 (Redump entries 39273 and 59351).
            // 2.90.040 (Redump entries 52606 and 62505).
            // 3.10.020 (Redump entries 13230 and 68204).
            // 3.15.010 (Redump entries 36511 and 74338).
            // 3.15.011 (Redump entries 15383 and 35512).
            // 3.20.020 (Redump entries 30404 and 56748).
            // 3.20.022 (Redump entries 58625 and 91552). 
            // 3.20.024 (CD: Redump entries 20729 and 63813. DVD: Redump entries 20728 and 64255).
            // 4.00.000 (CD: Redump entries 35382 and 79729. DVD: Redump entry 74520).
            // 4.00.001 (CD: Redump entries 8842 and 83017. DVD: Redump entries 15614 and 38143).
            // 4.00.002 (CD: Redump entries 42034 and 71646. DVD: Redump entries 78980 and 86196).
            // 4.00.003 (CD: Redump entries 60021 and 68551. DVD: Redump entries 51597 and 83408).
            // 4.50.000 (CD: Redump entries 58990 and 80776. DVD: Redump entries 65569 and 76813).
            // 4.60.000 (CD: Redump entries 45686 and 46765. DVD: Redump entries 45469 and 50682).
            // 4.70.000 (CD: Redump entry 56320. DVD: Redump entries 34783 and 66403).
            // 4.80.000 (CD: Redump entries 64145 and 78543. DVD: No samples so far).
            // 4.81.000 (CD: No samples so far. DVD: Redump entries 52523 and 76346).
            // 4.85.000 (CD: No samples so far. DVD: Redump entries 20434 and 31766).
            // 4.90.000 (CD: No samples so far. DVD: Redump entries 56319 and 66333).
            // 4.90.010 (CD: Redump entries 58573 and 78976. DVD: redump entries 11347 and 29069).

            // Known SafeCast versions:
            // 2.11.010 (Redump entry 83145).
            // 2.16.050 (IA items "cdrom-turbotax-2002", "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282", and "TurboTax_Premier_Tax_Year_2002_for_Windows_v02.00Z-R_Intuit_352283_2002").
            // 2.42.000 (found in "Dreamweaver MX 2004 v7.0.1" according to https://web.archive.org/web/20210331144912/https://protectionid.net/).
            // 2.50.030 (found in "ArcSoft Media Card Companion v1.0" according to https://web.archive.org/web/20210331144912/https://protectionid.net/).
            // 2.51.000 (found in "Autodesk Inventor Professional v9.0" according to https://web.archive.org/web/20210331144912/https://protectionid.net/).
            // 2.60.030 (found in "Data Becker Web To Date v3.1" according to https://web.archive.org/web/20210331144912/https://protectionid.net/).
            // 2.67.010 (found in "Adobe Photoshop CS2" according to https://web.archive.org/web/20210331144912/https://protectionid.net/).

            int index = positions[0] + 20; // Begin reading after "BoG_ *90.0&!!  Yy>" for old SafeDisc
            int version = fileContent.ReadInt32(ref index);
            int subVersion = fileContent.ReadInt32(ref index);
            int subsubVersion = fileContent.ReadInt32(ref index);

            if (version != 0)
                return $"{version}.{subVersion:00}.{subsubVersion:000}";

            index = positions[0] + 18 + 14; // Begin reading after "BoG_ *90.0&!!  Yy>" for newer SafeDisc
            version = fileContent.ReadInt32(ref index);
            subVersion = fileContent.ReadInt32(ref index);
            subsubVersion = fileContent.ReadInt32(ref index);

            if (version == 0)
                return string.Empty;

            return $"{version}.{subVersion:00}.{subsubVersion:000}";
        }

        internal static string GetSafeDisc00000001TMPVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // This file is present in most, if not all, SafeDisc protected discs. It seems to have very consistent file sizes, only being found to use three different file sizes in it's entire run.
            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                // Found in Redump entries 37832 and 66005. 
                case 20:
                    return "1.00.025-1.41.001";
                // Found in Redump entries 30555 and 58573.
                case 2_048:
                    return "1.45.011+ (CD) (Confirm presence of other SafeDisc files)";
                // Found in Redump entries 11347 and 64255.
                case 20_482_048:
                    return "3+ (DVD)";
                default:
                    return "Unknown Version (Report this to us on GitHub)";
            }
        }

        internal static string GetSafeDiscCLCD16Version(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The hash of the file CLCD16.dll is able to provide a broad version range that appears to be consistent, but it seems it was rarely updated so these checks are quite broad.
            string sha1 = Utilities.GetFileSHA1(firstMatchedString);
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
                    return "1.06.000-1.50.020";
                default:
                    return "Unknown Version (Report this to us on GitHub)";
            }
        }

        internal static string GetSafeDiscCLCD32Version(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The hash of the file CLCD32.dll so far appears to be a solid indicator of version for versions it was used with. It appears to have been updated with every release, unlike it's counterpart, CLCD16.dll.
            string sha1 = Utilities.GetFileSHA1(firstMatchedString);
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
            string sha1 = Utilities.GetFileSHA1(firstMatchedString);
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

        public static string GetSafeDiscDrvmgtVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            // The file "drvmgt.dll" has been found to be incredibly consistent between versions, with the vast majority of files based on hash corresponding 1:1 with the SafeDisc version used according to the EXE.
            // There are occasionaly inconsistencies, even within the well detected version range. This seems to me to mostly happen with later (3.20+) games, and seems to me to be an example of the SafeDisc distribution becoming more disorganized with time.
            // Particularly interesting inconsistencies will be noted below:
            // Redump entry 73786 has an EXE with a scrubbed version, a DIAG.exe with a version of 4.60.000, and a copy of drvmgt.dll belonging to version 3.10.020. This seems like an accidental(?) distribution of older drivers, as this game was released 3 years after the use of 3.10.020.
            string sha1 = Utilities.GetFileSHA1(firstMatchedString);
            switch (sha1)
            {
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
                // Found in Redump entry 56320.
                case "84480ABCE4676EEB9C43DFF7C5C49F0D574FAC25":
                    return "4.70.000";
                // Found distributed in https://web.archive.org/web/20040614184055/http://www.macrovision.com:80/products/safedisc/safedisc.exe, but unknown what version it is associated with.
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

        // TODO: Verify these checks and remove any that may not be needed, file version checks should remove the need for any checks for 2.80+.
        public static string GetSafeDiscSecdrvVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(firstMatchedString) || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                // Found in Redump entries 9718, 12885, 21154, 31149, 37523, 37920.
                case 14_304:
                    return "/ SafeDisc 1.06.000-1.20.001";
                // Found in Redump entries 9617 and 31526.
                case 14_368:
                    return "/ SafeDisc 1.30.010-1.35.000";
                // Found in Redump entries 2595, 37832, and 44350.
                case 10_848:
                    return "/ SafeDisc 1.40.004-1.41.001";
                // Found in Redump entries 30555 and 55078.
                case 11_968:
                    return "/ SafeDisc 1.45.011";
                // Found in Redump entries 28810 and 62935.
                case 11_616:
                    return "/ SafeDisc 1.50.020";
                // Found in Redump entries 72195 and 73502.
                case 18_768:
                    return "/ SafeDisc 2.05.030";
                // Found in Redump entries 38541 and 59462.
                case 20_128:
                    return "/ SafeDisc 2.10.030";
                // Found in Redump entries 9819, 15312, 55823.
                case 27_440:
                    return "/ SafeDisc 2.30.030-2.30.033";
                // Found in Redump entries 9846 and 23786.
                case 28_624:
                    return "/ SafeDisc 2.40.010-2.40.011";
                // Found in Redump entries 30022 and 31666.
                case 28_400:
                    return "/ SafeDisc 2.51.020-2.51.021";
                // Found in Redump entries 2064 and 47047.
                case 29_392:
                    return "/ SafeDisc 2.60.052";
                // Found in Redump entries 13048 and 48101.
                case 11_376:
                    return "/ SafeDisc 2.70.030-2.72.000";
                // Found in Redump entries 32783 and 39273.
                case 12_464:
                    return "3.17.000 / SafeDisc 2.80.010";
                // Found in Redump entries 11638 and 52606.
                case 12_400:
                    return "3.18.000 / SafeDisc 2.90.010-2.90.040";
                // Found in Redump entries 13230, 15383, and 36511.
                case 12_528:
                    return "3.19.000 / SafeDisc 3.10.020-3.15.011";
                // Found in Redump entries 58625 and 84586.
                case 11_973:
                    return "3.22.000 / SafeDisc 3.20.020-3.20.022";
                // Found in Redump entries 15614, 42034, 45686, 56320, 60021, 79729, and 80776.
                case 163_644:
                    return "4.00.060 / SafeDisc 4.00.000-4.70.000";
                // Found distributed online, but so far not in a game release. TODO: Discover original source.
                // Can be found at https://github.com/ericwj/PsSecDrv/blob/master/tools/SECDRV/SECDRV.sys, and the file is confirmed to be distributed officialy by Microsoft: https://www.virustotal.com/gui/file/34bbb0459c96b3de94ccb0d73461562935c583d7bf93828da4e20a6bc9b7301d/.
                case 23_040:
                    return "4.03.086 / Unknown SafeDisc version";
                // This file is not currently known to be used in versions past 4.70.000.
                default:
                    return "Unknown Version (Report this to us on GitHub)";
            }
        }

        private string GetVersionFromSHA1Hash(string sha1Hash)
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

        private string CheckSectionForProtection(string file, bool includeDebug, PortableExecutable pex, string sectionName)
        {
            // This subtract is needed because BoG_ starts before the section
            var sectionRaw = pex.ReadRawSection(sectionName, first: true, offset: -64);
            if (sectionRaw != null)
            {
                // TODO: Add more checks to help differentiate between SafeDisc and SafeCast.
                var matchers = new List<ContentMatchSet>
                {
                    // Checks for presence of two different strings to differentiate between SafeDisc and SafeCast.
                    new ContentMatchSet(new List<byte?[]>
                    {
                        // BoG_ *90.0&!!  Yy>
                        new byte?[]
                        {
                            0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                            0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                            0x79, 0x3E
                        },

                        // product activation library
                        new byte?[]
                        {
                            0x70, 0x72, 0x6F, 0x64, 0x75, 0x63, 0x74, 0x20,
                            0x61, 0x63, 0x74, 0x69, 0x76, 0x61, 0x74, 0x69,
                            0x6F, 0x6E, 0x20, 0x6C, 0x69, 0x62, 0x72, 0x61,
                            0x72, 0x79
                        },
                    }, GetSafeDiscVersion, "SafeCast"),

                    // TODO: Investigate likely false positive in Redump entry 74384.
                    // Unfortunately, this string is used throughout a wide variety of SafeDisc and SafeCast versions. If no previous checks are able to able to differentiate between them, then a generic result has to be given.
                    // BoG_ *90.0&!!  Yy>
                    new ContentMatchSet(new byte?[]
                    {
                        0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                        0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                        0x79, 0x3E
                    }, GetSafeDiscVersion, "SafeCast/SafeDisc"),

                    // (char)0x00 + (char)0x00 + BoG_
                    new ContentMatchSet(new byte?[] { 0x00, 0x00, 0x42, 0x6F, 0x47, 0x5F }, GetSafeDisc320to4xVersion, "SafeDisc"),
                };

                return MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
            }

            return null;
        }

        private string GetSecDrvExecutableVersion(PortableExecutable pex)
        {
            // Different versions of this driver correspond to different SafeDisc versions.
            // TODO: Check if earlier versions of this driver contain the version string in a less obvious place. 
            string version = pex.FileVersion;
            if (!string.IsNullOrEmpty(version))
            {
                switch (version)
                {
                    // Found to be in Redump entry 32783.
                    // The product version is "3.17.000 Windows NT 2002/07/01".
                    case "3.17.000":
                        return "3.17.000 / SafeDisc 2.80.010-2.80.011";
                    // Found to be in Redump entry 52606.
                    // The product version is "3.18.000 Windows NT 2002/11/14".
                    case "3.18.000":
                        return "3.18.000 / SafeDisc 2.90.010-2.90.040";
                    // Found to be in Redump entry 13230.
                    // The product version is "3.19.000 Windows NT/2K/XP 2003/03/19".
                    case "3.19.000":
                        return "3.19.000 / SafeDisc 3.10.020-3.15.011";
                    // Found to be in Redump entry 58625.
                    // The product version is "SECURITY Driver 3.22.000 2004/01/16".
                    case "3.22.000":
                        return "3.22.000 / SafeDisc 3.20.020-3.20.022";
                    // Found to be in Redump entry 15614.
                    // The product version is "SECURITY Driver 4.00.060 2004/08/31".
                    case "4.00.060":
                        return "4.00.060 / SafeDisc 4.00.000-4.70.000";
                    // Found distributed online, but so far not in a game release. TODO: Discover original source.
                    // Can be found at https://github.com/ericwj/PsSecDrv/blob/master/tools/SECDRV/SECDRV.sys, and the file is confirmed to be distributed officialy by Microsoft: https://www.virustotal.com/gui/file/34bbb0459c96b3de94ccb0d73461562935c583d7bf93828da4e20a6bc9b7301d/.
                    // The product version is "SECURITY Driver 4.03.086 2006/09/13".
                    case "4.03.086":
                        return "4.03.086 / Unknown SafeDisc version";
                    default:
                        return $"Unknown Version {version} (Report this to us on GitHub)";
                }
            }

            return "Unknown Version (Report this to us on GitHub)";
        }
    }
}
