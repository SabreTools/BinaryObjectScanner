using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class StarForce : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.LegalCopyright;
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("Protection Technology")) // Protection Technology (StarForce)?
                return $"StarForce {Utilities.GetInternalVersion(pex)}";

            name = pex.InternalName;
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("CORE.EXE", StringComparison.Ordinal))
                return $"StarForce {Utilities.GetInternalVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.Equals("protect.exe", StringComparison.Ordinal))
                return $"StarForce {Utilities.GetInternalVersion(pex)}";

            // TODO: Find what fvinfo field actually maps to this
            name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("Protected Module"))
                return $"StarForce 5";

            // TODO: Check to see if there are any missing checks
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/StarForce.2.sg

            // TODO: Find this inside of the .rsrc section using the executable header
            // Get the .rsrc section, if it exists
            var rsrcSection = pex.GetLastSection(".rsrc", exact: true);
            if (rsrcSection != null)
            {
                var rsrcSectionData = pex.ReadRawSection(".rsrc", first: true);
                if (rsrcSectionData != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + e + (char)0x00 + d + (char)0x00 +   + (char)0x00 + M + (char)0x00 + o + (char)0x00 + d + (char)0x00 + u + (char)0x00 + l + (char)0x00 + e + (char)0x00
                        new ContentMatchSet(
                            new byte?[]
                            {
                                0x50, 0x00, 0x72, 0x00, 0x6f, 0x00, 0x74, 0x00,
                                0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x65, 0x00,
                                0x64, 0x00, 0x20, 0x00, 0x4d, 0x00, 0x6f, 0x00,
                                0x64, 0x00, 0x75, 0x00, 0x6c, 0x00, 0x65, 0x00
                            },
                            "StarForce 5 [Protected Module]"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, rsrcSectionData, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
            }

            // Get the .brick section, if it exists
            bool brickSection = pex.ContainsSection(".brick", exact: true);
            if (brickSection)
                return "StarForce 3-5";

            // Get the .sforce* section, if it exists
            bool sforceSection = pex.ContainsSection(".sforce", exact: false);
            if (sforceSection)
                return "StarForce 3-5";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // These have too high of a chance of over-matching by themselves
            // var matchers = new List<PathMatchSet>
            // {
            //     // TODO: Re-consolidate these once path matching is improved
            //     new PathMatchSet(new PathMatch("/protect.dll", useEndsWith: true), "StarForce"),
            //     new PathMatchSet(new PathMatch("/protect.exe", useEndsWith: true), "StarForce"),
            // };

            // return MatchUtil.GetAllMatches(files, matchers, any: false);
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // These have too high of a chance of over-matching by themselves
            // var matchers = new List<PathMatchSet>
            // {
            //     // TODO: Re-consolidate these once path matching is improved
            //     new PathMatchSet(new PathMatch("/protect.dll", useEndsWith: true), "StarForce"),
            //     new PathMatchSet(new PathMatch("/protect.exe", useEndsWith: true), "StarForce"),
            // };

            // return MatchUtil.GetFirstMatch(path, matchers, any: true);
            return null;
        }

        // This section contains extraneous checks in the .rsrc section that have not been confirmed
        // new ContentMatchSet(new List<ContentMatch>
        // {
        //     // P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + T + (char)0x00 + e + (char)0x00 + c + (char)0x00 + h + (char)0x00 + n + (char)0x00 + o + (char)0x00 + l + (char)0x00 + o + (char)0x00 + g + (char)0x00 + y + (char)0x00
        //     new ContentMatch(new byte?[]
        //     {
        //         0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x74, 0x00,
        //         0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00,
        //         0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x54, 0x00,
        //         0x65, 0x00, 0x63, 0x00, 0x68, 0x00, 0x6E, 0x00,
        //         0x6F, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x67, 0x00,
        //         0x79, 0x00
        //     }, start: sectionAddr, end: sectionEnd),

        //     // // PSA_GetDiscLabel
        //     // new ContentMatch(new byte?[]
        //     // {
        //     //     0x50, 0x53, 0x41, 0x5F, 0x47, 0x65, 0x74, 0x44,
        //     //     0x69, 0x73, 0x63, 0x4C, 0x61, 0x62, 0x65, 0x6C
        //     // }, start: sectionAddr, end: sectionEnd),

        //     // (c) Protection Technology
        //     // new ContentMatch(new byte?[]
        //     // {
        //     //     0x28, 0x63, 0x29, 0x20, 0x50, 0x72, 0x6F, 0x74,
        //     //     0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54,
        //     //     0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67,
        //     //     0x79
        //     // }, start: sectionAddr, end: sectionEnd),

        //     // TradeName
        //     new ContentMatch(new byte?[] { 0x54, 0x72, 0x61, 0x64, 0x65, 0x4E, 0x61, 0x6D, 0x65 }, start: sectionAddr, end: sectionEnd),
        // }, GetVersion, "StarForce"),
        // public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        // {
        //     return $"{Utilities.GetInternalVersion(fileContent)} ({fileContent.Skip(positions[1] + 22).TakeWhile(c => c != 0x00)})";
        // }
    }
}
