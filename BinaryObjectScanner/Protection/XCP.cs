using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to use path check framework here
    public class XCP : IPathCheck, IPortableExecutableCheck
    {
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

            // Get the .rdata section strings, if they exist
#if NET48
            var strs = pex.GetFirstSectionStrings(".rdata");
#else
            List<string>? strs = pex.GetFirstSectionStrings(".rdata");
#endif
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("XCP.DAT")))
                    return "XCP";

                if (strs.Any(s => s.Contains("xcpdrive")))
                    return "XCP";

                if (strs.Any(s => s.Contains("XCPPlugins.dll")))
                    return "XCP";

                if (strs.Any(s => s.Contains("XCPPhoenix.dll")))
                    return "XCP";
            }

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var protections = new ConcurrentQueue<string>();

            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)))
            {
                var versionDatPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(versionDatPath))
                {
                    var xcpVersion = GetDatVersion(versionDatPath);
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
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            if (Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase))
            {
                return "XCP";
            }

            return null;
        }

#if NET48
        private static string GetDatVersion(string path)
#else
        private static string? GetDatVersion(string path)
#endif
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
