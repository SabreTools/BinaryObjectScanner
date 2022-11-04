﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

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
    /// SafeCast resources: 
    /// https://web.archive.org/web/20000129013431/http://www.macrovision.com/safecast_faq.html (SafeCast FAQ)
    /// https://web.archive.org/web/20031204024544mp_/http://www.macrovision.com/products/safecast/index.shtml
    /// https://web.archive.org/web/20010417222834/http://www.macrovision.com/press_rel3_17_99.html
    /// https://www.extremetech.com/computing/53394-turbotax-so-what-do-i-do-now/4
    /// https://web.archive.org/web/20031013085038/http://www.pestpatrol.com/PestInfo/c/c-dilla.asp
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc/>
        public string SafeCastCheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Get the DOS stub from the executable, if possible
            var stub = nex?.DOSStubHeader;
            if (stub == null)
                return null;

            // TODO: Implement the following NE checks:

            // File Description "CdaC01A" in "cdac01aa.dll" from IA item "ejay_nestle_trial".
            // File Description "CdaC01BA" in "cdac01ba.dll" from IA item "ejay_nestle_trial".
            // Product name "SafeCas" in "cdac01aa.dll" from IA item "ejay_nestle_trial".
            // Product name "SafeCast" in "cdac01ba.dll" from IA item "ejay_nestle_trial".

            return null;
        }

        internal string SafeCastCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // SOFTWARE\C-Dilla\SafeCast
                    // Found in "DJMixStation\DJMixStation.exe" in IA item "ejay_nestle_trial".
                    new ContentMatchSet(new byte?[] {
                        0x53, 0x4F, 0x46, 0x54, 0x57, 0x41, 0x52, 0x45, 
                        0x5C, 0x43, 0x2D, 0x44, 0x69, 0x6C, 0x6C, 0x61, 
                        0x5C, 0x53, 0x61, 0x66, 0x65, 0x43, 0x61, 0x73,
                        0x74 }, "SafeCast"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            string name = pex.FileDescription;
            if (name?.Equals("SafeCast2", StringComparison.OrdinalIgnoreCase) == true)
                return $"SafeCast";

            // Check for CDSHARE/DISAG_SH sections

            return null;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
