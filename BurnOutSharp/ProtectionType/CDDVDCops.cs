using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDDVDCops : IContentCheck, INEContentCheck, IPEContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
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
                    }, GetVersion, "CD-Cops"),

                    // // DVD-Cops,  ver. 
                    new ContentMatchSet(new byte?[]
                    {
                        0x44, 0x56, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73,
                        0x2C, 0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20
                    }, GetVersion, "DVD-Cops"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckNEContents(string file, bool includeDebug, NewExecutable nex)
        {
            // Get the DOS stub from the executable, if possible
            var stub = nex?.DOSStubHeader;
            if (stub == null)
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
                }, GetVersion, "CD-Cops"),
            };
            
            return MatchUtil.GetFirstMatch(file, nex.SourceArray, neMatchSets, includeDebug);
        }

        /// <inheritdoc/>
        public string CheckPEContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .grand section, if it exists -- TODO: Confirm is this is in DVD-Cops as well
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
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CDCOPS.DLL", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".GZ_", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".W_X", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".Qz", useEndsWith: true), "CD-Cops"),
                new PathMatchSet(new PathMatch(".QZ_", useEndsWith: true), "CD-Cops"),
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
