using System;
using System.Collections.Generic;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// C-Dilla (https://web.archive.org/web/19980204101314/http://www.c-dilla.com/).
    /// As of May 2000, Ç-Dilla Ltd was renamed to Ç-Dilla Labs (https://web.archive.org/web/20000519231449/http://www.c-dilla.com:80/). As of June 2001, it was renamed to Macrovision Europe (https://web.archive.org/web/20010628061027/http://www.c-dilla.com:80/).
    /// AutoCAD appears to have been using C-Dilla products since 1996 (https://web.archive.org/web/19980204102208/http://www.c-dilla.com/press/201296.html).
    /// https://knowledge.autodesk.com/support/3ds-max/troubleshooting/caas/sfdcarticles/sfdcarticles/Description-of-the-C-Dilla-License-Management-System.html
    /// https://web.archive.org/web/20040223025801/www.macrovision.com/products/legacy_products/safecast/safecast_cdilla_faq.shtml
    /// https://forums.anandtech.com/threads/found-c-dilla-on-my-computer-get-it-off-get-it-off-heeeelp.857554/
    /// https://www.extremetech.com/extreme/53108-macrovision-offers-closer-look-at-safecastcdilla
    /// https://www.cadlinecommunity.co.uk/hc/en-us/articles/201873101-C-dilla-Failing-to-Install-or-Stops-Working-
    /// https://archive.org/details/acadlt2002cd
    /// https://archive.org/details/telepower
    /// 
    /// It seems that C-Dilla License Management System is a newer name for their CD-Secure product, based on this URL (https://web.archive.org/web/20050211004709/http://www.macrovision.com/products/cdsecure/downloads.shtml) leading to a download of LMS.
    /// Known versions:
    /// 1.31.34 (1.37.00?) (IA item "PCDDec1995").
    /// 2.19.001 (Needs further research) (IA item "great-explorers").
    /// 3.12.093 (Needs further research) ("PCMania CD78_1.iso" in IA item "PCMCDS").
    /// 3.23.000 (IA item "3ds-max-4.2original").
    /// 3.24.010 (IA item "ejay_nestle_trial").
    /// 3.27.000 (https://download.autodesk.com/mne/web/support/3dstudio/C-Dilla3.27.zip).
    /// 4.11.000 (Possibly an internal version used by SafeCast in Redump entry 95524).
    /// 
    /// TODO:
    /// Investigate C-Dilla CD-Compress.
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IExecutableCheck{T}.CheckExecutable(string, T, bool)"/>
        internal static string? CDillaCheckExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // TODO: Implement NE checks for "CDILLA05", "CDILLA10", "CDILLA16", and "CDILLA40".

            // TODO: Implement the following NE checks:

            // File Description "C-Dilla LMS Uninstaller" in "CdUnin16.exe" from CD-Secure/CD-Compress version 1.31.34.
            // File Description "C-Dilla RTS DLL" in "CDILLA05.DLL" from CD-Secure/CD-Compress version 1.31.34.
            // File Description "C-Dilla RTS TASK" in "CDILLA10.DLL" from CD-Secure/CD-Compress version 1.31.34.
            // File Description "C-Dilla Shell dialogs DLL" in "CDILLA40.DLL" from CD-Secure/CD-Compress version 1.31.34.
            // Product Name "C-Dilla License Management System" in "CdUnin16.exe" from CD-Secure/CD-Compress version 1.31.34.
            // Product Name "CD-Secure/CD-Compress" in "CDILLA05.DLL"/"CDILLA10.EXE" from CD-Secure/CD-Compress version 1.31.34.

            // File Description "16-bit C-Dilla DLL" in "cdilla51.dll" from C-Dilla LMS version 3.24.010.

            // File Description "C-Dilla 16-bit DLL" in "CDILLA40.DLL" from C-Dilla LMS version 3.27.000 for Windows 3.1/95/NT (This file specifically is known to report as version 3.15.000).
            // File Description "C-Dilla Windows 3.1x RTS" in "CDILLA05.DLL"/"CDILLA10.EXE" from C-Dilla LMS version 3.27.000 for Windows 3.1.
            // File Description "C-Dilla Windows 95 RTS" in "CDILLA05.DLL"/"CDILLA10.EXE" from C-Dilla LMS version 3.27.000 for Windows 95.
            // File Description "C-Dilla Windows NT RTS" in "CDILLA05.DLL"/"CDILLA10.EXE"/"CDILLA16.EXE" from C-Dilla LMS version 3.27.000 for Windows NT.
            // File Description "C-Dilla Windows 16-Bit RTS Installer" in "CdaIns16.dll"/"CdSetup.exe" from C-Dilla LMS version 3.27.000.


            return null;
        }

        /// <inheritdoc cref="Interfaces.IExecutableCheck{T}.CheckExecutable(string, T, bool)"/>
        internal static string? CDillaCheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;

            // Found in in "cdilla52.dll" from C-Dilla LMS version 3.24.010.
            if (name.OptionalEquals("32-bit C-Dilla DLL", StringComparison.OrdinalIgnoreCase))
                return $"C-Dilla License Management System";

            // Found in "CdaIns32.dll" and "CdSet32.exe" from version 3.27.000 of C-Dilla LMS.
            if (name.OptionalEquals("C-Dilla Windows 32-Bit RTS Installer", StringComparison.OrdinalIgnoreCase))
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDILLA32.DLL"/"CDILLA64.EXE" from C-Dilla LMS version 3.27.000 for Windows 3.1.
            if (name.OptionalEquals("C-Dilla Windows 3.1x RTS", StringComparison.OrdinalIgnoreCase))
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDILLA13.DLL"/"CDILLA32.DLL"/"CDILLA64.EXE" from C-Dilla LMS version 3.27.000 for Windows 95.
            if (name.OptionalEquals("C-Dilla Windows 95 RTS", StringComparison.OrdinalIgnoreCase))
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDANT.SYS"/"CDILLA13.DLL"/"CDILLA32.DLL"/"CDILLA64.EXE" from C-Dilla LMSversion 3.27.000 for Windows NT.
            if (name.OptionalEquals("C-Dilla Windows NT RTS", StringComparison.OrdinalIgnoreCase))
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDANTSRV.EXE" from C-Dilla LMS version 3.27.000 for Windows NT, and an embedded executable contained in Redump entry 95524.
            if (name.OptionalEquals("C-Dilla RTS Service", StringComparison.OrdinalIgnoreCase))
                return $"C-Dilla RTS Service Version {pex.ProductVersion}";

            name = pex.ProductName;

            // Found in "CDANTSRV.EXE" from version 3.27.000 of C-Dilla LMS.
            if (name.OptionalEquals("CD-Secure/CD-Compress Windows NT", StringComparison.OrdinalIgnoreCase))
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Get string table resources
            if (pex.FindStringTableByEntry("C-Dilla Licence Management System").Count > 0)
                return $"C-Dilla License Management System";
            if (pex.FindStringTableByEntry("C-DiIla Licence Management System").Count > 0)
                return $"C-Dilla License Management System";
            if (pex.FindStringTableByEntry("C-DILLA_BITMAP_NAMES_TAG").Count > 0)
                return $"C-Dilla License Management System";
            if (pex.FindStringTableByEntry("C-DILLA_EDITABLE_STRINGS_TAG").Count > 0)
                return $"C-Dilla License Management System";
            if (pex.FindStringTableByEntry("CdaLMS.exe").Count > 0)
                return $"C-Dilla License Management System";
            if (pex.FindStringTableByEntry("cdilla51.dll").Count > 0)
                return $"C-Dilla License Management System";
            if (pex.FindStringTableByEntry("cdilla52.dll").Count > 0)
                return $"C-Dilla License Management System";
            if (pex.FindStringTableByEntry("http://www.c-dilla.com/support/lms.html").Count > 0)
                return $"C-Dilla License Management System";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "DJMixStation\DJMixStation.exe" in IA item "ejay_nestle_trial".
                if (strs.Exists(s => s.Contains("SOFTWARE\\C-Dilla\\RTS")))
                    return "C-Dilla License Management System";
            }

            // Check for CDSHARE/DISAG_SH sections

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, List{string})"/>
        internal static List<string> CDillaCheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in C-Dilla CD-Secure/CD-Compress 1.31.34.
                new(new FilePathMatch("CDANT.DLL"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA05.DLL"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA10.EXE"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA40.DLL"), "C-Dilla License Management System"),

                // Found in C-Dilla LMS version 3.24.010 (IA item "ejay_nestle_trial").
                // TODO: Verify that all of these are exclusively part of LMS, and not SafeCast.
                new(new FilePathMatch("CdaLMS.exe"), "C-Dilla License Management System"),
                new(new FilePathMatch("cdilla51.dll"), "C-Dilla License Management System"),
                new(new FilePathMatch("cdilla52.dll"), "C-Dilla License Management System"),

                // Found in the installer C-Dilla LMS version 3.27.000.
                // The files "CdRemove.exe", "CdSet32.exe", "CdSet32.ini", "CdSetup.exe", "CdSetup.ini", and "CdUnin16.exe" are found there as well, but aren't currently checked for due to possibly being too generic.
                // TODO: Add grouped check for "CdRemove.exe", "CdSet32.exe", "CdSet32.ini", "CdSetup.exe", "CdSetup.ini", and "CdUnin16.exe".
                new(new FilePathMatch("CdaIns16.dll"), "C-Dilla License Management System"),
                new(new FilePathMatch("CdaIns32.dll"), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 3.1.
                // The files "CDILLA05.DLL", "CDILLA10.EXE", and "CDILLA40.DLL" are included as well.
                // TODO: Check into what file "CDAW31X.38_" gets installed as. I wasn't able to find what it gets installed to.
                new(new FilePathMatch("CDILLA32.DLL"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA64.EXE"), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 95. All the files installed for Windows 3.1 are also installed for 95.
                new(new FilePathMatch("CDAINT2F.VXD"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDAWIN95.VXD"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA13.DLL"), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows NT. All the files installed for Windows 95 and 3.1 (except for the VXD files) are also installed for NT.
                new(new FilePathMatch("CDANT.SYS"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDANTSRV.EXE"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA16.EXE"), "C-Dilla License Management System"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal static string? CDillaCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in C-Dilla CD-Secure/CD-Compress 1.31.34.
                new(new FilePathMatch("CDANT.DLL"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA05.DLL"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA10.EXE"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA40.DLL"), "C-Dilla License Management System"),

                // Found in C-Dilla LMS version 3.24.010 (IA item "ejay_nestle_trial").
                // TODO: Verify that all of these are exclusively part of LMS, and not SafeCast.
                new(new FilePathMatch("CdaLMS.exe"), "C-Dilla License Management System"),
                new(new FilePathMatch("cdilla51.dll"), "C-Dilla License Management System"),
                new(new FilePathMatch("cdilla52.dll"), "C-Dilla License Management System"),

                // Found in the installer C-Dilla LMS version 3.27.000.
                // The files "CdRemove.exe", "CdSet32.exe", "CdSet32.ini", "CdSetup.exe", "CdSetup.ini", and "CdUnin16.exe" are found there as well, but aren't currently checked for due to possibly being too generic.
                // TODO: Add grouped check for "CdRemove.exe", "CdSet32.exe", "CdSet32.ini", "CdSetup.exe", "CdSetup.ini", and "CdUnin16.exe".
                new(new FilePathMatch("CdaIns16.dll"), "C-Dilla License Management System"),
                new(new FilePathMatch("CdaIns32.dll"), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 3.1.
                // The files "CDILLA05.DLL", "CDILLA10.EXE", and "CDILLA40.DLL" are included as well.
                // TODO: Check into what file "CDAW31X.38_" gets installed as. I wasn't able to find what it gets installed to.
                new(new FilePathMatch("CDILLA32.DLL"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA64.EXE"), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 95. All the files installed for Windows 3.1 are also installed for 95.
                new(new FilePathMatch("CDAINT2F.VXD"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDAWIN95.VXD"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA13.DLL"), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows NT. All the files installed for Windows 95 and 3.1 (except for the VXD files) are also installed for NT.
                new(new FilePathMatch("CDANT.SYS"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDANTSRV.EXE"), "C-Dilla License Management System"),
                new(new FilePathMatch("CDILLA16.EXE"), "C-Dilla License Management System"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
