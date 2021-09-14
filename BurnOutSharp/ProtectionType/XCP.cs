using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.FileType;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to use path check framework here
    public class XCP : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section, if it exists
            if (pex.ResourceDataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // XCP.DAT
                    new ContentMatchSet(new byte?[] { 0x58, 0x43, 0x50, 0x2E, 0x44, 0x41, 0x54 }, "XCP"),

                    // xcpdrive
                    new ContentMatchSet(new byte?[] { 0x78, 0x63,  0x70, 0x64, 0x72, 0x69, 0x76, 0x65 }, "XCP"),

                    // XCPPlugins.dll
                    new ContentMatchSet(new byte?[]
                    {
                        0x58, 0x43, 0x50, 0x50, 0x6C, 0x75, 0x67, 0x69,
                        0x6E, 0x73, 0x2E, 0x64, 0x6C, 0x6C
                    }, "XCP"),

                    // XCPPhoenix.dll
                    new ContentMatchSet(new byte?[]
                    {
                        0x58, 0x43, 0x50, 0x50, 0x68, 0x6F, 0x65, 0x6E,
                        0x69, 0x78, 0x2E, 0x64, 0x6C, 0x6C
                    }, "XCP"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.ResourceDataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var protections = new ConcurrentQueue<string>();

            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)))
            {
                string versionDatPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(versionDatPath))
                {
                    string xcpVersion = GetDatVersion(versionDatPath);
                    if (!string.IsNullOrWhiteSpace(xcpVersion))
                        protections.Enqueue(xcpVersion);
                }
                else
                {
                    protections.Enqueue("XCP");
                }
            }

            return protections;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase))
            {
                return "XCP";
            }

            return null;
        }

        private static string GetDatVersion(string path)
        {
            try
            {
                var xcpIni = new IniFile(path);
                return xcpIni["XCP.VERSION"];
            }
            catch
            {
               return null;
            }
        }
    }
}
