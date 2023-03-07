using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, which should be possible with LibMSPackN, but it refuses to extract due to SFX files lacking the typical CAB identifiers.
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class MicrosoftCABSFX : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.InternalName;
            if (name?.Equals("Wextract", StringComparison.OrdinalIgnoreCase) == true)
                return $"Microsoft CAB SFX {GetVersion(pex)}";

            name = pex.OriginalFilename;
            if (name?.Equals("WEXTRACT.EXE", StringComparison.OrdinalIgnoreCase) == true)
                return $"Microsoft CAB SFX {GetVersion(pex)}";

            // Get the .data/DATA section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("wextract_cleanup")))
                    return $"Microsoft CAB SFX {GetVersion(pex)}";
            }

            // Get the .text section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                // This detects a different but similar type of SFX that uses Microsoft CAB files.
                // Further research is needed to see if it's just a different version or entirely separate.
                if (strs.Any(s => s.Contains("MSCFu")))
                    return $"Microsoft CAB SFX {GetVersion(pex)}";
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }
    
        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            string version = Tools.Utilities.GetInternalVersion(pex);
            if (!string.IsNullOrWhiteSpace(version))
                return $"v{version}";

            return string.Empty;
        }
    }
}
