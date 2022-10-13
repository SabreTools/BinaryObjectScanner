﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
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
            // Get the DOS stub from the executable, if possible
            var stub = nex?.DOSStubHeader;
            if (stub == null)
                return null;

            // TODO: Don't read entire file
            var data = nex.ReadArbitraryRange();
            if (data == null)
                return null;

            // TODO: Do something with these strings in the NE header(?)
            // - CDCOPS
            // - CDcops assembly-language DLL

            // TODO: Figure out what NE section this lives in
            var neMatchSets = new List<ContentMatchSet>
            {
                // CD-Cops,  ver. 
                new ContentMatchSet(new byte?[]
                {
                    0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C,
                    0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                }, GetVersion, "CD-Cops (Unconfirmed - Please report to us on Github)"),
            };
            
            return MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .grand section, if it exists -- TODO: Confirm is this is in DVD-Cops as well
            // Found in "AGENTHUG.QZ_" in Redump entry 84517
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
                // Found in Redump entry 84517
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),

                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 84517
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),

                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops (Unconfirmed - Please report to us on Github)"),
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
