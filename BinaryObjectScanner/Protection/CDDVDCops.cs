using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// TODO: Investigate "Cops Copylock II" (https://www.cbmstuff.com/forum/showthread.php?tid=488).
    /// TODO: Investigate additional products mentioned on the Link Data Security website (https://www.linkdatasecurity.com/index.htm#/protection-products/overview).
    /// `AgentHugo.exe`
    ///      Embedded PE executable in one of the NE sections
    /// `AgentHugo.exe` / `NE.EXE` (1.46) / `NETINST.EXE` (1.48) / `NETINST.QZ_`
    ///      Embedded PKZIP archive that may contain the CD-Cops files
    /// `CDCOPS.DLL` (1.46) / `CDCOPS.DLL` (1.48)
    ///      `WINCOPS.INI`
    ///
    /// TODO: Investigate if "DVD-Cops" is a separate product, or simply what CD-Cops is referred to when used on a DVD.
    ///
    /// Known versions of CD-Cops:
    /// * 1.08 (Unconfirmed) (Redump entry 84517).
    /// * 1,13[sic] (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168 patched with https://web.archive.org/web/20040307124358/http://www.caprino.no:80/download/fgpgold_upd4.exe).
    /// * 1.21 (Unconfirmed) (Redump entry 91713).
    /// * 1,22[sic] (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168 patched with https://web.archive.org/web/20030430194917/http://www.caprino.no:80/download/fgpgold_upd2.exe).
    /// * 1,28[sic] (Confirmed) ("RunMenu.exe" in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes").
    /// * 1,31[sic] (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168).
    /// * 1.31 (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168 patched with Patch 11).
    /// * 1.46 (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168 patched with https://web.archive.org/web/20210103064517/http://www.caprino.no/download/FGPGOLD_UPD12.exe)
    /// * 1,63[sic] (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168 patched with https://web.archive.org/web/20060926082522/http://www.caprino.no:80/download/fgpgold_upd7.exe).
    /// * 1.72 (Confirmed) ("h3blade.exe" in Redump entry 85077).
    /// * 1.73 (Confirmed) ("WETFLIPPER.EXE" in IA item "LULA_Erotic_Pinball_-_Windows95_Eng).
    /// * 1,81[sic] (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168 patched with https://web.archive.org/web/20030308040529/http://www.caprino.no:80/download/fgpgold_upd1.exe).
    /// * 2.03 (Confirmed) ("HyperBowl.exe" in IA item "hyperbowl_20190626").
    ///
    /// Known versions of DVD-Cops:
    /// * 1.69 (Confirmed) ("FGP.exe" in IA item "flaklypa-grand-prix-dvd"/Redump entry 108169).
    ///
    /// Known samples of CD-Cops include:
    /// * IA item "der-brockhaus-multimedial-2002-premium".
    /// * IA item "der-brockhaus-multimedial-2003-premium".
    /// * IA item "SCIENCESENCYCLOPEDIAV2.0ARISSCD1".
    /// * IA item "SCIENCESENCYCLOPEDIAV2.0ARISSCD2".
    /// * IA item "Triada_Russian_DVD_Complete_Collection_of_Erotic_Games".
    /// * IA item "LULA_Erotic_Pinball_-_Windows95_Eng".
    /// * IA item "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168.
    /// * Patches for "flaklypa-grand-prix-gullutgave-2cd"/Redump entries 108167-108168, found at https://web.archive.org/web/*/http://www.caprino.no/download/* (FGPGOLD_UPD files).
    /// * IA item "hyperbowl_20190626"/"hyperbowl-arcade-edition".
    /// * Redump entries 51403(?), 84517, and 85077.
    ///
    /// Known samples of DVD-Cops include:
    /// * IA item "flaklypa-grand-prix-dvd"/Redump entry 108169.
    ///
    /// Known samples of WEB-Cops include:
    /// * https://web.archive.org/web/20120616074941/http://icm.games.tucows.com/files2/HyperDemo-109a.exe
    ///
    /// A sample of CD-Cops that makes use of encrypted PDFs (LDSCRYPT) can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
    ///
    /// List of applications that have CD/DVD/WEB-Cops relating to a Windows update: https://www.betaarchive.com/wiki/index.php/Microsoft_KB_Archive/924867
    /// </summary>
    // TODO: Investigate reference to "CD32COPS.DLL" in "WETFLIPP.QZ_" in IA item "Triada_Russian_DVD_Complete_Collection_of_Erotic_Games".
    // TODO: Investigate cdcode.key for redump ID 108167, may be key-less cd-cops?
    // TODO: Document update 12 for redump ID 108167 bumping version, adding key, adding vista(?) support

    public class CDDVDCops : IExecutableCheck<NewExecutable>, IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, NewExecutable exe, bool includeDebug)
        {
            // TODO: Don't read entire file
#pragma warning disable CS0618
            byte[]? data = exe.ReadArbitraryRange();
            if (data is null)
                return null;

            // TODO: Figure out what NE section this lives in
            var neMatchSets = new List<ContentMatchSet>
            {
                // Checking for variants with one or two spaces, just in case; the Brockhaus DVDs only had one
                new(new byte?[]
                {
                    0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                    0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "CD-Cops"),
                // CD-Cops, ver.

                // Found in "h3blade.exe" in Redump entry 85077.
                new(new byte?[]
                {
                    0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                    0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "CD-Cops"),
                // CD-Cops,  ver.

                // Found in IA entries "der-brockhaus-multimedial-2002-premium" and "der-brockhaus-multimedial-2003-premium"
                // TODO: 2002 returns DVD-Cops 2.01, 2003 returns DVD-Cops 1,60. CD-Cops version numbers seem to "reset"
                // after some point in time in existing redump entries- perhaps the command instead of the period may have
                // some significance?
                new(new byte?[]
                {
                    0x44, 0x56, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73,
                    0x2C, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "DVD-Cops"),
                // DVD-Cops, ver.

                new(new byte?[]
                {
                    0x44, 0x56, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73,
                    0x2C, 0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "DVD-Cops"),
                // DVD-Cops,  ver.
            };

            var match = MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
            if (!string.IsNullOrEmpty(match))
                return match;

            // Get the resident and non-resident name table strings
            var nrntStrs = Array.ConvertAll(exe.NonResidentNameTable ?? [],
                nrnte => nrnte?.NameString is null ? string.Empty : Encoding.ASCII.GetString(nrnte.NameString));

            // Check the imported-name table
            // Found in "h3blade.exe" in Redump entry 85077.
            if (exe.ImportedNameTable is not null)
            {
                foreach (var inte in exe.ImportedNameTable.Values)
                {
                    if (inte.NameString.IsNullOrEmpty())
                        continue;

                    string ns = Encoding.ASCII.GetString(inte.NameString!);
                    if (ns.Contains("CDCOPS"))
                        return "CD-Cops";
                }
            }

            // Check the nonresident-name table
            // Found in "CDCOPS.DLL" in Redump entry 85077.
            if (Array.Exists(nrntStrs, s => s.Contains("CDcops assembly-language DLL")))
                return "CD-Cops";

            return null;
        }

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the stub executable data, if it exists
            if (exe.StubExecutableData is not null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // WEBCOPS
                    // Found in "HyperBowl.C_S" in https://web.archive.org/web/20120616074941/http://icm.games.tucows.com/files2/HyperDemo-109a.exe.
                    new(new byte?[]
                    {
                        0x57, 0x45, 0x42, 0x43, 0x4F, 0x50, 0x53
                    }, "WEB-Cops")
                };

                var match = MatchUtil.GetFirstMatch(file, exe.StubExecutableData, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
            }

            // Get the .grand section, if it exists
            // Found in "AGENTHUG.QZ_" in Redump entry 84517 and "h3blade.QZ_" in Redump entry 85077.
            if (exe.ContainsSection(".grand", exact: true))
                return "CD/DVD/WEB-Cops";

            // Get the UNICops section, if it exists
            // Found in "FGP.exe" in IA item "flaklypa-grand-prix-dvd"/Redump entry 108169.
            if (exe.ContainsSection("UNICops", exact: true))
                return "UNI-Cops";

            // Get the DATA section, if it exists
            // Found in "bib.dll" in IA item "https://archive.org/details/cover_202501"
            // This contains the version section that the Content Check looked for. There are likely other sections
            // that may contain it. Update when more are found.
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs is not null)
            {
                var match = strs.Find(s => s.Contains(" ver. ") && (s.Contains("CD-Cops, ") || s.Contains("DVD-Cops, ")));
                if (match is not null)
                    if (match.Contains("CD-Cops"))
                        return $"CD-Cops {GetVersionString(match)}";
                    else if (match.Contains("DVD-Cops"))
                        return $"DVD-Cops {GetVersionString(match)}";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            // TODO: Original had "CDCOPS.DLL" required and all the rest in a combined OR
            var matchers = new List<PathMatchSet>
            {
                // A 400+ MB file called "WASTE.DAT" that is fully 00 padded can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
                // Presumably used to increase the amount of data written to the disc to allow DPM checking to be used for the protection. It's unknown if this file is used on any other protected discs.

                // Found in Redump entry 84517.
                new(new FilePathMatch("CDCOPS.DLL"), "CD-Cops"),
                new(new PathMatch(".W_X", matchCase: true, useEndsWith: true), "CD/DVD-Cops"),
                new(new PathMatch(".QZ_", matchCase: true, useEndsWith: true), "CD/DVD-Cops"),

                new(new PathMatch(".GZ_", matchCase: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new(new PathMatch(".Qz", matchCase: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),

                // Found in Redump entries 84517, 108167, 119435, 119436, and 119437. This is the official
                // name from their website https://www.linkdatasecurity.com/index.htm#/protection-products/cd-dvd-usb-copy-protection/cdcops
                // I can't find this specific filename documented anywhere, but, all of these
                // games do not require a key to be input
                new(new FilePathMatch("cdcode.key"), "CD-Cops Codefree"),

                // DVD-Cops Codefree does exist https://www.linkdatasecurity.com/index.htm#/protection-products/cd-dvd-usb-copy-protection/dvdvers
                // but we currently have no samples. Presumably this is what the file would be called?
                new(new FilePathMatch("dvdcode.key"), "DVD-Cops Codefree (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // A 400+ MB file called "WASTE.DAT" that is fully 00 padded can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
                // Presumably used to increase the amount of data written to the disc to allow DPM checking to be used for the protection. It's unknown if this file is used on any other protected discs.

                // Found in Redump entry 84517.
                new(new FilePathMatch("CDCOPS.DLL"), "CD-Cops"),
                new(new PathMatch(".W_X", matchCase: true, useEndsWith: true), "CD/DVD-Cops"),
                new(new PathMatch(".QZ_", matchCase: true, useEndsWith: true), "CD/DVD-Cops"),

                new(new PathMatch(".GZ_", matchCase: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new(new PathMatch(".Qz", matchCase: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                // Found in Redump entries 84517, 108167, 119435, 119436, and 119437. This is the official
                // name from their website https://www.linkdatasecurity.com/index.htm#/protection-products/cd-dvd-usb-copy-protection/cdcops
                // I can't find this specific filename documented anywhere, but, all of these
                // games do not require a key to be input
                new(new FilePathMatch("cdcode.key"), "CD-Cops Codefree"),

                // DVD-Cops Codefree does exist https://www.linkdatasecurity.com/index.htm#/protection-products/cd-dvd-usb-copy-protection/dvdvers
                // but we currently have no samples. Presumably this is what the file would be called?
                new(new FilePathMatch("dvdcode.key"), "DVD-Cops Codefree (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private static string? GetVersion(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent is null)
                return null;

            string version = Encoding.ASCII.GetString(fileContent, positions[0] + 15, 4);
            if (version[0] == 0x00)
                return string.Empty;

            return version;
        }

        private static string GetVersionString(string match)
        {
            // Full string ends with # (i.e. "CD-Cops,  ver. 1.72,  #"), use that to compensate for comma in version
            // number cases (don't change the comma, see earlier to-do) like "DVD-Cops, ver. 1,60,  #"
            // TODO: improve regex via the starting "N" character? Possibly unnecessary?
            var versionMatch = Regex.Match(match, @"(?<=D-Cops,\s{1,}ver. )(.*?)(?=,\s{1,}#)", RegexOptions.Compiled);
            if (versionMatch.Success)
                return versionMatch.Value;

            return "(Unknown Version - Please report to us on GitHub)";
        }
    }
}
