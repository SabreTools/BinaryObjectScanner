using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.FileType;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class XCP : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // XCP.DAT
                new Matcher(new byte?[] { 0x58, 0x43, 0x50, 0x2E, 0x44, 0x41, 0x54 }, "XCP"),

                // XCPPlugins.dll
                new Matcher(new byte?[]
                {
                    0x58, 0x43, 0x50, 0x50, 0x6C, 0x75, 0x67, 0x69,
                    0x6E, 0x73, 0x2E, 0x64, 0x6C, 0x6C
                }, "XCP"),

                // XCPPhoenix.dll
                new Matcher(new byte?[]
                {
                    0x58, 0x43, 0x50, 0x50, 0x68, 0x6F, 0x65, 0x6E,
                    0x69, 0x78, 0x2E, 0x64, 0x6C, 0x6C
                }, "XCP"),
            };

            return Utilities.GetContentMatches(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("go.exe", StringComparison.OrdinalIgnoreCase))) // Path.Combine("contents", "go.exe")
            {
                string versionDatPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(versionDatPath))
                {
                    string xcpVersion = GetDatVersion(versionDatPath);
                    if (!string.IsNullOrWhiteSpace(xcpVersion))
                        return xcpVersion;
                }

                return "XCP";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("go.exe", StringComparison.OrdinalIgnoreCase))
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
