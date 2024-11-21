using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to use path check framework here
    public class XCP : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section strings, if they exist
            List<string>? strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("XCP.DAT")))
                    return "XCP";

                if (strs.Exists(s => s.Contains("xcpdrive")))
                    return "XCP";

                if (strs.Exists(s => s.Contains("XCPPlugins.dll")))
                    return "XCP";

                if (strs.Exists(s => s.Contains("XCPPhoenix.dll")))
                    return "XCP";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var protections = new List<string>();
            if (files == null)
                return protections;

            // TODO: Verify if these are OR or AND
            if (files.Exists(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                || files.Exists(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)))
            {
                var versionDatPath = files.Find(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(versionDatPath))
                {
                    var xcpVersion = GetDatVersion(versionDatPath);
                    if (!string.IsNullOrEmpty(xcpVersion))
                        protections.Add(xcpVersion!);
                }
                else
                {
                    protections.Add("XCP");
                }
            }

            return protections;
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase))
            {
                return "XCP";
            }

            return null;
        }

        private static string? GetDatVersion(string path)
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
