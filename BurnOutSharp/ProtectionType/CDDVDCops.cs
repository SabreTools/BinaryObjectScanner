using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Investigate "Cops Copylock II" (https://www.cbmstuff.com/forum/showthread.php?tid=488).
    // `AgentHugo.exe`
    //      Embedded PE executable in one of the NE sections
    // `AgentHugo.exe` / `NE.EXE` (1.46) / `NETINST.EXE` (1.48) / `NETINST.QZ_`
    //      Embedded PKZIP archive that may contain the CD-Cops files
    // `CDCOPS.DLL` (1.46) / `CDCOPS.DLL` (1.48)
    //      `WINCOPS.INI`
    // Samples of CD-Cops can be found in IA items "der-brockhaus-multimedial-2002-premium" and "der-brockhaus-multimedial-2003-premium".
    // A sample of CD-Cops that makes use of encrypted PDFs (LDSCRYPT) can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
    public class CDDVDCops : IContentCheck, INewExecutableCheck, IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
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
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Check we have a valid executable
            if (nex == null)
                return null;

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

            string match = MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
            if (!string.IsNullOrEmpty(match))
                return match;

            // Check the imported-name table
            // Found in "h3blade.exe" in Redump entry 85077.
            bool importedNameTableEntries = nex.ImportedNameTable?
                .Select(kvp => kvp.Value)
                .Where(inte => inte.NameString != null)
                .Select(inte => Encoding.ASCII.GetString(inte.NameString))
                .Any(s => s.Contains("CDCOPS")) ?? false;
            if (importedNameTableEntries)
                return "CD-Cops";

            // Check the nonresident-name table
            // Found in "CDCOPS.DLL" in Redump entry 85077.
            bool nonresidentNameTableEntries = nex.NonResidentNameTable?
                .Where(nrnte => nrnte.NameString != null)
                .Select(nrnte => Encoding.ASCII.GetString(nrnte.NameString))
                .Any(s => s.Contains("CDcops assembly-language DLL")) ?? false;
            if (nonresidentNameTableEntries)
                return "CD-Cops";

            return null;
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .grand section, if it exists -- TODO: Confirm is this is in DVD-Cops as well
            // Found in "AGENTHUG.QZ_" in Redump entry 84517 and "h3blade.QZ_" in Redump entry 85077.
            bool grandSection = pex.ContainsSection(".grand", exact: true);
            if (grandSection)
                return "CD-Cops";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Original had "CDCOPS.DLL" required and all the rest in a combined OR
            var matchers = new List<PathMatchSet>
            {
                // A 400+ MB file called "WASTE.DAT" that is fully 00 padded can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
                // Presumably used to increase the amount of data written to the disc to allow DPM checking to be used for the protection. It's unknown if this file is used on any other protected discs.

                // Found in Redump entry 84517
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", matchExact: true, useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", matchExact: true, useEndsWith: true), "CD-Cops"),

                new PathMatchSet(new PathMatch(".GZ_", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".Qz", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // A 400+ MB file called "WASTE.DAT" that is fully 00 padded can be found in IA item "Faculty_Edition_People_Problems_and_Power_by_Joseph_Unekis_Textbytes".
                // Presumably used to increase the amount of data written to the disc to allow DPM checking to be used for the protection. It's unknown if this file is used on any other protected discs.

                // Found in Redump entry 84517
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", matchExact: true, useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", matchExact: true, useEndsWith: true), "CD-Cops"),

                new PathMatchSet(new PathMatch(".GZ_", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".Qz", matchExact: true, useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            char[] version = new ArraySegment<byte>(fileContent, positions[0] + 15, 4).Select(b => (char)b).ToArray();
            if (version[0] == 0x00)
                return string.Empty;

            return new string(version);
        }
    }
}
