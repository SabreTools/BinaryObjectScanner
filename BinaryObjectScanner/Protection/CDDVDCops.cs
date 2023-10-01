using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// TODO: Investigate "Cops Copylock II" (https://www.cbmstuff.com/forum/showthread.php?tid=488).
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
    /// Demo that may contain WEB-Cops: https://web.archive.org/web/20040602210926/http://games.tucows.com/preview/266462.html
    /// 
    /// Known samples of DVD-Cops include:
    /// * IA item "flaklypa-grand-prix-dvd"/Redump entry 108169.
    /// 
    /// A sample of CD-Cops that makes use of encrypted PDFs (LDSCRYPT) can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
    /// 
    /// List of applications that have CD/DVD/WEB-Cops relating to a Windows update: https://www.betaarchive.com/wiki/index.php/Microsoft_KB_Archive/924867
    /// </summary>

    public class CDDVDCops : IContentCheck, INewExecutableCheck, IPathCheck, IPortableExecutableCheck
    {
        // TODO: Investigate reference to "CD32COPS.DLL" in "WETFLIPP.QZ_" in IA item "Triada_Russian_DVD_Complete_Collection_of_Erotic_Games".
        /// <inheritdoc/>
#if NET48
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
#else
        public string? CheckContents(string file, byte[] fileContent, bool includeDebug)
#endif
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // TODO: Remove from here once it's confirmed that no PE executables contain this string
                    // CD-Cops,  ver. 
                    new ContentMatchSet(new byte?[]
                    {
                        0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                        0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                    }, GetVersion, "CD-Cops (Unconfirmed - Please report to us on Github)"),

                    // // DVD-Cops,  ver. 
                    new ContentMatchSet(new byte?[]
                    {
                        0x44, 0x56, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73,
                        0x2C, 0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                    }, GetVersion, "DVD-Cops (Unconfirmed - Please report to us on Github)"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
#else
        public string? CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
#endif
        {
            // TODO: Don't read entire file
            var data = nex.ReadArbitraryRange();
            if (data == null)
                return null;

            // TODO: Figure out what NE section this lives in
            var neMatchSets = new List<ContentMatchSet>
            {
                // CD-Cops,  ver. 
                // Found in "h3blade.exe" in Redump entry 85077.
                new ContentMatchSet(new byte?[]
                {
                    0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                    0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "CD-Cops"),
            };

            var match = MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
            if (!string.IsNullOrEmpty(match))
                return match;

            // Check the imported-name table
            // Found in "h3blade.exe" in Redump entry 85077.
            bool importedNameTableEntries = nex.Model.ImportedNameTable?
                .Select(kvp => kvp.Value)
                .Select(inte => inte?.NameString == null ? string.Empty : Encoding.ASCII.GetString(inte.NameString))
                .Any(s => s.Contains("CDCOPS")) ?? false;
            if (importedNameTableEntries)
                return "CD-Cops";

            // Check the nonresident-name table
            // Found in "CDCOPS.DLL" in Redump entry 85077.
            bool nonresidentNameTableEntries = nex.Model.NonResidentNameTable?
                .Select(nrnte => nrnte?.NameString == null ? string.Empty : Encoding.ASCII.GetString(nrnte.NameString))
                .Any(s => s.Contains("CDcops assembly-language DLL")) ?? false;
            if (nonresidentNameTableEntries)
                return "CD-Cops";

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .grand section, if it exists
            // Found in "AGENTHUG.QZ_" in Redump entry 84517 and "h3blade.QZ_" in Redump entry 85077.
            bool grandSection = pex.ContainsSection(".grand", exact: true);
            if (grandSection)
                return "CD/DVD-Cops";

            // Get the UNICops section, if it exists
            // Found in "FGP.exe" in IA item "flaklypa-grand-prix-dvd"/Redump entry 108169.
            bool UNICopsSection = pex.ContainsSection("UNICops", exact: true);
            if (UNICopsSection)
                return "CD/DVD-Cops Obfuscated Executable";

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            // TODO: Original had "CDCOPS.DLL" required and all the rest in a combined OR
            var matchers = new List<PathMatchSet>
            {
                // A 400+ MB file called "WASTE.DAT" that is fully 00 padded can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
                // Presumably used to increase the amount of data written to the disc to allow DPM checking to be used for the protection. It's unknown if this file is used on any other protected discs.

                // Found in Redump entry 84517.
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", matchExact: true, useEndsWith: true), "CD/DVD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", matchExact: true, useEndsWith: true), "CD/DVD-Cops"),

                new PathMatchSet(new PathMatch(".GZ_", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".Qz", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files ?? System.Array.Empty<string>(), matchers, any: true);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // A 400+ MB file called "WASTE.DAT" that is fully 00 padded can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
                // Presumably used to increase the amount of data written to the disc to allow DPM checking to be used for the protection. It's unknown if this file is used on any other protected discs.

                // Found in Redump entry 84517.
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", matchExact: true, useEndsWith: true), "CD/DVD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", matchExact: true, useEndsWith: true), "CD/DVD-Cops"),

                new PathMatchSet(new PathMatch(".GZ_", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".Qz", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

#if NET48
        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
#else
        public static string? GetVersion(string file, byte[]? fileContent, List<int> positions)
#endif
        {
            // If we have no content
            if (fileContent == null)
                return null;

            char[] version = new ArraySegment<byte>(fileContent, positions[0] + 15, 4).Select(b => (char)b).ToArray();
            if (version[0] == 0x00)
                return string.Empty;

            return new string(version);
        }
    }
}
