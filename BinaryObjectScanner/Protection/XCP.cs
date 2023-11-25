﻿using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
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
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section strings, if they exist
            List<string>? strs = pex.GetFirstSectionStrings(".rdata");
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
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif
            if (files == null)
                return protections;

            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)))
            {
                var versionDatPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(versionDatPath))
                {
                    var xcpVersion = GetDatVersion(versionDatPath);
                    if (!string.IsNullOrEmpty(xcpVersion))
                        protections.Enqueue(xcpVersion!);
                }
                else
                {
                    protections.Enqueue("XCP");
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
