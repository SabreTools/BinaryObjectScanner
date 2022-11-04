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
    /// 1.31.34 (https://archive.org/details/PCDDec1995).
    /// 3.23.000 (https://archive.org/details/3ds-max-4.2original).
    /// 3.24.010 (https://archive.org/details/ejay_nestle_trial).
    /// 3.27.000 (https://download.autodesk.com/mne/web/support/3dstudio/C-Dilla3.27.zip).
    /// 
    /// TODO:
    /// Investigate C-Dilla CD-Compress.
    /// Find 2.X versions of CD-Secure/LMS.
    /// </summary>
    public partial class Macrovision
    {
        internal string CDillaCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        { 
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;

            // Found in "CdaIns16.dll" and "CdSetup.exe" from version 3.27.000 of C-Dilla LMS.
            if (name?.Equals("C-Dilla Windows 16-Bit RTS Installer", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CdaIns32.dll" and "CdSet32.exe" from version 3.27.000 of C-Dilla LMS.
            if (name?.Equals("C-Dilla Windows 32-Bit RTS Installer", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CdUnin16.exe" from version 3.27.000 of C-Dilla LMS.
            if (name?.Equals("C-Dilla LMS Uninstaller", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDILLA05.DLL"/"CDILLA10.EXE"/"CDILLA32.DLL"/"CDILLA64.EXE" from C-Dilla LMS version 3.27.000 for Windows 3.1.
            if (name?.Equals("C-Dilla Windows 3.1x RTS", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";
            
            // TODO: Implement check properly once NE checks are supported.
            // Found in "CDILLA40.DLL" from C-Dilla LMS version 3.27.000 for Windows 3.1/95/NT.
            // This file specifically is known to stop at version 3.15.000, even when included in newer versions of C-Dilla LMS.
            //    if (name?.Equals("C-Dilla 16-bit DLL", StringComparison.OrdinalIgnoreCase) == true)
            //        return $"C-Dilla License Management System Version {pex.ProductVersion}"; 

            // Found in "CDILLA05.DLL"/"CDILLA10.EXE"/"CDILLA13.DLL"/"CDILLA32.DLL"/"CDILLA64.EXE" from C-Dilla LMS version 3.27.000 for Windows 95.
            if (name?.Equals("C-Dilla Windows 95 RTS", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDANT.SYS"/"CDILLA05.DLL"/"CDILLA10.EXE"/"CDILLA13.DLL"/"CDILLA16.EXE"/"CDILLA32.DLL"/"CDILLA64.EXE" from C-Dilla LMSversion 3.27.000 for Windows NT.
            if (name?.Equals("C-Dilla Windows NT RTS", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDANTSRV.EXE" from C-Dilla LMS version 3.27.000 for Windows NT.
            if (name?.Equals("C-Dilla RTS Service", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            name = pex.ProductName;

            // Found in "CdUnin16.exe" from version 3.27.000 of C-Dilla LMS.
            if (name?.Equals("C-Dilla License Management System", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Found in "CDANTSRV.EXE" from version 3.27.000 of C-Dilla LMS.
            if (name?.Equals("CD-Secure/CD-Compress Windows NT", StringComparison.OrdinalIgnoreCase) == true)
                return $"C-Dilla License Management System Version {pex.ProductVersion}";

            // Check for CDSHARE/DISAG_SH sections

            return null;
        }

        /// <inheritdoc/>
        internal ConcurrentQueue<string> CDillaCheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the installer C-Dilla LMS version 3.27.000.
                // The files "CdRemove.exe", "CdSet32.exe", "CdSet32.ini", "CdSetup.exe", "CdSetup.ini", and "CdUnin16.exe" are found there as well, but aren't currently checked for due to possibly being too generic.
                // TODO: Add grouped check for "CdRemove.exe", "CdSet32.exe", "CdSet32.ini", "CdSetup.exe", "CdSetup.ini", and "CdUnin16.exe".
                new PathMatchSet(new PathMatch("CdaIns16.dll", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CdaIns32.dll", useEndsWith: true), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 3.1.
                // TODO: Check into what file "CDAW31X.38_" gets installed as. I wasn't able to find what it gets installed to.
                new PathMatchSet(new PathMatch("CDILLA05.DLL", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA10.EXE", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA32.DLL", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA40.DLL", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA64.EXE", useEndsWith: true), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 95. All the files installed for Windows 3.1 are also installed for 95.
                new PathMatchSet(new PathMatch("CDAINT2F.VXD", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDAWIN95.VXD", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA13.DLL", useEndsWith: true), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows NT. All the files installed for Windows 95 and 3.1 (except for the VXD files) are also installed for NT.
                new PathMatchSet(new PathMatch("CDANT.SYS", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDANTSRV.EXE", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA16.EXE", useEndsWith: true), "C-Dilla License Management System"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        internal string CDillaCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the installer C-Dilla LMS version 3.27.000.
                // The files "CdRemove.exe", "CdSet32.exe", "CdSet32.ini", "CdSetup.exe", "CdSetup.ini", and "CdUnin16.exe" are found there as well, but aren't currently checked for due to possibly being too generic.
                new PathMatchSet(new PathMatch("CdaIns16.dll", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CdaIns32.dll", useEndsWith: true), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 3.1.
                // TODO: Check into what file "CDAW31X.38_" gets installed as. I wasn't able to find what it gets installed to.
                new PathMatchSet(new PathMatch("CDILLA05.DLL", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA10.EXE", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA32.DLL", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA40.DLL", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA64.EXE", useEndsWith: true), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows 95. All the files installed for Windows 3.1 are also installed for 95.
                new PathMatchSet(new PathMatch("CDAINT2F.VXD", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDAWIN95.VXD", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA13.DLL", useEndsWith: true), "C-Dilla License Management System"),

                // Found installed in C-Dilla LMS version 3.27.000 for Windows NT. All the files installed for Windows 95 and 3.1 (except for the VXD files) are also installed for NT.
                new PathMatchSet(new PathMatch("CDANT.SYS", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDANTSRV.EXE", useEndsWith: true), "C-Dilla License Management System"),
                new PathMatchSet(new PathMatch("CDILLA16.EXE", useEndsWith: true), "C-Dilla License Management System"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
