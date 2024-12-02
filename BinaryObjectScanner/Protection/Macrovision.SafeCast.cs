using System;
using System.Collections.Generic;
using System.Text;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// SafeCast is in the same family of protections as SafeDisc, and appears to mainly be for license management, and doesn't appear to affect the mastering of the disc in any way.
    /// Although SafeCast is most commonly used in non-game software, there is one game that comes with both SafeDisc and SafeCast protections (Redump entry 83145).
    /// SafeCast has been confirmed to be present on some programs, such as AutoDesk 3ds Max (IA items CMCD0204 and game-programming-in-c-start-to-finish-2006), Photoshop CS2 (IA item ccd0605), and Boonty Box (IA items PC_Gamer_Disc_7.55_July_2005 and cdrom-pcgamercd7.58).
    /// TODO: Check Boonty Box samples closer for new possible detections, there are at least more checks for FlexLM possible. 
    /// Macrovision bought the company C-Dilla and created SafeCast based on C-Dilla's existing products (https://web.archive.org/web/20030212040047/http://www.auditmypc.com/freescan/readingroom/cdilla.asp).
    /// There are multiple different versions of SafeCast out there.
    /// Deep dive of MechWarrior 4 and its expansions, which use SafeDisc, possibly SafeDisc LT, and SafeCast: https://digipres.club/@TheRogueArchivist/110224192068908590
    /// SafeCast ESD: https://web.archive.org/web/20000306044246/http://www.macrovision.com/safecast_ESD.html
    /// SafeCast ESD Demo: https://web.archive.org/web/20010417215236/http://www.macrovision.com:80/demos/SafeCast_ESD.exe
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
    /// http://web.archive.org/web/20010417222834/http://www.macrovision.com/press_rel3_17_99.html (Press release introducing SafeCast)
    /// https://web.archive.org/web/20000129013431/http://www.macrovision.com/safecast_faq.html (SafeCast FAQ)
    /// https://web.archive.org/web/20040223025801/http://www.macrovision.com/products/legacy_products/safecast/safecast_cdilla_faq.shtml
    /// https://web.archive.org/web/20031204024544mp_/http://www.macrovision.com/products/safecast/index.shtml
    /// https://web.archive.org/web/20010417222834/http://www.macrovision.com/press_rel3_17_99.html
    /// https://www.extremetech.com/computing/53394-turbotax-so-what-do-i-do-now/4
    /// https://web.archive.org/web/20031013085038/http://www.pestpatrol.com/PestInfo/c/c-dilla.asp
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IExecutableCheck{T}.CheckExecutable(string, T, bool)"/>
        internal static string? SafeCastCheckExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Check for the CDAC01AA name string.
            if (nex.Model.ResidentNameTable != null)
            {
                var residentNames = Array.ConvertAll(nex.Model.ResidentNameTable,
                    rnte => rnte?.NameString == null ? string.Empty : Encoding.ASCII.GetString(rnte.NameString));
                if (Array.Exists(residentNames, s => s.Contains("CDAC01AA")))
                    return "SafeCast";
            }

            // TODO: Don't read entire file
            var data = nex.ReadArbitraryRange();
            if (data == null)
                return null;

            var neMatchSets = new List<ContentMatchSet>
            {
                // SafeCast
                // Found as the Product Name in "cdac01aa.dll" from IA item "ejay_nestle_trial". Windows 10 appears to incorrectly truncate this to "SafeCas" in File Explorer.
                new(new byte?[] { 0x53, 0x61, 0x66, 0x65, 0x43, 0x61, 0x73, 0x74 }, "SafeCast"),
            };

            return MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
        }

        /// <inheritdoc cref="Interfaces.IExecutableCheck{T}.CheckExecutable(string, T, bool)"/>
        internal static string? SafeCastCheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Investigate import hint/name table entry "CdaSysInstall"
            // TODO: Investigate string table entries: "CDWP02DG", "CDWP02DG", "CDWS02DG"
            // TODO: Invesitgate if the "AdobeLM.dll" file (along with mentions of "AdobeLM" in executables) uniquely identifies SafeCast, or if it can be used with different DRM. (Found in IA item ccd0605)

            // Get the import directory table, if it exists
            if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            {
                if (Array.Exists(pex.Model.ImportTable.ImportDirectoryTable,
                    idte => idte?.Name != null && idte.Name.Equals("CdaC14BA.dll", StringComparison.OrdinalIgnoreCase)))
                {
                    return "SafeCast";
                }
            }

            // Get the dialog box resources
            // Found in "CDAC21BA.DLL" in Redump entry 95524.
            if (pex.FindDialogByTitle("SafeCast API").Count > 0)
                return "SafeCast";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "DJMixStation\DJMixStation.exe" in IA item "ejay_nestle_trial".
                if (strs.Exists(s => s.Contains("SOFTWARE\\C-Dilla\\SafeCast")))
                    return "SafeCast";
            }

            // Found in "32bit\Tax02\cdac14ba.dll" in IA item "TurboTax Deluxe Tax Year 2002 for Wndows (2.00R)(Intuit)(2002)(352282)".
            var name = pex.FileDescription;
            if (name.OptionalEquals("SafeCast2", StringComparison.OrdinalIgnoreCase))
                return "SafeCast";

            // Found in "cdac01ba.dll" from IA item "ejay_nestle_trial".
            // TODO: Figure out a reasonable way to parse version.
            if (name.OptionalEquals("CdaC01BA", StringComparison.OrdinalIgnoreCase))
                return $"SafeCast";

            // Found in "C2CDEL.EXE" in IA item "britney-spears-special-edition-cd-rom".
            if (name.OptionalEquals("32-bit SafeCast Copy To Clear Delete", StringComparison.OrdinalIgnoreCase))
                return $"SafeCast";

            // Found in "C2C.DLL" in IA item "britney-spears-special-edition-cd-rom".
            if (name.OptionalEquals("32-bit SafeCast Shell Copy To Clear DLL", StringComparison.OrdinalIgnoreCase))
                return $"SafeCast";

            // Found in "SCRfrsh.exe" in Redump entry 102979.
            if (name.OptionalEquals("32-bit SafeCast Toolkit", StringComparison.OrdinalIgnoreCase))
                return $"SafeCast {pex.FileVersion}";

            // Found in "CDAC14BA.DLL" in Redump entry 95524.
            if (name.OptionalEquals("32-bit SafeCast Anchor Installer", StringComparison.OrdinalIgnoreCase))
                return $"SafeCast";

            // Found in "CDAC21BA.DLL" in Redump entry 95524.
            if (name.OptionalEquals("32-bit CdaC20BA", StringComparison.OrdinalIgnoreCase))
                return $"SafeCast";

            // Found in hidden resource of "32bit\Tax02\cdac14ba.dll" in IA item "TurboTax Deluxe Tax Year 2002 for Wndows (2.00R)(Intuit)(2002)(352282)".
            name = pex.ProductName;
            if (name.OptionalEquals("SafeCast Windows NT", StringComparison.OrdinalIgnoreCase))
                return "SafeCast";

            // Found in "cdac01ba.dll" from IA item "ejay_nestle_trial".
            if (name.OptionalEquals("SafeCast", StringComparison.OrdinalIgnoreCase))
                return "SafeCast";

            // Check for CDSHARE/DISAG_SH sections

            return null;
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckDirectoryPath(string, List{string})"/>
        internal static List<string> SafeCastCheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "britney-spears-special-edition-cd-rom".
                new(new List<PathMatch>
                {
                    new FilePathMatch("C2C.16"),
                    new FilePathMatch("C2C.DLL"),
                    new FilePathMatch("C2CDEL.16"),
                    new FilePathMatch("C2CDEL.EXE"),
                }, "SafeCast"),

                // Found in IA item "ejay_nestle_trial".
                new(new FilePathMatch("cdac01aa.dll"), "SafeCast"),
                new(new FilePathMatch("cdac01ba.dll"), "SafeCast"),

                // Found in multiple versions of SafeCast, including Redump entries 83145 and 95524, as well as IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new(new FilePathMatch("cdac14ba.dll"), "SafeCast"),

                // Found in Redump entry 83145.
                new(new FilePathMatch("CDAC21BA.DLL"), "SafeCast"),

                // Found in Redump entry 102979.
                new(new FilePathMatch("SCRfrsh.exe"), "SafeCast"),

                // Found in Redump entries 26211 and 95524.
                new(new FilePathMatch("SCSHD.CSA"), "SafeCast"),

                // Found in Redump entries 95524.
                new(new FilePathMatch("SCSHD.EXE"), "SafeCast"),

                // Found in IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new(new FilePathMatch("CDAC15BA.SYS"), "SafeCast"),

                // Found in "cdac14ba.dll" in IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new(new FilePathMatch("CDAC13BA.EXE"), "SafeCast"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc cref="Interfaces.IPathCheck.CheckFilePath(string)"/>
        internal static string? SafeCastCheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "ejay_nestle_trial".
                new(new FilePathMatch("cdac01aa.dll"), "SafeCast"),
                new(new FilePathMatch("cdac01ba.dll"), "SafeCast"),

                new(new FilePathMatch("cdac11ba.exe"), "SafeCast"),

                // Found in multiple versions of SafeCast, including Redump entry 83145 and IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new(new FilePathMatch("cdac14ba.dll"), "SafeCast"),

                // Found in Redump entry 83145.
                new(new FilePathMatch("CDAC21BA.DLL"), "SafeCast"),

                // Found in Redump entry 102979.
                new(new FilePathMatch("SCRfrsh.exe"), "SafeCast"),
                
                // Found in Redump entries 26211 and 95524.
                new(new FilePathMatch("SCSHD.CSA"), "SafeCast"),

                // Found in Redump entries 95524.
                new(new FilePathMatch("SCSHD.EXE"), "SafeCast"),

                // Found in IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new(new FilePathMatch("CDAC15BA.SYS"), "SafeCast"),

                // Found in "cdac14ba.dll" in IA item "TurboTax_Deluxe_Tax_Year_2002_for_Wndows_2.00R_Intuit_2002_352282".
                new(new FilePathMatch("CDAC13BA.EXE"), "SafeCast"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
