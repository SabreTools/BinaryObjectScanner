using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// SafeCast is in the same family of protections as SafeDisc, and appears to mainly be for license management, and doesn't appear to affect the mastering of the disc in any way.
    /// Although SafeCast is most commonly used in non-game software, there is one game that comes with both SafeDisc and SafeCast protections (Redump entry 83145).
    /// Macrovision bought the company C-Dilla and created SafeCast based on C-Dilla's existing products (https://web.archive.org/web/20030212040047/http://www.auditmypc.com/freescan/readingroom/cdilla.asp).
    /// There are multiple different versions of SafeCast out there.
    /// SafeCast ESD: https://web.archive.org/web/20000306044246/http://www.macrovision.com/safecast_ESD.html
    /// SafeCast Gold: https://web.archive.org/web/20000129071444/http://www.macrovision.com/scp_gold.html
    /// SafeCast LM: https://web.archive.org/web/20000128224337/http://www.macrovision.com/safecast_LM.html
    /// SafeCast UNSORTED samples:
    /// https://www.virustotal.com/gui/file/220b8eec64d43fd854a48f28efa915c158a06f3675cbf0ef52796750984d9e1c/details
    /// https://groups.google.com/g/alt.english.usage/c/kcBzeqXgE-M
    /// https://www.shouldiremoveit.com/aerosofts-FDC-Live-Cockpit-64651-program.aspx
    /// https://archive.org/details/danceejay4
    /// https://archive.org/details/ejay_nestle_trial
    /// https://archive.org/details/eJayXtremeSoundtraxx
    /// https://community.ptc.com/t5/Mathcad/SafeCast/td-p/25233
    /// SafeCast resources: 
    /// https://web.archive.org/web/20000129013431/http://www.macrovision.com/safecast_faq.html (SafeCast FAQ)
    /// https://web.archive.org/web/20040223025801/http://www.macrovision.com/products/legacy_products/safecast/safecast_cdilla_faq.shtml
    /// https://web.archive.org/web/20031204024544mp_/http://www.macrovision.com/products/safecast/index.shtml
    /// https://web.archive.org/web/20010417222834/http://www.macrovision.com/press_rel3_17_99.html
    /// https://www.extremetech.com/computing/53394-turbotax-so-what-do-i-do-now/4
    /// https://web.archive.org/web/20031013085038/http://www.pestpatrol.com/PestInfo/c/c-dilla.asp
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.INewExecutableCheck.CheckNewExecutable(string, NewExecutable, bool)"/>
        internal string SafeCastCheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Check we have a valid executable
            if (nex == null)
                return null;

            // TODO: Implement the following NE checks:

            // File Description "CdaC01A" in "cdac01aa.dll" from IA item "ejay_nestle_trial".
            // File Description "CdaC01BA" in "cdac01ba.dll" from IA item "ejay_nestle_trial".
            // Product name "SafeCas" in "cdac01aa.dll" from IA item "ejay_nestle_trial".
            // Product name "SafeCast" in "cdac01ba.dll" from IA item "ejay_nestle_trial".

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
        internal string SafeCastCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // TODO: Investigate import hint/name table entry "CdaSysInstall"
            // TODO: Investigate string table entries: "CDWP02DG", "CDWP02DG", "CDWS02DG"

            // Get the import directory table, if it exists
            if (pex.ImportTable?.ImportDirectoryTable != null)
            {
                if (pex.ImportTable.ImportDirectoryTable.Any(idte => idte.Name?.Equals("CdaC14BA.dll", StringComparison.OrdinalIgnoreCase) == true))
                    return "SafeCast";
            }

            // Get the dialog box resources
            var resource = pex.FindDialogByTitle("SafeCast API");
            if (resource.Any())
                return "SafeCast";

            // Get the .data/DATA section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "DJMixStation\DJMixStation.exe" in IA item "ejay_nestle_trial".
                if (strs.Any(s => s.Contains("SOFTWARE\\C-Dilla\\SafeCast")))
                    return "SafeCast";
            }

            // Found in "32bit\Tax02\cdac14ba.dll" in IA item "TurboTax Deluxe Tax Year 2002 for Wndows (2.00R)(Intuit)(2002)(352282)".
            string name = pex.FileDescription;
            if (name?.Equals("SafeCast2", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeCast";

            // Found in hidden resource of "32bit\Tax02\cdac14ba.dll" in IA item "TurboTax Deluxe Tax Year 2002 for Wndows (2.00R)(Intuit)(2002)(352282)".
            // TODO: Fix File Description not getting properly pulled for this executable.
            // Known versions:
            // 4.16.050 Windows NT 2002/04/24
            if (name?.Equals("Macrovision RTS Service", StringComparison.OrdinalIgnoreCase) == true)
                return $"Macrovision RTS Service " + pex.FileVersion;

            // Found in hidden resource of "32bit\Tax02\cdac14ba.dll" in IA item "TurboTax Deluxe Tax Year 2002 for Wndows (2.00R)(Intuit)(2002)(352282)".
            // TODO: Fix Product Name not getting properly pulled for this executable.
            name = pex.ProductName;
            if (name?.Equals("SafeCast Windows NT", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeCast";

            // Check for CDSHARE/DISAG_SH sections

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, IEnumerable{string})"/>
        internal ConcurrentQueue<string> SafeCastCheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "ejay_nestle_trial".
                new PathMatchSet(new PathMatch("cdac01aa.dll", useEndsWith: true), "SafeCast"),
                new PathMatchSet(new PathMatch("cdac01ba.dll", useEndsWith: true), "SafeCast"),

                // Found in multiple versions of SafeCast, including Redump entry 83145 and IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new PathMatchSet(new PathMatch("cdac14ba.dll", useEndsWith: true), "SafeCast"),

                // Found in Redump entry 83145.
                new PathMatchSet(new PathMatch("CDAC21BA.DLL", useEndsWith: true), "SafeCast"),

                // Shown in multiple sources (such as https://groups.google.com/g/alt.english.usage/c/kcBzeqXgE-M) to be associated with SafeCast, but no samples have been found as of yet.
                new PathMatchSet(new PathMatch("SCRfrsh.exe", useEndsWith: true), "SafeCast (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal string SafeCastCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "ejay_nestle_trial".
                new PathMatchSet(new PathMatch("cdac01aa.dll", useEndsWith: true), "SafeCast"),
                new PathMatchSet(new PathMatch("cdac01ba.dll", useEndsWith: true), "SafeCast"),

                new PathMatchSet(new PathMatch("cdac11ba.exe", useEndsWith: true), "SafeCast"),

                // Found in multiple versions of SafeCast, including Redump entry 83145 and IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new PathMatchSet(new PathMatch("cdac14ba.dll", useEndsWith: true), "SafeCast"),

                // Found in Redump entry 83145.
                new PathMatchSet(new PathMatch("CDAC21BA.DLL", useEndsWith: true), "SafeCast"),

                // Shown in multiple sources (such as https://groups.google.com/g/alt.english.usage/c/kcBzeqXgE-M) to be associated with SafeCast, but no samples have been found as of yet.
                new PathMatchSet(new PathMatch("SCRfrsh.exe", useEndsWith: true), "SafeCast (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
